using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms;

namespace BlazorWebFormsComponents.Cli.Io;

/// <summary>
/// Copies non-page .cs files (Models, Logic, Services, etc.) to the output directory.
/// Applies code-behind transforms (using strip, EF namespace) to the copied files.
/// Preserves relative directory structure from the source project.
/// </summary>
public class SourceFileCopier
{
    private static readonly HashSet<string> ExcludedDirs = new(StringComparer.OrdinalIgnoreCase)
    {
        "bin", "obj", "packages", "node_modules", ".vs", ".git",
        "Properties", "App_Start", "App_Data"
    };

    private static readonly HashSet<string> QuarantinedFileNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "AssemblyInfo.cs", "Global.asax.cs", "Startup.cs",
        "Startup.Auth.cs", "BundleConfig.cs", "FilterConfig.cs",
        "RouteConfig.cs", "WebApiConfig.cs", "IdentityConfig.cs"
    };

    private static readonly (Regex Pattern, string Reason)[] QuarantineRules =
    [
        (new Regex(@"\b(Microsoft\.Owin|Owin|IAppBuilder|GetOwinContext|DefaultAuthenticationTypes)\b", RegexOptions.Compiled), "OWIN/bootstrap wiring does not belong in the generated Blazor SSR compile surface."),
        (new Regex(@"\b(ApplicationUserManager|ApplicationSignInManager|IdentityFactoryOptions|IIdentityMessageService|IdentityDbContext)\b", RegexOptions.Compiled), "ASP.NET Identity bootstrap code needs app-specific migration and should not be compiled as-is."),
        (new Regex(@"\b(HttpContext\.Current|System\.Web\.HttpContext)\b", RegexOptions.Compiled), "Legacy HttpContext access indicates runtime assumptions that are unsafe in the generated SSR compile surface."),
        (new Regex(@"\b(DropCreateDatabaseAlways|DropCreateDatabaseIfModelChanges|CreateDatabaseIfNotExists|DbMigrationsConfiguration|DbMigration)\b", RegexOptions.Compiled), "Legacy EF initializer or migrations bootstrap should be reviewed manually instead of compiled directly.")
    ];

    // Matches constructors with DI-injected parameters (non-empty parameter lists)
    private static readonly Regex InjectedConstructorRegex = new(
        @"public\s+(?<name>[A-Za-z_]\w*)\s*\((?<params>[^)]+)\)",
        RegexOptions.Compiled);

    private static readonly Regex NamespaceRegex = new(
        @"namespace\s+(?<ns>[A-Za-z_][\w.]*)\s*(?:\{|;)",
        RegexOptions.Compiled);

    private static readonly Regex ClassRegex = new(
        @"(?:public|internal)\s+(?:(?:partial|static|sealed|abstract)\s+)*class\s+(?<name>[A-Za-z_]\w*)(?:\s*<[^>]+>)?(?:\s*:\s*[^{]+)?\s*\{",
        RegexOptions.Compiled);

    private static readonly Regex ConstructorRegex = new(
        @"public\s+(?<name>[A-Za-z_]\w*)\s*\((?<params>[^)]*)\)",
        RegexOptions.Compiled);

    private static readonly Regex PublicMethodRegex = new(
        @"public\s+(?:(?:static|virtual|override|async)\s+)*(?<return>[A-Za-z_][\w<>,\s\?\[\]]*)\s+(?<name>[A-Za-z_]\w*)\s*\((?<params>[^)]*)\)",
        RegexOptions.Compiled);

    private readonly OutputWriter _outputWriter;
    private readonly IReadOnlyList<ICodeBehindTransform> _transforms;

    public SourceFileCopier(OutputWriter outputWriter, IEnumerable<ICodeBehindTransform> transforms)
    {
        _outputWriter = outputWriter;
        // Only apply a subset of transforms relevant to non-page .cs files
        _transforms = transforms
            .Where(t => t.Name is "UsingStrip" or "IdentityUsing" or "HttpUtilityRewrite" or "EntityFramework" or "EfContextConstructor" or "DbContextInstantiation" or "HttpContextAccessor" or "SelectMethodMaterialize" or "LegacyHelperStub" or "TypeMismatchFix" or "EagerLoadNavigation" or "DisposeReadonlyField" or "NamespaceAlign")
            .OrderBy(t => t.Order)
            .ToList();
    }

    /// <summary>
    /// Scan for non-page .cs files and copy them with namespace transforms applied.
    /// Returns a result containing copy counts and discovered service classes that need DI registration.
    /// </summary>
    public async Task<SourceFileCopyResult> CopySourceFilesAsync(
        string sourcePath,
        string outputPath,
        IReadOnlyList<SourceFile> pageFiles,
        bool verbose,
        MigrationReport report,
        ISet<string>? additionalExcludedFiles = null,
        ISet<string>? scaffoldedClassNames = null,
        string? projectNamespace = null)
    {
        if (!Directory.Exists(sourcePath))
            return new SourceFileCopyResult(0, 0, []);

        var normalizedSourcePath = Path.GetFullPath(sourcePath);
        var normalizedOutputPath = Path.GetFullPath(outputPath);
        var normalizedExcludedFiles = additionalExcludedFiles is null
            ? null
            : new HashSet<string>(additionalExcludedFiles.Select(Path.GetFullPath), StringComparer.OrdinalIgnoreCase);

        // Build set of code-behind paths to skip (already handled by page pipeline)
        var codeBehindPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pf in pageFiles)
        {
            if (pf.CodeBehindPath != null)
                codeBehindPaths.Add(Path.GetFullPath(pf.CodeBehindPath));
        }

        var copiedCount = 0;
        var quarantinedCount = 0;
        var discoveredServiceClasses = new List<DiscoveredServiceClass>();

        // Track fully-qualified class names to detect duplicates.
        // Web Forms projects sometimes have the same class in multiple files
        // (e.g., a BLL class and a generated model stub) which causes CS0101.
        var copiedClassNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in Directory.EnumerateFiles(sourcePath, "*.cs", SearchOption.AllDirectories))
        {
            var fullPath = Path.GetFullPath(file);

            // Skip code-behind files (already processed)
            if (codeBehindPaths.Contains(fullPath))
                continue;

            if (normalizedExcludedFiles is not null && normalizedExcludedFiles.Contains(fullPath))
                continue;

            var relativePath = GetRelativePathPreservingSourceCasing(normalizedSourcePath, fullPath);
            var fileName = Path.GetFileName(file);

            // Skip excluded directories
            var topDir = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)[0];
            if (ExcludedDirs.Contains(topDir))
                continue;

            // Skip designer files
            if (fileName.EndsWith(".designer.cs", StringComparison.OrdinalIgnoreCase))
                continue;

            var content = await File.ReadAllTextAsync(file);

            // Apply transforms first — transforms like HttpContextAccessor may fix
            // quarantine-triggering patterns, allowing the file to pass classification
            var metadata = new FileMetadata
            {
                SourceFilePath = file,
                SourceRootPath = normalizedSourcePath,
                OutputFilePath = Path.Combine(normalizedOutputPath, relativePath),
                FileType = FileType.CodeFile, // Standalone .cs — not a page/control/master code-behind
                OriginalContent = content,
                OutputRootPath = normalizedOutputPath,
                ProjectNamespace = projectNamespace
            };

            foreach (var transform in _transforms)
            {
                var transformed = transform.Apply(content, metadata);
                if (!string.Equals(transformed, content, StringComparison.Ordinal))
                    report.TransformsApplied++;
                content = transformed;
            }

            var decision = Classify(relativePath, fileName, topDir, content);
            if (decision.ShouldQuarantine)
            {
                var artifactPath = Path.Combine(normalizedOutputPath, "migration-artifacts", "compile-surface", relativePath + ".txt");
                var quarantinedContent = BuildQuarantineArtifact(relativePath, decision.Reason!, content);
                await _outputWriter.WriteFileAsync(artifactPath, quarantinedContent, $"Compile-surface artifact: {relativePath}");
                report.AddManualItem(relativePath, 0, "bwfc-compile-surface", decision.Reason!);

                // Generate a compile-safe stub at the original location so dependent files still build
                // But skip if the class is already provided by scaffolding (e.g., identity migration)
                var stub = BuildCompileStub(content, relativePath, decision.Reason!, scaffoldedClassNames);
                if (stub != null)
                {
                    var stubPath = Path.Combine(normalizedOutputPath, relativePath);
                    await _outputWriter.WriteFileAsync(stubPath, stub, $"Compile stub: {relativePath}");
                }

                quarantinedCount++;
                continue;
            }

            // Detect duplicate classes: if a class with the same fully-qualified name
            // was already copied from another file, skip this one to avoid CS0101.
            var fileNamespace = NamespaceRegex.Match(content);
            var fileClasses = ClassRegex.Matches(content);
            var isDuplicate = false;
            if (fileNamespace.Success && fileClasses.Count > 0)
            {
                var ns = fileNamespace.Groups["ns"].Value;
                foreach (Match cm in fileClasses)
                {
                    var fqn = $"{ns}.{cm.Groups["name"].Value}";
                    if (!copiedClassNames.Add(fqn))
                    {
                        isDuplicate = true;
                        if (verbose)
                            Console.WriteLine($"  Skipped duplicate class: {fqn} in {relativePath}");
                        break;
                    }
                }
            }
            if (isDuplicate)
                continue;

            var destPath = Path.Combine(normalizedOutputPath, relativePath);
            await _outputWriter.WriteFileAsync(destPath, content, $"Source: {relativePath}");
            copiedCount++;

            // Detect service classes for DI registration. BLL/Logic classes are always
            // registered, even when they still use parameterless construction.
            var serviceClass = DetectServiceClass(content, relativePath);
            if (serviceClass != null)
                discoveredServiceClasses.Add(serviceClass);
        }

        if (verbose || copiedCount > 0)
            Console.WriteLine($"  Source files copied:  {copiedCount} (with namespace transforms)");
        if (verbose || quarantinedCount > 0)
            Console.WriteLine($"  Compile-surface artifacts quarantined: {quarantinedCount}");
        if (verbose && discoveredServiceClasses.Count > 0)
            Console.WriteLine($"  Service classes discovered for DI: {discoveredServiceClasses.Count}");

        return new SourceFileCopyResult(copiedCount, quarantinedCount, discoveredServiceClasses);
    }

    private static string GetRelativePathPreservingSourceCasing(string normalizedSourcePath, string fullPath)
    {
        var root = normalizedSourcePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var prefix = root + Path.DirectorySeparatorChar;

        if (fullPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return fullPath[prefix.Length..];

        return Path.GetRelativePath(root, fullPath);
    }

    private static CompileSurfaceDecision Classify(string relativePath, string fileName, string topDir, string content)
    {
        if (QuarantinedFileNames.Contains(fileName))
        {
            return new CompileSurfaceDecision(true, $"Legacy bootstrap file '{fileName}' was quarantined from the generated compile surface.");
        }

        if (topDir.Equals("Migrations", StringComparison.OrdinalIgnoreCase))
        {
            return new CompileSurfaceDecision(true, $"Legacy migrations source '{relativePath}' was quarantined from the generated compile surface.");
        }

        foreach (var (pattern, reason) in QuarantineRules)
        {
            if (pattern.IsMatch(content))
            {
                return new CompileSurfaceDecision(true, reason);
            }
        }

        return new CompileSurfaceDecision(false, null);
    }

    private static string BuildQuarantineArtifact(string relativePath, string reason, string content)
    {
        return
            $"// TODO: Review — '{relativePath}' was quarantined from the generated Blazor SSR compile surface.{Environment.NewLine}" +
            $"// TODO: Reason — {reason}{Environment.NewLine}" +
            $"// TODO: Move or rewrite only the relevant pieces into Program.cs, services, middleware, or application code.{Environment.NewLine}{Environment.NewLine}" +
            content;
    }

    /// <summary>
    /// Generates a minimal compile-safe stub class so dependent files still build.
    /// Extracts namespace, class name, constructors, and public method signatures
    /// from the original source and generates empty bodies.
    /// Returns null if the class is already provided by scaffolding.
    /// </summary>
    private static string? BuildCompileStub(string originalContent, string relativePath, string reason, ISet<string>? scaffoldedClassNames = null)
    {
        var nsMatch = NamespaceRegex.Match(originalContent);
        var classMatch = ClassRegex.Match(originalContent);

        if (!classMatch.Success)
            return null;

        var className = classMatch.Groups["name"].Value;

        // Skip stub if this class is already generated by scaffolding (e.g., ApplicationUser from identity migration)
        if (scaffoldedClassNames != null && scaffoldedClassNames.Contains(className))
            return null;

        var namespaceName = nsMatch.Success ? nsMatch.Groups["ns"].Value : null;

        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"// Compile-safe stub for quarantined file '{relativePath}'");
        sb.AppendLine($"// Reason: {reason}");
        sb.AppendLine($"// Full original source preserved in migration-artifacts/compile-surface/{relativePath}.txt");
        sb.AppendLine();

        // Only include safe framework usings — skip legacy usings that caused quarantine
        var safeUsings = new HashSet<string>(StringComparer.Ordinal);
        foreach (var line in originalContent.Split('\n'))
        {
            var trimmed = line.Trim().TrimEnd('\r');
            if (trimmed.StartsWith("using ", StringComparison.Ordinal) && trimmed.EndsWith(";"))
            {
                // Skip legacy namespaces that don't exist in modern .NET
                if (trimmed.Contains("Microsoft.Owin") || trimmed.Contains("Owin;") ||
                    trimmed.Contains("System.Web") || trimmed.Contains("System.Data.Entity") ||
                    trimmed.Contains("Microsoft.AspNet.Identity") || trimmed.Contains("Microsoft.Owin"))
                    continue;
                safeUsings.Add(trimmed);
            }
            else if (!string.IsNullOrWhiteSpace(trimmed) && !trimmed.StartsWith("//"))
                break;
        }
        // Always include System
        safeUsings.Add("using System;");
        foreach (var u in safeUsings.OrderBy(x => x))
            sb.AppendLine(u);
        sb.AppendLine();

        if (namespaceName != null)
        {
            sb.AppendLine($"namespace {namespaceName};");
            sb.AppendLine();
        }

        sb.AppendLine($"/// <summary>");
        sb.AppendLine($"/// Stub for quarantined class. Provides compile compatibility for dependent code.");
        sb.AppendLine($"/// Replace with a proper implementation during Layer 2 migration.");
        sb.AppendLine($"/// </summary>");
        sb.AppendLine($"public class {className}");
        sb.AppendLine("{");

        // Generate parameterless constructor
        sb.AppendLine($"    public {className}() {{ }}");

        // Generate public method stubs with default return values (skip params to avoid type issues)
        foreach (Match methodMatch in PublicMethodRegex.Matches(originalContent))
        {
            var methodName = methodMatch.Groups["name"].Value;
            if (methodName == className) continue; // Skip constructors
            var returnType = methodMatch.Groups["return"].Value.Trim();

            var defaultReturn = returnType switch
            {
                "void" => "",
                "string" => " return string.Empty;",
                "bool" => " return false;",
                "int" => " return 0;",
                "double" or "decimal" or "float" => " return 0;",
                "Task" => " return Task.CompletedTask;",
                _ => " throw new NotImplementedException();"
            };

            sb.AppendLine($"    public {returnType} {methodName}() {{{defaultReturn} }}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <summary>
    /// Detects whether a transformed source file should be registered for DI in Program.cs.
    /// BLL/Logic classes are always registered; other classes must have constructor injection.
    /// </summary>
    private static DiscoveredServiceClass? DetectServiceClass(string content, string relativePath)
    {
        var classMatch = ClassRegex.Match(content);
        if (!classMatch.Success)
            return null;

        var className = classMatch.Groups["name"].Value;

        // Skip DbContext subclasses — they're registered separately by ProgramCsEmitter
        if (Regex.IsMatch(content, @"class\s+\w+\s*:\s*(?:Db|Identity).*Context\b"))
            return null;

        // Skip static classes
        if (Regex.IsMatch(content, @"\bstatic\s+class\b"))
            return null;

        var pathSegments = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var isLogicService = pathSegments.Any(segment =>
            string.Equals(segment, "BLL", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(segment, "Logic", StringComparison.OrdinalIgnoreCase));

        if (!isLogicService)
        {
            // Outside BLL/Logic folders, only register classes that received constructor injection.
            var ctorMatch = InjectedConstructorRegex.Match(content);
            if (!ctorMatch.Success || ctorMatch.Groups["name"].Value != className)
                return null;
        }

        var nsMatch = NamespaceRegex.Match(content);
        var namespaceName = nsMatch.Success ? nsMatch.Groups["ns"].Value : null;

        return new DiscoveredServiceClass(className, namespaceName);
    }

    private sealed record CompileSurfaceDecision(bool ShouldQuarantine, string? Reason);
}

/// <summary>
/// Result of source file copy operation, including discovered service classes that need DI registration.
/// </summary>
public sealed record SourceFileCopyResult(
    int CopiedCount,
    int QuarantinedCount,
    IReadOnlyList<DiscoveredServiceClass> DiscoveredServiceClasses)
{
    public int TotalCount => CopiedCount + QuarantinedCount;
}

/// <summary>
/// A non-page class that was discovered to have constructor injection and needs DI registration.
/// </summary>
public sealed record DiscoveredServiceClass(string ClassName, string? Namespace);

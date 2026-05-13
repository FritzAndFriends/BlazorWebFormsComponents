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
            .Where(t => t.Name is "UsingStrip" or "IdentityUsing" or "HttpUtilityRewrite" or "EntityFramework" or "EfContextConstructor" or "DbContextInstantiation" or "HttpContextAccessor" or "SelectMethodMaterialize" or "LegacyHelperStub")
            .OrderBy(t => t.Order)
            .ToList();
    }

    /// <summary>
    /// Scan for non-page .cs files and copy them with namespace transforms applied.
    /// </summary>
    public async Task<int> CopySourceFilesAsync(
        string sourcePath,
        string outputPath,
        IReadOnlyList<SourceFile> pageFiles,
        bool verbose,
        MigrationReport report,
        ISet<string>? additionalExcludedFiles = null,
        ISet<string>? scaffoldedClassNames = null)
    {
        if (!Directory.Exists(sourcePath))
            return 0;

        // Build set of code-behind paths to skip (already handled by page pipeline)
        var codeBehindPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pf in pageFiles)
        {
            if (pf.CodeBehindPath != null)
                codeBehindPaths.Add(Path.GetFullPath(pf.CodeBehindPath));
        }

        var copiedCount = 0;
        var quarantinedCount = 0;

        foreach (var file in Directory.EnumerateFiles(sourcePath, "*.cs", SearchOption.AllDirectories))
        {
            var fullPath = Path.GetFullPath(file);

            // Skip code-behind files (already processed)
            if (codeBehindPaths.Contains(fullPath))
                continue;

            if (additionalExcludedFiles is not null && additionalExcludedFiles.Contains(fullPath))
                continue;

            var relativePath = Path.GetRelativePath(sourcePath, file);
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
                OutputFilePath = Path.Combine(outputPath, relativePath),
                FileType = FileType.Page, // Doesn't matter for UsingStrip/EF transforms
                OriginalContent = content
            };

            foreach (var transform in _transforms)
            {
                content = transform.Apply(content, metadata);
            }

            var decision = Classify(relativePath, fileName, topDir, content);
            if (decision.ShouldQuarantine)
            {
                var artifactPath = Path.Combine(outputPath, "migration-artifacts", "compile-surface", relativePath + ".txt");
                var quarantinedContent = BuildQuarantineArtifact(relativePath, decision.Reason!, content);
                await _outputWriter.WriteFileAsync(artifactPath, quarantinedContent, $"Compile-surface artifact: {relativePath}");
                report.AddManualItem(relativePath, 0, "bwfc-compile-surface", decision.Reason!);

                // Generate a compile-safe stub at the original location so dependent files still build
                // But skip if the class is already provided by scaffolding (e.g., identity migration)
                var stub = BuildCompileStub(content, relativePath, decision.Reason!, scaffoldedClassNames);
                if (stub != null)
                {
                    var stubPath = Path.Combine(outputPath, relativePath);
                    await _outputWriter.WriteFileAsync(stubPath, stub, $"Compile stub: {relativePath}");
                }

                quarantinedCount++;
                continue;
            }

            var destPath = Path.Combine(outputPath, relativePath);
            await _outputWriter.WriteFileAsync(destPath, content, $"Source: {relativePath}");
            copiedCount++;
        }

        if (verbose || copiedCount > 0)
            Console.WriteLine($"  Source files copied:  {copiedCount} (with namespace transforms)");
        if (verbose || quarantinedCount > 0)
            Console.WriteLine($"  Compile-surface artifacts quarantined: {quarantinedCount}");

        return copiedCount + quarantinedCount;
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

    private sealed record CompileSurfaceDecision(bool ShouldQuarantine, string? Reason);
}

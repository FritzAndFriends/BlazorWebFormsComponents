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

    private readonly OutputWriter _outputWriter;
    private readonly IReadOnlyList<ICodeBehindTransform> _transforms;

    public SourceFileCopier(OutputWriter outputWriter, IEnumerable<ICodeBehindTransform> transforms)
    {
        _outputWriter = outputWriter;
        // Only apply a subset of transforms relevant to non-page .cs files
        _transforms = transforms
            .Where(t => t.Name is "UsingStrip" or "EntityFramework" or "IdentityUsing")
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
        ISet<string>? additionalExcludedFiles = null)
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

            var decision = Classify(relativePath, fileName, topDir, content);
            if (decision.ShouldQuarantine)
            {
                var artifactPath = Path.Combine(outputPath, "migration-artifacts", "compile-surface", relativePath + ".txt");
                var quarantinedContent = BuildQuarantineArtifact(relativePath, decision.Reason!, content);
                await _outputWriter.WriteFileAsync(artifactPath, quarantinedContent, $"Compile-surface artifact: {relativePath}");
                report.AddManualItem(relativePath, 0, "bwfc-compile-surface", decision.Reason!);
                quarantinedCount++;
                continue;
            }

            // Read and apply transforms
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

    private sealed record CompileSurfaceDecision(bool ShouldQuarantine, string? Reason);
}

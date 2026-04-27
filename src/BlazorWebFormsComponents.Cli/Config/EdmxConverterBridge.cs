using BlazorWebFormsComponents.Cli.Interop;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Config;

/// <summary>
/// Invokes the existing EDMX-to-EF Core converter through the CLI pipeline until the converter is fully ported to C#.
/// </summary>
public class EdmxConverterBridge
{
    private readonly PowerShellScriptRunner _scriptRunner;

    public EdmxConverterBridge(PowerShellScriptRunner scriptRunner)
    {
        _scriptRunner = scriptRunner;
    }

    public async Task<HashSet<string>> ConvertAsync(string sourcePath, string outputPath, string projectName, bool dryRun, MigrationReport report)
    {
        var excludedSourceFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var edmxFiles = Directory.EnumerateFiles(sourcePath, "*.edmx", SearchOption.AllDirectories)
            .Where(file => Path.GetDirectoryName(file)?.EndsWith("Models", StringComparison.OrdinalIgnoreCase) == true)
            .ToList();

        if (edmxFiles.Count == 0)
            return excludedSourceFiles;

        if (dryRun)
        {
            report.Warnings.Add("EDMX conversion is skipped in --dry-run mode.");
            return excludedSourceFiles;
        }

        var scriptPath = RepoPathResolver.FindFileFromRepoRoot(Path.Combine("migration-toolkit", "scripts", "Convert-EdmxToEfCore.ps1"));

        foreach (var edmxFile in edmxFiles)
        {
            var relativeDir = Path.GetDirectoryName(Path.GetRelativePath(sourcePath, edmxFile)) ?? string.Empty;
            var outputDir = Path.Combine(outputPath, relativeDir);
            Directory.CreateDirectory(outputDir);

            var args = new List<string>
            {
                "-EdmxPath", edmxFile,
                "-OutputPath", outputDir,
                "-Namespace", $"{projectName}.Models"
            };

            var result = await _scriptRunner.RunAsync(scriptPath, args);
            if (result.ExitCode != 0)
            {
                report.Warnings.Add($"EDMX conversion failed for {Path.GetFileName(edmxFile)}: {result.StandardError.Trim()}");
                continue;
            }

            var stem = Path.GetFileNameWithoutExtension(edmxFile);
            excludedSourceFiles.Add(Path.Combine(Path.GetDirectoryName(edmxFile)!, $"{stem}.cs"));
            report.AddManualItem(Path.GetRelativePath(sourcePath, edmxFile), 0, "EDMX", "EDMX converted to EF Core entities and DbContext — verify generated relationships and configuration.");
        }

        return excludedSourceFiles;
    }
}

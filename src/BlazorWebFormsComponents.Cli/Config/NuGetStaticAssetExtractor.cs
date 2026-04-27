using System.Text.Json;
using BlazorWebFormsComponents.Cli.Interop;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Config;

/// <summary>
/// Invokes the legacy NuGet static asset extractor through the CLI pipeline until the extractor is fully ported to C#.
/// </summary>
public class NuGetStaticAssetExtractor
{
    private readonly PowerShellScriptRunner _scriptRunner;

    public NuGetStaticAssetExtractor(PowerShellScriptRunner scriptRunner)
    {
        _scriptRunner = scriptRunner;
    }

    public async Task<NuGetAssetExtractionResult?> ExtractAsync(string sourcePath, string outputPath, bool dryRun, MigrationReport report)
    {
        var packagesConfig = Path.Combine(sourcePath, "packages.config");
        if (!File.Exists(packagesConfig))
            return null;

        if (dryRun)
        {
            report.Warnings.Add("NuGet static asset extraction is skipped in --dry-run mode.");
            return null;
        }

        var scriptPath = RepoPathResolver.FindFileFromRepoRoot(Path.Combine("migration-toolkit", "scripts", "Migrate-NugetStaticAssets.ps1"));
        var args = new List<string>
        {
            "-SourcePath", sourcePath,
            "-OutputPath", outputPath
        };

        var result = await _scriptRunner.RunAsync(scriptPath, args);
        if (result.ExitCode != 0)
        {
            report.Warnings.Add($"NuGet static asset extraction failed: {result.StandardError.Trim()}");
            return null;
        }

        var manifestPath = Path.Combine(outputPath, "asset-manifest.json");
        if (!File.Exists(manifestPath))
            return null;

        await using var stream = File.OpenRead(manifestPath);
        var manifest = await JsonSerializer.DeserializeAsync<NuGetAssetManifest>(stream, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return manifest is null
            ? null
            : new NuGetAssetExtractionResult(manifest.PackagesWithAssets, manifest.TotalFilesExtracted);
    }

    private sealed class NuGetAssetManifest
    {
        public int PackagesWithAssets { get; set; }
        public int TotalFilesExtracted { get; set; }
    }
}

public sealed record NuGetAssetExtractionResult(int PackagesWithAssets, int TotalFilesExtracted);

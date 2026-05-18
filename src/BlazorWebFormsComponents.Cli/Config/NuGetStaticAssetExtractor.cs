using BlazorWebFormsComponents.Cli.Pipeline;
using NativeNuGetStaticAssetExtractor = BlazorWebFormsComponents.Cli.Services.NuGetStaticAssetExtractor;
using NativeNuGetAssetExtractionResult = BlazorWebFormsComponents.Cli.Services.NuGetAssetExtractionResult;

namespace BlazorWebFormsComponents.Cli.Config;

[Obsolete("Use BlazorWebFormsComponents.Cli.Services.NuGetStaticAssetExtractor instead.")]
public class NuGetStaticAssetExtractor
{
    private readonly NativeNuGetStaticAssetExtractor _inner;

    public NuGetStaticAssetExtractor(NativeNuGetStaticAssetExtractor inner)
    {
        _inner = inner;
    }

    public Task<NativeNuGetAssetExtractionResult?> ExtractAsync(string sourcePath, string outputPath, bool dryRun, MigrationReport report)
    {
        return _inner.ExtractAsync(sourcePath, outputPath, dryRun, report);
    }
}

using System.Text.Json;
using BlazorWebFormsComponents.Cli.Services;

namespace BlazorWebFormsComponents.Cli.Tests.Services;

public sealed class NuGetStaticAssetExtractorTests : IDisposable
{
    private readonly string _testRoot = Path.Combine(AppContext.BaseDirectory, "TestOutput", nameof(NuGetStaticAssetExtractorTests), Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_testRoot))
            Directory.Delete(_testRoot, recursive: true);
    }

    [Fact]
    public async Task ExtractAsync_PackagesDirectory_CopiesAssetsAndWritesManifest()
    {
        var sourcePath = CreateDirectory("basic", "source");
        var outputPath = CreateDirectory("basic", "output");
        var packagesPath = Path.Combine(sourcePath, "packages");
        Directory.CreateDirectory(packagesPath);

        await File.WriteAllTextAsync(Path.Combine(sourcePath, "packages.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <packages>
              <package id="bootstrap" version="5.3.3" />
              <package id="jQuery" version="3.7.1" />
            </packages>
            """);

        await WriteFileAsync(Path.Combine(packagesPath, "bootstrap.5.3.3", "Content", "Content", "bootstrap.css"), "body { color: black; }");
        await WriteFileAsync(Path.Combine(packagesPath, "jQuery.3.7.1", "Scripts", "jquery-3.7.1.js"), "console.log('jquery');");

        var extractor = new NuGetStaticAssetExtractor();
        var result = await extractor.ExtractAsync(new NuGetAssetExtractionOptions(sourcePath, outputPath, packagesPath));

        Assert.True(result.Success);
        Assert.False(result.Skipped);
        Assert.Equal(2, result.PackagesWithAssets);
        Assert.Equal(2, result.TotalFilesExtracted);
        Assert.True(File.Exists(Path.Combine(outputPath, "wwwroot", "lib", "bootstrap", "bootstrap.css")));
        Assert.True(File.Exists(Path.Combine(outputPath, "wwwroot", "lib", "jQuery", "jquery-3.7.1.js")));

        var manifestJson = await File.ReadAllTextAsync(Path.Combine(outputPath, "asset-manifest.json"));
        Assert.Contains("\"PackageId\": \"bootstrap\"", manifestJson);
        Assert.Contains("\"PackageId\": \"jQuery\"", manifestJson);

        var assetReferences = await File.ReadAllTextAsync(Path.Combine(outputPath, "AssetReferences.html"));
        Assert.Contains("/lib/bootstrap/bootstrap.css", assetReferences);
        Assert.Contains("/lib/jQuery/jquery-3.7.1.js", assetReferences);
    }

    [Fact]
    public async Task ExtractAsync_ManifestOnly_WritesManifestWithoutCopyingFiles()
    {
        var sourcePath = CreateDirectory("manifest-only", "source");
        var outputPath = CreateDirectory("manifest-only", "output");
        var packagesPath = Path.Combine(sourcePath, "packages");
        Directory.CreateDirectory(packagesPath);

        await File.WriteAllTextAsync(Path.Combine(sourcePath, "packages.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <packages>
              <package id="bootstrap" version="5.3.3" />
            </packages>
            """);

        await WriteFileAsync(Path.Combine(packagesPath, "bootstrap.5.3.3", "Content", "bootstrap.min.css"), "body { color: black; }");

        var extractor = new NuGetStaticAssetExtractor();
        var result = await extractor.ExtractAsync(new NuGetAssetExtractionOptions(sourcePath, outputPath, packagesPath, ManifestOnly: true));

        Assert.True(result.Success);
        Assert.False(result.Skipped);
        Assert.Equal(1, result.PackagesWithAssets);
        Assert.False(File.Exists(Path.Combine(outputPath, "wwwroot", "lib", "bootstrap", "bootstrap.min.css")));

        using var manifestDocument = JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(outputPath, "asset-manifest.json")));
        Assert.True(manifestDocument.RootElement.GetProperty("ManifestOnly").GetBoolean());
        Assert.Equal("analyzed", manifestDocument.RootElement.GetProperty("Packages")[0].GetProperty("Status").GetString());
        Assert.True(File.Exists(Path.Combine(outputPath, "AssetReferences.html")));
    }

    [Fact]
    public async Task ExtractAsync_MissingPackagesDirectory_CompletesWithoutCopyingFiles()
    {
        var sourcePath = CreateDirectory("missing-packages", "source");
        var outputPath = CreateDirectory("missing-packages", "output");
        var missingPackagesPath = Path.Combine(sourcePath, "packages");

        await File.WriteAllTextAsync(Path.Combine(sourcePath, "packages.config"), """
            <?xml version="1.0" encoding="utf-8"?>
            <packages>
              <package id="Definitely.Not.In.Cache" version="1.0.0" />
            </packages>
            """);

        var extractor = new NuGetStaticAssetExtractor();
        var result = await extractor.ExtractAsync(new NuGetAssetExtractionOptions(sourcePath, outputPath, missingPackagesPath));

        Assert.True(result.Success);
        Assert.Equal(0, result.PackagesWithAssets);
        Assert.Equal(0, result.TotalFilesExtracted);
        Assert.False(Directory.Exists(Path.Combine(outputPath, "wwwroot", "lib")));
        Assert.True(File.Exists(Path.Combine(outputPath, "asset-manifest.json")));
    }

    private string CreateDirectory(params string[] segments)
    {
        var path = Path.Combine([_testRoot, .. segments]);
        Directory.CreateDirectory(path);
        return path;
    }

    private static async Task WriteFileAsync(string path, string content)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(path, content);
    }
}

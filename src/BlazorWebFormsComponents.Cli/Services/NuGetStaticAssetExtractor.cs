using System.Text.Json;
using System.Xml.Linq;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Services;

public sealed class NuGetStaticAssetExtractor
{
    private static readonly HashSet<string> AssetExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".css", ".js", ".map", ".woff", ".woff2", ".ttf", ".eot", ".svg", ".png", ".jpg", ".gif", ".ico"
    };

    private static readonly string[] SearchDirectories = ["Content", "Scripts", "lib", "dist"];

    private static readonly string[] SkipPackagePrefixes =
    [
        "Microsoft.AspNet.Identity",
        "Microsoft.AspNet.Providers",
        "Microsoft.AspNet.ScriptManager",
        "Microsoft.AspNet.Web.Optimization",
        "Microsoft.AspNet.FriendlyUrls",
        "Microsoft.Owin",
        "Microsoft.Web.Infrastructure",
        "Microsoft.CodeDom",
        "Microsoft.Net.Compilers",
        "Microsoft.ApplicationInsights",
        "EntityFramework",
        "Newtonsoft.Json",
        "Owin",
        "Antlr",
        "WebGrease",
        "elmah",
        "AspNet.ScriptManager"
    ];

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public async Task<NuGetAssetExtractionResult?> ExtractAsync(string sourcePath, string outputPath, bool dryRun, MigrationReport report)
    {
        if (dryRun)
        {
            report.Warnings.Add("NuGet static asset extraction is skipped in --dry-run mode.");
            return null;
        }

        var outcome = await ExtractAsync(new NuGetAssetExtractionOptions(sourcePath, outputPath));
        if (!outcome.Success)
        {
            report.Warnings.Add($"NuGet static asset extraction failed: {outcome.ErrorMessage}");
            return null;
        }

        if (outcome.Skipped)
            return null;

        return new NuGetAssetExtractionResult(outcome.PackagesWithAssets, outcome.TotalFilesExtracted);
    }

    public async Task<NuGetAssetExtractionOutcome> ExtractAsync(NuGetAssetExtractionOptions options, CancellationToken cancellationToken = default)
    {
        var sourcePath = Path.GetFullPath(options.SourcePath);
        var outputPath = Path.GetFullPath(options.OutputPath);
        var packages = DiscoverPackages(sourcePath);
        if (packages.Count == 0)
            return NuGetAssetExtractionOutcome.SkippedResult("No packages.config or PackageReference entries found.");

        var packageRoots = ResolvePackageRoots(sourcePath, options.PackagesPath);
        if (packageRoots.Count == 0)
            return NuGetAssetExtractionOutcome.SkippedResult("No local packages directory or global NuGet cache was found.");

        var manifestEntries = new List<NuGetAssetManifestEntry>();
        var packagesWithAssets = 0;
        var totalFilesExtracted = 0;

        foreach (var package in packages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (ShouldSkipPackage(package.Id))
            {
                manifestEntries.Add(new NuGetAssetManifestEntry(
                    package.Id,
                    package.Version,
                    "skipped",
                    "Build/runtime dependency — no static assets expected",
                    []));
                continue;
            }

            if (!TryResolvePackageDirectory(package, packageRoots, out var packageDirectory))
            {
                manifestEntries.Add(new NuGetAssetManifestEntry(
                    package.Id,
                    package.Version,
                    "not-found",
                    "Package folder not found in local packages directory or global cache",
                    []));
                continue;
            }

            var assets = DiscoverAssets(packageDirectory!);
            if (assets.Count == 0)
            {
                manifestEntries.Add(new NuGetAssetManifestEntry(
                    package.Id,
                    package.Version,
                    "no-assets",
                    "No static asset files found in Content, Scripts, lib, or dist.",
                    []));
                continue;
            }

            var copiedFiles = new List<string>();
            foreach (var asset in assets)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var relativeAssetPath = asset.RelativePath.Replace('\\', '/');
                var webPath = $"lib/{package.Id}/{relativeAssetPath}";
                copiedFiles.Add(webPath);

                if (options.ManifestOnly)
                    continue;

                var destinationPath = Path.Combine(outputPath, "wwwroot", "lib", package.Id, asset.RelativePath);
                var destinationDirectory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                File.Copy(asset.SourceFile, destinationPath, overwrite: true);
            }

            packagesWithAssets++;
            totalFilesExtracted += copiedFiles.Count;
            manifestEntries.Add(new NuGetAssetManifestEntry(
                package.Id,
                package.Version,
                options.ManifestOnly ? "analyzed" : "extracted",
                $"{copiedFiles.Count} asset file(s) {(options.ManifestOnly ? "discovered" : "extracted")}",
                copiedFiles));
        }

        Directory.CreateDirectory(outputPath);
        var manifest = new NuGetAssetManifest(
            DateTimeOffset.UtcNow,
            sourcePath,
            string.Join(";", packageRoots),
            outputPath,
            packages.Count,
            packagesWithAssets,
            totalFilesExtracted,
            options.ManifestOnly,
            manifestEntries);

        var manifestPath = Path.Combine(outputPath, "asset-manifest.json");
        await using (var manifestStream = File.Create(manifestPath))
        {
            await JsonSerializer.SerializeAsync(manifestStream, manifest, SerializerOptions, cancellationToken);
        }

        var referencesPath = Path.Combine(outputPath, "AssetReferences.html");
        await File.WriteAllTextAsync(referencesPath, BuildAssetReferences(manifestEntries), cancellationToken);

        return new NuGetAssetExtractionOutcome(
            true,
            false,
            null,
            manifestPath,
            referencesPath,
            packagesWithAssets,
            totalFilesExtracted);
    }

    private static List<PackageReferenceInfo> DiscoverPackages(string sourcePath)
    {
        var packagesConfigPath = Path.Combine(sourcePath, "packages.config");
        if (File.Exists(packagesConfigPath))
        {
            var packagesDocument = XDocument.Load(packagesConfigPath);
            return packagesDocument.Root?
                .Elements()
                .Where(e => e.Name.LocalName == "package")
                .Select(e => new PackageReferenceInfo(
                    e.Attribute("id")?.Value ?? string.Empty,
                    e.Attribute("version")?.Value ?? string.Empty))
                .Where(p => !string.IsNullOrWhiteSpace(p.Id) && !string.IsNullOrWhiteSpace(p.Version))
                .Distinct(PackageReferenceInfoComparer.Instance)
                .ToList() ?? [];
        }

        var projectFile = Directory.EnumerateFiles(sourcePath, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
        if (projectFile is null)
            return [];

        var projectDocument = XDocument.Load(projectFile);
        var properties = projectDocument.Root?
            .Elements()
            .Where(e => e.Name.LocalName == "PropertyGroup")
            .SelectMany(pg => pg.Elements())
            .Where(e => !string.IsNullOrWhiteSpace(e.Value))
            .GroupBy(e => e.Name.LocalName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Last().Value.Trim(), StringComparer.OrdinalIgnoreCase)
            ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        return projectDocument.Root?
            .Descendants()
            .Where(e => e.Name.LocalName == "PackageReference")
            .Select(e =>
            {
                var id = e.Attribute("Include")?.Value ?? e.Attribute("Update")?.Value ?? string.Empty;
                var version = e.Attribute("Version")?.Value
                    ?? e.Elements().FirstOrDefault(child => child.Name.LocalName == "Version")?.Value
                    ?? string.Empty;

                version = ResolveMsBuildProperty(version, properties);
                return new PackageReferenceInfo(id, version);
            })
            .Where(p => !string.IsNullOrWhiteSpace(p.Id) && !string.IsNullOrWhiteSpace(p.Version))
            .Distinct(PackageReferenceInfoComparer.Instance)
            .ToList() ?? [];
    }

    private static string ResolveMsBuildProperty(string version, IReadOnlyDictionary<string, string> properties)
    {
        if (!version.StartsWith("$(", StringComparison.Ordinal) || !version.EndsWith(')'))
            return version.Trim();

        var propertyName = version[2..^1];
        return properties.TryGetValue(propertyName, out var propertyValue)
            ? propertyValue.Trim()
            : version.Trim();
    }

    private static List<string> ResolvePackageRoots(string sourcePath, string? explicitPackagesPath)
    {
        var candidates = new List<string>();

        if (!string.IsNullOrWhiteSpace(explicitPackagesPath))
            candidates.Add(explicitPackagesPath);

        candidates.Add(Path.Combine(Directory.GetParent(sourcePath)?.FullName ?? sourcePath, "packages"));
        candidates.Add(Path.Combine(sourcePath, "packages"));
        candidates.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages"));

        return candidates
            .Where(Directory.Exists)
            .Select(Path.GetFullPath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool TryResolvePackageDirectory(PackageReferenceInfo package, IEnumerable<string> packageRoots, out string? packageDirectory)
    {
        foreach (var packageRoot in packageRoots)
        {
            if (IsGlobalPackagesFolder(packageRoot))
            {
                var packageIdRoot = Path.Combine(packageRoot, package.Id.ToLowerInvariant());
                if (!Directory.Exists(packageIdRoot))
                    continue;

                var directVersionPath = Path.Combine(packageIdRoot, package.Version);
                if (Directory.Exists(directVersionPath))
                {
                    packageDirectory = Path.GetFullPath(directVersionPath);
                    return true;
                }

                var matchedVersion = Directory.EnumerateDirectories(packageIdRoot)
                    .FirstOrDefault(directory => string.Equals(Path.GetFileName(directory), package.Version, StringComparison.OrdinalIgnoreCase));
                if (matchedVersion is not null)
                {
                    packageDirectory = Path.GetFullPath(matchedVersion);
                    return true;
                }

                continue;
            }

            var expectedFolder = Path.Combine(packageRoot, $"{package.Id}.{package.Version}");
            if (Directory.Exists(expectedFolder))
            {
                packageDirectory = Path.GetFullPath(expectedFolder);
                return true;
            }

            var matchedDirectory = Directory.EnumerateDirectories(packageRoot)
                .FirstOrDefault(directory => string.Equals(Path.GetFileName(directory), $"{package.Id}.{package.Version}", StringComparison.OrdinalIgnoreCase));
            if (matchedDirectory is not null)
            {
                packageDirectory = Path.GetFullPath(matchedDirectory);
                return true;
            }
        }

        packageDirectory = null;
        return false;
    }

    private static bool IsGlobalPackagesFolder(string packageRoot)
    {
        var normalized = Path.GetFullPath(packageRoot)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        return normalized.EndsWith($".nuget{Path.DirectorySeparatorChar}packages", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ShouldSkipPackage(string packageId)
    {
        return SkipPackagePrefixes.Any(prefix =>
            string.Equals(packageId, prefix, StringComparison.OrdinalIgnoreCase)
            || packageId.StartsWith(prefix + ".", StringComparison.OrdinalIgnoreCase));
    }

    private static List<DiscoveredAsset> DiscoverAssets(string packageDirectory)
    {
        var assets = new List<DiscoveredAsset>();
        var visitedDirectories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var searchDirectory in SearchDirectories)
        {
            var assetDirectory = Path.Combine(packageDirectory, searchDirectory);
            if (!Directory.Exists(assetDirectory))
                continue;

            var fullAssetDirectory = Path.GetFullPath(assetDirectory);
            if (!visitedDirectories.Add(fullAssetDirectory))
                continue;

            foreach (var file in Directory.EnumerateFiles(fullAssetDirectory, "*", SearchOption.AllDirectories))
            {
                if (!AssetExtensions.Contains(Path.GetExtension(file)))
                    continue;
                if (ShouldSkipFile(file))
                    continue;

                var relativePath = StripRedundantContainerFolder(Path.GetRelativePath(fullAssetDirectory, file));
                assets.Add(new DiscoveredAsset(file, relativePath));
            }
        }

        return assets
            .OrderBy(asset => asset.RelativePath, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool ShouldSkipFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        if (fileName.Contains("intellisense", StringComparison.OrdinalIgnoreCase))
            return true;
        if (fileName.Contains("-vsdoc", StringComparison.OrdinalIgnoreCase))
            return true;
        if (string.Equals(fileName, "_references.js", StringComparison.OrdinalIgnoreCase))
            return true;
        if (filePath.Contains($"{Path.DirectorySeparatorChar}WebForms{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            return true;
        if (fileName.EndsWith(".min.map", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private static string StripRedundantContainerFolder(string relativePath)
    {
        var normalized = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        var segments = normalized.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 1
            && (segments[0].Equals("Content", StringComparison.OrdinalIgnoreCase)
                || segments[0].Equals("Scripts", StringComparison.OrdinalIgnoreCase)))
        {
            return Path.Combine(segments.Skip(1).ToArray());
        }

        return normalized;
    }

    private static string BuildAssetReferences(IEnumerable<NuGetAssetManifestEntry> packages)
    {
        var files = packages
            .Where(package => package.Status is "extracted" or "analyzed")
            .SelectMany(package => package.Files)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var cssFiles = SelectPreferredAssets(files.Where(file => file.EndsWith(".css", StringComparison.OrdinalIgnoreCase)));
        var jsFiles = SelectPreferredAssets(files.Where(file => file.EndsWith(".js", StringComparison.OrdinalIgnoreCase)));

        var lines = new List<string>
        {
            "<!-- NuGet static assets — extracted by webforms-to-blazor -->",
            "<!-- Paste CSS references into App.razor <head> section -->"
        };

        if (cssFiles.Count == 0)
        {
            lines.Add("<!-- No CSS assets found in NuGet packages -->");
        }
        else
        {
            lines.AddRange(cssFiles.Select(file => $"    <link rel=\"stylesheet\" href=\"/{file}\" />"));
        }

        lines.Add(string.Empty);
        lines.Add("<!-- Paste JS references before </body> in App.razor -->");

        if (jsFiles.Count == 0)
        {
            lines.Add("<!-- No JS assets found in NuGet packages -->");
        }
        else
        {
            lines.AddRange(jsFiles.Select(file => $"    <script src=\"/{file}\"></script>"));
        }

        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }

    private static List<string> SelectPreferredAssets(IEnumerable<string> files)
    {
        return files
            .GroupBy(GetAssetPreferenceKey, StringComparer.OrdinalIgnoreCase)
            .Select(group => group
                .OrderByDescending(file => file.Contains(".min.", StringComparison.OrdinalIgnoreCase))
                .ThenBy(file => file, StringComparer.OrdinalIgnoreCase)
                .First())
            .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string GetAssetPreferenceKey(string file)
    {
        return file.Replace(".min.", ".", StringComparison.OrdinalIgnoreCase);
    }

    private sealed record PackageReferenceInfo(string Id, string Version);

    private sealed record DiscoveredAsset(string SourceFile, string RelativePath);

    private sealed class PackageReferenceInfoComparer : IEqualityComparer<PackageReferenceInfo>
    {
        public static PackageReferenceInfoComparer Instance { get; } = new();

        public bool Equals(PackageReferenceInfo? x, PackageReferenceInfo? y)
        {
            if (x is null && y is null)
                return true;
            if (x is null || y is null)
                return false;

            return string.Equals(x.Id, y.Id, StringComparison.OrdinalIgnoreCase)
                && string.Equals(x.Version, y.Version, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(PackageReferenceInfo obj)
        {
            return HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Id),
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Version));
        }
    }
}

public sealed record NuGetAssetExtractionOptions(string SourcePath, string OutputPath, string? PackagesPath = null, bool ManifestOnly = false);

public sealed record NuGetAssetExtractionOutcome(
    bool Success,
    bool Skipped,
    string? ErrorMessage,
    string? ManifestPath,
    string? AssetReferencesPath,
    int PackagesWithAssets,
    int TotalFilesExtracted)
{
    public static NuGetAssetExtractionOutcome SkippedResult(string message) => new(true, true, message, null, null, 0, 0);
}

public sealed record NuGetAssetExtractionResult(int PackagesWithAssets, int TotalFilesExtracted);

public sealed record NuGetAssetManifest(
    DateTimeOffset Timestamp,
    string SourceProject,
    string PackagesFolder,
    string OutputProject,
    int TotalPackages,
    int PackagesWithAssets,
    int TotalFilesExtracted,
    bool ManifestOnly,
    IReadOnlyList<NuGetAssetManifestEntry> Packages);

public sealed record NuGetAssetManifestEntry(
    string PackageId,
    string Version,
    string Status,
    string Reason,
    IReadOnlyList<string> Files);

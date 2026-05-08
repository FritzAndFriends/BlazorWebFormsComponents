namespace BlazorWebFormsComponents.Cli.Scaffolding;

public class RuntimeDetector
{
    private readonly IReadOnlyList<IRuntimeSignalDetector> _detectors;

    public RuntimeDetector(IEnumerable<IRuntimeSignalDetector> detectors)
    {
        _detectors = detectors.ToList();
    }

    public RuntimeProfile Detect(string sourcePath)
    {
        var profile = new RuntimeProfile();

        if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
            return profile;

        foreach (var detector in _detectors)
        {
            detector.Apply(sourcePath, profile);
        }

        profile.ConnectionStringNames = profile.ConnectionStringNames
            .Where(static name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        profile.ApplicationStartPatterns = profile.ApplicationStartPatterns
            .Where(static pattern => !string.IsNullOrWhiteSpace(pattern))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return profile;
    }
}

public class RuntimeProfile
{
    public bool NeedsSession { get; set; }
    public bool NeedsIdentity { get; set; }
    public bool NeedsEntityFramework { get; set; }
    public string? DbContextClassName { get; set; }
    public string? DbContextNamespace { get; set; }
    public List<string> ConnectionStringNames { get; set; } = [];
    public List<string> ApplicationStartPatterns { get; set; } = [];

    public string? ResolvedDbContextTypeName =>
        string.IsNullOrWhiteSpace(DbContextClassName)
            ? null
            : string.IsNullOrWhiteSpace(DbContextNamespace)
                ? DbContextClassName
                : $"global::{DbContextNamespace}.{DbContextClassName}";
}

internal static class RuntimeDetectionFiles
{
    private static readonly HashSet<string> SkippedDirectories =
    [
        ".git",
        ".vs",
        "bin",
        "obj",
        "node_modules",
        "packages"
    ];

    public static IEnumerable<string> EnumerateFiles(string sourcePath, params string[] extensions)
    {
        var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);

        foreach (var file in Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories))
        {
            if (IsSkipped(sourcePath, file))
                continue;

            if (allowedExtensions.Count == 0 || allowedExtensions.Contains(Path.GetExtension(file)))
                yield return file;
        }
    }

    private static bool IsSkipped(string sourcePath, string filePath)
    {
        var relativePath = Path.GetRelativePath(sourcePath, filePath);
        var segments = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return segments.Any(SkippedDirectories.Contains);
    }
}

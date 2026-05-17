using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Aligns the code-behind namespace with the generated Razor namespace derived
/// from the output path under the migration project root.
/// Also normalizes project namespace segment casing (for example Bll -> BLL)
/// to match the actual source folder casing.
/// </summary>
public class NamespaceAlignTransform : ICodeBehindTransform
{
    public string Name => "NamespaceAlign";
    public int Order => 212; // After ClassNameAlignTransform (210)

    private static readonly Regex FileScopedNamespaceRegex = new(
        @"(?m)^\s*namespace\s+([A-Za-z_][A-Za-z0-9_\.]*)\s*;\s*$",
        RegexOptions.Compiled);

    private static readonly Regex BlockScopedNamespaceRegex = new(
        @"(?m)^(\s*namespace\s+)([A-Za-z_][A-Za-z0-9_\.]*)(\s*\{)",
        RegexOptions.Compiled);

    private static readonly Regex ProjectNamespaceLineRegex = new(
        @"(?m)^(?<indent>\s*(?:using|namespace)\s+)(?<ns>[A-Za-z_][A-Za-z0-9_\.]*)(?<suffix>\s*(?:;|\{))",
        RegexOptions.Compiled);

    private static readonly ConcurrentDictionary<string, IReadOnlyDictionary<string, string>> SegmentCasingCache =
        new(StringComparer.OrdinalIgnoreCase);

    public string Apply(string content, FileMetadata metadata)
    {
        var expectedNamespace = GetExpectedNamespace(metadata);
        if (!string.IsNullOrEmpty(expectedNamespace))
        {
            if (FileScopedNamespaceRegex.IsMatch(content))
            {
                content = FileScopedNamespaceRegex.Replace(content, $"namespace {expectedNamespace};", 1);
            }
            else if (BlockScopedNamespaceRegex.IsMatch(content))
            {
                content = BlockScopedNamespaceRegex.Replace(content, $"$1{expectedNamespace}$3", 1);
            }
        }

        return NormalizeProjectNamespaceSegmentCasing(content, metadata);
    }

    private static string NormalizeProjectNamespaceSegmentCasing(string content, FileMetadata metadata)
    {
        if (string.IsNullOrWhiteSpace(metadata.ProjectNamespace) ||
            string.IsNullOrWhiteSpace(metadata.SourceRootPath))
        {
            return content;
        }

        var segmentCasing = GetSegmentCasing(metadata.SourceRootPath);
        if (segmentCasing.Count == 0)
            return content;

        return ProjectNamespaceLineRegex.Replace(content, match =>
        {
            var namespaceValue = match.Groups["ns"].Value;
            if (!namespaceValue.Equals(metadata.ProjectNamespace, StringComparison.Ordinal) &&
                !namespaceValue.StartsWith(metadata.ProjectNamespace + ".", StringComparison.Ordinal))
            {
                return match.Value;
            }

            var normalizedNamespace = NormalizeNamespaceSegments(namespaceValue, metadata.ProjectNamespace, segmentCasing);
            return $"{match.Groups["indent"].Value}{normalizedNamespace}{match.Groups["suffix"].Value}";
        });
    }

    private static string NormalizeNamespaceSegments(
        string namespaceValue,
        string projectNamespace,
        IReadOnlyDictionary<string, string> segmentCasing)
    {
        if (namespaceValue.Equals(projectNamespace, StringComparison.Ordinal))
            return namespaceValue;

        var segments = namespaceValue[(projectNamespace.Length + 1)..]
            .Split('.', StringSplitOptions.RemoveEmptyEntries)
            .Select(segment => segmentCasing.TryGetValue(segment, out var normalized) ? normalized : segment);

        return $"{projectNamespace}.{string.Join(".", segments)}";
    }

    private static IReadOnlyDictionary<string, string> GetSegmentCasing(string sourceRootPath)
    {
        var normalizedSourceRoot = Path.GetFullPath(sourceRootPath);
        return SegmentCasingCache.GetOrAdd(normalizedSourceRoot, static root => BuildSegmentCasing(root));
    }

    private static IReadOnlyDictionary<string, string> BuildSegmentCasing(string sourceRootPath)
    {
        var segmentCasing = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!Directory.Exists(sourceRootPath))
            return segmentCasing;

        foreach (var directory in Directory.EnumerateDirectories(sourceRootPath, "*", SearchOption.AllDirectories))
        {
            var segment = Path.GetFileName(directory);
            if (!string.IsNullOrWhiteSpace(segment) && !segmentCasing.ContainsKey(segment))
                segmentCasing[segment] = segment;
        }

        return segmentCasing;
    }

    private static string? GetExpectedNamespace(FileMetadata metadata)
    {
        if (string.IsNullOrWhiteSpace(metadata.ProjectNamespace) ||
            string.IsNullOrWhiteSpace(metadata.OutputRootPath))
        {
            return null;
        }

        var relativePath = Path.GetRelativePath(metadata.OutputRootPath, metadata.OutputFilePath);
        var relativeDirectory = Path.GetDirectoryName(relativePath);

        if (string.IsNullOrWhiteSpace(relativeDirectory) || relativeDirectory == ".")
            return metadata.ProjectNamespace;

        var namespaceSegments = relativeDirectory
            .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            .Where(static segment => !string.IsNullOrWhiteSpace(segment))
            .Select(SanitizeNamespaceSegment);

        return $"{metadata.ProjectNamespace}.{string.Join(".", namespaceSegments)}";
    }

    private static string SanitizeNamespaceSegment(string segment)
    {
        var sanitized = segment.Replace('.', '_').Replace('-', '_');
        if (sanitized.Length > 0 && char.IsDigit(sanitized[0]))
            sanitized = "_" + sanitized;

        return sanitized;
    }
}

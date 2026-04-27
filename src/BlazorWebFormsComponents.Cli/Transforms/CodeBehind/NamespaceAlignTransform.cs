using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Aligns the code-behind namespace with the generated Razor namespace derived
/// from the output path under the migration project root.
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

    public string Apply(string content, FileMetadata metadata)
    {
        var expectedNamespace = GetExpectedNamespace(metadata);
        if (string.IsNullOrEmpty(expectedNamespace))
            return content;

        if (FileScopedNamespaceRegex.IsMatch(content))
        {
            return FileScopedNamespaceRegex.Replace(content, $"namespace {expectedNamespace};", 1);
        }

        if (BlockScopedNamespaceRegex.IsMatch(content))
        {
            return BlockScopedNamespaceRegex.Replace(content, $"$1{expectedNamespace}$3", 1);
        }

        return content;
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

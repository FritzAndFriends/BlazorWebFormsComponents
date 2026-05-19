using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Ensures code-behind and source files have using directives for known project sub-namespaces
/// (Logic, BLL, Services) when those directories exist. This prevents CS0246 errors for types
/// that live in sibling namespace folders.
/// </summary>
public class ProjectNamespaceUsingTransform : ICodeBehindTransform
{
    public string Name => "ProjectNamespaceUsing";
    public int Order => 102; // Early — before most transforms, after UsingStrip (100)

    private static readonly Regex NamespaceRegex = new(
        @"^\s*namespace\s+(?<ns>[A-Za-z_][\w.]*)",
        RegexOptions.Compiled | RegexOptions.Multiline);

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.SourceRootPath == null || metadata.ProjectNamespace == null)
            return content;

        // Discover sub-namespace directories
        var subDirs = new[] { "Logic", "BLL", "Services" };
        var namespacesToAdd = new List<string>();

        foreach (var dir in subDirs)
        {
            var dirPath = Path.Combine(metadata.SourceRootPath, dir);
            if (!Directory.Exists(dirPath)) continue;

            // Read actual namespace from first .cs in that dir
            var firstFile = Directory.EnumerateFiles(dirPath, "*.cs").FirstOrDefault();
            if (firstFile == null) continue;

            var fileContent = File.ReadAllText(firstFile);
            var nsMatch = NamespaceRegex.Match(fileContent);
            if (!nsMatch.Success) continue;

            var ns = nsMatch.Groups["ns"].Value;
            // Only add if it's a child of the project namespace
            if (!ns.StartsWith(metadata.ProjectNamespace, StringComparison.Ordinal))
                continue;

            // Don't add if file is already in that namespace
            var fileNsMatch = NamespaceRegex.Match(content);
            if (fileNsMatch.Success && fileNsMatch.Groups["ns"].Value == ns)
                continue;

            // Don't add if already present as a using
            if (content.Contains($"using {ns};", StringComparison.Ordinal))
                continue;

            namespacesToAdd.Add(ns);
        }

        if (namespacesToAdd.Count == 0)
            return content;

        // Insert after the last existing using directive
        // First try: using on its own line (standard case)
        var lastUsingMatch = Regex.Match(content, @"^using\s+[^;(\n]+;\s*$",
            RegexOptions.Multiline | RegexOptions.RightToLeft);
        if (lastUsingMatch.Success)
        {
            var insertAt = lastUsingMatch.Index + lastUsingMatch.Length;
            var usings = string.Join("", namespacesToAdd.Select(ns => $"\nusing {ns};"));
            content = content[..insertAt] + usings + content[insertAt..];
        }
        else
        {
            // Fallback: find last "using X;" even if followed by other code on same line
            // (happens when namespace declaration was stripped without adding newline)
            var fallbackMatch = Regex.Match(content, @"using\s+[^;(\n]+;",
                RegexOptions.RightToLeft);
            if (fallbackMatch.Success)
            {
                var insertAt = fallbackMatch.Index + fallbackMatch.Length;
                var usings = string.Join("", namespacesToAdd.Select(ns => $"\nusing {ns};"));
                content = content[..insertAt] + usings + content[insertAt..];
            }
        }

        return content;
    }
}

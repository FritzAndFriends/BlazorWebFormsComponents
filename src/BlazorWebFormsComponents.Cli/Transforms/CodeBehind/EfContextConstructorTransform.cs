using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Rewrites EF6 DbContext constructors that call <c>base("name")</c> to the
/// EF Core <c>DbContextOptions&lt;TContext&gt;</c> pattern.
/// </summary>
public class EfContextConstructorTransform : ICodeBehindTransform
{
    private static readonly Regex DbContextClassRegex = new(
        @"class\s+(?<name>\w+)\s*:\s*(?<base>(?:\w+\.)*(?:DbContext|IdentityDbContext)(?:<[^\r\n>{]+>)?)(?=\s|,|\{)",
        RegexOptions.Compiled);

    public string Name => "EfContextConstructor";
    public int Order => 106;

    public string Apply(string content, FileMetadata metadata)
    {
        var contextClassNames = DbContextClassRegex.Matches(content)
            .Select(match => match.Groups["name"].Value)
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (contextClassNames.Count == 0)
            return content;

        var updated = false;

        foreach (var className in contextClassNames)
        {
            var constructorRegex = new Regex(
                $@"(?<indent>^[ \t]*)(?<modifier>(?:public|protected|internal|private)\s+){Regex.Escape(className)}\s*\([^)]*\)\s*(?:\r?\n\s*)?:\s*base\s*\(\s*@?""[^""]*""\s*\)",
                RegexOptions.Compiled | RegexOptions.Multiline);

            content = constructorRegex.Replace(content, match =>
            {
                updated = true;
                var indent = match.Groups["indent"].Value;
                var modifier = match.Groups["modifier"].Value;
                return $"{indent}{modifier}{className}(DbContextOptions<{className}> options) : base(options)";
            });
        }

        if (!updated)
            return content;

        return EnsureUsing(content, "Microsoft.EntityFrameworkCore");
    }

    private static string EnsureUsing(string content, string @namespace)
    {
        var usingLine = $"using {@namespace};";
        if (content.Contains(usingLine, StringComparison.Ordinal))
            return content;

        var lastUsing = Regex.Match(content, @"^using\s+[^;(\n]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
        if (lastUsing.Success)
        {
            var insertAt = lastUsing.Index + lastUsing.Length;
            return content[..insertAt] + Environment.NewLine + usingLine + content[insertAt..];
        }

        return usingLine + Environment.NewLine + content;
    }
}


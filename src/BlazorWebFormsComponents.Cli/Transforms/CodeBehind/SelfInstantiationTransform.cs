using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Rewrites zero-argument self-instantiation (new CurrentClass()) to <c>this</c>
/// so DI-enabled pages/services don't try to construct themselves without required dependencies.
/// </summary>
public sealed class SelfInstantiationTransform : ICodeBehindTransform
{
    private static readonly Regex ClassNameRegex = new(
        @"(?:public|protected|internal|private)?\s*(?:(?:abstract|sealed|partial|static)\s+)*class\s+(?<name>[A-Za-z_]\w*)",
        RegexOptions.Compiled);

    public string Name => "SelfInstantiation";
    public int Order => 218; // After MethodNameCollision (215), before ComponentRefCodeBehind (220)

    public string Apply(string content, FileMetadata metadata)
    {
        var classMatch = ClassNameRegex.Match(content);
        if (!classMatch.Success)
            return content;

        var className = classMatch.Groups["name"].Value;
        if (string.IsNullOrWhiteSpace(className))
            return content;

        return Regex.Replace(
            content,
            $@"(?<![\w.])new\s+{Regex.Escape(className)}\s*\(\s*\)",
            "this",
            RegexOptions.None,
            TimeSpan.FromMilliseconds(500));
    }
}

using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Transforms Web Forms event handler signatures to Blazor-compatible signatures:
///   - Strip the 'object sender' parameter, keep EventArgs param
///   - Normalize ImageClickEventArgs → EventArgs (BWFC components use EventCallback&lt;EventArgs&gt;)
/// </summary>
public class EventHandlerSignatureTransform : ICodeBehindTransform
{
    public string Name => "EventHandlerSignature";
    public int Order => 700;

    // Group 1: everything before parens (modifiers + return type + method name)
    // Group 2: the EventArgs type name
    // Group 3: the EventArgs parameter name
    private static readonly Regex HandlerRegex = new(
        @"((?:(?:protected|private|public|internal)\s+)?(?:(?:static|virtual|override|new|sealed|abstract|async)\s+)*(?:void|Task(?:<[^>]+>)?)\s+\w+)\s*\(\s*object\s+\w+\s*,\s*(\w*EventArgs)\s+(\w+)\s*\)",
        RegexOptions.Compiled);

    // Web Forms EventArgs types that don't exist in Blazor or don't match BWFC component signatures.
    // These are normalized to EventArgs so the handler matches EventCallback<EventArgs>.
    private static readonly HashSet<string> NormalizedToEventArgs = new(StringComparer.Ordinal)
    {
        "ImageClickEventArgs",
    };

    private const int MaxIterations = 200;

    public string Apply(string content, FileMetadata metadata)
    {
        var iterations = 0;

        while (HandlerRegex.IsMatch(content) && iterations < MaxIterations)
        {
            iterations++;
            var match = HandlerRegex.Match(content);
            var prefix = match.Groups[1].Value;
            var eventArgsType = match.Groups[2].Value;
            var eventArgsParam = match.Groups[3].Value;

            // Normalize Web Forms-only EventArgs types to standard EventArgs
            if (NormalizedToEventArgs.Contains(eventArgsType))
                eventArgsType = "EventArgs";

            // Always keep the EventArgs param — BWFC components use EventCallback<EventArgs>
            var replacement = $"{prefix}({eventArgsType} {eventArgsParam})";

            content = content[..match.Index] + replacement + content[(match.Index + match.Length)..];
        }

        return content;
    }
}

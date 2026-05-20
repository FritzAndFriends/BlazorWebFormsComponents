using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Transforms Web Forms event handler signatures to Blazor-compatible signatures:
///   - Strip the 'object sender' parameter, keep EventArgs param
///   - Normalize ImageClickEventArgs → EventArgs (BWFC components use EventCallback&lt;EventArgs&gt;)
///   - Map GridViewPageEventArgs → PageChangedEventArgs (BWFC paging event type)
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

    // Web Forms EventArgs types that map to specific BWFC event types.
    private static readonly Dictionary<string, string> EventArgsMappings = new(StringComparer.Ordinal)
    {
        ["GridViewPageEventArgs"] = "PageChangedEventArgs",
        ["DetailsViewPageEventArgs"] = "PageChangedEventArgs",
        ["FormViewPageEventArgs"] = "PageChangedEventArgs",
        ["ListViewPageEventArgs"] = "PageChangedEventArgs",
    };

    private const int MaxIterations = 200;

    // Matches direct invocations of a method with two arguments (potential sender+eventArgs call sites)
    private static readonly Regex TwoArgCallRegex = new(
        @"\b(?<method>\w+)\s*\(\s*(?<arg1>[^,()]+?)\s*,\s*(?<arg2>[^,()]+?)\s*\)",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        var iterations = 0;
        var convertedMethods = new HashSet<string>(StringComparer.Ordinal);

        while (HandlerRegex.IsMatch(content) && iterations < MaxIterations)
        {
            iterations++;
            var match = HandlerRegex.Match(content);
            var prefix = match.Groups[1].Value;
            var eventArgsType = match.Groups[2].Value;
            var eventArgsParam = match.Groups[3].Value;

            // Extract the method name from the prefix for call-site fixing
            var methodNameMatch = Regex.Match(prefix, @"\b(\w+)\s*$");
            if (methodNameMatch.Success)
                convertedMethods.Add(methodNameMatch.Groups[1].Value);

            // Normalize Web Forms-only EventArgs types to standard EventArgs
            if (NormalizedToEventArgs.Contains(eventArgsType))
                eventArgsType = "EventArgs";
            else if (EventArgsMappings.TryGetValue(eventArgsType, out var mappedType))
                eventArgsType = mappedType;

            // Always keep the EventArgs param — BWFC components use EventCallback<EventArgs>
            var replacement = $"{prefix}({eventArgsType} {eventArgsParam})";

            content = content[..match.Index] + replacement + content[(match.Index + match.Length)..];
        }

        // Fix call sites: convert methodName(arg1, arg2) → methodName(arg2) for converted methods
        if (convertedMethods.Count > 0)
        {
            foreach (var method in convertedMethods)
            {
                var callPattern = new Regex(
                    $@"\b{Regex.Escape(method)}\s*\(\s*(?<arg1>[^,()]+?)\s*,\s*(?<arg2>[^,()]+?)\s*\)",
                    RegexOptions.Compiled);
                content = callPattern.Replace(content, m =>
                {
                    // Only fix calls that look like event handler invocations (2 args → 1 arg)
                    // Skip if this is the method definition itself (has return type before it)
                    var beforeMatch = content[..m.Index];
                    var lastLine = beforeMatch.LastIndexOf('\n');
                    var linePrefix = lastLine >= 0 ? beforeMatch[(lastLine + 1)..] : beforeMatch;
                    if (linePrefix.Contains("void ") || linePrefix.Contains("Task "))
                        return m.Value; // This is a method definition, not a call
                    return $"{method}({m.Groups["arg2"].Value})";
                });
            }
        }

        return content;
    }
}

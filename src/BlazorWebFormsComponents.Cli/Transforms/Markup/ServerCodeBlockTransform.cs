using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts ASP.NET Web Forms server-side code blocks to Razor control-flow syntax.
/// Handles if/else/foreach/for/while statement blocks and bare code blocks.
/// Also sanitizes any residual &lt;% %&gt; delimiters found inside @* *@ Razor comments
/// (which ExpressionTransform produced from &lt;%-- --%&gt; comments).
/// Must run after ExpressionTransform (500) and before AspPrefixTransform (610).
/// </summary>
public class ServerCodeBlockTransform : IMarkupTransform
{
    public string Name => "ServerCodeBlock";
    public int Order => 510;

    // Match @* ... *@ Razor comments so we can sanitize inner <% %> remnants
    private static readonly Regex RazorCommentRegex = new(
        @"(?s)@\*(.*?)\*@",
        RegexOptions.Compiled);

    // Match <% ... %> statement blocks — excludes <%#, <%:, <%=, <%--
    private static readonly Regex StatementBlockRegex = new(
        @"(?s)<%(?![#:=\-])(.*?)%>",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // Step 1: neutralize <% %> inside @* *@ Razor comments so Razor doesn't choke.
        // ExpressionTransform already converted <%-- --%> to @* *@, but the inner
        // <% %> fragments survived as raw text.
        content = RazorCommentRegex.Replace(content, m =>
        {
            var inner = m.Groups[1].Value
                .Replace("<%", "[%")
                .Replace("%>", "%]");
            return $"@*{inner}*@";
        });

        // Step 2: convert remaining statement blocks to Razor control-flow.
        content = StatementBlockRegex.Replace(content, m =>
            TransformStatementBlock(m.Groups[1].Value));

        return content;
    }

    private static string TransformStatementBlock(string rawInner)
    {
        // Collapse all whitespace (including newlines from multi-line blocks) to single spaces.
        var normalized = Regex.Replace(rawInner.Trim(), @"\s+", " ").Trim();

        // <% } %> — closing brace only
        if (Regex.IsMatch(normalized, @"^\}\s*$"))
            return "}";

        // <% } else if (condition) { %>
        var elseIfMatch = Regex.Match(normalized,
            @"^\}\s*else\s+if\s*(\(.*\))\s*\{?\s*$", RegexOptions.IgnoreCase);
        if (elseIfMatch.Success)
            return "}\nelse if " + elseIfMatch.Groups[1].Value + "\n{";

        // <% } else { %>
        if (Regex.IsMatch(normalized, @"^\}\s*else\s*\{?\s*$", RegexOptions.IgnoreCase))
            return "}\nelse\n{";

        // <% foreach (...) { %>
        var foreachCond = TryExtractCondition(normalized, "foreach");
        if (foreachCond != null)
            return "@foreach " + foreachCond + "\n{";

        // <% for (...) { %>
        var forCond = TryExtractCondition(normalized, "for");
        if (forCond != null)
            return "@for " + forCond + "\n{";

        // <% while (...) { %>
        var whileCond = TryExtractCondition(normalized, "while");
        if (whileCond != null)
            return "@while " + whileCond + "\n{";

        // <% if (...) { %> — checked after foreach/for/while to avoid false matches
        var ifCond = TryExtractCondition(normalized, "if");
        if (ifCond != null)
            return "@if " + ifCond + "\n{";

        // Bare statement block: <% code; %>
        return "@{ " + normalized + " }";
    }

    /// <summary>
    /// Tries to extract the parenthesised condition after a control-flow keyword.
    /// Returns the condition string (including outer parens) or null if no match.
    /// </summary>
    private static string? TryExtractCondition(string normalized, string keyword)
    {
        if (!normalized.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
            return null;

        // Ensure the keyword is not a prefix of a longer identifier
        if (normalized.Length > keyword.Length)
        {
            var next = normalized[keyword.Length];
            if (char.IsLetterOrDigit(next) || next == '_')
                return null;
        }

        var rest = normalized.Substring(keyword.Length).TrimStart();

        // Condition must start with '('
        if (!rest.StartsWith("("))
            return null;

        // Strip trailing ' {' or '{' when the opening brace was on the same line
        rest = rest.TrimEnd();
        if (rest.EndsWith("{"))
            rest = rest[..^1].TrimEnd();

        return rest.Trim();
    }
}

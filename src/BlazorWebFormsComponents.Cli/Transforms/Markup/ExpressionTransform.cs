using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Converts ASP.NET Web Forms expressions to Razor syntax.
/// Handles comments, Bind(), Eval(), Item., encoded and unencoded expressions.
/// </summary>
public class ExpressionTransform : IMarkupTransform
{
    public string Name => "Expression";
    public int Order => 500;

    // Comments: <%-- ... --%> → @* ... *@
    private static readonly Regex CommentRegex = new(@"(?s)<%--(.+?)--%>", RegexOptions.Compiled);

    // Bind() inside attribute values (single-quoted): attr='<%# Bind("Prop") %>' → @bind-Value="context.Prop"
    private static readonly Regex BindAttrSingleRegex = new(
        @"(\w+)\s*=\s*'<%#\s*Bind\(""(\w+)""\)\s*%>'",
        RegexOptions.Compiled);

    // Bind() inside attribute values (double-quoted): attr="<%# Bind("Prop") %>"
    private static readonly Regex BindAttrDoubleRegex = new(
        @"(\w+)\s*=\s*""<%#\s*Bind\(""(\w+)""\)\s*%>""",
        RegexOptions.Compiled);

    // Bind() with HTML-encoded delimiter: <%#: Bind("Prop") %> → @context.Prop
    private static readonly Regex BindEncodedRegex = new(
        @"<%#:\s*Bind\(""(\w+)""\)\s*%>",
        RegexOptions.Compiled);

    // Standalone Bind(): <%# Bind("Prop") %> → @context.Prop
    private static readonly Regex BindStandaloneRegex = new(
        @"<%#\s*Bind\(""(\w+)""\)\s*%>",
        RegexOptions.Compiled);

    // Eval with format string: <%#: Eval("prop", "{0:fmt}") %> → @context.prop.ToString("fmt")
    private static readonly Regex EvalFmtRegex = new(
        @"<%#:\s*Eval\(""(\w+)"",\s*""\{0:([^}]+)\}""\)\s*%>",
        RegexOptions.Compiled);

    // String.Format with Item.Property: <%#: String.Format("{0:fmt}", Item.Prop) %>
    private static readonly Regex StringFmtRegex = new(
        @"<%#:\s*String\.Format\(""\{0:([^}]+)\}"",\s*Item\.(\w+)\)\s*%>",
        RegexOptions.Compiled);

    // Eval: <%#: Eval("prop") %> → @context.prop
    private static readonly Regex EvalRegex = new(
        @"<%#:\s*Eval\(""(\w+)""\)\s*%>",
        RegexOptions.Compiled);

    // Item property: <%#: Item.Prop %> → @context.Prop
    private static readonly Regex ItemPropRegex = new(
        @"<%#:\s*Item\.(\w+)\s*%>",
        RegexOptions.Compiled);

    // Bare Item: <%#: Item %> → @context
    private static readonly Regex BareItemRegex = new(
        @"<%#:\s*Item\s*%>",
        RegexOptions.Compiled);

    // Encoded: <%: expr %> → @(expr)
    private static readonly Regex EncodedRegex = new(
        @"<%:\s*(.+?)\s*%>",
        RegexOptions.Compiled);

    // Unencoded: <%= expr %> → @(expr)
    private static readonly Regex UnencodedRegex = new(
        @"<%=\s*(.+?)\s*%>",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // Comments first
        content = CommentRegex.Replace(content, "@*$1*@");

        // Bind() transforms (before Eval to avoid conflicts)
        content = BindAttrSingleRegex.Replace(content, "@bind-Value=\"context.$2\"");
        content = BindAttrDoubleRegex.Replace(content, "@bind-Value=\"context.$2\"");
        content = BindEncodedRegex.Replace(content, "@context.$1");
        content = BindStandaloneRegex.Replace(content, "@context.$1");

        // Eval with format string
        content = EvalFmtRegex.Replace(content, "@context.$1.ToString(\"$2\")");

        // String.Format with Item
        content = StringFmtRegex.Replace(content, "@($\"{context.$2:$1}\")");

        // Eval binding
        content = EvalRegex.Replace(content, "@context.$1");

        // Item property binding
        content = ItemPropRegex.Replace(content, "@context.$1");

        // Bare Item
        content = BareItemRegex.Replace(content, "@context");

        // Encoded expressions
        content = EncodedRegex.Replace(content, "@($1)");

        // Unencoded expressions
        content = UnencodedRegex.Replace(content, "@($1)");

        return content;
    }
}

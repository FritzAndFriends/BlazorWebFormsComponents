using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for ServerCodeBlockTransform — converts ASP.NET &lt;% %&gt; statement blocks
/// to Razor control-flow and sanitizes code-block fragments inside Razor comments.
/// </summary>
public class ServerCodeBlockTransformTests
{
    private readonly ServerCodeBlockTransform _transform = new();

    private static FileMetadata TestMeta => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = string.Empty
    };

    // ──────────────────────────────────────────────────────────────
    // Metadata
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void HasCorrectOrder()
    {
        Assert.Equal(510, _transform.Order);
    }

    [Fact]
    public void HasCorrectName()
    {
        Assert.Equal("ServerCodeBlock", _transform.Name);
    }

    // ──────────────────────────────────────────────────────────────
    // if / else if / else / closing brace
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void ConvertsIfBlock()
    {
        var input = "<% if (IsEnabled) { %>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal("@if (IsEnabled)\n{", result);
    }

    [Fact]
    public void ConvertsClosingBrace()
    {
        var input = "<% } %>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal("}", result);
    }

    [Fact]
    public void ConvertsElseBlock()
    {
        var input = "<% } else { %>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal("}\nelse\n{", result);
    }

    [Fact]
    public void ConvertsElseIfBlock()
    {
        var input = "<% } else if (count > 0) { %>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal("}\nelse if (count > 0)\n{", result);
    }

    // ──────────────────────────────────────────────────────────────
    // foreach / for / while
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void ConvertsForeachBlock()
    {
        var input = "<% foreach (var item in Items) { %>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal("@foreach (var item in Items)\n{", result);
    }

    [Fact]
    public void ConvertsForBlock()
    {
        var input = "<% for (int i = 0; i < 10; i++) { %>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal("@for (int i = 0; i < 10; i++)\n{", result);
    }

    [Fact]
    public void ConvertsWhileBlock()
    {
        var input = "<% while (hasMore) { %>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal("@while (hasMore)\n{", result);
    }

    // ──────────────────────────────────────────────────────────────
    // Bare statement block
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void ConvertsBareStatementBlock()
    {
        var input = "<% var x = GetValue(); %>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal("@{ var x = GetValue(); }", result);
    }

    // ──────────────────────────────────────────────────────────────
    // Multi-line blocks (WingtipToys pattern where { is on next line)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void ConvertsMultilineIfBlock_WingtipToysPattern()
    {
        // From Manage.aspx lines 61-62: <% if (TwoFactorEnabled)\n  { %>
        var input = "<% if (TwoFactorEnabled)\r\n  { %>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal("@if (TwoFactorEnabled)\n{", result);
    }

    [Fact]
    public void ConvertsMultilineElseBlock_WingtipToysPattern()
    {
        // From Manage.aspx: <% }\n  else\n  { %>
        var input = "<%\r\n  }\r\n  else\r\n  { %>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal("}\nelse\n{", result);
    }

    [Fact]
    public void ConvertsMultilineClosingBrace_WingtipToysPattern()
    {
        // <% \n  } \n  %>  (whitespace around the closing brace)
        var input = "<%\r\n  }\r\n%>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal("}", result);
    }

    // ──────────────────────────────────────────────────────────────
    // Full WingtipToys if/else pattern (multiple blocks in sequence)
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void ConvertsFullIfElseSequence()
    {
        var input = """
            <% if (TwoFactorEnabled)
              { %>
            <p>Enabled</p>
            <% }
              else
              { %>
            <p>Not enabled</p>
            <% } %>
            """;

        var result = _transform.Apply(input, TestMeta);

        Assert.Contains("@if (TwoFactorEnabled)", result);
        Assert.Contains("}\nelse\n{", result);
        // Closing brace must appear
        Assert.Contains("\n}", result);
        // No raw <% %> should remain
        Assert.DoesNotContain("<%", result);
        Assert.DoesNotContain("%>", result);
    }

    // ──────────────────────────────────────────────────────────────
    // Comment sanitization — <% %> inside @* *@ must be neutralized
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void SanitizesCodeBlocksInsideRazorComment()
    {
        // ExpressionTransform already converted <%-- --%> to @* *@;
        // inner <% %> should be neutralized to [% %] so Razor doesn't parse them.
        var input = "@*\n<% if (HasPhoneNumber) { %>\n<dd>...</dd>\n<% } %>\n*@";
        var result = _transform.Apply(input, TestMeta);

        Assert.DoesNotContain("<%", result);
        Assert.DoesNotContain("%>", result);
        Assert.Contains("[%", result);
        Assert.Contains("%]", result);
        // The @* *@ wrapper must still be present
        Assert.StartsWith("@*", result);
        Assert.EndsWith("*@", result);
    }

    [Fact]
    public void DoesNotConvertCodeBlocksInsideComment_TheyAreSanitizedNot_Converted()
    {
        // Blocks inside comments should become [% %] not @if / @foreach etc.
        var input = "@*\n<% if (x) { %>\ncontent\n<% } %>\n*@";
        var result = _transform.Apply(input, TestMeta);

        Assert.DoesNotContain("@if", result);
        Assert.Contains("[% if (x) { %]", result);
    }

    [Fact]
    public void ActiveBlocksOutsideCommentStillConverted_WhenCommentAlsoPresent()
    {
        var input = "@*\n<% if (Hidden) { %>\n*@\n<% if (Visible) { %>\n<p>Hello</p>\n<% } %>";
        var result = _transform.Apply(input, TestMeta);

        // Inside comment: sanitized
        Assert.Contains("[% if (Hidden) { %]", result);
        // Outside comment: converted
        Assert.Contains("@if (Visible)", result);
    }

    // ──────────────────────────────────────────────────────────────
    // Pass-through: non-statement tokens must not be touched
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void DoesNotAffectPlainHtml()
    {
        var input = "<div class=\"container\"><span>Hello</span></div>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal(input, result);
    }
}

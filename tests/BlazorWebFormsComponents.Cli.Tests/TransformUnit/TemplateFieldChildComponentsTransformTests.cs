using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for TemplateFieldChildComponentsTransform — wraps style children
/// of TemplateField in a &lt;ChildComponents&gt; element.
/// </summary>
public class TemplateFieldChildComponentsTransformTests
{
    private readonly TemplateFieldChildComponentsTransform _transform = new();

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
        Assert.Equal(620, _transform.Order);
    }

    [Fact]
    public void HasCorrectName()
    {
        Assert.Equal("TemplateFieldChildComponents", _transform.Name);
    }

    // ──────────────────────────────────────────────────────────────
    // Single style element wrapping
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void WrapsItemStyle_InChildComponents()
    {
        var input = """
            <TemplateField>
                <ItemTemplate>
                    <h3>Shipping Address:</h3>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Left" />
            </TemplateField>
            """;

        var result = _transform.Apply(input, TestMeta);

        Assert.Contains("<ChildComponents>", result);
        Assert.Contains("<ItemStyle HorizontalAlign=\"Left\" />", result);
        Assert.DoesNotContain("<ChildComponents>\n    <ChildComponents>", result);
    }

    [Fact]
    public void WrapsHeaderStyle_InChildComponents()
    {
        var input = "<TemplateField>\n    <HeaderStyle CssClass=\"header\" />\n</TemplateField>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Contains("<ChildComponents>", result);
        Assert.Contains("<HeaderStyle CssClass=\"header\" />", result);
    }

    [Fact]
    public void WrapsFooterStyle_InChildComponents()
    {
        var input = "<TemplateField>\n    <FooterStyle Font-Bold=\"true\" />\n</TemplateField>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Contains("<ChildComponents>", result);
        Assert.Contains("<FooterStyle Font-Bold=\"true\" />", result);
    }

    // ──────────────────────────────────────────────────────────────
    // Multiple style elements grouped into ONE ChildComponents block
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void GroupsMultipleStyleElementsIntoOneChildComponents()
    {
        var input = """
            <TemplateField>
                <ItemTemplate><span>x</span></ItemTemplate>
                <ItemStyle CssClass="a" />
                <HeaderStyle CssClass="b" />
            </TemplateField>
            """;

        var result = _transform.Apply(input, TestMeta);

        // Only one ChildComponents block
        var openCount = CountOccurrences(result, "<ChildComponents>");
        Assert.Equal(1, openCount);

        Assert.Contains("<ItemStyle CssClass=\"a\" />", result);
        Assert.Contains("<HeaderStyle CssClass=\"b\" />", result);
    }

    // ──────────────────────────────────────────────────────────────
    // Already-wrapped styles must not be double-wrapped
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void DoesNotDoubleWrap_WhenChildComponentsAlreadyPresent()
    {
        var input = """
            <TemplateField>
                <ItemTemplate><span>x</span></ItemTemplate>
                <ChildComponents>
                    <ItemStyle CssClass="a" />
                </ChildComponents>
            </TemplateField>
            """;

        var result = _transform.Apply(input, TestMeta);

        var count = CountOccurrences(result, "<ChildComponents>");
        Assert.Equal(1, count);
        Assert.Equal(input, result);
    }

    // ──────────────────────────────────────────────────────────────
    // Template fragments must NOT be moved
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void LeavesItemTemplate_InPlace()
    {
        var input = """
            <TemplateField>
                <ItemTemplate>
                    <span>Hello</span>
                </ItemTemplate>
                <ItemStyle CssClass="x" />
            </TemplateField>
            """;

        var result = _transform.Apply(input, TestMeta);

        // ItemTemplate must still be a direct child of TemplateField (outside ChildComponents)
        Assert.Contains("<ItemTemplate>", result);
        Assert.DoesNotContain("<ChildComponents>\n    <ItemTemplate>", result);
    }

    [Fact]
    public void LeavesHeaderTemplate_InPlace()
    {
        var input = "<TemplateField>\n    <HeaderTemplate><th>Name</th></HeaderTemplate>\n    <HeaderStyle CssClass=\"h\" />\n</TemplateField>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Contains("<HeaderTemplate>", result);
        Assert.Contains("<ChildComponents>", result);
    }

    // ──────────────────────────────────────────────────────────────
    // Self-closing vs open/close style tags
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void WrapsSelfClosingStyleTag()
    {
        var input = "<TemplateField>\n    <ItemStyle Width=\"100px\" />\n</TemplateField>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Contains("<ChildComponents>", result);
        Assert.Contains("<ItemStyle Width=\"100px\" />", result);
    }

    [Fact]
    public void WrapsOpenCloseStyleTag()
    {
        var input = "<TemplateField>\n    <ItemStyle></ItemStyle>\n</TemplateField>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Contains("<ChildComponents>", result);
        Assert.Contains("<ItemStyle></ItemStyle>", result);
    }

    // ──────────────────────────────────────────────────────────────
    // Multiple sibling TemplateField elements — each wrapped independently
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void WrapsSiblingTemplateFieldsIndependently()
    {
        var input = """
            <TemplateField>
                <ItemTemplate><span>A</span></ItemTemplate>
                <ItemStyle CssClass="a" />
            </TemplateField>
            <TemplateField>
                <ItemTemplate><span>B</span></ItemTemplate>
                <HeaderStyle CssClass="b" />
            </TemplateField>
            """;

        var result = _transform.Apply(input, TestMeta);

        var count = CountOccurrences(result, "<ChildComponents>");
        Assert.Equal(2, count);
        Assert.Contains("<ItemStyle CssClass=\"a\" />", result);
        Assert.Contains("<HeaderStyle CssClass=\"b\" />", result);
    }

    // ──────────────────────────────────────────────────────────────
    // Pass-through: no style elements → no change
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void DoesNotModify_WhenNoStyleElements()
    {
        var input = "<TemplateField>\n    <ItemTemplate><span>x</span></ItemTemplate>\n</TemplateField>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Equal(input, result);
    }

    [Fact]
    public void DoesNotModify_WhenNoTemplateField()
    {
        var input = "<GridView ItemType=\"object\">\n    <ItemStyle CssClass=\"x\" />\n</GridView>";
        var result = _transform.Apply(input, TestMeta);

        // ItemStyle outside TemplateField is untouched
        Assert.Equal(input, result);
    }

    // ──────────────────────────────────────────────────────────────
    // All supported style element names
    // ──────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("ItemStyle")]
    [InlineData("HeaderStyle")]
    [InlineData("FooterStyle")]
    [InlineData("ControlStyle")]
    [InlineData("EditItemStyle")]
    [InlineData("AlternatingItemStyle")]
    public void WrapsAllSupportedStyleElementNames(string styleName)
    {
        var input = $"<TemplateField>\n    <{styleName} CssClass=\"x\" />\n</TemplateField>";
        var result = _transform.Apply(input, TestMeta);

        Assert.Contains("<ChildComponents>", result);
        Assert.Contains($"<{styleName} CssClass=\"x\" />", result);
    }

    // ──────────────────────────────────────────────────────────────
    // Indentation: style element should be indented inside ChildComponents
    // ──────────────────────────────────────────────────────────────

    [Fact]
    public void IndentsStyleElement_InsideChildComponents()
    {
        var input = "<TemplateField>\n    <ItemStyle CssClass=\"x\" />\n</TemplateField>";
        var result = _transform.Apply(input, TestMeta);

        // The ItemStyle inside ChildComponents must have more indentation than ChildComponents itself
        var lines = result.Split('\n');
        var childComponentsLine = Array.FindIndex(lines, l => l.Contains("<ChildComponents>"));
        var itemStyleLine = Array.FindIndex(lines, l => l.Contains("<ItemStyle"));

        Assert.True(childComponentsLine >= 0, "ChildComponents line not found");
        Assert.True(itemStyleLine > childComponentsLine, "ItemStyle should be after ChildComponents");

        var childIndent = lines[childComponentsLine].Length - lines[childComponentsLine].TrimStart().Length;
        var styleIndent = lines[itemStyleLine].Length - lines[itemStyleLine].TrimStart().Length;
        Assert.True(styleIndent > childIndent, "ItemStyle should have more indentation than ChildComponents");
    }

    // ──────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────

    private static int CountOccurrences(string source, string target)
    {
        int count = 0, index = 0;
        while ((index = source.IndexOf(target, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += target.Length;
        }
        return count;
    }
}

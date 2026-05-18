using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for TemplatePlaceholderTransform — converts placeholder elements
/// (id containing "Placeholder") inside template blocks to @context.
/// </summary>
public class TemplatePlaceholderTransformTests
{
    private readonly TemplatePlaceholderTransform _transform = new();

    private static FileMetadata TestMetadata => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = ""
    };

    [Fact]
    public void ConvertsSelfClosing_WithPlaceholderId()
    {
        var input = @"<Label id=""itemPlaceholder"" runat=""server"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal("@context", result);
    }

    [Fact]
    public void ConvertsOpenClose_WithPlaceholderId()
    {
        var input = @"<tr id=""itemPlaceholder"" runat=""server""></tr>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal("@context", result);
    }

    [Fact]
    public void ConvertsOpenClose_WithWhitespaceContent()
    {
        var input = @"<div id=""itemPlaceholder"" runat=""server"">  </div>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal("@context", result);
    }

    [Fact]
    public void Handles_UppercasePlaceholder()
    {
        var input = @"<span id=""ItemPlaceholder"" runat=""server"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal("@context", result);
    }

    [Fact]
    public void Handles_PlaceholderInMiddleOfId()
    {
        var input = @"<tr id=""myPlaceholderRow"" runat=""server"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal("@context", result);
    }

    [Fact]
    public void PreservesContent_WithoutPlaceholderId()
    {
        var input = @"<div id=""content"" class=""wrapper""><span>Hello</span></div>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void PreservesContent_WhenNoIdAttribute()
    {
        var input = @"<div class=""container""><p>Text</p></div>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void MultiplePlaceholders_AllConverted()
    {
        var input = @"<tr id=""itemPlaceholder"" runat=""server"" />
<tr id=""groupPlaceholder"" runat=""server"" />";
        var result = _transform.Apply(input, TestMetadata);

        var lines = result.Split('\n');
        Assert.Equal("@context", lines[0].Trim());
        Assert.Equal("@context", lines[1].Trim());
    }

    [Fact]
    public void OrderIs800()
    {
        Assert.Equal(800, _transform.Order);
    }
}

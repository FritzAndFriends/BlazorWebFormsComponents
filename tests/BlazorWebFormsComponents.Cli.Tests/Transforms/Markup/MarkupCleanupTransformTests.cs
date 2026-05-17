using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.Markup;

public class MarkupCleanupTransformTests
{
    private readonly MarkupCleanupTransform _sut = new();

    private static FileMetadata MakeMetadata() => new()
    {
        SourceFilePath = "Pages/Test.aspx",
        OutputFilePath = "Pages/Test.razor",
        FileType = FileType.Page,
        OriginalContent = string.Empty
    };

    [Fact]
    public void UnclosedBoldTag_IsClosed()
    {
        const string input = "<b>Add To Cart<b>";

        var result = _sut.Apply(input, MakeMetadata());

        result.ShouldContain("<b>Add To Cart</b>");
    }

    [Fact]
    public void SelfClosingTd_IsExpanded()
    {
        const string input = "<td/>";

        var result = _sut.Apply(input, MakeMetadata());

        result.ShouldBe("<td></td>");
    }

    [Fact]
    public void SelfClosingBr_NotExpanded()
    {
        const string input = "<br/>";

        var result = _sut.Apply(input, MakeMetadata());

        result.ShouldBe("<br/>");
    }

    [Fact]
    public void MissingChildCloser_IsInsertedBeforeParentClose()
    {
        const string input = "<UpdatePanel>\n    <div id=\"autoComplete\">\n        <DetailsView></DetailsView>\n</UpdatePanel>";

        var result = _sut.Apply(input, MakeMetadata());

        result.ShouldContain("</div>");
        result.IndexOf("</div>", StringComparison.Ordinal).ShouldBeLessThan(result.IndexOf("</UpdatePanel>", StringComparison.Ordinal));
    }
}

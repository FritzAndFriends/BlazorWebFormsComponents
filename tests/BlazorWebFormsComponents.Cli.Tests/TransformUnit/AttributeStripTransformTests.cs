using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for AttributeStripTransform — removes server-side ASP.NET attributes.
/// Corresponds to TC02-AttributeStrip test case.
/// </summary>
public class AttributeStripTransformTests
{
    private readonly AttributeStripTransform _transform = new();

    private static FileMetadata TestMetadata => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = string.Empty
    };

    [Fact]
    public void StripsRunatServer()
    {
        var result = _transform.Apply(@"<div runat=""server""><span>Content</span></div>", TestMetadata);

        Assert.Equal(@"<div><span>Content</span></div>", result);
    }

    [Fact]
    public void StripsEnableViewState()
    {
        var result = _transform.Apply(@"<div EnableViewState=""true"" runat=""server"">", TestMetadata);

        Assert.DoesNotContain("EnableViewState", result);
        Assert.DoesNotContain("runat", result);
    }

    [Fact]
    public void StripsMultipleServerAttributes()
    {
        var result = _transform.Apply(@"<div runat=""server"" EnableViewState=""true"" ViewStateMode=""Enabled"" ValidateRequest=""false"" MaintainScrollPositionOnPostBack=""true"" ClientIDMode=""Static"">", TestMetadata);

        Assert.Equal(@"<div>", result);
    }

    [Fact]
    public void PreservesItemTypeInsteadOfConvertingToTItem()
    {
        var result = _transform.Apply(@"<GridView ItemType=""TestApp.Models.Product"" runat=""server"" />", TestMetadata);

        Assert.Contains(@"ItemType=""Product""", result);
        Assert.DoesNotContain("TItem=", result);
    }
}

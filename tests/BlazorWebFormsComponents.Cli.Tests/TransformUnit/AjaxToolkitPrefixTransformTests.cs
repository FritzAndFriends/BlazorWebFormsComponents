using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for AjaxToolkitPrefixTransform — converts ajaxToolkit: prefix controls.
/// Known controls get prefix stripped; unknown controls are replaced with TODO comments.
/// ToolkitScriptManager is removed entirely.
/// </summary>
public class AjaxToolkitPrefixTransformTests
{
    private readonly AjaxToolkitPrefixTransform _transform = new();

    private static FileMetadata TestMetadata => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = ""
    };

    [Fact]
    public void StripsKnownControl_Accordion()
    {
        var input = @"<ajaxToolkit:Accordion ID=""acc1"" runat=""server""></ajaxToolkit:Accordion>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<Accordion", result);
        Assert.Contains("</Accordion>", result);
        Assert.DoesNotContain("ajaxToolkit:", result);
    }

    [Fact]
    public void StripsKnownControl_TabContainer()
    {
        var input = @"<ajaxToolkit:TabContainer ID=""tabs"" runat=""server""></ajaxToolkit:TabContainer>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<TabContainer", result);
        Assert.Contains("</TabContainer>", result);
        Assert.DoesNotContain("ajaxToolkit:", result);
    }

    [Fact]
    public void StripsKnownControl_CalendarExtender()
    {
        var input = @"<ajaxToolkit:CalendarExtender TargetControlID=""txtDate"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.DoesNotContain("ajaxToolkit:", result);
        Assert.DoesNotContain("TODO", result);
    }

    [Fact]
    public void StripsKnownControl_AccordionPane()
    {
        var input = @"<ajaxToolkit:AccordionPane><Header>H</Header></ajaxToolkit:AccordionPane>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<AccordionPane>", result);
        Assert.Contains("</AccordionPane>", result);
        Assert.DoesNotContain("ajaxToolkit:", result);
    }

    [Fact]
    public void StripsKnownControl_ConfirmButtonExtender()
    {
        var input = @"<ajaxToolkit:ConfirmButtonExtender TargetControlID=""btn1"" ConfirmText=""Sure?"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.DoesNotContain("ajaxToolkit:", result);
        Assert.DoesNotContain("TODO", result);
    }

    [Fact]
    public void RemovesToolkitScriptManager_SelfClosing()
    {
        var input = @"<ajaxToolkit:ToolkitScriptManager ID=""tsm1"" runat=""server"" />
<div>Content</div>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.DoesNotContain("ToolkitScriptManager", result);
        Assert.Contains("<div>Content</div>", result);
    }

    [Fact]
    public void RemovesToolkitScriptManager_BlockForm()
    {
        var input = @"<ajaxToolkit:ToolkitScriptManager ID=""tsm1"" runat=""server"">
</ajaxToolkit:ToolkitScriptManager>
<div>Content</div>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.DoesNotContain("ToolkitScriptManager", result);
        Assert.Contains("<div>Content</div>", result);
    }

    [Fact]
    public void UnknownSelfClosing_ReplacesWithTodo()
    {
        var input = @"<ajaxToolkit:RatingExtender TargetControlID=""r1"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("TODO(bwfc-ajax-toolkit)", result);
        Assert.Contains("RatingExtender", result);
        Assert.DoesNotContain("<ajaxToolkit:", result);
    }

    [Fact]
    public void UnknownBlock_ReplacesWithTodo()
    {
        var input = @"<ajaxToolkit:DragPanelExtender ID=""dp1"">inner</ajaxToolkit:DragPanelExtender>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("TODO(bwfc-ajax-toolkit)", result);
        Assert.Contains("DragPanelExtender", result);
        Assert.DoesNotContain("<ajaxToolkit:", result);
    }

    [Fact]
    public void PreservesNonAjaxToolkitContent()
    {
        var input = @"<div class=""container""><asp:Button Text=""Go"" /></div>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void MultipleKnownControls_AllStripped()
    {
        var input = @"<ajaxToolkit:TabContainer>
    <ajaxToolkit:TabPanel HeaderText=""Tab1""></ajaxToolkit:TabPanel>
    <ajaxToolkit:TabPanel HeaderText=""Tab2""></ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.DoesNotContain("ajaxToolkit:", result);
        Assert.Contains("<TabContainer>", result);
        Assert.Contains("<TabPanel", result);
        Assert.Contains("</TabPanel>", result);
        Assert.Contains("</TabContainer>", result);
    }

    [Fact]
    public void MixedKnownAndUnknown_HandledCorrectly()
    {
        var input = @"<ajaxToolkit:Accordion ID=""a1""></ajaxToolkit:Accordion>
<ajaxToolkit:ReorderList ID=""rl1"">items</ajaxToolkit:ReorderList>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<Accordion", result);
        Assert.Contains("</Accordion>", result);
        Assert.Contains("TODO(bwfc-ajax-toolkit)", result);
        Assert.Contains("ReorderList", result);
    }

    [Fact]
    public void OrderIs600()
    {
        Assert.Equal(600, _transform.Order);
    }
}

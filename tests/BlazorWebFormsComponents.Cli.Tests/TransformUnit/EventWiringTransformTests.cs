using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for EventWiringTransform — converts event handler attributes
/// to Blazor @-prefixed syntax: OnClick="Handler" → OnClick="@Handler".
/// </summary>
public class EventWiringTransformTests
{
    private readonly EventWiringTransform _transform = new();

    private static FileMetadata TestMetadata => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = ""
    };

    [Fact]
    public void WiresOnClick()
    {
        var input = @"<Button OnClick=""btnSubmit_Click"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"OnClick=""@btnSubmit_Click""", result);
    }

    [Fact]
    public void WiresOnSelectedIndexChanged()
    {
        var input = @"<DropDownList OnSelectedIndexChanged=""ddl_Changed"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"OnSelectedIndexChanged=""@ddl_Changed""", result);
    }

    [Fact]
    public void WiresOnTextChanged()
    {
        var input = @"<TextBox OnTextChanged=""txt_Changed"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"OnTextChanged=""@txt_Changed""", result);
    }

    [Fact]
    public void WiresOnRowCommand()
    {
        var input = @"<GridView OnRowCommand=""gv_RowCommand"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"OnRowCommand=""@gv_RowCommand""", result);
    }

    [Fact]
    public void WiresOnCheckedChanged()
    {
        var input = @"<CheckBox OnCheckedChanged=""chk_Changed"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"OnCheckedChanged=""@chk_Changed""", result);
    }

    [Fact]
    public void WiresOnServerValidate()
    {
        var input = @"<CustomValidator OnServerValidate=""cv_Validate"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"OnServerValidate=""@cv_Validate""", result);
    }

    [Fact]
    public void DoesNotDoubleWire_AlreadyPrefixed()
    {
        var input = @"<Button OnClick=""@btnSubmit_Click"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"OnClick=""@btnSubmit_Click""", result);
        Assert.DoesNotContain(@"OnClick=""@@", result);
    }

    [Fact]
    public void MultipleEventsOnSameElement()
    {
        var input = @"<GridView OnRowCommand=""gv_Cmd"" OnRowDataBound=""gv_DataBound"" OnSorting=""gv_Sort"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"OnRowCommand=""@gv_Cmd""", result);
        Assert.Contains(@"OnRowDataBound=""@gv_DataBound""", result);
        Assert.Contains(@"OnSorting=""@gv_Sort""", result);
    }

    [Fact]
    public void PreservesContent_WithNoEventAttributes()
    {
        var input = @"<div class=""wrapper""><Label Text=""Hello"" /></div>";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void PreservesNonEventAttributes()
    {
        var input = @"<Button Text=""Submit"" CssClass=""btn"" OnClick=""Submit_Click"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"Text=""Submit""", result);
        Assert.Contains(@"CssClass=""btn""", result);
        Assert.Contains(@"OnClick=""@Submit_Click""", result);
    }

    [Fact]
    public void WiresOnPageIndexChanging()
    {
        var input = @"<GridView OnPageIndexChanging=""gv_Paging"" />";
        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains(@"OnPageIndexChanging=""@gv_Paging""", result);
    }

    [Fact]
    public void OrderIs710()
    {
        Assert.Equal(710, _transform.Order);
    }
}

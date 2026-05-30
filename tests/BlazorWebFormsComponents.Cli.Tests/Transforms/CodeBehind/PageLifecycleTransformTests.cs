using BlazorWebFormsComponents.Cli.Analysis;
using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class PageLifecycleTransformTests
{
    private readonly PageLifecycleTransform _transform = new();

    [Fact]
    public void Apply_ControlOnLoadOverride_ConvertsToOnParametersSet()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "MyControl.ascx",
            OutputFilePath = "MyControl.razor",
            FileType = FileType.Control,
            OriginalContent = "",
            AscxDescriptor = new AscxDescriptor
            {
                ControlName = "MyControl",
                HasPageLoadOverride = true
            }
        };

        var input = """
            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                Prepare();
            }
            """;

        var result = _transform.Apply(input, metadata);

        Assert.Contains("protected override void OnParametersSet()", result);
        Assert.Contains("base.OnParametersSet();", result);
        Assert.DoesNotContain("OnLoad(EventArgs e)", result);
    }

    [Fact]
    public void Apply_PageStillConvertsPageLoad_ToOnInitializedAsync()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "Default.aspx",
            OutputFilePath = "Default.razor",
            FileType = FileType.Page,
            OriginalContent = ""
        };

        var input = """
            protected void Page_Load(object sender, EventArgs e)
            {
                LoadData();
            }
            """;

        var result = _transform.Apply(input, metadata);

        Assert.Contains("protected override async Task OnInitializedAsync()", result);
        Assert.Contains("await base.OnInitializedAsync();", result);
    }
}

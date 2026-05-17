using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class WebMethodAnnotationTransformTests
{
    private readonly WebMethodAnnotationTransform _transform = new();

    private static FileMetadata TestMetadata => new()
    {
        SourceFilePath = "Students.aspx.cs",
        OutputFilePath = "Students.razor.cs",
        FileType = FileType.CodeFile,
        OriginalContent = string.Empty
    };

    [Fact]
    public void HasExpectedMetadata()
    {
        Assert.Equal("WebMethodAnnotation", _transform.Name);
        Assert.Equal(550, _transform.Order);
    }

    [Fact]
    public void AddsTodoAndRemovesLegacySystemWebWebMethodAttributes()
    {
        var input = """
            [System.Web.Services.WebMethod]
            [System.Web.Script.Services.ScriptMethod]
            public static List<string> GetCompletionList(string prefixText, int count)
            {
                return new List<string>();
            }
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("TODO(bwfc-webmethod)", result);
        Assert.Contains("Legacy [WebMethod] attribute removed", result);
        Assert.Contains("Legacy [ScriptMethod] attribute removed", result);
        Assert.DoesNotContain("System.Web", result);
    }

    [Fact]
    public void RenamesPagePreRenderCompleteForLifecycleTransform()
    {
        var input = """
            protected void Page_PreRenderComplete(object sender, EventArgs e)
            {
                grv.DataBind();
            }
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("Page_PreRender", result);
        Assert.DoesNotContain("Page_PreRenderComplete", result);
    }
}

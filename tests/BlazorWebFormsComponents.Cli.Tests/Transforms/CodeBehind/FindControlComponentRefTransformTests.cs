using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Tests;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

/// <summary>
/// Verifies that FindControl calls are preserved through the pipeline (not rewritten).
/// FindControl is a BWFC runtime feature — the CLI should leave these calls unchanged.
/// </summary>
public class FindControlPreservationTests
{
    [Fact]
    public void Pipeline_PreservesFindControlCalls()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var metadata = new FileMetadata
        {
            SourceFilePath = "Control.ascx",
            OutputFilePath = "Control.razor",
            FileType = FileType.Control,
            OriginalContent = "",
        };
        metadata.ComponentRefs["SearchBox"] = "TextBox";
        var input = """var box = (TextBox)FindControl("SearchBox");""";

        var result = pipeline.TransformCodeBehind(input, metadata);

        Assert.Contains("""FindControl("SearchBox")""", result);
    }

    [Fact]
    public void Pipeline_PreservesThisFindControlCalls()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var metadata = new FileMetadata
        {
            SourceFilePath = "Control.ascx",
            OutputFilePath = "Control.razor",
            FileType = FileType.Control,
            OriginalContent = "",
        };
        metadata.ComponentRefs["SearchBox"] = "TextBox";
        var input = """var box = this.FindControl("SearchBox");""";

        var result = pipeline.TransformCodeBehind(input, metadata);

        Assert.Contains("""FindControl("SearchBox")""", result);
    }

    [Fact]
    public void Pipeline_PreservesChainedFindControlCalls()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var metadata = new FileMetadata
        {
            SourceFilePath = "Default.aspx",
            OutputFilePath = "Default.razor",
            FileType = FileType.Page,
            OriginalContent = "",
        };
        var input = """var child = FindControl("Panel1").FindControl("TextBox1");""";

        var result = pipeline.TransformCodeBehind(input, metadata);

        Assert.Contains("""FindControl("Panel1").FindControl("TextBox1")""", result);
    }
}

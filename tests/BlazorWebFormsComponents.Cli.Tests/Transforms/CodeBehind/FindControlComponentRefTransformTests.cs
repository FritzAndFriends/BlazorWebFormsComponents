using BlazorWebFormsComponents.Cli.Analysis;
using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class FindControlComponentRefTransformTests
{
    private readonly FindControlComponentRefTransform _transform = new();

    private static FileMetadata CreateMetadata() => new()
    {
        SourceFilePath = "Control.ascx",
        OutputFilePath = "Control.razor",
        FileType = FileType.Control,
        OriginalContent = "",
        AscxDescriptor = new AscxDescriptor
        {
            ControlName = "Control",
            ReferencedControlIds = ["SearchBox"]
        }
    };

    [Fact]
    public void Apply_RewritesDirectFindControl_WhenRefExists()
    {
        var metadata = CreateMetadata();
        metadata.ComponentRefs["SearchBox"] = "TextBox";
        var input = """var box = FindControl("SearchBox");""";

        var result = _transform.Apply(input, metadata);

        Assert.Equal("var box = SearchBox;", result);
    }

    [Fact]
    public void Apply_RewritesThisFindControl_WhenRefExists()
    {
        var metadata = CreateMetadata();
        metadata.ComponentRefs["SearchBox"] = "TextBox";
        var input = """var box = this.FindControl("SearchBox");""";

        var result = _transform.Apply(input, metadata);

        Assert.Equal("var box = SearchBox;", result);
    }

    [Fact]
    public void Apply_DoesNotRewriteUnknownControl()
    {
        var metadata = CreateMetadata();
        metadata.ComponentRefs["Known"] = "TextBox";
        var input = """var box = FindControl("Unknown");""";

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void Apply_DoesNotRewriteWhenNoAnalysisReferences()
    {
        var metadata = CreateMetadata();
        metadata.AscxDescriptor = new AscxDescriptor
        {
            ControlName = "Control",
            ReferencedControlIds = []
        };
        metadata.ComponentRefs["SearchBox"] = "TextBox";
        var input = """var box = FindControl("SearchBox");""";

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
    }
}

using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class CompiledCodeBehindStubPipelineTests
{
    [Fact]
    public void Pipeline_UsesTransformedMarkup_WhenGeneratingMissingMemberStubs()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var markup = "<asp:Button ID=\"SubmitButton\" runat=\"server\" OnClick=\"SubmitOrder\" />\n<p>@FormatTotal()</p>\n<div>@_orderTotal</div>";
        var codeBehind = "namespace TestApp;\n\npublic partial class CheckoutReview\n{\n}";
        var metadata = new FileMetadata
        {
            SourceFilePath = "CheckoutReview.aspx",
            OutputFilePath = "CheckoutReview.razor.cs",
            FileType = FileType.Page,
            OriginalContent = markup,
            CodeBehindContent = codeBehind
        };

        var transformedMarkup = pipeline.TransformMarkup(markup, metadata);
        var transformedCodeBehind = pipeline.TransformCodeBehind(codeBehind, metadata);

        Assert.Contains("OnClick=\"@SubmitOrder\"", transformedMarkup);
        Assert.Contains("protected void SubmitOrder(object? sender, EventArgs e)", transformedCodeBehind);
        Assert.Contains("protected object? FormatTotal()", transformedCodeBehind);
        Assert.Contains("private object? _orderTotal;", transformedCodeBehind);
    }
}

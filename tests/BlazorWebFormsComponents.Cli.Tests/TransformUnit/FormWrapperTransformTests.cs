using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for FormWrapperTransform — converts server form elements to WebFormsForm.
/// </summary>
public class FormWrapperTransformTests
{
    private readonly FormWrapperTransform _transform = new();

    private static FileMetadata TestMetadata() => new()
    {
        SourceFilePath = "Default.aspx",
        OutputFilePath = "Default.razor",
        FileType = FileType.Page,
        OriginalContent = ""
    };

    [Fact]
    public void ConvertsFormToWebFormsForm()
    {
        var input = @"<form id=""form1"" runat=""server"">content</form>";
        var result = _transform.Apply(input, TestMetadata());

        Assert.Contains("<WebFormsForm", result);
        Assert.Contains("</WebFormsForm>", result);
        Assert.DoesNotContain("<div", result);
        Assert.DoesNotContain("<form", result);
    }

    [Fact]
    public void PreservesFormId()
    {
        var input = @"<form id=""form1"" runat=""server"">content</form>";
        var result = _transform.Apply(input, TestMetadata());

        Assert.Contains(@"id=""form1""", result);
    }

    [Fact]
    public void HandlesFormWithoutId()
    {
        var input = @"<form runat=""server"">content</form>";
        var result = _transform.Apply(input, TestMetadata());

        Assert.Equal("<WebFormsForm>content</WebFormsForm>", result);
    }

    [Fact]
    public void IgnoresNonServerForms()
    {
        var input = @"<form action=""/search"">content</form>";
        var result = _transform.Apply(input, TestMetadata());

        Assert.Equal(input, result);
    }

    [Fact]
    public void HandlesMultilineForm()
    {
        var input = "<form id=\"form1\"\n    runat=\"server\">\n  <div>content</div>\n</form>";
        var result = _transform.Apply(input, TestMetadata());

        Assert.Contains("<WebFormsForm id=\"form1\">", result);
        Assert.Contains("</WebFormsForm>", result);
    }

    [Fact]
    public void OrderIs310()
    {
        Assert.Equal(310, _transform.Order);
    }
}

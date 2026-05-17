using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class SsrFormContractTransformTests
{
    private readonly SsrFormContractTransform _transform = new();

    private static FileMetadata TestMetadata(string outputFile = "Students.razor") => new()
    {
        SourceFilePath = Path.ChangeExtension(outputFile, ".aspx"),
        OutputFilePath = outputFile,
        FileType = FileType.Page,
        OriginalContent = string.Empty
    };

    [Fact]
    public void Apply_PostForm_AddsAntiforgeryTokenAndFormName()
    {
        var input = "<form method=\"post\">\n    <input name=\"q\" />\n</form>";

        var result = _transform.Apply(input, TestMetadata());

        Assert.Contains("@formname=\"StudentsForm\"", result);
        Assert.Contains("<AntiforgeryToken />", result);
        Assert.True(result.IndexOf("<AntiforgeryToken />", StringComparison.Ordinal) < result.IndexOf("<input", StringComparison.Ordinal));
    }

    [Fact]
    public void Apply_EditForm_AddsFormName()
    {
        var input = "<EditForm Model=\"@Model\">\n    <InputText @bind-Value=\"Model.Name\" />\n</EditForm>";

        var result = _transform.Apply(input, TestMetadata("EditStudent.razor"));

        Assert.Contains("FormName=\"EditStudentForm\"", result);
        Assert.Contains("<AntiforgeryToken />", result);
    }

    [Fact]
    public void Apply_WebFormsForm_AddsAntiforgeryTokenAndFormName()
    {
        var input = "<WebFormsForm id=\"form1\">\n    <input name=\"UserName\" />\n</WebFormsForm>";

        var result = _transform.Apply(input, TestMetadata("Login.razor"));

        Assert.Contains("@formname=\"LoginForm\"", result);
        Assert.Contains("<AntiforgeryToken />", result);
    }

    [Fact]
    public void Apply_ExistingAntiforgeryToken_DoesNotDuplicate()
    {
        var input = "<form method=\"post\" @formname=\"Existing\">\n    <AntiforgeryToken />\n    <input name=\"q\" />\n</form>";

        var result = _transform.Apply(input, TestMetadata());

        Assert.Equal(1, CountOccurrences(result, "<AntiforgeryToken />"));
        Assert.Equal(1, CountOccurrences(result, "@formname=\"Existing\""));
    }

    [Fact]
    public void Apply_WithoutForms_ReturnsUnchanged()
    {
        var input = "<div><p>No forms here</p></div>";

        var result = _transform.Apply(input, TestMetadata());

        Assert.Equal(input, result);
    }

    private static int CountOccurrences(string input, string token) =>
        input.Split(token, StringSplitOptions.None).Length - 1;
}

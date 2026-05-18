using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.CodeBehind;

public class EventHandlerSignatureTransformTests
{
    private readonly EventHandlerSignatureTransform _transform = new();
    private static FileMetadata MakeMetadata() => new() { SourceFilePath = "Test.aspx.cs", OutputFilePath = "Test.razor.cs", FileType = FileType.CodeFile, OriginalContent = "" };

    [Fact]
    public void Apply_StandardEventArgs_KeepsEventArgsParam()
    {
        var input = "protected void UpdateBtn_Click(object sender, EventArgs e)\n{\n}";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("protected void UpdateBtn_Click(EventArgs e)", result);
        Assert.DoesNotContain("object sender", result);
    }

    [Fact]
    public void Apply_ImageClickEventArgs_NormalizesToEventArgs()
    {
        var input = "protected void CheckoutBtn_Click(object sender, ImageClickEventArgs e)\n{\n}";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("protected void CheckoutBtn_Click(EventArgs e)", result);
        Assert.DoesNotContain("ImageClickEventArgs", result);
    }

    [Fact]
    public void Apply_CommandEventArgs_KeepsSpecialized()
    {
        var input = "protected void Repeater_ItemCommand(object sender, CommandEventArgs e)\n{\n}";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("protected void Repeater_ItemCommand(CommandEventArgs e)", result);
        Assert.DoesNotContain("object sender", result);
    }

    [Fact]
    public void Apply_PageLoad_KeepsEventArgsParam()
    {
        var input = "protected void Page_Load(object sender, EventArgs e)\n{\n}";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("protected void Page_Load(EventArgs e)", result);
    }

    [Fact]
    public void Apply_MultipleHandlers_TransformsAll()
    {
        var input = """
            protected void Btn1_Click(object sender, EventArgs e) { }
            protected void Btn2_Click(object sender, ImageClickEventArgs e) { }
            protected void Cmd_Command(object sender, CommandEventArgs e) { }
            """;
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("Btn1_Click(EventArgs e)", result);
        Assert.Contains("Btn2_Click(EventArgs e)", result);
        Assert.Contains("Cmd_Command(CommandEventArgs e)", result);
        Assert.DoesNotContain("object sender", result);
    }

    [Fact]
    public void Apply_NoHandlers_ReturnsUnchanged()
    {
        var input = "public void DoWork(string name) { }";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Equal(input, result);
    }

    [Fact]
    public void Apply_AsyncHandler_TransformsCorrectly()
    {
        var input = "protected async Task LoadData_Click(object sender, EventArgs e)\n{\n}";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("protected async Task LoadData_Click(EventArgs e)", result);
    }
}

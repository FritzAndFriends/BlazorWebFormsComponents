using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;
using Xunit;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.Markup;

public class ColorAttributeTransformTests
{
    private readonly ColorAttributeTransform _transform = new();
    private static FileMetadata MakeMetadata() => new() { SourceFilePath = "Test.aspx", OutputFilePath = "Test.razor", FileType = FileType.Page, OriginalContent = "" };

    [Fact]
    public void Apply_TransparentBackColor_WrapsAsStringExpression()
    {
        var input = """<ImageButton BackColor="Transparent" BorderWidth="0" />""";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("""BackColor='@("Transparent")'""", result);
        Assert.Contains("""BorderWidth="0" """, result);
    }

    [Fact]
    public void Apply_ForeColorRed_WrapsAsStringExpression()
    {
        var input = """<Label ForeColor="Red" Text="Error" />""";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("""ForeColor='@("Red")'""", result);
    }

    [Fact]
    public void Apply_BorderColor_WrapsAsStringExpression()
    {
        var input = """<Panel BorderColor="Navy" />""";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("""BorderColor='@("Navy")'""", result);
    }

    [Fact]
    public void Apply_AlreadyWebColorRef_LeavesUnchanged()
    {
        var input = """<Panel BackColor="WebColor.Transparent" />""";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Equal(input, result);
    }

    [Fact]
    public void Apply_NumericValue_LeavesUnchanged()
    {
        var input = """<Panel Width="100" />""";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Equal(input, result);
    }

    [Fact]
    public void Apply_NonColorAttribute_LeavesUnchanged()
    {
        var input = """<Label Text="Transparent" />""";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Equal(input, result);
    }

    [Fact]
    public void Apply_HexColor_WrapsAsStringExpression()
    {
        // Hex values like #FF0000 must be wrapped to avoid Razor treating # as a preprocessor directive
        var input = """<Panel BackColor="#FF0000" />""";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("""BackColor='@("#FF0000")'""", result);
    }

    [Fact]
    public void Apply_ShortHexColor_WrapsAsStringExpression()
    {
        var input = """<Panel ForeColor="#333" />""";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("""ForeColor='@("#333")'""", result);
    }

    [Fact]
    public void Apply_HexColor_BorderColor_WrapsAsStringExpression()
    {
        var input = """<GridView BorderColor="#3366CC" />""";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("""BorderColor='@("#3366CC")'""", result);
    }

    [Fact]
    public void Apply_MultipleColorAttributes_TransformsBoth()
    {
        var input = """<Panel BackColor="White" ForeColor="Black" />""";
        var result = _transform.Apply(input, MakeMetadata());
        Assert.Contains("""BackColor='@("White")'""", result);
        Assert.Contains("""ForeColor='@("Black")'""", result);
    }
}

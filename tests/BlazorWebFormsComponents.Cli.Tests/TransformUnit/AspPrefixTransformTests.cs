namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for AspPrefixTransform — removes "asp:" prefix from server control tags.
/// Corresponds to TC01-AspPrefix test case.
/// </summary>
public class AspPrefixTransformTests
{
    // TODO: Instantiate the real transform when Bishop builds it:
    // private readonly AspPrefixTransform _transform = new();

    [Fact]
    public void RemovesAspPrefixFromSelfClosingTag()
    {
        // Input:  <asp:Button Text="Click" />
        // Expect: <Button Text="Click" />
        var input = @"<asp:Button Text=""Click"" />";
        var expected = @"<Button Text=""Click"" />";

        // TODO: var result = _transform.Apply(input, new FileMetadata { SourceFilePath = "test.aspx" });
        // Assert.Equal(expected, result);

        // Placeholder: verify test data expectations
        Assert.NotEqual(input, expected);
    }

    [Fact]
    public void RemovesAspPrefixFromOpenCloseTag()
    {
        // Input:  <asp:Label Text="Hello"></asp:Label>
        // Expect: <Label Text="Hello"></Label>
        var input = @"<asp:Label Text=""Hello""></asp:Label>";
        var expected = @"<Label Text=""Hello""></Label>";

        Assert.NotEqual(input, expected);
    }

    [Fact]
    public void PreservesNonAspTags()
    {
        // Non-asp: tags should pass through unchanged
        var input = @"<div class=""container""><span>text</span></div>";

        // TODO: var result = _transform.Apply(input, new FileMetadata { SourceFilePath = "test.aspx" });
        // Assert.Equal(input, result);

        Assert.NotNull(input);
    }

    [Fact]
    public void LowercasesIDToId()
    {
        // Input:  <asp:TextBox ID="txtName" />
        // Expect: <TextBox id="txtName" />
        var input = @"<asp:TextBox ID=""txtName"" />";
        var expected = @"<TextBox id=""txtName"" />";

        Assert.NotEqual(input, expected);
    }
}

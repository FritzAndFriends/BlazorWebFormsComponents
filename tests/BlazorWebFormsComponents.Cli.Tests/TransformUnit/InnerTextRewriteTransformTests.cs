using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class InnerTextRewriteTransformTests
{
    private readonly InnerTextRewriteTransform _transform = new();

    [Fact]
    public void RewritesInnerTextAssignment()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "ShoppingCart.aspx.cs",
            OutputFilePath = "ShoppingCart.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty
        };

        var input = @"namespace TestApp;

public partial class ShoppingCart
{
    private void UpdateCart()
    {
        ShoppingCartTitle.InnerText = ""Shopping Cart is Empty"";
    }
}";

        var result = _transform.Apply(input, metadata);

        Assert.Contains("ShoppingCartTitle = \"Shopping Cart is Empty\"", result);
        Assert.DoesNotContain(".InnerText", result);
    }

    [Fact]
    public void RewritesInnerHtmlAssignment()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "Default.aspx.cs",
            OutputFilePath = "Default.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty
        };

        var input = @"namespace TestApp;

public partial class Default
{
    private void Load()
    {
        cartCount.InnerHtml = cartStr;
    }
}";

        var result = _transform.Apply(input, metadata);

        Assert.Contains("cartCount = cartStr", result);
        Assert.DoesNotContain(".InnerHtml", result);
    }

    [Fact]
    public void DoesNotModifyContent_WithoutInnerTextOrInnerHtml()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "About.aspx.cs",
            OutputFilePath = "About.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty
        };

        var input = @"namespace TestApp;

public partial class About
{
    private string title = ""About"";
}";

        var result = _transform.Apply(input, metadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void InjectsStringFieldStub_ForPascalCaseHtmlServerControlId()
    {
        // Verifies the WingtipToys ShoppingCartTitle pattern:
        // <div id="ShoppingCartTitle" runat="server"> → ShoppingCartTitle.InnerText = "..."
        // After InnerText rewrite → ShoppingCartTitle = "..." but ShoppingCartTitle undeclared.
        var metadata = new FileMetadata
        {
            SourceFilePath = "ShoppingCart.aspx.cs",
            OutputFilePath = "ShoppingCart.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty
        };

        var input = @"namespace TestApp;

public partial class ShoppingCart : WebFormsPageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ShoppingCartTitle.InnerText = ""Shopping Cart is Empty"";
    }
}";

        var result = _transform.Apply(input, metadata);

        // The InnerText access should be rewritten
        Assert.DoesNotContain(".InnerText", result);
        Assert.Contains("ShoppingCartTitle = \"Shopping Cart is Empty\"", result);

        // A private string field stub should be injected for the undeclared PascalCase ID
        Assert.Contains("private string ShoppingCartTitle = \"\";", result);
    }

    [Fact]
    public void DoesNotInjectStub_WhenIdentifierAlreadyDeclared()
    {
        var metadata = new FileMetadata
        {
            SourceFilePath = "ShoppingCart.aspx.cs",
            OutputFilePath = "ShoppingCart.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty
        };

        // ShoppingCartTitle is already declared as a field
        var input = @"namespace TestApp;

public partial class ShoppingCart : WebFormsPageBase
{
    private string ShoppingCartTitle = """";

    protected void Page_Load(object sender, EventArgs e)
    {
        ShoppingCartTitle.InnerText = ""Shopping Cart is Empty"";
    }
}";

        var result = _transform.Apply(input, metadata);

        // Should NOT add a second declaration
        var count = System.Text.RegularExpressions.Regex.Matches(result, @"private string ShoppingCartTitle").Count;
        Assert.Equal(1, count);
    }

    [Fact]
    public void DoesNotInjectStub_ForCamelCaseOrUnderscoredIdentifiers()
    {
        // camelCase and _underscore identifiers are local variables/fields, not HTML server control IDs
        var metadata = new FileMetadata
        {
            SourceFilePath = "Cart.aspx.cs",
            OutputFilePath = "Cart.razor.cs",
            FileType = FileType.Page,
            OriginalContent = string.Empty
        };

        var input = @"namespace TestApp;

public partial class Cart : WebFormsPageBase
{
    protected void Load()
    {
        cartCount.InnerHtml = ""3"";
        _titleDiv.InnerText = ""My Title"";
    }
}";

        var result = _transform.Apply(input, metadata);

        // camelCase: no stub (cartCount starts with lowercase)
        Assert.DoesNotContain("private string cartCount", result);
        // _underscore: no stub (_titleDiv starts with underscore, not uppercase)
        Assert.DoesNotContain("private string _titleDiv", result);
    }
}

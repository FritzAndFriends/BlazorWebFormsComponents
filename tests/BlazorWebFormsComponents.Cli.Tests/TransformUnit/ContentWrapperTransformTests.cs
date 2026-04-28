using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for ContentWrapperTransform — converts &lt;asp:Content&gt; wrappers to BWFC
/// &lt;Content ContentPlaceHolderID="X"&gt; and wraps in the master page component.
/// Corresponds to TC09-ContentWrappers test case.
/// </summary>
public class ContentWrapperTransformTests
{
    private readonly ContentWrapperTransform _transform = new();

    private static FileMetadata PageMeta(string path = "Default.aspx", string originalContent = "") => new()
    {
        SourceFilePath = path,
        OutputFilePath = Path.ChangeExtension(path, ".razor"),
        FileType = FileType.Page,
        OriginalContent = originalContent
    };

    private static FileMetadata MasterMeta(string path = "Site.master") => new()
    {
        SourceFilePath = path,
        OutputFilePath = Path.ChangeExtension(path, ".razor"),
        FileType = FileType.Master,
        OriginalContent = ""
    };

    private static FileMetadata PageMetaWithMaster(string masterFile) => new()
    {
        SourceFilePath = "Default.aspx",
        OutputFilePath = "Default.razor",
        FileType = FileType.Page,
        OriginalContent = $"<%@ Page MasterPageFile=\"{masterFile}\" %>"
    };

    [Fact]
    public void ConvertsOpenTagToNamedContent()
    {
        var input = "<asp:Content ID=\"BodyContent\" ContentPlaceHolderID=\"MainContent\" runat=\"server\">\n" +
                    "    <h1>Hello</h1>\n" +
                    "</asp:Content>";

        var result = _transform.Apply(input, PageMeta());

        Assert.Contains("<Content ContentPlaceHolderID=\"MainContent\">", result);
        Assert.DoesNotContain("<asp:Content", result);
    }

    [Fact]
    public void ConvertsCloseTagToContentClose()
    {
        var input = "<asp:Content ContentPlaceHolderID=\"Main\" runat=\"server\">\n" +
                    "    <p>Body</p>\n" +
                    "</asp:Content>";

        var result = _transform.Apply(input, PageMeta());

        Assert.Contains("</Content>", result);
        Assert.DoesNotContain("</asp:Content>", result);
    }

    [Fact]
    public void PreservesInnerContent()
    {
        var input = "<asp:Content ID=\"BodyContent\" ContentPlaceHolderID=\"MainContent\" runat=\"server\">\n" +
                    "    <h1>Welcome</h1>\n" +
                    "    <p>Hello World</p>\n" +
                    "</asp:Content>";

        var result = _transform.Apply(input, PageMeta());

        Assert.Contains("<h1>Welcome</h1>", result);
        Assert.Contains("<p>Hello World</p>", result);
    }

    [Fact]
    public void PreservesContentPlaceHolderID_Value()
    {
        var input = "<asp:Content ContentPlaceHolderID=\"FeaturedContent\" runat=\"server\"><p>test</p></asp:Content>";

        var result = _transform.Apply(input, PageMeta());

        Assert.Contains("ContentPlaceHolderID=\"FeaturedContent\"", result);
    }

    [Fact]
    public void WrapsInMasterComponent_WhenMasterPageFilePresent()
    {
        var input = "<asp:Content ContentPlaceHolderID=\"MainContent\" runat=\"server\">\n" +
                    "    <h1>Body</h1>\n" +
                    "</asp:Content>";

        var result = _transform.Apply(input, PageMetaWithMaster("~/Site.Master"));

        Assert.Contains("<Site>", result);
        Assert.Contains("</Site>", result);
    }

    [Fact]
    public void ComponentNameDerivedFromMasterPageFile()
    {
        var input = "<asp:Content ContentPlaceHolderID=\"Main\" runat=\"server\"><p>x</p></asp:Content>";

        var result = _transform.Apply(input, PageMetaWithMaster("~/Layouts/AdminLayout.master"));

        Assert.Contains("<AdminLayout>", result);
        Assert.Contains("</AdminLayout>", result);
    }

    [Fact]
    public void NoWrapping_WhenNoMasterPageFile()
    {
        var input = "<asp:Content ContentPlaceHolderID=\"Main\" runat=\"server\"><p>x</p></asp:Content>";

        var result = _transform.Apply(input, PageMeta(originalContent: ""));

        // Content converted but no master component wrapper
        Assert.Contains("<Content ContentPlaceHolderID=\"Main\">", result);
        Assert.DoesNotContain("<Site>", result);
    }

    [Fact]
    public void MultipleContentBlocks_AllWrapped()
    {
        var input = "<asp:Content ID=\"Head\" ContentPlaceHolderID=\"HeadContent\" runat=\"server\">\n" +
                    "    <title>My Page</title>\n" +
                    "</asp:Content>\n" +
                    "<asp:Content ID=\"Body\" ContentPlaceHolderID=\"MainContent\" runat=\"server\">\n" +
                    "    <h1>Page body</h1>\n" +
                    "</asp:Content>";

        var result = _transform.Apply(input, PageMetaWithMaster("~/Site.Master"));

        Assert.DoesNotContain("<asp:Content", result);
        Assert.DoesNotContain("</asp:Content>", result);
        Assert.Contains("<Content ContentPlaceHolderID=\"HeadContent\">", result);
        Assert.Contains("<Content ContentPlaceHolderID=\"MainContent\">", result);
        Assert.Contains("<title>My Page</title>", result);
        Assert.Contains("<h1>Page body</h1>", result);
        // Both Content blocks wrapped in a single Site component
        Assert.Equal(1, result.Split("<Site>").Length - 1);
        Assert.Equal(1, result.Split("</Site>").Length - 1);
    }

    [Fact]
    public void SkipsMasterFiles()
    {
        var input = "<asp:Content ContentPlaceHolderID=\"Main\" runat=\"server\"><p>master</p></asp:Content>";
        var result = _transform.Apply(input, MasterMeta());
        Assert.Equal(input, result);
    }

    [Fact]
    public void PassesThroughInputWithNoContentTags()
    {
        var input = "<div class=\"wrapper\"><p>No asp:Content here</p></div>";
        var result = _transform.Apply(input, PageMeta());
        Assert.Equal(input, result);
    }

    [Fact]
    public void Order_Is_300()
    {
        Assert.Equal(300, _transform.Order);
    }

    [Fact]
    public void Name_IsContentWrapper()
    {
        Assert.Equal("ContentWrapper", _transform.Name);
    }
}

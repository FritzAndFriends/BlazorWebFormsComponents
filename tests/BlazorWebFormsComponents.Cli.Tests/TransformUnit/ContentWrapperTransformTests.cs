using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for ContentWrapperTransform — strips &lt;asp:Content&gt; wrappers and keeps
/// only the inner content.
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
    public void StripsOpenTagKeepingInnerContent()
    {
        var input = "<asp:Content ID=\"BodyContent\" ContentPlaceHolderID=\"MainContent\" runat=\"server\">\n" +
                    "    <h1>Hello</h1>\n" +
                    "</asp:Content>";

        var result = _transform.Apply(input, PageMeta());

        Assert.Contains("<h1>Hello</h1>", result);
        Assert.DoesNotContain("<asp:Content", result);
        Assert.DoesNotContain("ContentPlaceHolderID", result);
    }

    [Fact]
    public void StripsCloseTagKeepingInnerContent()
    {
        var input = "<asp:Content ContentPlaceHolderID=\"Main\" runat=\"server\">\n" +
                    "    <p>Body</p>\n" +
                    "</asp:Content>";

        var result = _transform.Apply(input, PageMeta());

        Assert.Contains("<p>Body</p>", result);
        Assert.DoesNotContain("</asp:Content>", result);
        Assert.DoesNotContain("</Content>", result);
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
    public void RemovesContentPlaceHolderID_Attribute()
    {
        var input = "<asp:Content ContentPlaceHolderID=\"FeaturedContent\" runat=\"server\"><p>test</p></asp:Content>";

        var result = _transform.Apply(input, PageMeta());

        Assert.DoesNotContain("ContentPlaceHolderID=\"FeaturedContent\"", result);
        Assert.Equal("<p>test</p>", result.Trim());
    }

    [Fact]
    public void DoesNotWrapInMasterComponent_WhenMasterPageFilePresent()
    {
        var input = "<asp:Content ContentPlaceHolderID=\"MainContent\" runat=\"server\">\n" +
                    "    <h1>Body</h1>\n" +
                    "</asp:Content>";

        var result = _transform.Apply(input, PageMetaWithMaster("~/Site.Master"));

        Assert.Contains("<h1>Body</h1>", result);
        Assert.DoesNotContain("<Site>", result);
        Assert.DoesNotContain("</Site>", result);
    }

    [Fact]
    public void IgnoresMasterPageComponentName_WhenStrippingContent()
    {
        var input = "<asp:Content ContentPlaceHolderID=\"Main\" runat=\"server\"><p>x</p></asp:Content>";

        var result = _transform.Apply(input, PageMetaWithMaster("~/Layouts/AdminLayout.master"));

        Assert.Equal("<p>x</p>", result.Trim());
        Assert.DoesNotContain("<AdminLayout>", result);
        Assert.DoesNotContain("</AdminLayout>", result);
    }

    [Fact]
    public void NoWrapping_WhenNoMasterPageFile()
    {
        var input = "<asp:Content ContentPlaceHolderID=\"Main\" runat=\"server\"><p>x</p></asp:Content>";

        var result = _transform.Apply(input, PageMeta(originalContent: ""));

        Assert.Equal("<p>x</p>", result.Trim());
        Assert.DoesNotContain("<Site>", result);
        Assert.DoesNotContain("ContentPlaceHolderID", result);
    }

    [Fact]
    public void MultipleContentBlocks_AllStripped()
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
        Assert.DoesNotContain("ContentPlaceHolderID", result);
        Assert.DoesNotContain("<Site>", result);
        Assert.Contains("<title>My Page</title>", result);
        Assert.Contains("<h1>Page body</h1>", result);
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

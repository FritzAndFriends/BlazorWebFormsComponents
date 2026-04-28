using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for MasterPageTransform — converts .master files to BWFC MasterPage component syntax
/// with named ContentPlaceHolder components and HTML scaffold stripping.
/// </summary>
public class MasterPageTransformTests
{
    private readonly MasterPageTransform _transform = new();

    private static FileMetadata MasterMetadata => new()
    {
        SourceFilePath = "Site.master",
        OutputFilePath = "Site.razor",
        FileType = FileType.Master,
        OriginalContent = ""
    };

    private static FileMetadata PageMetadata => new()
    {
        SourceFilePath = "Default.aspx",
        OutputFilePath = "Default.razor",
        FileType = FileType.Page,
        OriginalContent = ""
    };

    [Fact]
    public void ConvertsSelfClosingContentPlaceHolder()
    {
        var input = "<asp:ContentPlaceHolder ID=\"HeadContent\" runat=\"server\" />";

        var result = _transform.Apply(input, MasterMetadata);
        Assert.Contains("<ContentPlaceHolder ID=\"HeadContent\"", result);
        Assert.DoesNotContain("asp:ContentPlaceHolder", result);
        Assert.DoesNotContain("@Body", result);
    }

    [Fact]
    public void ConvertsBlockContentPlaceHolder_PreservesId()
    {
        var input = "<asp:ContentPlaceHolder ID=\"MainContent\" runat=\"server\">\n" +
                    "    <p>Default content</p>\n" +
                    "</asp:ContentPlaceHolder>";

        var result = _transform.Apply(input, MasterMetadata);
        Assert.Contains("<ContentPlaceHolder ID=\"MainContent\">", result);
        Assert.DoesNotContain("asp:ContentPlaceHolder", result);
        Assert.DoesNotContain("@Body", result);
    }

    [Fact]
    public void ConvertsBlockContentPlaceHolder_PreservesDefaultContent()
    {
        var input = "<asp:ContentPlaceHolder ID=\"MainContent\" runat=\"server\">\n" +
                    "    <p>Default content</p>\n" +
                    "</asp:ContentPlaceHolder>";

        var result = _transform.Apply(input, MasterMetadata);
        // Default content inside ContentPlaceHolder is preserved
        Assert.Contains("Default content", result);
    }

    [Fact]
    public void WrapsMasterPageInMasterPageComponent()
    {
        var input = "<html><body></body></html>";

        var result = _transform.Apply(input, MasterMetadata);
        Assert.Contains("<MasterPage>", result);
        Assert.Contains("</MasterPage>", result);
    }

    [Fact]
    public void DoesNotAddInheritsDirective()
    {
        var input = "<html><body></body></html>";

        var result = _transform.Apply(input, MasterMetadata);
        Assert.DoesNotContain("@inherits LayoutComponentBase", result);
    }

    [Fact]
    public void AddsTodoComment()
    {
        var input = "<html></html>";

        var result = _transform.Apply(input, MasterMetadata);
        Assert.Contains("@* TODO(bwfc-master-page):", result);
    }

    [Fact]
    public void PreservesCssLinksInsideHeadParameter()
    {
        var input = "<head runat=\"server\">\n" +
                    "    <link href=\"/Content/site.css\" rel=\"stylesheet\" />\n" +
                    "</head>\n<body></body>";

        var result = _transform.Apply(input, MasterMetadata);
        Assert.Contains("<Head>", result);
        Assert.Contains("<link href=\"/Content/site.css\" rel=\"stylesheet\" />", result);
    }

    [Fact]
    public void NonCssHeadContent_GoesIntoHeadParameter()
    {
        var input = "<head runat=\"server\">\n" +
                    "    <title>My Site</title>\n" +
                    "    <link href=\"/site.css\" rel=\"stylesheet\" />\n" +
                    "</head>\n<body></body>";

        var result = _transform.Apply(input, MasterMetadata);
        // Non-CSS head content goes into <MasterPage><Head>
        Assert.Contains("<title>My Site</title>", result);
        // The <head> element itself is stripped (content goes into <Head> parameter)
        Assert.DoesNotContain("</head>", result);
    }

    [Fact]
    public void StripsOuterHtmlScaffold()
    {
        var input = "<!DOCTYPE html>\n<html>\n<head runat=\"server\"></head>\n<body>\n    <form id=\"form1\">\n    </form>\n</body>\n</html>";

        var result = _transform.Apply(input, MasterMetadata);
        Assert.DoesNotContain("<!DOCTYPE html>", result);
        Assert.DoesNotContain("<html>", result);
        Assert.DoesNotContain("</html>", result);
        Assert.DoesNotContain("<body>", result);
        Assert.DoesNotContain("</body>", result);
    }

    [Fact]
    public void StripsRunatFromHead()
    {
        var input = "<head runat=\"server\">\n    <title>Test</title>\n</head>";

        var result = _transform.Apply(input, MasterMetadata);
        Assert.DoesNotContain("<head runat=\"server\">", result);
    }

    [Fact]
    public void StripsRunatFromHeadPreservingOtherAttributes()
    {
        var input = "<head id=\"Head1\" runat=\"server\">";

        var result = _transform.Apply(input, MasterMetadata);
        Assert.Contains("<head id=\"Head1\">", result);
        Assert.DoesNotContain("runat", result.Substring(result.IndexOf("<head")));
    }

    [Fact]
    public void StripsRunatFromForm()
    {
        var input = "<form id=\"form1\" runat=\"server\">\n    <div>content</div>\n</form>";

        var result = _transform.Apply(input, MasterMetadata);
        Assert.DoesNotContain("<form", result);
        Assert.Contains("<div>content</div>", result);
    }

    [Fact]
    public void SkipsNonMasterFiles()
    {
        var input = "<asp:ContentPlaceHolder ID=\"MainContent\" runat=\"server\" />";

        var result = _transform.Apply(input, PageMetadata);
        Assert.Equal(input, result);
    }

    [Fact]
    public void ConvertsFullMasterPage()
    {
        var input = "<!DOCTYPE html>\n" +
                    "<html>\n" +
                    "<head runat=\"server\">\n" +
                    "    <title>My Site</title>\n" +
                    "</head>\n" +
                    "<body>\n" +
                    "    <form id=\"form1\" runat=\"server\">\n" +
                    "        <asp:ContentPlaceHolder ID=\"MainContent\" runat=\"server\" />\n" +
                    "    </form>\n" +
                    "</body>\n" +
                    "</html>";

        var result = _transform.Apply(input, MasterMetadata);

        Assert.Contains("<MasterPage>", result);
        Assert.Contains("</MasterPage>", result);
        Assert.Contains("<ContentPlaceHolder ID=\"MainContent\"", result);
        Assert.DoesNotContain("<form id=\"form1\">", result);
        Assert.DoesNotContain("asp:ContentPlaceHolder", result);
        Assert.DoesNotContain("runat=\"server\"", result);
        Assert.DoesNotContain("@inherits LayoutComponentBase", result);
        Assert.Contains("@ChildContent", result);
        Assert.Contains("TODO(bwfc-master-page)", result);
    }

    [Fact]
    public void EmitsChildContentParameterAndRendersIt()
    {
        var input = "<html><body><asp:ContentPlaceHolder ID=\"MainContent\" runat=\"server\" /></body></html>";

        var result = _transform.Apply(input, MasterMetadata);

        Assert.Contains("@ChildContent", result);
        Assert.Contains("[Parameter]", result);
        Assert.Contains("public RenderFragment? ChildContent { get; set; }", result);
    }

    [Fact]
    public void OrderIs250()
    {
        Assert.Equal(250, _transform.Order);
    }

    [Fact]
    public void NameIsMasterPage()
    {
        Assert.Equal("MasterPage", _transform.Name);
    }

    [Fact]
    public void ConvertsMultipleContentPlaceHolders_EachNamed()
    {
        var input = "<asp:ContentPlaceHolder ID=\"Head\" runat=\"server\" />\n" +
                    "<asp:ContentPlaceHolder ID=\"MainContent\" runat=\"server\" />";

        var result = _transform.Apply(input, MasterMetadata);

        Assert.Contains("<ContentPlaceHolder ID=\"Head\"", result);
        Assert.Contains("<ContentPlaceHolder ID=\"MainContent\"", result);
        Assert.DoesNotContain("asp:ContentPlaceHolder", result);
        Assert.DoesNotContain("@Body", result);
    }

    [Fact]
    public void StripsRunatFromHeadWhenRunatIsFirstAttribute()
    {
        var input = "<head runat=\"server\" id=\"Head1\">";

        var result = _transform.Apply(input, MasterMetadata);

        Assert.Contains("<head", result);
        Assert.DoesNotContain("runat", result.Substring(result.IndexOf("<head")));
        Assert.Contains("id=\"Head1\"", result);
    }

    [Fact]
    public void StripsRunatFromFormWithActionAttribute()
    {
        var input = "<form id=\"form1\" action=\"/submit\" runat=\"server\">";

        var result = _transform.Apply(input, MasterMetadata);

        Assert.Contains("<form id=\"form1\" action=\"/submit\">", result);
        Assert.DoesNotContain("runat", result);
    }

    [Fact]
    public void ContentPlaceHolderBlock_DefaultContentPreserved()
    {
        var input = "<asp:ContentPlaceHolder ID=\"Main\" runat=\"server\">\n" +
                    "    <p>Line 1</p>\n" +
                    "    <p>Line 2</p>\n" +
                    "    <p>Line 3</p>\n" +
                    "</asp:ContentPlaceHolder>";

        var result = _transform.Apply(input, MasterMetadata);

        Assert.Contains("<ContentPlaceHolder ID=\"Main\">", result);
        Assert.Contains("Line 1", result);
        Assert.DoesNotContain("asp:ContentPlaceHolder", result);
    }

    [Fact]
    public void DirectiveLinesPreservedOutsideMasterPageWrapper()
    {
        var input = "@using MyApp.Models\n" +
                    "<html><body><asp:ContentPlaceHolder ID=\"Main\" runat=\"server\" /></body></html>";

        var result = _transform.Apply(input, MasterMetadata);

        // @using should appear before <MasterPage>
        var usingIndex = result.IndexOf("@using MyApp.Models");
        var masterPageIndex = result.IndexOf("<MasterPage>");
        Assert.True(usingIndex >= 0 && masterPageIndex >= 0);
        Assert.True(usingIndex < masterPageIndex);
    }
}

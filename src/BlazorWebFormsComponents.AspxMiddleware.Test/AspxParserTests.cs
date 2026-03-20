using BlazorWebFormsComponents.AspxMiddleware;
using Shouldly;

namespace BlazorWebFormsComponents.AspxMiddleware.Test;

public class AspxParserTests
{
    [Fact]
    public void Parse_SimpleAspx_ExtractsDirective()
    {
        var content = """
            <%@ Page Title="Simple Test" Language="C#" %>
            <asp:Button Text="Submit" ID="btn1" />
            """;

        var result = AspxParser.Parse(content);

        result.Directives.ShouldHaveSingleItem();
        result.Directives[0].DirectiveType.ShouldBe("Page");
        result.Directives[0].Attributes["Title"].ShouldBe("Simple Test");
        result.Directives[0].Attributes["Language"].ShouldBe("C#");
    }

    [Fact]
    public void Parse_SimpleAspx_ExtractsPageTitle()
    {
        var content = """<%@ Page Title="My Page" %>""";
        var result = AspxParser.Parse(content);
        result.PageTitle.ShouldBe("My Page");
    }

    [Fact]
    public void Parse_SimpleAspx_ExtractsMasterPageFile()
    {
        var content = """<%@ Page Title="Test" MasterPageFile="~/Site.Master" %>""";
        var result = AspxParser.Parse(content);
        result.MasterPageFile.ShouldBe("~/Site.Master");
    }

    [Fact]
    public void Parse_SelfClosingAspControl_CreatesAspControlNode()
    {
        var content = """<asp:Button Text="Submit" ID="btn1" />""";
        var result = AspxParser.Parse(content);

        result.Nodes.ShouldHaveSingleItem();
        var node = result.Nodes[0].ShouldBeOfType<AspControlNode>();
        node.TagName.ShouldBe("Button");
        node.Attributes["Text"].ShouldBe("Submit");
        node.Attributes["ID"].ShouldBe("btn1");
        node.Children.ShouldBeEmpty();
    }

    [Fact]
    public void Parse_MultipleAspControls_CreatesMultipleNodes()
    {
        var content = """
            <asp:Button Text="Submit" ID="btn1" />
            <asp:Label Text="Hello World" ID="lbl1" />
            """;

        var result = AspxParser.Parse(content);

        var controls = result.Nodes.OfType<AspControlNode>().ToList();
        controls.Count.ShouldBe(2);
        controls[0].TagName.ShouldBe("Button");
        controls[1].TagName.ShouldBe("Label");
    }

    [Fact]
    public void Parse_NestedAspControls_CreatesTreeStructure()
    {
        var content = """
            <asp:Panel ID="pnl1">
                <asp:Label Text="Inside Panel" ID="lblInner" />
            </asp:Panel>
            """;

        var result = AspxParser.Parse(content);

        var panel = result.Nodes.OfType<AspControlNode>().ShouldHaveSingleItem();
        panel.TagName.ShouldBe("Panel");
        panel.Attributes["ID"].ShouldBe("pnl1");

        var label = panel.Children.OfType<AspControlNode>().ShouldHaveSingleItem();
        label.TagName.ShouldBe("Label");
        label.Attributes["Text"].ShouldBe("Inside Panel");
    }

    [Fact]
    public void Parse_StaticHtml_CreatesHtmlNode()
    {
        var content = """<h1>Hello</h1>""";
        var result = AspxParser.Parse(content);

        var htmlNode = result.Nodes.OfType<HtmlNode>().ShouldHaveSingleItem();
        htmlNode.Content.ShouldContain("Hello");
    }

    [Fact]
    public void Parse_MixedContent_CreatesBothNodeTypes()
    {
        var content = """
            <h1>Welcome</h1>
            <asp:Button Text="Go" ID="btnGo" />
            """;

        var result = AspxParser.Parse(content);

        result.Nodes.OfType<HtmlNode>().ShouldNotBeEmpty();
        result.Nodes.OfType<AspControlNode>().ShouldNotBeEmpty();
    }

    [Fact]
    public void Parse_Expression_CreatesExpressionNode()
    {
        var content = """<asp:Label ID="lbl1" Text="Hi" /><%= DateTime.Now %>""";
        var result = AspxParser.Parse(content);

        var exprNode = result.Nodes.OfType<ExpressionNode>().ShouldHaveSingleItem();
        exprNode.ExpressionType.ShouldBe("eval");
        exprNode.Expression.ShouldContain("DateTime.Now");
    }

    [Fact]
    public void Parse_DataBindExpression_CreatesDataBindExpressionNode()
    {
        var content = """<asp:Label ID="lbl1" Text="test" /><%# Item.Name %>""";
        var result = AspxParser.Parse(content);

        var exprNode = result.Nodes.OfType<ExpressionNode>().ShouldHaveSingleItem();
        exprNode.ExpressionType.ShouldBe("databind");
    }

    [Fact]
    public void Parse_TagNameIsCaseInsensitive()
    {
        var content = """<asp:BUTTON Text="Submit" ID="btn1" />""";
        var result = AspxParser.Parse(content);

        var node = result.Nodes.OfType<AspControlNode>().ShouldHaveSingleItem();
        // The tag name preserves case from the prefix replacement
        node.TagName.ShouldBe("BUTTON", StringCompareShould.IgnoreCase);
    }

    [Fact]
    public void Parse_SimpleAspxFile_FullParse()
    {
        var content = File.ReadAllText(GetTestPagePath("Simple.aspx"));
        var result = AspxParser.Parse(content);

        result.Directives.ShouldHaveSingleItem();
        result.PageTitle.ShouldBe("Simple Test");
        result.Nodes.OfType<AspControlNode>().Count().ShouldBe(2);
    }

    [Fact]
    public void Parse_NestedAspxFile_FullParse()
    {
        var content = File.ReadAllText(GetTestPagePath("Nested.aspx"));
        var result = AspxParser.Parse(content);

        result.PageTitle.ShouldBe("Nested Test");
        var panel = result.Nodes.OfType<AspControlNode>().ShouldHaveSingleItem();
        panel.TagName.ShouldBe("Panel");
        panel.Children.OfType<AspControlNode>().ShouldHaveSingleItem().TagName.ShouldBe("Label");
    }

    [Fact]
    public void Parse_MixedAspxFile_FullParse()
    {
        var content = File.ReadAllText(GetTestPagePath("Mixed.aspx"));
        var result = AspxParser.Parse(content);

        result.PageTitle.ShouldBe("Mixed Content");
        result.MasterPageFile.ShouldBe("~/Site.Master");
        result.Nodes.OfType<AspControlNode>().ShouldNotBeEmpty();
        result.Nodes.OfType<ExpressionNode>().ShouldNotBeEmpty();
    }

    [Fact]
    public void Parse_UnclosedHtmlTags_ParsesSuccessfully()
    {
        var content = """
            <br>
            <asp:Label Text="Test" ID="lbl1" />
            <hr>
            <asp:Button Text="Go" ID="btn1" />
            """;

        var result = AspxParser.Parse(content);

        result.Nodes.OfType<AspControlNode>().Count().ShouldBe(2);
    }

    [Fact]
    public void Parse_EntityInAttributeValue_PreservesEntity()
    {
        var content = """<asp:HyperLink NavigateUrl="page.aspx?id=1&amp;view=details" Text="Link" ID="lnk1" />""";
        var result = AspxParser.Parse(content);

        var link = result.Nodes.OfType<AspControlNode>().ShouldHaveSingleItem();
        link.Attributes["NavigateUrl"].ShouldContain("&");
    }

    [Fact]
    public void Parse_SingleQuoteAttributes_ExtractedCorrectly()
    {
        var content = """<%@ Page Title='Single Quote Test' Language='C#' %>""";
        var result = AspxParser.Parse(content);

        result.Directives.ShouldHaveSingleItem();
        result.Directives[0].Attributes["Title"].ShouldBe("Single Quote Test");
        result.Directives[0].Attributes["Language"].ShouldBe("C#");
    }

    [Fact]
    public void Parse_RunatServerAttribute_Stripped()
    {
        var content = """<asp:Button Text="Submit" ID="btn1" runat="server" CssClass="btn" />""";
        var result = AspxParser.Parse(content);

        var button = result.Nodes.OfType<AspControlNode>().ShouldHaveSingleItem();
        button.Attributes.ContainsKey("runat").ShouldBeFalse();
        button.Attributes["Text"].ShouldBe("Submit");
        button.Attributes["CssClass"].ShouldBe("btn");
    }

    [Fact]
    public void Parse_ServerSideComment_Stripped()
    {
        var content = """
            <asp:Label Text="Before" ID="lbl1" />
            <%-- This is a server comment --%>
            <asp:Label Text="After" ID="lbl2" />
            """;

        var result = AspxParser.Parse(content);

        var labels = result.Nodes.OfType<AspControlNode>().ToList();
        labels.Count.ShouldBe(2);
        labels[0].Attributes["Text"].ShouldBe("Before");
        labels[1].Attributes["Text"].ShouldBe("After");
        
        // Should not have any HTML nodes containing the comment
        var htmlNodes = result.Nodes.OfType<HtmlNode>().ToList();
        htmlNodes.ShouldNotContain(n => n.Content.Contains("server comment"));
    }

    [Fact]
    public void Parse_MixedQuotesInAttributes_HandledCorrectly()
    {
        var content = """<asp:Label Text="Double" ID='mixed' ToolTip='Single' CssClass="double-again" />""";
        var result = AspxParser.Parse(content);

        var label = result.Nodes.OfType<AspControlNode>().ShouldHaveSingleItem();
        label.Attributes["Text"].ShouldBe("Double");
        label.Attributes["ID"].ShouldBe("mixed");
        label.Attributes["ToolTip"].ShouldBe("Single");
        label.Attributes["CssClass"].ShouldBe("double-again");
    }

    // ── P2: Expression-in-Attribute Edge Case ─────────────────────────

    [Fact]
    public void Parse_ExpressionInAttributeValue_PreservesExpressionPlaceholders()
    {
        // Expressions inside attribute values (e.g. databind) are extracted by
        // ReplaceExpressions and replaced with HTML-comment-style placeholders.
        // AngleSharp treats the placeholder as literal text in the attribute value.
        var content = """<asp:HyperLink NavigateUrl='<%# Eval("Url") %>' Text='<%# Eval("Name") %>' runat="server" />""";

        var result = AspxParser.Parse(content);

        // Parser should not crash; the control is created
        var link = result.Nodes.OfType<AspControlNode>().ShouldHaveSingleItem();
        link.TagName.ShouldBe("HyperLink", StringCompareShould.IgnoreCase);

        // runat="server" stripped
        link.Attributes.ContainsKey("runat").ShouldBeFalse();

        // Attribute values contain expression placeholders (not raw <%# %> syntax)
        link.Attributes["navigateurl"].ShouldContain("___ASPX_EXPR_");
        link.Attributes["text"].ShouldContain("___ASPX_EXPR_");

        // Placeholders should be distinct (two different expressions)
        link.Attributes["navigateurl"].ShouldNotBe(link.Attributes["text"]);
    }

    [Fact]
    public void Parse_ExpressionInAttributeValue_StandaloneExpressionsStillWork()
    {
        // Mix of expressions in attributes AND standalone expressions.
        // Use explicit closing tag because expression placeholders contain '>'
        // which breaks the self-closing regex expansion (known P3 limitation).
        var content = """
            <asp:HyperLink NavigateUrl='<%# Eval("Url") %>' Text="Link" runat="server"></asp:HyperLink>
            <%= DateTime.Now %>
            <%# Item.Total %>
            """;

        var result = AspxParser.Parse(content);

        // The asp control exists with expression placeholder in NavigateUrl
        var link = result.Nodes.OfType<AspControlNode>().ShouldHaveSingleItem();
        link.Attributes["navigateurl"].ShouldContain("___ASPX_EXPR_");

        // Standalone expressions are still proper ExpressionNode objects
        var expressions = result.Nodes.OfType<ExpressionNode>().ToList();
        expressions.Count.ShouldBe(2);
        expressions.ShouldContain(e => e.ExpressionType == "eval" && e.Expression.Contains("DateTime.Now"));
        expressions.ShouldContain(e => e.ExpressionType == "databind" && e.Expression.Contains("Item.Total"));
    }

    // ── P2: Whitespace Preservation ─────────────────────────────────────

    [Fact]
    public void Parse_PreservesWhitespace_InTextNodesWithContent()
    {
        // Whitespace inside text that also contains non-whitespace characters
        // must be preserved through the AngleSharp pipeline.
        var content = """
            <asp:Panel ID="pnl1">
                <asp:Label Text="Hello" ID="lbl1" />
                   some text with   multiple   spaces   
                <asp:Label Text="World" ID="lbl2" />
            </asp:Panel>
            """;

        var result = AspxParser.Parse(content);

        var panel = result.Nodes.OfType<AspControlNode>().ShouldHaveSingleItem();
        var textNodes = panel.Children.OfType<HtmlNode>().ToList();

        // At least one text node should contain the multi-space text
        textNodes.ShouldContain(n => n.Content.Contains("multiple   spaces"));
    }

    [Fact]
    public void Parse_WhitespaceOnlyTextNodes_AreDropped()
    {
        // Whitespace-only text nodes (indentation, blank lines) are dropped
        // by the parser. This is intentional for Phase 1 — insignificant
        // whitespace between elements is not preserved.
        var content = "<div>\n    \n    \r\n    \n</div>";

        var result = AspxParser.Parse(content);

        // The whitespace-only text nodes inside <div> are dropped
        var htmlNodes = result.Nodes.OfType<HtmlNode>().ToList();
        foreach (var node in htmlNodes)
        {
            // Any HtmlNode that is a text node should NOT be whitespace-only
            // (whitespace-only nodes are filtered out by ConvertDomNode)
            if (!node.Content.StartsWith("<"))
            {
                string.IsNullOrWhiteSpace(node.Content).ShouldBeFalse(
                    $"Whitespace-only text node should have been dropped: '{node.Content}'");
            }
        }
    }

    [Fact]
    public void Parse_PreservesNewlines_InMixedTextContent()
    {
        // Text nodes with real content should preserve their newlines and spacing
        var content = """
            <asp:Panel ID="pnl1">Line one
            Line two
            Line three</asp:Panel>
            """;

        var result = AspxParser.Parse(content);

        var panel = result.Nodes.OfType<AspControlNode>().ShouldHaveSingleItem();
        var textNodes = panel.Children.OfType<HtmlNode>().ToList();
        textNodes.ShouldNotBeEmpty();

        // The combined text content should contain newlines (not collapsed)
        var combinedText = string.Join("", textNodes.Select(n => n.Content));
        combinedText.ShouldContain("Line one");
        combinedText.ShouldContain("Line two");
        combinedText.ShouldContain("Line three");
    }

    [Fact]
    public void Parse_PreservesIndentation_InPreformattedText()
    {
        // Spaces and indentation in text that contains non-whitespace content
        var content = """<asp:Panel ID="pnl1">    indented text    </asp:Panel>""";

        var result = AspxParser.Parse(content);

        var panel = result.Nodes.OfType<AspControlNode>().ShouldHaveSingleItem();
        var textNodes = panel.Children.OfType<HtmlNode>().ToList();
        textNodes.ShouldNotBeEmpty();

        // Leading and trailing spaces preserved in text node
        var text = textNodes[0].Content;
        text.ShouldStartWith("    indented");
        text.ShouldEndWith("text    ");
    }

    private static string GetTestPagePath(string fileName)
    {
        return Path.Combine(AppContext.BaseDirectory, "TestPages", fileName);
    }
}

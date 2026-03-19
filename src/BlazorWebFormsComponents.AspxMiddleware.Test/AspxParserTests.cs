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

    private static string GetTestPagePath(string fileName)
    {
        return Path.Combine(AppContext.BaseDirectory, "TestPages", fileName);
    }
}

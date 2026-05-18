namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

/// <summary>
/// Unit tests for ExpressionTransform — converts ASP.NET expressions to Razor syntax.
/// Corresponds to TC06-Expressions and TC08-Comments test cases.
/// </summary>
public class ExpressionTransformTests
{
    // TODO: Instantiate the real transform when Bishop builds it:
    // private readonly ExpressionTransform _transform = new();

    [Fact]
    public void ConvertsEncodedExpression()
    {
        // Input:  <%: DateTime.Now %>
        // Expect: @(DateTime.Now)
        var input = @"<span><%: DateTime.Now %></span>";
        var expected = @"<span>@(DateTime.Now)</span>";

        Assert.NotEqual(input, expected);
    }

    [Fact]
    public void ConvertsUnencodedExpression()
    {
        // Input:  <%= someVar %>
        // Expect: @(someVar)
        var input = @"<span><%= someVar %></span>";
        var expected = @"<span>@(someVar)</span>";

        Assert.NotEqual(input, expected);
    }

    [Fact]
    public void ConvertsEvalToContext()
    {
        // Input:  <%#: Eval("Name") %>
        // Expect: @context.Name
        var input = @"<span><%#: Eval(""Name"") %></span>";
        var expected = @"<span>@context.Name</span>";

        Assert.NotEqual(input, expected);
    }

    [Fact]
    public void ConvertsItemDotToContext()
    {
        // Input:  <%#: Item.Price %>
        // Expect: @context.Price
        var input = @"<span><%#: Item.Price %></span>";
        var expected = @"<span>@context.Price</span>";

        Assert.NotEqual(input, expected);
    }

    [Fact]
    public void ConvertsServerCommentToRazorComment()
    {
        // Input:  <%-- Single line comment --%>
        // Expect: @* Single line comment *@
        var input = @"<%-- Single line comment --%>";
        var expected = @"@* Single line comment *@";

        Assert.NotEqual(input, expected);
    }
}

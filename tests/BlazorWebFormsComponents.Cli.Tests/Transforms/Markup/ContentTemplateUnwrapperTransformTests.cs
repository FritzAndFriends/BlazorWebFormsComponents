using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.Transforms.Markup;

public class ContentTemplateUnwrapperTransformTests
{
    private readonly ContentTemplateUnwrapperTransform _transform = new();

    private static FileMetadata MakeMetadata() => new()
    {
        SourceFilePath = "ProductsControl.ascx",
        OutputFilePath = "ProductsControl.razor",
        FileType = FileType.Control,
        OriginalContent = string.Empty
    };

    [Fact]
    public void Apply_UnwrapsContentTemplateAndRewritesEvalBinding()
    {
        var input = """
            <UpdatePanel>
                <ContentTemplate>
                    <Repeater id="rptItems">
                        <ItemTemplate>
                            <%# Eval("ProductName") %>
                        </ItemTemplate>
                    </Repeater>
                </ContentTemplate>
            </UpdatePanel>
            """;

        var result = _transform.Apply(input, MakeMetadata());

        Assert.DoesNotContain("<ContentTemplate>", result);
        Assert.DoesNotContain("</ContentTemplate>", result);
        Assert.Contains("<ItemTemplate>", result);
        Assert.Contains("@Item.ProductName", result);
    }

    [Fact]
    public void Apply_RewritesEvalWithFormatString()
    {
        var input = """
            <ItemTemplate>
                <%# Eval("Price", "{0:C}") %>
            </ItemTemplate>
            """;

        var result = _transform.Apply(input, MakeMetadata());

        Assert.Contains("@Item.Price.ToString(\"C\")", result);
    }

    [Fact]
    public void Apply_RewritesContainerDataItemMethodCall()
    {
        var input = """
            <ItemTemplate>
                <%# GetValue(Container.DataItem) %>
            </ItemTemplate>
            """;

        var result = _transform.Apply(input, MakeMetadata());

        Assert.Contains("@GetValue(Item)", result);
    }

    [Fact]
    public void Apply_PreservesExistingTemplateContext()
    {
        var input = """
            <ItemTemplate Context="row">
                <%# Eval("Name") %>
            </ItemTemplate>
            """;

        var result = _transform.Apply(input, MakeMetadata());

        Assert.Contains("<ItemTemplate Context=\"row\">", result);
        Assert.Contains("@Item.Name", result);
    }

    [Fact]
    public void Apply_LeavesNonTemplateBindingsUnchanged()
    {
        var input = """<div><%# Eval("OutsideTemplate") %></div>""";

        var result = _transform.Apply(input, MakeMetadata());

        Assert.Equal(input, result);
    }
}

using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class DataControlChildComponentsTransformTests
{
    private readonly DataControlChildComponentsTransform _transform = new();

    private static FileMetadata TestMetadata => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = string.Empty
    };

    [Fact]
    public void HasCorrectOrder()
    {
        Assert.Equal(625, _transform.Order);
    }

    [Fact]
    public void WrapsGridViewRowStyle()
    {
        var input = """
            <GridView ItemType="CartItem">
                <Columns>
                    <BoundField DataField="Name" />
                </Columns>
                <RowStyle CssClass="row" />
            </GridView>
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<ChildComponents>", result);
        Assert.Contains("<RowStyle CssClass=\"row\" />", result);
        Assert.Contains("</ChildComponents>", result);
    }

    [Fact]
    public void WrapsMultipleStyleElements()
    {
        var input = """
            <GridView>
                <HeaderStyle CssClass="header" />
                <RowStyle CssClass="row" />
                <PagerStyle CssClass="pager" />
                <Columns>
                    <BoundField DataField="Id" />
                </Columns>
            </GridView>
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<ChildComponents>", result);
        Assert.Contains("<HeaderStyle CssClass=\"header\" />", result);
        Assert.Contains("<RowStyle CssClass=\"row\" />", result);
        Assert.Contains("<PagerStyle CssClass=\"pager\" />", result);
        Assert.Contains("</ChildComponents>", result);
    }

    [Fact]
    public void WrapsFormViewStyles()
    {
        var input = """
            <FormView ItemType="Product">
                <ItemTemplate Context="Item">
                    <span>@Item.Name</span>
                </ItemTemplate>
                <RowStyle CssClass="row" />
                <EditRowStyle CssClass="edit" />
            </FormView>
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<ChildComponents>", result);
        Assert.Contains("<RowStyle CssClass=\"row\" />", result);
        Assert.Contains("<EditRowStyle CssClass=\"edit\" />", result);
    }

    [Fact]
    public void DoesNotDoubleWrap()
    {
        var input = """
            <GridView>
                <Columns>
                    <BoundField DataField="Id" />
                </Columns>
                <ChildComponents>
                    <RowStyle CssClass="row" />
                </ChildComponents>
            </GridView>
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal(1, CountOccurrences(result, "<ChildComponents>"));
    }

    [Fact]
    public void IgnoresNonDataControls()
    {
        var input = """
            <Panel>
                <RowStyle CssClass="row" />
            </Panel>
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.DoesNotContain("<ChildComponents>", result);
    }

    [Fact]
    public void PreservesNonStyleChildren()
    {
        var input = """
            <ListView ItemType="Product">
                <ItemTemplate Context="Item">
                    <div>@Item.Name</div>
                </ItemTemplate>
                <HeaderStyle CssClass="header" />
            </ListView>
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<ItemTemplate Context=\"Item\">", result);
        Assert.Contains("<div>@Item.Name</div>", result);
        Assert.Contains("<ChildComponents>", result);
        Assert.Contains("<HeaderStyle CssClass=\"header\" />", result);
    }

    [Fact]
    public void HandlesDetailsView()
    {
        var input = """
            <DetailsView ItemType="Student">
                <RowStyle CssClass="row" />
                <PagerStyle CssClass="pager" />
                <SelectedRowStyle CssClass="selected" />
            </DetailsView>
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<ChildComponents>", result);
        Assert.Contains("<SelectedRowStyle CssClass=\"selected\" />", result);
    }

    [Fact]
    public void NoStyleElements_ReturnsUnchanged()
    {
        var input = """
            <GridView ItemType="Product">
                <Columns>
                    <BoundField DataField="Name" />
                </Columns>
            </GridView>
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.DoesNotContain("<ChildComponents>", result);
    }

    [Fact]
    public void PipelineWrapsGridViewStyles()
    {
        var input = """
            <asp:GridView ID="gv" runat="server" ItemType="CartItem">
                <Columns>
                    <asp:BoundField DataField="ProductID" />
                </Columns>
                <RowStyle CssClass="data-row" />
                <HeaderStyle CssClass="header-row" />
            </asp:GridView>
            """;

        var pipeline = TestHelpers.CreateDefaultPipeline();
        var result = pipeline.TransformMarkup(input, TestMetadata);

        Assert.Contains("<ChildComponents>", result);
        Assert.Contains("<RowStyle CssClass=\"data-row\" />", result);
        Assert.Contains("<HeaderStyle CssClass=\"header-row\" />", result);
        Assert.DoesNotContain("asp:", result);
    }

    private static int CountOccurrences(string source, string target)
    {
        var count = 0;
        var index = 0;
        while ((index = source.IndexOf(target, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += target.Length;
        }
        return count;
    }
}

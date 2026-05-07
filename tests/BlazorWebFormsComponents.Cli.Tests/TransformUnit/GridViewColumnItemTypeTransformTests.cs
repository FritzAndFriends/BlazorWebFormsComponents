using BlazorWebFormsComponents.Cli.Pipeline;
using BlazorWebFormsComponents.Cli.Transforms.Markup;

namespace BlazorWebFormsComponents.Cli.Tests.TransformUnit;

public class GridViewColumnItemTypeTransformTests
{
    private readonly GridViewColumnItemTypeTransform _transform = new();

    private static FileMetadata TestMetadata => new()
    {
        SourceFilePath = "test.aspx",
        OutputFilePath = "test.razor",
        FileType = FileType.Page,
        OriginalContent = string.Empty
    };

    [Fact]
    public void UsesParentGridViewItemTypeForObjectColumns()
    {
        var input = """
            <GridView ItemType="CartItem" AutoGenerateColumns="false">
                <Columns>
                    <BoundField ItemType="object" DataField="ProductID" />
                    <TemplateField ItemType="object" HeaderText="Quantity">
                        <ItemTemplate Context="Item">
                            <TextBox Text="@Item.Quantity" />
                        </ItemTemplate>
                    </TemplateField>
                </Columns>
            </GridView>
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<BoundField ItemType=\"CartItem\" DataField=\"ProductID\" />", result);
        Assert.Contains("<TemplateField ItemType=\"CartItem\" HeaderText=\"Quantity\">", result);
        Assert.DoesNotContain("ItemType=\"object\"", result);
        Assert.Contains("</GridView>", result);
    }

    [Fact]
    public void LeavesObjectTypedGridViewColumnsUntouched()
    {
        var input = """
            <GridView ItemType="object" AutoGenerateColumns="false">
                <Columns>
                    <BoundField ItemType="object" DataField="ProductName" />
                </Columns>
            </GridView>
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.Equal(input, result);
    }

    [Fact]
    public void PipelineRewritesWingtipShoppingCartColumnsToTypedFields()
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        var metadata = TestMetadata;

        var result = pipeline.TransformMarkup("""
            <asp:GridView ID="CartList" runat="server" AutoGenerateColumns="False" ItemType="WingtipToys.Models.CartItem">
                <Columns>
                    <asp:BoundField DataField="ProductID" HeaderText="ID" />
                    <asp:TemplateField HeaderText="Quantity">
                        <ItemTemplate>
                            <asp:TextBox ID="PurchaseQuantity" runat="server" Text="<%#: Item.Quantity %>"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            """, metadata);

        Assert.Contains("<GridView id=\"CartList\" AutoGenerateColumns=\"false\" ItemType=\"CartItem\">", result);
        Assert.Contains("<BoundField ItemType=\"CartItem\" DataField=\"ProductID\" HeaderText=\"ID\" />", result);
        Assert.Contains("<TemplateField ItemType=\"CartItem\" HeaderText=\"Quantity\">", result);
        Assert.Contains("<ItemTemplate Context=\"Item\">", result);
        Assert.Contains("Text=\"@Item.Quantity\"", result);
        Assert.DoesNotContain("ItemType=\"object\"", result);
        Assert.Equal(1, CountOccurrences(result, "<Columns>"));
        Assert.Contains("</GridView>", result);
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

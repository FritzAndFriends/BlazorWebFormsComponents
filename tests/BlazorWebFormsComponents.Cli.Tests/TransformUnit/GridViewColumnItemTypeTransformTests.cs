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
    public void PipelinePreservesTemplateFieldWithInnerTextBoxControl()
    {
        var result = TransformMarkup("""
            <asp:GridView ID="CartList" runat="server" AutoGenerateColumns="False" ItemType="WingtipToys.Models.CartItem">
                <Columns>
                    <asp:TemplateField HeaderText="Quantity">
                        <ItemTemplate>
                            <asp:TextBox ID="PurchaseQuantity" Width="40" runat="server" Text="<%#: Item.Quantity %>"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            """);

        Assert.Contains("<GridView id=\"CartList\" AutoGenerateColumns=\"false\" ItemType=\"CartItem\">", result);
        Assert.Contains("<TemplateField ItemType=\"CartItem\" HeaderText=\"Quantity\">", result);
        Assert.Contains("<ItemTemplate Context=\"Item\">", result);
        Assert.Contains("<TextBox id=\"PurchaseQuantity\" Width=\"40\" Text=\"@Item.Quantity.ToString()\"></TextBox>", result);
        Assert.Equal(1, CountOccurrences(result, "<TemplateField"));
        Assert.DoesNotContain("<BoundField", result);
    }

    [Fact]
    public void PipelinePreservesTemplateFieldWithInnerCheckBoxControl()
    {
        var result = TransformMarkup("""
            <asp:GridView ID="CartList" runat="server" AutoGenerateColumns="False" ItemType="WingtipToys.Models.CartItem">
                <Columns>
                    <asp:TemplateField HeaderText="Remove Item">
                        <ItemTemplate>
                            <asp:CheckBox ID="Remove" runat="server"></asp:CheckBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            """);

        Assert.Contains("<TemplateField ItemType=\"CartItem\" HeaderText=\"Remove Item\">", result);
        Assert.Contains("<ItemTemplate Context=\"Item\">", result);
        Assert.Contains("<CheckBox id=\"Remove\"></CheckBox>", result);
        Assert.Equal(1, CountOccurrences(result, "<TemplateField"));
        Assert.DoesNotContain("<BoundField", result);
    }

    [Fact]
    public void PipelinePreservesTemplateFieldWithDisplayExpressionOnly()
    {
        var result = TransformMarkup("""
            <asp:GridView ID="CartList" runat="server" AutoGenerateColumns="False" ItemType="WingtipToys.Models.CartItem">
                <Columns>
                    <asp:TemplateField HeaderText="Item Total">
                        <ItemTemplate>
                            <%#: String.Format("{0:c}", ((Convert.ToDouble(Item.Quantity)) * Convert.ToDouble(Item.Product.UnitPrice))) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            """);

        Assert.Contains("<TemplateField ItemType=\"CartItem\" HeaderText=\"Item Total\">", result);
        Assert.Contains("<ItemTemplate Context=\"Item\">", result);
        Assert.Contains("@(String.Format(\"{0:c}\", ((Convert.ToDouble(Item.Quantity)) * Convert.ToDouble(Item.Product.UnitPrice))))", result);
        Assert.Equal(1, CountOccurrences(result, "<TemplateField"));
        Assert.DoesNotContain("<BoundField", result);
    }

    [Fact]
    public void PipelinePreservesTemplateFieldsMixedWithBoundFieldSiblings()
    {
        var result = TransformMarkup("""
            <asp:GridView ID="CartList" runat="server" AutoGenerateColumns="False" ItemType="WingtipToys.Models.CartItem">
                <Columns>
                    <asp:BoundField DataField="ProductID" HeaderText="ID" />
                    <asp:BoundField DataField="Product.ProductName" HeaderText="Name" />
                    <asp:TemplateField HeaderText="Quantity">
                        <ItemTemplate>
                            <asp:TextBox ID="PurchaseQuantity" Width="40" runat="server" Text="<%#: Item.Quantity %>"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Item Total">
                        <ItemTemplate>
                            <%#: String.Format("{0:c}", ((Convert.ToDouble(Item.Quantity)) * Convert.ToDouble(Item.Product.UnitPrice))) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Remove Item">
                        <ItemTemplate>
                            <asp:CheckBox ID="Remove" runat="server"></asp:CheckBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            """);

        Assert.Contains("<BoundField ItemType=\"CartItem\" DataField=\"ProductID\" HeaderText=\"ID\" />", result);
        Assert.Contains("<BoundField ItemType=\"CartItem\" DataField=\"Product.ProductName\" HeaderText=\"Name\" />", result);
        Assert.Contains("<TemplateField ItemType=\"CartItem\" HeaderText=\"Quantity\">", result);
        Assert.Contains("<TemplateField ItemType=\"CartItem\" HeaderText=\"Item Total\">", result);
        Assert.Contains("<TemplateField ItemType=\"CartItem\" HeaderText=\"Remove Item\">", result);
        Assert.Equal(2, CountOccurrences(result, "<BoundField"));
        Assert.Equal(3, CountOccurrences(result, "<TemplateField"));
    }

    [Fact]
    public void PipelinePreservesGridViewWithOnlyTemplateFieldColumns()
    {
        var result = TransformMarkup("""
            <asp:GridView ID="CartList" runat="server" AutoGenerateColumns="False" ItemType="WingtipToys.Models.CartItem">
                <Columns>
                    <asp:TemplateField HeaderText="Quantity">
                        <ItemTemplate>
                            <asp:TextBox ID="PurchaseQuantity" Width="40" runat="server" Text="<%#: Item.Quantity %>"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Remove Item">
                        <ItemTemplate>
                            <asp:CheckBox ID="Remove" runat="server"></asp:CheckBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            """);

        Assert.Contains("<TemplateField ItemType=\"CartItem\" HeaderText=\"Quantity\">", result);
        Assert.Contains("<TemplateField ItemType=\"CartItem\" HeaderText=\"Remove Item\">", result);
        Assert.Equal(2, CountOccurrences(result, "<TemplateField"));
        Assert.DoesNotContain("<BoundField", result);
    }

    [Fact]
    public void UsesParentGridViewItemTypeForCommandField()
    {
        var input = """
            <GridView ItemType="StudentRow" AutoGenerateColumns="false">
                <Columns>
                    <CommandField ButtonType="Button" ShowDeleteButton="true" />
                </Columns>
            </GridView>
            """;

        var result = _transform.Apply(input, TestMetadata);

        Assert.Contains("<CommandField ItemType=\"StudentRow\" ButtonType=\"Button\" ShowDeleteButton=\"true\" />", result);
    }

    [Fact]
    public void PipelinePreservesBoundFieldReadOnlyAndCommandField()
    {
        var result = TransformMarkup("""
            <asp:GridView ID="grv" runat="server" AutoGenerateColumns="False" ItemType="ContosoUniversity.Models.StudentRow">
                <Columns>
                    <asp:CommandField HeaderText="Delete Student" ShowDeleteButton="True" ButtonType="Button" />
                    <asp:BoundField DataField="ID" HeaderText="Student ID" ReadOnly="True" />
                </Columns>
            </asp:GridView>
            """);

        Assert.Contains("<CommandField ItemType=\"StudentRow\" HeaderText=\"Delete Student\" ShowDeleteButton=\"true\" ButtonType=\"Button\" />", result);
        Assert.Contains("<BoundField ItemType=\"StudentRow\" DataField=\"ID\" HeaderText=\"Student ID\" ReadOnly=\"true\" />", result);
    }

    private static string TransformMarkup(string input)
    {
        var pipeline = TestHelpers.CreateDefaultPipeline();
        return pipeline.TransformMarkup(input, TestMetadata);
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

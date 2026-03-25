# Repeater

The **Repeater** component emulates the ASP.NET Web Forms `asp:Repeater` control. It provides a lightweight, template-driven way to display repeating data without imposing any HTML structure. Unlike GridView or DataList, the Repeater produces no wrapper markup — you have full control over the rendered output.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.repeater?view=netframework-4.8

## Features Supported in Blazor

- `ItemTemplate` — Template for each data item
- `AlternatingItemTemplate` — Template for alternating items
- `HeaderTemplate` — Rendered once before all items
- `FooterTemplate` — Rendered once after all items
- `SeparatorTemplate` — Rendered between each item
- `ItemType` — Strongly typed data binding
- `Items` / `SelectMethod` — Data source binding
- `Context` attribute — Access current item as `@Item` for Web Forms compatibility

## Web Forms Features NOT Supported

- **DataSourceID** — Blazor does not use server-side data source controls; pass data directly via `Items` or `SelectMethod`
- **EnableViewState** — Not needed; Blazor preserves component state natively
- **EnableTheming / SkinID** — Not applicable to Blazor

## Syntax Comparison

=== "Web Forms"

    ```html
    <asp:Repeater
        DataMember="string"
        DataSource="string"
        DataSourceID="string"
        EnableTheming="True|False"
        EnableViewState="True|False"
        ID="string"
        OnDataBinding="DataBinding event handler"
        OnItemCommand="ItemCommand event handler"
        OnItemCreated="ItemCreated event handler"
        OnItemDataBound="ItemDataBound event handler"
        runat="server"
        Visible="True|False"
    >
        <HeaderTemplate>
            <!-- header content -->
        </HeaderTemplate>
        <ItemTemplate>
            <!-- item content -->
        </ItemTemplate>
        <AlternatingItemTemplate>
            <!-- alternating item content -->
        </AlternatingItemTemplate>
        <SeparatorTemplate>
            <!-- separator content -->
        </SeparatorTemplate>
        <FooterTemplate>
            <!-- footer content -->
        </FooterTemplate>
    </asp:Repeater>
    ```

=== "Blazor"

    ```razor
    <Repeater Items="@dataSource" ItemType="MyItem" Context="Item">
        <HeaderTemplate>
            <!-- header content -->
        </HeaderTemplate>
        <ItemTemplate>
            @Item.Name — @Item.Value
        </ItemTemplate>
        <AlternatingItemTemplate>
            <em>@Item.Name — @Item.Value</em>
        </AlternatingItemTemplate>
        <SeparatorTemplate>
            <hr />
        </SeparatorTemplate>
        <FooterTemplate>
            <!-- footer content -->
        </FooterTemplate>
    </Repeater>
    ```

## Usage Notes

- **ItemType attribute** — Required to specify the type of items in the collection
- **Context attribute** — For Web Forms compatibility, use `Context="Item"` to access the current item as `@Item` in templates instead of Blazor's default `@context`
- **ID** — Use `@ref` instead of `ID` when referencing the component in code
- **runat** and **EnableViewState** — Not used in Blazor (these attributes are ignored)

!!! tip "No Wrapper Markup"
    The Repeater produces no additional HTML wrapper elements. The output is exactly what you define in your templates. This makes it ideal for generating custom lists, navigation menus, or any repeating structure.

## Examples

### Product List

```razor
<Repeater Items="@products" ItemType="Product" Context="Item">
    <HeaderTemplate>
        <h2>Product List</h2>
        <ul>
    </HeaderTemplate>
    <ItemTemplate>
        <li>@Item.Name — @Item.Price.ToString("C")</li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
        <p>Total: @products.Count items</p>
    </FooterTemplate>
</Repeater>

@code {
    private var products = new List<Product>
    {
        new Product { Name = "Widget", Price = 9.99m },
        new Product { Name = "Gadget", Price = 24.99m },
        new Product { Name = "Gizmo", Price = 14.99m }
    };
}
```

### Table with Alternating Rows

```razor
<Repeater Items="@employees" ItemType="Employee" Context="Item">
    <HeaderTemplate>
        <table class="table">
            <thead><tr><th>Name</th><th>Department</th></tr></thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td>@Item.Name</td>
            <td>@Item.Department</td>
        </tr>
    </ItemTemplate>
    <AlternatingItemTemplate>
        <tr style="background-color: #f5f5f5;">
            <td>@Item.Name</td>
            <td>@Item.Department</td>
        </tr>
    </AlternatingItemTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
</Repeater>
```

### Navigation Menu with Separators

```razor
<Repeater Items="@menuItems" ItemType="MenuItem" Context="Item">
    <ItemTemplate>
        <a href="@Item.Url">@Item.Label</a>
    </ItemTemplate>
    <SeparatorTemplate>
        <span> | </span>
    </SeparatorTemplate>
</Repeater>
```

## Migration Notes

1. **Remove `asp:` prefix** — Change `<asp:Repeater>` to `<Repeater>`
2. **Remove `runat="server"`** — Not needed in Blazor
3. **Add `ItemType`** — Specify the data item type: `ItemType="Product"`
4. **Add `Context="Item"`** — To use `@Item` in templates (matching Web Forms `<%# Container.DataItem %>`)
5. **Replace data binding** — Change `DataSourceID` or code-behind `DataSource`/`DataBind()` to `Items="@collection"` or `SelectMethod`
6. **Replace `<%# Eval("Name") %>`** — Use `@Item.Name` with strongly typed binding
7. **Templates are identical** — `HeaderTemplate`, `ItemTemplate`, `AlternatingItemTemplate`, `SeparatorTemplate`, `FooterTemplate` all work the same way

### Migration Example

=== "Web Forms"

    ```html
    <asp:Repeater ID="rptProducts" DataSourceID="SqlDS" runat="server">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li><%# Eval("Name") %> - <%# Eval("Price", "{0:C}") %></li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>
    ```

=== "Blazor"

    ```razor
    <Repeater Items="@products" ItemType="Product" Context="Item">
        <HeaderTemplate>
            <ul>
        </HeaderTemplate>
        <ItemTemplate>
            <li>@Item.Name - @Item.Price.ToString("C")</li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </Repeater>
    ```

## See Also

- [DataList](DataList.md) — Repeating data with table/flow layout options
- [DataGrid](DataGrid.md) — Legacy grid control
- [ListView](ListView.md) — Full-featured list with CRUD, paging, and grouping
- [GridView](GridView.md) — Tabular data grid
- [DataPager](DataPager.md) — Shared paging for multiple controls

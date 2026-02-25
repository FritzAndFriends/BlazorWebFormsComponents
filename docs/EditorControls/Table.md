# Table

The Table component provides a container for table rows and cells, rendering as an HTML `<table>` element. It includes related components: TableRow, TableCell, TableHeaderCell, TableHeaderRow, and TableFooterRow.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.table?view=netframework-4.8

## Features Supported in Blazor

### Table Component

- **Caption** - Table caption text
- **CaptionAlign** - Position of caption (Top, Bottom)
- **CellPadding** - Padding within cells in pixels
- **CellSpacing** - Spacing between cells in pixels
- **GridLines** - Display of grid lines (None, Horizontal, Vertical, Both)
- **HorizontalAlign** - Table alignment (Left, Center, Right)
- **BackImageUrl** - Background image for the table
- All style properties (CssClass, BackColor, etc.)

### TableRow Component

- **TableSection** - Where the row belongs (TableHeader, TableBody, TableFooter)
- **HorizontalAlign** - Horizontal alignment of row content
- **VerticalAlign** - Vertical alignment of row content
- All style properties

### TableCell Component

- **ColumnSpan** - Number of columns the cell spans
- **RowSpan** - Number of rows the cell spans
- **HorizontalAlign** / **VerticalAlign** - Content alignment
- **Wrap** - Whether content wraps (default: true)
- **Text** - Text content of the cell
- **AssociatedHeaderCellID** - Accessibility association with header

### TableHeaderCell Component

- All TableCell properties plus:
- **Scope** - Header scope for accessibility (Row, Column)
- **AbbreviatedText** - Abbreviated header text for accessibility

### Specialized Row Components

- **TableHeaderRow** - Renders in `<thead>` section
- **TableFooterRow** - Renders in `<tfoot>` section

## Web Forms Features NOT Supported

- **Rows collection** - Use declarative child content instead
- **Programmatic row generation** - Build rows in Blazor code

## Web Forms Declarative Syntax

```html
<asp:Table
    ID="Table1"
    Caption="Product List"
    CaptionAlign="Top"
    CellPadding="5"
    CellSpacing="0"
    GridLines="Both"
    HorizontalAlign="Center"
    runat="server">
    <asp:TableHeaderRow>
        <asp:TableHeaderCell Scope="Column">Name</asp:TableHeaderCell>
        <asp:TableHeaderCell Scope="Column">Price</asp:TableHeaderCell>
    </asp:TableHeaderRow>
    <asp:TableRow>
        <asp:TableCell>Widget</asp:TableCell>
        <asp:TableCell>$10.00</asp:TableCell>
    </asp:TableRow>
    <asp:TableFooterRow>
        <asp:TableCell ColumnSpan="2">Total: $10.00</asp:TableCell>
    </asp:TableFooterRow>
</asp:Table>
```

## Blazor Syntax

### Basic Table

```razor
<Table>
    <TableRow>
        <TableCell>Cell 1</TableCell>
        <TableCell>Cell 2</TableCell>
    </TableRow>
    <TableRow>
        <TableCell>Cell 3</TableCell>
        <TableCell>Cell 4</TableCell>
    </TableRow>
</Table>
```

### Table with Header and Footer

```razor
<Table Caption="Product Inventory" GridLines="GridLines.Both" CellPadding="5">
    <TableHeaderRow>
        <TableHeaderCell Scope="TableHeaderScope.Column">Product</TableHeaderCell>
        <TableHeaderCell Scope="TableHeaderScope.Column">Quantity</TableHeaderCell>
        <TableHeaderCell Scope="TableHeaderScope.Column">Price</TableHeaderCell>
    </TableHeaderRow>
    <TableRow>
        <TableCell>Widget A</TableCell>
        <TableCell HorizontalAlign="HorizontalAlign.Right">50</TableCell>
        <TableCell HorizontalAlign="HorizontalAlign.Right">$25.00</TableCell>
    </TableRow>
    <TableRow>
        <TableCell>Widget B</TableCell>
        <TableCell HorizontalAlign="HorizontalAlign.Right">30</TableCell>
        <TableCell HorizontalAlign="HorizontalAlign.Right">$15.00</TableCell>
    </TableRow>
    <TableFooterRow>
        <TableCell ColumnSpan="2">Total</TableCell>
        <TableCell HorizontalAlign="HorizontalAlign.Right">$1,700.00</TableCell>
    </TableFooterRow>
</Table>
```

### Cell Spanning

```razor
<Table GridLines="GridLines.Both" CellPadding="10">
    <TableRow>
        <TableCell ColumnSpan="2" HorizontalAlign="HorizontalAlign.Center">
            Header spanning 2 columns
        </TableCell>
    </TableRow>
    <TableRow>
        <TableCell RowSpan="2">Spans 2 rows</TableCell>
        <TableCell>Row 1</TableCell>
    </TableRow>
    <TableRow>
        <TableCell>Row 2</TableCell>
    </TableRow>
</Table>
```

### Accessible Table with Scope

```razor
<Table Caption="Sales Report" GridLines="GridLines.Both">
    <TableHeaderRow>
        <TableHeaderCell></TableHeaderCell>
        <TableHeaderCell Scope="TableHeaderScope.Column">Q1</TableHeaderCell>
        <TableHeaderCell Scope="TableHeaderScope.Column">Q2</TableHeaderCell>
    </TableHeaderRow>
    <TableRow>
        <TableHeaderCell Scope="TableHeaderScope.Row">North</TableHeaderCell>
        <TableCell>$100</TableCell>
        <TableCell>$150</TableCell>
    </TableRow>
    <TableRow>
        <TableHeaderCell Scope="TableHeaderScope.Row">South</TableHeaderCell>
        <TableCell>$200</TableCell>
        <TableCell>$175</TableCell>
    </TableRow>
</Table>
```

### Styled Table

```razor
<Table CssClass="data-table" 
       CellPadding="8" 
       CellSpacing="0" 
       GridLines="GridLines.Horizontal"
       HorizontalAlign="HorizontalAlign.Center">
    <TableHeaderRow CssClass="header-row" BackColor="WebColor.LightGray">
        <TableHeaderCell>Name</TableHeaderCell>
        <TableHeaderCell>Status</TableHeaderCell>
    </TableHeaderRow>
    <TableRow>
        <TableCell>Item 1</TableCell>
        <TableCell ForeColor="WebColor.Green">Active</TableCell>
    </TableRow>
</Table>
```

## HTML Output

**Blazor:**
```razor
<Table Caption="Data" CaptionAlign="TableCaptionAlign.Top" GridLines="GridLines.Both" CellPadding="5">
    <TableHeaderRow>
        <TableHeaderCell Scope="TableHeaderScope.Column">Header</TableHeaderCell>
    </TableHeaderRow>
    <TableRow>
        <TableCell>Data</TableCell>
    </TableRow>
</Table>
```

**Rendered HTML:**
```html
<table cellpadding="5" border="1" rules="all" style="border-collapse: collapse">
    <caption style="caption-side: top">Data</caption>
    <thead>
        <tr>
            <th scope="col">Header</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Data</td>
        </tr>
    </tbody>
</table>
```

## GridLines Property Values

| Value | HTML Output |
|-------|-------------|
| `GridLines.None` | No border or rules |
| `GridLines.Horizontal` | `rules="rows"` |
| `GridLines.Vertical` | `rules="cols"` |
| `GridLines.Both` | `rules="all" border="1"` |

## Migration Notes

When migrating from Web Forms to Blazor:

1. Remove the `asp:` prefix and `runat="server"` attribute
2. Use enum types with full qualification (e.g., `GridLines.Both`)
3. Replace programmatic row generation with Blazor `@foreach` loops
4. The component uses standard `<thead>`, `<tbody>`, `<tfoot>` sections

### Before (Web Forms):
```html
<asp:Table ID="tblProducts" GridLines="Both" runat="server">
    <asp:TableRow>
        <asp:TableCell>Data</asp:TableCell>
    </asp:TableRow>
</asp:Table>
```

### After (Blazor):
```razor
<Table GridLines="GridLines.Both">
    <TableRow>
        <TableCell>Data</TableCell>
    </TableRow>
</Table>
```

### Dynamic Rows

```razor
<Table GridLines="GridLines.Both">
    <TableHeaderRow>
        <TableHeaderCell>Name</TableHeaderCell>
        <TableHeaderCell>Value</TableHeaderCell>
    </TableHeaderRow>
    @foreach (var item in Items)
    {
        <TableRow>
            <TableCell>@item.Name</TableCell>
            <TableCell>@item.Value</TableCell>
        </TableRow>
    }
</Table>
```

## See Also

- [GridView](../DataControls/GridView.md) - Data-bound grid control
- [DataList](../DataControls/DataList.md) - Data-bound repeating control
- [Panel](Panel.md) - Container control

# Styling Components

Style sub-components are special Blazor components that define the appearance of rows, headers, footers, and other UI elements within data controls and complex layout controls. They use the **cascading parameter pattern** to propagate styling configuration from parent controls to their child elements.

The library includes **66 style sub-components** used across GridView, DetailsView, DataList, FormView, DataGrid, Calendar, Menu, TreeView, and Login controls. This documentation explains how they work and how to use them when migrating from Web Forms.

## Overview

In Web Forms, properties like `HeaderStyle`, `RowStyle`, and `AlternatingRowStyle` accepted a `TableItemStyle` object that could be configured:

```asp
<asp:GridView ID="gvCustomers" runat="server">
    <HeaderStyle BackColor="Navy" ForeColor="White" Font-Bold="true" />
    <RowStyle BackColor="White" />
    <AlternatingRowStyle BackColor="LightGray" />
</asp:GridView>
```

These style properties accepted inline configuration and rendered CSS styles to the corresponding HTML elements.

In Blazor, the same styles are applied using **style sub-components** that accept the same parameters and apply styles in the same way, using the cascading parameter pattern:

```razor
<GridView ItemsSource="customers">
    <GridViewHeaderStyle BackColor="Navy" ForeColor="White" Font-Bold="true" />
    <GridViewRowStyle BackColor="White" />
    <GridViewAlternatingRowStyle BackColor="LightGray" />
</GridView>
```

## How Style Sub-Components Work

Style sub-components inherit from `UiStyle` or `UiTableItemStyle` base classes and use the **cascading parameter pattern** to pass styling configuration to their parent control:

1. **Style Definition**: A style sub-component (e.g., `<GridViewHeaderStyle>`) is declared as a child of a data control
2. **Style Object Creation**: The parent control creates a style object (e.g., `HeaderStyle` property) and registers it
3. **Cascading Parameter**: The parent control cascades the style object to all style sub-components as a cascading parameter
4. **Style Configuration**: The style sub-component applies its parameters (BackColor, ForeColor, BorderWidth, etc.) to the cascaded style object
5. **Rendering**: When the parent control renders, it applies the configured styles to the appropriate HTML elements

## Common Style Parameters

All style sub-components support a common set of parameters for controlling appearance:

### Color and Border Properties
- **BackColor**: Background color (any named color, hex value, or RGB)
- **ForeColor**: Text/foreground color
- **BorderColor**: Border color
- **BorderStyle**: Border style (Solid, Dotted, Dashed, Double, Groove, Ridge, Inset, Outset)
- **BorderWidth**: Border width (pixels or units)

### Sizing Properties
- **Width**: Element width (pixels, percentage, or units)
- **Height**: Element height (pixels, percentage, or units)

### Text Properties
- **CssClass**: CSS class name(s) to apply to the element
- **Font-Bold**: Apply bold font weight
- **Font-Italic**: Apply italic font style
- **Font-Names**: Comma-separated font family names
- **Font-Size**: Font size (pixels or units)
- **Font-Underline**: Apply text underline
- **Font-Strikeout**: Apply strikethrough

### Table Cell Properties (Table Item Styles Only)
- **HorizontalAlign**: Text alignment (Left, Center, Right, Justify)
- **VerticalAlign**: Vertical alignment (Top, Middle, Bottom)
- **Wrap**: Whether text wraps in the cell (default: true)

### Additional Attributes
- **AdditionalAttributes**: Capture any unmatched attributes for direct HTML styling

## Style Sub-Component Categories

Style sub-components fall into several categories based on which controls use them:

### GridView Styles
- `<GridViewHeaderStyle>` — Header row styling
- `<GridViewFooterStyle>` — Footer row styling
- `<GridViewRowStyle>` — Data row styling
- `<GridViewAlternatingRowStyle>` — Alternating row styling
- `<GridViewSelectedRowStyle>` — Selected/highlighted row styling
- `<GridViewEditRowStyle>` — Edit mode row styling
- `<GridViewEmptyDataRowStyle>` — Empty data message styling
- `<GridViewPagerStyle>` — Pager controls styling

### DetailsView Styles
- `<DetailsViewHeaderStyle>` — Header styling
- `<DetailsViewFooterStyle>` — Footer styling
- `<DetailsViewRowStyle>` — Field row styling
- `<DetailsViewAlternatingRowStyle>` — Alternating field row styling
- `<DetailsViewEditRowStyle>` — Edit mode row styling
- `<DetailsViewInsertRowStyle>` — Insert mode row styling
- `<DetailsViewCommandRowStyle>` — Command button row styling
- `<DetailsViewFieldHeaderStyle>` — Field label column styling
- `<DetailsViewEmptyDataRowStyle>` — Empty data message styling
- `<DetailsViewPagerStyle>` — Pager controls styling

### FormView Styles
- `<FormViewHeaderStyle>` — Header styling
- `<FormViewFooterStyle>` — Footer styling
- `<FormViewRowStyle>` — Content row styling
- `<FormViewAlternatingRowStyle>` — Alternating row styling
- `<FormViewEditRowStyle>` — Edit mode styling
- `<FormViewInsertRowStyle>` — Insert mode styling
- `<FormViewEmptyDataRowStyle>` — Empty data message styling
- `<FormViewPagerStyle>` — Pager controls styling

### DataList Styles
- `<AlternatingItemStyle>` — Alternating item styling

### DataGrid Styles
- `<DataGridHeaderStyle>` — Header styling
- `<DataGridFooterStyle>` — Footer styling
- `<DataGridItemStyle>` — Item styling
- `<DataGridAlternatingItemStyle>` — Alternating item styling
- `<DataGridEditItemStyle>` — Edit mode styling
- `<DataGridSelectedItemStyle>` — Selected item styling
- `<DataGridPagerStyle>` — Pager styling

### Calendar Styles
- `<CalendarTitleStyle>` — Month/year title styling
- `<CalendarNextPrevStyle>` — Next/previous navigation buttons styling
- `<CalendarDayHeaderStyle>` — Day-of-week header styling
- `<CalendarDayStyle>` — Regular day cell styling
- `<CalendarWeekendDayStyle>` — Weekend day cell styling
- `<CalendarSelectedDayStyle>` — Selected date cell styling
- `<CalendarTodayDayStyle>` — Today's date cell styling
- `<CalendarOtherMonthDayStyle>` — Other month's dates styling
- `<CalendarSelectorStyle>` — Month/year selector styling

### Menu Styles
- `<MenuItemStyle>` — Menu item styling
- `<SeparatorStyle>` — Menu separator styling

### TreeView Styles
- `<TreeViewNodeStyle>` — General node styling
- `<TreeViewRootNodeStyle>` — Root-level node styling
- `<TreeViewParentNodeStyle>` — Parent node styling
- `<TreeViewLeafNodeStyle>` — Leaf node styling
- `<TreeViewSelectedNodeStyle>` — Selected node styling
- `<TreeViewHoverNodeStyle>` — Hover state styling

### Login Control Styles
- `<CheckBoxStyle>` — Checkbox styling
- `<HyperLinkStyle>` — Hyperlink styling
- `<LabelStyle>` — Label styling
- `<TextBoxStyle>` — Text input styling
- `<LoginButtonStyle>` — Button styling
- `<InstructionTextStyle>` — Instruction text styling
- `<FailureTextStyle>` — Error message styling
- `<SuccessTextStyle>` — Success message styling
- `<TitleTextStyle>` — Title text styling
- `<ValidatorTextStyle>` — Validator message styling

## Usage Examples

### GridView with Styled Rows

**Before (Web Forms):**
```asp
<asp:GridView ID="gvProducts" runat="server" DataSource="products">
    <HeaderStyle BackColor="Navy" ForeColor="White" Font-Bold="true" HorizontalAlign="Center" />
    <RowStyle BackColor="White" />
    <AlternatingRowStyle BackColor="LightGray" />
    <SelectedRowStyle BackColor="Yellow" ForeColor="Black" />
    <Columns>
        <asp:BoundField DataField="ProductName" HeaderText="Product" />
        <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="{0:C}" />
    </Columns>
</asp:GridView>
```

**After (Blazor):**
```razor
<GridView ItemsSource="products">
    <GridViewHeaderStyle BackColor="Navy" ForeColor="White" Font-Bold="true" HorizontalAlign="Center" />
    <GridViewRowStyle BackColor="White" />
    <GridViewAlternatingRowStyle BackColor="LightGray" />
    <GridViewSelectedRowStyle BackColor="Yellow" ForeColor="Black" />
    <BoundColumn DataField="ProductName" HeaderText="Product" />
    <BoundColumn DataField="Price" HeaderText="Price" DataFormatString="{0:C}" />
</GridView>
```

### Calendar with Themed Days

**Before (Web Forms):**
```asp
<asp:Calendar ID="cal1" runat="server">
    <TitleStyle BackColor="Navy" ForeColor="White" Font-Bold="true" />
    <DayStyle BackColor="White" />
    <WeekendDayStyle BackColor="LightGray" />
    <SelectedDayStyle BackColor="Gold" ForeColor="Black" Font-Bold="true" />
</asp:Calendar>
```

**After (Blazor):**
```razor
<Calendar>
    <CalendarTitleStyle BackColor="Navy" ForeColor="White" Font-Bold="true" />
    <CalendarDayStyle BackColor="White" />
    <CalendarWeekendDayStyle BackColor="LightGray" />
    <CalendarSelectedDayStyle BackColor="Gold" ForeColor="Black" Font-Bold="true" />
</Calendar>
```

### DetailsView with Alternating Row Colors

**Before (Web Forms):**
```asp
<asp:DetailsView ID="dvProduct" runat="server" DataSource="product">
    <FieldHeaderStyle BackColor="LightGray" Font-Bold="true" />
    <RowStyle BackColor="White" />
    <AlternatingRowStyle BackColor="LightCyan" />
</asp:DetailsView>
```

**After (Blazor):**
```razor
<DetailsView ItemsSource="product">
    <DetailsViewFieldHeaderStyle BackColor="LightGray" Font-Bold="true" />
    <DetailsViewRowStyle BackColor="White" />
    <DetailsViewAlternatingRowStyle BackColor="LightCyan" />
</DetailsView>
```

### TreeView with Node-Level Styling

**Before (Web Forms):**
```asp
<asp:TreeView ID="tvMenu" runat="server">
    <NodeStyle BackColor="White" ForeColor="Black" />
    <RootNodeStyle BackColor="Navy" ForeColor="White" Font-Bold="true" />
    <LeafNodeStyle BackColor="White" ForeColor="Black" />
    <SelectedNodeStyle BackColor="Yellow" ForeColor="Black" />
</asp:TreeView>
```

**After (Blazor):**
```razor
<TreeView>
    <TreeViewNodeStyle BackColor="White" ForeColor="Black" />
    <TreeViewRootNodeStyle BackColor="Navy" ForeColor="White" Font-Bold="true" />
    <TreeViewLeafNodeStyle BackColor="White" ForeColor="Black" />
    <TreeViewSelectedNodeStyle BackColor="Yellow" ForeColor="Black" />
</TreeView>
```

### Login Control with Custom Styling

**Before (Web Forms):**
```asp
<asp:Login ID="login1" runat="server">
    <LoginButtonStyle BackColor="Navy" ForeColor="White" Font-Bold="true" />
    <TextBoxStyle BorderWidth="1px" BorderColor="Gray" />
    <LabelStyle Font-Bold="true" />
    <FailureTextStyle ForeColor="Red" Font-Italic="true" />
</asp:Login>
```

**After (Blazor):**
```razor
<Login>
    <LoginButtonStyle BackColor="Navy" ForeColor="White" Font-Bold="true" />
    <TextBoxStyle BorderWidth="1px" BorderColor="Gray" />
    <LabelStyle Font-Bold="true" />
    <FailureTextStyle ForeColor="Red" Font-Italic="true" />
</Login>
```

## Migration Tips

When migrating from Web Forms:

1. **Identify styled regions**: Look for the `<XxxStyle>` properties in your Web Forms controls
2. **Replace with Blazor equivalents**: Use the corresponding `<XxxStyle>` components in your Blazor markup
3. **Match parameter names**: The parameter names are identical to Web Forms (BackColor, ForeColor, Font-Bold, etc.)
4. **Test rendering**: Verify that the rendered HTML matches your Web Forms output and that your existing CSS continues to work
5. **Use CSS classes**: For complex styling, consider using the `CssClass` parameter and moving styles to CSS files

## Style Object Architecture

The style system is built on several base classes:

- **UiStyle**: Base class for simple styles with color, border, sizing, and font properties
- **UiTableItemStyle**: Extends UiStyle with horizontal alignment, vertical alignment, and text wrapping (used for grid/table rows)
- **Style**: The C# model class that holds styled property values and renders CSS inline styles

When a style sub-component is configured, its parameters are passed to an internal `Style` object that:
- Validates property values
- Converts color and unit values to proper formats
- Generates CSS inline styles for rendering
- Provides the `ToString()` method to output the style attribute

## Best Practices

1. **Use Consistent Naming**: Follow the "Xxx**Control**Yyy**Style**" naming pattern for clarity (e.g., GridViewHeaderStyle, CalendarSelectedDayStyle)
2. **Keep Styles Focused**: Use each style sub-component for a single visual purpose (header row, alternating rows, etc.)
3. **Avoid Inline Style Overuse**: For complex layouts, consider using CSS classes instead of multiple inline style parameters
4. **Test Visual Consistency**: Ensure migrated styles match the original Web Forms appearance
5. **Document Custom Styles**: If you create custom controls with style sub-components, document which style sub-components they support
6. **Consider Accessibility**: Use sufficient color contrast in BackColor/ForeColor combinations for accessibility compliance

## Related Documentation

- [ID Rendering](IDRendering.md) — Controlling element IDs for JavaScript integration
- [ViewState](ViewState.md) — Understanding state management for styled controls
- [Databinder](Databinder.md) — Data binding patterns within styled data controls
- [WebFormsPage](WebFormsPage.md) — Full-page theming and naming container configuration

## Common Issues and Solutions

### Styles Not Applying

**Problem**: Style properties set but HTML doesn't show the styles.  
**Solution**: Ensure the style sub-component is declared as a child of the control. The parent control must recognize it as a cascading parameter receiver.

### CSS Classes Not Working with Inline Styles

**Problem**: CssClass parameter doesn't override inline styles.  
**Solution**: Use CSS specificity rules or !important flags in your CSS to override inline styles if needed.

### Font Parameters Not Recognized

**Problem**: Font-Bold, Font-Italic, etc., showing as "unmatched attributes."  
**Solution**: Use PascalCase with hyphens: `Font-Bold="true"`, `Font-Italic="true"`, `Font-Names="Arial,Helvetica"`.

---

For complete working examples, see the samples in the repository's sample applications and migration test cases.

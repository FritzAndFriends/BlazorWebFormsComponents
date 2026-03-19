# ListSearchExtender

The **ListSearchExtender** enables search and filter functionality on a ListBox or DropDownList control. As the user types in a search field, items that don't match are filtered, or if auto-select is enabled, the selection jumps to the first matching item. Supports configurable search patterns (Contains or StartsWith) and sorted list optimization.

Original Ajax Control Toolkit documentation: https://www.asp.net/ajax/ajaxcontroltoolkit/ListSearchExtender

## Features Supported in Blazor

- `TargetControlID` — ID of the ListBox or DropDownList to search
- `PromptText` — Placeholder text for the search input
- `PromptCssClass` — CSS class for search prompt styling
- `PromptPosition` — Where to position the search prompt (Top, Bottom)
- `IsSorted` — Whether list items are sorted for optimized binary search
- `QueryPattern` — Search pattern type (Contains, StartsWith)
- `Enabled` — Enable or disable the extender behavior
- `BehaviorID` — Optional identifier for JavaScript behavior lookup

## PromptPosition Enum

Controls search prompt placement:

```csharp
enum PromptPosition
{
    Top = 0,
    Bottom = 1
}
```

## QueryPattern Enum

Controls search matching:

```csharp
enum QueryPattern
{
    Contains = 0,    // Match anywhere in text
    StartsWith = 1   // Match from beginning only
}
```

## Web Forms Syntax

```html
<asp:ListBox ID="lstCountries" runat="server" Height="150px">
    <asp:ListItem>Afghanistan</asp:ListItem>
    <asp:ListItem>Albania</asp:ListItem>
    <asp:ListItem>Algeria</asp:ListItem>
    <!-- ... more items ... -->
</asp:ListBox>

<ajaxToolkit:ListSearchExtender
    ID="search1"
    runat="server"
    TargetControlID="lstCountries"
    PromptText="Type to search..."
    PromptPosition="Top"
    IsSorted="true"
    QueryPattern="StartsWith" />
```

## Blazor Migration

```razor
<ListBox ID="lstCountries" style="height: 150px;">
    <ListItem>Afghanistan</ListItem>
    <ListItem>Albania</ListItem>
    <ListItem>Algeria</ListItem>
    <!-- ... more items ... -->
</ListBox>

<ListSearchExtender
    TargetControlID="lstCountries"
    PromptText="Type to search..."
    PromptPosition="PromptPosition.Top"
    IsSorted="true"
    QueryPattern="QueryPattern.StartsWith" />
```

**Migration is simple:** Remove the `ajaxToolkit:` prefix and `runat="server"`. Use enum type names!

## Properties Reference

| Property | Type | Default | Description |
|---|---|---|---|
| `TargetControlID` | `string` | (required) | ID of the ListBox or DropDownList to search |
| `PromptText` | `string` | `"Type to search"` | Placeholder text shown in the search input when empty |
| `PromptCssClass` | `string` | `""` | CSS class applied to the search prompt text |
| `PromptPosition` | `PromptPosition` | `Top` | Where to display the search prompt relative to the list |
| `IsSorted` | `bool` | `false` | Whether list items are sorted; enables optimized binary search |
| `QueryPattern` | `QueryPattern` | `Contains` | Search pattern type (Contains matches anywhere, StartsWith from beginning) |
| `BehaviorID` | `string` | TargetControlID | Optional identifier for JavaScript behavior lookup |
| `Enabled` | `bool` | `true` | Whether the extender is active |

## Usage Examples

### Basic List Search

```razor
@rendermode InteractiveServer

<div>
    <label>Select a Country:</label>
    
    <ListBox ID="lstCountries" style="width: 200px; height: 200px;">
        <ListItem>Afghanistan</ListItem>
        <ListItem>Albania</ListItem>
        <ListItem>Algeria</ListItem>
        <ListItem>Andorra</ListItem>
        <ListItem>Angola</ListItem>
        <ListItem>Argentina</ListItem>
        <!-- ... more countries ... -->
    </ListBox>
    
    <ListSearchExtender
        TargetControlID="lstCountries"
        PromptText="Search countries..."
        PromptPosition="PromptPosition.Top"
        IsSorted="true"
        QueryPattern="QueryPattern.StartsWith" />
</div>
```

### Dropdown List Search

```razor
@rendermode InteractiveServer

<div>
    <label>Select a State:</label>
    
    <DropDownList ID="ddlStates" style="width: 200px;">
        <ListItem Value="">-- Select a State --</ListItem>
        <ListItem Value="AL">Alabama</ListItem>
        <ListItem Value="AK">Alaska</ListItem>
        <ListItem Value="AZ">Arizona</ListItem>
        <ListItem Value="AR">Arkansas</ListItem>
        <ListItem Value="CA">California</ListItem>
        <!-- ... more states ... -->
    </DropDownList>
    
    <ListSearchExtender
        TargetControlID="ddlStates"
        PromptText="Type state name..."
        PromptPosition="PromptPosition.Top"
        IsSorted="true"
        QueryPattern="QueryPattern.StartsWith" />
</div>
```

### Search with Contains Pattern

```razor
@rendermode InteractiveServer

<div>
    <label>Find a Product:</label>
    
    <ListBox ID="lstProducts" style="width: 250px; height: 200px;">
        <ListItem>Widget Pro</ListItem>
        <ListItem>Pro Widget</ListItem>
        <ListItem>Super Widget</ListItem>
        <ListItem>Widget Deluxe</ListItem>
        <ListItem>Gadget Pro</ListItem>
        <!-- ... more products ... -->
    </ListBox>
    
    <ListSearchExtender
        TargetControlID="lstProducts"
        PromptText="Search by product name..."
        PromptPosition="PromptPosition.Top"
        IsSorted="false"
        QueryPattern="QueryPattern.Contains" />
</div>
```

### Search at Bottom

```razor
@rendermode InteractiveServer

<div>
    <label>Available Themes:</label>
    
    <ListBox ID="lstThemes" style="width: 200px; height: 150px;">
        <ListItem>Dark Modern</ListItem>
        <ListItem>Light Classic</ListItem>
        <ListItem>Blue Ocean</ListItem>
        <ListItem>Forest Green</ListItem>
        <ListItem>Sunset Orange</ListItem>
    </ListBox>
    
    <ListSearchExtender
        TargetControlID="lstThemes"
        PromptText="Filter themes..."
        PromptPosition="PromptPosition.Bottom"
        IsSorted="false"
        QueryPattern="QueryPattern.Contains"
        PromptCssClass="search-prompt" />
</div>

<style>
    .search-prompt {
        color: #999;
        font-style: italic;
        font-size: 12px;
    }
</style>
```

## HTML Output

The ListSearchExtender creates a search input field positioned above or below the list control and filters the list items based on user input.

## JavaScript Interop

The ListSearchExtender loads `list-search-extender.js` as an ES module. JavaScript handles:

- Creating and positioning the search input field
- Listening to keyup events in the search field
- Matching items against search text (Contains or StartsWith)
- Showing/hiding list items based on matches
- Auto-selecting first match if configured
- Optimized binary search if list is sorted
- Applying CSS classes to prompt

## Render Mode Requirements

The ListSearchExtender requires **InteractiveServer** render mode:

```razor
@rendermode InteractiveServer
```

### Graceful Degradation

- **SSR/Static mode:** The extender silently skips initialization. List displays without search.
- **JavaScript disabled:** Same as SSR — No search functionality.

## Migration Notes

### From Web Forms Ajax Toolkit

1. **Remove `ajaxToolkit:` prefix**
   ```diff
   - <ajaxToolkit:ListSearchExtender
   + <ListSearchExtender
   ```

2. **Remove `runat="server"` and `ID` attributes**

3. **Use enum types for positions and patterns**
   ```diff
   - PromptPosition="Top"
   + PromptPosition="PromptPosition.Top"
   - QueryPattern="StartsWith"
   + QueryPattern="QueryPattern.StartsWith"
   ```

### Before (Web Forms)

```html
<asp:ListBox ID="lstCountries" runat="server" Height="150px">
    <asp:ListItem>Afghanistan</asp:ListItem>
    <asp:ListItem>Albania</asp:ListItem>
    <!-- ... -->
</asp:ListBox>

<ajaxToolkit:ListSearchExtender
    ID="search1"
    TargetControlID="lstCountries"
    PromptText="Search..."
    IsSorted="true"
    QueryPattern="StartsWith"
    runat="server" />
```

### After (Blazor)

```razor
<ListBox ID="lstCountries" style="height: 150px;">
    <ListItem>Afghanistan</ListItem>
    <ListItem>Albania</ListItem>
    <!-- ... -->
</ListBox>

<ListSearchExtender
    TargetControlID="lstCountries"
    PromptText="Search..."
    IsSorted="true"
    QueryPattern="QueryPattern.StartsWith" />
```

## Best Practices

1. **Set IsSorted for large lists** — If your list is sorted, enable `IsSorted="true"` for better performance
2. **Choose QueryPattern wisely** — Use `StartsWith` for name lists, `Contains` for flexible matching
3. **Keep prompt text short** — Use concise placeholder text (e.g., "Search..." vs "Type to search...")
4. **Position search appropriately** — Top is default and typically preferred
5. **Test with many items** — Verify performance with 100+ list items
6. **Style the search** — Use CSS to make the search input match your design

## Troubleshooting

| Issue | Solution |
|---|---|
| Search not appearing | Verify `TargetControlID` matches the ListBox/DropDownList ID. Ensure `@rendermode InteractiveServer` is set. |
| Search not filtering | Verify list items are being rendered. Check that `IsSorted` matches your actual list order. |
| Performance issues with large lists | Enable `IsSorted="true"` if your list is sorted for optimized binary search. |
| StartsWith not working as expected | Verify items start with search text (case-sensitive). Try `QueryPattern="QueryPattern.Contains"`. |
| Search prompt not visible | Check CSS styling. Verify `PromptPosition` value is valid (Top or Bottom). |

## See Also

- [Ajax Control Toolkit Overview](index.md) — How extenders work and render mode requirements
- ListBox Component — The ListBox control (documentation coming soon)
- DropDownList Component — The DropDownList control (documentation coming soon)
- Original Ajax Control Toolkit: https://www.asp.net/ajax/ajaxcontroltoolkit

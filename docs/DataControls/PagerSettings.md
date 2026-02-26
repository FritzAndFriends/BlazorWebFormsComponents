# PagerSettings

The **PagerSettings** class represents the properties of paging controls in data-bound components that support pagination. It is a shared sub-component used by [FormView](FormView.md), [DetailsView](DetailsView.md), and [GridView](GridView.md) to configure pager button behavior, text, images, and position.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.pagersettings?view=netframework-4.8

## Features Supported in Blazor

- **Mode** — Pager button style: `Numeric`, `NextPrevious`, `NextPreviousFirstLast`, `NumericFirstLast`
- **PageButtonCount** — Number of numeric page buttons to display (default: 10)
- **FirstPageText** / **LastPageText** — Text for First and Last page buttons
- **NextPageText** / **PreviousPageText** — Text for Next and Previous page buttons
- **FirstPageImageUrl** / **LastPageImageUrl** — Image URLs for First and Last page buttons
- **NextPageImageUrl** / **PreviousPageImageUrl** — Image URLs for Next and Previous page buttons
- **Position** — Pager placement: `Bottom` (default), `Top`, or `TopAndBottom`
- **Visible** — Show or hide the pager (default: `true`)

### Blazor Notes

- PagerSettings is not a standalone component — it is configured as a child element of a data-bound control (FormView, DetailsView, GridView).
- In Blazor, PagerSettings is specified using a `<PagerSettings>` child element inside the parent component, similar to how style sub-components work.
- The `PagerButtons` enum replaces the Web Forms `PagerButtons` enumeration with identical values.

## Web Forms Features NOT Supported

- **OnPropertyChanged** event — Not applicable in Blazor's reactive model
- **RenderNonBreakingSpacesBetweenControls** — Not implemented

## Web Forms Declarative Syntax

In Web Forms, `PagerSettings` is configured as a child element or via dash-separated attributes:

```html
<asp:FormView ID="FormView1" runat="server" AllowPaging="True">
    <PagerSettings
        Mode="NextPreviousFirstLast"
        FirstPageText="First"
        LastPageText="Last"
        NextPageText="Next"
        PreviousPageText="Previous"
        PageButtonCount="10"
        Position="Bottom"
        Visible="True"
        FirstPageImageUrl="~/images/first.gif"
        LastPageImageUrl="~/images/last.gif"
        NextPageImageUrl="~/images/next.gif"
        PreviousPageImageUrl="~/images/prev.gif"
    />
    <ItemTemplate>
        <!-- content -->
    </ItemTemplate>
</asp:FormView>
```

Or using dash-separated attributes on the parent control:

```html
<asp:GridView ID="GridView1" runat="server"
    AllowPaging="True"
    PagerSettings-Mode="NumericFirstLast"
    PagerSettings-FirstPageText="First"
    PagerSettings-LastPageText="Last"
    PagerSettings-PageButtonCount="5"
    PagerSettings-Position="TopAndBottom" />
```

## Blazor Syntax

```razor
<FormView DataSource="@Items" ItemType="Product" AllowPaging="true">
    <PagerSettings
        Mode="PagerButtons.NextPreviousFirstLast"
        FirstPageText="First"
        LastPageText="Last"
        NextPageText="Next >"
        PreviousPageText="< Previous"
        PageButtonCount="10"
        Position="PagerPosition.Bottom"
        Visible="true" />
    <ItemTemplate Context="Item">
        <!-- content -->
    </ItemTemplate>
</FormView>
```

## Properties Reference

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Mode` | `PagerButtons` | `Numeric` | The style of pager buttons to display |
| `PageButtonCount` | `int` | `10` | Number of numeric page buttons shown |
| `FirstPageText` | `string` | `"..."` | Text for the First page button |
| `LastPageText` | `string` | `"..."` | Text for the Last page button |
| `NextPageText` | `string` | `">"` | Text for the Next page button |
| `PreviousPageText` | `string` | `"<"` | Text for the Previous page button |
| `FirstPageImageUrl` | `string` | `null` | Image URL for the First page button |
| `LastPageImageUrl` | `string` | `null` | Image URL for the Last page button |
| `NextPageImageUrl` | `string` | `null` | Image URL for the Next page button |
| `PreviousPageImageUrl` | `string` | `null` | Image URL for the Previous page button |
| `Position` | `PagerPosition` | `Bottom` | Where to display the pager |
| `Visible` | `bool` | `true` | Whether the pager is displayed |

## PagerButtons Enum

| Value | Description |
|-------|-------------|
| `PagerButtons.NextPrevious` | Displays Next and Previous buttons |
| `PagerButtons.Numeric` | Displays numeric page buttons (default) |
| `PagerButtons.NextPreviousFirstLast` | Displays Next, Previous, First, and Last buttons |
| `PagerButtons.NumericFirstLast` | Displays numeric page buttons with First and Last buttons |

## PagerPosition Enum

| Value | Description |
|-------|-------------|
| `PagerPosition.Bottom` | Pager at the bottom of the control (default) |
| `PagerPosition.Top` | Pager at the top of the control |
| `PagerPosition.TopAndBottom` | Pager at both top and bottom |

## Usage with Parent Controls

### FormView

```razor
<FormView DataSource="@Employees" ItemType="Employee" AllowPaging="true">
    <PagerSettings Mode="PagerButtons.NumericFirstLast"
                   FirstPageText="« First"
                   LastPageText="Last »"
                   PageButtonCount="5" />
    <ItemTemplate Context="Item">
        <p><strong>@Item.Name</strong> — @Item.Title</p>
    </ItemTemplate>
</FormView>
```

### DetailsView

```razor
<DetailsView ItemType="Product" Items="@Products" AllowPaging="true">
    <PagerSettings Mode="PagerButtons.NextPreviousFirstLast"
                   FirstPageText="First"
                   LastPageText="Last"
                   NextPageText="Next >"
                   PreviousPageText="< Prev"
                   Position="PagerPosition.TopAndBottom" />
</DetailsView>
```

## Migration Notes

1. **Remove dash-separated attributes** — Web Forms `PagerSettings-Mode="Numeric"` becomes a child `<PagerSettings Mode="PagerButtons.Numeric" />` element
2. **Use enum values** — Web Forms string values like `"NextPreviousFirstLast"` become `PagerButtons.NextPreviousFirstLast`
3. **Position enum** — Web Forms `Position="TopAndBottom"` becomes `Position="PagerPosition.TopAndBottom"`
4. **Image URLs** — Image URL properties work the same way but should use Blazor static asset paths instead of `~/` server-relative paths

### Before (Web Forms)

```html
<asp:GridView ID="gv1" runat="server" AllowPaging="True">
    <PagerSettings
        Mode="NumericFirstLast"
        FirstPageText="First"
        LastPageText="Last"
        Position="TopAndBottom" />
</asp:GridView>
```

### After (Blazor)

```razor
<GridView ItemType="Product" Items="@Products" AllowPaging="true">
    <PagerSettings
        Mode="PagerButtons.NumericFirstLast"
        FirstPageText="First"
        LastPageText="Last"
        Position="PagerPosition.TopAndBottom" />
</GridView>
```

## See Also

- [FormView](FormView.md) — Single-record view with paging support
- [DetailsView](DetailsView.md) — Single-record detail view with paging support
- [GridView](GridView.md) — Multi-record tabular display with paging support
- [DataPager](DataPager.md) — Standalone paging control for ListView

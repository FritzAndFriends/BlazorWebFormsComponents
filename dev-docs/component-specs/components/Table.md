>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# Table — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.table?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.Table`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Caption | `string` | ✅ Match | `[Parameter]` — renders `<caption>` element |
| CaptionAlign | `TableCaptionAlign` | ✅ Match | `[Parameter]` using `TableCaptionAlign` enum; renders `caption-side` CSS |
| CellPadding | `int` | ✅ Match | `[Parameter]` — renders `cellpadding` attribute |
| CellSpacing | `int` | ✅ Match | `[Parameter]` — renders `cellspacing` attribute |
| GridLines | `GridLines` | ✅ Match | `[Parameter]` — renders `rules` and `border` attributes |
| HorizontalAlign | `HorizontalAlign` | ✅ Match | `[Parameter]` — renders margin CSS for alignment |
| BackImageUrl | `string` | ✅ Match | `[Parameter]` — renders `background-image` CSS |
| Rows | `TableRowCollection` | ⚠️ Needs Work | Not a direct property; rows are added via `ChildContent` RenderFragment |
| ChildContent | — | ✅ Match | Blazor-specific `RenderFragment` for rows |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent`; rendered as `id` |
| ClientID | `string` (read-only) | ✅ Match | Rendered on `<table>` element |
| Visible | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Enabled | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| TabIndex | `short` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| CssClass | `string` | ✅ Match | Inherited from `BaseStyledComponent` |
| BackColor | `Color` | ✅ Match | Inherited from `BaseStyledComponent` |
| ForeColor | `Color` | ✅ Match | Inherited from `BaseStyledComponent` |
| BorderColor | `Color` | ✅ Match | Inherited from `BaseStyledComponent` |
| BorderStyle | `BorderStyle` | ✅ Match | Inherited from `BaseStyledComponent` |
| BorderWidth | `Unit` | ✅ Match | Inherited from `BaseStyledComponent` |
| Font | `FontInfo` | ✅ Match | Inherited from `BaseStyledComponent` |
| Height | `Unit` | ✅ Match | Inherited from `BaseStyledComponent` |
| Width | `Unit` | ✅ Match | Inherited from `BaseStyledComponent` |
| AccessKey | `string` | 🔴 Missing | Not in any base class |
| ToolTip | `string` | 🔴 Missing | Not in any base class |
| Style | `CssStyleCollection` | ⚠️ Needs Work | Computed internally via `CombinedStyle` combining base + alignment + image + gridlines |
| EnableViewState | `bool` | N/A | Obsolete, accepted but ignored |
| EnableTheming | `bool` | N/A | Obsolete, accepted but ignored |
| SkinID | `string` | N/A | Obsolete, accepted but ignored |
| ViewState | `StateBag` | N/A | Server-side only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| Init | `EventHandler` | ✅ Match | `OnInit` on base |
| Load | `EventHandler` | ✅ Match | `OnLoad` on base |
| PreRender | `EventHandler` | ✅ Match | `OnPreRender` on base |
| Unload | `EventHandler` | ✅ Match | `OnUnload` on base |
| Disposed | `EventHandler` | ✅ Match | `OnDisposed` on base |
| DataBinding | `EventHandler` | ✅ Match | `OnDataBinding` on base |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | `void DataBind()` | N/A | No-op in Blazor |
| Focus() | `void Focus()` | 🔴 Missing | Would require JS interop |
| FindControl() | `Control FindControl(string)` | ✅ Match | On `BaseWebFormsComponent` |

## HTML Output Comparison

**Web Forms** renders a `<table>` element:
```html
<table id="Table1" cellpadding="5" cellspacing="0" rules="all" border="1">
  <caption>My Table</caption>
  <tr><td>Cell 1</td><td>Cell 2</td></tr>
</table>
```

**Blazor** renders the same structure with additional CSS:
```html
<table id="Table1" class="" style="border-collapse: collapse" cellpadding="5" cellspacing="0" border="1" rules="all">
  <caption>My Table</caption>
  <!-- ChildContent rows here -->
</table>
```

✅ HTML output structure matches. Blazor adds `border-collapse: collapse` CSS when GridLines are set, which is a progressive enhancement. The deprecated `rules`/`border` attributes are preserved for compatibility.

## Related Components

The Table component works with:
- `TableRow` — renders `<tr>`
- `TableCell` — renders `<td>`
- `TableHeaderRow` — renders `<tr>` for headers
- `TableHeaderCell` — renders `<th>`
- `TableFooterRow` — renders `<tr>` for footers

## Summary

- **Matching:** 21 properties, 6 events
- **Needs Work:** 2 properties (Rows collection, Style)
- **Missing:** 2 properties (AccessKey, ToolTip), 0 events
- **N/A (server-only):** 4 items

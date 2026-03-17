>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# DataGrid — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.datagrid?view=netframework-4.8.1
**Blazor Component:** `BlazorWebFormsComponents.DataGrid<ItemType>`
**Implementation Status:** ⚠️ Partial

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | Inherited from BaseWebFormsComponent |
| Visible | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| Enabled | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| TabIndex | short | ✅ Match | Inherited from BaseWebFormsComponent |
| CssClass | string | ✅ Match | Direct parameter on DataGrid |
| AutoGenerateColumns | bool | ✅ Match | Defaults to true |
| DataKeyField | string | ⚠️ Needs Work | Parameter exists but "Not supported yet" per comment |
| ShowHeader | bool | ✅ Match | Defaults to true |
| ShowFooter | bool | ✅ Match | Defaults to false |
| AllowPaging | bool | ✅ Match | Parameter exists |
| AllowSorting | bool | ✅ Match | Parameter exists |
| PageSize | int | ✅ Match | Defaults to 10 |
| CurrentPageIndex | int | ✅ Match | Defaults to 0 |
| EmptyDataText | string | ✅ Match | Shown when no items |
| Columns | RenderFragment | ✅ Match | Template-based column definitions |
| Items | IEnumerable<T> | ✅ Match | Inherited from DataBoundComponent<T> |
| DataSource | object | ✅ Match | Inherited from DataBoundComponent<T> |
| DataMember | string | ✅ Match | Inherited from DataBoundComponent<T> |
| ChildContent | RenderFragment | ✅ Match | Blazor composition |
| AllowCustomPaging | bool | 🔴 Missing | |
| AlternatingItemStyle | TableItemStyle | 🔴 Missing | No alternating row styles |
| BackColor | Color | 🔴 Missing | Not inherited (uses DataBoundComponent, not BaseStyledComponent) |
| BackImageUrl | string | 🔴 Missing | |
| BorderColor | Color | 🔴 Missing | Not styled base |
| BorderStyle | BorderStyle | 🔴 Missing | |
| BorderWidth | Unit | 🔴 Missing | |
| Caption | string | 🔴 Missing | |
| CaptionAlign | TableCaptionAlign | 🔴 Missing | |
| CellPadding | int | 🔴 Missing | |
| CellSpacing | int | 🔴 Missing | |
| EditItemIndex | int | 🔴 Missing | No inline editing support |
| EditItemStyle | TableItemStyle | 🔴 Missing | |
| Font | FontInfo | 🔴 Missing | |
| FooterStyle | TableItemStyle | 🔴 Missing | |
| ForeColor | Color | 🔴 Missing | |
| GridLines | GridLines | 🔴 Missing | |
| HeaderStyle | TableItemStyle | 🔴 Missing | |
| Height | Unit | 🔴 Missing | |
| HorizontalAlign | HorizontalAlign | 🔴 Missing | |
| ItemStyle | TableItemStyle | 🔴 Missing | |
| PageCount | int | 🔴 Missing | Read-only computed |
| PagerStyle | TableItemStyle | 🔴 Missing | |
| SelectedIndex | int | 🔴 Missing | |
| SelectedItem | DataGridItem | 🔴 Missing | |
| SelectedItemStyle | TableItemStyle | 🔴 Missing | |
| ToolTip | string | 🔴 Missing | |
| UseAccessibleHeader | bool | 🔴 Missing | |
| VirtualItemCount | int | 🔴 Missing | |
| Width | Unit | 🔴 Missing | |
| DataSourceID | string | N/A | Server-only |
| EnableViewState | bool | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |
| Style | CssStyleCollection | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| ItemCommand | DataGridCommandEventHandler | ✅ Match | `OnItemCommand` EventCallback |
| EditCommand | DataGridCommandEventHandler | ✅ Match | `OnEditCommand` EventCallback |
| DeleteCommand | DataGridCommandEventHandler | ✅ Match | `OnDeleteCommand` EventCallback |
| UpdateCommand | DataGridCommandEventHandler | ✅ Match | `OnUpdateCommand` EventCallback |
| CancelCommand | DataGridCommandEventHandler | ✅ Match | `OnCancelCommand` EventCallback |
| ItemCreated | DataGridItemEventHandler | 🔴 Missing | |
| ItemDataBound | DataGridItemEventHandler | 🔴 Missing | |
| PageIndexChanged | DataGridPageChangedEventHandler | 🔴 Missing | |
| SelectedIndexChanged | EventHandler | 🔴 Missing | |
| SortCommand | DataGridSortCommandEventHandler | 🔴 Missing | |
| DataBinding | EventHandler | ✅ Match | Inherited |
| DataBound | EventHandler | ✅ Match | Inherited from BaseDataBoundComponent |
| Init | EventHandler | ✅ Match | Inherited (OnInit) |
| Load | EventHandler | ✅ Match | Inherited (OnLoad) |
| PreRender | EventHandler | ✅ Match | Inherited (OnPreRender) |
| Unload | EventHandler | ✅ Match | Inherited (OnUnload) |
| Disposed | EventHandler | ✅ Match | Inherited (OnDisposed) |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | void | N/A | Server-only pattern |
| Focus() | void | N/A | Server-only |
| AddColumn() | — | ✅ Match | Blazor-specific via IColumnCollection |
| RemoveColumn() | — | ✅ Match | Blazor-specific via IColumnCollection |
| AddRow() | — | ✅ Match | Blazor-specific via IRowCollection |
| RemoveRow() | — | ✅ Match | Blazor-specific via IRowCollection |

## HTML Output Comparison

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| Root element | `<table>` | `<table>` ✅ |
| Header | `<thead><tr><th>` | `<thead><tr><th>` ✅ |
| Body | `<tbody><tr><td>` | `<tbody><tr><td>` via DataGridRow ✅ |
| Empty state | — | `<tr><td colspan>` with EmptyDataText |
| CSS class | `class` attribute | `class="@CssClass"` ✅ |
| CellPadding/CellSpacing | On `<table>` | 🔴 Missing |
| GridLines | `rules` attribute | 🔴 Missing |

## Summary

- **Matching:** 15 properties, 10 events
- **Needs Work:** 1 property (DataKeyField stub)
- **Missing:** ~25 properties (styles, paging UI, selection, formatting), 5 events (ItemCreated, ItemDataBound, PageIndexChanged, SelectedIndexChanged, SortCommand)
- **N/A (server-only):** ~6 items

Key gaps: No style properties (BackColor, ForeColor, Font, etc.) because DataGrid inherits DataBoundComponent which does not extend BaseStyledComponent. No alternating/selected/edit item styles. No pager UI. Core command events are well-covered.

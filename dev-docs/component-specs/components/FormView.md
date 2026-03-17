>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# FormView — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.formview?view=netframework-4.8.1
**Blazor Component:** `BlazorWebFormsComponents.FormView<ItemType>`
**Implementation Status:** ⚠️ Partial

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | Inherited from BaseWebFormsComponent |
| Visible | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| Enabled | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| TabIndex | short | ✅ Match | Inherited from BaseWebFormsComponent |
| CurrentMode | FormViewMode | ✅ Match | ReadOnly, Edit, Insert |
| DefaultMode | FormViewMode | ✅ Match | Defaults to ReadOnly |
| EditItemTemplate | RenderFragment<T> | ✅ Match | Template for edit mode |
| InsertItemTemplate | RenderFragment<T> | ✅ Match | Template for insert mode |
| ItemTemplate | RenderFragment<T> | ✅ Match | Template for read-only mode |
| Items | IEnumerable<T> | ✅ Match | Inherited from DataBoundComponent<T> |
| DataSource | object | ✅ Match | Inherited |
| DataMember | string | ✅ Match | Inherited |
| AllowPaging | bool | ⚠️ Needs Work | Pager is always rendered when Items > 1; no explicit AllowPaging parameter |
| BackColor | Color | 🔴 Missing | Not BaseStyledComponent |
| BackImageUrl | string | 🔴 Missing | |
| BorderColor | Color | 🔴 Missing | |
| BorderStyle | BorderStyle | 🔴 Missing | |
| BorderWidth | Unit | 🔴 Missing | |
| Caption | string | 🔴 Missing | |
| CaptionAlign | TableCaptionAlign | 🔴 Missing | |
| CellPadding | int | 🔴 Missing | |
| CellSpacing | int | 🔴 Missing | Hardcoded to 0 in razor |
| CssClass | string | 🔴 Missing | Not on the component |
| DataKeyNames | string | 🔴 Missing | |
| EditRowStyle | TableItemStyle | 🔴 Missing | |
| EmptyDataRowStyle | TableItemStyle | 🔴 Missing | |
| EmptyDataTemplate | RenderFragment | 🔴 Missing | |
| EmptyDataText | string | 🔴 Missing | Shows "Loading..." instead |
| Font | FontInfo | 🔴 Missing | |
| FooterRow | FormViewRow | 🔴 Missing | |
| FooterStyle | TableItemStyle | 🔴 Missing | |
| FooterTemplate | RenderFragment | 🔴 Missing | |
| FooterText | string | 🔴 Missing | |
| ForeColor | Color | 🔴 Missing | |
| GridLines | GridLines | 🔴 Missing | |
| HeaderRow | FormViewRow | 🔴 Missing | |
| HeaderStyle | TableItemStyle | 🔴 Missing | |
| HeaderTemplate | RenderFragment | 🔴 Missing | |
| HeaderText | string | 🔴 Missing | |
| Height | Unit | 🔴 Missing | |
| HorizontalAlign | HorizontalAlign | 🔴 Missing | |
| InsertRowStyle | TableItemStyle | 🔴 Missing | |
| PageCount | int | 🔴 Missing | Could be computed from Items.Count() |
| PageIndex | int | 🔴 Missing | Uses internal Position (1-based) instead |
| PagerSettings | PagerSettings | 🔴 Missing | |
| PagerStyle | TableItemStyle | 🔴 Missing | |
| PagerTemplate | RenderFragment | 🔴 Missing | |
| RenderOuterTable | bool | 🔴 Missing | Always renders outer table |
| RowStyle | TableItemStyle | 🔴 Missing | |
| SelectedValue | object | 🔴 Missing | |
| ToolTip | string | 🔴 Missing | |
| Width | Unit | 🔴 Missing | |
| DeleteMethod | string | 🔴 Missing | Model binding |
| InsertMethod | string | 🔴 Missing | Model binding |
| UpdateMethod | string | 🔴 Missing | Model binding |
| DataSourceID | string | N/A | Server-only |
| EnableViewState | bool | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |
| EnableModelValidation | bool | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| ModeChanging | FormViewModeEventHandler | ✅ Match | EventCallback |
| ItemDeleting | FormViewDeleteEventHandler | ✅ Match | `OnItemDeleting` |
| ItemDeleted | FormViewDeletedEventHandler | ✅ Match | `OnItemDeleted` |
| ItemInserting | FormViewInsertEventHandler | ✅ Match | `OnItemInserting` |
| ItemInserted | FormViewInsertedEventHandler | ✅ Match | `OnItemInserted` — same args as Inserting |
| ItemUpdating | FormViewUpdateEventHandler | ✅ Match | `OnItemUpdating` |
| ItemUpdated | FormViewUpdatedEventHandler | ✅ Match | `OnItemUpdated` |
| DataBinding | EventHandler | ✅ Match | Inherited |
| DataBound | EventHandler | ✅ Match | Inherited |
| Init | EventHandler | ✅ Match | Inherited (OnInit) |
| Load | EventHandler | ✅ Match | Inherited (OnLoad) |
| PreRender | EventHandler | ✅ Match | Inherited (OnPreRender) |
| Unload | EventHandler | ✅ Match | Inherited (OnUnload) |
| Disposed | EventHandler | ✅ Match | Inherited (OnDisposed) |
| ModeChanged | FormViewModeEventHandler | 🔴 Missing | Only ModeChanging exists, no ModeChanged |
| ItemCommand | FormViewCommandEventHandler | 🔴 Missing | Commands handled internally, not exposed |
| ItemCreated | FormViewItemEventHandler | 🔴 Missing | |
| PageIndexChanging | FormViewPageEventHandler | 🔴 Missing | Paging is Position-based internally |
| PageIndexChanged | FormViewPageEventHandler | 🔴 Missing | |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| ChangeMode() | void | ⚠️ Needs Work | Handled internally via HandleCommandArgs, not exposed as public method |
| DeleteItem() | void | ⚠️ Needs Work | Triggered via command bubble, not direct method |
| InsertItem() | void | ⚠️ Needs Work | Triggered via command bubble |
| UpdateItem() | void | ⚠️ Needs Work | Triggered via command bubble |
| DataBind() | void | N/A | Server-only |
| Focus() | void | N/A | Server-only |

## HTML Output Comparison

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| Root element | `<table>` | `<table cellspacing="0" style="border-collapse:collapse;">` ✅ |
| Content area | `<tr><td colspan="2">` | `<tr><td colspan="2">` ✅ |
| Template rendering | Based on CurrentMode | Switch on CurrentMode ✅ |
| Pager | Configurable pager row | Numeric pager in nested `<table>` ✅ |
| Loading state | — | Shows "Loading..." (Blazor-specific) |

## Summary

- **Matching:** 10 properties, 12 events
- **Needs Work:** 1 property (AllowPaging implicit), 4 methods (internal only)
- **Missing:** ~35 properties (all style objects, header/footer, CssClass, pager config, empty data), 5 events (ModeChanged, ItemCommand, ItemCreated, page events)
- **N/A (server-only):** ~6 items

FormView has good template and mode-switching support (ReadOnly/Edit/Insert) and covers the core CRUD events. However, it's missing many display properties — CssClass, all style objects, header/footer templates, empty data handling, and pager configuration. The internal Position is 1-based instead of 0-based PageIndex. Command handling uses `.GetAwaiter().GetResult()` which risks deadlocks in Blazor's sync context (noted in prior reviews).

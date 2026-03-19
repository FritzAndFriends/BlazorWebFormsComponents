>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# ListView — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.listview?view=netframework-4.8.1
**Blazor Component:** `BlazorWebFormsComponents.ListView<ItemType>`
**Implementation Status:** ⚠️ Partial

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | Inherited from BaseWebFormsComponent |
| Visible | bool | ✅ Match | Inherited; controls rendering |
| Enabled | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| TabIndex | short | ✅ Match | Inherited from BaseWebFormsComponent |
| ItemTemplate | RenderFragment<T> | ✅ Match | Core template |
| AlternatingItemTemplate | RenderFragment<T> | ✅ Match | Alternating items |
| EmptyDataTemplate | RenderFragment | ✅ Match | Shown when no items |
| GroupSeparatorTemplate | RenderFragment | ✅ Match | Between groups |
| GroupTemplate | RenderFragment<RenderFragment> | ✅ Match | Wraps groups |
| ItemSeparatorTemplate | RenderFragment | ✅ Match | Between items |
| LayoutTemplate | RenderFragment<RenderFragment> | ✅ Match | Overall layout wrapper |
| ItemPlaceHolder | RenderFragment | ✅ Match | Blazor-specific placeholder |
| GroupItemCount | int | ✅ Match | Items per group |
| InsertItemPosition | InsertItemPosition | ⚠️ Needs Work | Parameter exists with TODO comment |
| SelectedIndex | int | ⚠️ Needs Work | Parameter exists with TODO comment |
| Items | IEnumerable<T> | ✅ Match | Inherited from DataBoundComponent<T> |
| DataSource | object | ✅ Match | Inherited |
| DataMember | string | ✅ Match | Inherited |
| Style | string | ⚠️ Needs Work | Parameter exists but marked [Obsolete] — "not applied by this control" |
| ChildContent | RenderFragment | ✅ Match | Blazor composition |
| BackColor | Color | 🔴 Missing | Not BaseStyledComponent |
| BorderColor | Color | 🔴 Missing | |
| BorderStyle | BorderStyle | 🔴 Missing | |
| BorderWidth | Unit | 🔴 Missing | |
| CssClass | string | 🔴 Missing | |
| ConvertEmptyStringToNull | bool | 🔴 Missing | |
| DataKeyNames | string[] | 🔴 Missing | |
| DataKeys | DataKeyArray | 🔴 Missing | |
| EditIndex | int | 🔴 Missing | No inline editing |
| EditItem | ListViewItem | 🔴 Missing | |
| EditItemTemplate | ITemplate | 🔴 Missing | |
| EmptyItemTemplate | ITemplate | 🔴 Missing | |
| Font | FontInfo | 🔴 Missing | |
| ForeColor | Color | 🔴 Missing | |
| Height | Unit | 🔴 Missing | |
| InsertItem | ListViewItem | 🔴 Missing | |
| InsertItemTemplate | ITemplate | 🔴 Missing | |
| ItemPlaceholderID | string | N/A | Marked obsolete in base |
| MaximumRows | int | 🔴 Missing | Used with DataPager |
| SelectedDataKey | DataKey | 🔴 Missing | |
| SelectedItemTemplate | ITemplate | 🔴 Missing | |
| SelectedPersistedDataKey | DataKey | 🔴 Missing | |
| SelectedValue | object | 🔴 Missing | |
| SortDirection | SortDirection | 🔴 Missing | |
| SortExpression | string | 🔴 Missing | |
| StartRowIndex | int | 🔴 Missing | |
| ToolTip | string | 🔴 Missing | |
| Width | Unit | 🔴 Missing | |
| DeleteMethod | string | 🔴 Missing | Model binding |
| InsertMethod | string | 🔴 Missing | Model binding |
| UpdateMethod | string | 🔴 Missing | Model binding |
| GroupPlaceholderID | string | 🔴 Missing | |
| DataSourceID | string | N/A | Server-only |
| EnableViewState | bool | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |
| EnablePersistedSelection | bool | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| ItemDataBound | ListViewItemEventHandler | ✅ Match | `OnItemDataBound` EventCallback |
| LayoutCreated | EventHandler | ✅ Match | `OnLayoutCreated` EventHandler |
| DataBinding | EventHandler | ✅ Match | Inherited |
| DataBound | EventHandler | ✅ Match | Inherited |
| Init | EventHandler | ✅ Match | Inherited (OnInit) |
| Load | EventHandler | ✅ Match | Inherited (OnLoad) |
| PreRender | EventHandler | ✅ Match | Inherited (OnPreRender) |
| Unload | EventHandler | ✅ Match | Inherited (OnUnload) |
| Disposed | EventHandler | ✅ Match | Inherited (OnDisposed) |
| ItemCanceling | ListViewCancelEventHandler | 🔴 Missing | |
| ItemCommand | ListViewCommandEventHandler | 🔴 Missing | |
| ItemCreated | ListViewItemEventHandler | 🔴 Missing | |
| ItemDeleted | ListViewDeletedEventHandler | 🔴 Missing | |
| ItemDeleting | ListViewDeleteEventHandler | 🔴 Missing | |
| ItemEditing | ListViewEditEventHandler | 🔴 Missing | |
| ItemInserted | ListViewInsertedEventHandler | 🔴 Missing | |
| ItemInserting | ListViewInsertEventHandler | 🔴 Missing | |
| ItemUpdated | ListViewUpdatedEventHandler | 🔴 Missing | |
| ItemUpdating | ListViewUpdateEventHandler | 🔴 Missing | |
| PagePropertiesChanged | EventHandler | 🔴 Missing | |
| PagePropertiesChanging | EventHandler | 🔴 Missing | |
| SelectedIndexChanged | EventHandler | 🔴 Missing | |
| SelectedIndexChanging | ListViewSelectEventHandler | 🔴 Missing | |
| Sorted | EventHandler | 🔴 Missing | |
| Sorting | ListViewSortEventHandler | 🔴 Missing | |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | void | N/A | Server-only |
| Focus() | void | N/A | Server-only |
| DeleteItem() | void | 🔴 Missing | |
| InsertNewItem() | void | 🔴 Missing | |
| UpdateItem() | void | 🔴 Missing | |
| Sort() | void | 🔴 Missing | |

## HTML Output Comparison

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| Root element | User-defined (LayoutTemplate) | User-defined (LayoutTemplate) ✅ |
| Items | ItemTemplate per item | ItemTemplate per item ✅ |
| Alternating | AlternatingItemTemplate | AlternatingItemTemplate ✅ |
| Groups | GroupTemplate wrapper | GroupTemplate wrapper ✅ |
| Separators | Between items/groups | ItemSeparatorTemplate / GroupSeparatorTemplate ✅ |
| Empty state | EmptyDataTemplate | EmptyDataTemplate ✅ |

ListView's template-driven HTML output is fully user-controlled, so fidelity depends on the templates provided.

## Summary

- **Matching:** 14 properties, 9 events
- **Needs Work:** 3 properties (InsertItemPosition, SelectedIndex with TODOs, Style obsoleted)
- **Missing:** ~25 properties (editing, selection, sorting, paging integration, style), 16 events (all CRUD, selection, sorting, paging events)
- **N/A (server-only):** ~6 items

ListView has excellent template support (ItemTemplate, AlternatingItemTemplate, GroupTemplate, LayoutTemplate, separators, empty data). The core rendering pipeline with grouping and alternating items is well-implemented. Main gaps are CRUD operations (edit/insert/delete/update methods and events are all missing), selection, sorting, and DataPager integration (no MaximumRows/StartRowIndex support).

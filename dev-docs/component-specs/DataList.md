>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# DataList — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.datalist?view=netframework-4.8.1
**Blazor Component:** `BlazorWebFormsComponents.DataList<ItemType>`
**Implementation Status:** ⚠️ Partial (well-covered)

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | Inherited from BaseWebFormsComponent |
| Visible | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| Enabled | bool | ✅ Match | Inherited from BaseWebFormsComponent |
| TabIndex | short | ✅ Match | Inherited from BaseWebFormsComponent; used in markup |
| AccessKey | string | ✅ Match | Direct parameter, rendered on `<table>` |
| BackColor | WebColor | ✅ Match | Direct parameter, via IStyle |
| BorderColor | WebColor | ✅ Match | Direct parameter |
| BorderStyle | BorderStyle | ✅ Match | Direct parameter |
| BorderWidth | Unit | ✅ Match | Direct parameter |
| Caption | string | ✅ Match | Rendered in `<caption>` |
| CaptionAlign | VerticalAlign | ✅ Match | Applied on `<caption>` |
| CellPadding | int | ✅ Match | Rendered on `<table>` |
| CellSpacing | int | ✅ Match | Rendered on `<table>` |
| CssClass | string | ✅ Match | Direct parameter |
| Font | FontInfo | ✅ Match | Direct parameter |
| ForeColor | WebColor | ✅ Match | Direct parameter |
| GridLines | DataListEnum | ✅ Match | Rendered as `rules` attribute |
| HeaderStyle | TableItemStyle | ✅ Match | Cascading style object |
| FooterStyle | TableItemStyle | ✅ Match | Cascading style object |
| HeaderTemplate | RenderFragment | ✅ Match | |
| FooterTemplate | RenderFragment | ✅ Match | |
| Height | Unit | ✅ Match | Direct parameter |
| ItemStyle | TableItemStyle | ✅ Match | Cascading style object |
| ItemTemplate | RenderFragment<T> | ✅ Match | |
| AlternatingItemStyle | TableItemStyle | ✅ Match | Cascading style object |
| AlternatingItemTemplate | RenderFragment<T> | ✅ Match | |
| Items | IEnumerable<T> | ✅ Match | Inherited from DataBoundComponent<T> |
| DataSource | object | ✅ Match | Inherited from DataBoundComponent<T> |
| DataMember | string | ✅ Match | Inherited from DataBoundComponent<T> |
| RepeatColumns | int | ✅ Match | Defaults to 1 |
| RepeatDirection | RepeatDirection | ✅ Match | Enum (Horizontal/Vertical) |
| RepeatLayout | RepeatLayout | ✅ Match | Table and Flow layouts implemented |
| SeparatorStyle | TableItemStyle | ✅ Match | Cascading style object |
| SeparatorTemplate | RenderFragment | ✅ Match | |
| ShowFooter | bool | ✅ Match | Defaults to true |
| ShowHeader | bool | ✅ Match | Defaults to true |
| Style | string | ✅ Match | Direct CSS string parameter |
| ToolTip | string | ✅ Match | Rendered as `title` attribute |
| UseAccessibleHeader | bool | ✅ Match | Renders `<th>` vs `<td>` |
| Width | Unit | ✅ Match | Direct parameter |
| DataKeyField | string | 🔴 Missing | |
| EditItemIndex | int | 🔴 Missing | No inline editing |
| EditItemStyle | TableItemStyle | 🔴 Missing | |
| EditItemTemplate | ITemplate | 🔴 Missing | |
| ExtractTemplateRows | bool | 🔴 Missing | |
| HorizontalAlign | HorizontalAlign | 🔴 Missing | |
| SelectedIndex | int | 🔴 Missing | |
| SelectedItem | DataListItem | 🔴 Missing | |
| SelectedItemStyle | TableItemStyle | 🔴 Missing | |
| SelectedItemTemplate | ITemplate | 🔴 Missing | |
| SelectedValue | object | 🔴 Missing | |
| BackImageUrl | string | 🔴 Missing | |
| DataSourceID | string | N/A | Server-only |
| EnableViewState | bool | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| ItemDataBound | DataListItemEventHandler | ✅ Match | `OnItemDataBound` EventCallback |
| DataBinding | EventHandler | ✅ Match | Inherited |
| DataBound | EventHandler | ✅ Match | Inherited from BaseDataBoundComponent |
| Init | EventHandler | ✅ Match | Inherited (OnInit) |
| Load | EventHandler | ✅ Match | Inherited (OnLoad) |
| PreRender | EventHandler | ✅ Match | Inherited (OnPreRender) |
| Unload | EventHandler | ✅ Match | Inherited (OnUnload) |
| Disposed | EventHandler | ✅ Match | Inherited (OnDisposed) |
| ItemCreated | DataListItemEventHandler | 🔴 Missing | |
| ItemCommand | DataListCommandEventHandler | 🔴 Missing | |
| DeleteCommand | DataListCommandEventHandler | 🔴 Missing | |
| EditCommand | DataListCommandEventHandler | 🔴 Missing | |
| UpdateCommand | DataListCommandEventHandler | 🔴 Missing | |
| SelectedIndexChanged | EventHandler | 🔴 Missing | |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | void | N/A | Server-only |
| Focus() | void | N/A | Server-only |

## HTML Output Comparison

| Aspect | Web Forms | Blazor |
|--------|-----------|--------|
| Table layout | `<table>` with cellpadding/cellspacing/rules | `<table>` with cellpadding/cellspacing/rules ✅ |
| Flow layout | `<span>` wrappers | `<span>` wrappers ✅ |
| Header | `<tr><td>` or `<tr><th>` | `<tr><td>` or `<tr><th>` (UseAccessibleHeader) ✅ |
| Items | `<tr><td>` per item | `<tr><td>` per item ✅ |
| Separators | `<tr><td>` | `<tr><td>` ✅ |
| Caption | `<caption>` | `<caption>` ✅ |
| Style | `style="border-collapse:collapse;..."` | `style="border-collapse:collapse;..."` ✅ |

HTML output fidelity is excellent for DataList.

## Summary

- **Matching:** 38 properties, 8 events
- **Needs Work:** 0
- **Missing:** 11 properties (editing, selection, DataKeyField, HorizontalAlign), 6 events (command events, selection)
- **N/A (server-only):** ~5 items

DataList is one of the best-implemented data controls. Style properties, templates, repeat layouts, and accessible headers are all present. The main gaps are inline editing (EditItemIndex/EditItemStyle/EditItemTemplate) and selection (SelectedIndex/SelectedItem/SelectedItemStyle). Command events (Delete, Edit, Update) are also missing.

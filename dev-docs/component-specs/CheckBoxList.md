>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# CheckBoxList — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.checkboxlist?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.CheckBoxList<TItem>`
**Implementation Status:** ✅ Implemented

## Properties

### Control-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| RepeatColumns | int | ✅ Match | Number of columns for table layout |
| RepeatDirection | RepeatDirection | ✅ Match | Vertical or Horizontal (uses `DataListEnum`) |
| RepeatLayout | RepeatLayout | ✅ Match | Table, Flow, OrderedList, UnorderedList |
| TextAlign | TextAlign | ✅ Match | Label position relative to checkbox |
| CellPadding | int | ✅ Match | Table layout cell padding |
| CellSpacing | int | ✅ Match | Table layout cell spacing |

### ListControl Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Items | ListItemCollection | ✅ Match | Via `StaticItems` parameter |
| DataTextField | string | ✅ Match | Maps data to text |
| DataValueField | string | ✅ Match | Maps data to value |
| DataTextFormatString | string | 🔴 Missing | No format string support |
| DataSource | object | ✅ Match | Via DataBoundComponent |
| DataMember | string | ✅ Match | Via DataBoundComponent |
| DataSourceID | string | 🔴 Missing | No server-side DataSource controls |
| AppendDataBoundItems | bool | 🔴 Missing | Always appends static + data items |
| SelectedIndex | int | ✅ Match | Read-only computed property |
| SelectedItem | ListItem | ✅ Match | Read-only computed property |
| SelectedValue | string | ✅ Match | Read-only computed property |
| SelectedValues | List\<string\> | ✅ Match | Blazor-specific multi-select binding |
| AutoPostBack | bool | ✅ Match | Marked obsolete |
| CausesValidation | bool | 🔴 Missing | Not implemented |
| ValidationGroup | string | 🔴 Missing | Not implemented |

### WebControl Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | string | 🔴 Missing | Not in IStyle or base |
| BackColor | Color | ✅ Match | Via IStyle implementation |
| BorderColor | Color | ✅ Match | Via IStyle implementation |
| BorderStyle | BorderStyle | ✅ Match | Via IStyle implementation |
| BorderWidth | Unit | ✅ Match | Via IStyle implementation |
| CssClass | string | ✅ Match | Via IStyle implementation |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent; propagates to individual checkboxes |
| Font | FontInfo | ✅ Match | Via IStyle implementation |
| ForeColor | Color | ✅ Match | Via IStyle implementation |
| Height | Unit | ✅ Match | Via IStyle implementation |
| Width | Unit | ✅ Match | Via IStyle implementation |
| TabIndex | short | ✅ Match | From BaseWebFormsComponent |
| ToolTip | string | 🔴 Missing | Not implemented on this component |
| Style | CssStyleCollection | ✅ Match | Computed from IStyle |

### Control Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | From BaseWebFormsComponent |
| ClientID | string | ✅ Match | From BaseWebFormsComponent |
| Visible | bool | ✅ Match | From BaseWebFormsComponent |
| EnableViewState | bool | N/A | Server-only |
| ViewState | StateBag | N/A | Server-only |
| EnableTheming | bool | N/A | Server-only |
| SkinID | string | N/A | Server-only |
| Page | Page | N/A | Server-only |
| NamingContainer | Control | N/A | Server-only |
| UniqueID | string | N/A | Server-only |
| ClientIDMode | ClientIDMode | N/A | Server-only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| SelectedIndexChanged | EventHandler | ✅ Match | `EventCallback<ChangeEventArgs> OnSelectedIndexChanged` |
| SelectedValuesChanged | — | ✅ Match | `EventCallback<List<string>> SelectedValuesChanged` (Blazor two-way binding) |
| Init | EventHandler | ✅ Match | Via base class |
| Load | EventHandler | ✅ Match | Via base class |
| PreRender | EventHandler | ✅ Match | Via base class |
| Unload | EventHandler | ✅ Match | Via base class |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | void | N/A | Server-only |
| Focus() | void | N/A | Server-only |

## HTML Output Comparison

Web Forms renders a `<table>` by default with each checkbox in a table cell. The Blazor component supports all four RepeatLayout modes:
- **Table**: `<table>` with `<tr>/<td>` wrapping each checkbox
- **Flow**: `<span>` wrapper with `<br>` separators for vertical
- **OrderedList**: `<ol>` with `<li>` per item
- **UnorderedList**: `<ul>` with `<li>` per item

Each checkbox item renders as `<input type="checkbox"><label>` — matching Web Forms output.

Horizontal vs vertical direction and RepeatColumns are properly supported in table layout.

## Summary

- **Matching:** 25 properties, 6 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 7 properties (AccessKey, ToolTip, DataTextFormatString, DataSourceID, AppendDataBoundItems, CausesValidation, ValidationGroup), 0 events
- **N/A (server-only):** 7 items

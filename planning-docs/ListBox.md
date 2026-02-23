# ListBox — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.listbox?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.ListBox<TItem>`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Rows | `int` | ✅ Match | Default 4 (matches Web Forms) |
| SelectionMode | `ListSelectionMode` | ✅ Match | Uses `ListSelectionMode` enum |
| SelectedValue | `string` | ✅ Match | `[Parameter]` with two-way binding |
| SelectedIndex | `int` | ✅ Match | `[Parameter]` with two-way binding |
| SelectedItem | `ListItem` (read-only) | ✅ Match | Computed property |
| Items | `ListItemCollection` | ✅ Match | Via `StaticItems` parameter (renamed) |
| DataTextField | `string` | ✅ Match | `[Parameter]` for data binding |
| DataValueField | `string` | ✅ Match | `[Parameter]` for data binding |
| DataSource | `object` | ✅ Match | Via `Items` parameter from `DataBoundComponent<TItem>` |
| AutoPostBack | `bool` | N/A | Accepted with `[Obsolete]` warning |
| AppendDataBoundItems | `bool` | 🔴 Missing | Not implemented |
| DataTextFormatString | `string` | 🔴 Missing | Not implemented |
| CausesValidation | `bool` | 🔴 Missing | Not on ListBox |
| ValidationGroup | `string` | 🔴 Missing | Not on ListBox |
| SelectedValues | `List<string>` | ✅ Match | Blazor-specific for multi-select; no Web Forms equivalent |
| SelectedItems | `IEnumerable<ListItem>` (read-only) | ✅ Match | Blazor-specific for multi-select |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| ClientID | `string` (read-only) | ✅ Match | Computed |
| Visible | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Enabled | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| TabIndex | `short` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| CssClass | `string` | ✅ Match | Via `IStyle` implementation |
| BackColor | `Color` | ✅ Match | Via `IStyle` implementation |
| ForeColor | `Color` | ✅ Match | Via `IStyle` implementation |
| BorderColor | `Color` | ✅ Match | Via `IStyle` implementation |
| BorderStyle | `BorderStyle` | ✅ Match | Via `IStyle` implementation |
| BorderWidth | `Unit` | ✅ Match | Via `IStyle` implementation |
| Font | `FontInfo` | ✅ Match | Via `IStyle` implementation |
| Height | `Unit` | ✅ Match | Via `IStyle` implementation |
| Width | `Unit` | ✅ Match | Via `IStyle` implementation |
| AccessKey | `string` | 🔴 Missing | Not in any base class |
| ToolTip | `string` | 🔴 Missing | Not in any base class |
| Style | `CssStyleCollection` | ⚠️ Needs Work | Computed internally via `IStyle.ToStyle()` |
| EnableViewState | `bool` | N/A | Obsolete, accepted but ignored |
| EnableTheming | `bool` | N/A | Obsolete, accepted but ignored |
| SkinID | `string` | N/A | Obsolete, accepted but ignored |
| ViewState | `StateBag` | N/A | Server-side only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| SelectedIndexChanged | `EventHandler` | ✅ Match | `OnSelectedIndexChanged` EventCallback<ChangeEventArgs> |
| TextChanged | `EventHandler` | 🔴 Missing | Not implemented |
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
| ClearSelection() | `void ClearSelection()` | 🔴 Missing | Not implemented |
| GetSelectedIndices() | `int[] GetSelectedIndices()` | 🔴 Missing | Not implemented (use SelectedValues instead) |

## HTML Output Comparison

**Web Forms** renders a `<select>` element:
```html
<select id="ListBox1" size="4">
  <option value="1">Item 1</option>
  <option selected="selected" value="2">Item 2</option>
</select>
```

**Blazor** renders the same structure:
```html
<select class="" style="" size="4">
  <option value="1">Item 1</option>
  <option selected value="2">Item 2</option>
</select>
```

⚠️ Blazor does not render `id` on the `<select>` element (no `ClientID` attribute in template). The `multiple` attribute is correctly added for `ListSelectionMode.Multiple`.

## Summary

- **Matching:** 22 properties, 7 events
- **Needs Work:** 1 property (Style)
- **Missing:** 6 properties (AppendDataBoundItems, DataTextFormatString, CausesValidation, ValidationGroup, AccessKey, ToolTip), 1 event (TextChanged)
- **N/A (server-only):** 5 items

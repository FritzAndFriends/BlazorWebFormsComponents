# RadioButtonList — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.radiobuttonlist?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.RadioButtonList<TItem>`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| RepeatColumns | `int` | ✅ Match | `[Parameter]` — default 0 |
| RepeatDirection | `RepeatDirection` | ✅ Match | `[Parameter]` using `DataListEnum` (Vertical/Horizontal) |
| RepeatLayout | `RepeatLayout` | ✅ Match | `[Parameter]` — supports Table, Flow, OrderedList, UnorderedList |
| TextAlign | `TextAlign` | ✅ Match | `[Parameter]` — default `Right` matches Web Forms |
| CellPadding | `int` | ✅ Match | `[Parameter]` — for table layout |
| CellSpacing | `int` | ✅ Match | `[Parameter]` — for table layout |
| SelectedValue | `string` | ✅ Match | `[Parameter]` with two-way binding |
| SelectedIndex | `int` | ✅ Match | `[Parameter]` with two-way binding |
| SelectedItem | `ListItem` (read-only) | ✅ Match | Computed property |
| Items | `ListItemCollection` | ✅ Match | Via `StaticItems` parameter (renamed) |
| DataTextField | `string` | ✅ Match | `[Parameter]` for data binding |
| DataValueField | `string` | ✅ Match | `[Parameter]` for data binding |
| DataSource | `object` | ✅ Match | Via `Items` from `DataBoundComponent<TItem>` |
| AutoPostBack | `bool` | N/A | Accepted with `[Obsolete]` warning |
| AppendDataBoundItems | `bool` | 🔴 Missing | Not implemented |
| DataTextFormatString | `string` | 🔴 Missing | Not implemented |
| CausesValidation | `bool` | 🔴 Missing | Not on RadioButtonList |
| ValidationGroup | `string` | 🔴 Missing | Not on RadioButtonList |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent` |
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
| TextChanged | `EventHandler` | 🔴 Missing | Not implemented (inherited from ListControl in Web Forms) |
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

## HTML Output Comparison

**Web Forms** renders a table (default RepeatLayout) with radio inputs:
```html
<table id="RadioButtonList1">
  <tr><td><input id="RadioButtonList1_0" type="radio" name="RadioButtonList1" value="1" /><label for="RadioButtonList1_0">Option 1</label></td></tr>
  <tr><td><input id="RadioButtonList1_1" type="radio" name="RadioButtonList1" value="2" /><label for="RadioButtonList1_1">Option 2</label></td></tr>
</table>
```

**Blazor** renders a similar table structure:
```html
<table class="" style="" cellpadding="" cellspacing="">
  <tr><td><input id="guid_0" type="radio" name="guid" value="1" /><label for="guid_0">Option 1</label></td></tr>
  <tr><td><input id="guid_1" type="radio" name="guid" value="2" /><label for="guid_1">Option 2</label></td></tr>
</table>
```

⚠️ The group name uses a generated GUID rather than the control's ClientID. The outer table does not render `id`. All four `RepeatLayout` modes (Table, Flow, OrderedList, UnorderedList) are supported.

## Summary

- **Matching:** 23 properties, 7 events
- **Needs Work:** 1 property (Style)
- **Missing:** 6 properties (AppendDataBoundItems, DataTextFormatString, CausesValidation, ValidationGroup, AccessKey, ToolTip), 1 event (TextChanged)
- **N/A (server-only):** 5 items

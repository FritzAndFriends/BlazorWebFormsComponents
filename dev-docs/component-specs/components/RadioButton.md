>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# RadioButton — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.radiobutton?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.RadioButton`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| GroupName | `string` | ✅ Match | `[Parameter]` — falls back to generated ID if empty |
| Checked | `bool` | ✅ Match | `[Parameter]` with two-way binding via `CheckedChanged` |
| Text | `string` | ✅ Match | `[Parameter]` — renders `<label>` when set |
| TextAlign | `TextAlign` | ✅ Match | `[Parameter]` — default `Right` matches Web Forms |
| AutoPostBack | `bool` | N/A | Accepted with `[Obsolete]` warning |
| CausesValidation | `bool` | 🔴 Missing | Not on RadioButton (present on CheckBox in Web Forms) |
| ValidationGroup | `string` | 🔴 Missing | Not on RadioButton |
| InputAttributes | `AttributeCollection` | 🔴 Missing | Not implemented |
| LabelAttributes | `AttributeCollection` | 🔴 Missing | Not implemented |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| ClientID | `string` (read-only) | ✅ Match | Computed; used as input `id` |
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
| Style | `CssStyleCollection` | ⚠️ Needs Work | Computed internally; not directly settable |
| EnableViewState | `bool` | N/A | Obsolete, accepted but ignored |
| EnableTheming | `bool` | N/A | Obsolete, accepted but ignored |
| SkinID | `string` | N/A | Obsolete, accepted but ignored |
| ViewState | `StateBag` | N/A | Server-side only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| CheckedChanged | `EventHandler` | ✅ Match | `OnCheckedChanged` EventCallback<ChangeEventArgs> + `CheckedChanged` EventCallback<bool> |
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

**Web Forms** renders an `<input type="radio">` wrapped in a `<span>` with a `<label>`:
```html
<span><input id="RadioButton1" type="radio" name="GroupName" value="RadioButton1" /><label for="RadioButton1">Option A</label></span>
```

**Blazor** renders a similar structure:
```html
<span class="" style=""><input id="..." type="radio" name="GroupName" checked /><label for="...">Option A</label></span>
```

✅ HTML output structure matches. The `<span>` wrapper with `<input>` + `<label>` pattern is correct. Label placement switches based on `TextAlign`.

## Summary

- **Matching:** 17 properties, 7 events
- **Needs Work:** 1 property (Style)
- **Missing:** 6 properties (CausesValidation, ValidationGroup, InputAttributes, LabelAttributes, AccessKey, ToolTip), 0 events
- **N/A (server-only):** 5 items

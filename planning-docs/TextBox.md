# TextBox — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.textbox?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.TextBox`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Text | `string` | ✅ Match | `[Parameter]` — default empty string |
| TextMode | `TextBoxMode` | ✅ Match | `[Parameter]` using `TextBoxMode` enum; supports SingleLine, MultiLine, Password, Color, Date, DateTime, DateTimeLocal, Email, Month, Number, Range, Search, Phone, Time, Url, Week |
| MaxLength | `int` | ✅ Match | `[Parameter]` — renders `maxlength` attribute |
| Columns | `int` | ✅ Match | `[Parameter]` — renders `size` (single-line) or `cols` (multi-line) |
| Rows | `int` | ✅ Match | `[Parameter]` — renders `rows` attribute for multi-line |
| ReadOnly | `bool` | ✅ Match | `[Parameter]` — renders `readonly` attribute |
| Placeholder | `string` | ✅ Match | `[Parameter]` — Blazor-specific; no direct Web Forms equivalent (was custom in .NET 4.5+) |
| AutoPostBack | `bool` | N/A | Accepted with `[Obsolete]` warning |
| AutoCompleteType | `AutoCompleteType` | N/A | Accepted with `[Obsolete]` warning — browser handles autocomplete |
| CausesValidation | `bool` | 🔴 Missing | Not on TextBox |
| ValidationGroup | `string` | 🔴 Missing | Not on TextBox |
| Wrap | `bool` | 🔴 Missing | Not implemented (controls word wrap in multi-line) |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| ClientID | `string` (read-only) | ✅ Match | Rendered on `<input>`/`<textarea>` |
| Visible | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Enabled | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent`; renders `disabled` |
| TabIndex | `short` | ✅ Match | Inherited from `BaseWebFormsComponent`; renders `tabindex` |
| CssClass | `string` | ✅ Match | Inherited from `BaseStyledComponent`; rendered via `CalculatedAttributes` |
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
| Style | `CssStyleCollection` | ⚠️ Needs Work | Computed internally via `BaseStyledComponent`; rendered through `CalculatedAttributes` |
| EnableViewState | `bool` | N/A | Obsolete, accepted but ignored |
| EnableTheming | `bool` | N/A | Obsolete, accepted but ignored |
| SkinID | `string` | N/A | Obsolete, accepted but ignored |
| ViewState | `StateBag` | N/A | Server-side only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| TextChanged | `EventHandler` | ✅ Match | `OnTextChanged` EventCallback<ChangeEventArgs> + `TextChanged` EventCallback<string> |
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

**Web Forms** renders an `<input>` or `<textarea>`:
```html
<!-- SingleLine -->
<input name="TextBox1" type="text" id="TextBox1" />

<!-- MultiLine -->
<textarea name="TextBox1" rows="5" cols="20" id="TextBox1">Content</textarea>

<!-- Password -->
<input name="TextBox1" type="password" id="TextBox1" />
```

**Blazor** renders the same structure:
```html
<!-- SingleLine -->
<input type="text" value="Content" id="TextBox1" />

<!-- MultiLine -->
<textarea id="TextBox1" rows="5" cols="20">Content</textarea>

<!-- Password -->
<input type="password" value="" id="TextBox1" />
```

✅ HTML output matches for all TextMode values. The Blazor version supports modern HTML5 input types (color, date, email, etc.) that Web Forms added in .NET 4.5.

## Summary

- **Matching:** 20 properties, 7 events
- **Needs Work:** 1 property (Style)
- **Missing:** 5 properties (CausesValidation, ValidationGroup, Wrap, AccessKey, ToolTip), 0 events
- **N/A (server-only):** 6 items

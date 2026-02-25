# CheckBox — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.checkbox?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.CheckBox`
**Implementation Status:** ✅ Implemented

## Properties

### Control-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Checked | bool | ✅ Match | Two-way bindable via CheckedChanged |
| Text | string | ✅ Match | Label text for the checkbox |
| TextAlign | TextAlign | ✅ Match | Left or Right label placement |
| AutoPostBack | bool | ✅ Match | Marked obsolete — use OnCheckedChanged instead |
| CausesValidation | bool | 🔴 Missing | Not implemented |
| ValidationGroup | string | 🔴 Missing | Not implemented |
| InputAttributes | AttributeCollection | 🔴 Missing | No direct equivalent |
| LabelAttributes | AttributeCollection | 🔴 Missing | No direct equivalent |

### WebControl Inherited Properties (from BaseStyledComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | string | 🔴 Missing | Not in BaseStyledComponent |
| BackColor | Color | ✅ Match | From BaseStyledComponent |
| BorderColor | Color | ✅ Match | From BaseStyledComponent |
| BorderStyle | BorderStyle | ✅ Match | From BaseStyledComponent |
| BorderWidth | Unit | ✅ Match | From BaseStyledComponent |
| CssClass | string | ✅ Match | From BaseStyledComponent; applied to wrapper `<span>` |
| Enabled | bool | ✅ Match | From BaseWebFormsComponent; renders `disabled` on input |
| Font | FontInfo | ✅ Match | From BaseStyledComponent |
| ForeColor | Color | ✅ Match | From BaseStyledComponent |
| Height | Unit | ✅ Match | From BaseStyledComponent |
| Width | Unit | ✅ Match | From BaseStyledComponent |
| TabIndex | short | ✅ Match | From BaseWebFormsComponent |
| ToolTip | string | 🔴 Missing | Not implemented on this component |
| Style | CssStyleCollection | ✅ Match | Computed from BaseStyledComponent |

### Control Inherited Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | string | ✅ Match | From BaseWebFormsComponent |
| ClientID | string | ✅ Match | Used for input `id` and label `for` |
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
| CheckedChanged | EventHandler | ✅ Match | `EventCallback<ChangeEventArgs> OnCheckedChanged` + `EventCallback<bool> CheckedChanged` |
| Init | EventHandler | ✅ Match | Via base class |
| Load | EventHandler | ✅ Match | Via base class |
| PreRender | EventHandler | ✅ Match | Via base class |
| Unload | EventHandler | ✅ Match | Via base class |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| Focus() | void | N/A | Server-only |
| DataBind() | void | N/A | Server-only |

## HTML Output Comparison

Web Forms renders a `<span>` wrapper containing an `<input type="checkbox">` and a `<label>` element. The label is positioned left or right based on `TextAlign`. When no text is provided, just the bare `<input>` is rendered without a wrapper.

The Blazor component matches this pattern: `<span class="..." style="..."><input id="..." type="checkbox" /><label for="...">Text</label></span>`. The `for` attribute links the label to the input via a generated or client ID.

## Summary

- **Matching:** 17 properties, 5 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 6 properties (AccessKey, ToolTip, CausesValidation, ValidationGroup, InputAttributes, LabelAttributes), 0 events
- **N/A (server-only):** 7 items

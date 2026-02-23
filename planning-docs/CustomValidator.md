# CustomValidator — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.customvalidator?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.Validations.CustomValidator`
**Implementation Status:** ✅ Implemented

## Properties

### CustomValidator-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ClientValidationFunction | `string` | N/A | Client-side JavaScript function name; not applicable in Blazor |
| ValidateEmptyText | `bool` | ✅ Match | `[Parameter] public bool ValidateEmptyText` |

### Inherited from BaseValidator

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ControlToValidate | `string` | ⚠️ Needs Work | Blazor uses `ForwardRef<InputBase<string>>` instead of string ID |
| Display | `ValidatorDisplay` | 🔴 Missing | Controls None/Static/Dynamic display behavior |
| EnableClientScript | `bool` | N/A | Server-side concept |
| ErrorMessage | `string` | ✅ Match | `[Parameter] public string ErrorMessage` |
| ForeColor | `Color` | ✅ Match | Via `BaseStyledComponent.ForeColor` |
| IsValid | `bool` | ✅ Match | Internal state `protected bool IsValid` |
| SetFocusOnError | `bool` | 🔴 Missing | Focus management on validation failure |
| Text | `string` | ✅ Match | `[Parameter] public string Text` |
| ValidationGroup | `string` | ✅ Match | `[Parameter] public string ValidationGroup` |

### Inherited from WebControl (via BaseStyledComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | `string` | 🔴 Missing | Not implemented |
| BackColor | `Color` | ✅ Match | Via `BaseStyledComponent.BackColor` |
| BorderColor | `Color` | ✅ Match | Via `BaseStyledComponent.BorderColor` |
| BorderStyle | `BorderStyle` | ✅ Match | Via `BaseStyledComponent.BorderStyle` |
| BorderWidth | `Unit` | ✅ Match | Via `BaseStyledComponent.BorderWidth` |
| CssClass | `string` | ✅ Match | Via `BaseStyledComponent.CssClass` |
| Enabled | `bool` | ✅ Match | Via `BaseWebFormsComponent.Enabled` |
| Font | `FontInfo` | ✅ Match | Via `BaseStyledComponent.Font` |
| Height | `Unit` | ✅ Match | Via `BaseStyledComponent.Height` |
| Style | `CssStyleCollection` | ⚠️ Needs Work | Computed via `ToStyle()`, not a direct parameter |
| TabIndex | `short` | ✅ Match | Via `BaseWebFormsComponent.TabIndex` |
| ToolTip | `string` | 🔴 Missing | Not implemented |
| Width | `Unit` | ✅ Match | Via `BaseStyledComponent.Width` |

### Inherited from Control

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | `string` | ✅ Match | Via `BaseWebFormsComponent.ID` |
| Visible | `bool` | ✅ Match | Via `BaseWebFormsComponent.Visible` |
| ClientID | `string` | ✅ Match | Via `BaseWebFormsComponent.ClientID` |
| EnableViewState | `bool` | N/A | Server-side only; stub exists |
| ViewState | `StateBag` | N/A | Server-side only |
| Parent | `Control` | ✅ Match | Via `BaseWebFormsComponent.Parent` |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| ServerValidate | `ServerValidateEventHandler` | ⚠️ Needs Work | Blazor uses `Func<string, bool> ServerValidate` parameter instead of event — different signature (no `ServerValidateEventArgs`) |
| DataBinding | `EventHandler` | ✅ Match | Via `BaseWebFormsComponent.OnDataBinding` |
| Init | `EventHandler` | ✅ Match | Via `BaseWebFormsComponent.OnInit` |
| Load | `EventHandler` | ✅ Match | Via `BaseWebFormsComponent.OnLoad` |
| PreRender | `EventHandler` | ✅ Match | Via `BaseWebFormsComponent.OnPreRender` |
| Unload | `EventHandler` | ✅ Match | Via `BaseWebFormsComponent.OnUnload` |
| Disposed | `EventHandler` | ✅ Match | Via `BaseWebFormsComponent.OnDisposed` |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| Validate() | `void Validate()` | ⚠️ Needs Work | Exists as `bool Validate(string value)` — different signature |
| DataBind() | `void DataBind()` | N/A | No-op stub |
| Focus() | `void Focus()` | 🔴 Missing | Client focus management |
| FindControl() | `Control FindControl(string)` | ✅ Match | Via `BaseWebFormsComponent.FindControl` |

## HTML Output Comparison

Web Forms renders a `<span>` with error text. Blazor also renders a `<span>` with inline styles when invalid. Output is functionally equivalent. Note: Blazor's `CustomValidator` is hard-typed to `string` (`BaseValidator<string>`) which limits it to string input controls, whereas Web Forms `CustomValidator` works with any control type.

## Summary

- **Matching:** 16 properties, 6 events
- **Needs Work:** 2 properties (ControlToValidate, Style), 1 event (ServerValidate signature), 1 method (Validate)
- **Missing:** 4 properties (Display, SetFocusOnError, ToolTip, AccessKey), 1 method (Focus)
- **N/A (server-only):** 4 items (ClientValidationFunction, EnableClientScript, EnableViewState, ViewState)

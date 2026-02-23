# CompareValidator — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.comparevalidator?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.Validations.CompareValidator<InputType>`
**Implementation Status:** ✅ Implemented

## Properties

### CompareValidator-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ControlToCompare | `string` | 🔴 Missing | Web Forms compares against another control by ID; Blazor only supports `ValueToCompare` |
| Operator | `ValidationCompareOperator` | ✅ Match | `[Parameter] public ValidationCompareOperator Operator` — defaults to `Equal` |
| ValueToCompare | `string` | ✅ Match | `[Parameter] public string ValueToCompare` |

### Inherited from BaseCompareValidator

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| CultureInvariantValues | `bool` | ✅ Match | `[Parameter] public bool CultureInvariantValues` |
| Type | `ValidationDataType` | ✅ Match | `[Parameter] public ValidationDataType Type` — defaults to `String` |

### Inherited from BaseValidator

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ControlToValidate | `string` | ⚠️ Needs Work | Blazor uses `ForwardRef<InputBase<Type>>` instead of string ID — different API shape |
| Display | `ValidatorDisplay` | 🔴 Missing | Controls None/Static/Dynamic display behavior |
| EnableClientScript | `bool` | N/A | Client-script is a server-side concept |
| ErrorMessage | `string` | ✅ Match | `[Parameter] public string ErrorMessage` |
| ForeColor | `Color` | ✅ Match | Inherited via `BaseStyledComponent.ForeColor` |
| IsValid | `bool` | ✅ Match | `protected bool IsValid` (not a parameter, internal state) |
| SetFocusOnError | `bool` | 🔴 Missing | Focus management on validation failure |
| Text | `string` | ✅ Match | `[Parameter] public string Text` |
| ValidationGroup | `string` | ✅ Match | `[Parameter] public string ValidationGroup` |

### Inherited from WebControl (via BaseStyledComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | `string` | 🔴 Missing | Not in BaseStyledComponent or BaseWebFormsComponent |
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
| EnableViewState | `bool` | N/A | Server-side only; parameter exists but does nothing |
| ViewState | `StateBag` | N/A | Server-side only |
| NamingContainer | `Control` | N/A | Server-side only |
| Page | `Page` | N/A | Server-side only |
| Parent | `Control` | ✅ Match | Via `BaseWebFormsComponent.Parent` |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
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
| DataBind() | `void DataBind()` | N/A | No-op stub exists |
| Focus() | `void Focus()` | 🔴 Missing | Client focus management |
| FindControl() | `Control FindControl(string)` | ✅ Match | Via `BaseWebFormsComponent.FindControl` |

## HTML Output Comparison

Web Forms renders a `<span>` element with the error message. Blazor also renders a `<span>` with inline style when validation fails. The output is functionally equivalent, though Blazor uses inline styles via `CalculatedStyle` rather than CSS class-based styling by default. The `Display` property (None/Static/Dynamic) that controls visibility behavior in Web Forms is not implemented.

## Summary

- **Matching:** 18 properties, 6 events
- **Needs Work:** 3 properties (ControlToValidate API shape, Style, Validate signature)
- **Missing:** 4 properties (ControlToCompare, Display, SetFocusOnError, ToolTip, AccessKey)
- **N/A (server-only):** 5 items (EnableClientScript, EnableViewState, ViewState, NamingContainer, Page)

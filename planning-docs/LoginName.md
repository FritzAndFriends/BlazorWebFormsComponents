# LoginName — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.loginname?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.LoginControls.LoginName`
**Implementation Status:** ✅ Implemented

## Properties

### LoginName-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| FormatString | `string` | ✅ Match | `[Parameter] public string FormatString` — defaults to `"{0}"` |

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
| ForeColor | `Color` | ✅ Match | Via `BaseStyledComponent.ForeColor` |
| Height | `Unit` | ✅ Match | Via `BaseStyledComponent.Height` |
| Style | `CssStyleCollection` | ✅ Match | Via `BaseStyledComponent.Style` |
| TabIndex | `short` | ✅ Match | Via `BaseWebFormsComponent.TabIndex` |
| ToolTip | `string` | 🔴 Missing | Not implemented |
| Width | `Unit` | ✅ Match | Via `BaseStyledComponent.Width` |

### Inherited from Control

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | `string` | ✅ Match | Via `BaseWebFormsComponent.ID` |
| Visible | `bool` | ✅ Match | Via `BaseWebFormsComponent.Visible` |
| ClientID | `string` | ✅ Match | Via `BaseWebFormsComponent.ClientID` |
| EnableViewState | `bool` | N/A | Server-side only |
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
| DataBind() | `void DataBind()` | N/A | No-op stub |
| Focus() | `void Focus()` | 🔴 Missing | Not implemented |
| FindControl() | `Control FindControl(string)` | ✅ Match | |
| RenderBeginTag() | -- | N/A | Server-side rendering |

## HTML Output Comparison

Web Forms renders a `<span>` with the formatted username. Blazor also renders a `<span>` with `style` and `class` attributes, displaying the formatted username. Only renders when user is authenticated, matching Web Forms behavior.

The Blazor component correctly inherits from `BaseStyledComponent`, giving it full style support.

## Summary

- **Matching:** 15 properties, 6 events
- **Needs Work:** 0
- **Missing:** 2 properties (AccessKey, ToolTip), 1 method (Focus)
- **N/A (server-only):** 1 item (EnableViewState)

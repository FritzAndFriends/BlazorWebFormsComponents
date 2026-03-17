>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# Label — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.label?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.Label`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Text | `string` | ✅ Match | `[Parameter]` on Label |
| AssociatedControlID | `string` | 🔴 Missing | Sets `for` attribute on rendered `<label>` element |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| ClientID | `string` (read-only) | ✅ Match | Computed via `ComponentIdGenerator` |
| Visible | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Enabled | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| TabIndex | `short` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| CssClass | `string` | 🔴 Missing | Label inherits `BaseWebFormsComponent`, not `BaseStyledComponent` |
| BackColor | `Color` | 🔴 Missing | No style support — wrong base class |
| ForeColor | `Color` | 🔴 Missing | No style support — wrong base class |
| BorderColor | `Color` | 🔴 Missing | No style support — wrong base class |
| BorderStyle | `BorderStyle` | 🔴 Missing | No style support — wrong base class |
| BorderWidth | `Unit` | 🔴 Missing | No style support — wrong base class |
| Font | `FontInfo` | 🔴 Missing | No style support — wrong base class |
| Height | `Unit` | 🔴 Missing | No style support — wrong base class |
| Width | `Unit` | 🔴 Missing | No style support — wrong base class |
| AccessKey | `string` | 🔴 Missing | Not implemented in any base class |
| ToolTip | `string` | 🔴 Missing | Not implemented in any base class |
| Style | `CssStyleCollection` | 🔴 Missing | No style support — wrong base class |
| EnableViewState | `bool` | N/A | Obsolete, accepted but ignored |
| EnableTheming | `bool` | N/A | Obsolete, accepted but ignored |
| SkinID | `string` | N/A | Obsolete, accepted but ignored |
| ViewState | `StateBag` | N/A | Server-side only |
| ViewStateMode | `ViewStateMode` | N/A | Server-side only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| Init | `EventHandler` | ✅ Match | `OnInit` EventCallback on base |
| Load | `EventHandler` | ✅ Match | `OnLoad` EventCallback on base |
| PreRender | `EventHandler` | ✅ Match | `OnPreRender` EventCallback on base |
| Unload | `EventHandler` | ✅ Match | `OnUnload` EventCallback on base |
| Disposed | `EventHandler` | ✅ Match | `OnDisposed` EventCallback on base |
| DataBinding | `EventHandler` | ✅ Match | `OnDataBinding` EventCallback on base |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | `void DataBind()` | N/A | No-op in Blazor |
| Focus() | `void Focus()` | 🔴 Missing | Would require JS interop |
| FindControl() | `Control FindControl(string)` | ✅ Match | On `BaseWebFormsComponent` |

## HTML Output Comparison

**Web Forms** renders a `<span>` with the ID and text content:
```html
<span id="Label1">Hello World</span>
```

**Blazor** renders the same structure:
```html
<span id="Label1">Hello World</span>
```

✅ HTML output matches. However, Web Forms Label renders a `<label for="...">` when `AssociatedControlID` is set — this is not supported.

## Summary

- **Matching:** 7 properties, 6 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 11 properties (all WebControl style props — Label uses wrong base class), 0 events
- **N/A (server-only):** 4 items

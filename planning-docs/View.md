# View — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.view?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.View`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ChildContent | — | ✅ Match | Blazor-specific `RenderFragment` for child content |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Visible | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent`; controlled by parent MultiView |
| Enabled | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| EnableViewState | `bool` | N/A | Obsolete, accepted but ignored |
| EnableTheming | `bool` | N/A | Obsolete, accepted but ignored |
| SkinID | `string` | N/A | Obsolete, accepted but ignored |
| ViewState | `StateBag` | N/A | Server-side only |

Note: Web Forms `View` inherits from `Control`, **not** `WebControl`. It has no style properties by design. The Blazor implementation correctly inherits from `BaseWebFormsComponent`.

### Cascading Parameters

| Parameter | Type | Notes |
|-----------|------|-------|
| ParentMultiView | `MultiView` | `[CascadingParameter]` — automatically registers with parent MultiView |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| Activate | `EventHandler` | ✅ Match | `OnActivate` EventCallback<EventArgs> — called via `NotifyActivated()` |
| Deactivate | `EventHandler` | ✅ Match | `OnDeactivate` EventCallback<EventArgs> — called via `NotifyDeactivated()` |
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
| FindControl() | `Control FindControl(string)` | ✅ Match | On `BaseWebFormsComponent` |
| Focus() | `void Focus()` | N/A | Not applicable — container control |

## HTML Output Comparison

**Web Forms** renders no wrapper HTML — only the child content is rendered when the View is active (visible).

**Blazor** renders the same — only `ChildContent` appears when `Visible` is true. Visibility is toggled by the parent `MultiView`.

✅ HTML output matches. No wrapper element in either case.

## Summary

- **Matching:** 4 properties, 8 events
- **Needs Work:** 0
- **Missing:** 0
- **N/A (server-only):** 4 items

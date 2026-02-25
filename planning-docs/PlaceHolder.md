>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# PlaceHolder — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.placeholder?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.PlaceHolder`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ChildContent | — | ✅ Match | Blazor-specific `RenderFragment` for child content |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Visible | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Enabled | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Controls | `ControlCollection` | ✅ Match | `List<BaseWebFormsComponent>` on `BaseWebFormsComponent` |
| EnableViewState | `bool` | N/A | Obsolete, accepted but ignored |
| EnableTheming | `bool` | N/A | Obsolete, accepted but ignored |
| SkinID | `string` | N/A | Obsolete, accepted but ignored |
| ViewState | `StateBag` | N/A | Server-side only |

Note: Web Forms `PlaceHolder` inherits from `Control`, **not** `WebControl`. It has no style properties by design — it is purely a container for dynamically added controls. The Blazor implementation correctly inherits from `BaseWebFormsComponent` and uses `RenderFragment ChildContent` for content projection (the Blazor equivalent of dynamically adding controls).

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
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
| Focus() | `void Focus()` | N/A | Not applicable — renders no element |

## HTML Output Comparison

**Web Forms** renders no wrapper HTML — only the child controls' output appears.

**Blazor** renders no wrapper HTML — only `ChildContent` appears (when `Visible` is true).

✅ HTML output matches. Neither version produces any wrapper element.

## Summary

- **Matching:** 5 properties, 6 events
- **Needs Work:** 0
- **Missing:** 0
- **N/A (server-only):** 4 items

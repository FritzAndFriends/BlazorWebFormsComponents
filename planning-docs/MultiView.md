# MultiView — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.multiview?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.MultiView`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ActiveViewIndex | `int` | ✅ Match | `[Parameter]` with validation and range checking |
| Views | `ViewCollection` (read-only) | ✅ Match | `List<View>` — populated via `RegisterView` |
| ChildContent | — | ✅ Match | Blazor-specific `RenderFragment` for child views |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Visible | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Enabled | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| EnableViewState | `bool` | N/A | Obsolete, accepted but ignored |
| EnableTheming | `bool` | N/A | Obsolete, accepted but ignored |
| SkinID | `string` | N/A | Obsolete, accepted but ignored |
| ViewState | `StateBag` | N/A | Server-side only |

Note: Web Forms `MultiView` inherits from `Control`, **not** `WebControl`. It has no style properties by design. The Blazor implementation correctly inherits from `BaseWebFormsComponent`.

### Constants

| Constant | Web Forms | Blazor Status | Notes |
|----------|-----------|---------------|-------|
| NextViewCommandName | `"NextView"` | ✅ Match | Static const on MultiView |
| PreviousViewCommandName | `"PrevView"` | ✅ Match | Static const on MultiView |
| SwitchViewByIDCommandName | `"SwitchViewByID"` | ✅ Match | Static const on MultiView |
| SwitchViewByIndexCommandName | `"SwitchViewByIndex"` | ✅ Match | Static const on MultiView |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| ActiveViewChanged | `EventHandler` | ✅ Match | `OnActiveViewChanged` EventCallback<EventArgs> |
| Init | `EventHandler` | ✅ Match | `OnInit` on base |
| Load | `EventHandler` | ✅ Match | `OnLoad` on base |
| PreRender | `EventHandler` | ✅ Match | `OnPreRender` on base |
| Unload | `EventHandler` | ✅ Match | `OnUnload` on base |
| Disposed | `EventHandler` | ✅ Match | `OnDisposed` on base |
| DataBinding | `EventHandler` | ✅ Match | `OnDataBinding` on base |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| GetActiveView() | `View GetActiveView()` | ✅ Match | Returns `Views[ActiveViewIndex]` with validation |
| SetActiveView(View) | `void SetActiveView(View)` | ✅ Match | Looks up view index and sets `ActiveViewIndex` |
| DataBind() | `void DataBind()` | N/A | No-op in Blazor |
| Focus() | `void Focus()` | N/A | Not applicable — container control |
| FindControl() | `Control FindControl(string)` | ✅ Match | On `BaseWebFormsComponent` |

## HTML Output Comparison

**Web Forms** renders no wrapper HTML — only the active View's content is rendered.

**Blazor** uses `<CascadingValue>` internally but renders no visible wrapper — only the active View's content appears in the DOM.

✅ HTML output matches. The `CascadingValue` wrapper is invisible in rendered HTML.

## Summary

- **Matching:** 6 properties, 4 constants, 7 events, 3 methods
- **Needs Work:** 0
- **Missing:** 0
- **N/A (server-only):** 4 items

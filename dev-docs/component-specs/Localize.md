>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# Localize — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.localize?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.Localize`
**Implementation Status:** ✅ Implemented

## Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Text | `string` | ✅ Match | Inherited from `Literal` |
| Mode | `LiteralMode` | ✅ Match | Inherited from `Literal` |
| ID | `string` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Visible | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| Enabled | `bool` | ✅ Match | Inherited from `BaseWebFormsComponent` |
| EnableViewState | `bool` | N/A | Obsolete, accepted but ignored |
| EnableTheming | `bool` | N/A | Obsolete, accepted but ignored |
| SkinID | `string` | N/A | Obsolete, accepted but ignored |
| ViewState | `StateBag` | N/A | Server-side only |

Note: Web Forms `Localize` inherits from `Literal`. It exists primarily for Visual Studio design-time support — the designer treats `<asp:Localize>` as a localizable control and maps its `Text` property to resource files. The runtime behavior is identical to `Literal`. The Blazor implementation (`public class Localize : Literal { }`) correctly mirrors this relationship. In Blazor, developers should pass localized strings (via `IStringLocalizer`) to the `Text` property.

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
| Focus() | `void Focus()` | N/A | Not applicable — renders no focusable element |
| FindControl() | `Control FindControl(string)` | ✅ Match | On `BaseWebFormsComponent` |

## HTML Output Comparison

Identical to Literal. Localize renders no wrapper element — just raw or encoded text.

✅ HTML output matches.

## Summary

- **Matching:** 5 properties, 6 events
- **Needs Work:** 0 properties, 0 events
- **Missing:** 0 properties, 0 events
- **N/A (server-only):** 4 items

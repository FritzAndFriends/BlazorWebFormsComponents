>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# LoginView — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.loginview?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.LoginControls.LoginView`
**Implementation Status:** ✅ Implemented

## Properties

### LoginView-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AnonymousTemplate | `ITemplate` | ✅ Match | `[Parameter] public RenderFragment AnonymousTemplate` |
| LoggedInTemplate | `ITemplate` | ✅ Match | `[Parameter] public RenderFragment LoggedInTemplate` |
| RoleGroups | `RoleGroupCollection` | ✅ Match | `[Parameter] public RoleGroupCollection RoleGroups` — custom implementation |

### Inherited from Control (via BaseWebFormsComponent)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | `string` | ✅ Match | Via `BaseWebFormsComponent.ID` |
| Visible | `bool` | ✅ Match | Via `BaseWebFormsComponent.Visible` |
| Enabled | `bool` | ✅ Match | Via `BaseWebFormsComponent.Enabled` |
| TabIndex | `short` | ✅ Match | Via `BaseWebFormsComponent.TabIndex` |
| EnableViewState | `bool` | N/A | Server-side only |

### Missing Web Forms Properties (from WebControl)

> **Note:** Web Forms `LoginView` inherits from `Control` (not `WebControl`), so it does NOT have style properties like BackColor, CssClass, Font, etc. This is consistent with the Blazor implementation inheriting from `BaseWebFormsComponent`.

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| EnableTheming | `bool` | N/A | Theming not applicable in Blazor |
| SkinID | `string` | N/A | Theming not applicable in Blazor |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| ViewChanged | `EventHandler` | 🔴 Missing | Fires when the active view changes between anonymous/logged-in/role |
| ViewChanging | `EventHandler` | 🔴 Missing | Fires before the active view changes |
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
| Focus() | `void Focus()` | N/A | Web Forms `LoginView` inherits `Control`, not `WebControl` — no `Focus()` |
| FindControl() | `Control FindControl(string)` | ✅ Match | |

## HTML Output Comparison

Web Forms `LoginView` renders no wrapper HTML — it simply outputs the content of the active template. Blazor similarly renders the active template's RenderFragment directly.

The template selection logic matches Web Forms:
1. If user is not authenticated → render `AnonymousTemplate`
2. If user is authenticated and matches a role group → render that role group's content
3. If user is authenticated but no role group matches → render `LoggedInTemplate`

The `RoleGroupCollection` with `GetRoleGroup(ClaimsPrincipal)` provides role-based template selection using Blazor's claims-based identity system, which is the natural Blazor equivalent of Web Forms' role provider.

## Summary

- **Matching:** 7 properties, 6 events
- **Needs Work:** 0
- **Missing:** 2 events (ViewChanged, ViewChanging)
- **N/A (server-only):** 3 items (EnableTheming, SkinID, EnableViewState)

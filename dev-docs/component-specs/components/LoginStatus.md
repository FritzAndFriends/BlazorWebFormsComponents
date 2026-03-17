>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# LoginStatus — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.loginstatus?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.LoginControls.LoginStatus`
**Implementation Status:** ✅ Implemented

## Properties

### LoginStatus-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| LoginImageUrl | `string` | ✅ Match | `[Parameter] public string LoginImageUrl` |
| LoginText | `string` | ✅ Match | Defaults to "Login" |
| LogoutAction | `LogoutAction` | ✅ Match | `[Parameter] public LogoutAction LogoutAction` — defaults to `Refresh` |
| LogoutImageUrl | `string` | ✅ Match | `[Parameter] public string LogoutImageUrl` |
| LogoutPageUrl | `string` | ✅ Match | `[Parameter] public string LogoutPageUrl` |
| LogoutText | `string` | ✅ Match | Defaults to "Logout" |

### Additional Blazor Properties (not in Web Forms)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| LoginPageUrl | `string` | N/A | Blazor addition — not in Web Forms (WF uses FormsAuthentication.LoginUrl) |

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
| LoggedOut | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnLoggedOut` |
| LoggingOut | `LoginCancelEventHandler` | ✅ Match | `EventCallback<LoginCancelEventArgs> OnLoggingOut` |
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

## HTML Output Comparison

Web Forms renders either an `<a>` tag (for text links) or `<input type="image">` (for image buttons) depending on the login/logout state and image URL configuration. Blazor produces the same output with matching element IDs (`{ID}_status`).

Key differences:
- Web Forms uses `FormsAuthentication.LoginUrl` for login redirect; Blazor uses a custom `LoginPageUrl` parameter
- Blazor `LogoutAction.Refresh` doesn't actually refresh — the developer must handle `OnLoggedOut` to perform actual sign-out logic
- Web Forms automatically calls `FormsAuthentication.SignOut()`; Blazor delegates this to the developer

The component correctly inherits from `BaseStyledComponent`, giving it full style support.

## Summary

- **Matching:** 19 properties, 8 events
- **Needs Work:** 0
- **Missing:** 2 properties (AccessKey, ToolTip), 1 method (Focus)
- **N/A (server-only):** 1 item (EnableViewState)

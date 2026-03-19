>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# Login — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.login?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.LoginControls.Login`
**Implementation Status:** ✅ Implemented

## Properties

### Login-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| BorderPadding | `int` | ✅ Match | Defaults to 1 |
| CreateUserIconUrl | `string` | ✅ Match | |
| CreateUserText | `string` | ✅ Match | |
| CreateUserUrl | `string` | ✅ Match | |
| DestinationPageUrl | `string` | ✅ Match | Navigates after successful login |
| DisplayRememberMe | `bool` | ✅ Match | Defaults to `true` |
| FailureText | `string` | ✅ Match | Defaults to "Your login attempt was not successful. Please try again." |
| HelpPageIconUrl | `string` | ✅ Match | |
| HelpPageText | `string` | ✅ Match | |
| HelpPageUrl | `string` | ✅ Match | |
| InstructionText | `string` | ✅ Match | |
| LoginButtonImageUrl | `string` | ✅ Match | |
| LoginButtonText | `string` | ✅ Match | Defaults to "Log In" |
| LoginButtonType | `ButtonType` | ✅ Match | Defaults to `Button` |
| Password | `string` | ✅ Match | Backed by Model |
| PasswordLabelText | `string` | ✅ Match | Defaults to "Password:" |
| PasswordRecoveryIconUrl | `string` | ✅ Match | |
| PasswordRecoveryText | `string` | ✅ Match | |
| PasswordRecoveryUrl | `string` | ✅ Match | |
| PasswordRequiredErrorMessage | `string` | ✅ Match | |
| RememberMeSet | `bool` | ✅ Match | Maps to `Model.RememberMe` |
| RememberMeText | `string` | ✅ Match | Defaults to "Remember me next time." |
| TitleText | `string` | ✅ Match | Defaults to "Log In" |
| UserName | `string` | ✅ Match | Backed by Model |
| UserNameLabelText | `string` | ✅ Match | Defaults to "User Name:" |
| UserNameRequiredErrorMessage | `string` | ✅ Match | |
| VisibleWhenLoggedIn | `bool` | ✅ Match | Defaults to `true` |

### Missing Login Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| FailureAction | `LoginFailureAction` | 🔴 Missing | Commented out in code — controls redirect vs. inline failure |
| LayoutTemplate | `ITemplate` | 🔴 Missing | Commented out — allows complete layout override |
| MembershipProvider | `string` | N/A | Marked obsolete |
| Orientation | `Orientation` | 🔴 Missing | Commented out — horizontal/vertical layout |
| RenderOuterTable | `bool` | 🔴 Missing | Commented out — controls outer table rendering |
| TextLayout | `LoginTextLayout` | 🔴 Missing | Commented out — text-on-left vs text-on-top |

### Style Properties (via CascadingParameters)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| CheckBoxStyle | `Style` | ✅ Match | Via CascadingParameter |
| FailureTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| HyperLinkStyle | `Style` | ✅ Match | Via CascadingParameter |
| InstructionTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| LabelStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| LoginButtonStyle | `Style` | ✅ Match | Via CascadingParameter |
| TextBoxStyle | `Style` | ✅ Match | Via CascadingParameter |
| TitleTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| ValidatorTextStyle | `Style` | ✅ Match | Via CascadingParameter |

### Inherited from WebControl

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | `string` | 🔴 Missing | Inherits `BaseWebFormsComponent`, not `BaseStyledComponent` |
| BackColor | `Color` | 🔴 Missing | Same |
| BorderColor | `Color` | 🔴 Missing | Same |
| CssClass | `string` | 🔴 Missing | Same |
| Font | `FontInfo` | 🔴 Missing | Same |
| ForeColor | `Color` | 🔴 Missing | Same |
| Height | `Unit` | 🔴 Missing | Same |
| Style | `CssStyleCollection` | 🔴 Missing | Same |
| ToolTip | `string` | 🔴 Missing | Same |
| Width | `Unit` | 🔴 Missing | Same |

### Inherited from Control

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | `string` | ✅ Match | Via `BaseWebFormsComponent.ID` |
| Visible | `bool` | ✅ Match | Via `BaseWebFormsComponent.Visible` |
| Enabled | `bool` | ✅ Match | Via `BaseWebFormsComponent.Enabled` |
| TabIndex | `short` | ✅ Match | Via `BaseWebFormsComponent.TabIndex` |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| Authenticate | `AuthenticateEventHandler` | ✅ Match | `EventCallback<AuthenticateEventArgs> OnAuthenticate` |
| LoggedIn | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnLoggedIn` |
| LoggingIn | `LoginCancelEventHandler` | ✅ Match | `EventCallback<LoginCancelEventArgs> OnLoggingIn` |
| LoginError | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnLoginError` |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | `void DataBind()` | N/A | No-op stub |
| Focus() | `void Focus()` | 🔴 Missing | |
| FindControl() | `Control FindControl(string)` | ✅ Match | |

## HTML Output Comparison

Web Forms renders a nested `<table>` layout with title, instruction, username/password fields, remember me checkbox, login button, and links. Blazor produces an identical table structure with `cellspacing="0"`, `cellpadding`, `border-collapse:collapse`. Field IDs follow the Web Forms pattern (`{ID}_UserName`, `{ID}_Password`, etc.). Button types (Button, Link, Image) are all rendered correctly.

The Blazor component wraps everything in an `EditForm` for validation, which is the Blazor equivalent of ASP.NET's form validation. Built-in `RequiredFieldValidator` instances are added for username and password.

## Summary

- **Matching:** 36 properties, 4 events
- **Needs Work:** 0
- **Missing:** 16 properties (FailureAction, LayoutTemplate, Orientation, RenderOuterTable, TextLayout, WebControl styles)
- **N/A (server-only):** 1 item (MembershipProvider)

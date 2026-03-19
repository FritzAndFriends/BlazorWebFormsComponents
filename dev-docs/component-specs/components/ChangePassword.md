>  **Historical Snapshot (Pre-Milestone 6):** This audit was conducted before Milestones 6-8 which closed the majority of gaps listed below. For current status, see `status.md` and `planning-docs/MILESTONE9-PLAN.md`.

# ChangePassword — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.changepassword?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.LoginControls.ChangePassword`
**Implementation Status:** ✅ Implemented

## Properties

### Button Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| CancelButtonImageUrl | `string` | ✅ Match | `[Parameter] public string CancelButtonImageUrl` |
| CancelButtonText | `string` | ✅ Match | Defaults to "Cancel" |
| CancelButtonType | `ButtonType` | ✅ Match | `[Parameter] public ButtonType CancelButtonType` |
| CancelDestinationPageUrl | `string` | ✅ Match | `[Parameter] public string CancelDestinationPageUrl` |
| ChangePasswordButtonImageUrl | `string` | ✅ Match | `[Parameter] public string ChangePasswordButtonImageUrl` |
| ChangePasswordButtonText | `string` | ✅ Match | Defaults to "Change Password" |
| ChangePasswordButtonType | `ButtonType` | ✅ Match | `[Parameter] public ButtonType ChangePasswordButtonType` |
| ContinueButtonImageUrl | `string` | ✅ Match | `[Parameter] public string ContinueButtonImageUrl` |
| ContinueButtonText | `string` | ✅ Match | Defaults to "Continue" |
| ContinueButtonType | `ButtonType` | ✅ Match | `[Parameter] public ButtonType ContinueButtonType` |
| ContinueDestinationPageUrl | `string` | ✅ Match | `[Parameter] public string ContinueDestinationPageUrl` |

### Text Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ChangePasswordTitleText | `string` | ✅ Match | Defaults to "Change Your Password" |
| ConfirmNewPasswordLabelText | `string` | ✅ Match | Defaults to "Confirm New Password:" |
| ConfirmPasswordCompareErrorMessage | `string` | ✅ Match | |
| ConfirmPasswordRequiredErrorMessage | `string` | ✅ Match | |
| ChangePasswordFailureText | `string` | ✅ Match | |
| InstructionText | `string` | ✅ Match | |
| NewPasswordLabelText | `string` | ✅ Match | |
| NewPasswordRegularExpression | `string` | ✅ Match | |
| NewPasswordRegularExpressionErrorMessage | `string` | ✅ Match | |
| NewPasswordRequiredErrorMessage | `string` | ✅ Match | |
| PasswordHintText | `string` | ✅ Match | |
| PasswordLabelText | `string` | ✅ Match | Defaults to "Password:" |
| PasswordRequiredErrorMessage | `string` | ✅ Match | |
| SuccessPageUrl | `string` | ✅ Match | |
| SuccessText | `string` | ✅ Match | |
| SuccessTitleText | `string` | ✅ Match | Defaults to "Change Password Complete" |

### User Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| CurrentPassword | `string` | ✅ Match | Public property backed by Model |
| DisplayUserName | `bool` | ✅ Match | |
| NewPassword | `string` | ✅ Match | Public property backed by Model |
| UserName | `string` | ✅ Match | |
| UserNameLabelText | `string` | ✅ Match | |
| UserNameRequiredErrorMessage | `string` | ✅ Match | |

### Link Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| CreateUserIconUrl | `string` | ✅ Match | |
| CreateUserText | `string` | ✅ Match | |
| CreateUserUrl | `string` | ✅ Match | |
| EditProfileIconUrl | `string` | ✅ Match | |
| EditProfileText | `string` | ✅ Match | |
| EditProfileUrl | `string` | ✅ Match | |
| HelpPageIconUrl | `string` | ✅ Match | |
| HelpPageText | `string` | ✅ Match | |
| HelpPageUrl | `string` | ✅ Match | |
| PasswordRecoveryIconUrl | `string` | ✅ Match | |
| PasswordRecoveryText | `string` | ✅ Match | |
| PasswordRecoveryUrl | `string` | ✅ Match | |

### Layout Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| BorderPadding | `int` | ✅ Match | Defaults to 1 |
| RenderOuterTable | `bool` | ✅ Match | Parameter exists but always renders table in template |

### Template Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ChangePasswordTemplate | `ITemplate` | ✅ Match | `[Parameter] public RenderFragment ChangePasswordTemplate` |
| SuccessTemplate | `ITemplate` | ✅ Match | `[Parameter] public RenderFragment SuccessTemplate` |

### Style Properties (via CascadingParameters)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| CancelButtonStyle | `Style` | 🔴 Missing | No dedicated cancel button style |
| ChangePasswordButtonStyle | `Style` | ⚠️ Needs Work | Uses `LoginButtonStyle` cascading parameter |
| ContinueButtonStyle | `Style` | ⚠️ Needs Work | Uses `LoginButtonStyle` cascading parameter |
| FailureTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| HyperLinkStyle | `Style` | ✅ Match | Via CascadingParameter |
| InstructionTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| LabelStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| PasswordHintStyle | `TableItemStyle` | 🔴 Missing | No dedicated password hint style |
| SuccessTextStyle | `TableItemStyle` | 🔴 Missing | Not implemented |
| TextBoxStyle | `Style` | ✅ Match | Via CascadingParameter |
| TitleTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| ValidatorTextStyle | `Style` | ✅ Match | Via CascadingParameter |

### Missing Web Forms Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| MailDefinition | `MailDefinition` | 🔴 Missing | Email notification on password change |
| MembershipProvider | `string` | N/A | Marked obsolete — `[Parameter, Obsolete]` |
| Orientation | `Orientation` | 🔴 Missing | Horizontal/vertical layout (commented out in code) |
| PasswordRecoveryText/Url | -- | ✅ Match | Already listed in Link Properties |

### Inherited from WebControl

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | `string` | 🔴 Missing | Not implemented |
| BackColor | `Color` | 🔴 Missing | ChangePassword inherits `BaseWebFormsComponent`, not `BaseStyledComponent` |
| BorderColor | `Color` | 🔴 Missing | Same — no style base class |
| CssClass | `string` | 🔴 Missing | Same — no style base class |
| Font | `FontInfo` | 🔴 Missing | Same |
| ForeColor | `Color` | 🔴 Missing | Same |
| Height | `Unit` | 🔴 Missing | Same |
| Style | `CssStyleCollection` | 🔴 Missing | Same |
| ToolTip | `string` | 🔴 Missing | Not implemented |
| Width | `Unit` | 🔴 Missing | Same |

### Inherited from Control

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | `string` | ✅ Match | Via `BaseWebFormsComponent.ID` |
| Visible | `bool` | ✅ Match | Via `BaseWebFormsComponent.Visible` |
| Enabled | `bool` | ✅ Match | Via `BaseWebFormsComponent.Enabled` |
| TabIndex | `short` | ✅ Match | Via `BaseWebFormsComponent.TabIndex` |
| EnableViewState | `bool` | N/A | Server-side only |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| CancelButtonClick | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnCancelButtonClick` |
| ChangedPassword | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnChangedPassword` |
| ChangePasswordError | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnChangePasswordError` |
| ChangingPassword | `LoginCancelEventHandler` | ✅ Match | `EventCallback<LoginCancelEventArgs> OnChangingPassword` |
| ContinueButtonClick | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnContinueButtonClick` |
| SendingMail | `MailMessageEventHandler` | 🔴 Missing | Email functionality not implemented |
| SendMailError | `SendMailErrorEventHandler` | 🔴 Missing | Email functionality not implemented |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | `void DataBind()` | N/A | No-op stub |
| Focus() | `void Focus()` | 🔴 Missing | Not implemented |
| FindControl() | `Control FindControl(string)` | ✅ Match | Via `BaseWebFormsComponent.FindControl` |

## HTML Output Comparison

Web Forms renders a `<table>` layout with nested `<table>` for fields. Blazor produces the same table-based layout with `cellspacing="0"`, `border-collapse:collapse`, consistent with Web Forms output. Success view also uses table layout. Template support (`ChangePasswordTemplate`, `SuccessTemplate`) allows custom rendering.

Key difference: Blazor inherits from `BaseWebFormsComponent` (not `BaseStyledComponent`), so outer-level style properties (BackColor, CssClass, ForeColor, Width, Height, etc.) from WebControl are **not available**. Sub-element styles are handled via CascadingParameters.

## Summary

- **Matching:** 44 properties, 5 events
- **Needs Work:** 2 properties (button style sharing)
- **Missing:** 14 properties (WebControl styles, MailDefinition, Orientation, PasswordHintStyle, SuccessTextStyle, CancelButtonStyle), 2 events (mail events)
- **N/A (server-only):** 2 items (MembershipProvider, EnableViewState)

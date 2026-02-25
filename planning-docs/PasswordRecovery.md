# PasswordRecovery — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.passwordrecovery?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.LoginControls.PasswordRecovery`
**Implementation Status:** ✅ Implemented

## Properties

### PasswordRecovery-Specific Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AnswerLabelText | `string` | 🔴 Missing | Web Forms default: "Answer:" — not implemented; `QuestionLabelText` serves a similar role |
| AnswerRequiredErrorMessage | `string` | ✅ Match | Default: "Answer is required." |
| BorderPadding | `int` | ✅ Match | Default: 1 |
| GeneralFailureText | `string` | ✅ Match | Default: "Your attempt to retrieve your password was not successful. Please try again." |
| HelpPageIconUrl | `string` | ✅ Match | |
| HelpPageText | `string` | ✅ Match | |
| HelpPageUrl | `string` | ✅ Match | |
| MailDefinition | `MailDefinition` | N/A | Email sending is a server concern; string placeholder parameter exists |
| MembershipProvider | `string` | N/A | Marked `[Obsolete]` — server-side membership provider |
| QuestionFailureText | `string` | ✅ Match | Default: "Your answer could not be verified. Please try again." |
| QuestionInstructionText | `string` | ✅ Match | Default: "Answer the following question to receive your password." |
| QuestionLabelText | `string` | ✅ Match | Default: "Answer:" |
| QuestionTitleText | `string` | ✅ Match | Default: "Identity Confirmation" |
| RenderOuterTable | `bool` | ⚠️ Needs Work | Parameter exists (default: true) but not wired to rendering logic |
| SuccessPageUrl | `string` | ✅ Match | Navigates via NavigationManager |
| SuccessText | `string` | ✅ Match | Default: "Your password has been sent to you." |
| SubmitButtonImageUrl | `string` | ⚠️ Needs Work | Parameter exists but not used in default rendering |
| SubmitButtonText | `string` | ✅ Match | Default: "Submit" |
| SubmitButtonType | `ButtonType` | ⚠️ Needs Work | Parameter exists but default rendering always uses `<input type="submit">` |
| UserName | `string` | ✅ Match | Public property backed by internal Model |
| UserNameFailureText | `string` | ✅ Match | Default: "Your attempt to retrieve your password was not successful. Please try again." |
| UserNameInstructionText | `string` | ✅ Match | Default: "Enter your User Name to receive your password." |
| UserNameLabelText | `string` | ✅ Match | Default: "User Name:" |
| UserNameRequiredErrorMessage | `string` | ✅ Match | Default: "User Name is required." |
| UserNameTitleText | `string` | ✅ Match | Default: "Forgot Your Password?" |

### Template Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| QuestionTemplate | `ITemplate` | ✅ Match | `RenderFragment` — Step 2: Security question template |
| SuccessTemplate | `ITemplate` | ✅ Match | `RenderFragment` — Step 3: Success message template |
| UserNameTemplate | `ITemplate` | ✅ Match | `RenderFragment` — Step 1: Username entry template |

### Style Properties (via CascadingParameters)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| FailureTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| HyperLinkStyle | `Style` | ✅ Match | Via CascadingParameter (as TableItemStyle) |
| InstructionTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| LabelStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| SubmitButtonStyle | `Style` | ✅ Match | Via CascadingParameter (mapped as "LoginButtonStyle") |
| SuccessTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| TextBoxStyle | `Style` | ✅ Match | Via CascadingParameter |
| TitleTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| ValidatorTextStyle | `Style` | ✅ Match | Via CascadingParameter |

### Inherited from WebControl

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | `string` | ✅ Match | Via `BaseWebFormsComponent` |
| BackColor | `Color` | 🔴 Missing | Inherits `BaseWebFormsComponent`, not `BaseStyledComponent` |
| BorderColor | `Color` | 🔴 Missing | Same |
| BorderStyle | `BorderStyle` | 🔴 Missing | Same |
| BorderWidth | `Unit` | 🔴 Missing | Same |
| CssClass | `string` | 🔴 Missing | Same |
| Enabled | `bool` | ✅ Match | Via `BaseWebFormsComponent` |
| Font | `FontInfo` | 🔴 Missing | Same |
| ForeColor | `Color` | 🔴 Missing | Same |
| Height | `Unit` | 🔴 Missing | Same |
| Style | `CssStyleCollection` | 🔴 Missing | Same |
| TabIndex | `short` | ✅ Match | Via `BaseWebFormsComponent` |
| ToolTip | `string` | 🔴 Missing | Same |
| Width | `Unit` | 🔴 Missing | Same |

### Inherited from Control

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | `string` | ✅ Match | Via `BaseWebFormsComponent.ID` |
| Visible | `bool` | ✅ Match | Via `BaseWebFormsComponent.Visible` |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| AnswerLookupError | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnAnswerLookupError` |
| SendingMail | `MailMessageEventHandler` | ✅ Match | `EventCallback<MailMessageEventArgs> OnSendingMail` |
| SendMailError | `SendMailErrorEventHandler` | ✅ Match | `EventCallback<SendMailErrorEventArgs> OnSendMailError` |
| UserLookupError | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnUserLookupError` |
| VerifyingAnswer | `LoginCancelEventHandler` | ✅ Match | `EventCallback<LoginCancelEventArgs> OnVerifyingAnswer` |
| VerifyingUser | `LoginCancelEventHandler` | ✅ Match | `EventCallback<LoginCancelEventArgs> OnVerifyingUser` |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | `void DataBind()` | N/A | No-op stub |
| Focus() | `void Focus()` | 🔴 Missing | Server-initiated focus requires JS interop |
| FindControl() | `Control FindControl(string)` | ✅ Match | Via `BaseWebFormsComponent` |

## HTML Output Comparison

Web Forms `PasswordRecovery` renders a 3-step wizard:
1. **Step 1 (UserName):** Username input with submit button inside nested `<table>` layout
2. **Step 2 (Question):** Security question display with answer input
3. **Step 3 (Success):** Success message

The Blazor component produces matching table structures with `cellspacing="0"`, `cellpadding`, `border-collapse:collapse`. Field IDs follow the Web Forms pattern (`{ID}_UserName`, `{ID}_Answer`, `{ID}_SubmitButton`, `{ID}_HelpLink`). Steps 1 and 2 wrap content in an `EditForm` for Blazor validation. Help links and icons are conditionally rendered matching the Web Forms pattern. Style properties are applied via CascadingParameters and the `HandleUnknownAttributes` pattern used by other login controls.

## Summary

- **Matching:** 36 properties, 6 events
- **Needs Work:** 3 properties (RenderOuterTable, SubmitButtonImageUrl, SubmitButtonType)
- **Missing:** 12 properties (AnswerLabelText + 11 WebControl style properties)
- **N/A (server-only):** 2 properties (MailDefinition, MembershipProvider)

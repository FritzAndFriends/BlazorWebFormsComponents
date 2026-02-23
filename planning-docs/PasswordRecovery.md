# PasswordRecovery — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.passwordrecovery?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.LoginControls.PasswordRecovery`
**Implementation Status:** 🔴 Not Found in Source

> **Note:** Despite history.md referencing PasswordRecovery tests in Sprint 3 (29 bUnit tests), no `PasswordRecovery.razor` or `PasswordRecovery.razor.cs` file exists in `src/BlazorWebFormsComponents/LoginControls/`. The component may exist on a different branch or may have been removed. This audit documents the expected Web Forms API for future implementation reference.

## Properties

### PasswordRecovery-Specific Properties (Web Forms API)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AnswerLabelText | `string` | 🔴 Missing | Default: "Answer:" |
| AnswerRequiredErrorMessage | `string` | 🔴 Missing | |
| BorderPadding | `int` | 🔴 Missing | Default: 1 |
| GeneralFailureText | `string` | 🔴 Missing | Default: "Your attempt to retrieve your password was not successful." |
| HelpPageIconUrl | `string` | 🔴 Missing | |
| HelpPageText | `string` | 🔴 Missing | |
| HelpPageUrl | `string` | 🔴 Missing | |
| MailDefinition | `MailDefinition` | 🔴 Missing | Email config for sending recovered password |
| MembershipProvider | `string` | 🔴 Missing | |
| QuestionFailureText | `string` | 🔴 Missing | |
| QuestionInstructionText | `string` | 🔴 Missing | |
| QuestionLabelText | `string` | 🔴 Missing | |
| QuestionTitleText | `string` | 🔴 Missing | |
| RenderOuterTable | `bool` | 🔴 Missing | |
| SuccessPageUrl | `string` | 🔴 Missing | |
| SuccessText | `string` | 🔴 Missing | |
| SubmitButtonImageUrl | `string` | 🔴 Missing | |
| SubmitButtonText | `string` | 🔴 Missing | Default: "Submit" |
| SubmitButtonType | `ButtonType` | 🔴 Missing | |
| UserName | `string` | 🔴 Missing | |
| UserNameFailureText | `string` | 🔴 Missing | |
| UserNameInstructionText | `string` | 🔴 Missing | |
| UserNameLabelText | `string` | 🔴 Missing | Default: "User Name:" |
| UserNameRequiredErrorMessage | `string` | 🔴 Missing | |
| UserNameTitleText | `string` | 🔴 Missing | Default: "Forgot Your Password?" |

### Template Properties (Web Forms API)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| QuestionTemplate | `ITemplate` | 🔴 Missing | Step 2: Security question template |
| SuccessTemplate | `ITemplate` | 🔴 Missing | Step 3: Success message template |
| UserNameTemplate | `ITemplate` | 🔴 Missing | Step 1: Username entry template |

### Style Properties (Web Forms API)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| FailureTextStyle | `TableItemStyle` | 🔴 Missing | |
| HyperLinkStyle | `Style` | 🔴 Missing | |
| InstructionTextStyle | `TableItemStyle` | 🔴 Missing | |
| LabelStyle | `TableItemStyle` | 🔴 Missing | |
| SubmitButtonStyle | `Style` | 🔴 Missing | |
| SuccessTextStyle | `TableItemStyle` | 🔴 Missing | |
| TextBoxStyle | `Style` | 🔴 Missing | |
| TitleTextStyle | `TableItemStyle` | 🔴 Missing | |
| ValidatorTextStyle | `Style` | 🔴 Missing | |

### Inherited from WebControl

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | `string` | 🔴 Missing | |
| BackColor | `Color` | 🔴 Missing | |
| BorderColor | `Color` | 🔴 Missing | |
| BorderStyle | `BorderStyle` | 🔴 Missing | |
| BorderWidth | `Unit` | 🔴 Missing | |
| CssClass | `string` | 🔴 Missing | |
| Enabled | `bool` | 🔴 Missing | |
| Font | `FontInfo` | 🔴 Missing | |
| ForeColor | `Color` | 🔴 Missing | |
| Height | `Unit` | 🔴 Missing | |
| Style | `CssStyleCollection` | 🔴 Missing | |
| TabIndex | `short` | 🔴 Missing | |
| ToolTip | `string` | 🔴 Missing | |
| Width | `Unit` | 🔴 Missing | |

### Inherited from Control

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ID | `string` | 🔴 Missing | |
| Visible | `bool` | 🔴 Missing | |

## Events

| Event | Web Forms Signature | Blazor Status | Notes |
|-------|-------------------|---------------|-------|
| AnswerLookupError | `EventHandler` | 🔴 Missing | Incorrect security answer |
| SendingMail | `MailMessageEventHandler` | 🔴 Missing | Email sending |
| SendMailError | `SendMailErrorEventHandler` | 🔴 Missing | Email error |
| UserLookupError | `EventHandler` | 🔴 Missing | User not found |
| VerifyingAnswer | `LoginCancelEventHandler` | 🔴 Missing | Before verifying answer |
| VerifyingUser | `LoginCancelEventHandler` | 🔴 Missing | Before verifying user |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | `void DataBind()` | 🔴 Missing | |
| Focus() | `void Focus()` | 🔴 Missing | |
| FindControl() | `Control FindControl(string)` | 🔴 Missing | |

## HTML Output Comparison

Web Forms `PasswordRecovery` renders a 3-step wizard:
1. **Step 1 (UserName):** Username input with submit button
2. **Step 2 (Question):** Security question with answer input
3. **Step 3 (Success):** Success message

Each step renders in a `<table>` layout. The Blazor component does not exist in the source tree, so no output comparison is possible.

## Summary

- **Matching:** 0 properties, 0 events
- **Needs Work:** 0
- **Missing:** ALL — 52 properties, 6 events, 3 methods (component not found in source)
- **N/A (server-only):** N/A

> ⚠️ **Action Required:** Locate the PasswordRecovery component. History.md references Sprint 3 delivery with 29 tests. Check `dev` branch or other feature branches.

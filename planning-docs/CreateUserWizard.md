# CreateUserWizard — Feature Comparison Audit

**ASP.NET Docs:** https://learn.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.createuserwizard?view=netframework-4.8
**Blazor Component:** `BlazorWebFormsComponents.LoginControls.CreateUserWizard`
**Implementation Status:** ✅ Implemented

## Properties

### User Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| Answer | `string` | ✅ Match | Backed by Model |
| AnswerLabelText | `string` | ✅ Match | Defaults to "Security Answer:" |
| AnswerRequiredErrorMessage | `string` | ✅ Match | |
| AutoGeneratePassword | `bool` | ✅ Match | Hides password fields when true |
| ConfirmPassword | `string` | ⚠️ Needs Work | Available via `Model.ConfirmPassword` but not exposed as `[Parameter]` |
| ConfirmPasswordCompareErrorMessage | `string` | ✅ Match | |
| ConfirmPasswordLabelText | `string` | ✅ Match | |
| ConfirmPasswordRequiredErrorMessage | `string` | ✅ Match | |
| DisableCreatedUser | `bool` | ✅ Match | |
| Email | `string` | ✅ Match | Backed by Model |
| EmailLabelText | `string` | ✅ Match | |
| EmailRegularExpression | `string` | ✅ Match | |
| EmailRegularExpressionErrorMessage | `string` | ✅ Match | |
| EmailRequiredErrorMessage | `string` | ✅ Match | |
| LoginCreatedUser | `bool` | ✅ Match | Defaults to `true` |
| Password | `string` | ✅ Match | Backed by Model |
| PasswordHintText | `string` | ✅ Match | |
| PasswordLabelText | `string` | ✅ Match | |
| PasswordRegularExpression | `string` | ✅ Match | |
| PasswordRegularExpressionErrorMessage | `string` | ✅ Match | |
| PasswordRequiredErrorMessage | `string` | ✅ Match | |
| Question | `string` | ✅ Match | Backed by Model |
| QuestionLabelText | `string` | ✅ Match | |
| QuestionRequiredErrorMessage | `string` | ✅ Match | |
| RequireEmail | `bool` | ✅ Match | Defaults to `true` |
| UserName | `string` | ✅ Match | Backed by Model |
| UserNameLabelText | `string` | ✅ Match | |
| UserNameRequiredErrorMessage | `string` | ✅ Match | |

### Button Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| CancelButtonImageUrl | `string` | ✅ Match | |
| CancelButtonText | `string` | ✅ Match | Defaults to "Cancel" |
| CancelButtonType | `ButtonType` | ✅ Match | |
| CancelDestinationPageUrl | `string` | ✅ Match | |
| ContinueButtonImageUrl | `string` | ✅ Match | |
| ContinueButtonText | `string` | ✅ Match | Defaults to "Continue" |
| ContinueButtonType | `ButtonType` | ✅ Match | |
| ContinueDestinationPageUrl | `string` | ✅ Match | |
| CreateUserButtonImageUrl | `string` | ✅ Match | |
| CreateUserButtonText | `string` | ✅ Match | Defaults to "Create User" |
| CreateUserButtonType | `ButtonType` | ✅ Match | |
| DisplayCancelButton | `bool` | ✅ Match | |

### Text / Error Message Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| CompleteSuccessText | `string` | ✅ Match | Defaults to "Your account has been successfully created." |
| DuplicateEmailErrorMessage | `string` | ✅ Match | |
| DuplicateUserNameErrorMessage | `string` | ✅ Match | |
| InstructionText | `string` | ✅ Match | |
| InvalidAnswerErrorMessage | `string` | ✅ Match | |
| InvalidEmailErrorMessage | `string` | ✅ Match | |
| InvalidPasswordErrorMessage | `string` | ✅ Match | |
| InvalidQuestionErrorMessage | `string` | ✅ Match | |
| UnknownErrorMessage | `string` | ✅ Match | |

### Link Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| EditProfileIconUrl | `string` | ✅ Match | |
| EditProfileText | `string` | ✅ Match | |
| EditProfileUrl | `string` | ✅ Match | |
| HelpPageIconUrl | `string` | ✅ Match | |
| HelpPageText | `string` | ✅ Match | |
| HelpPageUrl | `string` | ✅ Match | |

### Layout Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| ActiveStepIndex | `int` | ✅ Match | `[Parameter] public int ActiveStepIndex` |
| BorderPadding | `int` | ✅ Match | Defaults to 1 |
| DisplaySideBar | `bool` | ✅ Match | Defaults to `true` |
| RenderOuterTable | `bool` | ✅ Match | Parameter exists |

### Template Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| CompleteStep | `CompleteWizardStep` | ✅ Match | `[Parameter] public RenderFragment CompleteStep` — simplified to RenderFragment |
| CreateUserStep | `CreateUserWizardStep` | ✅ Match | `[Parameter] public RenderFragment CreateUserStep` — simplified to RenderFragment |
| HeaderTemplate | `ITemplate` | ✅ Match | `[Parameter] public RenderFragment HeaderTemplate` |
| SideBarTemplate | `ITemplate` | ✅ Match | `[Parameter] public RenderFragment SideBarTemplate` |
| WizardSteps | `WizardStepCollection` | 🔴 Missing | Web Forms supports arbitrary wizard steps; Blazor only has CreateUser + Complete |

### Style Properties (via CascadingParameters)

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| CancelButtonStyle | `Style` | 🔴 Missing | No dedicated style |
| CompleteSuccessTextStyle | `TableItemStyle` | 🔴 Missing | |
| ContinueButtonStyle | `Style` | ⚠️ Needs Work | Uses `LoginButtonStyle` |
| CreateUserButtonStyle | `Style` | ⚠️ Needs Work | Uses `LoginButtonStyle` |
| FailureTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| HeaderStyle | `TableItemStyle` | 🔴 Missing | |
| HyperLinkStyle | `Style` | ✅ Match | Via CascadingParameter |
| InstructionTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| LabelStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| NavigationButtonStyle | `Style` | 🔴 Missing | Wizard navigation style |
| NavigationStyle | `TableItemStyle` | 🔴 Missing | Wizard navigation container style |
| PasswordHintStyle | `TableItemStyle` | 🔴 Missing | |
| SideBarButtonStyle | `Style` | 🔴 Missing | |
| SideBarStyle | `TableItemStyle` | 🔴 Missing | |
| StepStyle | `TableItemStyle` | 🔴 Missing | |
| TextBoxStyle | `Style` | ✅ Match | Via CascadingParameter |
| TitleTextStyle | `TableItemStyle` | ✅ Match | Via CascadingParameter |
| ValidatorTextStyle | `Style` | ✅ Match | Via CascadingParameter |

### Missing Web Forms Properties

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| MailDefinition | `MailDefinition` | 🔴 Missing | Email after user creation |
| MembershipProvider | `string` | N/A | Marked obsolete |
| CompleteStepTitleText | `string` | 🔴 Missing | Hardcoded to "Complete" in template |
| CreateUserStepTitleText | `string` | 🔴 Missing | Hardcoded to "Sign Up for Your New Account" |

### Inherited from WebControl

| Property | Web Forms Type | Blazor Status | Notes |
|----------|---------------|---------------|-------|
| AccessKey | `string` | 🔴 Missing | Inherits `BaseWebFormsComponent`, not `BaseStyledComponent` |
| BackColor | `Color` | 🔴 Missing | Same |
| CssClass | `string` | 🔴 Missing | Same |
| Font | `FontInfo` | 🔴 Missing | Same |
| ForeColor | `Color` | 🔴 Missing | Same |
| Height | `Unit` | 🔴 Missing | Same |
| ToolTip | `string` | 🔴 Missing | |
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
| ActiveStepChanged | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnActiveStepChanged` |
| CancelButtonClick | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnCancelButtonClick` |
| ContinueButtonClick | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnContinueButtonClick` |
| CreatedUser | `EventHandler` | ✅ Match | `EventCallback<EventArgs> OnCreatedUser` |
| CreateUserError | `CreateUserErrorEventHandler` | ✅ Match | `EventCallback<CreateUserErrorEventArgs> OnCreateUserError` |
| CreatingUser | `LoginCancelEventHandler` | ✅ Match | `EventCallback<LoginCancelEventArgs> OnCreatingUser` |
| FinishButtonClick | `WizardNavigationEventHandler` | ✅ Match | `EventCallback<EventArgs> OnFinishButtonClick` (simplified args) |
| NextButtonClick | `WizardNavigationEventHandler` | ✅ Match | `EventCallback<EventArgs> OnNextButtonClick` (simplified args) |
| PreviousButtonClick | `WizardNavigationEventHandler` | ✅ Match | `EventCallback<EventArgs> OnPreviousButtonClick` (simplified args) |
| SendingMail | `MailMessageEventHandler` | 🔴 Missing | Email not implemented |
| SendMailError | `SendMailErrorEventHandler` | 🔴 Missing | Email not implemented |
| SideBarButtonClick | `WizardNavigationEventHandler` | 🔴 Missing | No dedicated event |

## Methods

| Method | Web Forms Signature | Blazor Status | Notes |
|--------|-------------------|---------------|-------|
| DataBind() | `void DataBind()` | N/A | No-op stub |
| Focus() | `void Focus()` | 🔴 Missing | |
| FindControl() | `Control FindControl(string)` | ✅ Match | |
| GetHistory() | `ICollection GetHistory()` | 🔴 Missing | Wizard step navigation history |
| MoveTo() | `void MoveTo(WizardStepBase)` | 🔴 Missing | Navigate to specific step |

## HTML Output Comparison

Web Forms renders a complex wizard with table-based layout, sidebar navigation, header, and step content. Blazor produces a simplified 2-step wizard (CreateUser → Complete) with table layout that closely matches the default Web Forms rendering for the first two steps. Sidebar rendering is supported with `DisplaySideBar` toggle.

Key limitations:
1. Only 2 fixed steps (CreateUser + Complete) vs. Web Forms arbitrary `WizardSteps` collection
2. Step title text is hardcoded in the template, not configurable via parameters
3. Wizard navigation (MoveTo, GetHistory) is not supported

## Summary

- **Matching:** 55 properties, 9 events
- **Needs Work:** 3 properties (ConfirmPassword exposure, button style sharing)
- **Missing:** 24 properties (styles, WizardSteps, MailDefinition, step titles, WebControl properties), 3 events (mail + sidebar), 2 methods
- **N/A (server-only):** 2 items (MembershipProvider, EnableViewState)

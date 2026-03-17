# Login Controls + Blazor Identity Integration Analysis (D-09)

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-02-27
**Status:** Research / Analysis

---

## 1. Current State Audit

### 1.1 Login (`LoginControls/Login.razor` + `.razor.cs`)

**What it renders:**
- Outer `<table>` with `border-collapse:collapse`, `cellspacing="0"`, `cellpadding` set by `BorderPadding`
- Nested inner `<table cellpadding="0">` containing:
  - Title row (TitleText, default "Log In")
  - Instruction row (InstructionText)
  - Username `<InputText>` with `<label>` and `RequiredFieldValidator`
  - Password `<InputText type="password">` with `<label>` and `RequiredFieldValidator`
  - "Remember me" `<InputCheckbox>` row (when `DisplayRememberMe=true`)
  - Failure text row (when ShowFailureText is true)
  - Submit button row (supports Button, LinkButton, ImageButton types)
  - Help/PasswordRecovery/CreateUser links row
- Wrapped in `<EditForm>` with `OnValidSubmit`
- Supports Vertical/Horizontal + TextOnLeft/TextOnTop layout combinations

**Parameters/Events exposed:**
- **Layout:** `Orientation`, `TextLayout`, `BorderPadding`, `VisibleWhenLoggedIn`
- **Text:** `TitleText`, `InstructionText`, `UserNameLabelText`, `PasswordLabelText`, `RememberMeText`, `FailureText`, `LoginButtonText`
- **Links:** `CreateUserText/Url/IconUrl`, `PasswordRecoveryText/Url/IconUrl`, `HelpPageText/Url/IconUrl`
- **Button:** `LoginButtonType`, `LoginButtonImageUrl`
- **Data:** `UserName`, `Password`, `RememberMeSet`, `DestinationPageUrl`, `DisplayRememberMe`
- **Events:** `OnLoggingIn`, `OnAuthenticate`, `OnLoggedIn`, `OnLoginError`
- **Styles:** 9 cascading style sub-components (FailureTextStyle, TitleTextStyle, LabelStyle, CheckBoxStyle, HyperLinkStyle, InstructionTextStyle, TextBoxStyle, LoginButtonStyle, ValidatorTextStyle)
- **Obsolete:** `MembershipProvider` (marked obsolete)

**DI Services injected:**
- `AuthenticationStateProvider` — used to check `UserAuthenticated` in `OnInitializedAsync`
- `NavigationManager` — used for `DestinationPageUrl` redirect

**Identity integration status:**
- ⚠️ **Partial.** Auth state is read to conditionally hide the form (`VisibleWhenLoggedIn`), but **no actual sign-in occurs**. The `OnAuthenticate` callback fires and the developer must set `AuthenticateEventArgs.Authenticated = true` manually. There is no connection to `SignInManager`.

**Missing vs. Web Forms:**
- `FailureAction` (RedirectToLoginPage vs. InPlace) — commented out
- `LayoutTemplate` — commented out
- `RenderOuterTable` — commented out
- No built-in `SignInManager` integration
- No `MembershipProvider` replacement strategy

---

### 1.2 LoginName (`LoginControls/LoginName.razor` + `.razor.cs`)

**What it renders:**
- `<span>` with `style`, `class`, `title` attributes containing the formatted display name
- Only renders when user is authenticated

**Parameters/Events exposed:**
- `FormatString` (default `"{0}"`) — wraps `User.Identity.Name`
- Inherited: `Style`, `CssClass`, `ToolTip` from `BaseStyledComponent`

**DI Services injected:**
- `AuthenticationStateProvider` — reads `User.Identity.Name`

**Identity integration status:**
- ✅ **Functional.** Already correctly reads `AuthenticationStateProvider` and displays `User.Identity.Name`. This is the most complete login control.

**Missing vs. Web Forms:**
- No `ID` attribute on the `<span>` (Web Forms renders `id`)
- Does not re-render on auth state changes (only reads once in `OnInitializedAsync`)

---

### 1.3 LoginStatus (`LoginControls/LoginStatus.razor` + `.razor.cs`)

**What it renders:**
- When authenticated: logout `<a>` or `<input type="image">` with `@onclick="LogoutHandle"`
- When not authenticated: login `<a>` or `<input type="image">` with `@onclick="LoginHandle"`

**Parameters/Events exposed:**
- `LoginText`, `LoginImageUrl`, `LoginPageUrl`
- `LogoutText`, `LogoutImageUrl`, `LogoutPageUrl`, `LogoutAction` (Refresh/Redirect)
- **Events:** `OnLoggingOut`, `OnLoggedOut`

**DI Services injected:**
- `AuthenticationStateProvider` — checks auth state
- `NavigationManager` — handles login/logout navigation

**Identity integration status:**
- ⚠️ **Visual shell only.** Login navigates to `LoginPageUrl`. Logout fires `OnLoggedOut` event and optionally redirects, but **does not call `SignInManager.SignOutAsync()`**. The developer must handle sign-out in the `OnLoggedOut` callback.

**Missing vs. Web Forms:**
- `LoginPageUrl` is a custom addition (not in original Web Forms — Web Forms used Forms Auth redirect)
- No built-in sign-out mechanism
- Does not re-render on auth state changes

---

### 1.4 LoginView (`LoginControls/LoginView.razor` + `.razor.cs`)

**What it renders:**
- Cascading `LoginView` value to children (for `RoleGroup` registration)
- Calls `GetView()` which returns:
  - `AnonymousTemplate` if user is not authenticated
  - Matching `RoleGroup.ChildContent` if user is in a role
  - `LoggedInTemplate` as fallback for authenticated users

**Parameters/Events exposed:**
- `RoleGroups` (RoleGroupCollection)
- `LoggedInTemplate`, `AnonymousTemplate`, `ChildContent`

**DI Services injected:**
- `AuthenticationStateProvider` — reads user and roles

**Supporting types:**
- `RoleGroup` — has `Roles` (comma-separated string) and `ChildContent`; registers itself with parent `LoginView` via `CascadingParameter`
- `RoleGroupCollection` — `IList<RoleGroup>` with `GetRoleGroup(ClaimsPrincipal)` that checks `user.IsInRole()`

**Identity integration status:**
- ✅ **Mostly functional.** Correctly reads `AuthenticationStateProvider` and dispatches to templates based on roles. This closely mirrors `<AuthorizeView>` behavior.

**Missing vs. Web Forms:**
- Does not re-render on auth state changes (reads once in `OnInitializedAsync`)
- No `Activity` event from Web Forms
- Role matching doesn't trim whitespace around commas in `Roles` string

---

### 1.5 ChangePassword (`LoginControls/ChangePassword.razor` + `.razor.cs`)

**What it renders:**
- `<EditForm>` with two views:
  1. **Change Password form:** outer/inner `<table>` with UserName (optional), CurrentPassword, NewPassword, ConfirmNewPassword fields, submit + cancel buttons, help/recovery/create-user/edit-profile links
  2. **Success view:** table with SuccessTitleText, SuccessText, Continue button
- Supports Vertical/Horizontal + TextOnLeft/TextOnTop layouts
- Supports `ChangePasswordTemplate` and `SuccessTemplate` custom templates

**Parameters/Events exposed:**
- **Button:** Cancel, ChangePassword, Continue (Text, ImageUrl, Type for each)
- **Text:** ChangePasswordTitleText, ConfirmNewPasswordLabelText, various error messages, InstructionText, PasswordHintText, PasswordLabelText, NewPasswordLabelText, SuccessText, SuccessTitleText
- **User:** DisplayUserName, UserName, UserNameLabelText
- **Links:** CreateUser, EditProfile, HelpPage, PasswordRecovery (Text/Url/IconUrl each)
- **Layout:** BorderPadding, RenderOuterTable, Orientation, TextLayout
- **Events:** `OnCancelButtonClick`, `OnChangedPassword`, `OnChangePasswordError`, `OnChangingPassword`, `OnContinueButtonClick`
- **Templates:** ChildContent, ChangePasswordTemplate, SuccessTemplate

**DI Services injected:**
- `AuthenticationStateProvider` — reads current username when `DisplayUserName=false`
- `NavigationManager` — cancel/continue/success page navigation

**Identity integration status:**
- ⚠️ **Visual shell only.** Form renders and fires `OnChangingPassword` → assumes success if not cancelled → shows success view. **Does not call `UserManager.ChangePasswordAsync()`**. Developer must handle everything in `OnChangingPassword` callback.

**Missing vs. Web Forms:**
- No built-in password change logic
- No password policy validation (regex support declared but not wired)
- `CompareValidator` for confirm password not implemented
- `MembershipProvider` marked obsolete

---

### 1.6 CreateUserWizard (`LoginControls/CreateUserWizard.razor` + `.razor.cs`)

**What it renders:**
- `<EditForm>` with two steps:
  1. **Create User step:** table with UserName, Password, ConfirmPassword (unless AutoGeneratePassword), Email (if RequireEmail), SecurityQuestion/Answer fields, Create User + Cancel buttons, Help/EditProfile links, optional SideBar
  2. **Complete step:** table with success text and Continue button
- Supports `CreateUserStep`, `CompleteStep`, `SideBarTemplate`, `HeaderTemplate` custom templates

**Parameters/Events exposed:**
- **User:** UserName, Password, Email, Question, Answer (and their label/error message variants), RequireEmail, AutoGeneratePassword, DisableCreatedUser, LoginCreatedUser
- **Button:** Cancel, CreateUser, Continue (Text, ImageUrl, Type), DisplayCancelButton
- **Text:** CompleteSuccessText, InstructionText, various error messages (DuplicateEmail, DuplicateUserName, InvalidAnswer, etc.)
- **Links:** EditProfile, HelpPage
- **Layout:** ActiveStepIndex, BorderPadding, RenderOuterTable, DisplaySideBar
- **Events:** `OnCreatingUser`, `OnCreatedUser`, `OnCreateUserError`, `OnCancelButtonClick`, `OnContinueButtonClick`, `OnActiveStepChanged`, `OnNextButtonClick`, `OnPreviousButtonClick`, `OnFinishButtonClick`

**DI Services injected:**
- `NavigationManager` — cancel/continue navigation
- **Note:** Does NOT inject `AuthenticationStateProvider`

**Identity integration status:**
- ⚠️ **Visual shell only.** Fires `OnCreatingUser` → assumes success if not cancelled → shows complete step. **Does not call `UserManager.CreateAsync()`**. Developer must handle user creation in `OnCreatingUser` callback.

**Missing vs. Web Forms:**
- No built-in user creation logic
- No email confirmation workflow
- No auto-login after creation (despite `LoginCreatedUser` parameter)
- Security question/answer logic is UI-only
- Wizard navigation (Previous/Next/Finish) not fully implemented
- `MembershipProvider` marked obsolete

---

### 1.7 PasswordRecovery (`LoginControls/PasswordRecovery.razor` + `.razor.cs`)

**What it renders:**
- Three-step wizard (no `<EditForm>` wrapping all steps — each step has its own):
  1. **UserName step:** table with UserName input, Submit button, Help links
  2. **Question step:** table with read-only UserName, Question display, Answer input, Submit button
  3. **Success step:** table with SuccessText
- Supports `UserNameTemplate`, `QuestionTemplate`, `SuccessTemplate` custom templates

**Parameters/Events exposed:**
- **UserName step:** UserNameLabelText, UserNameTitleText, UserNameInstructionText, UserNameFailureText, SubmitButtonText/Type/ImageUrl
- **Question step:** QuestionLabelText, QuestionTitleText, QuestionInstructionText, QuestionFailureText, AnswerRequiredErrorMessage
- **Success:** SuccessText, SuccessPageUrl
- **General:** GeneralFailureText, MailDefinition, HelpPage links
- **Layout:** BorderPadding, RenderOuterTable
- **Events:** `OnVerifyingUser`, `OnUserLookupError`, `OnVerifyingAnswer`, `OnAnswerLookupError`, `OnSendingMail`, `OnSendMailError`
- **Public methods:** `SetQuestion(string)`, `SkipToSuccess()`

**DI Services injected:**
- `NavigationManager` — success page redirect
- **Note:** Does NOT inject `AuthenticationStateProvider`

**Identity integration status:**
- ⚠️ **Visual shell only.** Three-step flow fires events at each transition. Developer must handle all verification and mail sending. **Does not call `UserManager.GeneratePasswordResetTokenAsync()`** or any email services.

**Missing vs. Web Forms:**
- No built-in password reset token generation
- No email sending infrastructure
- `MailDefinition` is a string parameter with no implementation
- Security question flow is event-driven shell only
- `MembershipProvider` marked obsolete

---

## 2. Blazor Identity Architecture

### 2.1 AuthenticationStateProvider

`AuthenticationStateProvider` is the core abstraction in Blazor for accessing authentication state. It provides:

```csharp
public abstract class AuthenticationStateProvider
{
    public abstract Task<AuthenticationState> GetAuthenticationStateAsync();
    public event AuthenticationStateChangedHandler? AuthenticationStateChanged;
}
```

**Blazor Server:** The default `ServerAuthenticationStateProvider` reads from `HttpContext.User` (set by ASP.NET Core auth middleware). Auth state is per-circuit. When using Identity, `HttpContext.User` is populated by cookie authentication middleware.

**Blazor WebAssembly:** No `HttpContext` exists. Custom implementations (e.g., `RemoteAuthenticationService`) call backend APIs to determine auth state. Token-based auth is typical.

**Blazor Web App (.NET 8+):** Uses `PersistentComponentState` to pass auth state from server pre-rendering to interactive mode. The `PersistingServerAuthenticationStateProvider` and `PersistentAuthenticationStateProvider` pair handle this.

**Key concern for this library:** The components already inject `AuthenticationStateProvider` (Login, LoginName, LoginStatus, LoginView) — but they read it once in `OnInitializedAsync` and never subscribe to `AuthenticationStateChanged`. This means **auth state changes don't cause re-renders**.

### 2.2 CascadingAuthenticationState

`<CascadingAuthenticationState>` (or the `.AddCascadingAuthenticationState()` service) provides `Task<AuthenticationState>` as a cascading parameter to all descendant components. This is used by `<AuthorizeView>` and `[CascadingParameter] Task<AuthenticationState>`.

**Current usage:** None of the 7 login controls use `[CascadingParameter] Task<AuthenticationState>`. They all inject `AuthenticationStateProvider` directly and call `GetAuthenticationStateAsync()` manually. This works but misses the cascading re-render pattern.

**Recommendation:** Components should optionally accept `[CascadingParameter] Task<AuthenticationState>` to participate in the cascading auth system, falling back to the injected `AuthenticationStateProvider` when no cascading value is present.

### 2.3 AuthorizeView and LoginView

`<AuthorizeView>` is Blazor's built-in component for conditional rendering based on auth state:

```razor
<AuthorizeView>
    <Authorized>Welcome, @context.User.Identity.Name</Authorized>
    <NotAuthorized>Please log in</NotAuthorized>
</AuthorizeView>
```

`LoginView` is the Web Forms equivalent. The current implementation already mirrors this pattern:
- `AnonymousTemplate` → `<NotAuthorized>`
- `LoggedInTemplate` → `<Authorized>`
- `RoleGroups` → `<AuthorizeView Roles="...">`

**Key difference:** `AuthorizeView` uses `[CascadingParameter] Task<AuthenticationState>` and re-renders on auth changes. `LoginView` reads once and is static.

### 2.4 SignInManager<TUser> and UserManager<TUser>

These are the two primary ASP.NET Core Identity APIs:

**`SignInManager<TUser>`:**
- `PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure)` — validates credentials and creates auth cookie
- `SignOutAsync()` — removes auth cookie
- `IsSignedIn(ClaimsPrincipal)` — checks if user is signed in
- `ExternalLoginSignInAsync()` — handles OAuth/external provider sign-in
- **Blazor Server concern:** `SignInManager` relies on `HttpContext` for cookie operations. In Blazor Server interactive mode, there is no `HttpContext` during SignalR circuit operations. Sign-in/sign-out must happen via navigation to a server-rendered page or API endpoint.

**`UserManager<TUser>`:**
- `CreateAsync(user, password)` — creates a new user
- `ChangePasswordAsync(user, currentPassword, newPassword)` — changes password
- `GeneratePasswordResetTokenAsync(user)` — generates reset token
- `ResetPasswordAsync(user, token, newPassword)` — resets password with token
- `FindByNameAsync(userName)` — looks up user
- `CheckPasswordAsync(user, password)` — validates password without sign-in
- **Blazor concern:** `UserManager` does NOT depend on `HttpContext` — it can be called directly from Blazor components in any hosting model.

### 2.5 The HttpContext Problem

This is the **critical architectural constraint** for Identity integration:

1. **Blazor Server (interactive):** No `HttpContext` during SignalR operations. `SignInManager` operations that write cookies will fail or require workarounds (NavigationManager redirect to a Razor Page/endpoint).
2. **Blazor WebAssembly:** No server-side execution at all. Must call backend APIs.
3. **Blazor Web App (.NET 8+):** Static SSR pages have `HttpContext`. Interactive components do not.

**Implication:** Operations that require cookie manipulation (sign-in, sign-out) cannot be done purely within a Blazor component. They require a server-side endpoint. Operations that only query/modify data (change password, create user, check auth state) can work directly.

---

## 3. Integration Strategy for Each Control

### 3.1 Login → `SignInManager.PasswordSignInAsync()`

**Current state:** Fires `OnAuthenticate` callback. Developer must set `Authenticated = true`.

**Integration approach:**
- Cannot call `SignInManager.PasswordSignInAsync()` directly from a Blazor interactive component (no `HttpContext` for cookie creation)
- **Recommended:** Add an optional `Func<string, string, bool, Task<bool>> AuthenticateHandler` parameter
- If provided, Login calls it with (username, password, rememberMe) and uses the return value
- If not provided, falls back to existing `OnAuthenticate` event pattern
- For Identity integration, the consuming app provides a handler that calls a server endpoint or API
- Alternatively, provide a `NavigationManager`-based redirect to an Identity login endpoint (like `Account/Login`)

**What the component needs from DI:**
- `AuthenticationStateProvider` (already injected) — for `VisibleWhenLoggedIn` check
- `NavigationManager` (already injected) — for redirect-based auth
- **Optional:** `SignInManager<TUser>` — only if we attempt direct sign-in (not recommended due to HttpContext constraints)

**Events/Callbacks:**
- Keep existing: `OnLoggingIn`, `OnAuthenticate`, `OnLoggedIn`, `OnLoginError`
- Add: `Func<string, string, bool, Task<bool>> AuthenticateHandler` (opt-in, replaces OnAuthenticate when set)
- Add: `string LoginActionUrl` — URL to POST credentials to (for redirect-based flow)

**Breaking changes:** None — all additions are additive.

**Opt-in vs. auto-detect:** Opt-in. The component should not auto-detect Identity registration because the sign-in flow varies by hosting model.

---

### 3.2 LoginName → `AuthenticationStateProvider` → `User.Identity.Name`

**Current state:** ✅ Already functional. Reads `User.Identity.Name` via `AuthenticationStateProvider`.

**Integration approach:**
- No Identity-specific changes needed — `AuthenticationStateProvider` already provides the username regardless of the auth system
- **Fix needed:** Subscribe to `AuthenticationStateChanged` event to re-render when auth state changes, or accept `[CascadingParameter] Task<AuthenticationState>` for cascading re-render

**What the component needs from DI:**
- `AuthenticationStateProvider` (already injected)

**Events/Callbacks:** None needed.

**Breaking changes:** None.

**Opt-in vs. auto-detect:** N/A — works with any auth provider.

---

### 3.3 LoginStatus → `AuthenticationStateProvider` + `SignInManager.SignOutAsync()`

**Current state:** Shows login/logout links. Login navigates. Logout fires events but does nothing.

**Integration approach:**
- **Login:** Keep current `NavigationManager.NavigateTo(LoginPageUrl)` — this is the correct pattern
- **Logout:** Same HttpContext problem as Login. Cannot call `SignInManager.SignOutAsync()` directly.
- **Recommended:** Add `string LogoutActionUrl` parameter. When set, logout navigates to this URL (server endpoint that calls `SignOutAsync()` and redirects back). When not set, falls back to existing event pattern.
- Also add `Func<Task> SignOutHandler` — optional async callback that the developer can use for API-based sign-out

**What the component needs from DI:**
- `AuthenticationStateProvider` (already injected)
- `NavigationManager` (already injected)

**Events/Callbacks:**
- Keep existing: `OnLoggingOut`, `OnLoggedOut`
- Add: `Func<Task> SignOutHandler` (opt-in, called before OnLoggedOut)
- Add: `string LogoutActionUrl` (opt-in, replaces manual handling)

**Breaking changes:** None.

**Opt-in vs. auto-detect:** Opt-in.

---

### 3.4 LoginView → Map to `<AuthorizeView>` patterns

**Current state:** ✅ Mostly functional. Reads auth state, dispatches to AnonymousTemplate/LoggedInTemplate/RoleGroup templates.

**Integration approach:**
- **Primary fix:** Accept `[CascadingParameter] Task<AuthenticationState>` to participate in cascading auth system and re-render on state changes
- **Role matching fix:** Trim whitespace when splitting `Roles` string by commas
- **Optional:** Internal use of `<AuthorizeView>` instead of manual `GetView()` logic — but this would change the rendering behavior and is not recommended

**What the component needs from DI:**
- `AuthenticationStateProvider` (already injected)

**Events/Callbacks:**
- Consider adding `OnViewChanged` event (fires when auth state changes and a different template is selected)

**Breaking changes:** None if cascading parameter is additive.

**Opt-in vs. auto-detect:** Auto-detect — the cascading auth parameter is standard Blazor infrastructure.

---

### 3.5 ChangePassword → `UserManager.ChangePasswordAsync()`

**Current state:** Fires `OnChangingPassword`, assumes success if not cancelled.

**Integration approach:**
- **Good news:** `UserManager.ChangePasswordAsync()` does NOT require `HttpContext` — it can be called directly from Blazor components in any hosting model
- **Recommended:** Add `Func<string, string, string, Task<(bool Success, string ErrorMessage)>> ChangePasswordHandler` parameter
- When set, the component calls it with (userName, currentPassword, newPassword)
- The consuming app's handler calls `UserManager.ChangePasswordAsync()` and returns the result
- When not set, falls back to existing event pattern
- **Alternative (separate package):** Provide a built-in handler in `BlazorWebFormsComponents.Identity` that takes `UserManager<TUser>` from DI

**What the component needs from DI:**
- `AuthenticationStateProvider` (already injected) — for current username
- `NavigationManager` (already injected)
- **Optional (in Identity package):** `UserManager<TUser>` — for direct password change

**Events/Callbacks:**
- Keep existing: `OnChangingPassword`, `OnChangedPassword`, `OnChangePasswordError`
- Add: `Func<string, string, string, Task<(bool, string)>> ChangePasswordHandler`

**Breaking changes:** None.

**Opt-in vs. auto-detect:** Opt-in in core package. Auto-detect in Identity package.

---

### 3.6 CreateUserWizard → `UserManager.CreateAsync()`

**Current state:** Fires `OnCreatingUser`, assumes success if not cancelled.

**Integration approach:**
- **Good news:** `UserManager.CreateAsync()` does NOT require `HttpContext`
- **Recommended:** Add `Func<CreateUserModel, Task<(bool Success, string ErrorMessage)>> CreateUserHandler`
- When set, the component calls it with the model
- The consuming app's handler calls `UserManager.CreateAsync()` and returns the result
- When not set, falls back to existing event pattern
- **Post-creation sign-in:** `LoginCreatedUser` parameter exists but is not implemented. Implementation requires `SignInManager` which has the HttpContext problem. This should navigate to a login endpoint or use a `Func<Task> PostCreateSignInHandler`.

**What the component needs from DI:**
- `NavigationManager` (already injected)
- **Optional (in Identity package):** `UserManager<TUser>` — for direct user creation

**Events/Callbacks:**
- Keep existing: `OnCreatingUser`, `OnCreatedUser`, `OnCreateUserError`, etc.
- Add: `Func<CreateUserModel, Task<(bool, string)>> CreateUserHandler`
- Add: `Func<Task> PostCreateSignInHandler` (for LoginCreatedUser)

**Breaking changes:** None.

**Opt-in vs. auto-detect:** Opt-in in core package. Auto-detect in Identity package.

---

### 3.7 PasswordRecovery → `UserManager.GeneratePasswordResetTokenAsync()` + email

**Current state:** Three-step event-driven shell. Developer handles all logic.

**Integration approach:**
- **User lookup:** `UserManager.FindByNameAsync()` does NOT require `HttpContext` — can call directly
- **Token generation:** `UserManager.GeneratePasswordResetTokenAsync()` does NOT require `HttpContext`
- **Email sending:** Requires `IEmailSender` or equivalent — not part of Identity but commonly registered alongside it
- **Recommended:** Add three handler delegates:
  1. `Func<string, Task<(bool Found, string Question)>> UserLookupHandler` — looks up user, returns security question
  2. `Func<string, string, Task<bool>> AnswerVerificationHandler` — verifies security answer
  3. `Func<string, Task<bool>> SendResetHandler` — generates token and sends email
- When set, these replace the event-only pattern
- `MailDefinition` parameter should be deprecated or documented as non-functional

**What the component needs from DI:**
- `NavigationManager` (already injected)
- **Optional (in Identity package):** `UserManager<TUser>`, `IEmailSender`

**Events/Callbacks:**
- Keep existing: `OnVerifyingUser`, `OnVerifyingAnswer`, `OnSendingMail`, error events
- Add handler delegates as described above

**Breaking changes:** None.

**Opt-in vs. auto-detect:** Opt-in.

---

## 4. Recommended Approach

### 4.1 Separate `BlazorWebFormsComponents.Identity` Package: YES

**Recommendation: Create a separate NuGet package `BlazorWebFormsComponents.Identity`.**

Reasons:
1. **Dependency management.** The core `BlazorWebFormsComponents` package should not take a dependency on `Microsoft.AspNetCore.Identity`. Most consumers may not use Identity, or may use a different auth system (Auth0, Azure AD B2C, Okta, etc.).
2. **Hosting model flexibility.** The HttpContext constraints differ between Blazor Server, WebAssembly, and Web App. The Identity package can provide hosting-model-specific helpers.
3. **Web Forms parity.** In Web Forms, `MembershipProvider` was a separate subsystem that the controls called into. The Identity package plays the same role.

### 4.2 Architecture: Handler Delegates in Core, Wired Implementations in Identity

**Core package changes (additive, no breaking changes):**
- Each control gets optional `Func<>` handler delegate parameters (as described in Section 3)
- When a handler is set, the component calls it instead of relying on event-only patterns
- When no handler is set, existing behavior is preserved

**Identity package provides:**
- Extension method: `services.AddBlazorWebFormsIdentity<TUser>()` that registers:
  - A service implementing all handler delegates using `UserManager<TUser>` and `SignInManager<TUser>`
  - Endpoint mappings for cookie-based operations (login, logout) via minimal APIs or Razor Pages
- Pre-built handler implementations for each control
- Middleware/endpoint for redirect-based sign-in/sign-out

### 4.3 Minimum Viable Integration

The smallest useful integration that makes these controls functional:

1. **LoginName** — Already works. Fix auth state re-render.
2. **LoginView** — Already works. Fix auth state re-render + role matching trim.
3. **LoginStatus** — Add `LogoutActionUrl` parameter for redirect-based sign-out.
4. **Login** — Add `LoginActionUrl` parameter for redirect-based sign-in, OR `AuthenticateHandler` delegate.
5. **ChangePassword** — Add `ChangePasswordHandler` delegate.
6. **CreateUserWizard** — Add `CreateUserHandler` delegate.
7. **PasswordRecovery** — Add handler delegates for each step.

**MVP is items 1-4.** These cover the read-auth-state and sign-in/sign-out flows that 90% of apps need.

### 4.4 Non-Identity Auth (OAuth, External Providers)

The handler delegate approach inherently supports non-Identity auth:
- **OAuth/OIDC:** `AuthenticateHandler` can redirect to an OAuth endpoint. `SignOutHandler` can clear tokens.
- **External providers:** Same delegate pattern — the consuming app's handler calls whatever API is appropriate.
- **`AuthenticationStateProvider`** works with any auth system, not just Identity. The read-side components (LoginName, LoginView, LoginStatus) work today with any provider.

The key principle: **components should never directly depend on Identity types in the core package**. All Identity-specific logic goes in the Identity package or the consuming app's handler implementations.

---

## 5. Implementation Estimate

### Priority Order

| Priority | Control | Complexity | Rationale |
|----------|---------|------------|-----------|
| 1 | **LoginName** | S | Already works. Fix re-render on auth change. |
| 2 | **LoginView** | S | Already works. Fix re-render + trim roles. |
| 3 | **LoginStatus** | S | Add `LogoutActionUrl` + `SignOutHandler`. Small change. |
| 4 | **Login** | M | Add `AuthenticateHandler` delegate + `LoginActionUrl`. Redirect flow needs docs. |
| 5 | **ChangePassword** | M | Add `ChangePasswordHandler` delegate. UserManager integration is straightforward. |
| 6 | **CreateUserWizard** | L | Add `CreateUserHandler` delegate. Wizard navigation needs completion. `LoginCreatedUser` needs redirect flow. |
| 7 | **PasswordRecovery** | L | Three handler delegates. Email integration is complex. `MailDefinition` rework needed. |

### Complexity Breakdown

- **S (Small, 1-2 days):** Parameter additions, auth state subscription fix, no new DI requirements
- **M (Medium, 3-5 days):** New handler delegate pattern, documentation, sample code, unit tests
- **L (Large, 5-10 days):** Multiple handler delegates, complex flow logic, email infrastructure, wizard improvements

### Dependencies

```
LoginName (S) ──────────┐
LoginView (S) ──────────┤
LoginStatus (S) ────────┤── Can be done in parallel
                        │
Login (M) ──────────────┤── Depends on establishing handler delegate pattern
                        │
ChangePassword (M) ─────┤── Depends on handler delegate pattern from Login
                        │
CreateUserWizard (L) ────┤── Depends on handler delegate pattern
                        │
PasswordRecovery (L) ───┘── Depends on handler delegate pattern + email strategy
```

### Identity Package (Separate Track)

| Item | Complexity | Dependencies |
|------|------------|--------------|
| Package scaffold + DI registration | S | None |
| Login endpoint (cookie sign-in) | M | Login control handler delegate |
| Logout endpoint (cookie sign-out) | S | LoginStatus handler delegate |
| ChangePassword handler | S | ChangePassword handler delegate |
| CreateUser handler | M | CreateUserWizard handler delegate |
| PasswordRecovery handlers + email | L | PasswordRecovery handler delegates |

**Total estimate for core package changes:** ~4-6 weeks (one developer)
**Total estimate for Identity package:** ~3-4 weeks (one developer, can start after Login handler pattern established)

### Cross-Cutting Concerns

1. **Auth state re-render** — Affects LoginName, LoginView, LoginStatus, Login. Should be done as a single foundational change.
2. **Handler delegate pattern** — Define once in Login, replicate across all controls.
3. **Documentation** — Each control needs updated docs showing both event-only and handler delegate patterns.
4. **Samples** — Each control needs sample pages demonstrating Identity integration.
5. **Tests** — Each handler delegate path needs bUnit tests with mocked services.

---

## Appendix A: Web Forms Original Architecture

For reference, in ASP.NET Web Forms:
- Login controls used `MembershipProvider` (abstract class with SQL Server, Active Directory implementations)
- `FormsAuthentication.SetAuthCookie()` handled cookie creation
- `FormsAuthentication.SignOut()` handled cookie removal
- `Membership.GetUser()` provided user lookup
- `SmtpClient` handled email for PasswordRecovery
- All operations were synchronous and ran in the ASP.NET pipeline with `HttpContext` always available

The Blazor equivalent must account for the async nature of all operations and the HttpContext availability constraints of different hosting models.

## Appendix B: Files Examined

```
src/BlazorWebFormsComponents/LoginControls/
├── AuthenticateEventArgs.cs
├── ChangePassword.razor + .razor.cs
├── CreateUserErrorEventArgs.cs
├── CreateUserWizard.razor + .razor.cs
├── FailureTextStyle.razor + .razor.cs
├── Login.razor + .razor.cs
├── LoginCancelEventArgs.cs
├── LoginName.razor + .razor.cs
├── LoginStatus.razor + .razor.cs
├── LoginView.razor + .razor.cs
├── MailMessageEventArgs.cs
├── PasswordRecovery.razor + .razor.cs
├── RoleGroup.razor + .razor.cs
├── RoleGroupCollection.cs
├── SendMailErrorEventArgs.cs
└── [Various style sub-components]
```

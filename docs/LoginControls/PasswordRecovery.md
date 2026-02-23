# PasswordRecovery

The **PasswordRecovery** component emulates the ASP.NET Web Forms `asp:PasswordRecovery` control. It provides a three-step password recovery workflow: username identification, security question verification, and a success confirmation. The component renders table-based forms matching the original Web Forms HTML output.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.passwordrecovery?view=netframework-4.8

## Blazor Features Supported

- Three-step recovery flow:
    1. **UserName step** — User enters their username
    2. **Question step** — User answers a security question
    3. **Success step** — Confirmation message displayed
- Configurable labels and text for each step:
    - `UserNameTitleText`, `UserNameInstructionText`, `UserNameLabelText`, `UserNameFailureText`
    - `QuestionTitleText`, `QuestionInstructionText`, `QuestionLabelText`, `QuestionFailureText`
    - `SuccessText`
- `SubmitButtonText` and `SubmitButtonType` for the submit button
- `GeneralFailureText` for generic error messages
- Help page support: `HelpPageUrl`, `HelpPageText`, `HelpPageIconUrl`
- `SuccessPageUrl` — redirect after successful recovery
- Events:
    - `OnVerifyingUser` — before username validation (cancellable via `LoginCancelEventArgs`)
    - `OnUserLookupError` — after a failed user lookup
    - `OnVerifyingAnswer` — before answer validation (cancellable)
    - `OnAnswerLookupError` — after a failed answer verification
    - `OnSendingMail` — before sending recovery email
    - `OnSendMailError` — on mail send failure
- Custom templates: `UserNameTemplate`, `QuestionTemplate`, `SuccessTemplate`
- `SetQuestion()` method — set the security question text from your event handler
- `SkipToSuccess()` method — skip the question step when no security question is configured
- Styling through cascading style components:
    - `FailureTextStyle`, `TitleTextStyle`, `LabelStyle`, `InstructionTextStyle`
    - `TextBoxStyle`, `SubmitButtonStyle`, `ValidatorTextStyle`, `HyperLinkStyle`, `SuccessTextStyle`
- `BorderPadding` and `RenderOuterTable` layout properties
- Table-based layout matching Web Forms HTML output

### Blazor Notes

- The component does NOT perform any password recovery or email sending itself. You must implement the recovery logic in the `OnVerifyingUser` and `OnVerifyingAnswer` event handlers.
- To set the security question displayed in Step 2, call `SetQuestion("Your question text")` on the component reference from your `OnVerifyingUser` handler.
- If your application does not use security questions, call `SkipToSuccess()` from your `OnVerifyingUser` handler to jump directly to the success step.
- Cancel the `OnVerifyingUser` or `OnVerifyingAnswer` events (set `Cancel = true`) to display the corresponding failure text and remain on the current step.

## Web Forms Features NOT Supported

- **MembershipProvider** — marked `[Obsolete]`; use event handlers to integrate with ASP.NET Identity
- **MailDefinition** — email composition and sending must be handled in your own service
- **ViewState** — not needed; Blazor preserves component state natively
- **Theming / SkinID** — not applicable to Blazor

!!! warning "Authentication Integration"
    The PasswordRecovery component does NOT look up users, validate answers, or send emails. You must handle the `OnVerifyingUser` and `OnVerifyingAnswer` events and use ASP.NET Identity's `UserManager<T>` or your own authentication service to perform the actual recovery.

## Web Forms Declarative Syntax

```html
<asp:PasswordRecovery
    ID="PasswordRecovery1"
    UserNameTitleText="Forgot Your Password?"
    UserNameInstructionText="Enter your User Name to receive your password."
    QuestionTitleText="Identity Confirmation"
    SuccessText="Your password has been sent to you."
    SubmitButtonText="Submit"
    HelpPageUrl="~/Help.aspx"
    HelpPageText="Need help?"
    OnVerifyingUser="PasswordRecovery1_VerifyingUser"
    OnVerifyingAnswer="PasswordRecovery1_VerifyingAnswer"
    OnSendingMail="PasswordRecovery1_SendingMail"
    runat="server">
    <TitleTextStyle BackColor="#336699" ForeColor="White" Font-Bold="True" />
    <InstructionTextStyle Font-Italic="True" ForeColor="#333333" />
</asp:PasswordRecovery>
```

## Blazor Syntax

```razor
<PasswordRecovery @ref="passwordRecovery"
    ID="PasswordRecovery1"
    UserNameTitleText="Forgot Your Password?"
    UserNameInstructionText="Enter your User Name to receive your password."
    QuestionTitleText="Identity Confirmation"
    SuccessText="Your password has been sent to you."
    SubmitButtonText="Submit"
    HelpPageUrl="/help"
    HelpPageText="Need help?"
    OnVerifyingUser="HandleVerifyingUser"
    OnVerifyingAnswer="HandleVerifyingAnswer"
    OnSendingMail="HandleSendingMail" />

@code {
    private PasswordRecovery passwordRecovery;

    private async Task HandleVerifyingUser(LoginCancelEventArgs e)
    {
        // Look up the user by passwordRecovery.UserName
        // var user = await UserManager.FindByNameAsync(passwordRecovery.UserName);
        // if (user == null) { e.Cancel = true; return; }
        // passwordRecovery.SetQuestion(user.SecurityQuestion);
    }

    private async Task HandleVerifyingAnswer(LoginCancelEventArgs e)
    {
        // Validate the answer: passwordRecovery.Answer
        // if (!valid) { e.Cancel = true; return; }
    }

    private async Task HandleSendingMail(MailMessageEventArgs e)
    {
        // Send recovery email via your mail service
    }
}
```

### Skipping the Question Step

```razor
<PasswordRecovery @ref="passwordRecovery"
    OnVerifyingUser="HandleVerifyingUser" />

@code {
    private PasswordRecovery passwordRecovery;

    private async Task HandleVerifyingUser(LoginCancelEventArgs e)
    {
        var user = await UserManager.FindByNameAsync(passwordRecovery.UserName);
        if (user == null) { e.Cancel = true; return; }

        // No security question — skip directly to success
        await SendRecoveryEmail(user);
        await passwordRecovery.SkipToSuccess();
    }
}
```

### With Custom Templates

```razor
<PasswordRecovery>
    <UserNameTemplate>
        <div class="recovery-step">
            <h3>Find Your Account</h3>
            <input type="text" @bind="username" placeholder="Enter username" />
            <button @onclick="LookupUser">Next</button>
        </div>
    </UserNameTemplate>
    <SuccessTemplate>
        <div class="recovery-success">
            <h3>Check Your Email</h3>
            <p>We've sent recovery instructions to your registered email address.</p>
        </div>
    </SuccessTemplate>
</PasswordRecovery>
```

## HTML Output

### Step 1: UserName

```html
<form>
  <table cellspacing="0" cellpadding="1" id="PasswordRecovery1" style="border-collapse:collapse;">
    <tbody>
      <tr>
        <td>
          <table cellpadding="0">
            <tbody>
              <tr><td align="center" colspan="2">Forgot Your Password?</td></tr>
              <tr><td align="center" colspan="2">Enter your User Name to receive your password.</td></tr>
              <tr>
                <td align="right"><label for="PasswordRecovery1_UserName">User Name:</label></td>
                <td><input type="text" id="PasswordRecovery1_UserName" /></td>
              </tr>
              <tr>
                <td align="right" colspan="2">
                  <input type="submit" value="Submit" id="PasswordRecovery1_SubmitButton" />
                </td>
              </tr>
            </tbody>
          </table>
        </td>
      </tr>
    </tbody>
  </table>
</form>
```

### Step 2: Question

```html
<form>
  <table cellspacing="0" cellpadding="1" id="PasswordRecovery1" style="border-collapse:collapse;">
    <tbody>
      <tr>
        <td>
          <table cellpadding="0">
            <tbody>
              <tr><td align="center" colspan="2">Identity Confirmation</td></tr>
              <tr><td align="center" colspan="2">Answer the following question to receive your password.</td></tr>
              <tr>
                <td align="right">User Name:</td>
                <td>jsmith</td>
              </tr>
              <tr>
                <td align="right">Question:</td>
                <td>What is your pet's name?</td>
              </tr>
              <tr>
                <td align="right"><label for="PasswordRecovery1_Answer">Answer:</label></td>
                <td><input type="text" id="PasswordRecovery1_Answer" /></td>
              </tr>
              <tr>
                <td align="right" colspan="2">
                  <input type="submit" value="Submit" id="PasswordRecovery1_SubmitButton" />
                </td>
              </tr>
            </tbody>
          </table>
        </td>
      </tr>
    </tbody>
  </table>
</form>
```

### Step 3: Success

```html
<table cellspacing="0" cellpadding="1" id="PasswordRecovery1" style="border-collapse:collapse;">
  <tbody>
    <tr>
      <td>
        <table cellpadding="0">
          <tbody>
            <tr>
              <td align="center">Your password has been sent to you.</td>
            </tr>
          </tbody>
        </table>
      </td>
    </tr>
  </tbody>
</table>
```

## Migration Notes

1. **Remove `asp:` prefix** — Change `<asp:PasswordRecovery>` to `<PasswordRecovery>`
2. **Remove `runat="server"`** — Not needed in Blazor
3. **Replace MembershipProvider** — Handle `OnVerifyingUser` and `OnVerifyingAnswer` events with ASP.NET Identity
4. **Remove MailDefinition** — Handle email sending in your `OnSendingMail` event handler or service
5. **Use `@ref`** — Capture a component reference to call `SetQuestion()` and `SkipToSuccess()`
6. **Style migration** — Replace child style elements (`<TitleTextStyle BackColor="...">`) with cascading style components or CSS classes
7. **Event handler signatures** — `OnVerifyingUser` and `OnVerifyingAnswer` use `LoginCancelEventArgs`; `OnSendingMail` uses `MailMessageEventArgs`

### Before (Web Forms)

```html
<asp:PasswordRecovery ID="pr1"
    MembershipProvider="SqlMembershipProvider"
    OnVerifyingUser="pr1_VerifyingUser"
    OnSendingMail="pr1_SendingMail"
    runat="server">
    <TitleTextStyle BackColor="#336699" ForeColor="White" />
    <MailDefinition From="admin@example.com" Subject="Password Recovery" />
</asp:PasswordRecovery>
```

### After (Blazor)

```razor
<PasswordRecovery @ref="pr1"
    ID="pr1"
    OnVerifyingUser="HandleVerifyingUser"
    OnVerifyingAnswer="HandleVerifyingAnswer"
    OnSendingMail="HandleSendingMail"
    TitleTextStyle-CssClass="title-style" />

@code {
    private PasswordRecovery pr1;

    private async Task HandleVerifyingUser(LoginCancelEventArgs e)
    {
        var user = await UserManager.FindByNameAsync(pr1.UserName);
        if (user == null) { e.Cancel = true; return; }
        pr1.SetQuestion(user.SecurityQuestion);
    }

    private async Task HandleVerifyingAnswer(LoginCancelEventArgs e)
    {
        // Validate answer via your identity service
    }

    private async Task HandleSendingMail(MailMessageEventArgs e)
    {
        // Send email via your mail service
    }
}
```

## See Also

- [Login](Login.md) — Related login control with similar table layout
- [ChangePassword](ChangePassword.md) — Password change control
- [CreateUserWizard](CreateUserWizard.md) — User registration wizard

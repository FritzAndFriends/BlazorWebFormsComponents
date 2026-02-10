# CreateUserWizard

The **CreateUserWizard** component emulates the ASP.NET Web Forms `asp:CreateUserWizard` control. It provides a complete user registration interface with a two-step wizard: a registration form and a success confirmation page.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.createuserwizard?view=netframework-4.8

## Blazor Features Supported

- User registration form with UserName, Password, Confirm Password, Email fields
- Optional Security Question and Answer fields
- Two-step wizard: Create User step and Complete step
- `AutoGeneratePassword` — hides password fields when true
- `RequireEmail` — controls email field visibility
- `DisplaySideBar` — optional sidebar with step navigation
- `DisplayCancelButton` — optional cancel button
- Configurable labels, error messages, and button text for all fields
- `OnCreatingUser` — before user creation (cancellable via `LoginCancelEventArgs`)
- `OnCreatedUser` — after successful creation
- `OnCreateUserError` — on error (provides `CreateUserErrorEventArgs`)
- Navigation events: `OnCancelButtonClick`, `OnContinueButtonClick`, `OnActiveStepChanged`
- Custom templates: `CreateUserStep`, `CompleteStep`, `SideBarTemplate`, `HeaderTemplate`
- Styling through cascading style components
- Table-based layout matching Web Forms HTML output

## Web Forms Features NOT Supported

- `MembershipProvider` — marked `[Obsolete]`; use `OnCreatingUser` event to integrate with ASP.NET Identity
- `MailDefinition` — email sending not supported in Blazor components
- Custom `WizardStep` child components (v1 supports only the two built-in steps)
- `LoginCreatedUser` auto-login — the component fires `OnCreatedUser`; handle login in your callback
- `DisableCreatedUser` — handle in your `OnCreatingUser` callback

!!! warning "Authentication Integration"
    The CreateUserWizard does NOT create users directly. You must handle the `OnCreatingUser` event and use ASP.NET Identity's `UserManager<T>.CreateAsync()` or your own registration service to create the user account.

## Web Forms Declarative Syntax

```html
<asp:CreateUserWizard
    ID="CreateUserWizard1"
    RequireEmail="True"
    DisplaySideBar="False"
    OnCreatedUser="CreateUserWizard1_CreatedUser"
    runat="server" />
```

## Blazor Syntax

```razor
<CreateUserWizard ID="CreateUserWizard1"
    RequireEmail="true"
    DisplaySideBar="false"
    OnCreatingUser="HandleCreatingUser"
    OnCreatedUser="HandleCreatedUser"
    OnCreateUserError="HandleError" />

@code {
    private async Task HandleCreatingUser(LoginCancelEventArgs e)
    {
        // Use UserManager to create the user
        // var result = await UserManager.CreateAsync(new AppUser { UserName = ... }, password);
        // if (!result.Succeeded) { e.Cancel = true; }
    }

    private void HandleCreatedUser(EventArgs e)
    {
        // User created successfully — handle login, redirect, etc.
    }

    private void HandleError(CreateUserErrorEventArgs e)
    {
        // Display error: e.ErrorMessage
    }
}
```

## HTML Output

### Create User Step

```html
<table cellspacing="0" cellpadding="1" id="CreateUserWizard1" style="border-collapse:collapse;">
  <tbody>
    <tr>
      <td>
        <table cellpadding="0">
          <tbody>
            <tr><td align="center" colspan="2">Sign Up for Your New Account</td></tr>
            <tr>
              <td align="right"><label>User Name:</label></td>
              <td><input type="text" /></td>
            </tr>
            <tr>
              <td align="right"><label>Password:</label></td>
              <td><input type="password" /></td>
            </tr>
            <tr>
              <td align="right"><label>Confirm Password:</label></td>
              <td><input type="password" /></td>
            </tr>
            <tr>
              <td align="right"><label>E-mail:</label></td>
              <td><input type="text" /></td>
            </tr>
            <tr>
              <td align="right" colspan="2">
                <input type="submit" value="Create User" />
              </td>
            </tr>
          </tbody>
        </table>
      </td>
    </tr>
  </tbody>
</table>
```

### Complete Step

```html
<table cellspacing="0" cellpadding="1" id="CreateUserWizard1" style="border-collapse:collapse;">
  <tbody>
    <tr>
      <td>
        <table cellpadding="0">
          <tbody>
            <tr><td align="center">Complete</td></tr>
            <tr><td>Your account has been successfully created.</td></tr>
            <tr><td align="right"><input type="submit" value="Continue" /></td></tr>
          </tbody>
        </table>
      </td>
    </tr>
  </tbody>
</table>
```

## Migration Notes

1. **Remove `asp:` prefix** — Change `<asp:CreateUserWizard>` to `<CreateUserWizard>`
2. **Remove `runat="server"`** — Not needed in Blazor
3. **Replace MembershipProvider** — Handle `OnCreatingUser` event with ASP.NET Identity
4. **Remove MailDefinition** — Handle email in your own service
5. **Event handler signatures** — `OnCreatingUser` uses `LoginCancelEventArgs`; `OnCreateUserError` uses `CreateUserErrorEventArgs`

### Before (Web Forms)

```html
<asp:CreateUserWizard ID="cuw1"
    MembershipProvider="SqlMembershipProvider"
    OnCreatedUser="cuw1_CreatedUser"
    runat="server" />
```

### After (Blazor)

```razor
<CreateUserWizard ID="cuw1"
    OnCreatingUser="HandleCreating"
    OnCreatedUser="HandleCreated" />

@code {
    private async Task HandleCreating(LoginCancelEventArgs e)
    {
        // Create user via UserManager
    }

    private void HandleCreated(EventArgs e)
    {
        // Handle success
    }
}
```

## See Also

- [Login](Login.md) — Related login control
- [ChangePassword](ChangePassword.md) — Password change control

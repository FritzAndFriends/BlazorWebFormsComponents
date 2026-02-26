# ChangePassword

The **ChangePassword** component emulates the ASP.NET Web Forms `asp:ChangePassword` control. It provides a complete user interface for changing a user's password, including current password, new password, and confirmation fields with a two-view layout (Change Password view and Success view).

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.changepassword?view=netframework-4.8

## Blazor Features Supported

- Current password, new password, and confirm new password input fields
- Configurable labels and error messages for all fields
- Two-view state: Change Password view and Success view
- `DisplayUserName` — optionally show a username field
- Cancel, Change Password, and Continue buttons with configurable text and type
- Navigation via `CancelDestinationPageUrl`, `ContinueDestinationPageUrl`, `SuccessPageUrl`
- Create User, Password Recovery, Help Page, and Edit Profile links
- `InstructionText` and `PasswordHintText` display
- Failure text display on error
- `OnChangingPassword` — before password change (cancellable)
- `OnChangedPassword` — after successful password change
- `OnChangePasswordError` — on error
- `OnCancelButtonClick`, `OnContinueButtonClick` events
- Custom templates via `ChangePasswordTemplate` and `SuccessTemplate`
- Styling through cascading style components (follows Login control pattern)
- Table-based layout matching Web Forms HTML output
- **Orientation** property (Vertical or Horizontal layout)
- **TextLayout** property (TextOnLeft or TextOnTop)

## Web Forms Features NOT Supported

- `MembershipProvider` — marked `[Obsolete]`; use `OnChangingPassword` event to integrate with ASP.NET Identity
- `MailDefinition` — email sending not supported in Blazor components
- `OnSendingMail` / `OnSendMailError` events — not applicable

!!! warning "Authentication Integration"
    The ChangePassword component does NOT perform password changes directly. You must handle the `OnChangingPassword` event and use ASP.NET Identity's `UserManager<T>.ChangePasswordAsync()` or your own authentication service to perform the actual password change.

## Web Forms Declarative Syntax

```html
<asp:ChangePassword
    ID="ChangePassword1"
    ChangePasswordTitleText="Change Your Password"
    SuccessText="Your password has been changed!"
    DisplayUserName="False"
    OnChangingPassword="ChangePassword1_ChangingPassword"
    OnChangedPassword="ChangePassword1_ChangedPassword"
    runat="server" />
```

## Blazor Syntax

```razor
<ChangePassword ID="ChangePassword1"
    ChangePasswordTitleText="Change Your Password"
    DisplayUserName="false"
    OnChangingPassword="HandleChangingPassword"
    OnChangedPassword="HandleChangedPassword" />

@code {
    private async Task HandleChangingPassword(LoginCancelEventArgs e)
    {
        // Call your identity service to change the password
        // var result = await UserManager.ChangePasswordAsync(user, currentPassword, newPassword);
        // if (!result.Succeeded) { e.Cancel = true; }
    }

    private void HandleChangedPassword(EventArgs e)
    {
        // Password changed successfully
    }
}
```

## HTML Output

The component renders a table-based layout matching the Web Forms ChangePassword control:

```html
<table cellspacing="0" cellpadding="1" id="ChangePassword1" style="border-collapse:collapse;">
  <tbody>
    <tr>
      <td>
        <table cellpadding="0">
          <tbody>
            <tr><td align="center" colspan="2">Change Your Password</td></tr>
            <tr>
              <td align="right"><label>Password:</label></td>
              <td><input type="password" /></td>
            </tr>
            <tr>
              <td align="right"><label>New Password:</label></td>
              <td><input type="password" /></td>
            </tr>
            <tr>
              <td align="right"><label>Confirm New Password:</label></td>
              <td><input type="password" /></td>
            </tr>
            <tr>
              <td align="right" colspan="2">
                <input type="submit" value="Change Password" />
                <input type="button" value="Cancel" />
              </td>
            </tr>
          </tbody>
        </table>
      </td>
    </tr>
  </tbody>
</table>
```

## Migration Notes

1. **Remove `asp:` prefix** — Change `<asp:ChangePassword>` to `<ChangePassword>`
2. **Remove `runat="server"`** — Not needed in Blazor
3. **Replace MembershipProvider** — Handle `OnChangingPassword` event instead
4. **Remove MailDefinition** — Handle email notifications in your own service
5. **Event handler signatures** — `OnChangingPassword` uses `LoginCancelEventArgs` (set `Cancel = true` to abort)

### Before (Web Forms)

```html
<asp:ChangePassword ID="cp1"
    MembershipProvider="SqlMembershipProvider"
    OnChangedPassword="cp1_ChangedPassword"
    runat="server" />
```

### After (Blazor)

```razor
<ChangePassword ID="cp1"
    OnChangingPassword="HandleChanging"
    OnChangedPassword="HandleChanged" />

@code {
    private async Task HandleChanging(LoginCancelEventArgs e)
    {
        // Use UserManager to change password
    }

    private void HandleChanged(EventArgs e)
    {
        // Success handling
    }
}
```

## Orientation and TextLayout

The ChangePassword component supports two layout properties that control how the form is arranged, identical to the [Login](Login.md) control:

**Orientation** controls whether fields are stacked vertically or arranged side by side:

| Value | Description |
|-------|-------------|
| `Orientation.Vertical` | Fields stacked vertically (default) |
| `Orientation.Horizontal` | Fields arranged side by side |

**TextLayout** controls label placement relative to input fields:

| Value | Description |
|-------|-------------|
| `LoginTextLayout.TextOnLeft` | Labels appear to the left of inputs (default) |
| `LoginTextLayout.TextOnTop` | Labels appear above inputs |

```razor
@using BlazorWebFormsComponents.Enums

<ChangePassword
    ChangePasswordTitleText="Update Password"
    Orientation="Orientation.Horizontal"
    TextLayout="LoginTextLayout.TextOnTop"
    OnChangingPassword="HandleChanging" />
```

### Migration Example: Layout Properties

**Before (Web Forms):**
```html
<asp:ChangePassword ID="cp1" runat="server"
    Orientation="Horizontal"
    TextLayout="TextOnTop"
    OnChangingPassword="cp1_ChangingPassword" />
```

**After (Blazor):**
```razor
@using BlazorWebFormsComponents.Enums

<ChangePassword
    Orientation="Orientation.Horizontal"
    TextLayout="LoginTextLayout.TextOnTop"
    OnChangingPassword="HandleChanging" />

@code {
    private async Task HandleChanging(LoginCancelEventArgs e)
    {
        // Perform password change via your identity service
    }
}
```

!!! note "Migration Tip"
    In Web Forms, `Orientation` and `TextLayout` are simple strings (`"Horizontal"`, `"TextOnTop"`). In Blazor, they require the fully qualified enum values: `Orientation.Horizontal` and `LoginTextLayout.TextOnTop`. Add `@using BlazorWebFormsComponents.Enums` to your page.

## See Also

- [Login](Login.md) — Related login control with identical Orientation and TextLayout support
- [CreateUserWizard](CreateUserWizard.md) — User registration wizard

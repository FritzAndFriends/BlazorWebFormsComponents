# LoginStatus

The LoginStatus component displays a login or logout link depending on the user's authentication state. When the user is logged out, it shows a login link; when logged in, it shows a logout link. Original Web Forms documentation is at: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.loginstatus?view=netframework-4.8

## Blazor Features Supported

- Automatic display switching based on authentication state
- LoginText and LogoutText customization
- LoginImageUrl and LogoutImageUrl for image-based links
- LogoutAction (Refresh or Redirect)
- LogoutPageUrl for redirect after logout
- LoginPageUrl for navigation to login page (Blazor enhancement)
- OnLoggingOut and OnLoggedOut events
- Standard styling properties

## WebForms Features Not Supported

- LogoutAction.RedirectToLoginPage - use Redirect with LogoutPageUrl instead

## WebForms Syntax

```html
<asp:LoginStatus
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|
        Inset|Outset"
    BorderWidth="size"
    CssClass="string"
    Enabled="True|False"
    Font-Bold="True|False"
    Font-Italic="True|False"
    Font-Names="string"
    Font-Overline="True|False"
    Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|Medium|
        Large|X-Large|XX-Large"
    Font-Strikeout="True|False"
    Font-Underline="True|False"
    ForeColor="color name|#dddddd"
    Height="size"
    ID="string"
    LoginImageUrl="uri"
    LoginText="string"
    LogoutAction="Refresh|Redirect|RedirectToLoginPage"
    LogoutImageUrl="uri"
    LogoutPageUrl="uri"
    LogoutText="string"
    OnLoggedOut="LoggedOut event handler"
    OnLoggingOut="LoggingOut event handler"
    runat="server"
    SkinID="string"
    Style="string"
    TabIndex="integer"
    ToolTip="string"
    Visible="True|False"
    Width="size"
/>
```

## Blazor Syntax

```razor
<LoginStatus LoginText="Sign In"
             LogoutText="Sign Out"
             LoginPageUrl="/login"
             LogoutAction="LogoutAction.Redirect"
             LogoutPageUrl="/goodbye"
             OnLoggedOut="HandleLoggedOut" />
```

## Usage Notes

The LoginStatus component uses Blazor's `AuthenticationStateProvider` to determine the current authentication state.

### Basic Usage

```razor
<!-- Simple login/logout toggle -->
<LoginStatus />

<!-- With custom text -->
<LoginStatus LoginText="Sign In" LogoutText="Sign Out" />

<!-- With redirect after logout -->
<LoginStatus LogoutAction="LogoutAction.Redirect"
             LogoutPageUrl="/logged-out" />
```

### Logout Actions

| LogoutAction | Behavior |
|--------------|----------|
| `Refresh` | Stays on the current page after logout (default) |
| `Redirect` | Navigates to `LogoutPageUrl` after logout |

### Event Handling

```razor
<LoginStatus OnLoggingOut="HandleLoggingOut"
             OnLoggedOut="HandleLoggedOut" />

@code {
    private void HandleLoggingOut(LoginCancelEventArgs args)
    {
        // Can cancel logout by setting args.Cancel = true
        if (HasUnsavedChanges)
        {
            args.Cancel = true;
        }
    }

    private void HandleLoggedOut(EventArgs args)
    {
        // Perform cleanup after logout
        // Note: Your authentication service should handle the actual sign-out
    }
}
```

### Image-Based Links

```razor
<LoginStatus LoginImageUrl="/images/login-icon.png"
             LogoutImageUrl="/images/logout-icon.png"
             LoginPageUrl="/account/login" />
```

### Integration Notes

The LoginStatus component triggers the logout events but does not perform the actual authentication sign-out. You must handle the `OnLoggedOut` event to call your authentication service's sign-out method.

```razor
<LoginStatus OnLoggedOut="HandleLoggedOut" />

@code {
    [Inject] private AuthenticationService AuthService { get; set; }
    [Inject] private NavigationManager Navigation { get; set; }

    private async Task HandleLoggedOut(EventArgs args)
    {
        await AuthService.SignOutAsync();
        Navigation.NavigateTo("/", forceLoad: true);
    }
}
```

### LoginPageUrl Property

The `LoginPageUrl` property is a Blazor enhancement not present in the original Web Forms control. It specifies where to navigate when the login link is clicked.

```razor
<LoginStatus LoginPageUrl="/account/login"
             LogoutPageUrl="/account/logout" />
```

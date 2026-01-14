# Login

The Login component provides a complete user interface for logging into a web application, including username and password fields, a remember me checkbox, and configurable login button. Original Web Forms documentation is at: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.login?view=netframework-4.8

## Blazor Features Supported

- Username and Password input fields with labels
- Remember Me checkbox (configurable visibility)
- Login button with configurable text and type
- Required field validation for username and password
- Failure text display on authentication failure
- Password recovery, help page, and create user links
- Instruction text display
- OnLoggingIn, OnLoggedIn, OnLoginError, and OnAuthenticate events
- DestinationPageUrl for redirect after successful login
- VisibleWhenLoggedIn to control visibility for authenticated users
- Styling support through cascading style components

## WebForms Features Not Supported

- MembershipProvider - use OnAuthenticate event to implement authentication
- Orientation property
- TextLayout property
- FailureAction property
- LayoutTemplate
- RenderOuterTable property

## WebForms Syntax

```html
<asp:Login
    CreateUserIconUrl="uri"
    CreateUserText="string"
    CreateUserUrl="uri"
    DestinationPageUrl="uri"
    DisplayRememberMe="True|False"
    FailureText="string"
    HelpPageIconUrl="uri"
    HelpPageText="string"
    HelpPageUrl="uri"
    ID="string"
    InstructionText="string"
    LoginButtonImageUrl="uri"
    LoginButtonText="string"
    LoginButtonType="Button|Image|Link"
    MembershipProvider="string"
    OnAuthenticate="Authenticate event handler"
    OnLoggedIn="LoggedIn event handler"
    OnLoggingIn="LoggingIn event handler"
    OnLoginError="LoginError event handler"
    PasswordLabelText="string"
    PasswordRecoveryIconUrl="uri"
    PasswordRecoveryText="string"
    PasswordRecoveryUrl="uri"
    PasswordRequiredErrorMessage="string"
    RememberMeSet="True|False"
    RememberMeText="string"
    runat="server"
    TitleText="string"
    UserName="string"
    UserNameLabelText="string"
    UserNameRequiredErrorMessage="string"
    VisibleWhenLoggedIn="True|False"
/>
```

## Blazor Syntax

```razor
<Login TitleText="Sign In"
       UserNameLabelText="Email:"
       PasswordLabelText="Password:"
       LoginButtonText="Sign In"
       OnAuthenticate="HandleAuthenticate"
       OnLoggedIn="HandleLoggedIn"
       DestinationPageUrl="/dashboard">
    <TitleTextStyle ForeColor="Navy" Font-Bold="true" />
    <LoginButtonStyle BackColor="Blue" ForeColor="White" />
</Login>

@code {
    private void HandleAuthenticate(AuthenticateEventArgs args)
    {
        // Implement your authentication logic here
        args.Authenticated = ValidateCredentials(args);
    }

    private void HandleLoggedIn(EventArgs args)
    {
        // Handle successful login
    }
}
```

## Usage Notes

The Login component requires integration with Blazor's `AuthenticationStateProvider` for authentication state management.

### Authentication Flow

1. User enters credentials and clicks login button
2. `OnLoggingIn` event fires (can be cancelled)
3. `OnAuthenticate` event fires - you must set `args.Authenticated = true` if credentials are valid
4. If authenticated, `OnLoggedIn` fires and user is redirected to `DestinationPageUrl`
5. If not authenticated, `OnLoginError` fires and `FailureText` is displayed

### Example with Custom Authentication

```razor
<Login OnAuthenticate="HandleAuthenticate"
       OnLoggedIn="HandleLoggedIn"
       OnLoginError="HandleLoginError"
       DestinationPageUrl="/home">
</Login>

@code {
    private async Task HandleAuthenticate(AuthenticateEventArgs args)
    {
        // Your authentication logic
        var result = await AuthService.ValidateAsync(userName, password);
        args.Authenticated = result.Success;
    }
}
```

### Styling

The Login component supports child style components for customization:

| Style Component | Description |
|----------------|-------------|
| TitleTextStyle | Styles the title text |
| LabelStyle | Styles the username and password labels |
| TextBoxStyle | Styles the input text boxes |
| CheckBoxStyle | Styles the remember me checkbox |
| LoginButtonStyle | Styles the login button |
| FailureTextStyle | Styles the failure message |
| HyperLinkStyle | Styles the help and recovery links |
| InstructionTextStyle | Styles the instruction text |
| ValidatorTextStyle | Styles validation error messages |

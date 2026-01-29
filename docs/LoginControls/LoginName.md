# LoginName

The LoginName component displays the name of the currently logged-in user. It automatically retrieves the user's identity from the Blazor authentication state. Original Web Forms documentation is at: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.loginname?view=netframework-4.8

## Blazor Features Supported

- Displays authenticated user's name
- FormatString for customizing the display format
- Standard styling properties (BackColor, ForeColor, CssClass, Font, etc.)
- Only renders when user is authenticated

## WebForms Features Not Supported

- All core features are supported

## WebForms Syntax

```html
<asp:LoginName
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
    FormatString="string"
    Height="size"
    ID="string"
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
<LoginName FormatString="Welcome, {0}!" />
```

## Usage Notes

The LoginName component uses Blazor's `AuthenticationStateProvider` to retrieve the current user's authentication state and display name.

### Basic Usage

```razor
<!-- Displays just the username -->
<LoginName />

<!-- Displays with custom format -->
<LoginName FormatString="Hello, {0}!" />

<!-- With styling -->
<LoginName FormatString="Logged in as: {0}"
           ForeColor="Navy"
           Font-Bold="true" />
```

### Format String

The `FormatString` property uses standard .NET string formatting. The `{0}` placeholder is replaced with the authenticated user's name from `Identity.Name`.

| FormatString | Example Output |
|--------------|----------------|
| `{0}` | `john.doe` |
| `Welcome, {0}!` | `Welcome, john.doe!` |
| `Logged in as: {0}` | `Logged in as: john.doe` |
| `User: {0} - ` | `User: john.doe - ` |

### Visibility Behavior

The LoginName component only renders content when the user is authenticated. If the user is not logged in, nothing is displayed.

### Integration with Authentication

Ensure your Blazor application has authentication properly configured:

```razor
<!-- In App.razor -->
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(App).Assembly">
        <!-- ... -->
    </Router>
</CascadingAuthenticationState>
```

### Example in Navigation

```razor
<nav>
    <AuthorizeView>
        <Authorized>
            <LoginName FormatString="Welcome, {0}" />
            <LoginStatus LogoutText="Sign Out" />
        </Authorized>
        <NotAuthorized>
            <a href="/login">Sign In</a>
        </NotAuthorized>
    </AuthorizeView>
</nav>
```

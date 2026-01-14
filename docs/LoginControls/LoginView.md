# LoginView

The LoginView component displays different content based on the user's authentication state and role membership. It allows you to show different UI to anonymous users, authenticated users, and users in specific roles. Original Web Forms documentation is at: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.loginview?view=netframework-4.8

## Blazor Features Supported

- AnonymousTemplate for unauthenticated users
- LoggedInTemplate for authenticated users
- RoleGroups for role-based content display
- Automatic template selection based on authentication state

## WebForms Features Not Supported

- All core features are supported

## WebForms Syntax

```html
<asp:LoginView
    EnableTheming="True|False"
    EnableViewState="True|False"
    ID="string"
    OnViewChanged="ViewChanged event handler"
    OnViewChanging="ViewChanging event handler"
    runat="server"
    SkinID="string"
    Visible="True|False">
    <AnonymousTemplate>
        <!-- Content for anonymous users -->
    </AnonymousTemplate>
    <LoggedInTemplate>
        <!-- Content for logged-in users -->
    </LoggedInTemplate>
    <RoleGroups>
        <asp:RoleGroup Roles="Admin">
            <ContentTemplate>
                <!-- Content for Admin role -->
            </ContentTemplate>
        </asp:RoleGroup>
    </RoleGroups>
</asp:LoginView>
```

## Blazor Syntax

```razor
<LoginView>
    <AnonymousTemplate>
        <p>Please <a href="/login">sign in</a> to continue.</p>
    </AnonymousTemplate>
    <LoggedInTemplate>
        <p>Welcome! You are logged in.</p>
    </LoggedInTemplate>
    <RoleGroups>
        <RoleGroup Roles="Admin,SuperUser">
            <p>You have administrator access.</p>
        </RoleGroup>
        <RoleGroup Roles="Manager">
            <p>You have manager access.</p>
        </RoleGroup>
    </RoleGroups>
</LoginView>
```

## Usage Notes

The LoginView component uses Blazor's `AuthenticationStateProvider` to determine the current user's authentication state and role membership.

### Template Selection Priority

The LoginView selects which template to display using this priority:

1. If user is **not authenticated**: Display `AnonymousTemplate`
2. If user is **authenticated** and matches a **RoleGroup**: Display that RoleGroup's content
3. If user is **authenticated** but matches **no RoleGroup**: Display `LoggedInTemplate`

### Basic Usage

```razor
<LoginView>
    <AnonymousTemplate>
        <div class="alert alert-info">
            You are not logged in. <a href="/login">Click here to sign in.</a>
        </div>
    </AnonymousTemplate>
    <LoggedInTemplate>
        <div class="alert alert-success">
            Welcome back! <LoginName FormatString="Hello, {0}!" />
        </div>
    </LoggedInTemplate>
</LoginView>
```

### Role-Based Content

```razor
<LoginView>
    <AnonymousTemplate>
        <p>Please log in to access this content.</p>
    </AnonymousTemplate>
    <LoggedInTemplate>
        <p>Welcome, standard user!</p>
    </LoggedInTemplate>
    <RoleGroups>
        <RoleGroup Roles="Admin">
            <h2>Administrator Dashboard</h2>
            <p>You have full system access.</p>
            <a href="/admin">Go to Admin Panel</a>
        </RoleGroup>
        <RoleGroup Roles="Editor,Contributor">
            <h2>Content Management</h2>
            <p>You can edit and create content.</p>
            <a href="/content">Manage Content</a>
        </RoleGroup>
    </RoleGroups>
</LoginView>
```

### RoleGroup Component

The `RoleGroup` component defines content for users in specific roles:

| Property | Description |
|----------|-------------|
| `Roles` | Comma-separated list of role names |
| `ChildContent` | The content to display for users in these roles |

```razor
<RoleGroup Roles="Admin,SuperAdmin">
    <!-- This content shows for Admin OR SuperAdmin users -->
    <AdminPanel />
</RoleGroup>
```

### Multiple Roles

When specifying multiple roles in a RoleGroup, a user only needs to be in **one** of the roles for the content to display:

```razor
<RoleGroup Roles="Manager,Director,VP">
    <!-- Shows if user is Manager OR Director OR VP -->
    <ManagementDashboard />
</RoleGroup>
```

### Integration with Blazor Authorization

LoginView works alongside Blazor's built-in `AuthorizeView` component. Use LoginView when you need Web Forms compatibility or role group functionality:

```razor
<!-- Blazor native approach -->
<AuthorizeView>
    <Authorized>Logged in content</Authorized>
    <NotAuthorized>Anonymous content</NotAuthorized>
</AuthorizeView>

<!-- Web Forms compatible approach with role groups -->
<LoginView>
    <AnonymousTemplate>Anonymous content</AnonymousTemplate>
    <LoggedInTemplate>Logged in content</LoggedInTemplate>
    <RoleGroups>
        <RoleGroup Roles="Admin">Admin content</RoleGroup>
    </RoleGroups>
</LoginView>
```

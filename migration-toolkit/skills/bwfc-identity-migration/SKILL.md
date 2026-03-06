---
name: bwfc-identity-migration
description: "Migrate ASP.NET Web Forms Identity and Membership authentication to Blazor Server Identity. Covers OWIN to ASP.NET Core auth middleware, login page migration, BWFC login controls, role-based authorization, and AuthorizeView patterns. Use when migrating authentication and authorization from a Web Forms application."
---

# Web Forms Identity → Blazor Identity Migration

This skill covers migrating ASP.NET Web Forms authentication (Identity, Membership, FormsAuthentication) to Blazor Server using ASP.NET Core Identity.

**Related skills:**
- `/bwfc-migration` — Core markup migration (controls, expressions, layouts)
- `/bwfc-data-migration` — EF6 → EF Core, data access, architecture decisions

---

## Overview

Web Forms authentication typically uses one of three systems. The migration path depends on which one:

| Web Forms Auth System | Era | Blazor Migration Path |
|----------------------|-----|----------------------|
| ASP.NET Identity (OWIN) | 2013+ | ASP.NET Core Identity (closest match) |
| ASP.NET Membership | 2005-2013 | ASP.NET Core Identity (schema migration required) |
| FormsAuthentication | 2002-2005 | ASP.NET Core Identity or cookie auth |

---

## Step 1: Add Identity Packages

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.UI
```

## Step 2: Configure Identity in Program.cs

```csharp
// Program.cs
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Required for Blazor Server
builder.Services.AddCascadingAuthenticationState();

// Middleware pipeline (ORDER MATTERS)
app.UseAuthentication();
app.UseAuthorization();
```

## Step 3: Create ApplicationUser and DbContext

```csharp
// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Add custom properties from your Web Forms ApplicationUser
}

// Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
}
```

## Step 4: Migrate the Database Schema

If migrating from ASP.NET Identity (OWIN), the schema is similar but not identical:

```bash
dotnet ef migrations add IdentityMigration
dotnet ef database update
```

**Key schema differences:**

| ASP.NET Identity (OWIN) | ASP.NET Core Identity |
|--------------------------|----------------------|
| `AspNetUsers.Id` (string GUID) | Same |
| `AspNetUsers.PasswordHash` | Same format — passwords are compatible |
| `AspNetUserClaims` | Same |
| `AspNetUserRoles` | Same |
| `AspNetRoles` | Same |
| `__MigrationHistory` | `__EFMigrationsHistory` |

> **Important:** ASP.NET Identity v2 password hashes (from Web Forms) are compatible with ASP.NET Core Identity. Users will NOT need to reset passwords.

If migrating from **Membership** (older):
- Use the `Microsoft.AspNetCore.Identity.MicrosoftAccountMigration` or a custom SQL migration script
- Membership password hashes are NOT compatible — users will need password resets

---

## Step 5: Migrate Login Pages

### BWFC Login Controls

BWFC provides login controls that match Web Forms markup. These controls use native Blazor `AuthenticationStateProvider` internally — they are **not shims** and do not need to be replaced with `AuthorizeView`.

> **Important:** BWFC login controls require `builder.Services.AddBlazorWebFormsComponents()` in `Program.cs` and `builder.Services.AddCascadingAuthenticationState()` for authentication state propagation.

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:Login runat="server" />` | `<Login />` | Login form with username/password |
| `<asp:LoginName runat="server" />` | `<LoginName />` | Displays authenticated username |
| `<asp:LoginStatus runat="server" />` | `<LoginStatus />` | Login/Logout toggle link |
| `<asp:LoginView runat="server">` | `<LoginView>` | Shows different content for anon vs auth users |
| `<asp:CreateUserWizard runat="server" />` | `<CreateUserWizard />` | Registration form |
| `<asp:ChangePassword runat="server" />` | `<ChangePassword />` | Password change form |
| `<asp:PasswordRecovery runat="server" />` | `<PasswordRecovery />` | Password reset flow |

### Login Page Migration Example

```html
<!-- Web Forms — Login.aspx -->
<%@ Page Title="Log in" MasterPageFile="~/Site.Master" ... %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2>Log in</h2>
    <asp:Login ID="LoginCtrl" runat="server"
        ViewStateMode="Disabled"
        RenderOuterTable="false">
        <LayoutTemplate>
            <asp:TextBox ID="UserName" runat="server" CssClass="form-control" />
            <asp:RequiredFieldValidator ControlToValidate="UserName"
                ErrorMessage="Required" runat="server" />
            <asp:TextBox ID="Password" TextMode="Password" runat="server" />
            <asp:Button Text="Log in" CommandName="Login" runat="server" />
        </LayoutTemplate>
    </asp:Login>
</asp:Content>
```

```razor
@* Blazor — Login.razor *@
@page "/Account/Login"

<h2>Log in</h2>
<Login RenderOuterTable="false">
    <LayoutTemplate>
        <TextBox @bind-Text="model.UserName" CssClass="form-control" />
        <RequiredFieldValidator ControlToValidate="UserName" ErrorMessage="Required" />
        <TextBox TextMode="Password" @bind-Text="model.Password" />
        <Button Text="Log in" CommandName="Login" />
    </LayoutTemplate>
</Login>
```

---

## Step 6: Migrate Authorization Patterns

### Web.config Authorization → Blazor Authorization

```xml
<!-- Web Forms — Web.config -->
<location path="Admin">
    <system.web>
        <authorization>
            <allow roles="Administrator" />
            <deny users="*" />
        </authorization>
    </system.web>
</location>
```

```razor
@* Blazor — Admin pages use [Authorize] attribute *@
@page "/Admin"
@attribute [Authorize(Roles = "Administrator")]

<h1>Admin Panel</h1>
```

### LoginView Conditional Content

```html
<!-- Web Forms -->
<asp:LoginView runat="server">
    <AnonymousTemplate>
        <a href="~/Account/Login">Log in</a>
    </AnonymousTemplate>
    <LoggedInTemplate>
        Welcome, <asp:LoginName runat="server" />!
        <asp:LoginStatus runat="server" LogoutAction="Redirect" LogoutPageUrl="~/" />
    </LoggedInTemplate>
</asp:LoginView>
```

```razor
@* Blazor — BWFC LoginView (recommended — preserves markup and uses AuthenticationStateProvider natively) *@
<LoginView>
    <AnonymousTemplate>
        <a href="/Account/Login">Log in</a>
    </AnonymousTemplate>
    <LoggedInTemplate>
        Welcome, <LoginName />!
        <LoginStatus LogoutAction="Redirect" LogoutPageUrl="/" />
    </LoggedInTemplate>
</LoginView>
```

> **Note:** BWFC's `LoginView` internally injects `AuthenticationStateProvider` and evaluates authentication state natively — it is NOT a shim that needs to be replaced. Keep it for markup compatibility. If you prefer native Blazor syntax long-term:

```razor
@* Blazor — Native AuthorizeView (alternative — different template names) *@
<AuthorizeView>
    <NotAuthorized>
        <a href="/Account/Login">Log in</a>
    </NotAuthorized>
    <Authorized>
        Welcome, @context.User.Identity?.Name!
        <a href="/Account/Logout">Log out</a>
    </Authorized>
</AuthorizeView>
```

### Role-Based Content

```html
<!-- Web Forms -->
<asp:LoginView runat="server">
    <RoleGroups>
        <asp:RoleGroup Roles="Administrator">
            <ContentTemplate><a href="~/Admin">Admin Panel</a></ContentTemplate>
        </asp:RoleGroup>
    </RoleGroups>
</asp:LoginView>
```

```razor
@* Blazor *@
<AuthorizeView Roles="Administrator">
    <a href="/Admin">Admin Panel</a>
</AuthorizeView>
```

---

## Step 7: Migrate Authentication State Access

### Code-Behind Auth Patterns

```csharp
// Web Forms — code-behind
if (HttpContext.Current.User.Identity.IsAuthenticated)
{
    var userName = HttpContext.Current.User.Identity.Name;
    var isAdmin = HttpContext.Current.User.IsInRole("Administrator");
}

// Web Forms — OWIN
var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
var user = manager.FindById(User.Identity.GetUserId());
```

```csharp
// Blazor — inject auth state
@inject AuthenticationStateProvider AuthStateProvider

@code {
    private string? userName;
    private bool isAdmin;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        userName = user.Identity?.Name;
        isAdmin = user.IsInRole("Administrator");
    }
}
```

### Cascading Auth Parameter (Simpler)

```csharp
// Blazor — cascading parameter (requires AddCascadingAuthenticationState)
[CascadingParameter]
private Task<AuthenticationState>? AuthState { get; set; }

protected override async Task OnInitializedAsync()
{
    if (AuthState != null)
    {
        var state = await AuthState;
        var user = state.User;
    }
}
```

---

## OWIN Middleware → ASP.NET Core Middleware

| Web Forms (OWIN Startup.cs) | Blazor (Program.cs) |
|------------------------------|---------------------|
| `app.UseCookieAuthentication(...)` | `builder.Services.AddAuthentication().AddCookie(...)` |
| `app.UseExternalSignInCookie(...)` | `builder.Services.AddAuthentication().AddGoogle/Facebook(...)` |
| `ConfigureAuth(app)` in `Startup.Auth.cs` | Configuration in `Program.cs` services section |
| `app.CreatePerOwinContext<ApplicationUserManager>(...)` | `builder.Services.AddIdentity<ApplicationUser, IdentityRole>()` |
| `SecurityStampValidator.OnValidateIdentity(...)` | Built into ASP.NET Core Identity automatically |

---

## Common Identity Gotchas

### No HttpContext.Current
Blazor Server has no `HttpContext.Current`. Use dependency injection:
```csharp
// WRONG: HttpContext.Current.User
// RIGHT: Inject AuthenticationStateProvider or use [CascadingParameter]
```

### Cookie Auth Requires HTTP Endpoints
Blazor Server login/logout MUST use HTTP endpoints (not component-based), because cookies are set on HTTP responses:
```csharp
// Program.cs — Add login/logout endpoints
app.MapPost("/Account/Logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
});
```

### SignalR Circuit vs HTTP Request
Authentication state is captured when the circuit starts. If the user's session expires mid-circuit, they remain "authenticated" until the page refreshes. Use `RevalidatingServerAuthenticationStateProvider` for periodic revalidation.

### Blazor Identity UI Scaffolding
For a complete Identity UI (login, register, manage profile), scaffold it:
```bash
dotnet aspnet-codegenerator identity -dc ApplicationDbContext --files "Account.Login;Account.Register;Account.Logout"
```
This generates Razor Pages (not components) under `/Areas/Identity/`. They coexist with Blazor.

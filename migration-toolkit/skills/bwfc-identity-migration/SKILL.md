---
name: bwfc-identity-migration
description: "Migrate ASP.NET Web Forms Identity and Membership authentication to Blazor Server Identity. Covers OWIN to ASP.NET Core auth middleware, login page migration, BWFC login controls, role-based authorization, and AuthorizeView patterns. Use when migrating authentication and authorization from a Web Forms application."
---

# Web Forms Identity → Blazor Identity Migration

> ## 🚫 MANDATORY: USE BWFC LOGIN CONTROLS FIRST
>
> BWFC provides **LoginView, LoginStatus, LoginName, Login, CreateUserWizard, ChangePassword, PasswordRecovery, and RoleGroup** components.
> Use these BWFC components for initial migration. Do NOT skip them and jump straight to `<AuthorizeView>` or raw HTML.
> `<AuthorizeView>` is an **optional long-term upgrade**, not the first step.

This skill covers migrating ASP.NET Web Forms authentication (Identity, Membership, FormsAuthentication) to Blazor Server using ASP.NET Core Identity.

**Related skills:**
- `/bwfc-migration` — Core markup migration (controls, expressions, layouts) — **read this first**
- `/bwfc-data-migration` — EF6 → EF Core, data access, architecture decisions

---

## 1. BWFC Login Controls — USE THESE FIRST

BWFC provides drop-in replacements for ALL Web Forms login controls. **Use these during migration for maximum markup fidelity.** Converting to native Blazor `<AuthorizeView>` is an optional step AFTER the migration is working.

### Complete Login Control Mapping

| Web Forms | BWFC Component | Purpose |
|-----------|----------------|---------|
| `<asp:Login runat="server" />` | `<Login />` | Login form with username/password |
| `<asp:LoginName runat="server" />` | `<LoginName />` | Displays authenticated username |
| `<asp:LoginStatus runat="server" />` | **`<LoginStatus />`** | Login/Logout toggle link — **commonly missed!** |
| `<asp:LoginView runat="server">` | **`<LoginView>`** | Shows different content for anon vs auth — **commonly missed!** |
| `<asp:CreateUserWizard runat="server" />` | `<CreateUserWizard />` | Registration form |
| `<asp:ChangePassword runat="server" />` | `<ChangePassword />` | Password change form |
| `<asp:PasswordRecovery runat="server" />` | `<PasswordRecovery />` | Password reset flow |
| `<asp:RoleGroup Roles="Admin">` | `<RoleGroup Roles="Admin">` | Role-specific content within LoginView |

### ⚠️ Commonly Missed: LoginView and LoginStatus

These two controls are the **most frequently broken** during migration. Agents consistently replace them with raw `@if` blocks or `<a>` tags, destroying BWFC functionality:

```razor
@* ❌ WRONG — LoginView replaced with raw @if block *@
@if (isAuthenticated)
{
    <span>Welcome, @userName!</span>
    <a href="/Account/Logout">Log out</a>
}
else
{
    <a href="/Account/Login">Log in</a>
}

@* ✅ CORRECT — BWFC LoginView preserved with all sub-controls *@
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

### Full LoginView Migration Example

```html
<!-- Web Forms — Site.Master header -->
<asp:LoginView runat="server">
    <AnonymousTemplate>
        <a href="~/Account/Login">Log in</a>
    </AnonymousTemplate>
    <LoggedInTemplate>
        Welcome, <asp:LoginName runat="server" />!
        <asp:LoginStatus runat="server" LogoutAction="Redirect" LogoutPageUrl="~/" />
    </LoggedInTemplate>
    <RoleGroups>
        <asp:RoleGroup Roles="Administrator">
            <ContentTemplate><a href="~/Admin">Admin Panel</a></ContentTemplate>
        </asp:RoleGroup>
    </RoleGroups>
</asp:LoginView>
```

```razor
@* Blazor — BWFC controls preserved (initial migration) *@
<LoginView>
    <AnonymousTemplate>
        <a href="/Account/Login">Log in</a>
    </AnonymousTemplate>
    <LoggedInTemplate>
        Welcome, <LoginName />!
        <LoginStatus LogoutAction="Redirect" LogoutPageUrl="/" />
    </LoggedInTemplate>
    <RoleGroups>
        <RoleGroup Roles="Administrator">
            <ContentTemplate><a href="/Admin">Admin Panel</a></ContentTemplate>
        </RoleGroup>
    </RoleGroups>
</LoginView>
```

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
@* Blazor — ALL controls preserved as BWFC *@
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

### Optional Long-Term Upgrade: BWFC → Native AuthorizeView

After the BWFC migration is working and verified, you MAY optionally convert `LoginView` → `AuthorizeView` for native Blazor auth:

```razor
@* Optional upgrade — native AuthorizeView (recommended long-term) *@
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

> **This is an optimization step, not a migration step.** Do BWFC first, verify it works, then upgrade if desired.

---

## 2. Auth System Overview

Web Forms authentication typically uses one of three systems. The migration path depends on which one:

| Web Forms Auth System | Era | Blazor Migration Path |
|----------------------|-----|----------------------|
| ASP.NET Identity (OWIN) | 2013+ | ASP.NET Core Identity (closest match) |
| ASP.NET Membership | 2005-2013 | ASP.NET Core Identity (schema migration required) |
| FormsAuthentication | 2002-2005 | ASP.NET Core Identity or cookie auth |

## 3. Add Identity Packages

```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.UI
```

## 4. Configure Identity in Program.cs

!!! warning "AuthorizeView Without Full Identity"
    Even WITHOUT full Identity setup, if you convert `LoginView` → `AuthorizeView`, you **MUST** add:
    ```csharp
    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddAuthorization();
    ```
    Without these, ANY page containing `<AuthorizeView>` crashes at runtime with:
    `InvalidOperationException: Authorization requires a cascading parameter of type Task<AuthenticationState>`

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

## 5. Create ApplicationUser and DbContext

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

## 6. Migrate the Database Schema

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
- Use a custom SQL migration script
- Membership password hashes are NOT compatible — users will need password resets

---

## 7. Migrate Authentication State Access

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

### AuthorizeView Without Identity Setup

!!! danger "CRASH-LEVEL: Runtime exception on any page with AuthorizeView"
    If `<asp:LoginView>` was converted to `<AuthorizeView>` (either by the migration script or manually), the app will crash immediately with `InvalidOperationException` unless auth services are registered.

Even if you are NOT implementing full ASP.NET Core Identity (e.g., you just want the app to compile and render), you must add these two lines to `Program.cs`:

```csharp
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();
```

This is the minimum required to prevent the crash. Full Identity setup (Step 1–4 above) provides actual authentication functionality. These two lines only provide the cascading parameter infrastructure that `<AuthorizeView>` demands.

### Blazor Identity UI Scaffolding
For a complete Identity UI (login, register, manage profile), scaffold it:
```bash
dotnet aspnet-codegenerator identity -dc ApplicationDbContext --files "Account.Login;Account.Register;Account.Logout"
```
This generates Razor Pages (not components) under `/Areas/Identity/`. They coexist with Blazor.

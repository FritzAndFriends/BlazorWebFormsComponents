# Identity Migration Patterns & Reference

## Cookie Auth Under Interactive Server Mode

> **CRITICAL:** When using `<Routes @rendermode="InteractiveServer" />`, `HttpContext` is **NULL** during WebSocket circuits. Cookie-based auth operations — login, register, logout — **cannot** be performed via Blazor component event handlers. They will silently fail.

**Required pattern:** Use standard HTML `<form method="post">` that submits to **minimal API endpoints** via full HTTP POST requests.

```razor
@* Login.razor — form posts to a minimal API endpoint *@
<form method="post" action="/Account/LoginHandler">
    <div>
        <label>Email</label>
        <input type="email" name="email" required />
    </div>
    <div>
        <label>Password</label>
        <input type="password" name="password" required />
    </div>
    <button type="submit">Log in</button>
</form>
```

```csharp
// Program.cs — minimal API endpoint handles SignInAsync()
app.MapPost("/Account/LoginHandler", async (HttpContext context, SignInManager<IdentityUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();
    var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
    return result.Succeeded
        ? Results.Redirect("/")
        : Results.Redirect("/Account/Login?error=Invalid+login+attempt");
}).DisableAntiforgery();
```

**Key points:**
- `<form>` submits HTTP POST — full page navigation, not a Blazor event
- Minimal API endpoint has real `HttpContext` → cookies work
- Applies to ALL cookie-writing operations: login, register, logout, external auth
- **NOT optional** — without it, auth silently fails

---

## Identity Configuration Steps

### Step 1: Add Identity Packages
```bash
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.UI
```

### Step 2: Configure Identity in Program.cs
```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddCascadingAuthenticationState();

app.UseAuthentication();
app.UseAuthorization();
```

### Step 3: Create ApplicationUser and DbContext
```csharp
public class ApplicationUser : IdentityUser { }

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}
```

### Step 4: Migrate Database
```bash
dotnet ef migrations add IdentityMigration
dotnet ef database update
```

ASP.NET Identity v2 password hashes (from Web Forms) are compatible with ASP.NET Core Identity. Users do NOT need to reset passwords.

---

## BWFC Login Controls

| Web Forms | BWFC | Notes |
|-----------|------|-------|
| `<asp:Login />` | `<Login />` | Login form |
| `<asp:LoginName />` | `<LoginName />` | Displays username |
| `<asp:LoginStatus />` | `<LoginStatus />` | Login/Logout toggle |
| `<asp:LoginView>` | `<LoginView>` | Conditional auth content |
| `<asp:CreateUserWizard />` | `<CreateUserWizard />` | Registration form |
| `<asp:ChangePassword />` | `<ChangePassword />` | Password change |
| `<asp:PasswordRecovery />` | `<PasswordRecovery />` | Password reset |

> **NEVER replace LoginView with AuthorizeView.** BWFC `LoginView` is a fully functional component using `AuthenticationStateProvider` natively. It uses the SAME template names (`AnonymousTemplate`, `LoggedInTemplate`).

---

## Authorization Patterns

### Web.config Authorization → Blazor
```xml
<!-- Web Forms -->
<location path="Admin">
    <authorization><allow roles="Administrator" /><deny users="*" /></authorization>
</location>
```
```razor
@* Blazor *@
@attribute [Authorize(Roles = "Administrator")]
```

### LoginView Conditional Content
```razor
<LoginView>
    <AnonymousTemplate><a href="/Account/Login">Log in</a></AnonymousTemplate>
    <LoggedInTemplate>Welcome, <LoginName />! <LoginStatus LogoutAction="Redirect" LogoutPageUrl="/" /></LoggedInTemplate>
</LoginView>
```

### Role-Based Content
```razor
<AuthorizeView Roles="Administrator">
    <a href="/Admin">Admin Panel</a>
</AuthorizeView>
```

---

## Authentication State Access

### Cascading Auth Parameter (Preferred)
```csharp
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

### Session-Based Auth Data
`WebFormsPageBase.Session` works for auth-related session data:
```csharp
Session["userCheckoutCompleted"] = true;  // Same pattern in Web Forms and Blazor
```

---

## Ready-to-Use Endpoint Templates

> Every endpoint calls `.DisableAntiforgery()` because Blazor `<form>` elements don't include antiforgery tokens.

### Login Endpoint
```csharp
app.MapPost("/Account/LoginHandler", async (HttpContext context, SignInManager<IdentityUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var result = await signInManager.PasswordSignInAsync(form["email"]!, form["password"]!, false, false);
    return result.Succeeded ? Results.Redirect("/") : Results.Redirect("/Account/Login?error=Invalid+login");
}).DisableAntiforgery();
```

### Register Endpoint
```csharp
app.MapPost("/Account/RegisterHandler", async (HttpContext context,
    UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var user = new IdentityUser { UserName = form["email"]!, Email = form["email"]! };
    var result = await userManager.CreateAsync(user, form["password"]!);
    if (result.Succeeded) { await signInManager.SignInAsync(user, false); return Results.Redirect("/"); }
    return Results.Redirect($"/Account/Register?error={Uri.EscapeDataString(string.Join("; ", result.Errors.Select(e => e.Description)))}");
}).DisableAntiforgery();
```

### Logout Endpoint
```csharp
app.MapPost("/Account/Logout", async (SignInManager<IdentityUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
}).DisableAntiforgery();
```

---

## OWIN Middleware → ASP.NET Core

| Web Forms (OWIN) | Blazor (Program.cs) |
|-------------------|---------------------|
| `app.UseCookieAuthentication(...)` | `builder.Services.AddAuthentication().AddCookie(...)` |
| `app.UseExternalSignInCookie(...)` | `.AddGoogle()` / `.AddFacebook()` |
| `app.CreatePerOwinContext<...>(...)` | `builder.Services.AddIdentity<...>()` |
| `SecurityStampValidator.OnValidateIdentity(...)` | Built into ASP.NET Core Identity automatically |

---

## Common Identity Gotchas

- **No HttpContext.Current** — Use DI: inject `AuthenticationStateProvider` or `[CascadingParameter]`
- **Cookie auth requires HTTP endpoints** — login/logout MUST use minimal API, not component events
- **SignalR circuit vs HTTP request** — Auth state captured at circuit start; use `RevalidatingServerAuthenticationStateProvider` for periodic revalidation
- **DisableAntiforgery required** — Blazor forms don't include antiforgery tokens; endpoints must call `.DisableAntiforgery()`
- **Logout must be `<form method="post">`** — Don't use `<a href>` — Blazor enhanced navigation intercepts it

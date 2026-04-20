# Identity & Authentication Migration Skill

The **Identity & Authentication Migration Skill** guides the migration of ASP.NET Web Forms authentication and authorization systems to Blazor Server using ASP.NET Core Identity.

---

## When to Use This Skill

Use this skill when migrating:

- Login pages (`Login.aspx`)
- Registration pages (`Register.aspx`)
- Account management pages (password reset, profile)
- ASP.NET Identity (OWIN-based)
- ASP.NET Membership (pre-2013)
- FormsAuthentication patterns
- Role-based authorization (`[Authorize(Roles = "Admin")]`)
- BWFC login controls (Login, LoginView, ChangePassword, CreateUserWizard)

---

## Web Forms Authentication Systems

Web Forms applications typically use one of three authentication systems:

| System | Era | Database Schema | Password Hash Compatibility |
|--------|-----|-----------------|---------------------------|
| **ASP.NET Identity (OWIN)** | 2013+ | AspNetUsers, AspNetRoles |  Compatible with ASP.NET Core Identity |
| **ASP.NET Membership** | 2005-2013 | aspnet_Users, aspnet_Membership |  Requires migration script |
| **FormsAuthentication** | 2002-2005 | Custom tables |  Requires custom migration |

---

## Critical: Cookie Auth Under Interactive Server Mode

** IMPORTANT:** When using global interactive server mode (`<Routes @rendermode="InteractiveServer" />`), `HttpContext` is **NULL** during WebSocket circuits. This means:

- `SignInAsync()` **does not work** in component event handlers
- `SignOutAsync()` **does not work** in component event handlers
- Cookies **cannot be set** from `@onclick` handlers

### Required Pattern: Minimal API Endpoints

Cookie-based authentication operations (login, register, logout) **must** use HTML `<form method="post">` that submits to **minimal API endpoints**:

```razor
@* Login.razor  form posts to endpoint, NOT a Blazor event handler *@
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
// Program.cs  minimal API endpoint performs SignInAsync
app.MapPost("/Account/LoginHandler", async (HttpContext context, SignInManager<IdentityUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();
    
    var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
    
    return result.Succeeded
        ? Results.Redirect("/")
        : Results.Redirect("/Account/Login?error=Invalid+login+attempt");
}).DisableAntiforgery();  // Required  Blazor forms don't include antiforgery tokens
```

**Why `.DisableAntiforgery()` is required:**

Blazor's HTML rendering does not automatically include `<input type="hidden" name="__RequestVerificationToken" />` in `<form>` elements. Without disabling antiforgery validation, the POST will fail with a 400 Bad Request error.

---

## Migration Paths

### Path 1: ASP.NET Identity (OWIN)  ASP.NET Core Identity

**Best fit when:**
- Original app uses `Microsoft.AspNet.Identity.EntityFramework`
- `Web.config` has `<add key="owin:appStartup" value="Startup" />`
- Database has `AspNetUsers`, `AspNetRoles`, `AspNetUserClaims` tables

**Steps:**

1. **Install packages:**
   ```bash
   dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
   dotnet add package Microsoft.AspNetCore.Identity.UI
   ```

2. **Create ApplicationUser and DbContext:**
   ```csharp
   public class ApplicationUser : IdentityUser
   {
       // Add custom properties from your Web Forms ApplicationUser
   }
   
   public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
   {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options) { }
   }
   ```

3. **Configure Identity in Program.cs:**
   ```csharp
   builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
   {
       options.SignIn.RequireConfirmedAccount = false;
       options.Password.RequiredLength = 6;
   })
       .AddEntityFrameworkStores<ApplicationDbContext>()
       .AddDefaultTokenProviders();
   
   builder.Services.AddCascadingAuthenticationState();
   
   // Middleware pipeline (ORDER MATTERS)
   app.UseAuthentication();
   app.UseAuthorization();
   ```

4. **Migrate database schema:**
   ```bash
   dotnet ef migrations add IdentityMigration
   dotnet ef database update
   ```

**Password hash compatibility:**  ASP.NET Identity v2 (Web Forms) password hashes are compatible with ASP.NET Core Identity. Users can log in with existing passwords.

### Path 2: ASP.NET Membership  ASP.NET Core Identity

**Best fit when:**
- Original app uses `System.Web.Security.Membership`
- Database has `aspnet_Users`, `aspnet_Membership`, `aspnet_Roles` tables
- No OWIN middleware

**Steps:**

1. Install packages (same as Path 1)

2. **Migrate schema using SQL script:**
   ```sql
   -- Example migration from Membership to ASP.NET Core Identity
   -- (Use Microsoft's migration tool or custom script)
   INSERT INTO AspNetUsers (Id, UserName, Email, EmailConfirmed, PasswordHash, SecurityStamp)
   SELECT 
       CAST(UserId AS NVARCHAR(450)),
       LoweredUserName,
       LoweredEmail,
       1,
       Password,  -- Hash format is NOT compatible
       NEWID()
   FROM aspnet_Membership m
   JOIN aspnet_Users u ON m.UserId = u.UserId;
   ```

3. **Force password resets:**
   Because Membership password hashes are **not compatible**, users must reset passwords on first login.

### Path 3: FormsAuthentication  Cookie Authentication

**Best fit when:**
- Original app uses `FormsAuthentication.SetAuthCookie()`
- Custom user tables (not AspNet* schema)
- Lightweight auth needs

**Steps:**

1. **Configure cookie authentication:**
   ```csharp
   builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(options =>
       {
           options.LoginPath = "/Account/Login";
           options.LogoutPath = "/Account/Logout";
       });
   ```

2. **Create login minimal API:**
   ```csharp
   app.MapPost("/Account/LoginHandler", async (HttpContext context, IUserService userService) =>
   {
       var form = await context.Request.ReadFormAsync();
       var username = form["username"].ToString();
       var password = form["password"].ToString();
       
       var user = await userService.ValidateCredentialsAsync(username, password);
       if (user == null)
           return Results.Redirect("/Account/Login?error=Invalid+credentials");
       
       var claims = new List<Claim>
       {
           new Claim(ClaimTypes.Name, user.Username),
           new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
       };
       
       var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
       var principal = new ClaimsPrincipal(identity);
       
       await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
       
       return Results.Redirect("/");
   }).DisableAntiforgery();
   ```

---

## BWFC Login Controls

BWFC provides Blazor equivalents of Web Forms login controls:

| Web Forms Control | BWFC Component | Status |
|-------------------|----------------|--------|
| `<asp:Login>` | `<Login>` |  Supported |
| `<asp:LoginView>` | `<LoginView>` |  Supported |
| `<asp:LoginStatus>` | `<LoginStatus>` |  Supported |
| `<asp:LoginName>` | `<LoginName>` |  Supported |
| `<asp:ChangePassword>` | `<ChangePassword>` |  Supported |
| `<asp:CreateUserWizard>` | `<CreateUserWizard>` |  Supported |
| `<asp:PasswordRecovery>` | `<PasswordRecovery>` |  Supported |

**Example Migration:**

```html
<!-- Web Forms -->
<asp:LoginView runat="server">
    <AnonymousTemplate>
        <asp:Login ID="LoginControl" runat="server" />
    </AnonymousTemplate>
    <LoggedInTemplate>
        Welcome, <asp:LoginName runat="server" />!
        <asp:LoginStatus runat="server" LogoutAction="Redirect" LogoutPageUrl="~/" />
    </LoggedInTemplate>
</asp:LoginView>
```

```razor
<!-- Blazor with BWFC -->
<LoginView>
    <NotAuthorized>
        <Login />
    </NotAuthorized>
    <Authorized>
        Welcome, <LoginName />!
        <LoginStatus LogoutAction="Redirect" LogoutPageUrl="/" />
    </Authorized>
</LoginView>
```

---

## Role-Based Authorization

**Web Forms:**
```csharp
<%@ Page ... %>
<script runat="server">
protected void Page_Load(object sender, EventArgs e)
{
    if (!User.IsInRole("Admin"))
    {
        Response.Redirect("~/AccessDenied.aspx");
    }
}
</script>
```

**Blazor with BWFC:**
```razor
@page "/Admin"
@attribute [Authorize(Roles = "Admin")]

<h1>Admin Dashboard</h1>
```

Or use `<AuthorizeView>`:

```razor
<AuthorizeView Roles="Admin">
    <Authorized>
        <h1>Admin Dashboard</h1>
    </Authorized>
    <NotAuthorized>
        <p>Access denied.</p>
    </NotAuthorized>
</AuthorizeView>
```

---

## Common Migration Scenarios

### Scenario 1: Simple Login Page

**Before (Web Forms):**
```html
<asp:Login ID="LoginControl" 
           OnAuthenticate="LoginControl_Authenticate" 
           DestinationPageUrl="~/"
           runat="server" />
```

```csharp
protected void LoginControl_Authenticate(object sender, AuthenticateEventArgs e)
{
    var username = LoginControl.UserName;
    var password = LoginControl.Password;
    e.Authenticated = Membership.ValidateUser(username, password);
}
```

**After (Blazor with minimal API):**
```razor
@page "/Account/Login"

<form method="post" action="/Account/LoginHandler">
    <div>
        <label>Username</label>
        <input type="text" name="username" required />
    </div>
    <div>
        <label>Password</label>
        <input type="password" name="password" required />
    </div>
    <button type="submit">Log in</button>
</form>
```

```csharp
// Program.cs
app.MapPost("/Account/LoginHandler", async (HttpContext context, SignInManager<ApplicationUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();
    
    var result = await signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);
    
    return result.Succeeded
        ? Results.Redirect("/")
        : Results.Redirect("/Account/Login?error=1");
}).DisableAntiforgery();
```

### Scenario 2: Registration Page

**Before (Web Forms):**
```html
<asp:CreateUserWizard ID="CreateUserWizard1" runat="server" OnCreatedUser="CreateUserWizard1_CreatedUser">
    <WizardSteps>
        <asp:CreateUserWizardStep runat="server" />
        <asp:CompleteWizardStep runat="server" />
    </WizardSteps>
</asp:CreateUserWizard>
```

**After (Blazor with minimal API):**
```razor
@page "/Account/Register"

<form method="post" action="/Account/RegisterHandler">
    <div>
        <label>Email</label>
        <input type="email" name="email" required />
    </div>
    <div>
        <label>Password</label>
        <input type="password" name="password" required />
    </div>
    <div>
        <label>Confirm Password</label>
        <input type="password" name="confirmPassword" required />
    </div>
    <button type="submit">Register</button>
</form>
```

```csharp
// Program.cs
app.MapPost("/Account/RegisterHandler", async (HttpContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();
    var email = form["email"].ToString();
    var password = form["password"].ToString();
    var confirmPassword = form["confirmPassword"].ToString();
    
    if (password != confirmPassword)
        return Results.Redirect("/Account/Register?error=passwords");
    
    var user = new ApplicationUser { UserName = email, Email = email };
    var result = await userManager.CreateAsync(user, password);
    
    if (result.Succeeded)
    {
        await signInManager.SignInAsync(user, isPersistent: false);
        return Results.Redirect("/");
    }
    
    return Results.Redirect("/Account/Register?error=failed");
}).DisableAntiforgery();
```

---

## How to Use This Skill

### In Copilot Chat

```
@workspace Use the identity migration skill to convert the Login.aspx page 
to Blazor. The original uses ASP.NET Identity with OWIN.
```

### Pattern-Specific Questions

```
Using the BWFC identity migration patterns, how should I handle cookie 
authentication in Interactive Server mode? The original uses FormsAuthentication.
```

---

## Related Skills

- **[Core Migration](CoreMigration.md)**  For markup and code-behind transforms
- **[Data & Architecture Migration](DataMigration.md)**  For database and architecture decisions

---

## Skill File Location

The full skill file is located at:
```
migration-toolkit/skills/bwfc-identity-migration/SKILL.md
```

---

## Related Documentation

- [Quick Start Guide](../QuickStart.md)
- [Three-Layer Methodology](../Methodology.md)
- [Login Controls Documentation](../../LoginControls/)

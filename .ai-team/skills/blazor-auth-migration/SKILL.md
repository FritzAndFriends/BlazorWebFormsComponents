---
name: "blazor-auth-migration"
description: "Cookie-based authentication pattern for Blazor Server migration — scoped providers, not singletons"
domain: "migration"
confidence: "medium"
source: "earned"
---

## Context

Discovered in WingtipToys Run 7, Iteration 2. When migrating ASP.NET Web Forms applications with Identity/OWIN auth to Blazor Server, the `AuthenticationStateProvider` registration pattern is critical. A singleton provider shares auth state across all browser sessions — causing test interference and real-world session bleed.

Confirmed by: Run 7 acceptance tests (14/14 pass after fix). The `RegisterAndLogin_EndToEnd` test failed reliably with singleton registration and passed reliably with scoped + cookie auth.

---

## ⚠️ Core Rule

**NEVER register `AuthenticationStateProvider` as a singleton in Blazor Server.** Blazor Server maintains per-circuit state; singletons break this model by sharing state across all connections.

---

## Patterns

### Correct: Scoped Provider + ASP.NET Core Cookie Auth

```csharp
// Program.cs — correct auth registration for Blazor Server migration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/Account/Login");
builder.Services.AddScoped<MockAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<MockAuthenticationStateProvider>());
```

**Why this works:**
- `AddAuthentication` + `AddCookie` gives ASP.NET Core's cookie middleware — session isolation is built-in.
- `AddScoped<MockAuthenticationStateProvider>` creates one provider per Blazor circuit.
- The factory registration (`AddScoped<AuthenticationStateProvider>(sp => ...)`) lets DI resolve the concrete type through the abstract base.

### When to Use This Pattern

- **Mock auth during migration:** When you need login/logout behavior without porting the full Identity database. Replace `MockAuthenticationStateProvider` with real Identity later.
- **Cookie auth for form POST endpoints:** Auth forms that POST to minimal API endpoints (register, login) need HTTP cookie handling — this pattern provides it.
- **Testing:** Scoped providers ensure Playwright/test browser instances don't interfere.

---

## Anti-Patterns

### ❌ Singleton AuthenticationStateProvider

```csharp
// WRONG — auth state shared across ALL browser sessions
builder.Services.AddSingleton<AuthenticationStateProvider, MockAuthenticationStateProvider>();
```

**Symptoms:**
- User A logs in, User B sees authenticated state
- Playwright tests interfere with each other (one test's login affects another)
- Logout in one tab affects all tabs/sessions

### ❌ Transient AuthenticationStateProvider

```csharp
// WRONG — creates a new instance per resolution, loses state within a circuit
builder.Services.AddTransient<AuthenticationStateProvider, MockAuthenticationStateProvider>();
```

**Symptoms:**
- Auth state appears to reset randomly within a session
- Components disagree about whether user is authenticated

---

## Examples

### Minimal API Endpoints for Auth Forms

When using cookie auth, register and login forms POST to minimal API endpoints:

```csharp
// Program.cs — minimal API auth endpoints
app.MapPost("/Account/DoRegister", async (HttpContext ctx,
    MockAuthenticationStateProvider auth,
    [FromForm] string email, [FromForm] string password) =>
{
    // Register user logic here
    await auth.LoginAsync(email);
    await ctx.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, email) },
            CookieAuthenticationDefaults.AuthenticationScheme)));
    return Results.Redirect("/");
});
```

### Integration with BWFC LoginView

The BWFC `LoginView` component wraps Blazor's `<AuthorizeView>` internally. It works correctly with scoped auth providers — no special configuration needed beyond the registration pattern above.

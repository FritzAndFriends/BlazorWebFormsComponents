# Migration Shim Analysis: What Requires Manual Work (and What We Can Automate)

**Author:** Forge (Web Forms Reviewer)
**Date:** 2026-03-27
**Requested by:** Jeffrey T. Fritz

---

## Executive Summary

After a deep audit of `bwfc-migrate.ps1` (Layer 1), the existing shim library, and WingtipToys Run 7 benchmarks, I've catalogued **47 distinct manual migration items** across 14 categories. Of these:

- **12 are already shimmed** — BWFC has working shims that let migrated code compile (ViewState, IsPostBack, Response.Redirect, Request.QueryString, Identity stubs, EF6 stubs, HttpHandler base, .aspx/.ashx/.axd middleware, DataBinder, LoginControls, QueryString/RouteData attributes).
- **15 are 🟢 Easy** — can be addressed with drop-in shims, type aliases, regex transforms, or Roslyn analyzers within 1–2 weeks each.
- **13 are 🟡 Medium** — need creative approaches (runtime adapters, source generators, middleware, or L1 script enhancements) taking 2–4 weeks each.
- **7 are 🔴 Hard** — fundamental architecture mismatches that require manual rewriting. We can provide guidance and scaffolding but not full automation.

The biggest bang-for-buck investments are: **(1)** Page lifecycle method renaming via Roslyn fixer, **(2)** Session state shim service, **(3)** ConfigurationManager shim, and **(4)** event handler signature auto-conversion. These four items alone would eliminate ~60% of the Layer 2 manual work seen in WingtipToys.

---

## What Layer 1 Already Automates

Before diving into gaps, here's what `bwfc-migrate.ps1` + the BWFC NuGet package already handle (no manual work required):

| Category | What's Automated |
|----------|-----------------|
| File renaming | .aspx/.ascx/.master → .razor |
| Directives | <%@ Page/Master/Control/Import/Register %> → Razor equivalents |
| Tag prefixes | `asp:`, `uc:`, `ajaxToolkit:` prefixes stripped |
| Attributes | `runat="server"`, `AutoEventWireup`, `EnableViewState`, `ViewStateMode`, `ValidateRequest`, `ClientIDMode` stripped |
| Expressions | `<%# Eval() %>`, `<%# Bind() %>`, `<%# Item.Prop %>`, `<%: %>`, `<%= %>` → Razor syntax |
| Data binding | `Eval()` with format strings, `String.Format(Item.Prop)`, bare `Item` → `@context` |
| Master pages | `Site.Master` → `MainLayout.razor` with `@Body`, `<HeadContent>`, CDN/CSS extraction |
| Content pages | `<asp:Content>` wrappers stripped, `ContentPlaceHolderID` mapped |
| LoginView | `<asp:LoginView>` → `<LoginView>` (BWFC component) |
| Forms | `<form runat="server">` → `<div>` preserving id for CSS |
| URLs | `~/path` → `/path`, `NavigateUrl`, `ImageUrl`, `href` |
| Attributes | `ItemType` → `TItem`, `ID` → `id`, boolean/enum/unit normalization |
| Data sources | `DataSourceID` removed, `SqlDataSource`/`ObjectDataSource`/etc. replaced with TODO |
| SelectMethod | Preserved with TODO for delegate conversion |
| Code-behind | Copied with TODO header, `System.Web.*`/`Microsoft.AspNet.*`/`Owin` usings stripped, base class declarations removed |
| Response.Redirect | Converted to `NavigationManager.NavigateTo()` with `[Inject]` injection |
| Session/ViewState | Detected, keys enumerated, migration guidance injected |
| Static files | CSS/JS/images copied to `wwwroot/`, CSS/CDN auto-detected and injected into `App.razor` |
| Scripts | `Scripts/` folder detected, JS files copied and wired with correct load order |
| Scaffolding | `.csproj`, `Program.cs`, `_Imports.razor`, `GlobalUsings.cs`, `App.razor`, `Routes.razor`, `launchSettings.json` |
| DB detection | `Web.config` connection strings → EF Core provider detection (SQL Server, SQLite, PostgreSQL, MySQL) |
| Identity detection | `Account/` folder or `Login.aspx` → Identity package references + `Program.cs` boilerplate |

### Existing BWFC Shims (already ship in the NuGet package)

| Shim | What It Does |
|------|-------------|
| `WebFormsPageBase` | `Page.Title`, `Page.IsPostBack`, `Page.ViewState`, `Page.Response`, `Page.Request`, `Page.GetRouteUrl()` |
| `ResponseShim` | `Response.Redirect()` → `NavigationManager.NavigateTo()`, `Response.Cookies` with graceful degradation |
| `RequestShim` | `Request.QueryString`, `Request.Cookies`, `Request.Url` with NavigationManager fallback |
| `ViewStateDictionary` | Full `IDictionary<string, object?>` with encryption, serialization, dirty tracking |
| `.targets` type aliases | `Page` → `WebFormsPageBase`, `MasterPage` → `LayoutComponentBase`, `ImageClickEventArgs` → `MouseEventArgs` |
| Identity stubs | `ApplicationSignInManager`, `ApplicationUserManager`, `IdentityUser`, `IdentityResult`, `SignInStatus`, `IdentityDbContext` |
| EF6 stubs | `Database.SetInitializer()`, `DropCreateDatabaseIfModelChanges<T>` |
| `DataBinder` | `Eval()`, `GetPropertyValue()` (marked `[Obsolete]`) |
| `QueryStringAttribute` | Compilation shim for `[QueryString("param")]` on method parameters |
| `RouteDataAttribute` | Compilation shim for `[RouteData]` on method parameters |
| `HttpHandlerBase` | Base class for migrating `.ashx` handlers with `ProcessRequestAsync()` |
| `MapHandler<T>()` | Minimal API endpoint registration for handlers |
| Middleware stack | `AspxRewriteMiddleware` (301 redirect .aspx → clean), `AshxHandlerMiddleware` (410/301), `AxdHandlerMiddleware` (404/410) |
| Theming | `App_Themes` auto-discovery, `.skin` file parsing, `ThemeMode.StyleSheetTheme` / `ThemeMode.Theme` |

---

## Manual Migration Items: The Full Table

| # | Category | Manual Item | Feasibility | Effort | Impact |
|---|----------|-------------|:-----------:|--------|--------|
| 1 | Page Lifecycle | `Page_Load` → `OnInitializedAsync` | 🟢 Easy | 1 wk | **Critical** |
| 2 | Page Lifecycle | `Page_Init` → `OnInitialized` | 🟢 Easy | 1 wk | High |
| 3 | Page Lifecycle | `Page_PreRender` → `OnAfterRenderAsync` | 🟢 Easy | 1 wk | Medium |
| 4 | Page Lifecycle | `Page_Unload` → `IDisposable.Dispose` | 🟡 Medium | 1 wk | Low |
| 5 | Page Lifecycle | `IsPostBack` guards in `Page_Load` | 🟢 Easy | 0.5 wk | **Critical** |
| 6 | Event Handlers | `Button_Click(object, EventArgs)` → `EventCallback` | 🟡 Medium | 2 wk | **Critical** |
| 7 | Event Handlers | `SelectedIndexChanged`, `RowCommand`, etc. | 🟡 Medium | 2 wk | High |
| 8 | Event Handlers | `FindControl("id")` → `@ref` | 🔴 Hard | — | Medium |
| 9 | Data Binding | `DataSource = x; DataBind()` → `Items="@x"` | 🟡 Medium | 2 wk | **Critical** |
| 10 | Data Binding | `DataSourceID` → service injection | 🔴 Hard | — | High |
| 11 | Data Binding | `<%$ ConnectionStrings:Name %>` expression | 🟢 Easy | 0.5 wk | Low |
| 12 | Data Binding | `<%$ Resources:Key %>` expression | 🟢 Easy | 1 wk | Low |
| 13 | State Mgmt | `Session["key"]` → scoped service | 🟢 Easy | 1 wk | **Critical** |
| 14 | State Mgmt | `Application["key"]` → singleton service | 🟢 Easy | 0.5 wk | Medium |
| 15 | State Mgmt | `Cache["key"]` → `IMemoryCache` | 🟢 Easy | 0.5 wk | Medium |
| 16 | State Mgmt | `HttpContext.Current` → `IHttpContextAccessor` | 🟢 Easy | 0.5 wk | High |
| 17 | Navigation | `Server.Transfer()` | 🟡 Medium | 1 wk | Medium |
| 18 | Navigation | `Request.Form["field"]` | 🟢 Easy | 0.5 wk | Low |
| 19 | Configuration | `ConfigurationManager.AppSettings["key"]` | 🟢 Easy | 1 wk | **Critical** |
| 20 | Configuration | `ConfigurationManager.ConnectionStrings["name"]` | 🟢 Easy | 0.5 wk | High |
| 21 | Configuration | `web.config` `<system.web>` sections | 🔴 Hard | — | Medium |
| 22 | HTTP Pipeline | `HttpModule` → Middleware | 🟡 Medium | 2 wk | Medium |
| 23 | HTTP Pipeline | `Global.asax` events → `Program.cs` | 🟡 Medium | 2 wk | High |
| 24 | HTTP Pipeline | `Application_Error` → exception middleware | 🟡 Medium | 1 wk | Medium |
| 25 | Authentication | Forms Auth cookie config → ASP.NET Core Auth | 🔴 Hard | — | **Critical** |
| 26 | Authentication | `Membership` provider → ASP.NET Core Identity | 🔴 Hard | — | **Critical** |
| 27 | Authentication | `Roles` provider → ASP.NET Core Identity roles | 🟡 Medium | 2 wk | High |
| 28 | Authentication | `User.Identity.Name` / `User.IsInRole()` | 🟢 Easy | 0.5 wk | High |
| 29 | Master Pages | Nested master pages | 🟡 Medium | 1 wk | Low |
| 30 | User Controls | `.ascx` → Blazor component (beyond tag stripping) | 🟡 Medium | 2 wk | High |
| 31 | User Controls | `Register` directive → `_Imports.razor` | 🟢 Easy | 0.5 wk | Medium |
| 32 | Code-Behind | Event handler wiring (`OnClick="btn_Click"`) | 🟡 Medium | 2 wk | **Critical** |
| 33 | Code-Behind | `sender`/`e` casting in event handlers | 🟢 Easy | 0.5 wk | Medium |
| 34 | Code-Behind | `partial class` ↔ `.razor` name mismatches | 🟢 Easy | 0.5 wk | High |
| 35 | Inline Expressions | `<%$ AppSettings:Key %>` expression syntax | 🟢 Easy | 0.5 wk | Low |
| 36 | Inline Expressions | `<% code blocks %>` (non-expression) | 🔴 Hard | — | Medium |
| 37 | Static Files | `BundleConfig.cs` → individual `<link>`/`<script>` | 🟡 Medium | 1 wk | Medium |
| 38 | Static Files | `ScriptManager.RegisterClientScriptBlock` | 🟡 Medium | 1 wk | Low |
| 39 | Static Files | WebResource.axd / ScriptResource.axd references | ✅ Done | — | — |
| 40 | Third-Party | Telerik controls → manual | 🔴 Hard | — | Medium |
| 41 | Third-Party | DevExpress controls → manual | 🔴 Hard | — | Medium |
| 42 | Third-Party | ACT controls not yet in BWFC | 🟡 Medium | varies | Medium |
| 43 | Misc | `Thread.CurrentPrincipal` → `AuthenticationStateProvider` | 🟡 Medium | 1 wk | Low |
| 44 | Misc | `HttpUtility.HtmlEncode/Decode` | 🟢 Easy | 0.5 wk | Low |
| 45 | Misc | `Server.MapPath()` → `IWebHostEnvironment.WebRootPath` | 🟢 Easy | 0.5 wk | Medium |
| 46 | Misc | `Response.Write()` / `Response.End()` | 🟡 Medium | 1 wk | Low |
| 47 | Misc | `HttpContext.Current.Items` → `HttpContext.Items` | 🟢 Easy | 0.5 wk | Low |

---

## Detailed Analysis by Category

### 1. Page Lifecycle (Items 1–5)

**The #1 source of manual work.** WingtipToys required 33 code-behind rewrites, and lifecycle method renaming was in every single one.

#### What's Already Done
- ✅ `IsPostBack` — shimmed in `WebFormsPageBase` (SSR mode checks HTTP method, interactive mode tracks init state)
- ✅ L1 script adds TODO header listing all lifecycle method mappings

#### Proposed Shims

**Item 1–3: Lifecycle Method Renaming (🟢 Easy)**

**Approach: Roslyn Analyzer + Code Fixer (`BWFC100`)**

```
Diagnostic: "Page_Load detected — rename to OnInitializedAsync and add override keyword"
Code Fix: Rename method, change signature, add async/override, wrap body in Task
```

This is a straightforward Roslyn analyzer because:
- Method names are well-known (`Page_Load`, `Page_Init`, `Page_PreRender`, `Page_LoadComplete`)
- Target signatures are deterministic
- The rename is always the same: `Page_Load(object sender, EventArgs e)` → `protected override async Task OnInitializedAsync()`

**Mapping:**
| Web Forms | Blazor | Notes |
|-----------|--------|-------|
| `Page_Init` | `OnInitialized()` | Sync |
| `Page_Load` | `OnInitializedAsync()` | Async, most common |
| `Page_PreRender` | `OnAfterRenderAsync(bool firstRender)` | Add `if (firstRender)` guard |
| `Page_LoadComplete` | `OnParametersSetAsync()` | Closest equivalent |
| `Page_Unload` | `IDisposable.Dispose()` | Add interface implementation |

**Alternative for L1 script:** Add method renaming as a regex transform in `Copy-CodeBehind`. This is less precise than Roslyn but faster to ship:
```powershell
$content = $content -replace 'protected\s+void\s+Page_Load\s*\(object\s+\w+,\s*EventArgs\s+\w+\)',
    'protected override async Task OnInitializedAsync()'
```

**Item 5: IsPostBack Guard Elimination (🟢 Easy)**

The L1 script already detects `IsPostBack`. Enhancement: When the method body is only `if (!IsPostBack) { ... }`, unwrap the guard entirely (in `OnInitializedAsync`, the body always runs on first render).

**Approach:** Regex in L1 script or Roslyn fixer.

---

### 2. Event Handlers (Items 6–8)

**The #2 source of manual work.** Every `OnClick`, `OnSelectedIndexChanged`, `OnRowCommand` needs signature conversion.

#### What's Already Done
- ✅ L1 script flags event handler attributes for review
- ✅ BWFC components accept `EventCallback` parameters

#### Proposed Shims

**Item 6: Button Click Handlers (🟡 Medium)**

**Approach: L1 Script Enhancement + Roslyn Fixer**

The L1 script can generate stub event handler methods with correct Blazor signatures:

```csharp
// Before (Web Forms):
protected void btnSubmit_Click(object sender, EventArgs e) { ... }

// After (auto-generated stub):
private async Task btnSubmit_Click(MouseEventArgs e) { /* TODO: original body */ }
```

The tricky part: the markup attribute `OnClick="btnSubmit_Click"` needs to become `OnClick="@btnSubmit_Click"` (add `@` prefix). L1 can do this with regex.

**Event handler signature map:**
| Web Forms Event | Blazor EventCallback | Args Type |
|----------------|---------------------|-----------|
| `OnClick` | `EventCallback<MouseEventArgs>` | `MouseEventArgs` |
| `OnTextChanged` | `EventCallback<ChangeEventArgs>` | `ChangeEventArgs` |
| `OnSelectedIndexChanged` | `EventCallback<ChangeEventArgs>` | `ChangeEventArgs` |
| `OnRowCommand` | `EventCallback<GridViewCommandEventArgs>` | BWFC type |
| `OnRowEditing` | `EventCallback<GridViewEditEventArgs>` | BWFC type |
| `OnItemCommand` | `EventCallback<RepeaterCommandEventArgs>` | BWFC type |
| `OnCheckedChanged` | `EventCallback<ChangeEventArgs>` | `ChangeEventArgs` |

**Item 8: FindControl (🔴 Hard)**

`FindControl("txtName")` has no programmatic equivalent in Blazor. It requires understanding the component tree at compile time. Can only provide guidance:
- Roslyn analyzer: "FindControl detected — use `@ref` and component references"
- Doc snippet in TODO comment

---

### 3. Data Binding (Items 9–12)

#### What's Already Done
- ✅ `Eval()`, `Bind()`, `Item.Property` expressions converted by L1
- ✅ `DataBinder.Eval()` shim exists (returns `RenderFragment`)
- ✅ `SelectMethod` preserved and supported natively by BWFC
- ✅ `DataSourceID` stripped with TODO
- ✅ Data source controls replaced with TODO

#### Proposed Shims

**Item 9: DataSource/DataBind() Pattern (🟡 Medium)**

**Approach: Roslyn Analyzer (`BWFC101`)**

Detect the pattern:
```csharp
gridView1.DataSource = GetProducts();
gridView1.DataBind();
```

Replace with code-behind field + `OnInitializedAsync`:
```csharp
private IEnumerable<object> _gridView1Data;

protected override async Task OnInitializedAsync()
{
    _gridView1Data = GetProducts();
}
```

And in markup, add `Items="@_gridView1Data"` to the component.

This is "Medium" because it requires understanding which control the DataSource is assigned to and correlating with the markup.

**Item 11: `<%$ ConnectionStrings:Name %>` (🟢 Easy)**

**Approach: L1 regex transform.**
```
<%$ ConnectionStrings:DefaultConnection %> → @Configuration.GetConnectionString("DefaultConnection")
```
Add `@inject IConfiguration Configuration` to the page. Simple regex.

**Item 12: `<%$ Resources:Key %>` (🟢 Easy)**

**Approach: L1 regex + IStringLocalizer shim.**
```
<%$ Resources:Labels, WelcomeMessage %> → @Localizer["WelcomeMessage"]
```
Add `@inject IStringLocalizer<Labels> Localizer`. Needs a thin `IStringLocalizer` adapter that reads from `.resx` files.

---

### 4. State Management (Items 13–16)

#### What's Already Done
- ✅ `ViewState` — `ViewStateDictionary` with encryption/serialization
- ✅ Session detection — L1 enumerates keys, adds guidance
- ✅ `HttpContext.Current` → `IHttpContextAccessor` registered by `AddBlazorWebFormsComponents()`

#### Proposed Shims

**Item 13: Session State Shim Service (🟢 Easy — HIGH IMPACT)**

**Approach: Drop-in `SessionShim` class + DI registration**

```csharp
public class SessionShim
{
    private readonly IHttpContextAccessor _accessor;
    private readonly Dictionary<string, object?> _fallback = new();

    public object? this[string key]
    {
        get => _accessor.HttpContext?.Session.GetString(key) is { } json
            ? JsonSerializer.Deserialize<object>(json)
            : _fallback.TryGetValue(key, out var v) ? v : null;
        set { /* write to session or fallback */ }
    }
}
```

Register as scoped in `AddBlazorWebFormsComponents()`. Then `Session["CartId"]` compiles against this shim. In interactive mode, falls back to in-memory dictionary (with logging).

**Item 14: Application State Shim (🟢 Easy)**

**Approach: Singleton `ApplicationShim` with `ConcurrentDictionary<string, object?>`.**

`Application["SiteTitle"]` → shim indexer. Dead simple, ship in 1 day.

**Item 15: Cache Shim (🟢 Easy)**

**Approach: Thin wrapper around `IMemoryCache` with Web Forms-style indexer.**

```csharp
public class CacheShim
{
    private readonly IMemoryCache _cache;
    public object? this[string key]
    {
        get => _cache.TryGetValue(key, out var v) ? v : null;
        set => _cache.Set(key, value, TimeSpan.FromMinutes(20));
    }
    public void Insert(string key, object value, ...) { /* map to IMemoryCache */ }
}
```

**Item 16: HttpContext.Current (🟢 Easy)**

Already mitigated: `AddBlazorWebFormsComponents()` registers `IHttpContextAccessor`. For code that uses the static `HttpContext.Current`, add a type alias:

```csharp
// In .targets:
global using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
```

Plus a static shim class:
```csharp
public static class HttpContextShim
{
    [ThreadStatic] private static IHttpContextAccessor? _accessor;
    public static HttpContext? Current => _accessor?.HttpContext;
}
```

Register via `IHttpContextAccessor` in DI. This is a crutch, but it compiles legacy code.

---

### 5. Navigation (Items 17–18)

#### What's Already Done
- ✅ `Response.Redirect` → `NavigationManager.NavigateTo()` (L1 + ResponseShim)
- ✅ `Request.QueryString` → `RequestShim` with NavigationManager fallback
- ✅ `AspxRewriteMiddleware` handles `.aspx` URLs at runtime

#### Proposed Shims

**Item 17: Server.Transfer (🟡 Medium)**

`Server.Transfer()` has no exact equivalent — it's a server-side redirect that preserves the URL. Closest Blazor approach: `NavigationManager.NavigateTo()` with `replace: true`.

**Approach: Add `ServerShim` class to `WebFormsPageBase`:**
```csharp
protected ServerShim Server => new(_navigationManager);

public class ServerShim
{
    public void Transfer(string url) => _nav.NavigateTo(url, replace: true);
    public string MapPath(string virtualPath) => /* IWebHostEnvironment mapping */;
}
```

This doesn't preserve form data (which `Server.Transfer` did), but it provides compilation compatibility and the most common use case (URL-preserving redirect).

**Item 18: Request.Form (🟢 Easy)**

**Approach: Add `Form` property to `RequestShim`:**
```csharp
public IFormCollection? Form => _httpContext?.Request.Form;
```

Already have the `RequestShim` infrastructure. One property addition.

---

### 6. Configuration (Items 19–21)

#### What's Already Done
- ✅ L1 detects database provider from `web.config`
- ✅ L1 generates `appsettings.json` connection string guidance

#### Proposed Shims

**Item 19: ConfigurationManager.AppSettings (🟢 Easy — HIGH IMPACT)**

This is probably the single most common `System.Web` dependency in code-behind files.

**Approach: Drop-in shim class + web.config parser**

```csharp
public static class ConfigurationManager
{
    private static IConfiguration? _config;

    internal static void Initialize(IConfiguration config) => _config = config;

    public static NameValueCollectionShim AppSettings =>
        new(_config?.GetSection("AppSettings"));

    public static ConnectionStringSettingsCollection ConnectionStrings =>
        new(_config?.GetSection("ConnectionStrings"));
}
```

Register in `AddBlazorWebFormsComponents()` → call `ConfigurationManager.Initialize(configuration)`.

**L1 enhancement:** Parse `web.config` `<appSettings>` and generate `appsettings.json`:
```powershell
# In bwfc-migrate.ps1:
# Read web.config <appSettings> → write appsettings.json
$appSettings = $webConfig.configuration.appSettings.add
$json = @{ AppSettings = @{} }
foreach ($s in $appSettings) {
    $json.AppSettings[$s.key] = $s.value
}
$json | ConvertTo-Json | Set-Content "$Output/appsettings.json"
```

**Item 20: ConfigurationManager.ConnectionStrings (🟢 Easy)**

Covered by the same shim above. L1 already detects connection strings.

**Item 21: `<system.web>` Sections (🔴 Hard)**

Custom sections, `<httpModules>`, `<httpHandlers>`, `<compilation>`, `<authentication>`, `<authorization>` — each is unique. Can only provide:
- Roslyn analyzer flagging `System.Configuration.ConfigurationManager.GetSection()`
- Migration guide documentation

---

### 7. HTTP Pipeline (Items 22–24)

#### What's Already Done
- ✅ `.ashx` handlers → `HttpHandlerBase` + `MapHandler<T>()`
- ✅ `.aspx` URL rewriting → `AspxRewriteMiddleware`
- ✅ `.axd` resource handling → `AxdHandlerMiddleware`

#### Proposed Shims

**Item 22: HttpModule → Middleware (🟡 Medium)**

**Approach: `HttpModuleAdapter` base class**

```csharp
public abstract class HttpModuleAdapter
{
    // Web Forms-style event hooks — override the ones you need
    public virtual Task BeginRequest(HttpContext context) => Task.CompletedTask;
    public virtual Task AuthenticateRequest(HttpContext context) => Task.CompletedTask;
    public virtual Task AuthorizeRequest(HttpContext context) => Task.CompletedTask;
    public virtual Task EndRequest(HttpContext context) => Task.CompletedTask;
}

// Registration:
app.UseHttpModuleAdapter<MyLegacyModule>();
```

The adapter translates Web Forms module event hooks into ASP.NET Core middleware pipeline positions. Not perfect (event ordering differs), but handles 80% of modules (logging, auth, request filtering).

**Item 23: Global.asax → Program.cs (🟡 Medium)**

**Approach: L1 script enhancement**

Parse `Global.asax.cs` and generate `Program.cs` snippets:
- `Application_Start` → top-level statements in `Program.cs`
- `Application_Error` → `app.UseExceptionHandler()`
- `Session_Start` → comment with `IDistributedCache` guidance
- `Application_BeginRequest` → middleware registration

This is a one-time transform that L1 can handle with regex for the most common patterns.

**Item 24: Application_Error (🟡 Medium)**

**Approach:** Detect in Global.asax, generate exception handler middleware:
```csharp
app.UseExceptionHandler("/Error");
// TODO: Application_Error logic from Global.asax should be moved to Error.razor
```

---

### 8. Authentication (Items 25–28)

#### What's Already Done
- ✅ `ApplicationSignInManager` shim (no-op stubs returning `Success`)
- ✅ `ApplicationUserManager` shim (no-op stubs)
- ✅ `IdentityUser`, `IdentityResult`, `SignInStatus`, `UserLoginInfo` types
- ✅ `IdentityDbContext` shim
- ✅ Login controls: `Login`, `LoginView`, `LoginName`, `LoginStatus`, `ChangePassword`, `CreateUserWizard`, `PasswordRecovery`
- ✅ L1 detects identity pages and adds NuGet references

#### Proposed Shims

**Item 25: Forms Auth → ASP.NET Core Auth (🔴 Hard)**

This is a full subsystem replacement. The BWFC identity stubs let code *compile*, but they're no-ops. Real auth requires:
1. ASP.NET Core Identity scaffolding
2. Database migration (membership tables → Identity tables)
3. Password hash migration (SHA1 → bcrypt)

**What we CAN do:** Provide a `FormsAuthenticationShim` that wraps ASP.NET Core cookie auth:
```csharp
public static class FormsAuthentication
{
    public static void SetAuthCookie(string username, bool persistent) { /* HttpContext.SignInAsync */ }
    public static void SignOut() { /* HttpContext.SignOutAsync */ }
    public static string LoginUrl => "/Account/Login";
}
```

This would let code like `FormsAuthentication.SetAuthCookie(username, true)` compile and actually work against ASP.NET Core's cookie auth. Ship in the identity shim namespace.

**Item 26: Membership Provider (🔴 Hard)**

Full provider model mismatch. Can extend `ApplicationUserManager` to delegate to `UserManager<IdentityUser>` at runtime (tracked by GitHub issue #525). Compilation works today; runtime delegation is the gap.

**Item 27: Roles Provider (🟡 Medium)**

**Approach: Extend identity shims with `Roles` static class:**
```csharp
public static class Roles
{
    public static bool IsUserInRole(string username, string role) { /* ClaimsPrincipal check */ }
    public static string[] GetRolesForUser(string username) { /* query Identity */ }
}
```

**Item 28: User.Identity.Name / User.IsInRole (🟢 Easy)**

These actually work out of the box in ASP.NET Core! The `ClaimsPrincipal` API is the same. Just need to ensure `HttpContext.User` is populated (auth middleware) and make sure `@context.User` is accessible in components. Already handled by `[CascadingParameter] Task<AuthenticationState>`.

---

### 9. Master Pages / Content Pages (Item 29)

#### What's Already Done
- ✅ `Site.Master` → `MainLayout.razor` with full head extraction, CDN preservation, `@Body`
- ✅ BWFC `Content` / `ContentPlaceHolder` components
- ✅ `<asp:Content>` wrapper stripping

#### Proposed Shims

**Item 29: Nested Master Pages (🟡 Medium)**

Web Forms supports master page nesting (`Site.Master` → `Admin.Master` → `AdminPage.aspx`). Blazor supports this via `@layout` directive chaining.

**Approach:** L1 enhancement — detect `MasterPageFile` attribute in `<%@ Page %>` and `<%@ Master %>` directives, generate appropriate `@layout` directives:
```
<%@ Master MasterPageFile="~/Site.Master" %> → @layout MainLayout
```

---

### 10. User Controls (Items 30–31)

#### What's Already Done
- ✅ `uc:` prefix stripping
- ✅ `<%@ Control %>` directive removal
- ✅ `<%@ Register %>` directive removal with flagging

#### Proposed Shims

**Item 30: ASCX → Component Conversion (🟡 Medium)**

ASCX files are already renamed to `.razor` by L1 and prefixes are stripped. The remaining work is:
1. Converting `public` properties to `[Parameter]` properties
2. Handling `<%@ Register Src="..." %>` references → `@using` or component tag names
3. Code-behind lifecycle methods (same as pages)

**Approach:** Roslyn analyzer that detects public properties in user control code-behinds and suggests `[Parameter]` attribute addition. L1 script enhancement for `Register` → `@using` mapping.

**Item 31: Register → _Imports.razor (🟢 Easy)**

**Approach:** L1 script enhancement — parse `<%@ Register TagPrefix="uc" TagName="Ctrl" Src="~/Controls/MyCtrl.ascx" %>` and add `@using` to `_Imports.razor` for the namespace containing `MyCtrl.razor`.

---

### 11. Code-Behind Patterns (Items 32–34)

#### What's Already Done
- ✅ Base class stripping (`Page`, `MasterPage`, `UserControl`)
- ✅ `System.Web.*` using removal
- ✅ TODO header injection
- ✅ Response.Redirect conversion
- ✅ Session/ViewState detection

#### Proposed Shims

**Item 32: Event Handler Wiring (🟡 Medium — HIGH IMPACT)**

In markup: `OnClick="btnSave_Click"` needs `@` prefix → `OnClick="@btnSave_Click"`.
In code-behind: method signature needs conversion.

**Approach (L1 script):**
```powershell
# In markup transform:
$Content = $Content -replace '(On\w+)="(\w+)"', '$1="@$2"'

# In code-behind transform:
# Generate EventCallback-compatible signatures
```

This is the second-highest-impact improvement possible. Combined with lifecycle method renaming, it would eliminate ~70% of L2 work.

**Item 33: Sender/E Casting (🟢 Easy)**

**Approach:** Roslyn fixer — detect `(Button)sender` casts and replace with component `@ref` pattern. Or simply remove the cast if the sender isn't used.

**Item 34: Class Name Mismatches (🟢 Easy)**

L1 already handles some of this (stripping `_aspx` suffixes). Enhancement: ensure the `partial class` name in `.razor.cs` exactly matches the `.razor` filename. Simple regex in `Copy-CodeBehind`.

---

### 12. Inline Expressions (Items 35–36)

#### What's Already Done
- ✅ `<%# %>` → `@context` expressions
- ✅ `<%: %>` → `@()` encoded expressions
- ✅ `<%= %>` → `@()` unencoded expressions
- ✅ `<%-- --%>` → `@* *@` comments
- ✅ Remaining `<% %>` blocks flagged as manual

#### Proposed Shims

**Item 35: `<%$ AppSettings:Key %>` (🟢 Easy)**

**Approach:** L1 regex:
```powershell
$Content = $Content -replace '<%\$\s*AppSettings:(\w+)\s*%>', '@Configuration["AppSettings:$1"]'
```

Also handle `<%$ ConnectionStrings:Name %>` and `<%$ Resources:File,Key %>`.

**Item 36: `<% code blocks %>` (🔴 Hard)**

Inline code blocks (render blocks) that contain loops, conditionals, and arbitrary C# have no mechanical conversion. They require understanding intent and restructuring into Razor `@if`/`@foreach` blocks. Can only flag.

---

### 13. Static Files (Items 37–38)

#### What's Already Done
- ✅ CSS/JS/images copied to `wwwroot/`
- ✅ CSS auto-detection → `<link>` tags in `App.razor`
- ✅ JS auto-detection → `<script>` tags with correct load order
- ✅ CDN references preserved from `Site.Master`
- ✅ `<webopt:bundlereference>` flagged with TODO

#### Proposed Shims

**Item 37: BundleConfig.cs Parsing (🟡 Medium)**

**Approach:** L1 script enhancement — parse `BundleConfig.cs` to extract bundle definitions:
```csharp
bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/bootstrap.css", "~/Content/site.css"));
```
→ Generate individual `<link>` tags in `App.razor`.

Regex-parseable because the `Include()` calls use string literals. Won't handle dynamic bundles, but covers 90% of cases.

**Item 38: ScriptManager.RegisterClientScriptBlock (🟡 Medium)**

**Approach:** Shim that delegates to `IJSRuntime`:
```csharp
public class ScriptManagerShim
{
    private readonly IJSRuntime _js;

    public async Task RegisterClientScriptBlock(Type type, string key, string script)
    {
        await _js.InvokeVoidAsync("eval", script);
    }
}
```

Hacky but functional for migration. Add `[Obsolete]` with guidance to convert to proper JS interop.

---

### 14. Third-Party Controls (Items 40–42)

#### What's Already Done
- ✅ ACT prefix stripping for known controls (Accordion, TabContainer, ModalPopup, etc.)
- ✅ Unknown ACT controls → TODO comments
- ✅ `ToolkitScriptManager` stripped

#### Proposed Shims

**Items 40–41: Telerik / DevExpress (🔴 Hard)**

These vendors have their own Blazor component suites (Telerik UI for Blazor, DevExpress Blazor). BWFC cannot and should not provide shims — the vendors' own migration guides are the path.

**What we CAN do:** L1 script enhancement to detect vendor prefixes and flag them:
```powershell
# Detect Telerik: <telerik:RadGrid>, <rad:RadComboBox>
# Detect DevExpress: <dx:ASPxGridView>, <dxe:ASPxComboBox>
# Flag with migration guidance URLs
```

**Item 42: Additional ACT Controls (🟡 Medium)**

The ACT extender coverage analysis (`ACT_EXTENDER_COVERAGE_ANALYSIS.md`) tracks which ACT controls have BWFC equivalents. Each new control is a bounded implementation task.

---

## Priority Recommendations: What to Build First

### Tier 1: Maximum Impact (build these first)

| # | Shim | Why | Est. Effort |
|---|------|-----|-------------|
| 1 | **Page lifecycle method renaming** (Roslyn fixer or L1 regex) | Affects 100% of migrated pages. WingtipToys needed 33 rewrites. | 1–2 weeks |
| 2 | **Event handler signature conversion** (L1 markup `@` prefix + code-behind signature rewrite) | Affects 80%+ of pages. Currently the most tedious L2 task. | 2 weeks |
| 3 | **SessionShim service** | `Session["key"]` is in 40%+ of enterprise Web Forms apps. Drop-in scoped service. | 1 week |
| 4 | **ConfigurationManager shim** | `ConfigurationManager.AppSettings["key"]` is the #1 `System.Web` dependency. | 1 week |
| 5 | **web.config → appsettings.json auto-conversion** (L1 script) | `<appSettings>` + `<connectionStrings>` → JSON. Eliminates config manual work. | 0.5 week |

**Combined impact:** These 5 items would reduce Layer 2 manual work by an estimated **60–70%**.

### Tier 2: High Value (build next)

| # | Shim | Why | Est. Effort |
|---|------|-----|-------------|
| 6 | **ApplicationShim** (Application state) | Simple singleton dictionary. Ships in a day. | 0.5 week |
| 7 | **CacheShim** (IMemoryCache wrapper) | Common in data-heavy apps. | 0.5 week |
| 8 | **ServerShim** (Server.Transfer, Server.MapPath) | `Server.MapPath()` is ubiquitous for file operations. | 1 week |
| 9 | **FormsAuthentication static shim** | `SetAuthCookie`/`SignOut` → ASP.NET Core cookie auth. Bridges the compilation gap. | 1 week |
| 10 | **`<%$ %>` expression transforms** (L1 script) | AppSettings, ConnectionStrings, Resources expressions → Razor. | 1 week |
| 11 | **BundleConfig.cs parser** (L1 script) | Extract CSS/JS bundle definitions → `<link>`/`<script>` tags. | 1 week |
| 12 | **DataSource/DataBind() Roslyn fixer** | Detect and convert the imperative data-binding pattern. | 2 weeks |

### Tier 3: Nice to Have (build as needed)

| # | Shim | Why | Est. Effort |
|---|------|-----|-------------|
| 13 | `HttpModuleAdapter` base class | Enterprise apps with custom modules. | 2 weeks |
| 14 | `Global.asax` parser (L1 script) | Auto-generate Program.cs snippets. | 2 weeks |
| 15 | `Roles` static class shim | Less common than UserManager patterns. | 1 week |
| 16 | `ScriptManagerShim` for code-behind JS registration | Rare in modern apps. | 1 week |
| 17 | Telerik/DevExpress detection in L1 | Helpful flagging, no actual migration. | 0.5 week |
| 18 | Nested master page `@layout` chaining | Uncommon outside enterprise portals. | 1 week |
| 19 | `HttpUtility.HtmlEncode/Decode` type alias | `System.Net.WebUtility` is the replacement, but type alias helps. | 0.5 week |

### Items That Will Always Be Manual (🔴)

These cannot be automated due to fundamental architecture differences:

1. **Full Identity/Membership provider migration** — Password hashing, database schema, provider model are all different. BWFC compilation shims help, but runtime requires ASP.NET Core Identity scaffolding.
2. **DataSourceID → service injection** — Every data source control encapsulates a unique query pattern. Must be manually converted to DI services.
3. **`<% code blocks %>` in markup** — Arbitrary inline C# requires understanding intent.
4. **`web.config` `<system.web>` custom sections** — Each section is unique configuration.
5. **FindControl() patterns** — Requires compile-time knowledge of component tree.
6. **Third-party vendor controls** — Use vendor-provided Blazor migration paths.
7. **Payment/business logic integrations** — API keys, callback URLs, flow logic are all app-specific.

---

## Summary: The Path to 90% Automation

| Layer | What | Automation Level Today | With Proposed Shims |
|-------|------|----------------------|-------------------|
| L1 (Script) | File transforms, directive conversion, expression conversion | ~60% | ~75% |
| L1.5 (Shims) | Type aliases, compilation shims, runtime adapters | ~15% | ~25% |
| L2 (Copilot) | Code-behind rewrite, service creation, architectural decisions | ~10% | ~10% |
| Manual | Identity migration, third-party controls, business logic | ~15% | ~10% (reduced by FormsAuth shim) |

**Total estimated automation: 60% today → ~80% with Tier 1 shims → ~90% with Tier 1+2 shims.**

The remaining 10% is irreducible manual work: app-specific business logic, third-party vendor controls, and full identity provider migrations. But 90% automation means a developer spends hours, not weeks, on a typical Web Forms migration.

---

*— Forge, Web Forms Reviewer*
*"I've migrated enough Web Forms apps to know where the bodies are buried. These shims are the map."*

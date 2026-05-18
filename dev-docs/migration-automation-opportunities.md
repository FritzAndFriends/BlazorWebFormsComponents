# Migration Automation Opportunities

> **Author:** Bishop (Migration Tooling Dev)
> **Date:** 2026-07-25
> **Requested by:** Jeffrey T. Fritz
> **Scope:** Identify what's manual today and propose creative automation solutions — shims, redirections, programmatic tricks.

---

## Executive Summary

The L1 migration script (`bwfc-migrate.ps1`) handles ~60% of markup transforms mechanically. The remaining ~40% is L2 (Copilot/manual) work, primarily: code-behind lifecycle rewrites, Identity integration, data access patterns, and Web Forms API calls that have no direct Blazor equivalent.

**This report identifies 23 automation opportunities.** Of these, **9 are quick wins** (shippable in a single PR) that collectively could push L1 coverage from ~60% to ~75% by eliminating the most common "manual review" items with clever shims and type aliases.

---

## 1. Current Automation Coverage

### What L1 (`bwfc-migrate.ps1`) Already Handles

| Category | What It Does | Coverage |
|----------|-------------|----------|
| **File renames** | .aspx/.ascx/.master → .razor | ✅ Complete |
| **Directive transforms** | `<%@ Page %>` → `@page`, `<%@ Import %>` → `@using` | ✅ Complete |
| **Tag prefix removal** | `<asp:Button>` → `<Button>`, `<ajaxToolkit:*>` → `<*>` | ✅ Complete |
| **Attribute stripping** | `runat="server"`, `AutoEventWireup`, `EnableViewState`, etc. | ✅ Complete |
| **Expression conversion** | `<%# Eval("Prop") %>` → `@context.Prop`, `<%: expr %>` → `@(expr)` | ✅ Complete |
| **Content/Form transforms** | `<asp:Content>` → strip, `<form runat="server">` → `<div>` | ✅ Complete |
| **Master page → Layout** | ContentPlaceHolder → `@Body`, head extraction → `<HeadContent>` | ✅ Complete |
| **URL references** | `~/path` → `/path` | ✅ Complete |
| **Boolean normalization** | `True`/`False` → `true`/`false` | ✅ Complete |
| **Enum type-qualifying** | `GridLines="Both"` → `GridLines="@GridLines.Both"` | ✅ Complete |
| **Unit normalization** | `Width="100px"` → `Width="100"` | ✅ Complete |
| **LoginView conversion** | `<asp:LoginView>` → `<LoginView>` with template preservation | ✅ Complete |
| **SelectMethod preservation** | Keeps `SelectMethod="X"` with TODO for delegate conversion | ✅ Complete |
| **DataSourceID removal** | Strips `DataSourceID=` and replaces data source controls with TODOs | ✅ Complete |
| **Response.Redirect** | `Response.Redirect("url")` → `NavigationManager.NavigateTo("url")` in code-behind | ✅ Complete |
| **Session/ViewState detection** | Detects usage, adds migration guidance comments | ✅ Detection only |
| **Scaffold generation** | .csproj, Program.cs, _Imports.razor, App.razor, Routes.razor, GlobalUsings.cs | ✅ Complete |
| **Static file copy** | CSS, JS, images, fonts → wwwroot/ | ✅ Complete |
| **CSS/CDN auto-detection** | Injects `<link>` and `<script>` tags into App.razor | ✅ Complete |
| **Models copy + DbContext transform** | Copies Models/, transforms DbContext for EF Core | ✅ Complete |
| **EDMX → EF Core** | Parses .edmx, generates entity classes + DbContext | ✅ Complete |
| **Business logic copy** | BLL/, Logic/, Services/ directories with using cleanup | ✅ Complete |
| **Placeholder → @context** | Template placeholder elements → `@context` | ✅ Complete |

### What BWFC Library Shims Already Provide

| Shim | Web Forms API | Implementation |
|------|-------------|----------------|
| **WebFormsPageBase** | `Page.Title`, `Page.IsPostBack`, `Page.MetaDescription`, `Page.GetRouteUrl()`, `Page.Response`, `Page.Request` | ✅ Full |
| **ResponseShim** | `Response.Redirect()`, `Response.Cookies` | ✅ Full |
| **RequestShim** | `Request.Cookies`, `Request.QueryString`, `Request.Url` | ✅ Full |
| **ViewStateDictionary** | `ViewState["key"]` | ✅ Full (encrypted round-trip in SSR) |
| **Type alias: Page** | `class MyPage : Page` → compiles as `WebFormsPageBase` | ✅ Via .targets |
| **Type alias: MasterPage** | `class MyLayout : MasterPage` → compiles as `LayoutComponentBase` | ✅ Via .targets |
| **Type alias: ImageClickEventArgs** | `→ MouseEventArgs` | ✅ Via .targets |
| **Identity stubs** | `ApplicationSignInManager`, `ApplicationUserManager`, `IdentityUser`, etc. | ⚠️ Stubs only (compile, no-op) |
| **EF6 stubs** | `Database.SetInitializer()`, `DropCreateDatabaseIfModelChanges` | ⚠️ Stubs only |
| **Handler framework** | `HttpHandlerBase`, `HttpHandlerContext`, request/response adapters | ✅ Full |
| **Middleware** | `.aspx` rewrite, `.ashx` handling, `.axd` interception | ✅ Full |
| **ScriptManager** | Renders nothing, absorbs all Web Forms ScriptManager attributes | ✅ No-op stub |
| **GridViewRow** (non-generic) | `GridViewRow` without `<T>` | ⚠️ L1 bridge only |
| **QueryStringAttribute** | `[QueryString("name")]` marker | ⚠️ Marker only |
| **RouteDataAttribute** | `[RouteData]` marker | ⚠️ Marker only |
| **FindControl()** | `BaseWebFormsComponent.FindControl()` | ✅ Tree search |
| **DataBind()** | `BaseWebFormsComponent.DataBind()` | ⚠️ No-op stub |
| **DataBinder.Eval()** | Static eval methods | ⚠️ Obsolete bridge |
| **GetRouteUrlHelper** | Static route URL generation | ✅ Full |
| **ShimControls** | `Panel`, `PlaceHolder`, `HtmlGenericControl`, `INamingContainer` | ✅ Functional |
| **RequiresSessionStateAttribute** | `[RequiresSessionState]` | ✅ Functional (handlers) |

---

## 2. Gap Inventory & Proposed Solutions

### GAP-01: `ConfigurationManager.AppSettings["key"]`

**Frequency:** Very common in Web Forms apps. Pre-scan rule BWFC014 catches `Request` patterns but not config access.
**Current handling:** None — manual replacement required.

**Proposed solution: Shim class** (Size: **S**)

```csharp
namespace BlazorWebFormsComponents;

/// <summary>
/// Shim for System.Configuration.ConfigurationManager. Wraps IConfiguration
/// so that ConfigurationManager.AppSettings["key"] compiles and works at runtime.
/// </summary>
public static class ConfigurationManager
{
    private static IConfiguration? _config;

    internal static void Initialize(IConfiguration config) => _config = config;

    public static AppSettingsShim AppSettings { get; } = new();

    public class AppSettingsShim
    {
        public string? this[string key]
            => _config?[key] ?? _config?.GetSection("AppSettings")?[key];
    }

    public static ConnectionStringSettingsCollection ConnectionStrings { get; } = new();
}
```

Plus: type alias in `.targets` → `Using Alias="ConfigurationManager" Include="BlazorWebFormsComponents.ConfigurationManager"` and initialization in `AddBlazorWebFormsComponents()`.

**Script transform in `bwfc-migrate.ps1`:** Strip `using System.Configuration;` (like we strip `using System.Web;`).

**Impact:** Eliminates the #1 most common manual fix in business logic files.

---

### GAP-02: `HttpContext.Current`

**Frequency:** Common in legacy service/utility classes.
**Current handling:** None — code-behind has `Request`/`Response` shims via `WebFormsPageBase`, but static utility classes calling `HttpContext.Current` break.

**Proposed solution: Shim class** (Size: **S**)

```csharp
namespace System.Web;

/// <summary>
/// Shim for HttpContext.Current. Resolves via IHttpContextAccessor registered at startup.
/// Logs a warning on first use — code should migrate to DI injection.
/// </summary>
[Obsolete("Migrate to constructor injection of IHttpContextAccessor")]
public static class HttpContext
{
    private static IHttpContextAccessor? _accessor;
    internal static void Initialize(IHttpContextAccessor accessor) => _accessor = accessor;

    public static Microsoft.AspNetCore.Http.HttpContext? Current
        => _accessor?.HttpContext;
}
```

**Trick:** Placing this in the `System.Web` namespace means existing `using System.Web;` imports (which we currently strip!) would resolve it. Instead, we keep the using and let the shim satisfy the reference.

**Script transform:** Stop stripping `using System.Web;` OR add a selective rule that keeps it only when `HttpContext.Current` is detected.

**Impact:** Many utility/helper classes compile immediately without touching them.

---

### GAP-03: `FormsAuthentication.RedirectToLoginPage()` / `FormsAuthentication.SignOut()`

**Frequency:** Common in account/login pages.
**Current handling:** None — completely manual.

**Proposed solution: Shim class** (Size: **S**)

```csharp
namespace BlazorWebFormsComponents;

[Obsolete("Migrate to ASP.NET Core Authentication")]
public static class FormsAuthentication
{
    private static NavigationManager? _nav;
    internal static void Initialize(NavigationManager nav) => _nav = nav;

    public static string LoginUrl { get; set; } = "/Account/Login";
    public static void RedirectToLoginPage() => _nav?.NavigateTo(LoginUrl);
    public static void SignOut() { /* TODO: Call SignInManager.SignOutAsync() */ }
    public static void SetAuthCookie(string userName, bool persistent) { /* no-op stub */ }
}
```

**Type alias:** `Using Alias="FormsAuthentication" Include="BlazorWebFormsComponents.FormsAuthentication"`

**Impact:** Login/logout pages compile at L1; real auth wired in L2.

---

### GAP-04: `Session["key"]` — Beyond Detection

**Frequency:** Very common (WingtipToys shopping cart, ContosoUniversity state).
**Current handling:** Detection + guidance comments. No compilable shim.

**Proposed solution: Session shim on WebFormsPageBase** (Size: **M**)

```csharp
// Already in WebFormsPageBase — add:
protected SessionShim Session => new(_httpContextAccessor);

public class SessionShim
{
    private readonly IHttpContextAccessor _accessor;
    private readonly Dictionary<string, object?> _fallback = new();

    public object? this[string key]
    {
        get => _accessor.HttpContext?.Session?.GetString(key) is string s
               ? System.Text.Json.JsonSerializer.Deserialize<object>(s)
               : _fallback.GetValueOrDefault(key);
        set
        {
            if (_accessor.HttpContext?.Session is { } session)
                session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
            else
                _fallback[key] = value;
        }
    }
}
```

**Script transform:** Add `builder.Services.AddDistributedMemoryCache(); builder.Services.AddSession();` and `app.UseSession();` to scaffolded Program.cs when session usage is detected.

**Impact:** Session-heavy apps compile AND run at L1. Cart logic works immediately.

---

### GAP-05: `Page_Load` / `Page_Init` → Lifecycle Conversion

**Frequency:** Every page has at least one lifecycle method.
**Current handling:** TODO comment in code-behind header. Fully manual.

**Proposed solution: PowerShell transform in `bwfc-migrate.ps1`** (Size: **M**)

```
# Regex transforms for common lifecycle methods:
Page_Load(object sender, EventArgs e) → OnInitializedAsync()  (body preserved)
Page_PreRender(object sender, EventArgs e) → OnAfterRenderAsync(bool firstRender)
Page_Init(object sender, EventArgs e) → OnInitialized()

# Also wrap body with firstRender guard if Page_PreRender:
if (firstRender) { /* original body */ }
```

**Script-level transform:** Strip `object sender, EventArgs e` parameters, rename method, add `async Task` return type, add `override` keyword.

**Impact:** Largest single source of L2 manual work. Even an 80% accurate transform saves massive time.

---

### GAP-06: `IsPostBack` Guards → Conditional Removal

**Frequency:** Almost every `Page_Load` has `if (!IsPostBack) { ... }`.
**Current handling:** `IsPostBack` works via `WebFormsPageBase` but the guard pattern is wrong for Blazor.

**Proposed solution: PowerShell transform** (Size: **S**)

```
# Transform: if (!IsPostBack) { body } → body (unwrap the guard)
# In OnInitializedAsync, everything should run on first load
# IsPostBack shim exists but the guard pattern should be unwrapped at L1
```

This is a regex transform that removes the `if (!IsPostBack)` wrapper and keeps the body, with a TODO comment.

**Impact:** Eliminates confusing IsPostBack guards that don't make sense in Blazor.

---

### GAP-07: Event Handler Signature Conversion

**Frequency:** Every button click, grid row command, etc.
**Current handling:** Detection only (BWFC011 prescan rule).

**Proposed solution: PowerShell transform** (Size: **M**)

```
# Button_Click(object sender, EventArgs e) → Button_Click()
# Grid_RowCommand(object sender, GridViewCommandEventArgs e) → Grid_RowCommand(GridViewCommandEventArgs e)
# Strip 'object sender' parameter; optionally keep specialized EventArgs
```

Plus: Add `protected` and `async Task` when the body contains `await` or redirect calls.

**Impact:** Makes most event handlers compilable at L1.

---

### GAP-08: `Startup.cs` / OWIN Middleware Configuration

**Frequency:** Every Identity-enabled app (WingtipToys has `Startup.Auth.cs`, `Startup.cs`).
**Current handling:** None — files are ignored by the script.

**Proposed solution: PowerShell transform** (Size: **L**)

Detect `Startup.cs` / `Startup.Auth.cs` → extract registration patterns → inject into scaffolded `Program.cs`:
- `app.UseCookieAuthentication()` → `builder.Services.AddAuthentication().AddCookie()`
- `app.UseExternalSignIn()` → `builder.Services.AddAuthentication().AddGoogle()` etc.
- `ConfigureAuth(IAppBuilder)` → extract and transform

**Impact:** Eliminates the most complex manual step in Identity-enabled apps.

---

### GAP-09: `using System.Web.*` — Selective Retention

**Frequency:** Every code-behind file.
**Current handling:** Strips ALL `System.Web.*` usings.

**Proposed solution: Smarter stripping in `bwfc-migrate.ps1`** (Size: **S**)

Instead of blanket removal, check if the file references types that have BWFC shims:
- `HttpContext.Current` → keep `using System.Web;` (if we add GAP-02 shim in that namespace)
- `ConfigurationManager` → keep `using System.Configuration;` (if we add GAP-01 shim)
- All others → strip as today

**Impact:** Reduces false-positive compile errors from overly aggressive using removal.

---

### GAP-10: `BundleConfig` / `RouteConfig` Classes

**Frequency:** Every Web Forms 4.5+ app has these in `App_Start/`.
**Current handling:** None — files ignored.

**Proposed solution: Type alias + no-op stub** (Size: **S**)

```csharp
namespace BlazorWebFormsComponents;

[Obsolete("Bundling handled by Blazor static assets")]
public class BundleCollection { public void Add(object bundle) { } }

[Obsolete("Routing handled by Blazor @page directives")]
public class RouteCollection { public void MapPageRoute(string name, string url, string page) { } }

public static class BundleTable { public static BundleCollection Bundles { get; } = new(); }
public static class RouteTable { public static RouteCollection Routes { get; } = new(); }
```

**Script transform:** Copy `App_Start/*.cs` files with header comment, strip Web Forms usings.

**Impact:** `Application_Start` / `Global.asax.cs` files compile without manual deletion.

---

### GAP-11: `Global.asax.cs` → Program.cs Integration

**Frequency:** Every Web Forms app.
**Current handling:** File is completely ignored.

**Proposed solution: PowerShell extract + inject** (Size: **M**)

Parse `Global.asax.cs` for:
- `Application_Start` → extract registrations → inject into Program.cs
- `Application_Error` → inject `app.UseExceptionHandler()` pattern
- `Session_Start` → inject session middleware registration comment
- Custom route registrations → inject `app.MapGet()` patterns

**Impact:** Captures app-level configuration that's otherwise lost.

---

### GAP-12: `Web.config` `<appSettings>` → `appsettings.json`

**Frequency:** Every Web Forms app.
**Current handling:** Database provider detection only.

**Proposed solution: PowerShell transform in `bwfc-migrate.ps1`** (Size: **S**)

```powershell
# Parse Web.config <appSettings> section
# Generate appsettings.json with matching keys
# Also extract <connectionStrings> into ConnectionStrings section
```

**Impact:** Combined with GAP-01 shim, makes `ConfigurationManager.AppSettings["key"]` actually return values.

---

### GAP-13: `<%# Bind("Prop") %>` → Two-Way Binding

**Frequency:** Common in EditItemTemplate, InsertItemTemplate.
**Current handling:** Not handled — only `Eval()` is converted.

**Proposed solution: PowerShell transform** (Size: **S**)

```
# <%# Bind("PropertyName") %> → @bind-Value="context.PropertyName"
# Context: if inside an attribute value, use @bind-Value
# If standalone, use @context.PropertyName (same as Eval)
```

**Impact:** Edit/insert templates work without manual intervention.

---

### GAP-14: `OnClientClick` → Blazor JS Interop Bridge

**Frequency:** Moderate — buttons with client-side confirmation dialogs.
**Current handling:** Attribute preserved but non-functional.

**Proposed solution: Component parameter + JS interop** (Size: **M**)

BWFC `Button` component could support `OnClientClick` parameter:
```csharp
[Parameter] public string? OnClientClick { get; set; }
// If OnClientClick is set, evaluate JS before firing server click
// Common pattern: OnClientClick="return confirm('Are you sure?')"
```

**Impact:** Confirmation dialogs and simple client-side validation work without changes.

---

### GAP-15: `Visible="false"` → Conditional Rendering

**Frequency:** Very common pattern for show/hide logic.
**Current handling:** Boolean normalized to `Visible="false"`, but code-behind `control.Visible = false` needs help.

**Proposed solution: Already works via BaseWebFormsComponent** — but add **PowerShell transform** for code-behind patterns (Size: **S**):

```
# myControl.Visible = false; → already works (BWFC supports Visible parameter)
# But: FindControl("myControl").Visible = false → needs shim awareness
```

Ensure `FindControl()` returns objects with settable `Visible` property.

---

### GAP-16: `Request.QueryString["key"]` in Code-Behind

**Frequency:** Very common for parameter passing.
**Current handling:** `WebFormsPageBase.Request.QueryString` works, but code-behind often uses `Request.QueryString["key"]` directly.

**Proposed solution: PowerShell transform** (Size: **S**)

```
# Request.QueryString["key"] → already works via RequestShim!
# But: add to GlobalUsings or auto-inject note that this works via WebFormsPageBase
```

Actually, this **already works** for pages inheriting `WebFormsPageBase`. The gap is in non-page classes. Add `Request` property to `HttpHandlerBase` for handler code.

---

### GAP-17: `Server.MapPath()` Outside Handlers

**Frequency:** Moderate — file operations in business logic.
**Current handling:** Only available in `HttpHandlerServer` (handlers only).

**Proposed solution: Static utility shim** (Size: **S**)

```csharp
public static class Server
{
    private static IWebHostEnvironment? _env;
    internal static void Initialize(IWebHostEnvironment env) => _env = env;

    public static string MapPath(string virtualPath)
    {
        var clean = virtualPath.TrimStart('~', '/');
        return Path.Combine(_env?.WebRootPath ?? "", clean);
    }
}
```

**Impact:** File upload, report generation, and other IO code compiles without changes.

---

### GAP-18: `UpdatePanel` Code-Behind API

**Frequency:** Moderate — `UpdatePanel.Update()` calls in code-behind.
**Current handling:** Markup `<UpdatePanel>` → `<UpdatePanel>` (BWFC component). ContentTemplate stripped. But code-behind `UpdatePanel.Update()` method calls break.

**Proposed solution: No-op method on component** (Size: **S**)

Already handled by BWFC component — just ensure `Update()` is a no-op method that exists on the component for code-behind references. Blazor handles re-rendering automatically.

---

### GAP-19: `Literal` Control `.Text` Property in Code-Behind

**Frequency:** Common — `Literal1.Text = "some HTML"`.
**Current handling:** `<Literal>` component exists in BWFC.

**Proposed solution:** Ensure component exposes `Text` as settable `[Parameter]` and confirm code-behind pattern `myLiteral.Text = "value"` works with component references. (Size: **S**)

---

### GAP-20: `.aspx` URL References in Code-Behind Strings

**Frequency:** Very common — `"~/Products.aspx?id=5"`.
**Current handling:** L1 converts `Response.Redirect("~/url.aspx")` but not other string literals containing `.aspx`.

**Proposed solution: PowerShell transform** (Size: **S**)

```
# In code-behind .cs files: "~/path.aspx" → "/path" in string literals
# Careful: only transform inside NavigateTo/Redirect calls and obvious URL patterns
# The AspxRewriteMiddleware handles runtime redirects, but clean URLs are preferred
```

**Impact:** Fewer runtime redirects, cleaner generated code.

---

### GAP-21: `ScriptManager.RegisterStartupScript()` / `RegisterClientScriptBlock()`

**Frequency:** Moderate — injecting JS from code-behind.
**Current handling:** ScriptManager markup is a no-op stub. Code-behind API calls break.

**Proposed solution: Shim methods on WebFormsPageBase** (Size: **M**)

```csharp
protected async Task RegisterStartupScript(string key, string script)
{
    await JSRuntime.InvokeVoidAsync("eval", script);
}
```

Plus: Add `[Inject] IJSRuntime JSRuntime` to `WebFormsPageBase`.

**Impact:** Client-side script injection compiles and partially works.

---

### GAP-22: `App_Start/` Detection and Copy

**Frequency:** Every Web Forms 4.5+ app.
**Current handling:** Directory completely ignored by bwfc-migrate.ps1.

**Proposed solution: PowerShell copy + annotate** (Size: **S**)

Copy `App_Start/*.cs` to output, strip Web Forms usings, add TODO header. With GAP-10 (BundleConfig/RouteConfig stubs), these files will compile.

---

### GAP-23: `App_Themes/` Auto-Copy in L1 Script

**Frequency:** Any themed Web Forms app.
**Current handling:** Theme migration documented in SKILL.md but not automated in bwfc-migrate.ps1.

**Proposed solution: PowerShell transform** (Size: **S**)

```powershell
# Detect App_Themes/ in source → copy to wwwroot/App_Themes/ in output
# AddBlazorWebFormsComponents() already auto-discovers themes at runtime
# Just need the file copy step in the script
```

**Impact:** Theme-enabled apps "just work" after L1 without any manual file copying.

---

## 3. Implementation Complexity & Priority

### Quick Wins (Single PR, ≤ 2 hours each)

| # | Gap | Type | Size | Impact |
|---|-----|------|------|--------|
| 1 | GAP-01 | ConfigurationManager shim + type alias | **S** | 🔥 Highest — every BLL file |
| 2 | GAP-06 | IsPostBack guard unwrapping in script | **S** | Common pattern elimination |
| 3 | GAP-09 | Selective using retention in script | **S** | Fewer false compile errors |
| 4 | GAP-12 | Web.config → appsettings.json in script | **S** | Config values actually resolve |
| 5 | GAP-13 | Bind() → @bind expression in script | **S** | Edit templates work |
| 6 | GAP-22 | App_Start/ copy in script | **S** | Files not lost |
| 7 | GAP-23 | App_Themes/ auto-copy in script | **S** | Themes work end-to-end |
| 8 | GAP-10 | BundleConfig/RouteConfig no-op stubs | **S** | App_Start compiles |
| 9 | GAP-20 | .aspx URL cleanup in code-behind strings | **S** | Cleaner generated code |

### Medium Effort (1-2 days each)

| # | Gap | Type | Size | Impact |
|---|-----|------|------|--------|
| 10 | GAP-04 | Session shim on WebFormsPageBase | **M** | Cart/state apps work |
| 11 | GAP-05 | Page_Load → OnInitializedAsync script transform | **M** | Biggest L2 work reducer |
| 12 | GAP-07 | Event handler signature transform | **M** | Most handlers compilable |
| 13 | GAP-11 | Global.asax.cs → Program.cs extraction | **M** | Captures app config |
| 14 | GAP-14 | OnClientClick JS interop bridge | **M** | Confirmation dialogs work |
| 15 | GAP-21 | RegisterStartupScript shim | **M** | JS injection from code-behind |

### Large Effort (3+ days)

| # | Gap | Type | Size | Impact |
|---|-----|------|------|--------|
| 16 | GAP-02 | HttpContext.Current shim | **S** code / **L** testing | Static utility classes compile |
| 17 | GAP-03 | FormsAuthentication shim | **S** code / **M** testing | Login pages compile at L1 |
| 18 | GAP-08 | Startup.cs/OWIN extraction | **L** | Identity apps scaffold correctly |

---

## 4. Proposed Implementation Order

### Phase 1: "Just Make It Compile" (Sprint)

Ship these together — they target the most common compile errors in L1 output:

1. **GAP-01** — ConfigurationManager shim (blocks almost every BLL file)
2. **GAP-10** — BundleConfig/RouteConfig stubs (blocks App_Start/)
3. **GAP-12** — Web.config → appsettings.json (makes GAP-01 return values)
4. **GAP-09** — Selective using retention (fewer false compile errors)
5. **GAP-22** — App_Start/ copy in script
6. **GAP-23** — App_Themes/ auto-copy in script

**Expected result:** L1 output compiles with ~50% fewer errors.

### Phase 2: "Just Make It Run" (Sprint)

These shims make the app actually functional:

7. **GAP-04** — Session shim (carts, state management)
8. **GAP-05** — Page_Load → OnInitializedAsync transform
9. **GAP-06** — IsPostBack guard unwrapping
10. **GAP-07** — Event handler signature transform
11. **GAP-13** — Bind() expression conversion
12. **GAP-20** — .aspx URL cleanup in strings

**Expected result:** L2 manual work reduced by ~40%.

### Phase 3: "Advanced Shims" (Sprint)

Complex but high-value:

13. **GAP-02** — HttpContext.Current shim
14. **GAP-03** — FormsAuthentication shim
15. **GAP-08** — Startup.cs/OWIN extraction
16. **GAP-11** — Global.asax.cs → Program.cs extraction
17. **GAP-14** — OnClientClick JS interop
18. **GAP-21** — RegisterStartupScript shim

**Expected result:** L1 coverage approaches ~80%.

---

## 5. The "Sneaky Wins" — Maximum Impact, Minimum Code

These are the tricks Jeff asked for — clever one-liners that eliminate disproportionate manual work:

### 🥇 `ConfigurationManager.AppSettings["key"]` (GAP-01)
One static class + one type alias = every BLL file that reads config compiles.

### 🥈 `Session["key"]` Shim (GAP-04)
One property on `WebFormsPageBase` + one shim class = shopping carts work.

### 🥉 `Web.config → appsettings.json` (GAP-12)
~20 lines of PowerShell = config values actually resolve at runtime.

### 🏅 `if (!IsPostBack)` Unwrap (GAP-06)
~10 lines of regex in the script = eliminates the most confusing migration pattern.

### 🏅 `App_Themes/ Auto-Copy` (GAP-23)
~5 lines of PowerShell = themes work end-to-end (SkinFileParser + ThemeProvider already handle the rest).

### 🏅 Type Aliases for Everything (expand .targets)
Add more aliases to `.targets`:
```xml
<Using Alias="UserControl" Include="Microsoft.AspNetCore.Components.ComponentBase" />
<Using Alias="EventArgs" Include="System.EventArgs" />
<Using Alias="HiddenField" Include="BlazorWebFormsComponents.HiddenField" />
```

Every alias we add is one fewer manual rename across the entire codebase.

---

## 6. Metrics to Track

After implementing these changes, re-run WingtipToys and ContosoUniversity migrations to measure:

| Metric | Current | Target |
|--------|---------|--------|
| L1 compile errors | ~30-40 per app | < 10 |
| L2 files needing manual touch | ~55 (WT), ~18 (CU) | < 30 (WT), < 10 (CU) |
| L2 time-to-working | ~3 hrs (WT), ~20 min (CU) | < 1 hr (WT), < 10 min (CU) |
| Acceptance test pass rate | 14/14 (WT), 39/40 (CU) | Maintain 100% / 100% |

---

*Report generated by Bishop — Migration Tooling Dev*
*Based on audit of bwfc-migrate.ps1 (2,714 lines), BWFC library shims, and migration test runs 7 (WT) and 22 (CU).*

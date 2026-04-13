---
name: bwfc-migration
description: "Migrate ASP.NET Web Forms applications to Blazor Server using the webforms-to-blazor CLI tool and BlazorWebFormsComponents (BWFC). Orchestrates L1 automated transforms via CLI, then guides L2 contextual transforms. WHEN: \"migrate aspx\", \"convert web forms\", \"web forms to blazor\", \"run migration\". INVOKES: webforms-to-blazor CLI tool. FOR SINGLE OPERATIONS: use /bwfc-identity-migration for auth, /bwfc-data-migration for EF/architecture."
---

# Web Forms → Blazor Migration with BWFC

## Overview

This skill orchestrates the full migration from ASP.NET Web Forms to Blazor Server using a **three-layer architecture**:

| Layer | Executor | Coverage | Description |
|-------|----------|----------|-------------|
| **L1: Deterministic** | `webforms-to-blazor` CLI tool | ~70% | 27 compiled transforms (16 markup + 11 code-behind), project scaffolding, config migration |
| **L2: Contextual** | Copilot (this skill) | ~15–20% | TODO-driven transforms requiring semantic understanding — session state, lifecycle, data binding |
| **L3: Architectural** | Developer | ~10–15% | Business logic, custom controls, auth flows, architectural decisions |

The CLI tool emits structured `// TODO(bwfc-*)` comments and a JSON migration report. L2 reads that report and applies contextual transforms per TODO category.

**Related skills:**
- `/bwfc-identity-migration` — ASP.NET Identity/Membership → Blazor Identity
- `/bwfc-data-migration` — EF6 → EF Core, DataSource → services, architecture decisions

---

## Prerequisites

- **.NET 10 SDK** or later
- **Install the global tool:**
  ```bash
  dotnet tool install -g Fritz.WebFormsToBlazor
  ```
- **BlazorWebFormsComponents NuGet package** (added automatically by the tool's scaffolding)

---

## 🚨 CRITICAL: USE THE SHIMS — PRIMARY MIGRATION STRATEGY 🚨

**ALWAYS inherit from `WebFormsPageBase` (via `_Imports.razor`) and use the Web Forms shims.** The BlazorWebFormsComponents library provides shims that make Web Forms patterns work AS-IS in Blazor — no manual rewrites needed.

### Web Forms Patterns That Work Via Shims

| Web Forms Pattern | Shim | Works In Interactive Mode? | Notes |
|-------------------|------|---------------------------|-------|
| `Response.Redirect("url")` | ResponseShim | ✅ Yes | Uses NavigationManager internally, strips `~/` and `.aspx` |
| `Request.QueryString["key"]` | RequestShim | ✅ Yes | Parses from NavigationManager.Uri |
| `Request.Cookies["key"]` | RequestShim | ⚠️ SSR only | Returns empty in interactive, logs warning |
| `Request.Form["key"]` | FormShim | ✅ Yes | Via WebFormsForm component in interactive mode |
| `Session["key"]` get/set | SessionShim | ✅ Yes | In-memory ConcurrentDictionary per circuit |
| `Session.Get<T>("key")` | SessionShim | ✅ Yes | Strongly-typed session access |
| `Server.MapPath("~/path")` | ServerShim | ✅ Yes | Maps to web root path |
| `Server.HtmlEncode(text)` | ServerShim | ✅ Yes | HTML encoding helper |
| `Cache["key"]` get/set | CacheShim | ✅ Yes | Backed by IMemoryCache |
| `Page.Title` | WebFormsPageBase | ✅ Yes | Sets page title |
| `Page.IsPostBack` | WebFormsPageBase | ✅ Yes | Always `false` in Blazor (no postbacks) |
| `ClientScript.RegisterStartupScript()` | ClientScriptShim | ✅ Yes | Injects JavaScript via JSRuntime |
| `ViewState["key"]` | WebFormsPageBase | ✅ Yes | In-memory dictionary per component instance |

### Key Benefits of Shims

1. **Minimal Code Changes** — Original Web Forms code works with ZERO changes in most cases
2. **Compile-Time Safety** — Shims provide the same APIs, so existing code compiles unchanged
3. **Interactive Mode Support** — Most shims work in both SSR and Interactive render modes
4. **Drop-In Replacement** — `builder.Services.AddBlazorWebFormsComponents()` registers all shims automatically

### When Shims Are Available via WebFormsPageBase

The `_Imports.razor` file includes `@inherits BlazorWebFormsComponents.WebFormsPageBase`, which gives EVERY migrated page access to:

```csharp
// Available on ALL pages via WebFormsPageBase:
Response.Redirect("/Products");           // ✅ Works
Session["CartId"] = 123;                  // ✅ Works
var param = Request.QueryString["id"];    // ✅ Works
var path = Server.MapPath("~/images");    // ✅ Works
Cache["Products"] = productList;          // ✅ Works
ViewState["SortColumn"] = "Name";         // ✅ Works
ClientScript.RegisterStartupScript(...);  // ✅ Works
```

**NO INJECTION NEEDED.** These properties are available directly in your `@code` block.

---

## ❌ ANTI-PATTERNS: DO NOT DO THESE

**These are WRONG approaches that waste time.** The shims already handle these patterns correctly.

### ❌ Do NOT Inject IHttpContextAccessor

```csharp
// ❌ WRONG — Fighting Blazor's architecture
[Inject] IHttpContextAccessor HttpContextAccessor { get; set; }

var cookies = HttpContextAccessor.HttpContext?.Request.Cookies;
```

**✅ CORRECT — Use the RequestShim:**
```csharp
// ✅ Inherits WebFormsPageBase via _Imports.razor
var cookieValue = Request.Cookies["MyCookie"];
```

### ❌ Do NOT Inject NavigationManager for Redirects

```csharp
// ❌ WRONG — Manual URL manipulation
[Inject] NavigationManager NavigationManager { get; set; }

NavigationManager.NavigateTo("/Products");
```

**✅ CORRECT — Use the ResponseShim:**
```csharp
// ✅ Works exactly like Web Forms
Response.Redirect("~/Products.aspx");  // Strips ~/ and .aspx automatically
```

### ❌ Do NOT Use HttpContext.Response.Cookies Directly

```csharp
// ❌ WRONG — Only works in SSR, breaks in interactive mode
HttpContext.Response.Cookies.Append("CartId", cartId);
```

**✅ CORRECT — Use SessionShim instead:**
```csharp
// ✅ Works in both SSR and interactive modes
Session["CartId"] = cartId;
```

### ❌ Do NOT Create Minimal API Endpoints for Actions

```csharp
// ❌ WRONG — Unnecessary ASP.NET Core endpoints
app.MapPost("/api/AddToCart", async (CartService cart, int productId) => 
{
    await cart.AddItemAsync(productId);
    return Results.Ok();
});
```

**✅ CORRECT — Keep as Blazor page/component methods:**
```csharp
// ✅ Original Web Forms pattern preserved
private async Task AddToCart_Click()
{
    Session["CartId"] = await _cartService.AddItemAsync(productId);
}
```

### ❌ Do NOT Use [ExcludeFromInteractiveRouting] Unless Necessary

```csharp
// ❌ WRONG — Forces SSR-only when shims handle interactive mode
@attribute [ExcludeFromInteractiveRouting]
```

**✅ CORRECT — Let pages run in interactive mode:**
```csharp
// ✅ Shims work in interactive mode — no attribute needed
@page "/Products"
@inherits WebFormsPageBase
```

**ONLY use `[ExcludeFromInteractiveRouting]` if:**
- Page genuinely needs HTTP form POST with `<form method="post">`
- Page requires server-side cookie manipulation
- Page uses 3rd-party libraries that require HttpContext

### ❌ Do NOT Manually Manage State Via Cookies

```csharp
// ❌ WRONG — Reinventing session management
Response.Cookies.Append("CartId", Guid.NewGuid().ToString(), new CookieOptions 
{
    Expires = DateTimeOffset.UtcNow.AddDays(30),
    IsEssential = true
});
```

**✅ CORRECT — If Web Forms used Session, use SessionShim:**
```csharp
// ✅ Original pattern preserved
Session["CartId"] = Guid.NewGuid().ToString();
```

### ❌ Do NOT Add onclick="window.location.href=..." Hacks

```csharp
// ❌ WRONG — JavaScript workarounds for navigation
<Button Text="View Details" 
        OnClientClick="window.location.href='/ProductDetails?id=5'; return false;" />
```

**✅ CORRECT — Use the BWFC Button with ResponseShim:**
```csharp
// ✅ Web Forms pattern works via shim
<Button Text="View Details" OnClick="@ViewDetails_Click" />

@code {
    private void ViewDetails_Click()
    {
        Response.Redirect($"~/ProductDetails.aspx?id={productId}");
    }
}
```

### ❌ Do NOT Fight Blazor's Interactive Router

```csharp
// ❌ WRONG — Trying to force HTTP semantics into Blazor
app.MapFallback("/Products", async context => 
{
    await context.Response.WriteAsync("Use the Blazor router!");
});
```

**✅ CORRECT — Work WITH Blazor using shims:**
```csharp
// ✅ Standard Blazor routing + shims = Web Forms compatibility
@page "/Products"
@inherits WebFormsPageBase

<GridView SelectMethod="GetProducts" />
```

---

## 🌳 Migration Decision Tree

Use this flowchart when encountering Web Forms patterns:

```
Original code uses Response.Redirect()?
  → Use Response.Redirect() — ResponseShim handles it ✅

Original code uses Session["key"]?
  → Use Session["key"] — SessionShim handles it ✅
  
Original code uses Request.QueryString["key"]?
  → Use Request.QueryString["key"] — RequestShim handles it ✅

Original code uses Request.Cookies["key"]?
  → If page runs in interactive mode: Use Session instead (cookies need SSR)
  → If page can be SSR: Request.Cookies works via RequestShim

Original code uses HttpContext.Current.Session?
  → Replace HttpContext.Current.Session with Session property from WebFormsPageBase ✅

Need form POST data?
  → Wrap form in <WebFormsForm>, use Request.Form["key"] ✅

Original code uses Server.MapPath()?
  → Use Server.MapPath() — ServerShim handles it ✅

Original code uses Cache["key"]?
  → Use Cache["key"] — CacheShim handles it ✅

Original code uses ViewState["key"]?
  → Use ViewState["key"] — WebFormsPageBase provides it ✅
  → Consider refactoring to component fields for clarity

Original code uses ClientScript.RegisterStartupScript()?
  → Use ClientScript.RegisterStartupScript() — ClientScriptShim handles it ✅

Need to inject a service?
  → @inject MyService Service — standard Blazor DI ✅
```

### The Golden Rule: **Preserve the Original Pattern**

**If the original Web Forms code uses `Session["CartId"]`, the migrated code should use `Session["CartId"]`.** The SessionShim makes this work. Don't reinvent the pattern — use the shims.

---

## Migration Workflow

### Phase 1: L1 Automated Transforms (CLI)

> ⚠️ **CRITICAL: Always run L1 via the CLI tool. Do NOT apply L1 transforms manually.** The tool produces deterministic, testable output. Manual L1 transforms corrupt measurement and miss edge cases.

#### Full Project Migration

```bash
webforms-to-blazor migrate -i ./MyWebFormsApp -o ./MyBlazorApp --report migration-report.json --verbose
```

| Option | Description |
|--------|-------------|
| `-i, --input <path>` | Source Web Forms project root (required) |
| `-o, --output <path>` | Output Blazor project directory (required) |
| `--report <path>` | Write JSON migration report to file |
| `--report-format <fmt>` | `json` (default) or `markdown` |
| `--skip-scaffold` | Skip `.csproj`, `Program.cs`, `_Imports.razor` generation |
| `--dry-run` | Show transforms without writing files |
| `-v, --verbose` | Detailed per-file transform log |
| `--overwrite` | Overwrite existing files in output directory |

#### Single File Conversion

```bash
webforms-to-blazor convert -i ./Pages/Products.aspx -o ./Pages/ --overwrite
```

| Option | Description |
|--------|-------------|
| `-i, --input <file>` | `.aspx`, `.ascx`, or `.master` file (required) |
| `-o, --output <path>` | Output directory (default: same directory) |
| `--overwrite` | Overwrite existing `.razor` file |

#### What L1 Handles (27 Transforms)

**Markup Transforms (16):**

| # | Transform | Description |
|---|-----------|-------------|
| 1 | PageDirective | `<%@ Page %>` → `@page "/route"` with title extraction |
| 2 | MasterDirective | Remove `<%@ Master %>`, add `@inherits LayoutComponentBase` |
| 3 | ControlDirective | Remove `<%@ Control %>` directives |
| 4 | ImportDirective | `<%@ Import Namespace="X" %>` → `@using X` |
| 5 | RegisterDirective | Remove `<%@ Register %>` tag registrations |
| 6 | ContentWrapper | Strip `<asp:Content>` wrappers, convert HeadContent |
| 7 | FormWrapper | `<form runat="server">` → `<div>` (preserves `id` for CSS) |
| 8 | GetRouteUrl | `Page.GetRouteUrl()` → `GetRouteUrlHelper.GetRouteUrl()` |
| 9 | Expression | `<%: %>` → `@()`, `<%# Item.X %>` → `@context.X`, Eval/Bind conversion |
| 10 | LoginView | Strip attributes, flag RoleGroups for review |
| 11 | SelectMethod | Preserve attribute, add TODO for delegate conversion |
| 12 | AjaxToolkitPrefix | `ajaxToolkit:X` → `X` (runs before asp: prefix) |
| 13 | AspPrefix | `asp:X` → `X` for all server controls |
| 14 | AttributeStrip | Remove `runat="server"`, normalize `ID` → `id` |
| 15 | EventWiring | `OnClick="Handler"` → `OnClick="@Handler"` |
| 16 | UrlReference | `~/path` → `/path` in href, NavigateUrl, ImageUrl |

**Code-Behind Transforms (11):**

| # | Transform | Description |
|---|-----------|-------------|
| 1 | UsingStrip | Remove `System.Web.*`, `Microsoft.AspNet.*` usings |
| 2 | BaseClassStrip | Remove `: Page`, `: System.Web.UI.Page` base classes |
| 3 | ResponseRedirect | ⚠️ **DEPRECATED** — L1 used to transform `Response.Redirect()` → `NavigationManager.NavigateTo()`, but this is WRONG. L2 should revert to `Response.Redirect()` and use ResponseShim. |
| 4 | SessionDetect | Detect `Session["key"]` patterns, inject `// TODO(bwfc-session-state)` guidance |
| 5 | ViewStateDetect | Detect `ViewState["key"]` patterns, inject `// TODO(bwfc-viewstate)` guidance |
| 6 | IsPostBack | Unwrap simple `if (!IsPostBack)` guards; TODO complex guards with `else` |
| 7 | PageLifecycle | `Page_Load` → `OnInitializedAsync`, `Page_Init` → `OnInitialized`, `Page_PreRender` → `OnAfterRenderAsync` |
| 8 | EventHandlerSignature | Strip `(object sender, EventArgs e)` from standard handlers |
| 9 | DataBind | Cross-file: `ctrl.DataSource = x` → field assignment, inject `Items=` in markup |
| 10 | UrlCleanup | `"~/Products.aspx?id=5"` → `"/Products?id=5"` in string literals |
| 11 | AttributeNormalize | Boolean, enum, and unit value normalization |

**Scaffolding:**
- `.csproj` with BWFC NuGet reference
- `Program.cs` with `AddBlazorWebFormsComponents()` — **registers ALL shims automatically** (SessionShim, ResponseShim, RequestShim, ServerShim, CacheShim, ClientScriptShim, FormShim)
- `_Imports.razor` with BWFC usings and `@inherits WebFormsPageBase` — **gives EVERY page access to Session, Response, Request, Server, Cache, ClientScript, ViewState, IsPostBack properties**
- `App.razor` with `InteractiveServer` render mode, detected CSS/JS references
- `Routes.razor`, `GlobalUsings.cs`, `launchSettings.json`
- `appsettings.json` from `web.config` connection strings and app settings
- `WebFormsShims.cs`, `IdentityShims.cs` when applicable
- Copies `App_Start/BundleConfig.cs` and `RouteConfig.cs` as no-op shims

**🔑 Key Point:** The CLI scaffolding sets up the shim infrastructure automatically. You do NOT need to:
- ❌ Manually register shim services in DI
- ❌ Add `[Inject]` attributes for Session, Response, Request, etc.
- ❌ Create custom services for patterns the shims already handle

#### Reading the Migration Report

The `--report` flag generates a JSON file that drives L2 decisions:

```json
{
  "summary": {
    "filesProcessed": 24,
    "transformsApplied": 187,
    "todosGenerated": 12,
    "scaffoldFilesCreated": 8
  },
  "todos": [
    {
      "category": "bwfc-session-state",
      "file": "Cart.razor.cs",
      "line": 15,
      "message": "Session[\"CartId\"] detected — convert to scoped service",
      "severity": "warning"
    },
    {
      "category": "bwfc-identity-migration",
      "file": "Login.razor.cs",
      "line": 8,
      "message": "FormsAuthentication.SignOut() → SignInManager.SignOutAsync()",
      "severity": "warning"
    }
  ],
  "transforms": [ ... ],
  "scaffolding": { ... }
}
```

**TODO categories** map directly to L2 sections below:
- `bwfc-session-state` → Session shim wiring
- `bwfc-identity-migration` → Auth conversion (delegate to `/bwfc-identity-migration`)
- `bwfc-data-migration` → DataSource → service conversion (delegate to `/bwfc-data-migration`)
- `bwfc-viewstate` → ViewState replacement
- `bwfc-page-lifecycle` → Complex lifecycle patterns L1 couldn't auto-convert
- `bwfc-manual` → Items requiring developer decision

---

### Phase 2: L2 Contextual Transforms (Copilot-Assisted)

After L1 completes, read the migration report (`migration-report.json`). For each TODO category, apply the corresponding transforms below.

> ⚠️ **MANDATORY — READ BEFORE STARTING L2:** Open and read **all three** child documents:
> - **[CODE-TRANSFORMS.md](CODE-TRANSFORMS.md)** — Lifecycle mapping, event handlers, data binding, Master Page → Layout
> - **[CONTROL-REFERENCE.md](CONTROL-REFERENCE.md)** — 58 BWFC component translation tables
> - **[AJAX-TOOLKIT.md](AJAX-TOOLKIT.md)** — Ajax Control Toolkit extender migration (14 components)

#### 🔧 First Step: Revert L1's Response.Redirect Transform

**CRITICAL:** L1's ResponseRedirect transform is WRONG. It converts `Response.Redirect()` to `NavigationManager.NavigateTo()`, which breaks the shim pattern.

**L2 must revert this transform:**

```csharp
// L1 output (WRONG):
[Inject] NavigationManager NavigationManager { get; set; }

private void ViewProduct_Click()
{
    NavigationManager.NavigateTo("/Products");
}

// L2 fix (CORRECT):
// Remove the [Inject] NavigationManager line

private void ViewProduct_Click()
{
    Response.Redirect("~/Products.aspx");  // ✅ Shim handles this
}
```

**Search pattern:** Look for `[Inject] NavigationManager` and `NavigationManager.NavigateTo()` calls that originated from Web Forms `Response.Redirect()`.

**Fix:**
1. Remove `[Inject] NavigationManager NavigationManager { get; set; }`
2. Replace `NavigationManager.NavigateTo("/path")` with `Response.Redirect("~/path.aspx")`
3. The ResponseShim will strip `~/` and `.aspx` automatically

#### TODO(bwfc-session-state)

L1 detects `Session["key"]` patterns and inserts guidance comments. **L2 preserves the original pattern** — no code changes needed.

**✅ The Original Pattern Works AS-IS:**

```csharp
// Original Web Forms code:
Session["CartId"] = cartId;
var id = Session["CartId"]?.ToString();

// Migrated Blazor code (IDENTICAL):
Session["CartId"] = cartId;
var id = Session["CartId"]?.ToString();
```

**Why this works:**
1. `_Imports.razor` contains `@inherits WebFormsPageBase`
2. `WebFormsPageBase` provides a `Session` property backed by `SessionShim`
3. `AddBlazorWebFormsComponents()` in `Program.cs` registers `SessionShim` automatically

**DO NOT:**
- ❌ Inject `IHttpContextAccessor` to access `HttpContext.Session`
- ❌ Create a custom session service when `SessionShim` exists
- ❌ Manually manage session state via cookies
- ❌ Change `Session["key"]` to `await SessionStorage.GetAsync("key")` (different pattern)

**DO:**
- ✅ Keep the original `Session["key"]` code unchanged
- ✅ Let `SessionShim` handle the storage (in-memory per circuit)
- ✅ Use `Session.Get<T>("key")` for strongly-typed access if desired

> **Note:** `SessionShim` is an in-memory per-circuit store. It does NOT persist across browser refreshes. For durable state, migrate to a scoped DI service with server-side persistence.

**For non-page components** that need session access, inject `SessionShim` directly:
```csharp
@inject SessionShim Session

@code {
    protected override void OnInitialized()
    {
        var cartId = Session["CartId"]?.ToString();  // ✅ Same pattern
    }
}
```

#### TODO(bwfc-identity-migration)

L1 detects `FormsAuthentication.*`, `Membership.*`, and `Roles.*` calls. These require deep auth migration.

**Quick patterns:**
```csharp
// Before (L1 output with TODO):
// TODO(bwfc-identity-migration): FormsAuthentication.SignOut() → SignInManager.SignOutAsync()
FormsAuthentication.SignOut();

// After (L2):
await SignInManager.SignOutAsync();
```

**For full auth migration**, invoke the `/bwfc-identity-migration` skill — it handles ASP.NET Membership → ASP.NET Core Identity conversion, including database schema migration, cookie configuration, and role-based authorization.

#### TODO(bwfc-data-migration)

L1 removes `DataSourceID` attributes from data-bound controls and replaces `<asp:SqlDataSource>`, `<asp:ObjectDataSource>`, and `<asp:EntityDataSource>` controls with TODO comments.

**Pattern — SqlDataSource → injected service:**
```csharp
// Before (Web Forms):
// <asp:SqlDataSource ID="ProductsDS" SelectCommand="SELECT * FROM Products" />
// <asp:GridView DataSourceID="ProductsDS" />

// After L1:
// TODO(bwfc-data-migration): Replace SqlDataSource "ProductsDS" with injected service
// <GridView />

// After L2:
@inject ProductService ProductService

<GridView ItemType="Product" SelectMethod="@ProductService.GetProducts" />
```

**For full data migration**, invoke the `/bwfc-data-migration` skill — it handles EF6 → EF Core conversion, service extraction, and repository patterns.

#### TODO(bwfc-viewstate)

L1 detects `ViewState["key"]` access patterns but cannot determine replacement strategy without context.

**Pattern — simple value storage → component field:**
```csharp
// Before (L1 output with TODO):
// TODO(bwfc-viewstate): ViewState["SortColumn"] detected — replace with component field or parameter
ViewState["SortColumn"] = "Name";
var sort = ViewState["SortColumn"]?.ToString();

// After (L2):
private string _sortColumn = "Name";
```

**Pattern — cross-page state → cascading parameter or query string:**
```csharp
// Before:
// TODO(bwfc-viewstate): ViewState["SelectedId"] detected
ViewState["SelectedId"] = selectedId;

// After (if needed across navigations):
NavigationManager.NavigateTo($"/Details?id={selectedId}");

// Or (if parent-child component communication):
[CascadingParameter] public int SelectedId { get; set; }
```

**Pattern — ViewStateDictionary shim** (compile-compatibility bridge):
```csharp
// For complex ViewState usage that can't be trivially replaced,
// BWFC's ViewStateDictionary provides a per-component dictionary:
// Code-behind that uses ViewState["key"] compiles unchanged via WebFormsPageBase.
```

#### TODO(bwfc-page-lifecycle)

L1 auto-converts simple lifecycle methods but flags complex patterns it cannot handle:

**Complex `IsPostBack` guards with `else`:**
```csharp
// L1 output (flagged, not unwrapped):
// TODO(bwfc-page-lifecycle): IsPostBack guard with else clause — review manually
if (!IsPostBack)
{
    LoadInitialData();
}
else
{
    ProcessPostBackData();
}

// L2 fix: Move 'if' body to OnInitializedAsync, 'else' body to event handlers
protected override async Task OnInitializedAsync()
{
    await LoadInitialDataAsync();
}

// ProcessPostBackData() logic moves to the specific event handler that triggers it
```

**Page_Load with async operations:**
```csharp
// L1 converts signature but can't determine async boundaries:
protected override async Task OnInitializedAsync()
{
    products = GetProducts();  // TODO(bwfc-page-lifecycle): consider making async
}

// L2 fix:
protected override async Task OnInitializedAsync()
{
    products = await GetProductsAsync();
}
```

**Page_PreRender patterns:**
```csharp
// L1 converts to OnAfterRenderAsync but complex logic needs review:
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        lblCount.Text = products.Count.ToString();
        StateHasChanged();  // Required — OnAfterRenderAsync runs AFTER render
    }
}
```

#### TODO(bwfc-manual)

Items requiring developer decision. Document these and move on:

- Custom `HttpModule` / `HttpHandler` implementations
- Complex `Page_Error` / `Application_Error` patterns
- Dynamic control creation (`Controls.Add(new TextBox())`)
- `Literal.Mode = LiteralMode.PassThrough` with raw HTML injection
- Custom `WebPart` or `WebPartZone` usage
- Third-party control libraries not covered by BWFC

**Action:** Create a `MIGRATION-NOTES.md` file documenting each manual item with context and recommended approach.

#### Data Binding Transforms (applies to all migrated files)

> **⚠️ MANDATORY: SelectMethod MUST be preserved as a delegate.** Do NOT convert to `Items=` binding — this is the #1 recurring migration error.

```csharp
// Before (Web Forms): SelectMethod="GetProducts"
// After (L2): SelectMethod="@productService.GetProducts"
// BWFC's DataBoundComponent.OnAfterRenderAsync calls the delegate to populate Items.
```

**Full L2 checklist for each file:**
- Convert `SelectMethod` string → `SelectHandler<ItemType>` delegate reference
- Preserve `ItemType` attribute (strip namespace prefix only)
- Add `Context="Item"` to `<ItemTemplate>` elements
- Ensure null-safe collection access for `Items`: `Items="@(_products ?? new())"`
- When `SelectMethod` is set, `Items` is auto-populated — do NOT also set `Items`
- Add `@inject` directives for required services (NavigationManager, DbContext, etc.)

---

### Phase 3: Build & Verify

```bash
cd MyBlazorApp
dotnet build
```

**Common build errors and fixes:**

| Error | Cause | Fix |
|-------|-------|-----|
| `CS0246: 'Page' could not be found` | Missing `@inherits WebFormsPageBase` | Verify `_Imports.razor` has `@inherits BlazorWebFormsComponents.WebFormsPageBase` |
| `CS0103: 'Session' does not exist` | Non-page component using Session | Add `@inject SessionShim Session` |
| `CS0103: 'Response' does not exist` | Code-behind using `Response.Redirect` | L1 should have converted; check for missed patterns |
| `CS1061: 'X' does not contain 'DataBind'` | Explicit `.DataBind()` calls remaining | Remove — BWFC auto-binds via `SelectMethod` or `Items` |
| `CS0234: 'Web' does not exist in 'System'` | Remaining `System.Web.*` using | Remove unless it's a BWFC shim namespace (`System.Web.Optimization`, `System.Web.Routing`) |
| `RZ9986: Component attributes do not support complex content` | Expression in attribute without `@()` | Wrap with `@()`: `Value="@(expr)"` |

---

### Phase 4: L3 Developer Tasks

These require human judgment and cannot be automated:

- **Custom controls** — Third-party or custom `WebControl` / `CompositeControl` subclasses need manual Blazor component creation
- **Business logic review** — Verify migrated BLL/DAL behaves correctly with async patterns
- **Authentication flows** — Full auth migration via `/bwfc-identity-migration`
- **Data architecture** — EF6 → EF Core via `/bwfc-data-migration`
- **Performance tuning** — `StateHasChanged()` call optimization, virtualization for large lists
- **Integration testing** — Verify form submissions, navigation, data operations end-to-end

---

## BWFC Configuration Reference

### Project Setup (scaffolded by L1)

**`_Imports.razor`:**
```razor
@using BlazorWebFormsComponents
@using BlazorWebFormsComponents.Enums
@using static Microsoft.AspNetCore.Components.Web.RenderMode
@inherits BlazorWebFormsComponents.WebFormsPageBase
```

The `@inherits` line gives every page `Page.Title`, `Page.MetaDescription`, `IsPostBack`, `Session`, `Server`, `Response`, `Request`, `Cache`, `ViewState`, `ClientScript`, `PostBack` event, `ResolveUrl()`, and `GetRouteUrl()` — so Web Forms code-behind compiles unchanged.

> **Note:** `@rendermode InteractiveServer` is a directive attribute for component instances, NOT a standalone line in `_Imports.razor`.

**`Program.cs`:**
```csharp
builder.Services.AddBlazorWebFormsComponents();

var app = builder.Build();
app.UseConfigurationManagerShim();
```

**`App.razor`** — render mode and BWFC script:
```razor
<HeadOutlet @rendermode="InteractiveServer" />
<Routes @rendermode="InteractiveServer" />
<script src="_content/Fritz.BlazorWebFormsComponents/js/Basepage.js"></script>
```

**Layout** (`MainLayout.razor`):
```razor
@inherits LayoutComponentBase

<BlazorWebFormsComponents.Page />

<header><!-- ... --></header>
<main>@Body</main>
```

> **Important:** `WebFormsPageBase` provides the code-behind API. The `<BlazorWebFormsComponents.Page />` component renders `<PageTitle>` and `<meta>` tags. Both are required.

### Available Shims

| Shim | Web Forms API | Blazor Implementation | Setup |
|------|--------------|----------------------|-------|
| **ConfigurationManager** | `ConfigurationManager.AppSettings["key"]`, `.ConnectionStrings["name"]` | Reads from `IConfiguration` | `app.UseConfigurationManagerShim()` |
| **SessionShim** | `Session["key"]` indexer, `.Get<T>()`, `.Remove()`, `.Clear()`, `.ContainsKey()` | In-memory per-circuit + optional `ISession` sync | Auto-registered by `AddBlazorWebFormsComponents()` |
| **ServerShim** | `Server.MapPath()`, `Server.HtmlEncode()`, `Server.HtmlDecode()`, `Server.UrlEncode()`, `Server.UrlDecode()` | Wraps `IWebHostEnvironment` + `WebUtility` | Auto-registered by `AddBlazorWebFormsComponents()` |
| **CacheShim** | `Cache["key"]` indexer, `Cache.Insert()`, `Cache.Get<T>()`, `Cache.Remove()` | Wraps `IMemoryCache` with absolute/sliding expiration | Auto-registered by `AddBlazorWebFormsComponents()` |
| **ResponseShim** | `Response.Redirect()`, `Response.Cookies` | Wraps `NavigationManager` + `HttpContext`; auto-strips `~/` and `.aspx` | Via `WebFormsPageBase.Response` |
| **RequestShim** | `Request.QueryString`, `Request.Cookies`, `Request.Url`, `Request.Form` | Wraps `NavigationManager` + `HttpContext`; Form via `FormShim` | Via `WebFormsPageBase.Request` |
| **FormShim** | `Request.Form["key"]`, `.GetValues()`, `.AllKeys`, `.Count`, `.ContainsKey()` | Wraps `IFormCollection` (SSR) or JS interop data (interactive) | Via `RequestShim.Form` — populated by `<WebFormsForm>` |
| **ClientScriptShim** | `Page.ClientScript.RegisterStartupScript()`, `.RegisterClientScriptBlock()`, `.RegisterClientScriptInclude()`, `.GetPostBackEventReference()` | Queues scripts, flushes via `IJSRuntime` in `OnAfterRenderAsync` | Auto-registered by `AddBlazorWebFormsComponents()` |
| **ScriptManagerShim** | `ScriptManager.GetCurrent(page)`, `.RegisterStartupScript()`, `.RegisterClientScriptBlock()`, `.RegisterClientScriptInclude()` | Delegates to `ClientScriptShim` | Auto-registered by `AddBlazorWebFormsComponents()` |
| **ViewStateDictionary** | `ViewState["key"]` indexer | Per-component in-memory dictionary | Via `WebFormsPageBase.ViewState` |
| **BundleConfig/RouteConfig** | `BundleTable.Bundles.Add()`, `RouteTable.Routes.MapPageRoute()` | No-op stubs | Compile-only — no setup needed |

### WebFormsForm Component (Form POST Migration)

The `<WebFormsForm>` component enables `Request.Form["key"]` access in interactive Blazor Server mode where `HttpContext` and `IFormCollection` are unavailable. It captures form data via JS interop and feeds it to `RequestShim.Form`.

**Before (Web Forms):**
```html
<form runat="server">
    <asp:TextBox ID="txtName" runat="server" />
    <asp:Button Text="Submit" OnClick="Submit_Click" runat="server" />
</form>

// Code-behind:
protected void Submit_Click(object sender, EventArgs e)
{
    var name = Request.Form["txtName"];
}
```

**After (Blazor with BWFC):**
```razor
<WebFormsForm OnSubmit="SetRequestFormData">
    <TextBox @bind-Text="name" />
    <Button Text="Submit" OnClick="Submit_Click" />
</WebFormsForm>

@code {
    private string name;

    private void Submit_Click()
    {
        // Request.Form["txtName"] works via FormShim
        var formName = Request.Form["txtName"];
    }
}
```

**Key points:**
- `<WebFormsForm>` renders a standard `<form>` element
- In interactive mode, `OnSubmit` captures form data via JS interop and populates `Request.Form`
- Bind `OnSubmit="SetRequestFormData"` to auto-wire form data into `WebFormsPageBase.Request.Form`
- Supports `Method` (Get/Post) and `Action` parameters
- SSR mode uses native `IFormCollection` — no JS interop needed

**When to use `<WebFormsForm>` vs native Blazor forms:**
- Use `<WebFormsForm>` when migrated code-behind accesses `Request.Form["key"]` directly
- Use `<EditForm>` for new Blazor forms with model binding
- Use `<form method="post" action="/endpoint">` for auth operations (see identity migration skill)

### ClientScript Migration (Shim-Based)

`ClientScriptShim` provides a compile-compatible bridge for `Page.ClientScript` patterns. It queues scripts during the component lifecycle and flushes them via `IJSRuntime` after render.

**Before (Web Forms):**
```csharp
Page.ClientScript.RegisterStartupScript(GetType(), "init",
    "alert('Page loaded!');", addScriptTags: true);

Page.ClientScript.RegisterClientScriptInclude("jquery",
    "~/Scripts/jquery.min.js");

if (!Page.ClientScript.IsStartupScriptRegistered(GetType(), "init"))
{
    Page.ClientScript.RegisterStartupScript(GetType(), "init", "doInit();", true);
}
```

**After (Blazor with BWFC — via `WebFormsPageBase.ClientScript`):**
```csharp
// Code-behind compiles unchanged — ClientScript is a property on WebFormsPageBase
ClientScript.RegisterStartupScript(GetType(), "init",
    "alert('Page loaded!');", addScriptTags: true);

ClientScript.RegisterClientScriptInclude("jquery",
    "/Scripts/jquery.min.js");

if (!ClientScript.IsStartupScriptRegistered(GetType(), "init"))
{
    ClientScript.RegisterStartupScript(GetType(), "init", "doInit();", true);
}
```

**ScriptManager code-behind also works:**
```csharp
// Before (Web Forms):
var sm = ScriptManager.GetCurrent(this.Page);
sm.RegisterStartupScript(this, GetType(), "key", "doWork();", true);

// After (Blazor — via ScriptManagerShim):
var sm = ScriptManagerShim.GetCurrent(this);
sm.RegisterStartupScript(this, GetType(), "key", "doWork();", true);
```

**When to use shim vs. native IJSRuntime:**
- **Use shim** for Phase 1 migration — existing `Page.ClientScript` code compiles unchanged
- **Use IJSRuntime** for new Blazor code or Phase 3 cleanup — cleaner, more idiomatic
- The shim internally uses `IJSRuntime` — no performance difference

### PostBack Event Handling

`WebFormsPageBase` provides PostBack compatibility via JS interop. The `__doPostBack()` JavaScript function is auto-bootstrapped and routes events back to the Blazor component.

**Before (Web Forms):**
```csharp
// IPostBackEventHandler implementation
public void RaisePostBackEvent(string eventArgument)
{
    // Handle postback with argument
    ProcessAction(eventArgument);
}

// Client-side trigger
Page.ClientScript.GetPostBackEventReference(this, "delete:42");
```

**After (Blazor with BWFC):**
```csharp
@inherits WebFormsPageBase

@code {
    protected override void OnInitialized()
    {
        PostBack += OnPostBack;
    }

    private void OnPostBack(object sender, PostBackEventArgs e)
    {
        // e.EventTarget = control ID, e.EventArgument = "delete:42"
        ProcessAction(e.EventArgument);
    }
}
```

**PostBack API surface on `WebFormsPageBase`:**
- `event EventHandler<PostBackEventArgs> PostBack` — raised when `__doPostBack()` fires
- `ClientScript.GetPostBackEventReference(control, argument)` — returns JS expression string
- `ClientScript.GetPostBackClientHyperlink(control, argument)` — returns `javascript:__doPostBack(...)` URL
- `ClientScript.GetCallbackEventReference(...)` — returns `__bwfc_callback(...)` expression
- `HandlePostBackFromJs(eventTarget, eventArgument)` — `[JSInvokable]` bridge method
- `HandleCallbackFromJs(eventTarget, eventArgument)` — `[JSInvokable]` callback bridge (override in derived pages)

**appsettings.json mapping** (from `web.config`):
```json
{
  "AppSettings": {
    "SiteName": "My Store",
    "ItemsPerPage": "20"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MyDb;Trusted_Connection=True;"
  }
}
```

### Component Reference

See **[CONTROL-REFERENCE.md](CONTROL-REFERENCE.md)** for the full translation table of 58 BWFC components across 6 categories.

---

## Common Patterns

### Expression Conversion

| Web Forms Expression | Blazor Equivalent | Notes |
|---------------------|-------------------|-------|
| `<%: expression %>` | `@(expression)` | HTML-encoded output |
| `<%= expression %>` | `@(expression)` | Blazor always encodes |
| `<%# Item.Property %>` | `@context.Property` | Inside data-bound templates |
| `<%#: Item.Property %>` | `@context.Property` | Same — Blazor always encodes |
| `<%# Eval("Property") %>` | `@context.Property` | Direct property access |
| `<%# Bind("Property") %>` | `@bind-Value="context.Property"` | Two-way binding |
| `<%$ RouteValue:id %>` | `@Id` (with `[Parameter]`) | Route parameters |
| `<%-- comment --%>` | `@* comment *@` | Razor comments |
| `<% if (cond) { %>` | `@if (cond) {` | Control flow |
| `<% foreach (var x in items) { %>` | `@foreach (var x in items) {` | Loops |

### File Conversion

| Web Forms | Blazor |
|-----------|--------|
| `MyPage.aspx` + `.aspx.cs` | `MyPage.razor` + `.razor.cs` |
| `MyControl.ascx` + `.ascx.cs` | `MyControl.razor` + `.razor.cs` |
| `Site.Master` + `.Master.cs` | `MainLayout.razor` + `.razor.cs` |

### Directive Conversion

| Web Forms Directive | Blazor Equivalent |
|--------------------|-------------------|
| `<%@ Page Title="X" ... %>` | `@page "/route"` |
| `<%@ Master ... %>` | (remove — layouts don't need directives) |
| `<%@ Control ... %>` | (remove — components don't need directives) |
| `<%@ Register TagPrefix="uc" Src="~/X.ascx" %>` | `@using MyApp.Components` |
| `<%@ Import Namespace="X" %>` | `@using X` |

**Drop entirely:** `AutoEventWireup`, `CodeBehind`, `Inherits`, `EnableViewState`, `MasterPageFile`, `ValidateRequest`, `ClientIDMode`, `EnableTheming`, `SkinID`

### Content/Layout Conversion

| Web Forms | Blazor |
|-----------|--------|
| `<asp:Content ContentPlaceHolderID="MainContent">` | (remove — page body IS the content) |
| `<asp:Content ContentPlaceHolderID="HeadContent">` | `<HeadContent>` ... `</HeadContent>` |
| `<asp:ContentPlaceHolder ID="MainContent" />` | `@Body` (in layout) |

### Route URL Conversion

| Web Forms | Blazor |
|-----------|--------|
| `href="~/Products"` | `href="/Products"` |
| `NavigateUrl="~/Products/<%: Item.ID %>"` | `NavigateUrl="@($"/Products/{context.ID}")"` |
| `GetRouteUrl("Route", new { id = Item.ID })` | `@($"/Products/{context.ID}")` or `GetRouteUrlHelper` |
| `Response.Redirect("~/Products")` | `NavigationManager.NavigateTo("/Products")` |

### Master Page → Blazor Layout

```razor
@* Before: <%@ Master Language="C#" CodeBehind="Site.master.cs" %> *@
@* After: *@
@inherits LayoutComponentBase

<BlazorWebFormsComponents.Page />

<header>
    <nav><Menu ... /></nav>
</header>
<main>@Body</main>
<footer>© @DateTime.Now.Year</footer>
```

**Key changes:**
- `<form runat="server">` → `<div>` (preserves CSS block formatting context)
- `<asp:ContentPlaceHolder ID="MainContent">` → `@Body`
- `<asp:ScriptManager>` → `<ScriptManager />` (renders nothing)
- CSS `<link>` from master `<head>` → `App.razor` `<head>` section

> **Tip:** Use `<WebFormsPage>@Body</WebFormsPage>` as the layout wrapper for NamingContainer, theming, and head rendering in one component.

---

## Reference Documents

- **[CONTROL-REFERENCE.md](CONTROL-REFERENCE.md)** — 58 component translation tables, structural components, theming, custom control base classes
- **[CODE-TRANSFORMS.md](CODE-TRANSFORMS.md)** — Lifecycle mapping, event handlers, data binding, navigation, Master Page → Layout
- **[AJAX-TOOLKIT.md](AJAX-TOOLKIT.md)** — Ajax Control Toolkit extender migration (14 components)

---

## Common Gotchas

### No ViewState
Replace `ViewState["key"]` with component fields. `ViewStateDictionary` shim available for compile-compat.

### PostBack Compatibility
`WebFormsPageBase.IsPostBack` works correctly: returns `false` for SSR GET / interactive first render, `true` for SSR POST / interactive subsequent renders. L1 auto-unwraps simple `if (!IsPostBack)` guards. Complex guards (with `else`) get TODO comments. For `__doPostBack()` JavaScript patterns, subscribe to the `PostBack` event on `WebFormsPageBase` — see [PostBack Event Handling](#postback-event-handling) above.

### No DataSource Controls
`SqlDataSource`, `ObjectDataSource`, `EntityDataSource` → injected services. See `/bwfc-data-migration`.

### ID Rendering
Blazor doesn't render component IDs. Use `CssClass` or explicit `id` attributes for CSS/JS targeting.

### Template Context Variable
Add `Context="Item"` on template elements:
```razor
<ItemTemplate Context="Item">
    @Item.PropertyName
</ItemTemplate>
```

### Event Handler Signatures
```csharp
// Web Forms: protected void Btn_Click(object sender, EventArgs e) { }
// Blazor:    private void Btn_Click() { }
```
L1 auto-strips standard `EventArgs`. Specialized types (`CommandEventArgs`, etc.) are preserved.

### `TextMode="MultiLine"` Casing
BWFC uses `Multiline` (lowercase 'l'), not `MultiLine`. Silent failure if wrong.

### ScriptManager/ScriptManagerProxy
`ScriptManager` and `ScriptManagerProxy` Razor components are no-op stubs (render nothing). For code-behind patterns like `ScriptManager.GetCurrent(page).RegisterStartupScript(...)`, use `ScriptManagerShim.GetCurrent(this)` which delegates to `ClientScriptShim`. Include the Razor components during migration to prevent markup errors; remove when stable.

### `runat="server"` on HTML Elements
L1 removes these. Use `@ref` if programmatic access is needed.

---

## Troubleshooting

### L1 Tool Issues

| Problem | Solution |
|---------|----------|
| `webforms-to-blazor` not found | Run `dotnet tool install -g Fritz.WebFormsToBlazor` |
| Tool version mismatch | Run `dotnet tool update -g Fritz.WebFormsToBlazor` |
| Output directory not empty | Use `--overwrite` flag |
| Need to preview changes first | Use `--dry-run` flag |
| Missing scaffolding files | Don't use `--skip-scaffold` unless you have an existing Blazor project |

### L2 Common Issues

| Problem | Solution |
|---------|----------|
| `SelectMethod` not firing | Ensure it's a delegate reference (`@service.Method`), not a string |
| `Items` always empty | Check that `SelectMethod` signature matches `SelectHandler<T>` delegate |
| Template binding errors | Add `Context="Item"` to `<ItemTemplate>` elements |
| Session data lost on refresh | `SessionShim` is per-circuit; use persistent storage for critical data |
| Infinite render loop | Guard `OnAfterRenderAsync` with `if (firstRender)`, call `StateHasChanged()` only when needed |

---

## Per-Page Migration Checklist

```markdown
## Page: [PageName.aspx] → [PageName.razor]

### L1 — CLI Tool (automated)
- [ ] `webforms-to-blazor migrate` or `convert` executed
- [ ] Migration report reviewed
- [ ] File renamed (.aspx → .razor)
- [ ] Directives converted
- [ ] asp: prefixes removed
- [ ] runat="server" removed
- [ ] Expressions converted
- [ ] URLs converted
- [ ] Content wrappers removed
- [ ] IsPostBack guards unwrapped/TODO'd
- [ ] .aspx URL literals cleaned up

### L2 — Copilot Transforms (per TODO category)
- [ ] TODO(bwfc-session-state) items resolved
- [ ] TODO(bwfc-viewstate) items resolved
- [ ] TODO(bwfc-page-lifecycle) items resolved
- [ ] TODO(bwfc-data-migration) items resolved or delegated
- [ ] TODO(bwfc-identity-migration) items resolved or delegated
- [ ] TODO(bwfc-manual) items documented
- [ ] SelectMethod string → SelectHandler delegate
- [ ] Template Context="Item" verified
- [ ] @inject directives added

### Verification
- [ ] `dotnet build` succeeds
- [ ] Page renders correctly
- [ ] Interactive features work
- [ ] No browser console errors
```

---


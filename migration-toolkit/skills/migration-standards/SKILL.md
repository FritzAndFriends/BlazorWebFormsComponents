---
name: "migration-standards"
description: "Enforce canonical migration standards for ASP.NET Web Forms to Blazor using BWFC. Covers target architecture (.NET 10, Server Interactive), database provider detection, event handler preservation, SelectMethod patterns, and page lifecycle mapping. WHEN: \"migration standards\", \"target architecture\", \"render mode placement\", \"page base class\", \"Layer 1 vs Layer 2\"."
domain: "migration"
confidence: "medium"
source: "earned"
---

## ⛔ MANDATORY RULES — Read Before Any Migration Work

1. **ALL migrated pages inherit WebFormsPageBase** via `@inherits BlazorWebFormsComponents.WebFormsPageBase` in `_Imports.razor`. This gives every page: Session, Response, Request, Server, Cache, ClientScript, ViewState, IsPostBack.

2. **PRESERVE the original Web Forms API calls.** If the Web Forms code says `Session["CartId"]`, the Blazor code says `Session["CartId"]`. If it says `Response.Redirect("ShoppingCart.aspx")`, the Blazor code says `Response.Redirect("ShoppingCart")`. The shims handle the translation.

3. **NEVER inject raw ASP.NET Core services** when a shim exists:
   - ❌ `[Inject] IHttpContextAccessor` → ✅ Use `Request`, `Response`, `Session` properties
   - ❌ `[Inject] NavigationManager` (for redirects) → ✅ Use `Response.Redirect()`
   - ❌ `[Inject] IMemoryCache` → ✅ Use `Cache` property
   - ❌ `HttpContext.Session["key"]` → ✅ Use `Session["key"]`

4. **NEVER create Minimal API endpoints** to replace Web Forms page actions. If it was a page in Web Forms, it's a page in Blazor.

5. **NEVER use cookies to replace Session.** If the original code used Session, use SessionShim.

6. **`AddBlazorWebFormsComponents()`** in Program.cs registers ALL shims automatically.

## Context

When migrating an ASP.NET Web Forms application to Blazor using BlazorWebFormsComponents, these standards define the canonical target architecture, tooling choices, and migration patterns. Established through five WingtipToys migration benchmark runs and codified as a directive by Jeffrey T. Fritz.

Apply these standards to:
- Migration script (`bwfc-migrate.ps1`) enhancements
- Copilot-assisted Layer 2 work
- Migration documentation and checklists
- Any new migration test runs

## Patterns

### Target Architecture

| Setting | Standard |
|---|---|
| Framework | **.NET 10** (or latest LTS/.NET preview) |
| Project template | `dotnet new blazor --interactivity Server` |
| Render mode | Global Server Interactive (see [Render Mode Placement](#render-mode-placement) below) |
| Base class | `WebFormsPageBase` for pages (`@inherits` in `_Imports.razor`); `ComponentBase` for non-page components |
| Layout | `MainLayout.razor` with `@inherits LayoutComponentBase` and `@Body` |

### Render Mode Placement

> **`@rendermode` is a directive *attribute*, not a standalone directive.** It goes on component instances in markup, not in `_Imports.razor`.

**`_Imports.razor`** — add the static using so you can write `InteractiveServer` instead of `RenderMode.InteractiveServer`:

```razor
@using static Microsoft.AspNetCore.Components.Web.RenderMode
```

**`App.razor`** — apply render mode to the top-level routable components:

```razor
<HeadOutlet @rendermode="InteractiveServer" />
...
<Routes @rendermode="InteractiveServer" />
```

This gives every page global server interactivity. Do **not** place `@rendermode InteractiveServer` as a line in `_Imports.razor` — it is not a valid Razor directive and will cause build errors (RZ10003, CS0103, RZ10024).

> **Reference:** [ASP.NET Core Blazor render modes](https://learn.microsoft.com/aspnet/core/blazor/components/render-modes)

### Page Base Class

`WebFormsPageBase` eliminates per-page boilerplate when migrating Web Forms code-behind. Instead of injecting `IPageService` into every page, a single `@inherits` directive in `_Imports.razor` gives all pages access to familiar Web Forms properties.

**One-time setup:**

1. **`_Imports.razor`** — add the base class directive:

```razor
@inherits BlazorWebFormsComponents.WebFormsPageBase
```

2. **Layout (`MainLayout.razor`)** — add the Page render component (renders `<PageTitle>` and `<meta>` tags):

```razor
<BlazorWebFormsComponents.Page />
```

**Properties available on every page:**

| Property | Behavior |
|---|---|
| `Title` | Delegates to `IPageService.Title` — `Page.Title = "X"` works unchanged |
| `MetaDescription` | Delegates to `IPageService.MetaDescription` |
| `MetaKeywords` | Delegates to `IPageService.MetaKeywords` |
| `IsPostBack` | SSR GET → `false`, SSR POST → `true`, Interactive first render → `false`, subsequent → `true` |
| `Page` | Returns `this` — enables `Page.Title = "X"` dot syntax |
| `Request` | `RequestShim` — `Request.QueryString`, `Request.Cookies`, `Request.Url`, `Request.Form` |
| `Response` | `ResponseShim` — `Response.Redirect()` (auto-strips `~/` and `.aspx`), `Response.Cookies` |
| `Server` | `ServerShim` — `Server.MapPath()`, `Server.HtmlEncode()`, `Server.UrlEncode()` |
| `Session` | `SessionShim` — `Session["key"]` indexer with `ISession` sync + in-memory fallback |
| `Cache` | `CacheShim` — `Cache["key"]` indexer, `Cache.Insert()` with expiration, `Cache.Remove()` |
| `ViewState` | `ViewStateDictionary` — per-component `ViewState["key"]` indexer |
| `ClientScript` | `ClientScriptShim` — `RegisterStartupScript()`, `RegisterClientScriptBlock()`, `GetPostBackEventReference()` |
| `PostBack` event | `EventHandler<PostBackEventArgs>` — raised when `__doPostBack()` fires from JavaScript |
| `ResolveUrl()` | Strips `~/` prefix and `.aspx` extensions |
| `GetRouteUrl()` | Route URL generation via `LinkGenerator` |
| `IsHttpContextAvailable` | Guards code that requires HTTP-level features (cookies, headers) |

**What is NOT provided (forces proper Blazor migration):**

- `Page.Application` — use singleton DI services
- `Page.Trace` — use `ILogger<T>`

**When to still use `@inject IPageService`:** Non-page components (e.g., a shared header or sidebar) that need access to page metadata should inject `IPageService` directly. `WebFormsPageBase` only applies to routable pages.

### Database Migration

- **Always** migrate EF6 → EF Core using the **latest .NET 10 packages** (currently **10.0.3**)
- Required packages: `Microsoft.EntityFrameworkCore` (10.0.3), `.SqlServer` (or provider matching original app), `.Tools`, `.Design`
- **⚠️ CRITICAL: Detect and match the original database provider.** Examine the source project's `Web.config` `<connectionStrings>` to identify the database provider (`System.Data.SqlClient` → SqlServer, `System.Data.SQLite` → Sqlite, `Npgsql` → PostgreSQL). Install the matching EF Core provider package (`Microsoft.EntityFrameworkCore.SqlServer`, `.Sqlite`, `Npgsql.EntityFrameworkCore.PostgreSQL`, etc.). The L1 script auto-detects this — verify its detection in the [DatabaseProvider] review item. NEVER substitute a different provider than what the original application used.
- Replace `DropCreateDatabaseIfModelChanges` with `EnsureCreated` + idempotent seed
- Use `IDbContextFactory<T>` or scoped `DbContext` injection
- Models: nullable reference types, file-scoped namespaces, modern init patterns

### Identity Migration

- When ASP.NET Identity is present → prefer **ASP.NET Core Identity**
- OWIN middleware → ASP.NET Core middleware pipeline
- Postback-based auth → HTTP endpoints + cookie auth
- Use `dotnet aspnet-codegenerator identity` for scaffolding
- `SignInManager` / `UserManager` APIs change — full subsystem replacement

### Event Handler Strategy

> **Phase 2 signature cleanup:** Standard `EventArgs` parameters are stripped (they carry no data). Specialized EventArgs types (`CommandEventArgs`, `GridViewEditEventArgs`, etc.) are preserved and mapped to their BWFC equivalents. See `bwfc-migration` skill CODE-TRANSFORMS.md for the full decision table.

BWFC components already expose EventCallback parameters with **matching Web Forms names**:

| Web Forms | BWFC | Action |
|---|---|---|
| `OnClick="Handler"` | `OnClick` (EventCallback<MouseEventArgs>) | **Preserve attribute verbatim** — only update handler signature |
| `OnCommand="Handler"` | `OnCommand` (EventCallback<CommandEventArgs>) | Preserve, update signature |
| `OnSelectedIndexChanged="Handler"` | `OnSelectedIndexChanged` (EventCallback<ChangeEventArgs>) | Preserve, update signature |
| `OnTextChanged="Handler"` | `OnTextChanged` (EventCallback<ChangeEventArgs>) | Preserve, update signature |
| `OnCheckedChanged="Handler"` | `OnCheckedChanged` (EventCallback<ChangeEventArgs>) | Preserve, update signature |

**Signature change pattern:**
```csharp
// Web Forms
protected void Button1_Click(object sender, EventArgs e) { ... }

// Blazor (BWFC)
private void Button1_Click(MouseEventArgs e) { ... }
// or
private async Task Button1_Click(MouseEventArgs e) { ... }
```

The script should preserve the attribute and annotate the signature change needed.

### Data Control Strategy — Prefer BWFC Over Raw HTML

| Web Forms Control | BWFC Component | Use Instead Of |
|---|---|---|
| `<asp:ListView>` | `<ListView Items="@data">` with `ItemTemplate` | `@foreach` + HTML table |
| `<asp:GridView>` | `<GridView Items="@data">` with columns | `@foreach` + `<table>` |
| `<asp:FormView>` | `<FormView Items="@data">` with `ItemTemplate` | Direct HTML rendering |
| `<asp:Repeater>` | `<Repeater Items="@data">` with `ItemTemplate` | `@foreach` loops |
| `<asp:DetailsView>` | `<DetailsView Items="@data">` with fields | Manual field rendering |
| `<asp:DataList>` | `<DataList Items="@data">` with `ItemTemplate` | `@foreach` + grid HTML |

**SelectMethod PRESERVED:** BWFC's `DataBoundComponent<ItemType>` has a native `SelectMethod` parameter of type `SelectHandler<ItemType>` (delegate signature: `(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount) → IQueryable<ItemType>`). Convert the Web Forms string method name to a delegate reference: `SelectMethod="@productService.GetProducts"` (if the service method signature matches) or use explicit lambda wiring: `SelectMethod="@((maxRows, startRow, sort, out total) => service.GetProducts(maxRows, startRow, sort, out total))"`. When `SelectMethod` is set, `DataBoundComponent.OnAfterRenderAsync` automatically calls it to populate `Items`.

> **⚠️ DO NOT convert SelectMethod to Items= binding.** When the original Web Forms markup uses `SelectMethod`, the migrated Blazor markup MUST preserve `SelectMethod` as a delegate reference. Converting to `Items=` loses the native BWFC data-binding pattern and defeats the purpose of drop-in replacement. The ONLY acceptable alternative is when the original Web Forms markup used `DataSource` (not `SelectMethod`), in which case `Items=` is correct.

### Session State — SessionShim Is the Solution

- **SessionShim is the PRIMARY solution for migrating Session state.** It's auto-registered by `AddBlazorWebFormsComponents()` — `Session["key"]` code-behind works unchanged via `WebFormsPageBase.Session`. Backed by ASP.NET Core `ISession` in SSR mode; falls back to in-memory `ConcurrentDictionary` in interactive mode.
- **API surface:** `Session["key"]` indexer (get/set), `Session.Get<T>(key)`, `Session.Remove(key)`, `Session.Clear()`, `Session.ContainsKey(key)`, `Session.Count`
- **When to upgrade:** Only replace `Session["key"]` with a scoped DI service when you need state persistence across browser sessions (e.g., server restarts, distributed servers, or cross-device access). For most migrations, SessionShim is sufficient.
- **If upgrading to scoped services:** Use `IHttpContextAccessor` for cookie-based persistence when needed. Register in `Program.cs` with `builder.Services.AddScoped<TService>()`. Example: `Session["CartId"]` → `CartStateService` with cookie-based cart ID.

**Example: SessionShim in action**

```csharp
// Web Forms
protected void AddToCart_Click(object sender, EventArgs e)
{
    var cartId = (string)Session["CartId"];
    if (string.IsNullOrEmpty(cartId))
    {
        cartId = Guid.NewGuid().ToString();
        Session["CartId"] = cartId;
    }
    // ... use cartId
}

// Blazor with SessionShim — IDENTICAL API
private async Task AddToCart_Click(MouseEventArgs e)
{
    var cartId = (string)Session["CartId"];
    if (string.IsNullOrEmpty(cartId))
    {
        cartId = Guid.NewGuid().ToString();
        Session["CartId"] = cartId;
    }
    // ... use cartId
}
```

### Blazor Enhanced Navigation

When linking to minimal API endpoints from Blazor pages, use `<form method="post">` or add `data-enhance-nav="false"` to prevent Blazor's enhanced navigation from intercepting the request. Enhanced navigation handles `<a href>` clicks as client-side SPA navigation, which breaks links to server endpoints (the request never reaches the server). This applies to all auth endpoints, cart operations, file downloads, and any other minimal API routes.

### TextBox Binding Timing for Playwright Tests

BWFC `TextBox` uses `@onchange` (fires on blur), **not** `@oninput` (fires on keystroke). This affects Playwright test interactions:

- Playwright `FillAsync()` triggers `input` events, but the Blazor binding value is **NOT committed** until the element loses focus
- The change event fires on blur, updating the bound property
- Tests that fill fields and immediately submit may fail if the binding hasn't updated yet

**Recommended Playwright pattern for form submissions:**

```csharp
// Fill the last field
await lastField.FillAsync("value");

// Trigger blur to commit the binding
await lastField.BlurAsync();

// Wait for binding propagation
await Task.Delay(200);

// Now click submit — the value is committed
await submitButton.ClickAsync();
```

**Alternative using keyboard navigation:**

```csharp
// Fill fields
await field1.FillAsync("value1");
await field2.FillAsync("value2");

// Press Tab after the last field to trigger blur
await lastField.PressAsync("Tab");

// Small delay for binding update
await Task.Delay(200);

// Submit
await submitButton.ClickAsync();
```

This is a BWFC-specific behavior that mirrors Web Forms' `TextBox` `TextChanged` event semantics — both fire on blur, not on keystroke.

### Static Asset Relocation

- All static files → `wwwroot/`
- CSS bundles (`BundleConfig.cs`) → explicit `<link>` tags in `App.razor`
- JS bundles → explicit `<script>` tags in `App.razor`
- Image paths update: `~/Images/` → `/Images/`
- Font paths: same pattern

### Compile-Compatibility Shims

BWFC ships shims that let migrated business logic and `App_Start` files compile without modification. These are part of the "Just Make It Compile" strategy — eliminate build errors first, then address runtime behavior.

**These are DROP-IN replacements.** The goal is to preserve the original Web Forms API calls with minimal changes.

| Shim | Namespace | What It Provides |
|---|---|---|
| **ConfigurationManager** | `BlazorWebFormsComponents` | `AppSettings["key"]` → reads from `IConfiguration`. `ConnectionStrings["name"]` → reads from `IConfiguration.GetConnectionString()`. |
| **BundleTable / Bundle / ScriptBundle / StyleBundle** | `System.Web.Optimization` | No-op stubs. `BundleTable.Bundles.Add(...)` compiles and does nothing. `App_Start/BundleConfig.cs` compiles as-is. |
| **RouteTable / RouteCollection** | `System.Web.Routing` | No-op stubs. `RouteTable.Routes.MapPageRoute(...)` compiles and does nothing. `App_Start/RouteConfig.cs` compiles as-is. |
| **SessionShim** | `BlazorWebFormsComponents` | `Session["key"]` indexer, `.Get<T>()`, `.Remove()`, `.Clear()`. Backed by `ISession` + in-memory fallback. Auto-registered. |
| **CacheShim** | `BlazorWebFormsComponents` | `Cache["key"]` indexer, `Cache.Insert()` with absolute/sliding expiration, `Cache.Get<T>()`, `Cache.Remove()`. Wraps `IMemoryCache`. Auto-registered. |
| **ServerShim** | `BlazorWebFormsComponents` | `Server.MapPath()`, `Server.HtmlEncode()`, `Server.HtmlDecode()`, `Server.UrlEncode()`, `Server.UrlDecode()`. Wraps `IWebHostEnvironment` + `WebUtility`. Auto-registered. |
| **RequestShim** | `BlazorWebFormsComponents` | `Request.QueryString`, `Request.Cookies`, `Request.Url`, `Request.Form`. Falls back to `NavigationManager` for QueryString/Url when HttpContext unavailable. |
| **ResponseShim** | `BlazorWebFormsComponents` | `Response.Redirect()` (auto-strips `~/` and `.aspx`), `Response.Cookies`. Wraps `NavigationManager` + `HttpContext`. |
| **FormShim** | `BlazorWebFormsComponents` | `Request.Form["key"]`, `.GetValues()`, `.AllKeys`, `.Count`, `.ContainsKey()`. Wraps `IFormCollection` or JS interop data. Populated by `<WebFormsForm>`. |
| **ClientScriptShim** | `BlazorWebFormsComponents` | `RegisterStartupScript()`, `RegisterClientScriptBlock()`, `RegisterClientScriptInclude()`, `GetPostBackEventReference()`, `GetCallbackEventReference()`. Queues scripts, flushes via `IJSRuntime`. Auto-registered. |
| **ScriptManagerShim** | `BlazorWebFormsComponents` | `ScriptManager.GetCurrent(page)` pattern, delegates all `RegisterXxx` calls to `ClientScriptShim`. Auto-registered. |

**Example: Before/After with Shims**

```csharp
// ===== Before (Web Forms) =====
protected void Page_Load(object sender, EventArgs e)
{
    string rawId = Request.QueryString["ProductID"];
    if (int.TryParse(rawId, out int productId))
    {
        var cart = new ShoppingCartActions();
        cart.AddToCart(productId);
    }
    Response.Redirect("ShoppingCart.aspx");
}

// ===== After (Blazor with Shims) — NOTICE: Nearly identical =====
protected override async Task OnInitializedAsync()
{
    string rawId = Request.QueryString["ProductID"];  // RequestShim
    if (int.TryParse(rawId, out int productId))
    {
        await CartService.AddToCartAsync(productId);
    }
    Response.Redirect("ShoppingCart");  // ResponseShim — .aspx stripped automatically
}
```

**Key observation:** The API calls (`Request.QueryString`, `Response.Redirect`) are **identical**. Only the lifecycle method (`Page_Load` → `OnInitializedAsync`) and service call pattern changed. This is the power of shims — migrate incrementally without rewriting everything.

**ConfigurationManager setup:**

```csharp
var app = builder.Build();
app.UseConfigurationManagerShim();   // Binds to IConfiguration
```

**appsettings.json mapping:** Map `Web.config` `<appSettings>` keys under an `"AppSettings"` section and `<connectionStrings>` under `"ConnectionStrings"`:

```json
{
  "AppSettings": {
    "SiteName": "My Store"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;"
  }
}
```

> **BundleConfig/RouteConfig are no-ops.** They exist only to prevent compile errors. In Blazor, use `<link>` / `<script>` tags in `App.razor` for assets and `@page` directives for routing.

### Generated Code — Variable Declaration Styles

> **CRITICAL:** All local variable declarations in generated Blazor code MUST use `var` (implicit typing), not explicit types. This is enforced by `.editorconfig` as **IDE0007 error**.

```csharp
// CORRECT — var for all local declarations
var students = db.Students.ToList();
var product = await productService.GetProductAsync(id);
var count = items.Count();

// WRONG — explicit type declarations cause build failures
List<Student> students = db.Students.ToList();
Product product = await productService.GetProductAsync(id);
int count = items.Count();
```

This applies to **both L1-generated scaffolding and L2 Copilot-generated code**. IDE0007 is enabled as a build error in `/.editorconfig` — explicit types will fail the build immediately.

### Page Lifecycle Mapping

| Web Forms | Blazor | Phase | Notes |
|---|---|---|---|
| `Page_Load` | `OnInitializedAsync` | Phase 1: compiles as-is; **Phase 2: transform** | One-time init |
| `Page_Init` | `OnInitialized` | Phase 1: compiles as-is; **Phase 2: transform** | Sync initialization |
| `Page_PreInit` | `OnInitializedAsync` (early) | Phase 2 | Theme setup |
| `Page_PreRender` | `OnAfterRenderAsync(bool firstRender)` | Phase 1: compiles as-is; **Phase 2: transform** | Guard with `if (firstRender)` to avoid render loops |
| `IsPostBack` check | `if (!IsPostBack)` works AS-IS via `WebFormsPageBase`; L1 script auto-unwraps simple guards | Phase 1 | SSR: checks HTTP method (GET→false, POST→true). Interactive: first render→false, subsequent→true. `if (IsPostBack)` without `!` runs in POST/subsequent renders — review for correctness. |
| `Page.Title` | `Page.Title = "X"` works AS-IS via `WebFormsPageBase` | Phase 1 | `WebFormsPageBase` delegates to `IPageService`. `<BlazorWebFormsComponents.Page />` in layout renders `<PageTitle>` and `<meta>` tags. |
| `Response.Redirect` | `Response.Redirect("~/Page.aspx")` works AS-IS via `ResponseShim` | Phase 1 | Auto-strips `~/` prefix and `.aspx` extension. Uses `NavigationManager.NavigateTo()` internally. |
| `Request.Form` | `Request.Form["key"]` works AS-IS via `FormShim` | Phase 1 | Use `<WebFormsForm OnSubmit="SetRequestFormData">` to populate in interactive mode. SSR mode uses native `IFormCollection`. |
| `Request.QueryString` | `Request.QueryString["key"]` works AS-IS via `RequestShim` | Phase 1 | Always parses from `NavigationManager.Uri` — correct in both SSR and interactive modes. |
| `Session["key"]` | `Session["key"]` works AS-IS via `SessionShim` | Phase 1 | In-memory fallback in interactive mode; syncs with `ISession` when available. |
| `Cache["key"]` | `Cache["key"]` works AS-IS via `CacheShim` | Phase 1 | `Cache.Insert()` supports absolute and sliding expiration. Wraps `IMemoryCache`. |
| `Server.MapPath` | `Server.MapPath("~/path")` works AS-IS via `ServerShim` | Phase 1 | `~/` maps to wwwroot. Other paths relative to ContentRootPath. |
| `Page.ClientScript` | `ClientScript.RegisterStartupScript(...)` works AS-IS via `ClientScriptShim` | Phase 1 | Queues scripts, flushes via `IJSRuntime` in `OnAfterRenderAsync`. Also supports `GetPostBackEventReference()`. |

> **Phase 2 lifecycle transforms:** After Phase 1 produces compilable code with original `Page_Load`/`Page_Init`/`Page_PreRender` signatures, Phase 2 converts these to proper Blazor lifecycle overrides. See `bwfc-migration` skill for full before/after examples.

### Layer 1 (Script) vs Layer 2 (Copilot-Assisted) Boundary

> ⚠️ **CRITICAL: Layer 1 and Layer 2 MUST both run in sequence. Do NOT make any manual code fixes between Layer 1 and Layer 2.** Manual fixes between layers corrupt pipeline quality measurement. If Layer 1 output has issues, fix the script — not the output.

**Layer 1 — Automated Script** (`migration-toolkit/scripts/bwfc-migrate.ps1`):

Run via:
```powershell
.\migration-toolkit\scripts\bwfc-migrate.ps1 -Path "<source-webforms-project>" -Output "<blazor-output-dir>"
```

Script handles:
- `asp:` prefix stripping (preserves BWFC tags)
- Data-binding expression conversion (5 variants)
- LoginView → **preserve as BWFC LoginView** — do NOT rewrite as AuthorizeView. The BWFC `LoginView` injects `AuthenticationStateProvider` natively and uses the same template names (`AnonymousTemplate`, `LoggedInTemplate`). The migration script handles this automatically.
- Master page → MainLayout.razor
- Scaffold generation (csproj, Program.cs, etc.)
- SelectMethod/GetRouteUrl flagging
- Register directive cleanup
- **IsPostBack guard unwrapping** — simple `if (!IsPostBack) { ... }` guards are auto-unwrapped (body extracted, `if` removed). Complex guards with `else` clauses get TODO comments for manual review.
- **`.aspx` URL cleanup** — string literals like `"~/Page.aspx?id=5"` are rewritten to `"/Page?id=5"` in code-behind files (tilde-prefixed and relative patterns).
- **`using` retention for BWFC shims** — `System.Configuration`, `System.Web.Optimization`, and `System.Web.Routing` usings are preserved (as comments with guidance) because BWFC provides compile-compatible shims.

**Layer 2 — Copilot-Assisted** (NOT manual — guided by the `bwfc-migration` skill):
- EF6 → EF Core (models, DbContext, seed)
- Identity/Auth subsystem
- Session → scoped services
- Business logic (checkout, payment, admin CRUD)
- Complex data-binding with arithmetic/method chains
- Data loading patterns (`SelectMethod` string → `SelectHandler` delegate, or `Items` via `OnInitializedAsync`)
- Template context wiring (`Context="Item"`)
- Navigation conversions (`Response.Redirect` → `NavigationManager.NavigateTo`)

## Examples

### Preserving a ListView (CORRECT)

```razor
@* Web Forms *@
<asp:ListView ID="productList" runat="server"
    DataKeyNames="ProductID" GroupItemCount="4"
    ItemType="WingtipToys.Models.Product"
    SelectMethod="GetProducts">
    <ItemTemplate>
        <td><%#: Item.ProductName %></td>
    </ItemTemplate>
</asp:ListView>

@* After migration — Option A: SelectMethod preserved as delegate (BWFC native) *@
<ListView SelectMethod="@productService.GetProducts" GroupItemCount="4">
    <ItemTemplate>
        <td>@context.ProductName</td>
    </ItemTemplate>
</ListView>

@code {
    [Inject] private ProductService productService { get; set; }
}
```

```razor
@* After migration — Option B: Items loaded in OnInitializedAsync *@
<ListView Items="@_products" GroupItemCount="4">
    <ItemTemplate>
        <td>@context.ProductName</td>
    </ItemTemplate>
</ListView>

@code {
    [Inject] private ProductContext Db { get; set; }
    private List<Product> _products;

    protected override async Task OnInitializedAsync()
    {
        _products = await Db.Products.ToListAsync();
    }
}
```

### Preserving Event Handlers (CORRECT)

```razor
@* Web Forms *@
<asp:Button ID="btnRemove" runat="server" Text="Remove"
    OnClick="RemoveItem_Click" CommandArgument='<%# Item.ItemId %>' />

@* After migration (BWFC preserved) *@
<Button Text="Remove"
    OnClick="RemoveItem_Click" CommandArgument="@context.ItemId" />

@code {
    // Only signature changes — method name stays the same
    private async Task RemoveItem_Click(MouseEventArgs e) { ... }
}
```

## Anti-Patterns

### ❌ Replacing BWFC Data Controls with Raw HTML

```razor
@* WRONG — loses all BWFC functionality *@
@foreach (var product in _products)
{
    <tr>
        <td>@product.ProductName</td>
    </tr>
}

@* RIGHT — use BWFC ListView *@
<ListView Items="@_products">
    <ItemTemplate>
        <tr><td>@context.ProductName</td></tr>
    </ItemTemplate>
</ListView>
```

### ❌ Stripping Event Handler Attributes

```razor
@* WRONG — strips the handler, requires manual re-wiring *@
<Button Text="Submit" />
@* TODO: re-add click handler *@

@* RIGHT — preserve the attribute, only annotate signature change *@
<Button Text="Submit" OnClick="Submit_Click" />
@* TODO: Update Submit_Click signature: (object, EventArgs) → (MouseEventArgs) *@
```

### ❌ Using System.Web.UI.Page as Base Class

```csharp
// WRONG — Web Forms base class
public partial class ProductList : Page { }

// RIGHT — BWFC page base class (provides Page.Title, IsPostBack, etc.)
// Set via @inherits WebFormsPageBase in _Imports.razor
public partial class ProductList : WebFormsPageBase { }

// ALSO RIGHT — for non-page components
public partial class MyComponent : ComponentBase { }
```

### ❌ Creating Minimal API Endpoints to Replace Page Actions

```csharp
// WRONG — turning a Web Forms page into an API endpoint
app.MapPost("/AddToCart", async (HttpContext context, int productId) =>
{
    // ... cart logic
    return Results.Redirect("/ShoppingCart");
});

// RIGHT — keep it as a Blazor page with shims
@page "/AddToCart"
@inherits WebFormsPageBase

@code {
    [Parameter] [SupplyParameterFromQuery] public int ProductID { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await CartService.AddToCartAsync(ProductID);
        Response.Redirect("ShoppingCart");  // Shim handles navigation
    }
}
```

### ❌ Injecting Raw ASP.NET Core Services When Shims Exist

```csharp
// WRONG — bypassing shims with raw services
[Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }
[Inject] private NavigationManager NavManager { get; set; }
[Inject] private IMemoryCache MemoryCache { get; set; }

protected override void OnInitialized()
{
    var session = HttpContextAccessor.HttpContext.Session;
    var cartId = session.GetString("CartId");
    
    MemoryCache.Set("key", value);
    
    NavManager.NavigateTo("/ShoppingCart");
}

// RIGHT — use shims from WebFormsPageBase
@inherits WebFormsPageBase

protected override void OnInitialized()
{
    var cartId = Session["CartId"];  // SessionShim
    
    Cache["key"] = value;  // CacheShim
    
    Response.Redirect("ShoppingCart");  // ResponseShim
}
```

### ❌ Using Cookies to Replace Session

```csharp
// WRONG — replacing Session with cookies manually
[Inject] private IHttpContextAccessor HttpContextAccessor { get; set; }

protected override void OnInitialized()
{
    var cartId = HttpContextAccessor.HttpContext.Request.Cookies["CartId"];
    if (string.IsNullOrEmpty(cartId))
    {
        cartId = Guid.NewGuid().ToString();
        HttpContextAccessor.HttpContext.Response.Cookies.Append("CartId", cartId);
    }
}

// RIGHT — use SessionShim
@inherits WebFormsPageBase

protected override void OnInitialized()
{
    var cartId = Session["CartId"];
    if (string.IsNullOrEmpty(cartId))
    {
        cartId = Guid.NewGuid().ToString();
        Session["CartId"] = cartId;
    }
}
```

### ❌ Using [ExcludeFromInteractiveRouting] to Work Around Interactive Mode

```razor
@* WRONG — fighting the Router instead of using shims *@
@attribute [ExcludeFromInteractiveRouting]
@page "/AddToCart"

@* RIGHT — embrace interactive mode with shims *@
@page "/AddToCart"
@inherits WebFormsPageBase
```

### ❌ Adding onclick JavaScript Hacks to Bypass Blazor Routing

```razor
@* WRONG — using JavaScript to work around routing *@
<a href="/ShoppingCart" onclick="window.location.href='/ShoppingCart'; return false;">
    View Cart
</a>

@* RIGHT — use standard Blazor navigation or shims *@
<a href="/ShoppingCart">View Cart</a>

@* OR if you need programmatic redirect *@
<Button Text="View Cart" OnClick="GoToCart" />

@code {
    private void GoToCart(MouseEventArgs e)
    {
        Response.Redirect("ShoppingCart");  // ResponseShim
    }
}
```

---
name: "migration-standards"
description: "Enforce canonical migration standards for ASP.NET Web Forms to Blazor using BWFC. Covers target architecture (.NET 10, Server Interactive), database provider detection, event handler preservation, SelectMethod patterns, and page lifecycle mapping. WHEN: \"migration standards\", \"target architecture\", \"render mode placement\", \"page base class\", \"Layer 1 vs Layer 2\"."
domain: "migration"
confidence: "medium"
source: "earned"
---

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
| `IsPostBack` | Always returns `false` — `if (!IsPostBack)` always enters block |
| `Page` | Returns `this` — enables `Page.Title = "X"` dot syntax |

**What is NOT provided (forces proper Blazor migration):**

- `Page.Request` — use `IHttpContextAccessor` or `NavigationManager`
- `Page.Response` — use `NavigationManager` for redirects
- `Page.Session` — use scoped DI services

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

### Session State → Scoped Services

- **Phase 2 quick-start:** Use `SessionShim` (`builder.Services.AddSessionShim()`) for immediate `Session["key"]` compatibility — code-behind works unchanged via `WebFormsPageBase.Session`
- **Final migration:** Replace `Session["key"]` with a scoped DI service for durable state
- Use `IHttpContextAccessor` for cookie-based persistence when needed
- Register in `Program.cs` with `builder.Services.AddScoped<TService>()`
- Example: `Session["CartId"]` → `CartStateService` with cookie-based cart ID

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

| Shim | Namespace | What It Provides |
|---|---|---|
| **ConfigurationManager** | `BlazorWebFormsComponents` | `AppSettings["key"]` → reads from `IConfiguration`. `ConnectionStrings["name"]` → reads from `IConfiguration.GetConnectionString()`. |
| **BundleTable / Bundle / ScriptBundle / StyleBundle** | `System.Web.Optimization` | No-op stubs. `BundleTable.Bundles.Add(...)` compiles and does nothing. `App_Start/BundleConfig.cs` compiles as-is. |
| **RouteTable / RouteCollection** | `System.Web.Routing` | No-op stubs. `RouteTable.Routes.MapPageRoute(...)` compiles and does nothing. `App_Start/RouteConfig.cs` compiles as-is. |

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
| `IsPostBack` check | `if (!IsPostBack)` works AS-IS via `WebFormsPageBase`; L1 script auto-unwraps simple guards | Phase 1 | Always enters block; `if (IsPostBack)` without `!` is dead code — flag for review |
| `Page.Title` | `Page.Title = "X"` works AS-IS via `WebFormsPageBase` | Phase 1 | `WebFormsPageBase` delegates to `IPageService`. `<BlazorWebFormsComponents.Page />` in layout renders `<PageTitle>` and `<meta>` tags. |
| `Response.Redirect` | `NavigationManager.NavigateTo()` | Phase 2 | Inject `NavigationManager` |

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

# Automated Migration with GitHub Copilot

This guide covers the end-to-end workflow for migrating an ASP.NET Web Forms application to Blazor using GitHub Copilot and the BWFC migration toolkit. It is a practical, run-it-yourself guide — every step is illustrated with the **WingtipToys** benchmark app, the most battle-tested example in the repository.

!!! note "Want the theory first?"
    This guide is about *doing*. For the reasoning behind the three-layer model, see [Three-Layer Methodology](Methodology.md). For a shorter linear walkthrough, see [Quick Start Guide](QuickStart.md).

---

## Overview

Automated migration with Copilot means letting the tools do the mechanical work — and then letting Copilot do the pattern work — so you only spend human judgment on architecture decisions.

The key insight: **stripping `asp:` prefixes and `runat="server"` is not migration; it's transliteration.** The BWFC CLI handles the full structural conversion in ~30 seconds. Copilot then closes the gap between "compiles" and "runs correctly." You make the architecture calls.

### How It Differs from Manual Migration

| Approach | Time to First Build | How Work Is Done |
|---|---|---|
| **Manual** | Days to weeks | You translate every control, lifecycle method, and data binding by hand |
| **Automated + Copilot** | ~5 minutes to first build | CLI handles structure; Copilot handles patterns; you handle architecture |
| **Strangler Fig (YARP)** | Weeks or months | Pages migrated incrementally behind a reverse proxy |

### When to Use Automated vs. Incremental (YARP)

Use **automated migration** when:
- Your app has ≥90% BWFC control coverage (check with `prescan`)
- You can tolerate a code freeze during migration (a few hours to a day)
- You want a clean cut — one commit that is the Blazor app

Use **[Strangler Fig / YARP](StranglerFigPattern.md)** when:
- Your app is very large (hundreds of pages) or has many unsupported controls
- You need to migrate incrementally without a freeze
- Teams need to continue Web Forms development during migration

### Prerequisites

| Requirement | Version | Check |
|---|---|---|
| .NET SDK | 10.0+ | `dotnet --version` |
| GitHub Copilot | Any current | VS Code or Visual Studio |
| BWFC CLI | Latest from repo | `dotnet build src/BlazorWebFormsComponents.Cli` |
| PowerShell | 7+ | `pwsh --version` |

---

## The Migration Pipeline

Migration happens in three layers. Each handles a fundamentally different *kind* of work:

```
Layer 1 (CLI)          Layer 2 (Copilot)        Layer 3 (You)
──────────────         ─────────────────         ─────────────
Deterministic          Pattern recognition        Architecture
~30 seconds            ~20–60 minutes             ~4–12 hours
~60% of work           ~30% of work               ~10% of work
```

### Layer 1 — CLI Automated Transforms

The `webforms-to-blazor migrate` command applies **24+ markup transforms** and **27 code-behind transforms** deterministically. It scaffolds a .NET 10 Blazor static SSR project, converts all `.aspx`/`.ascx`/`.master` files, and wires the BWFC shim infrastructure.

**Entry point:**
```bash
# Via the BWFC CLI directly
dotnet run --project src/BlazorWebFormsComponents.Cli -- migrate -i <source> -o <output>

# Via the migration-toolkit wrapper (recommended for external projects)
pwsh migration-toolkit/scripts/bwfc-migrate.ps1 -i <source> -o <output>
```

Layer 1 output compiles in most cases without any manual changes. The generated `Program.cs` calls `AddBlazorWebFormsComponents()` and `_Imports.razor` sets `@inherits WebFormsPageBase` — so `Session`, `Response`, `Request`, `Cache`, `Server`, and `IsPostBack` all work unchanged in code-behind files. See [Migration Shims](#migration-shims) below.

### Layer 2 — Copilot-Assisted Repair

After Layer 1, some files need structural cleanup the CLI couldn't handle deterministically:

- DbContext/service constructor injection
- EDMX-model primary key configuration (`HasKey()`)
- Data binding connections (`SelectMethod` → `OnInitializedAsync` + `Items`)
- Event handler signature cleanup
- Duplicate source file removal
- Quarantined page restoration

Copilot handles these patterns reliably with the BWFC migration skills. See [What Copilot Handles (L2)](#what-copilot-handles-l2) for the full list.

!!! warning "Trust the shims during L2"
    Do **not** replace shim-backed properties with native ASP.NET Core equivalents during Layer 2. `Response.Redirect()`, `Session["key"]`, `Request.QueryString`, and `IsPostBack` all work correctly via BWFC shims. Replacing them is an optional Layer 3 optimization — not a migration requirement.

### Layer 3 — Manual Tuning

The remaining ~10% of work requires human judgment:

- **EF6 → EF Core** — DbContext registration, async patterns, migrations
- **Authentication** — ASP.NET Membership/OWIN Identity → ASP.NET Core Identity
- **DataSource controls** — `SqlDataSource`, `ObjectDataSource` → injected services
- **Global.asax** — startup hooks → `Program.cs` middleware pipeline
- **Performance** — removing shims, async EF, `AsNoTracking()`, `@key` on loops

See [Data & Architecture Migration Skill](CopilotSkills/DataMigration.md) for Layer 3 guidance.

---

## Step-by-Step Walkthrough: WingtipToys

WingtipToys is an e-commerce Web Forms app (products, shopping cart, checkout, identity) with 26 acceptance tests. It is the most proven BWFC migration benchmark.

### Step 1 — Pre-Scan the Source App

Before migrating, scan the app to understand control coverage and complexity:

```bash
dotnet run --project src/BlazorWebFormsComponents.Cli -- prescan -i samples/WingtipToys
```

The prescan reports:
- File inventory (`.aspx`, `.ascx`, `.master` counts)
- Control usage by type and frequency
- DataSource controls (these need L3 replacement)
- Custom-control registrations from `Web.config` and `<%@ Register %>` directives
- ASCX descriptor signals (public members, lifecycle/data-binding clues, `FindControl` IDs, parse diagnostics)
- Migration readiness score

A score of **90%+** means automated migration will produce a clean result. Lower scores indicate more L3 architecture work ahead.

!!! tip "Save the prescan output"
    Run with `-o scan-report.md` to save the report for reference during L2 and L3 work.

### Step 2 — Run the Migration

```bash
dotnet run --project src/BlazorWebFormsComponents.Cli -- migrate \
  -i samples/WingtipToys \
  -o samples/AfterWingtipToys
```

Or with the migration-toolkit wrapper:

```powershell
pwsh migration-toolkit/scripts/bwfc-migrate.ps1 `
  -i samples/WingtipToys `
  -o samples/AfterWingtipToys
```

**What runs in ~5–30 seconds:**

| Transform | Example |
|---|---|
| Strip `asp:` prefixes | `<asp:Button>` → `<Button>` |
| Remove `runat="server"` | Removed from all controls |
| Convert expressions | `<%: Item.Name %>` → `@(Item.Name)` |
| Convert data binding | `<%#: Item.Price %>` → `@context.Price` |
| Convert URLs | `href="~/Products"` → `href="/Products"` |
| Convert directives | `<%@ Page ... %>` → `@page "/route"` |
| Rename files | `Products.aspx` → `Products.razor` |
| Convert `ItemType` | `ItemType="WingtipToys.Product"` → `TItem="Product"` |
| Remove content wrappers | `<asp:Content ...>` → unwrapped |
| Map page lifecycle | `Page_Load` → `OnInitializedAsync` |
| Prepare `FindControl` migrations | `id="CartList"` + code-behind → `@ref="CartList"` + backing field |
| Normalize `DataBind()` patterns | Keep `DataSource = ...`, remove `.DataBind()`, inject `Items="@( ... )"` |
| Convert event signatures | `Btn_Click(object, EventArgs)` → `Btn_Click()` |
| Master Page → Layout | `Site.Master` → `MainLayout.razor` |
| Scaffold project | `.csproj`, `Program.cs`, `_Imports.razor`, `App.razor` |
| Wire shims | `AddBlazorWebFormsComponents()` in `Program.cs` |
| Quarantine unsafe pages | `Account/`, `Checkout/` stubbed if needed |

After this step, a migration report is written to `samples/AfterWingtipToys/migration-artifacts/`.

TODO(P1-FindControl-callsite): automated callsite rewrites for all `FindControl(...)` cast/access shapes are still incomplete; verify those edits during L2 review.

### Step 3 — Build the Output

```bash
dotnet build samples/AfterWingtipToys/WingtipToys.csproj
```

For a well-covered app like WingtipToys, the first build typically passes or has only a handful of errors. If errors appear, move to Step 4.

!!! tip "Use `dotnet watch` for fast iteration"
    Once the project builds, `dotnet watch --project samples/AfterWingtipToys/WingtipToys.csproj` gives you hot reload as you fix L2 patterns.

### Step 4 — Fix L2 Build Errors with Copilot

Open the migrated project in VS Code with GitHub Copilot. Common L2 patterns and how Copilot handles them:

#### DbContext Constructor Injection

Web Forms often instantiates `DbContext` directly. Copilot migrates to injected lifetime:

**Before:**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    var db = new WingtipToysEntities();
    ProductList.Items = db.Products.ToList();
}
```

**After:**
```csharp
[Inject] private WingtipToysEntities Db { get; set; } = default!;

protected override async Task OnInitializedAsync()
{
    ProductList.Items = await Db.Products.ToListAsync();
}
```

Prompt Copilot:
> "Using the bwfc-migration skill, inject the DbContext and wire up data binding on this page."

#### EDMX HasKey() for Non-Conventional Primary Keys

EDMX-sourced models sometimes use composite or non-conventional keys. Copilot adds the required EF Core fluent configuration:

```csharp
// In ApplicationDbContext.OnModelCreating:
modelBuilder.Entity<CartItem>()
    .HasKey(c => new { c.CartId, c.ProductId });
```

Prompt Copilot:
> "The CartItem entity has a composite primary key on CartId and ProductId. Add HasKey() configuration."

#### Namespace Casing Mismatches

The CLI may preserve casing from the original project. If the assembly name differs from folder casing:

```csharp
// _Imports.razor — adjust to match your actual namespace
@using WingtipToys.Models
@using WingtipToys.Logic
```

#### Server.MapPath → Path.Combine

`Server.MapPath` is shim-backed and works, but if you need a physical path in a non-page context:

```csharp
// Shim-backed (works AS-IS in pages inheriting WebFormsPageBase)
var path = Server.MapPath("~/App_Data/catalog.xml");

// Layer 3 native alternative (for services not inheriting WebFormsPageBase)
var path = Path.Combine(env.ContentRootPath, "App_Data", "catalog.xml");
```

#### Self-Instantiation Cleanup

When a code-behind referenced itself by class name, the CLI may have left a `new ClassName()` call. Copilot removes these:

```csharp
// Remove — this was a Web Forms pattern for accessing page properties
// var page = new Products();  ← delete

// Use component state directly
var products = await ProductService.GetProductsAsync();
```

Prompt Copilot:
> "Find and remove any self-instantiation patterns left over from Web Forms."

### Step 5 — Run the App

```bash
dotnet run --project samples/AfterWingtipToys/WingtipToys.csproj
```

Open the app and verify:

- [ ] Home page renders
- [ ] Product catalog loads
- [ ] Navigation links work
- [ ] Static assets (CSS, images) load correctly

!!! warning "Always target static SSR"
    The migrated app targets Blazor static server-side rendering. Do **not** add `@rendermode InteractiveServer` to pages globally — only opt in to interactive render modes deliberately, per page, when you need interactivity.

### Step 6 — Run Acceptance Tests

The WingtipToys acceptance test suite verifies the migrated app end-to-end with Playwright:

```bash
# Build the test project
dotnet build src/WingtipToys.AcceptanceTests

# Install Playwright browsers (first time only)
pwsh src/WingtipToys.AcceptanceTests/bin/Debug/net10.0/playwright.ps1 install chromium

# Run the tests (app must be running)
dotnet test src/WingtipToys.AcceptanceTests
```

By default the tests connect to `https://localhost:5001`. To use a different URL:

```powershell
$env:WINGTIPTOYS_BASE_URL = "https://localhost:7042"
dotnet test src/WingtipToys.AcceptanceTests
```

A clean migration should pass **26/26 tests**. Tests that fail indicate pages or flows that need additional L2 or L3 attention.

### Step 7 — Iterate

Address any failing tests, then re-run until clean. Work through pages in this order:

1. **Shared layout** (`MainLayout.razor`) — errors here affect every page
2. **Home and catalog pages** — high-traffic, acceptance-test-covered
3. **Cart and checkout flows** — these are often quarantined initially; restore with Copilot
4. **Account and auth pages** — need the Identity migration skill

For each failing page, ask Copilot:
> "Using migration-toolkit/skills/bwfc-migration/SKILL.md, fix the build errors in this file."

---

## What the CLI Handles (L1)

The following transforms run automatically during `webforms-to-blazor migrate`:

**Markup transforms:**
- `asp:` prefix removal from all controls
- `runat="server"` removal
- `<%: %>` and `<%= %>` expression conversion to `@()`
- `<%#: %>` data binding expression conversion
- `<%-- --%>` comment conversion to `@* *@`
- `~/` URL prefix conversion to `/`
- `.aspx` extension removal from internal links
- `asp:Content` wrapper removal
- `<form runat="server">` wrapping with antiforgery support
- `ItemType="Namespace.Class"` → `TItem="Class"` stripping
- `EnableViewState`, `AutoEventWireup`, `CodeBehind` attribute removal
- Master Page `<asp:ContentPlaceHolder>` → BWFC `<ContentPlaceHolder>` conversion
- `<%@ Page %>` directive → `@page` route + `@layout` directive
- `<%@ Register %>` → `@using` namespace reference

**Code-behind transforms:**
- `Page_Load(object sender, EventArgs e)` → `OnInitializedAsync()`
- `Page_PreRender` / `Page_Init` lifecycle mapping
- `void Btn_Click(object sender, EventArgs e)` → `void Btn_Click()`
- `protected` → `private` on event handlers
- `Response.Redirect("~/path.aspx")` → shim-backed (left unchanged)
- `HttpUtility.HtmlEncode/UrlEncode` → `WebUtility.*`
- `@ref` field declaration injection

**Scaffolding:**
- `.csproj` targeting `.NET 10`, referencing `Fritz.BlazorWebFormsComponents`
- `Program.cs` with `AddBlazorWebFormsComponents()` and `UseAntiforgery()`
- `_Imports.razor` with `@inherits WebFormsPageBase`
- `App.razor`, `Routes.razor`, `MainLayout.razor` shells
- `appsettings.json` from `Web.config` connection strings and app settings
- Quarantine manifest for pages that need L2/L3 attention

---

## What Copilot Handles (L2)

These patterns require understanding the app's data layer or structure — Copilot resolves them with the BWFC migration skills:

| Pattern | Copilot Action |
|---|---|
| Inline `new DbContext()` | Inject via `[Inject]`, use in `OnInitializedAsync` |
| `SelectMethod="GetItems"` | Wire to `Items=` binding with loaded collection |
| EDMX composite primary keys | Add `HasKey()` in `OnModelCreating` |
| Duplicate `.cs` source files (Web Forms app_code) | Remove or consolidate |
| Quarantined page restoration | Reconstruct page from stub + artifact |
| `runat="server"` control property stubs | Replace with BWFC component parameters |
| `EF6 Include()` chain | Convert to EF Core `.Include()` / `.ThenInclude()` |
| `Page.User.Identity.Name` | Migrate to `AuthenticationStateProvider` |
| Namespace casing drift | Align `@using` statements with actual namespaces |
| Self-instantiation (`new ProductPage()`) | Remove; use component state directly |

**Skill references for L2 prompts:**
```
Use migration-toolkit/skills/bwfc-migration/SKILL.md for markup and code-behind.
Use migration-toolkit/skills/bwfc-identity-migration/SKILL.md for auth pages.
Use migration-toolkit/skills/bwfc-data-migration/SKILL.md for EF and services.
```

---

## Migration Shims

A key feature of the automated migration approach is the BWFC **compile-compatibility shim layer**. When `AddBlazorWebFormsComponents()` is called in `Program.cs` and pages inherit `WebFormsPageBase` (set in `_Imports.razor`), the following Web Forms APIs **work unchanged in migrated code-behind**:

| Shim | What It Provides |
|---|---|
| `Page.Request` | `QueryString["key"]`, `Form["key"]`, `Url`, `Cookies` |
| `Page.Response` | `Redirect()` — auto-strips `~/` and `.aspx` extension |
| `Page.Session` | Scoped in-memory dictionary — `Session["key"] = value` |
| `Page.Server` | `MapPath()`, `HtmlEncode()`, `UrlEncode()` |
| `Page.Cache` | `IMemoryCache`-backed application cache |
| `Page.ClientScript` | `RegisterStartupScript()`, `RegisterClientScriptBlock()` |
| `Page.ViewState` | Per-component dictionary (compile-compat; not serialized) |
| `<WebFormsForm>` | Enables `Request.Form["key"]` in interactive mode via JS interop |
| `ConfigurationManager` | `AppSettings["key"]` and `ConnectionStrings["name"]` from `IConfiguration` |
| `ScriptManagerShim` | `ScriptManager.GetCurrent(page)` pattern |
| `IsPostBack` | `false` on first render; `true` on subsequent interactions |

This means that migrated code-behind files with `Response.Redirect`, `Session`, `Request.QueryString`, `IsPostBack`, `Cache`, and `Server.MapPath` calls **do not need to be manually converted** to native ASP.NET Core equivalents during L1 or L2. Replacing shims with native Blazor patterns is an optional Layer 3 optimization.

!!! note "Full shim reference"
    See [Page System](../UtilityFeatures/PageService.md), [Request Shim](../UtilityFeatures/RequestShim.md), [Cache Shim](../UtilityFeatures/CacheShim.md), [Server & Path Resolution](../UtilityFeatures/ServerShim.md), and [WebFormsForm](../UtilityFeatures/WebFormsForm.md) for full API documentation.

---

## Benchmarks

These are measured results from automated migration benchmark runs in this repository:

| Benchmark | Acceptance Tests | L1 Time | L2 Time (with Copilot) |
|---|---|---|---|
| **WingtipToys** | **26/26 ✅** | ~5 seconds | ~20–30 minutes |
| **ContosoUniversity** | **37/40** | ~5 seconds | ~30–45 minutes |

!!! note "What these numbers mean"
    L1 time is the CLI transform time. L2 time includes Copilot-assisted repair to reach a passing acceptance test run. Your times will vary based on app complexity and coverage.

**WingtipToys** (e-commerce: products, cart, identity) is the current primary benchmark and the most proven. **ContosoUniversity** (university CRUD with EF, master-detail, search) is the next benchmark being actively refined.

---

## Tips

**Target static SSR — not Interactive Server.** The migration pipeline generates a .NET 10 static server-side rendering app. This is the correct target. Do not add `@rendermode InteractiveServer` globally. Add it deliberately, per-page or per-component, only where interactivity is required.

**Trust the shims.** The single biggest time-waster during L2 is manually replacing shim-backed APIs with native alternatives. `Response.Redirect()` calls `NavigationManager` internally. `Session["key"]` is backed by a scoped in-memory dictionary. They work. Leave them.

**Preserve BWFC data controls.** Never replace `GridView`, `ListView`, `FormView`, `DataList`, or `Repeater` with raw `<table>` or `<div>` markup. Fix the BWFC markup, templates, item types, or child components instead. The components exist precisely to avoid this.

**Preserve original URL patterns.** The CLI preserves `.aspx` → route mappings. Avoid renaming routes during L2 — fix functional issues first, optimize routes in L3 when the app is working.

**Use `dotnet watch` for quick L2 iteration.** Once the first build is clean, `dotnet watch` gives hot reload as you apply Copilot fixes — tightening the feedback loop significantly.

**Check the quarantine manifest.** After L1, look at `migration-artifacts/quarantine-manifest.json`. Quarantined pages are build-safe stubs with the original code preserved as an artifact. Address essential pages first; leave admin and account pages for L3.

**Use Copilot prompts that reference skill files.** Vague prompts produce vague results. Specific skill references give Copilot the exact rules it needs:

```
Use migration-toolkit/skills/bwfc-migration/SKILL.md to fix the build errors in ShoppingCart.razor.cs.
Focus on constructor injection for WingtipToysEntities and async data loading.
```

---

## See Also

- [Quick Start Guide](QuickStart.md) — shorter linear walkthrough
- [Three-Layer Methodology](Methodology.md) — theory behind the pipeline
- [Copilot Skills Overview](CopilotSkills/Overview.md) — L2 and L3 skill reference
- [Control Coverage](ControlCoverage.md) — supported controls by category
- [Migration Checklist Template](ChecklistTemplate.md) — per-page progress tracking
- [Strangler Fig Pattern](StranglerFigPattern.md) — incremental migration alternative

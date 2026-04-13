# Per-Page Migration Checklist

**Copy this template for each page you migrate.** Use it as a GitHub issue body, a markdown checklist in your tracking doc, or paste it into your project management tool.

The checklist is organized by the [three-layer pipeline](METHODOLOGY.md). Work top to bottom — each section assumes the previous one is complete.

---

## Template

```markdown
## Page: [PageName.aspx] → [PageName.razor]

**Source:** `[path/to/PageName.aspx]`
**Target:** `[path/to/PageName.razor]`
**Complexity:** [Trivial / Easy / Medium / Complex]
**Notes:** [Any page-specific context — what this page does, key controls used]

### Layer 1 — Automated (webforms-to-blazor CLI or bwfc-migrate.ps1)

- [ ] File renamed (.aspx → .razor, .ascx → .razor, .master → .razor)
- [ ] `<%@ Page %>` / `<%@ Control %>` / `<%@ Master %>` directive removed
- [ ] `@page "/route"` directive added
- [ ] `asp:` prefixes removed from all controls
- [ ] `runat="server"` removed from all elements
- [ ] Expressions converted (`<%: %>` → `@()`, `<%# %>` → `@context.`)
- [ ] URL references converted (`~/` → `/`)
- [ ] `<asp:Content>` wrappers removed (page body unwrapped)
- [ ] `ItemType` → `TItem` converted
- [ ] Code-behind file copied (.aspx.cs → .razor.cs) with TODO annotations
- [ ] `AddBlazorWebFormsComponents()` registered in `Program.cs`
- [ ] `_Imports.razor` has `@inherits WebFormsPageBase`
- [ ] Pages using `Request.Form` wrapped in `<WebFormsForm>`

### Layer 2 — Copilot-Assisted (Structural Transforms)

- [ ] `SelectMethod` → `Items` (or `DataItem`) binding wired
- [ ] Data loading moved to `OnInitializedAsync`
- [ ] Template `Context="Item"` variables added to all templates
- [ ] Event handlers converted to Blazor signatures (remove `sender`, `EventArgs`)
- [ ] ✅ `Page_Load` / `IsPostBack` — works AS-IS via `WebFormsPageBase` (only signature `Page_Load(sender, e)` → `OnInitializedAsync` needs converting; `IsPostBack` inside works unchanged)
- [ ] ✅ `Response.Redirect` — works AS-IS via ResponseShim (auto-strips `~/` and `.aspx`)
- [ ] Session state: Use `Session["key"]` from `WebFormsPageBase` (SessionShim) — works in interactive mode ✅
- [ ] ✅ Response.Redirect() calls work via ResponseShim
- [ ] ✅ Request.QueryString[] calls work via RequestShim
- [ ] No raw `IHttpContextAccessor` injected (use shim properties from `WebFormsPageBase` instead)
- [ ] No Minimal API endpoints created for page actions (use shim methods instead; minimal APIs are ONLY for cookie auth operations)
- [ ] ✅ `Page.Title` / `Page.MetaDescription` — works AS-IS via WebFormsPageBase
- [ ] ✅ `Request.QueryString["key"]` — works AS-IS via RequestShim
- [ ] ✅ `Cache["key"]` — works AS-IS via CacheShim
- [ ] `<form runat="server">` removed (or converted to `<WebFormsForm>` / `<EditForm>` if validators present)
- [ ] Query parameters converted (`[QueryString]` → `[SupplyParameterFromQuery]`)
- [ ] Route parameters converted (`[RouteData]` → `[Parameter]` with `@page` route)
- [ ] `@using` statements added for model namespaces
- [ ] `@inject` statements added for required services

### Layer 3 — Architecture Decisions

- [ ] Data access pattern decided (injected service, EF Core, Dapper, etc.)
- [ ] Data service implemented and registered in `Program.cs`
- [ ] Session state: basic usage works AS-IS via SessionShim; if persistent/distributed state needed, replace with `ProtectedSessionStorage` or scoped service (OPTIONAL — shim works correctly)
- [ ] Response.Redirect: works AS-IS via ResponseShim; if removing BWFC dependency, replace with `NavigationManager.NavigateTo()` (OPTIONAL — shim works correctly)
- [ ] Request.QueryString: works AS-IS via RequestShim; if cleaner Blazor pattern desired, replace with `[SupplyParameterFromQuery]` (OPTIONAL — shim works correctly)
- [ ] Authentication/authorization wired (if page requires auth)
- [ ] Third-party integrations ported (API calls, payment, etc.)
- [ ] Route registered and tested (`@page` directive matches expected URL)
- [ ] ViewState-dependent logic converted to component fields

### Verification

- [ ] Page builds without errors (`dotnet build`)
- [ ] Page renders in browser without exceptions
- [ ] Visual layout matches original Web Forms page
- [ ] All interactive features work (buttons, forms, navigation, sorting, paging)
- [ ] No JavaScript console errors in browser dev tools
- [ ] Data displays correctly (correct records, correct formatting)
- [ ] Form submissions work (validation fires, data saves)

### Optional: Refactor to Native Blazor (post-migration)

> These are optional improvements you can make after the app is fully functional. Shim-based code works correctly — these refactors reduce BWFC dependency and adopt native Blazor patterns.

- [ ] `Response.Redirect("~/path")` → `NavigationManager.NavigateTo("/path")` (if removing ResponseShim dependency)
- [ ] `Session["key"]` → scoped service or `ProtectedSessionStorage` (if needing persistence/distribution)
- [ ] `Request.QueryString["key"]` → `[SupplyParameterFromQuery]` parameter (cleaner Blazor pattern)
- [ ] `Cache["key"]` → `IMemoryCache` or `IDistributedCache` (if needing distributed cache)
- [ ] `ViewState["key"]` → component fields (ViewState is in-memory only, no page serialization)
- [ ] `Page.ClientScript.RegisterStartupScript(...)` → direct `IJSRuntime` calls

### L3-opt — Performance Optimization Pass (Optional, run after Verification ✅)

> Run after the app is fully functional. Use the [`l3-performance-optimization` skill](skills/l3-performance-optimization/SKILL.md).

- [ ] `OnInitialized` with DB calls → `OnInitializedAsync` (✅ Safe)
- [ ] Sync EF Core calls → async equivalents (`ToListAsync`, `SaveChangesAsync`, etc.) (✅ Safe)
- [ ] Read-only queries have `AsNoTracking()` (✅ Safe)
- [ ] String `Include("Nav")` replaced with lambda `Include(x => x.Nav)` (✅ Safe)
- [ ] `Task.Result` / `Task.Wait()` anti-patterns removed (✅ Safe)
- [ ] `@key` added to `@foreach` loops rendering components (✅ Safe)
- [ ] `[SupplyParameterFromQuery]` replaces manual `NavigationManager.Uri` parsing (✅ Safe)
- [ ] String concatenation in render logic → `$""` interpolation (✅ Safe)
- [ ] `[EditorRequired]` added to mandatory component parameters (✅ Safe)
- [ ] Heavy inline `@code` blocks (>50 lines) extracted to code-behind (✅ Safe)
- [ ] `AddDbContext` → `AddDbContextFactory` + `using var db = DbFactory.CreateDbContext()` (⚠️ Review)
- [ ] Multi-collection `Include()` chains evaluated for `AsSplitQuery()` (⚠️ Review)
- [ ] `[StreamRendering]` considered for pages with async data loads (⚠️ Review)
- [ ] `ShouldRender()` considered for high-frequency-render leaf components (⚠️ Review)
```

---

## Usage Tips

### For GitHub Issues

Create one issue per page (or per group of related pages). Paste the template above and fill in the header fields. As you work through the migration, check items off. This gives your team visibility into migration progress.

### For Tracking Documents

Create a single `MIGRATION-TRACKING.md` in your project. Paste one copy of the checklist per page. Use it as a daily standup reference:

```markdown
# Migration Tracking

## Completed
- [x] Default.aspx → Default.razor (Trivial) — Done 2026-03-01
- [x] About.aspx → About.razor (Trivial) — Done 2026-03-01

## In Progress
- [ ] ProductList.aspx → ProductList.razor (Medium) — Layer 2

## Not Started
- [ ] ShoppingCart.aspx → ShoppingCart.razor (Medium)
- [ ] Login.aspx → Login.razor (Complex)
```

### Recommended Migration Order

Migrate pages in this order to minimize blocked work:

1. **Layout** — `Site.Master` → `MainLayout.razor` (everything depends on this)
2. **Leaf pages** — About, Contact, Error pages (trivial, builds confidence)
3. **Read-only data pages** — Product list, catalog (medium, tests data binding)
4. **CRUD pages** — Cart, admin, forms (medium-complex, tests event handling)
5. **Auth-dependent pages** — Login, account management (complex, requires Identity setup)
6. **Integration pages** — Checkout, payment, external APIs (complex, requires Layer 3)

---

## Cross-References

- [QUICKSTART.md](QUICKSTART.md) — the full step-by-step walkthrough
- [METHODOLOGY.md](METHODOLOGY.md) — why the checklist is organized by layer
- [CONTROL-COVERAGE.md](CONTROL-COVERAGE.md) — complexity ratings for deciding page complexity

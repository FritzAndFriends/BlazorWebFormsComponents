# Per-Page Migration Checklist

**Copy this template for each page you migrate.** Use it as a GitHub issue body, a markdown checklist in your tracking doc, or paste it into your project management tool.

The checklist is organized by the [three-layer pipeline](METHODOLOGY.md). Work top to bottom — each section assumes the previous one is complete.

> ## 🚫 BWFC-FIRST RULE: Every `asp:` control MUST become a BWFC component.
> Never replace BWFC components with raw HTML (`<table>`, `<input>`, `<span>`, `<a>`, `@foreach`).
> This is verified at the end of every layer.

---

## Template

```markdown
## Page: [PageName.aspx] → [PageName.razor]

**Source:** `[path/to/PageName.aspx]`
**Target:** `[path/to/PageName.razor]`
**Complexity:** [Trivial / Easy / Medium / Complex]
**Notes:** [Any page-specific context — what this page does, key controls used]

### Layer 1 — Automated (bwfc-migrate.ps1)

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
- [ ] Static files copied to `wwwroot/` preserving directory structure
- [ ] CSS stylesheet links extracted from master page to `App.razor`
- [ ] **🚫 BWFC VERIFICATION: Control preservation verified (no deficit warnings)**
- [ ] **🚫 BWFC VERIFICATION: All asp: controls present as BWFC components in output**

### Layer 2 — Copilot-Assisted (Structural Transforms)

- [ ] `SelectMethod` → `Items` (or `DataItem`) binding wired
- [ ] Data loading moved to `OnInitializedAsync`
- [ ] Template `Context="Item"` variables added to all templates
- [ ] Event handlers converted to Blazor signatures (remove `sender`, `EventArgs`)
- [ ] `Page_Load` → `OnInitializedAsync`, `IsPostBack` checks removed
- [ ] Navigation calls converted (`Response.Redirect` → `NavigationManager.NavigateTo`)
- [ ] `<form runat="server">` removed (or converted to `<EditForm>` if validators present)
- [ ] `Session["key"]` references identified and marked for Layer 3
- [ ] Query parameters converted (`[QueryString]` → `[SupplyParameterFromQuery]`)
- [ ] Route parameters converted (`[RouteData]` → `[Parameter]` with `@page` route)
- [ ] `@using` statements added for model namespaces
- [ ] `@inject` statements added for required services
- [ ] **🚫 BWFC VERIFICATION: No asp: controls were flattened to raw HTML**
- [ ] **🚫 BWFC VERIFICATION: GridView/ListView/Repeater preserved (not @foreach)**
- [ ] **🚫 BWFC VERIFICATION: TextBox/CheckBox/Button preserved (not <input>/<button>)**
- [ ] **🚫 BWFC VERIFICATION: HyperLink/Label/Panel preserved (not <a>/<span>/<div>)**
- [ ] **🚫 BWFC VERIFICATION: LoginView/LoginStatus preserved (not @if block)**

### Layer 3 — Architecture Decisions

- [ ] Data access pattern decided (injected service, EF Core, Dapper, etc.)
- [ ] Data service implemented and registered in `Program.cs`
- [ ] Session state replaced with appropriate Blazor pattern (scoped service / ProtectedSessionStorage)
- [ ] Authentication/authorization wired (if page requires auth)
- [ ] Third-party integrations ported (API calls, payment, etc.)
- [ ] Route registered and tested (`@page` directive matches expected URL)
- [ ] ViewState-dependent logic converted to component fields

### Verification

- [ ] **🚫 BWFC FINAL CHECK: All original asp: controls preserved as BWFC components**
- [ ] **🚫 BWFC FINAL CHECK: BWFC utility features configured (AddBlazorWebFormsComponents, WebFormsPageBase, Page component)**
- [ ] Page builds without errors (`dotnet build`)
- [ ] Page renders in browser without exceptions
- [ ] Visual layout matches original Web Forms page
- [ ] Static files accessible in browser (images, CSS load correctly)
- [ ] `UseStaticFiles()` present in `Program.cs` before `MapStaticAssets()`
- [ ] CSS links in `App.razor` `<head>` (standard Blazor pattern)
- [ ] JS references in `App.razor` after `<Routes>` (standard Blazor pattern)
- [ ] Image paths in templates match `wwwroot/` directory structure
- [ ] If LoginView→AuthorizeView: auth services registered in `Program.cs`
- [ ] All interactive features work (buttons, forms, navigation, sorting, paging)
- [ ] No JavaScript console errors in browser dev tools
- [ ] Data displays correctly (correct records, correct formatting)
- [ ] Form submissions work (validation fires, data saves)
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

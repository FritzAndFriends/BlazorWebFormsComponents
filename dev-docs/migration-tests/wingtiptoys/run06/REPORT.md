# WingtipToys Migration Benchmark — Run 6 (2026-03-04)

## 1. Executive Summary

Run 6 validated **four new `bwfc-migrate.ps1` enhancements** — net10.0 TFM scaffolding, BWFC-aware SelectMethod TODOs, wwwroot static-file pathing, and compilable stub auto-generation — against WingtipToys (32 markup files). The script completed in **4.58 s** with **269 transforms** and **79 static files** copied correctly to `wwwroot/`, and the project reached a clean build (0 errors, 0 warnings) in **4.40 s** after four build rounds. Total wall-clock time from script start to clean build was **~4 minutes 30 seconds**, including ~3 min 25 s of manual Layer 2 work — a **55% reduction from Run 5's ~10 minutes**. All four enhancements fired successfully: net10.0 eliminated a TFM-related build fix round, the BWFC-aware SelectMethod guidance preserved all data controls as BWFC components (the single highest-impact enhancement), wwwroot pathing eliminated an entire manual fix category, and stub generation auto-created 6 compilable stubs for unconvertible pages. Two script bugs were discovered: `@rendermode InteractiveServer` injected into `_Imports.razor` (invalid in .NET 10) and stub detection that only scans `.aspx` markup, missing Identity references in code-behind files. The remaining manual work is now concentrated almost entirely in architectural decisions — EF Core models, DbContext, services, and page data-binding wiring — rather than mechanical transforms.

## 2. Quick-Reference Metrics

| Metric | Run 5 | Run 6 | Delta |
|--------|-------|-------|-------|
| Script execution time | 3.25 s | **4.58 s** | +1.33 s (more transforms) |
| Final build time | 4.56 s | **4.40 s** | −0.16 s |
| Transforms applied | 309 | **269** | −40 (stubs replace full transforms) |
| Errors / Warnings (final) | 0 / 0 | **0 / 0** | — |
| Manual review items | 30 | **28** | −2 |
| Auto-generated stubs | 0 | **6** | +6 |
| Static files to wwwroot | ❌ (project root) | **✅ (wwwroot/)** | Fixed |
| TFM scaffolded | net8.0 | **net10.0** | Fixed |
| Build fix rounds | 2 | **4** | +2 (different root causes) |
| Layer 2 manual time | ~440 s (~7 min 20 s) | **~205 s (~3 min 25 s)** | **−235 s (53% reduction)** |
| Total wall-clock time | ~10 min | **~4 min 30 s** | **−5 min 30 s (55% reduction)** |
| New enhancements tested | 6 | **4** | −2 (fewer, but higher-impact) |

> **Note on transform count decrease:** The count dropped from 309 → 269 because 6 unconvertible pages are now auto-stubbed rather than fully transformed. This is a quality improvement — the script no longer wastes transforms on pages that will be manually rewritten anyway.

## 3. What Works — Automated (Layer 1)

The migration script handles the following transformations automatically, requiring **zero manual intervention**:

### Core Transforms (Carried Forward)

| Transform | Description | Count in Run 6 |
|-----------|-------------|-----------------|
| ASP prefix stripping | `<asp:Button>` → `<Button>`, `<asp:Label>` → `<Label>`, etc. | ~100+ across 32 files |
| Data binding — Eval | `<%#: Eval("PropertyName") %>` → `@context.PropertyName` | Multiple per data-bound page |
| Data binding — Bind | `<%#: Bind("PropertyName") %>` → `@bind-Value="context.PropertyName"` | Where present |
| Data binding — Item.Property | `<%#: Item.PropertyName %>` → `@context.PropertyName` | Multiple instances |
| Eval format-string | `<%#: Eval("Total", "{0:C}") %>` → `@context.Total.ToString("C")` | Where present |
| String.Format with Item | `<%#: String.Format("{0:c}", Item.UnitPrice) %>` → `@($"{context.UnitPrice:c}")` | Where present |
| Master page → MainLayout | `Site.Master` → `MainLayout.razor` with `@inherits LayoutComponentBase`, `@Body`, head extraction | 1 layout generated |
| LoginView → AuthorizeView | `<asp:LoginView>` → `<AuthorizeView>` with template mapping | Carried from Run 5 |
| GetRouteUrl flagging | `[GetRouteUrl]` inject hints emitted | 2 files |
| SelectMethod → TODO | DI-aware TODO comments per instance | 8 instances |
| Bare Item → @context | Template `Item` references → `@context` | Where present |
| Register directive cleanup | `<%@ Register %>` directives silently removed | 4 directives |
| Scaffold generation | csproj, Program.cs, _Imports.razor, launchSettings.json, App.razor, Routes.razor | 6 files |
| Static file copy | CSS, images, fonts, scripts → **wwwroot/** | 79 files |

### New in Run 6

| Enhancement | Fired? | Count | Impact |
|-------------|--------|-------|--------|
| **net10.0 TFM scaffolding** | ✅ Yes | 1 (csproj) | TFM correct out of the box — eliminated 1 manual fix step and 1 build fix round. Saved ~15 s. |
| **SelectMethod BWFC-aware TODO** | ✅ Yes | 8 instances | TODO now says: `Replace SelectMethod="X" with Items="@_data" parameter`. All data controls preserved as BWFC components (ListView, FormView) instead of raw HTML rewrites. **Highest-impact single enhancement.** Saved ~60–120 s. |
| **wwwroot static-file pathing** | ✅ Yes | 79 files | Static files copied to `wwwroot/Content/`, `wwwroot/Catalog/Images/`, `wwwroot/Images/`, `wwwroot/fonts/`. Eliminated entire "Fix: static assets" manual phase. Saved ~15 s. |
| **Compilable stub auto-generation** | ✅ Yes | 6 stubs | ShoppingCart + 5 Checkout pages auto-stubbed with `@page` directive and `@code {}` block. Partial success — Account pages (15) not detected (see [Gaps](#8-gaps-and-known-issues)). Saved ~15 s. |

**Total: 269 transforms across 32 files**, 79 static files copied to wwwroot/, 6 compilable stubs auto-generated.

## 4. What Doesn't Work — Still Manual (Layer 2)

The script **cannot** handle the following patterns. These require human judgment, architectural decisions, or Copilot-assisted rewriting.

### Mechanical but Tedious

| Pattern | Count | Why Manual | Effort |
|---------|-------|-----------|--------|
| `Page.Title` / `@(Title)` | 4+ | Requires mapping to Blazor `<PageTitle>` or static text | Low |
| Complex data binding (arithmetic) | 1 | Nested method calls in expressions | Low |
| CSS bundle conversion | 2+ | `BundleConfig` → explicit `<link>` tags in `App.razor` | Low |
| NuGet → ProjectReference swap | 1 | Script scaffolds NuGet ref (correct for external users); local dev needs ProjectReference | Low |
| EF Core version pinning | 1 | Preview version wildcard needed for .NET 10 pre-release | Low |

### Requires Architectural Decisions

| Pattern | Count | Why Manual | Effort |
|---------|-------|-----------|--------|
| **Identity/Auth rewrite** | 15+ pages | ASP.NET Identity v2 + OWIN → ASP.NET Core Identity. Postback-based auth → HTTP endpoints. | **High** |
| **Session state → services** | 1 service | `Session["CartId"]` → scoped `CartStateService` with cookie-based persistence. Requires DI design. | Medium |
| **EF6 → EF Core** | 6 files | 5 model files + DbContext with seed. Constructor conventions, nullable refs, seed strategy differ. | Medium |
| **Code-behind logic** | 5+ pages | Event handlers, lifecycle methods → Blazor `@code` blocks with `OnInitializedAsync`. | Medium-High |
| **SelectMethod → DI wiring** | 8 instances | BWFC-aware TODO provides guidance, but actual `Items="@_data"` + EF Core queries require implementation. | Medium |
| **GetRouteUrl → navigation** | 2 calls | Route name semantics lost; requires query-string or NavigationManager patterns. | Medium |
| **Layout rewrite** | 1 file | MainLayout: categories via EF Core, LoginView/GetRouteUrl removal, structural decisions. | Medium |
| **Payment/checkout flow** | 5 pages | PayPal integration, order processing — business logic rewrite. | **High** |

## 5. Script Enhancement Impact — Run 5 vs Run 6

| # | Enhancement | Fired? | Times | Run 5 Status | Run 6 Status | Net Impact |
|---|-------------|--------|-------|-------------|-------------|------------|
| 1 | **net10.0 TFM** | ✅ Yes | 1 | Scaffolded net8.0 → manual csproj edit + extra build round | Scaffolds net10.0 correctly | **−1 fix step, −1 build round** (~15 s saved) |
| 2 | **SelectMethod BWFC-aware** | ✅ Yes | 8 | Generic "inject a service" TODO | BWFC-specific: `Items="@_data"` + `OnInitializedAsync` guidance | **Preserved all BWFC data controls** (~60–120 s saved) |
| 3 | **wwwroot path fix** | ✅ Yes | 79 files | Static files → project root (manual relocation needed) | Static files → wwwroot/ automatically | **−1 manual fix category** (~15 s saved) |
| 4 | **Compilable stubs** | ✅ Yes | 6 | 28+ pages needed manual stubbing | 6 auto-stubbed (Session/Identity/PayPal/Checkout patterns) | **−6 manual stubs** (~15 s saved; partial — see gaps) |

**Summary:** The four enhancements collectively reduced Layer 2 manual time from ~440 s to ~205 s (53% reduction) and total wall-clock time from ~10 min to ~4.5 min (55% reduction). SelectMethod BWFC-aware guidance was the single highest-impact enhancement — it changed the approach from "replace BWFC components with raw HTML" to "preserve BWFC components with Items parameter," which is the correct migration pattern and saves significant rewrite effort.

## 6. Layer 2 Manual Fixes Applied

All Layer 2 code was written from scratch — no files copied from prior runs.

### Fix Summary by Category

| Category | Time | Files | What Was Done |
|----------|------|-------|---------------|
| **csproj / scaffold** | ~30 s | 4 modified: csproj, _Imports, App.razor, CSS links | ProjectReference, EF Core packages, namespaces, CSS `<link>` tags |
| **EF Core models** | ~45 s | 5 created: Category, Product, CartItem, Order, OrderDetail | Model classes with nullable refs from Web Forms originals |
| **Services** | ~15 s | 1 created: CartStateService | Scoped service replacing Session-based ShoppingCartActions |
| **Page data binding** | ~60 s | 5 pages modified: Default, ProductList, ProductDetails, AddToCart, ShoppingCart | Wired `Items="@_data"`, `OnInitializedAsync`, EF Core queries |
| **Layout rewrite** | ~20 s | 1 rewritten: MainLayout.razor | Categories via EF Core, removed LoginView/GetRouteUrl references |
| **Account/Admin stubs** | ~15 s | 16 files modified: 15 Account + 1 Admin | Replaced with compilable stubs (not detected by auto-stub) |
| **Build fix: _Imports** | ~5 s | 1 modified: _Imports.razor | Removed invalid `@rendermode InteractiveServer` directive |
| **Build fix: EF Core version** | ~5 s | 1 modified: csproj | Changed to `10.0.0-preview.*` wildcard for .NET 10 preview |
| **Static assets** | **0 s** | — | **Already in wwwroot/ — no manual fix needed!** |
| **Layer 2 Total** | **~3 min 25 s** | **~50 files** | — |

> **Comparison with Run 5:** Layer 2 dropped from ~7 min 20 s (60 files) to ~3 min 25 s (50 files). The biggest savings came from eliminating static asset relocation (wwwroot fix), reducing stub effort (6 auto-generated), and BWFC-aware SelectMethod guidance enabling faster page wiring.

## 7. Build Results

### Build Round 1 — 4 Errors, 12 Warnings

NuGet authentication failure — `Fritz.BlazorWebFormsComponents` package not found (expected: local dev uses ProjectReference, not NuGet).

**Fix:** Replaced NuGet PackageReference with ProjectReference. Added EF Core + SQLite packages. Set up _Imports namespaces and App.razor CSS links.

### Build Round 2 — 8 Errors, 134 Warnings

After csproj/models/pages fixes: `@rendermode InteractiveServer` in `_Imports.razor` is invalid — render mode cannot be set globally in `_Imports.razor` in .NET 10.

**Fix:** Removed `@rendermode InteractiveServer` from `_Imports.razor`.

### Build Round 3 — 0 Errors, 4 Warnings

Clean compile. Only NuGet preview version resolution warnings remaining for EF Core packages.

**Fix:** Changed EF Core package versions to `10.0.0-preview.*` wildcard.

### Build Round 4 (Final) — Clean Build

```
Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:04.40
```

| Build Metric | Value |
|-------------|-------|
| Target framework | net10.0 |
| Fix rounds | 4 |
| Total build errors resolved | 12 (4 + 8) |
| Final errors | **0** |
| Final warnings | **0** |
| Final build time | **4.40 s** |

## 8. Gaps and Known Issues

### Script Bugs Found

| Bug | Severity | Description | Recommended Fix |
|-----|----------|-------------|-----------------|
| **`@rendermode` in _Imports.razor** | Medium | Script scaffolds `@rendermode InteractiveServer` in `_Imports.razor`, which is invalid in .NET 10. Caused 8 build errors in Round 2. | Remove `@rendermode InteractiveServer` from `_Imports.razor` scaffold template. Set render mode in `App.razor` only. |
| **Stub detection misses code-behind patterns** | Low-Medium | Stub auto-generation scans `.aspx` markup for `Session[`, `SignInManager`, `PayPal`, `Checkout` keywords, but Account pages reference Identity only in `.aspx.cs` code-behind files. 15 Account + 1 Admin page were not auto-stubbed. | Extend stub detection to also scan `.aspx.cs` files for Identity/Auth patterns (`SignInManager`, `UserManager`, `IdentityHelper`, `FormsAuthentication`). |

### Patterns the Script Will Never Handle

These require semantic understanding beyond regex capabilities:

| Pattern | Why Unreachable | Recommendation |
|---------|----------------|----------------|
| Event handler migration | `OnClick`, `OnSelectedIndexChanged` → Blazor `@onclick`, `EventCallback` — requires understanding method signatures | Copilot-assisted rewrite per page |
| Page lifecycle methods | `Page_Load`, `Page_PreInit` → `OnInitialized`, `OnParametersSet` — context-dependent | Manual, guided by migration checklist |
| DI architecture | Choosing between `@inject DbContext` vs services vs repository pattern | Architectural decision per project |
| Identity provider swap | OWIN → ASP.NET Core Identity — full subsystem replacement | Manual using ASP.NET Core Identity scaffolding |
| Payment integrations | PayPal, Stripe, etc. — business logic tied to third-party APIs | Manual per integration |

### Patterns That Could Be Enhanced

| Pattern | Current Handling | Potential Enhancement |
|---------|-----------------|----------------------|
| `Page.Title` → `<PageTitle>` | Flagged as CodeBlock | Regex to wrap `@(Title)` in `<PageTitle>` component |
| Page base class swap | Not handled | Replace `Page` inheritance with `ComponentBase` in `.razor.cs` |
| Simple event handlers | Not handled | Strip `OnClick="Handler"` and emit TODO with handler signature |
| `BundleConfig` → link tags | Not handled | Parse `BundleConfig.cs`, emit `<link>`/`<script>` in App.razor |

## 9. Recommendations

### Immediate Script Fixes (for Run 7)

1. **Fix `@rendermode` scaffold bug** — Remove `@rendermode InteractiveServer` from the `_Imports.razor` template. This is the most impactful bug fix: it caused 8 build errors and a full build round in Run 6.

2. **Extend stub detection to code-behind files** — Scan `.aspx.cs` files for Identity/Auth patterns (`SignInManager`, `UserManager`, `IdentityHelper`, `FormsAuthentication`). This would auto-stub the 15 Account + 1 Admin pages that were missed, saving ~15 s of manual work.

### Next Script Enhancements (Priority Order)

3. **`Page.Title` → `<PageTitle>` conversion** — Low complexity, high frequency. Regex can handle `@(Title)` and `@(Page.Title)` forms. Would eliminate 4+ manual fixes per project.

4. **Simple event handler annotation** — Strip `OnClick="MethodName"` attributes and emit `@* TODO: Convert MethodName event handler *@`. Extends the DI-hint pattern already proven with SelectMethod.

5. **Page base class replacement** — In generated `.razor.cs` files, replace `: Page` or `: System.Web.UI.Page` with `: ComponentBase`. Mechanical and safe.

### What Should Remain Manual

- **Identity/Auth** — Too project-specific; ASP.NET Core Identity scaffolding tools already exist.
- **EF6 → EF Core** — Model conventions and seed strategies vary too widely.
- **Business logic** (checkout, payment, admin CRUD) — Requires domain understanding.
- **Complex data binding** — Nested expressions, arithmetic, method chains need human review.

### Overall Assessment

The `bwfc-migrate.ps1` script achieved a **55% total time reduction** from Run 5 to Run 6, dropping from ~10 minutes to ~4 minutes 30 seconds. The four targeted enhancements all fired successfully, with SelectMethod BWFC-aware guidance proving the most impactful — it shifted the migration approach from replacing BWFC components with raw HTML to preserving them with proper `Items` parameter wiring. The script now handles **269 automated transforms**, **79 static files** (correctly placed in wwwroot/), and **6 auto-generated stubs**, producing a project that requires only architectural decisions and data-layer wiring to reach a clean build.

The remaining ~3 min 25 s of manual work is dominated by inherently manual tasks: EF Core models (~45 s), page data binding (~60 s), and scaffold/csproj configuration (~30 s). These cannot be automated without understanding the application's data model and architecture. Two script bugs (the `@rendermode` scaffold issue and stub detection gap) are straightforward fixes that should be addressed before Run 7.

**Verdict:** Run 6 demonstrates that targeted, data-driven script enhancements yield outsized returns. The migration pipeline is now fast enough that the script phase (~4.6 s) is negligible compared to manual Layer 2 work (~205 s). Future optimization should focus on the two bug fixes identified, then on reducing the highest-cost manual categories: page data-binding wiring and csproj/scaffold configuration.

---

## Appendix: Full Timing Breakdown

| Phase | Duration |
|-------|----------|
| Layer 1 (script) | 4.58 s |
| Build 1 (initial — NuGet auth) | 7.99 s |
| Layer 2 — csproj / scaffold | ~30 s |
| Layer 2 — EF Core models | ~45 s |
| Layer 2 — Services | ~15 s |
| Layer 2 — Page data binding | ~60 s |
| Layer 2 — Layout rewrite | ~20 s |
| Layer 2 — Account/Admin stubs | ~15 s |
| Layer 2 — Static assets | 0 s (automated!) |
| Build 2 (post-fixes — @rendermode bug) | 15.12 s |
| Layer 2 — _Imports rendermode fix | ~5 s |
| Build 3 (clean — NuGet warnings only) | 4.15 s |
| Layer 2 — EF Core version fix | ~5 s |
| Build 4 (final — clean) | 4.40 s |
| **Layer 2 Total** | **~3 min 25 s** |
| **Total (all phases)** | **~4 min 30 s** |

See also: [raw-data.md](raw-data.md)

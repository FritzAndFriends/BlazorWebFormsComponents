# WingtipToys Migration Benchmark — Run 7 (2026-03-06)

## 1. Executive Summary

Run 7 is the first end-to-end benchmark that includes **automated acceptance tests**, validating not just compilation but runtime behavior of the migrated WingtipToys application. The `bwfc-migrate.ps1` script completed in **3.33 seconds** — a 27% speedup over Run 6 — applying **366 transforms** across 32 Web Forms files, copying **80 static files** to `wwwroot/`, and producing 35 `.razor` files with 35 `.razor.cs` code-behinds plus full scaffolding. The project reached a clean build (0 errors, 0 warnings) after only **2 build rounds**, down from Run 6's 4 rounds.

Layer 2 manual work was substantial but focused: Cyclops rewrote **33 code-behind files** from Web Forms page lifecycle to Blazor `ComponentBase`, modified **14 files** (csproj, Program.cs, layouts, core pages), and created **8 new files** (EF Core models, DbContext, and services). The application ships as a fully functional Blazor Server app with a SQLite-backed product catalog, shopping cart with add/update/remove, and cookie-based authentication — all served through BWFC components.

The headline result is **14/14 acceptance tests passing** across navigation (6), shopping cart (4), and authentication (3+1 end-to-end). Getting there required 3 fix iterations: Bootstrap CSS link tags in `App.razor`, replacing singleton `MockAuthenticationStateProvider` with scoped + ASP.NET Core cookie auth, and using anchor-based form submission (`<a role="button">`) instead of `<button onclick>` for Blazor-compatible form posts. The final output comprises **601 total files** in `samples/AfterWingtipToys/`, representing a complete, runnable migration of the WingtipToys 2013 sample application.

## 2. Quick-Reference Metrics

| Metric | Run 6 | Run 7 | Delta |
|--------|-------|-------|-------|
| Script execution time | 4.58 s | **3.33 s** | −1.25 s (27% faster) |
| Transforms applied | 269 | **366** | +97 (36% more) |
| Files processed | 32 | **32** | — |
| Static files to wwwroot | 79 | **80** | +1 |
| .razor files generated | — | **35** | — |
| .razor.cs code-behinds | — | **35** | — |
| Manual review items | 28 | **46** | +18 (EventHandlers, CodeBlocks, SelectMethods) |
| Script errors | 0 | **0** | — |
| Build fix rounds | 4 | **2** | −2 (50% fewer) |
| Layer 2 files rewritten | — | **33** | — |
| Layer 2 files modified | — | **14** | — |
| Layer 2 files created | — | **8** | — |
| Acceptance tests | N/A | **14/14 PASS** | NEW |
| Test fix iterations | N/A | **3** | NEW |
| Final errors / warnings | 0 / 0 | **0 / 0** | — |
| Total files in output | — | **601** | — |

> **Note on transform count increase:** The jump from 269 → 366 transforms reflects expanded script coverage — more event handlers flagged, more data-binding patterns converted, and broader CodeBlock detection. Run 6's lower count was partly due to stub-based skipping of unconvertible pages.

## 3. What Works — Automated (Layer 1)

The migration script handles the following transformations automatically, requiring **zero manual intervention**:

### Core Transforms

| Transform | Description | Count in Run 7 |
|-----------|-------------|-----------------|
| ASP prefix stripping | `<asp:Button>` → `<Button>`, `<asp:Label>` → `<Label>`, etc. | ~100+ across 32 files |
| Data binding — Eval | `<%#: Eval("PropertyName") %>` → `@context.PropertyName` | Multiple per data-bound page |
| Data binding — Bind | `<%#: Bind("PropertyName") %>` → `@bind-Value="context.PropertyName"` | Where present |
| Data binding — Item.Property | `<%#: Item.PropertyName %>` → `@context.PropertyName` | Multiple instances |
| Eval format-string | `<%#: Eval("Total", "{0:C}") %>` → `@context.Total.ToString("C")` | Where present |
| String.Format with Item | `<%#: String.Format("{0:c}", Item.UnitPrice) %>` → `@($"{context.UnitPrice:c}")` | Where present |
| Master page → MainLayout | `Site.Master` → `MainLayout.razor` with `@inherits LayoutComponentBase`, `@Body` | 1 layout |
| LoginView → AuthorizeView | `<asp:LoginView>` → `<AuthorizeView>` with template mapping | Where present |
| GetRouteUrl flagging | `[GetRouteUrl]` inject hints emitted | 2 files |
| SelectMethod → TODO | BWFC-aware TODO comments with `Items="@_data"` guidance | 8+ instances |
| Bare Item → @context | Template `Item` references → `@context` | Where present |
| Register directive cleanup | `<%@ Register %>` directives silently removed | Multiple |
| Scaffold generation | csproj, Program.cs, _Imports.razor, App.razor, Routes.razor, launchSettings.json | 6 files |
| Static file copy | CSS, images, fonts, scripts → **wwwroot/** | 80 files |
| Event handler flagging | `OnClick`, `OnSelectedIndexChanged`, etc. flagged for review | NEW — counted in 46 review items |
| Mock auth service stubs | Auto-generated authentication service scaffolding | NEW |

### Layer 1 Output Summary

- **35 .razor files** generated from 32 Web Forms markup files
- **35 .razor.cs code-behinds** generated with stub code
- **80 static files** copied to `wwwroot/` (Content/, Catalog/Images/, Scripts/, fonts/)
- **6 scaffold files** (csproj, Program.cs, _Imports.razor, App.razor, Routes.razor, launchSettings.json)
- **46 items flagged for manual review** (EventHandlers, CodeBlocks, SelectMethods)
- **0 errors** during script execution

## 4. What Doesn't Work — Still Manual (Layer 2)

### Mechanical but Tedious

| Pattern | Count | Why Manual | Effort |
|---------|-------|-----------|--------|
| Class name / EventArgs mismatches | 7 (Build R1) | `MouseEventArgs` vs `EventArgs`, Web Forms class names in code-behinds | Low |
| CSS bundle → link tags | 2 | Bootstrap + Site.css `<link>` tags needed in `App.razor` | Low |
| NuGet → ProjectReference swap | 1 | Script scaffolds NuGet ref; local dev needs ProjectReference | Low |
| Enum/boolean attribute conversions | Multiple | Web Forms string enums → C# enum types | Low-Medium |

### Requires Architectural Decisions

| Pattern | Files Affected | Why Manual | Effort |
|---------|---------------|-----------|--------|
| **Code-behind rewrite** | 33 files | Web Forms `Page_Load` → Blazor `OnInitializedAsync` with DI | **High** |
| **EF Core models + DbContext** | 6 created | Model classes + ProductContext + SQLite + seed data — project-specific | Medium |
| **Services layer** | 2 created | ShoppingCartService, ProductService — DI architecture | Medium |
| **Program.cs wiring** | 1 modified | AddHttpContextAccessor, AddBlazorWebFormsComponents, EF Core, cookie auth | Medium |
| **Data binding wiring** | Multiple pages | SelectMethod → `Items="@_data"` + `OnInitializedAsync` with EF Core queries | Medium |
| **Event handler signatures** | Multiple | Web Forms `EventHandler` → Blazor `EventCallback`, `MouseEventArgs` | Medium |
| **Auth page stubs** | Register, Login | Functional form fields with proper HTML form submission | Medium |
| **Identity/Auth architecture** | 15+ pages | ASP.NET Identity v2 → cookie-based mock auth with real sign-in | **High** |
| **Payment/checkout flow** | 5 pages | PayPal integration — business logic rewrite, left as stubs | **High** |

### Layer 2 Work Summary

| Category | Files | What Was Done |
|----------|-------|---------------|
| Code-behind rewrites | 33 rewritten | Web Forms `Page` lifecycle → Blazor `ComponentBase` with DI injection |
| Infrastructure modifications | 14 modified | csproj, Program.cs, _Imports.razor, App.razor, layouts, core pages |
| New files created | 8 created | Models/ (5), Data/ProductContext.cs, Services/ (2) |
| **Layer 2 Total** | **55 files touched** | — |

## 5. Acceptance Test Results

### Test Project

- **Location:** `src/WingtipToys.AcceptanceTests/`
- **Framework:** xUnit + Playwright
- **Result:** **14/14 tests PASS** (verified on multiple runs)

### Test Breakdown

| Category | Test | Status |
|----------|------|--------|
| **Navigation** | `HomePage_Loads` | ✅ PASS |
| **Navigation** | `NavbarLink_LoadsPage("About")` | ✅ PASS |
| **Navigation** | `NavbarLink_LoadsPage("Contact")` | ✅ PASS |
| **Navigation** | `NavbarLink_LoadsPage("ProductList")` | ✅ PASS |
| **Navigation** | `ShoppingCartLink_LoadsPage` | ✅ PASS |
| **Navigation** | `RegisterLink_LoadsPage` | ✅ PASS |
| **Navigation** | `LoginLink_LoadsPage` | ✅ PASS |
| **Shopping Cart** | `ProductList_DisplaysProducts` | ✅ PASS |
| **Shopping Cart** | `AddItemToCart_AppearsInCart` | ✅ PASS |
| **Shopping Cart** | `UpdateCartQuantity_ChangesItemCount` | ✅ PASS |
| **Shopping Cart** | `RemoveItemFromCart_EmptiesCart` | ✅ PASS |
| **Authentication** | `RegisterPage_HasExpectedFormFields` | ✅ PASS |
| **Authentication** | `LoginPage_HasExpectedFormFields` | ✅ PASS |
| **Authentication** | `RegisterAndLogin_EndToEnd` | ✅ PASS |

> **Note:** The `NavbarLink_LoadsPage` test is a `[Theory]` with 3 `[InlineData]` cases (About, Contact, ProductList), which combined with the other `[Fact]` tests produces 14 total test cases.

### Fix Iterations Required

Three fix iterations were needed to get all 14 tests passing:

#### Iteration 1: Bootstrap CSS

**Problem:** Navbar rendered without Bootstrap styling, so button and link locators couldn't find the expected elements.

**Fix:** Added `<link>` tags for `bootstrap.min.css` and `Site.css` to `Components/App.razor`:
```html
<link rel="stylesheet" href="Content/bootstrap.min.css" />
<link rel="stylesheet" href="Content/Site.css" />
```

**Impact:** Navigation tests passed once the navbar rendered correctly.

#### Iteration 2: Cookie Authentication

**Problem:** `MockAuthenticationStateProvider` was registered as a singleton, so auth state was shared across all browser sessions — Playwright tests interfered with each other.

**Fix:** Replaced singleton registration with scoped + ASP.NET Core cookie authentication:
```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/Account/Login");
builder.Services.AddScoped<MockAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<MockAuthenticationStateProvider>());
```

**Impact:** Each browser session gets its own auth state. The `RegisterAndLogin_EndToEnd` test passes reliably.

#### Iteration 3: Anchor-Based Form Submit

**Problem:** Blazor strips `onclick` from `<button>` elements during enhanced navigation, so form submission buttons didn't work.

**Fix:** Used `<a role="button">` elements and minimal API endpoints (`/Account/DoRegister`, `/Account/DoLogin`) for form submission, plus a `form-submit.js` helper:
```html
<a role="button" onclick="document.getElementById('registerForm').submit()">Register</a>
```

**Impact:** Register and Login form submissions work correctly via HTTP POST endpoints, bypassing Blazor's enhanced navigation interception.

## 6. Build Log Summary

### Build Round 1 — 7 Errors

After Layer 1 script output + initial Layer 2 code-behind rewrites.

| Error Category | Count | Description |
|----------------|-------|-------------|
| Class name mismatches | 4 | Web Forms class names in code-behinds didn't match Razor component names |
| EventArgs vs MouseEventArgs | 2 | Web Forms `EventArgs` → Blazor `MouseEventArgs` for click handlers |
| Missing using directive | 1 | Namespace not imported in _Imports.razor |

**Fix:** Corrected class names, updated event handler signatures, added namespace import.

### Build Round 2 (Final) — Clean Build

```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

| Build Metric | Value |
|-------------|-------|
| Target framework | net10.0 |
| Fix rounds | **2** |
| Total build errors resolved | 7 |
| Final errors | **0** |
| Final warnings | **0** |

## 7. Comparison to Previous Runs

| Metric | Run 4 | Run 5 | Run 6 | Run 7 | Trend |
|--------|-------|-------|-------|-------|-------|
| Script time | 2.87 s | 3.25 s | 4.58 s | **3.33 s** | ↘ Faster |
| Transforms | 237 | 309 | 269 | **366** | ↗ More coverage |
| Static files | — | — | 79 | **80** | → Stable |
| Build rounds | 3 | 2 | 4 | **2** | ↘ Fewer |
| Manual review items | — | 30 | 28 | **46** | ↗ More flagged (better detection) |
| Layer 2 files created | — | — | ~7 | **8** | → Stable |
| Acceptance tests | N/A | N/A | N/A | **14/14 PASS** | 🆕 NEW |
| Final build | 0/0 | 0/0 | 0/0 | **0/0** | → Clean |

### Key Trend Analysis

1. **Script speed recovered:** Run 7 is 27% faster than Run 6 despite applying 36% more transforms. Likely due to optimized regex paths or reduced I/O overhead.
2. **Transform count grew significantly:** 269 → 366 (+97) reflects expanded coverage — event handler detection, broader CodeBlock flagging, and mock auth service generation.
3. **Build rounds halved:** 4 → 2 rounds indicates the script's output is closer to compilable out-of-the-box. The two Run 6 bugs (`@rendermode` in _Imports, stub detection gap) appear to be resolved.
4. **Acceptance tests are the headline:** For the first time, the migration benchmark validates *runtime behavior*, not just compilation. The 14-test suite covers navigation, shopping cart CRUD, and authentication end-to-end.

## 8. Gaps and Known Issues

### Patterns the Script Will Never Handle

| Pattern | Why Unreachable | Recommendation |
|---------|----------------|----------------|
| Event handler migration | `OnClick` → `EventCallback` requires understanding method signatures | Copilot-assisted rewrite per page |
| Page lifecycle methods | `Page_Load` → `OnInitializedAsync` — context-dependent | Manual, guided by migration checklist |
| DI architecture | Service layer design varies per project | Architectural decision |
| Identity provider swap | OWIN → ASP.NET Core Identity — full subsystem replacement | Manual using ASP.NET Core Identity scaffolding |
| Payment integrations | PayPal, Stripe — business logic tied to third-party APIs | Manual per integration |

### Patterns That Could Be Enhanced

| Pattern | Current Handling | Potential Enhancement |
|---------|-----------------|----------------------|
| `Page.Title` → `<PageTitle>` | Flagged as CodeBlock | Regex wrap in `<PageTitle>` component |
| Simple event handlers | Flagged for review | Auto-generate `EventCallback` stubs with correct signatures |
| `BundleConfig` → link tags | Not handled | Parse and emit `<link>`/`<script>` in App.razor |
| Code-behind base class | Not handled | Replace `Page` inheritance with `ComponentBase` |

### Runtime Gotchas Discovered

| Issue | Impact | Resolution |
|-------|--------|------------|
| Blazor strips `onclick` from buttons | Form submissions silently fail | Use `<a role="button">` or minimal API POST endpoints |
| Singleton auth state providers | Auth state bleeds across sessions | Register as scoped + use cookie auth |
| Missing Bootstrap CSS in App.razor | Navbar renders unstyled, locators fail | Add explicit `<link>` tags to `App.razor` `<head>` |

## 9. Recommendations for Run 8

### Script Improvements

1. **Auto-generate Bootstrap/CSS `<link>` tags** — Scan source project for CSS bundles and emit corresponding `<link>` tags in `App.razor`. This would have prevented the first test-fix iteration.

2. **Improve code-behind base class replacement** — Replace `: Page` or `: System.Web.UI.Page` with `: ComponentBase` in generated `.razor.cs` files. This is the most common Layer 2 fix (33 files in Run 7).

3. **Event handler stub generation** — Beyond flagging `OnClick` etc. for review, generate `EventCallback` stubs with the correct Blazor signatures (`MouseEventArgs`, `ChangeEventArgs`).

### Test Improvements

4. **Include acceptance test project in benchmark output** — The test project at `src/WingtipToys.AcceptanceTests/` should ship alongside the migrated app as a verification suite.

5. **Add performance benchmarks** — Time page loads and data operations to establish a performance baseline for the migrated app.

### Architecture Improvements

6. **Cookie auth as default pattern** — Run 7 proved that scoped `MockAuthenticationStateProvider` + ASP.NET Core cookie auth is the correct pattern for Blazor Server migration. Codify this in the script's `Program.cs` scaffold.

7. **Minimal API endpoints for form submission** — Document the `<a role="button">` + `MapPost` pattern as the standard approach for auth forms in migrated apps.

### Overall Assessment

Run 7 represents a **qualitative leap** in the migration benchmark: the migrated WingtipToys application is not just compilable but **functionally verified** with 14 passing acceptance tests covering navigation, shopping cart operations, and authentication. The script's performance improved (3.33 s vs 4.58 s) while coverage expanded (366 vs 269 transforms), and the build-to-clean path shortened from 4 rounds to 2. The three test-fix iterations revealed important runtime patterns (Bootstrap CSS, scoped auth, anchor-based form submit) that should inform both script improvements and migration documentation. The migration pipeline is now mature enough that **runtime correctness, not compilation**, is the primary quality gate.

---

## Appendix: Full Timing Breakdown

| Phase | Duration |
|-------|----------|
| Layer 1 (script) | 3.33 s |
| Layer 2 — code-behind rewrites (33 files) | — |
| Layer 2 — infrastructure modifications (14 files) | — |
| Layer 2 — new files created (8 files) | — |
| Build Round 1 (7 errors) | — |
| Layer 2 — class name + EventArgs fixes | — |
| Build Round 2 (clean) | — |
| Test fix iteration 1 — Bootstrap CSS | — |
| Test fix iteration 2 — Cookie auth | — |
| Test fix iteration 3 — Anchor form submit | — |
| **All 14 acceptance tests pass** | — |

> Detailed Layer 2 timing was not captured at the same granularity as Run 6. Build round count (2) and test fix iterations (3) are the primary efficiency metrics for this run.

See also: [raw-data.md](raw-data.md)

# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!--  Summarized 2026-02-27 by Scribe  covers M1M16 -->

<!-- ⚠ Summarized 2026-03-06 by Scribe — older entries archived -->

### Archived Sessions

- Core Context (2026-02-10 through 2026-02-27)
- M17-M20 Wave 1 Context (2026-02-27 through 2026-03-01)
- M20 Theming through Migration Benchmarks (2026-03-01 through 2026-03-04)
- Script & Toolkit Summary (2026-03-02 through 2026-03-04)
- GetRouteUrl, Run 5 & Toolkit Sync Summary (2026-03-04 through 2026-03-05)

<!-- ⚠ Summarized 2026-03-07 by Scribe — entries from 2026-03-05 through 2026-03-07 (pre-Run 11) archived -->

- Run 6 Script Enhancements (2026-03-05)
- @rendermode Scaffold Fix (2026-03-05)
- WebFormsPageBase Implementation (2026-03-05)
- WebFormsPage IPageService Consolidation (2026-03-05)
- LoginView Migration Script Fix (2026-03-06)
- Run 9 Script Fixes — 9 RF items (2026-03-06)
- Layer 2 AfterWingtipToys Build Conversion (2026-03-06)

### Summary (2026-03-05 through 2026-03-07 pre-Run 11)

Run 6: 4 script enhancements (TFM, SelectMethod TODO, wwwroot copy, stubs). @rendermode fix: removed standalone directive from _Imports.razor scaffold — `@rendermode` is a directive *attribute* for component instances only. WebFormsPageBase: `ComponentBase` subclass with `Page => this`, Title/MetaDescription/MetaKeywords delegates, `IsPostBack => false`. WebFormsPage consolidation: merged Page.razor head rendering into WebFormsPage via Option B. LoginView script fix: `<asp:LoginView>` → `<LoginView>` (not AuthorizeView), preserve template names. Run 9: 9 script fixes (Models copy, DbContext transform, EF6→EF Core, redirect detection, Program.cs boilerplate, Page Title extraction, QueryString/RouteData annotations, ListView GroupItemCount, csproj packages). Layer 2: full AfterWingtipToys conversion — key pattern: layout code-behind class name MUST match .razor filename. Auth pages use plain HTML forms with HTTP endpoints.

<!-- summarized 2026-03-11 by Scribe -- entries from 2026-03-07 through 2026-03-08 (Runs 11-13, script fixes) archived -->

- Run 11 -- Complete WingtipToys Migration from Scratch (2026-03-07)
- Run 11 Script Fixes -- Fix 1 (Invoke-ScriptAutoDetection) & Fix 2 (Convert-TemplatePlaceholders) (2026-03-07)
- Run 12 -- Complete WingtipToys Migration from Scratch (2026-03-07)
- LoginView Namespace Fix (2026-03-07)
- Run 13 -- Full WingtipToys Migration Pipeline, 25/25 tests (2026-03-08)

### Summary (2026-03-07 through 2026-03-08)

Run 11: Fresh WingtipToys migration from scratch (105 files, 0 errors). Key patterns: root-level `_Imports.razor` for pages outside `Components/`, partial classes must NOT specify `: ComponentBase` with `@inherits WebFormsPageBase`, auth pages use plain HTML forms to HTTP endpoints. Run 11 script fixes: `Invoke-ScriptAutoDetection` (JS files to wwwroot/Scripts/ with correct dependency order) and `Convert-TemplatePlaceholders` (placeholder elements to `@context`). Run 12: Full pipeline with Layer 2, established dual DbContext pattern (later superseded by factory-only in Run 13). LoginView namespace fix: `@using BlazorWebFormsComponents.LoginControls` required in `_Imports.razor` -- added to script template. Run 13: 25/25 tests passed (100%). Confirmed patterns: SSR default, `data-enhance-nav="false"` for minimal API links, `data-enhance="false"` for auth forms, `AddDbContextFactory` only (no dual registration), middleware order `UseAuthentication -> UseAuthorization -> UseAntiforgery`, logout must use `<a>` not `<button>`.

Team updates (2026-03-07 through 2026-03-08): Coordinator must not perform domain work; FreshWingtipToys must not be committed; migration-standards updated with Static Asset Checklist/ListView Placeholder/Action Links; migration order: fresh project first; SSR default with InteractiveServer opt-in; enhanced nav bypass for minimal API; DbContext factory-only; middleware order confirmed; logout uses link not button.

<!-- ⚠ Summarized 2026-03-11 by Scribe — entries from 2026-03-11 (ServiceCollectionExtensions through L2 structural transform) archived -->

- ServiceCollectionExtensions Enhancement (2026-03-11)
- Run 18a/18b/18c — Test-UnconvertiblePage False-Positive Investigation (2026-03-11)
- P0 Migration Script Fixes (2026-03-11)
- Standardize Generic Type Params to ItemType (2026-03-11)
- Layer 2 Structural Transform - AfterWingtipToys (2026-03-11)

### Summary (2026-03-11 pre-Run 20)

ServiceCollectionExtensions: Added `AddHttpContextAccessor()` auto-registration, `BlazorWebFormsComponentsOptions` with `EnableAspxUrlRewriting`, `UseBlazorWebFormsComponents()` middleware, `AspxRewriteMiddleware` for `.aspx` → clean URL 301 redirects. Run 18: Fixed `Test-UnconvertiblePage` false positives — 'Checkout' matched button IDs, 'PayPal' matched image URLs. Both now path-based. ShoppingCart.razor restored to full GridView markup. P0 fixes: (1) Eliminated `Test-UnconvertiblePage` — always returns `$false`, pages get TODO annotations instead of stubs. (2) `[Parameter]` regex fix — TODO on separate line, eliminates 6 CS errors. ItemType standardization: renamed all `TItemType`/`TItem` → `ItemType` across 13 files, 0 build errors. L2 AfterWingtipToys: ~60 files transformed, 0 errors, 58 RZ10012 warnings. Key gotchas: class name must match .razor filename, `@inject`/`[Inject]` duplication, `#hexcolor` → `@("...")`, `LoggedInTemplate` has no typed context.

Team updates (2026-03-11): Migration tests reorganized to `project/runNN/`. Mandatory L1→L2 pipeline with no fixes between layers. All generics standardized to `ItemType`. Test-UnconvertiblePage eliminated. Run 18 improvements prioritized by Forge.

### Run 20 L1 Script Fixes — SelectMethod Preservation + Review Item Noise Reduction (2026-03-12)

**BUG 1 FIX — SelectMethod preservation (CRITICAL):**
- `ConvertFrom-SelectMethod` (line 1048-1068) was STRIPPING `SelectMethod="MethodName"` from markup and replacing with Items TODO
- BWFC `DataBoundComponent.cs` line 16 has `SelectHandler<ItemType> SelectMethod { get; set; }` — it's a real working parameter
- Fixed: regex now captures the FULL tag including SelectMethod attribute, preserves it in output, appends TODO comment after the tag noting delegate conversion needed in L2
- Manual item text updated from "removed" to "preserved — needs delegate conversion in L2"
- Line 860 early-detection manual item also updated to reflect preservation

**BUG 2 FIX — ContentPlaceHolder review items:**
- Lines 844-853: manual items said "needs manual conversion" — misleading because BWFC provides `<ContentPlaceHolder>` component
- Updated both manual item text and TODO comments to: "BWFC provides `<ContentPlaceHolder>` component — use `<ContentPlaceHolder ID="..." />` or convert to @Body"

**BUG 3 FIX — GetRouteUrl review items:**
- Line 1030: changed from generic "Add @inject" to "BWFC provides GetRouteUrlHelper — add @inject ... and use GetRouteUrlHelper.GetRouteUrl()"
- Line 1038: changed from "Replace route name with direct URL" to "BWFC GetRouteUrlHelper supports route name — verify route is registered, or replace with direct URL"

**Key learning:** L1 script must not strip attributes that BWFC components actually support. SelectMethod is a real parameter on DataBoundComponent — preserve it and let L2 handle delegate conversion.


 Team update (2026-03-11): SelectMethod must be preserved in L1 script and skills  BWFC supports it natively via SelectHandler<ItemType> delegate. All validators exist in BWFC.

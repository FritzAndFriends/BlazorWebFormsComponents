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
- Key Test Patterns
- M17 and Issue #379 Test Summary (2026-02-27 through 2026-02-28)

### M20 Theming & FontInfo Test Summary (2026-03-01 through 2026-03-02)

**Skins & Themes pipeline tests (13 tests, Issue #368):** ThemingPipelineTests.razor validates ThemeProvider > BaseWebFormsComponent > BaseStyledComponent pipeline using Button/Label/Panel. Covers: default skin, named SkinID, explicit-overrides-theme, EnableTheming=false, no ThemeProvider, missing SkinID graceful, nested ThemeProvider override, ForeColor/CssClass/Width/Height/Font properties, multi-control theming. Total: 1426 tests.

**FontInfo sync tests (11 tests):** 9 unit tests (FontInfoSyncTests.cs) + 2 pipeline tests verifying Name/Names bidirectional sync. Setting Name updates Names, setting Names updates Name (first entry). Null/empty clears both. Last-write-wins semantics. Pipeline tests confirm theme Font.Name renders font-family via auto-sync. Total: 1437 tests.

Key patterns: Button=input, Label=span, Panel=div for theme tests. Missing SkinID returns null (no skin, not even default). FontInfo sync tests are pure C# (no bUnit). Theme font-family pipeline: ApplyThemeSkin sets Font.Name > auto-sync > Font.Names > style builder renders.

Team updates: Unified release process (PR #408), Skins & Themes roadmap (3 waves, 15 WIs).


<!-- Summarized 2026-03-03 by Scribe -- covers Issue #406 + #356 + FormView tests -->

### Issue #406, FormView & ListView CRUD Test Summary (2026-03-02 through 2026-03-03)

**ListView EditItemTemplate tests (Issue #406, 6 tests):** EditIndex matching/non-matching/negative, HandleCommand Edit/Cancel swap, null EditItemTemplate fallback. CSS class selectors (span.display vs span.edit) for template identification. TDD: 2 pass, 4 fail (confirming bug). HandleCommand tests use cut.InvokeAsync() for Blazor dispatcher context.

**FormView RenderOuterTable tests (8 tests):** Default/true/false rendering, template content, structural equivalence, empty data (text + template), edit mode. Compare element counts not raw Markup (Blazor generates unique event handler IDs per render). RenderOuterTable=false strips all table/tr/td wrappers.

**ListView CRUD Event tests (Issue #356, 43 tests):** All 16 events covered. Test categories: event firing + EventArgs data, cancellation (Cancel=true prevents downstream), event ordering (List<string> tracker), HandleCommand routing (case-insensitive, unknown->ItemCommand), modifier handlers, CancelMode detection, full CRUD lifecycle sequences, edge cases (empty items, zero paging).

Key test patterns: (1) CSS class selectors for template switching. (2) cut.InvokeAsync for HandleCommand. (3) FindAll().Count for structural comparison. (4) Event ordering via List<string> with index assertions. (5) ShouldBeGreaterThanOrEqualTo for bUnit double-render. (6) ItemCreated needs async + InvokeAsync for lifecycle callback.

Key file paths: `src/BlazorWebFormsComponents.Test/ListView/EditTemplateTests.razor`, `src/BlazorWebFormsComponents.Test/FormView/RenderOuterTable.razor`, `src/BlazorWebFormsComponents.Test/ListView/ListViewCrudEventTests.razor`.

Team updates (2026-03-02-03): Skins roadmap (Forge), M22 planned (Forge), project reframed as migration system (Jeff), ModelErrorMessage spec (Forge), themes last directive (Jeff Fritz), IOrderedDictionary on ListView EventArgs (Cyclops), WingtipToys 7-phase schedule (Forge).

 Team update (2026-03-04): PRs must target upstream FritzAndFriends/BlazorWebFormsComponents, not the fork  decided by Jeffrey T. Fritz

� Team update (2026-03-04): Run 6 improvement analysis  decided by Forge

### WebFormsPageBase Tests (2026-03-04)

**8 bUnit tests for WebFormsPageBase (all pass):** Title/MetaDescription/MetaKeywords delegate to IPageService, IsPostBack always false, Page returns this, Page.Title shim pattern delegates correctly, reading Title after setting via PageService, IsPostBack guard pattern block always executes. Test file: `src/BlazorWebFormsComponents.Test/WebFormsPageBase/WebFormsPageBaseTests.razor`. Used concrete inner `TestPage` class inheriting `WebFormsPageBase` with `BuildRenderTree` + public accessors for protected `Page` property. Registered `IPageService` as `Services.AddScoped<IPageService, PageService>()`. WebFormsPageBase inherits ComponentBase (not BaseWebFormsComponent), so no JSInterop or LinkGenerator mocking needed — simpler test setup than most component tests.

 Team update (2026-03-04): WebFormsPageBase implemented  decided by Forge, approved by Jeff

### WebFormsPage Head Rendering Tests (2026-03-05)

**7 bUnit tests for WebFormsPage head rendering (3 pass, 4 expected failures):** Tests written ahead of Cyclops's enhancement to merge Page.razor head rendering into WebFormsPage (Option B from forge-page-consolidation decision). Test file: `src/BlazorWebFormsComponents.Test/WebFormsPage/WebFormsPageHeadRenderingTests.razor`.

Tests: (1) RendersPageTitle when IPageService registered + title set — uses `FindComponent<PageTitle>()`, (2) RendersMetaDescription — checks `FindComponent<HeadContent>()` markup for `<meta name="description">`, (3) RendersMetaKeywords — same pattern for keywords, (4) DoesNotRenderHeadContent when RenderPageHead=false — asserts `FindComponents<PageTitle>().Count.ShouldBe(0)`, (5) WorksWithoutIPageServiceRegistered — verifies naming still works without IPageService in DI, (6) CascadesTheme while also rendering head content — verifies theme + head coexistence, (7) ChildContent renders normally — basic child content pass-through.

**Currently passing (3):** Tests 4, 5, 7 pass — they test existing behavior or pass vacuously since head rendering isn't implemented yet. **Expected failures (4):** Tests 1, 2, 3, 6 fail because WebFormsPage doesn't yet have IPageService integration or RenderPageHead parameter — awaiting Cyclops's implementation. Existing 6 WebFormsPage tests unaffected.

Key patterns: `FindComponent<PageTitle>()` and `FindComponent<HeadContent>()` for asserting Blazor built-in head components. `RenderPageHead="false"` captured by `BaseWebFormsComponent`'s unmatched attributes until Cyclops adds the actual parameter. Pre-set `PageService` values before rendering (not after) for initial render assertions.

� Team update (2026-03-05): WebFormsPage now includes IPageService head rendering (title + meta tags), merging Page.razor capability per Option B consolidation. Layout simplified to single <WebFormsPage> component. Page.razor remains standalone.  decided by Forge, implemented by Cyclops


 Team update (2026-03-06): CRITICAL  Git workflow: feature branches from dev, PRs target dev. NEVER push to or merge into upstream main (production releases only).  directed by Jeff Fritz

 Team update (2026-03-06): CONTROL-COVERAGE.md updated  library ships 153 Razor components (was listed as 58). ContentPlaceHolder reclassified from 'Not Supported' to Infrastructure Controls. Reference updated CONTROL-COVERAGE.md for accurate component inventory.  decided by Forge

� Team update (2026-03-06): LoginView is a native BWFC component  do NOT replace with AuthorizeView in migration guidance. Both migration-standards SKILL.md files (in .ai-team/skills/ and migration-toolkit/skills/) must be kept in sync. WebFormsPageBase patterns corrected in all supporting docs.  decided by Beast

 Team update (2026-03-06): LoginView must be preserved as BWFC component, not converted to AuthorizeView  decided by Jeff (directive)


 Team update (2026-03-06): Run 9 CSS/image failure RCA  script drops bundle refs, Layer 2 changed image paths without moving files. 5 fixes proposed.  decided by Forge

### StaticAssetTests Acceptance Tests (2026-03-06)

**12 Playwright acceptance tests for static asset / visual integrity (StaticAssetTests.cs):** Catches the Run 9 class of failures — missing CSS causing navbar bullet-list rendering, and image 404s on product pages. Tests: (1) HomePage_LoadsAtLeastOneCssFile — intercepts Playwright Response events for .css URLs, asserts count > 0. (2) CssFiles_ReturnHttp200 — collects non-200 CSS responses, fails with URL list. (3) ProductList_AllImagesLoad — iterates all <img> elements, checks src non-empty + naturalWidth > 0 via JS eval. (4) ProductList_ImageRequests_ReturnHttp200 — Response event listener for image extensions, collects failures. (5) Navbar_HasBootstrapClasses — asserts `.navbar` element exists. (6) Navbar_HasReasonableHeight — BoundingBoxAsync height ≥ 30px. (7) HomePage_HasStyledMainContent — checks .jumbotron/.carousel/main/.container exists with width > 100 + height > 50. (8) HomePage_Screenshot_VerifyLayout — full-page screenshot byte length > 5000. (9) ProductList_Screenshot_VerifyImagesAndLayout — image dimensions + screenshot size. (10) ProductDetails_Screenshot_VerifyImageAndStyling — navigates to first product detail, checks image naturalWidth + bounding box + screenshot. (11) HomePage_NoFailed_StaticAssetRequests — catch-all for any 4xx/5xx on .css/.js/.png/.jpg/.gif/.webp/.svg/.woff/.woff2/.ico.

Key patterns: Response event interception for network-level asset checks. `EvaluateAsync<int>("el => el.naturalWidth")` for broken image detection. `BoundingBoxAsync()` for CSS-applied dimension checks. Screenshot byte-size as a coarse visual sanity proxy. All follow existing Playwright collection pattern (no [Trait] used, matching other test classes).

 Team update (2026-03-07): Run 10 declared FAILED  coordinator violated protocol. Phase 1 acceptance tests applied correctly. 20/25 tests passed before failure called. Coordinator must use existing .NET test infrastructure, not Node.js alternatives.  decided by Jeffrey T. Fritz


 Team update (2026-03-07): Coordinator must not perform domain work  all code changes must route through specialist agents. Tests must run via dotnet test, not ad-hoc scripts  decided by Jeffrey T. Fritz, Beast

 Team update (2026-03-08): Default to SSR (Static Server Rendering) with per-component InteractiveServer opt-in; eliminates HttpContext/cookie/session problems  decided by Forge

 Team update (2026-03-08): @using BlazorWebFormsComponents.LoginControls must be in every generated _Imports.razor  decided by Cyclops

 Team update (2026-03-08): Run 12 migration patterns: auth via plain HTML forms with data-enhance=false, dual DbContext, LoginView _userName from cascading auth state  decided by Cyclops


 Team update (2026-03-11): `AddBlazorWebFormsComponents()` now auto-registers HttpContextAccessor, adds options pattern + `UseBlazorWebFormsComponents()` middleware with .aspx URL rewriting.  decided by Cyclops

 Team update (2026-03-11): Mandatory L1L2 migration pipeline  no code fixes between layers. Both layers must run in sequence.  decided by Jeffrey T. Fritz


 Team update (2026-03-11): SelectMethod must be preserved in L1 script and skills  BWFC supports it natively via SelectHandler<ItemType> delegate. All validators exist in BWFC.


 Team update (2026-03-11): NEVER default to SQLite; SelectMethod MUST be preserved as SelectHandler<ItemType> delegates. All skill files hardened, L1 script now scaffolds SQL Server.  decided by Jeffrey T. Fritz, Beast, Cyclops


 Team update (2026-03-11): Database provider guidance reframed  lead with 'detect and match original provider' instead of 'NEVER substitute'. L1 auto-detection connects to L2 verification.  decided by Beast (requested by Jeffrey T. Fritz)


 Team update (2026-03-11): ItemType renames must cover ALL consumers (tests, samples, docs)  not just component source. CI may only surface first few errors.  decided by Cyclops



 Team update (2026-03-11): WebFormsPageBase now has Response.Redirect shim, ViewState dict, GetRouteUrl, and Unit implicit string conversion. L2 skills should note these patterns compile unchanged on @inherits WebFormsPageBase pages.  decided by Cyclops


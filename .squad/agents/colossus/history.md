# Colossus — History

<!-- ⚠ Summarized 2026-02-28 by Scribe — entries before 2026-02-26 archived below as summaries. Full early history in history-archive.md -->

## Core Context

Integration test engineer. Built test coverage from M1 through M19. 130+ integration tests (smoke + interaction) covering all milestone sample pages. Key patterns established: `WaitUntilState.DOMContentLoaded` for async-bound components, `Filter(HasTextString)` for specific element targeting, ISO timestamp filtering for console errors, `PressSequentiallyAsync` + Tab for Blazor Server inputs. LoginControls pages require `@using BlazorWebFormsComponents.LoginControls`. Never use external image URLs. Full early history in `history-archive.md`.

## Key Learnings (Consolidated)

- FormView/ListView bind data in `OnAfterRenderAsync`/`OnAfterRender` — use `DOMContentLoaded` + `WaitForSelectorAsync`.
- Menu interaction tests: skip console error checks (JS interop produces expected errors in headless Playwright).
- Playwright `text=` locator matches innermost element — use `Filter(HasTextString)` on parent container instead.
- For strict-mode violations with duplicate text, target specific element (e.g., `page.Locator("td").Filter(...).First`).
- Use specific selectors like `button:has-text('Edit')` instead of generic selectors to avoid premature wait resolution.
- When sample data models change, interaction test assertions must be updated in lockstep (smoke tests won't catch text mismatches).
- Panel/BackImageUrl has external URLs — smoke test sufficient, no interaction test needed.
- Timer interaction test needs 3-second wait for 2000ms interval tick.
- AJAX controls form a natural test category group.

## Summary: M1–M9 (archived)

Covered milestones 1–9: initial smoke tests, Calendar/Chart/FileUpload/ImageMap integration tests, Sprint 2–3 components, M7 data controls (GridView, TreeView, Menu, DetailsView, FormView — 9 smoke + 9 interaction). M9 audit found 105 routes, 100 covered, 5 gaps identified.

## Summary: Issue #358 — Smoke + Interaction Tests (2026-02-25 to 2026-02-27)

Added 5 smoke test InlineData entries (M9 audit gaps: ListView/CrudOperations, Label, Panel/BackImageUrl, LoginControls/Orientation, DataGrid/Styles). Later added 5 interaction tests: ListView CRUD (2 tests), Label AssociatedControlID, DataGrid Styles, LoginControls Orientation. Panel/BackImageUrl skipped (static). All gaps closed.

## Summary: PR #377 DetailsView Integration Test Fix (2026-02-26)

Fixed 5 stale Customer→Product assertions in InteractiveComponentTests.cs after DetailsView sample pages migrated to Product model. All 7 DetailsView integration tests passing.

## Summary: M17 AJAX Control Integration Tests (2026-02-27)

Added 5 smoke tests (Timer, UpdatePanel, UpdateProgress, ScriptManager, Substitution) as `AjaxControl_Loads_WithoutErrors` Theory group. Added 1 interaction test for Timer auto-increment. Build green.

## Team Updates (Current)

📌 Team update (2026-03-17): #471 & #472 resolved. GUID IDs removed from CheckBox/RadioButton/RadioButtonList; L1 script test suite 100% passing. FileUpload already compliant. May need CheckBoxList audit. — decided by Cyclops

📌 Team update (2026-02-26): WebFormsPage unified wrapper — inherits NamingContainer, adds Theme cascading — decided by Jeffrey T. Fritz, Forge
📌 Team update (2026-02-26): SharedSampleObjects is the single source for sample data parity — decided by Jeffrey T. Fritz
📌 Team update (2026-02-26): M15 HTML fidelity strategy — full audit pipeline re-run assigned to Colossus — decided by Forge
📌 Team update (2026-02-27): Branching workflow directive — feature PRs from personal fork to upstream dev — decided by Jeffrey T. Fritz
📌 Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N' syntax — decided by Jeffrey T. Fritz
📌 Team update (2026-02-27): M17 AJAX controls implemented — decided by Cyclops
📌 Team update (2026-02-27): M17 audit fixes resolved — 5 fidelity issues, 9 new tests, PR #402 — decided by Forge, Cyclops
📌 Team update (2026-02-27): Timer duplicate [Parameter] bug fixed; 47 M17 tests — decided by Rogue
📌 Team update (2026-02-28): Cyclops fixed CheckBox bare input missing id attribute — integration tests targeting CheckBox by id may now work in no-text scenarios. All 5 M9 audit gap pages now have interaction test coverage.

 Team update (2026-03-01): Normalizer pipeline order is fixed  regex rules  style norm  empty style strip  boolean attrs  GUID IDs  attr sort  artifact cleanup  whitespace. Case-insensitive file pairing enabled  decided by Cyclops
 Team update (2026-03-01): D-11 through D-14 formally registered. D-12 boolean attrs and GUID IDs now handled by normalizer  decided by Forge
📌 Team update (2026-03-02): FontInfo.Name/Names now auto-synced bidirectionally. Theme font-family renders correctly. Integration tests targeting font-family should now work — decided by Cyclops, Rogue
📌 Team update (2026-03-02): CascadedTheme (not Theme) is the cascading parameter name on BaseWebFormsComponent — decided by Cyclops

 Team update (2026-03-02): Unified release process implemented  single release.yml triggered by GitHub Release publication coordinates all artifacts (NuGet, Docker, docs, demos). version.json now uses 3-segment SemVer (0.17.0). Existing nuget.yml and deploy-server-side.yml are workflow_dispatch-only escape hatches. PR #408  decided by Forge (audit), Cyclops (implementation)


 Team update (2026-03-02): M22 Copilot-Led Migration Showcase planned  decided by Forge

 Team update (2026-03-02): WingtipToys migration analysis complete  36 work items across 5 phases, FormView RenderOuterTable is only blocking gap  decided by Forge

 Team update (2026-03-02): Project reframed  final product is a migration acceleration system (tool/skill/agent), not just a component library. WingtipToys is proof-of-concept.  decided by Jeffrey T. Fritz

## Learnings

### ModelErrorMessage Integration Tests (added by task request)
- Added 1 smoke test `[InlineData]` for `/ControlSamples/ModelErrorMessage` in the `ValidationControl_Loads_WithoutErrors` Theory group.
- Added 3 interactive tests in `InteractiveComponentTests.cs`:
  - `ModelErrorMessage_Submit_ShowsErrors` — submits empty form, asserts `span.text-danger` appears with error text.
  - `ModelErrorMessage_ValidSubmit_NoErrors` — fills matching valid passwords, asserts no error spans and success message appears.
  - `ModelErrorMessage_ClearButton_RemovesErrors` — triggers errors then clicks Clear, asserts errors are removed from DOM.
- The ModelErrorMessage component renders nothing when no errors exist (conditional `@if`), so error-gone assertions use `CountAsync() == 0` rather than visibility checks.
- For the Clear button test, used `WaitForSelectorAsync` with `State.Hidden` to reliably wait for Blazor re-render after clearing the EditContext.
- `PressSequentiallyAsync` + `Tab` pattern used for Blazor Server InputText fields, consistent with established team conventions.

 Team update (2026-03-03): ListView CRUD events  ItemCreated now fires per-item, ItemCommand fires for ALL commands before specific handlers  decided by Cyclops

 Team update (2026-03-03): Themes (#369) implementation last  ListView CRUD first, WingtipToys features second, themes last  directed by Jeff Fritz


 Team update (2026-03-03): WingtipToys 7-phase feature schedule established  26 work items, critical path through Data Foundation  Product Browsing  Shopping Cart  Checkout  Polish  decided by Forge


 Team update (2026-03-03): ListView CRUD test conventions established  43 tests, event ordering via List<string>, cancellation assertions, bUnit double-render handling  decided by Rogue


 Team update (2026-03-04): PRs must target upstream FritzAndFriends/BlazorWebFormsComponents, not the fork  decided by Jeffrey T. Fritz

 Team update (2026-03-05): Migration report image paths must use ../../../ (3-level traversal) for repo-root assets  decided by Beast

 Team update (2026-03-06): CRITICAL  Git workflow: feature branches from dev, PRs target dev. NEVER push to or merge into upstream main (production releases only).  directed by Jeff Fritz

 Team update (2026-03-06): CONTROL-COVERAGE.md updated  library ships 153 Razor components (was listed as 58). ContentPlaceHolder reclassified from 'Not Supported' to Infrastructure Controls. Reference updated CONTROL-COVERAGE.md for accurate component inventory.  decided by Forge

� Team update (2026-03-06): LoginView is a native BWFC component  do NOT replace with AuthorizeView in migration guidance. Both migration-standards SKILL.md files (in .ai-team/skills/ and migration-toolkit/skills/) must be kept in sync. WebFormsPageBase patterns corrected in all supporting docs.  decided by Beast

 Team update (2026-03-06): LoginView must be preserved as BWFC component, not converted to AuthorizeView  decided by Jeff (directive)


 Team update (2026-03-08): Default to SSR (Static Server Rendering) with per-component InteractiveServer opt-in; eliminates HttpContext/cookie/session problems  decided by Forge

 Team update (2026-03-08): @using BlazorWebFormsComponents.LoginControls must be in every generated _Imports.razor  decided by Cyclops


 Team update (2026-03-11): `AddBlazorWebFormsComponents()` now auto-registers HttpContextAccessor, adds options pattern + `UseBlazorWebFormsComponents()` middleware with .aspx URL rewriting. Integration test Program.cs patterns updated  no longer need manual `AddHttpContextAccessor()`.  decided by Cyclops


 Team update (2026-03-11): SelectMethod must be preserved in L1 script and skills  BWFC supports it natively via SelectHandler<ItemType> delegate. All validators exist in BWFC.


 Team update (2026-03-11): ItemType renames must cover ALL consumers (tests, samples, docs)  not just component source. CI may only surface first few errors.  decided by Cyclops

### UpdatePanel Integration Test Coverage (2026-03-13)

**Summary:** Added 3 Playwright interaction tests for UpdatePanel ContentTemplate enhancement. Tests cover Block mode, ContentTemplate syntax, and Inline mode with proper assertion patterns for Blazor state updates.

**Tests added:**
1. `UpdatePanel_BlockMode_RendersAsDivAndInteractsCorrectly` — Block mode (default), button click
2. `UpdatePanel_ContentTemplate_RendersAndInteractsCorrectly` — Web Forms syntax, alert styling, interaction
3. `UpdatePanel_InlineMode_RendersAndRefreshesCorrectly` — Inline mode (span), time display, Refresh button

**Patterns:** `WaitUntilState.NetworkIdle` for page navigation (AJAX control standard), `Filter(HasTextString)` for element targeting (strict-mode safety), 500ms/1000ms waits for state updates, ISO timestamp console filtering, regex time validation.

**Coverage:** 1 smoke test (existing) + 3 interaction tests = 4 total UpdatePanel tests, all passing.

📌 Team update (2026-03-13): UpdatePanel integration tests complete — 3 interaction tests covering all rendering modes and interactive behaviors. All 4 UpdatePanel tests passing (1 smoke + 3 interaction). Follows established AJAX control test conventions.

### Students GridView LEFT JOIN Fix + Test Timing Verification (2026-03-14)

**Summary:** Verified Playwright test timing fixes already in place. `StudentsPageTests.cs` contains all required improvements: BlurAsync on last field, 1000ms post-click wait, 3-second retry loop.

**Verification:** No new changes needed. Test infrastructure already stable and meets requirements.

📌 Team update (2026-03-14): Students LEFT JOIN fix completed by Cyclops — replaced SelectMany (INNER JOIN) with Students.Include(Enrollments) loop. Students without enrollments appear with Count=0, Date=DateTime.Today. Colossus verified Playwright test timing fixes already in place from previous session. All tests passing. Commit d3dc610f.

## Session: 2026-03-22 — Analyzer Expansion BWFC020-023

**Task:** Expand BWFC Analyzers with 4 new custom control migration pattern detectors.

**Created:**
- BWFC020 (ViewStatePropertyPattern): Detects `get { return (T)ViewState["key"]; } set { ViewState["key"] = value; }` properties. Info severity. Code fix converts to `[Parameter] public T Name { get; set; }`.
- BWFC021 (FindControlUsage): Detects `FindControl("id")` calls. Warning severity. Code fix replaces with `FindControlRecursive("id")`.
- BWFC022 (PageClientScriptUsage): Detects `Page.ClientScript.*` usage. Warning severity. No code fix.
- BWFC023 (IPostBackEventHandlerUsage): Detects classes implementing `IPostBackEventHandler`. Warning severity. No code fix.

**Files created:** 6 analyzer source files, 4 test files. Updated `AnalyzerReleases.Unshipped.md` and `AllAnalyzersIntegrationTests.cs`.

**Verification:** All 139 tests pass (was 130 before). Build clean.

## Learnings

- Text-based (`SourceText.Replace`) code fixes are fragile for property replacement — FullSpan includes trivia that complicates newline/indentation. Prefer the syntax tree approach: use `property.WithAccessorList()` + `AddAttributeLists()` without `NormalizeWhitespace()`. Only use `NormalizeWhitespace()` when you can also fully control leading/trailing trivia on all lines.
- New "Migration" category introduced for BWFC020-023. Updated `AllAnalyzers_HaveValidCategory` integration test to accept both "Usage" and "Migration" categories.


**Summary:** 40 tests total — 11 passed, 29 failed, 0 skipped (33.5s duration)

**Breakdown by test class:**

| Class | Total | Passed | Failed | Root Cause |
|---|---|---|---|---|
| NavigationTests | 11 | 2 | 9 | .aspx URLs → 404; `/` → 404; DB pages → 500 |
| HomePageTests | 4 | 0 | 4 | `/Home.aspx` → 404 (missing middleware) |
| AboutPageTests | 5 | 0 | 5 | `/About.aspx` → 404 (missing middleware) |
| StudentsPageTests | 9 | 4 | 5 | `/Students.aspx` → 404; DB → 500 |
| CoursesPageTests | 6 | 4 | 2 | `/Courses.aspx` → 404; DB → 500 |
| InstructorsPageTests | 5 | 1 | 4 | `/Instructors.aspx` → 404; DB → 500 |

**⚠ 4 of 11 "passed" tests are vacuously true** — guard clauses (`if element.Count > 0`) skip assertions when page is 404.

**Three root causes identified:**

1. **Missing `app.UseBlazorWebFormsComponents()` middleware** (affects 20+ tests): Program.cs calls `AddBlazorWebFormsComponents()` for DI but never calls `UseBlazorWebFormsComponents()` in the middleware pipeline. Without this, `.aspx` URL rewriting is absent — all test navigations to `/Home.aspx`, `/About.aspx`, etc. return 404. This is a **Phase 2/3 (L2 transform) gap**.

2. **SQL Server LocalDB unavailable** (affects 3 `AllPages_ReturnHttp200` tests + cascading): `/Students`, `/Courses`, `/Instructors` return HTTP 500 because the `ContosoUniversity` database doesn't exist or the connection string targets `(localdb)\mssqllocaldb`. This is an **infrastructure/seed-data gap** — not a migration defect.

3. **No root route `/`** (affects 6 `NavLink_NavigatesToCorrectPage` tests): Tests start at `BaseUrl + "/"` which returns 404 — no `@page "/"` route exists. Navigation link tests can't find any elements.

**Infrastructure notes:**
- `--no-launch-profile` required to honor `ASPNETCORE_URLS`; launchSettings.json overrides to ports 5000/5001
- Playwright Chromium installed successfully; browser automation works
- App starts in ~2s, Production mode by default with `--no-launch-profile`

### UpdatePanel Integration Tests (2026-03-13)

**Summary:** Added 3 interaction tests for the UpdatePanel component after ContentTemplate RenderFragment parameter was added.

**Tests added to `InteractiveComponentTests.cs`:**
1. `UpdatePanel_BlockMode_RendersAsDivAndInteractsCorrectly` — Verifies Block mode (default) renders as a `<div>`, content displays correctly, and button clicks update the counter.
2. `UpdatePanel_ContentTemplate_RendersAndInteractsCorrectly` — Verifies Web Forms `<ContentTemplate>` syntax works, alert div renders with correct styling, and button interaction updates the counter.
3. `UpdatePanel_InlineMode_RendersAndRefreshesCorrectly` — Verifies Inline mode renders as a `<span>` wrapper, time display is visible, and Refresh button triggers Blazor re-render with updated time value.

**Patterns followed:**
- Used `WaitUntilState.NetworkIdle` for page navigation (consistent with AJAX control patterns)
- Used `Filter(new() { HasTextString = "..." })` pattern for specific element targeting to avoid strict-mode violations
- Used `500ms` wait after button clicks for Blazor state updates (conservative for CI stability)
- Used `1000ms` wait for time refresh test to ensure seconds change
- Console error filtering: ISO timestamp pattern + "Failed to load resource" (standard pattern)
- Regex pattern `@"\d{1,2}:\d{2}:\d{2} (AM|PM)"` to verify time format without asserting exact equality (time changes between reads)

**Coverage:**
- Smoke test already existed: `[InlineData("/ControlSamples/UpdatePanel")]` in `AjaxControl_Loads_WithoutErrors` Theory group (line 234, ControlSampleTests.cs)
- 3 new interaction tests verify all three rendering modes (Block, ContentTemplate, Inline) and their interactive behaviors
- All 8 UpdatePanel tests passing (5 smoke + 3 interaction)


📌 Team update (2026-03-16): Playwright infrastructure confirmed shipping. Unblocks HTML Fidelity dimension for Component Health Dashboard v1. — Forge

### Validator + New Page Integration Tests (2026-03-17)

**Summary:** Added 14 new integration tests — 11 interaction tests and 3 smoke tests.

**Smoke tests added to `ControlSampleTests.cs`:**
- `[InlineData("/ControlSamples/Content")]`, `[InlineData("/ControlSamples/ContentPlaceHolder")]`, `[InlineData("/ControlSamples/View")]` in `UtilityFeature_Loads_WithoutErrors` Theory group.

**Interaction tests added to `InteractiveComponentTests.cs`:**
1. `CompareValidator_InvalidValue_ShowsError` — submits "5" (not > 10), asserts error text appears
2. `CompareValidator_ValidValue_SubmitsSuccessfully` — submits "15", asserts no error
3. `RangeValidator_OutOfRange_ShowsError` — submits "1800" (below 1900–2100), asserts error
4. `RangeValidator_InRange_SubmitsSuccessfully` — submits "2000", asserts no error
5. `RegularExpressionValidator_NonMatching_ShowsError` — submits "abc" (not 5-digit), asserts error
6. `RegularExpressionValidator_Matching_SubmitsSuccessfully` — submits "12345", asserts no error
7. `CustomValidator_InvalidValue_ShowsError` — submits "Banana" (doesn't start with A), asserts error
8. `CustomValidator_ValidValue_SubmitsSuccessfully` — submits "Apple", asserts no error
9. `ValidationSummary_InvalidSubmit_ShowsSummaryWithMultipleErrors` — submits empty, asserts summary header + error messages
10. `Content_Renders_MasterPageDemoElements` — verifies heading and content rendered
11. `ContentPlaceHolder_Renders_DemoContent` — verifies heading and content rendered
12. `View_ClickThrough_ChangesVisibleContent` — verifies initial view, clicks button, checks content persists

**Patterns:** Used `data-audit-control` locators for all validators, `PressSequentiallyAsync` + `Tab` for input fields, `TextContentAsync()` on container + `Assert.Contains`/`DoesNotContain` for error text validation, `#region` blocks per component. Content/ContentPlaceHolder/View tests written defensively since pages are being created in parallel by Jubilee.

**Files modified:**
- `samples/AfterBlazorServerSide.Tests/ControlSampleTests.cs` — 3 new `[InlineData]` entries
- `samples/AfterBlazorServerSide.Tests/InteractiveComponentTests.cs` — 11 new `[Fact]` methods

### Theming Sections 7 & 8 Integration Tests (2026-03-22)

**Summary:** Added 2 Playwright interaction tests for upcoming Theming page enhancements (Sections 7 & 8 being built by Jubilee).

**Tests added to `InteractiveComponentTests.cs`:**
1. `Theming_ThemeMode_StyleSheetThemeVsTheme` — Navigates to /ControlSamples/Theming, verifies Section 7 (ThemeMode) has an h3 heading matching "ThemeMode" or "Theme Mode", confirms both StyleSheetTheme and Theme panels are rendered with text content assertions, and checks at least 2 buttons exist across both panels.
2. `Theming_SubStyles_GridViewHeaderAndFooter` — Navigates to /ControlSamples/Theming, verifies Section 8 (sub-styles/data controls) has an h3 heading, confirms a `<table>` (GridView) is present in that section, and asserts the table has `<th>` header cells.

**Patterns:**
- Used `HasTextRegex` with case-insensitive regex for heading matching — resilient to "ThemeMode" vs "Theme Mode" naming.
- Used `.Filter(new() { Has = heading })` to scope assertions to the correct `.demo-container` section.
- Used `?? string.Empty` on `TextContentAsync()` to eliminate CS8602 null reference warning.
- Tests are written defensively to work once Jubilee adds Sections 7 & 8 — they will fail with clear messages until those sections land.

**Coverage:** 1 smoke test (existing) + 1 existing interaction test + 2 new interaction tests = 4 total Theming tests.


## Session: L1 Integration Test Cases for Phase 1 Enhancements (2026-03-29 00:07)

Added 3 new test cases to migration-toolkit/tests/ for L1 script Phase 1 enhancements:
- **TC16-IsPostBackGuard**: Tests IsPostBack guard unwrapping (GAP-06)  verifies the script removes if (!IsPostBack) { } wrappers and adds explanatory comment
- **TC17-BindExpression**: Tests Bind()  @bind-Value transform (GAP-13)  verifies <%# Bind("Prop") %> becomes @bind-Value="context.Prop"
- **TC18-UrlCleanup**: Tests .aspx URL cleanup (GAP-20)  verifies Response.Redirect() arguments have .aspx extensions removed and tilde converted to slash

All 3 tests passing. Test suite now has 18 test cases (was 15). Pass rate 78% (14/18), line accuracy 98.2%.

**Key learnings:**
- L1 test discovery: Run-L1Tests.ps1 discovers test cases by scanning inputs/ for *.aspx files, expects corresponding .razor in expected/ directory
- Test input format: TC##-Name.aspx + optional TC##-Name.aspx.cs for code-behind
- Expected output format: TC##-Name.razor + optional TC##-Name.razor.cs for code-behind expectations
- Test runner uses normalized line-by-line comparison (trims trailing whitespace, normalizes line endings, removes trailing blank lines)
- Important: Expected files must match ACTUAL script output including:
  - Extra indentation/whitespace preserved from AST transformations (e.g., IsPostBack unwrapping leaves original indentation)
  - ItemType="object" attributes added by script to FormView/DropDownList
  - Standard TODO comment header in .razor.cs files
  - Removal of : System.Web.UI.Page base class from partial classes
- URL cleanup transform only applies to Response.Redirect() call arguments, not to arbitrary string literals containing URLs
- To verify exact output: copy input files to temp directory, run bwfc-migrate.ps1 on directory (not individual file), examine generated .razor/.razor.cs files

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

<!-- ⚠ Summarized 2026-03-12 by Scribe — StaticAssetTests and L2 Automation Shim Tests archived -->

- StaticAssetTests Acceptance Tests (2026-03-06)
- L2 Automation Shim Tests (OPP-2,3,5,6) (2026-03-11)

### Summary (2026-03-06 through 2026-03-11)

StaticAssetTests: 12 Playwright acceptance tests for static asset/visual integrity (catches Run 9 class failures). Tests cover CSS loading, image 404s, Bootstrap navbar classes, navbar height, styled main content, full-page screenshots, product list/detail screenshots, and catch-all static asset 4xx/5xx checks. Key patterns: Response event interception, naturalWidth JS eval for broken images, BoundingBoxAsync for CSS dimensions, screenshot byte-size as visual sanity proxy. L2 Automation Shim Tests: 43 new tests for OPP-2,3,5,6 (all pass). OPP-2 Unit implicit string (17 tests): px/%/em/bare int, empty/null/invalid, roundtrip, all UnitTypes via Theory. OPP-3 ResponseShim (9 tests): ~/ and .aspx stripping, null URL NullReferenceException bug found. OPP-5 ViewState (10 tests): set/get, overwrite, ContainsKey, [Obsolete]. OPP-6 GetRouteUrl (7 tests): .aspx stripping, LinkGenerator passthrough via CapturingLinkGenerator.




 Team update (2026-03-12): L2 automation consolidated  EnumParameter<T> (OPP-1) + WebFormsPageBase shims (OPP-2,3,5,6) all implemented. Rogue: 4 test files need .Value.ShouldBe() fix. Beast: L2 scripts can emit bare enum strings.  decided by Forge (analysis), Cyclops (implementation)

### UpdatePanel ContentTemplate Tests (2026-03-12)

**12 TDD bUnit tests for UpdatePanel ContentTemplate enhancement** (ContentTemplateTests.razor). Tests verify ContentTemplate RenderFragment parameter behavior (to be implemented) plus backward compatibility with existing ChildContent parameter.

**Test categories:**
1. **ContentTemplate rendering** (4 tests) — ContentTemplate renders correctly, takes priority over ChildContent, works with Block/Inline RenderMode
2. **Backward compatibility** (2 tests) — ChildContent (explicit and implicit) still works — PASS NOW
3. **Edge cases** (2 tests) — Empty ContentTemplate, no template provided — don't crash
4. **Nested components** (1 test) — Blazor components inside ContentTemplate render correctly
5. **Integration** (3 tests) — ContentTemplate with CssClass, Visible=false, multiple elements

**Current baseline (before ContentTemplate parameter exists):** 10 of 12 tests PASS. The 2 failures are expected:
- `ContentTemplate_WithNestedComponents_RendersCorrectly` — fails because `<ContentTemplate>` without a RenderFragment parameter is treated as HTML, not a component container, so nested Blazor components don't render
- `ContentTemplate_WithCssClass_RendersOnWrapper` — markup assertion issue (Shouldly.ShouldContain failure)

**Key test patterns followed:**
- Razor test files (`.razor`) inheriting `BlazorWebFormsTestContext` — standard pattern for this project
- `cut.Find()` for element selection, `ShouldNotBeNull()`, `ShouldContain()` for assertions
- CSS class selectors for template identification (pattern from ListView/FormView tests)
- Section headers with `// ===` separators for organization
- RenderMode enum tests using variable: `UpdatePanelRenderMode mode = UpdatePanelRenderMode.Inline;`

**Implementation requirement:** Once ContentTemplate parameter is added to UpdatePanel.razor.cs:
```csharp
[Parameter] public RenderFragment ContentTemplate { get; set; }
```
And UpdatePanel.razor is updated to render:
```razor
@(ContentTemplate ?? ChildContent)
```
Then all 12 tests should pass.

Test file: `src/BlazorWebFormsComponents.Test/UpdatePanel/ContentTemplateTests.razor` (12 tests, 280 lines)

### Base Class Property Tests — Issues #15, #16, #17, #18 (2026-07-25)

**40 new bUnit tests across 5 files** verifying base class changes Cyclops is implementing in parallel. Total suite: 1587 tests (1577 pass, 10 expected failures).

**Issue #15 — AccessKey (20 tests in BaseWebFormsComponent/AccessKeyTests.razor):**
- Button, Label, TextBox, Image, Panel, HyperLink, CheckBox with AccessKey → should render `accesskey="X"`
- Empty/null AccessKey → should NOT render attribute
- Label with AssociatedControlID + AccessKey renders on `<label>` element
- **5 pass now** (Button, Label — already render accesskey); **5 expected failures** (TextBox, Image, Panel, HyperLink, CheckBox — awaiting Cyclops's render changes)

**Issue #16 — ToolTip special characters (5 tests added to BaseWebFormsComponent/ToolTipTests.razor):**
- Ampersand, quotes, angle brackets, apostrophes in ToolTip render correctly via title attribute
- Tests on Button, Label, Image, Panel, TextBox
- **All 5 pass** — Blazor/AngleSharp handles HTML encoding correctly

**Issue #17 — DataBoundComponent styling (10 tests added to DataBoundComponent/StyleInheritanceTests.razor):**
- GridView: BackColor, ForeColor, Width, Height, MultipleStyles on `<table>` outer element
- DataList: ForeColor, Font, MultipleStyles on `<table>`, FlowLayout BackColor on `<span>`
- **4 DataList tests pass** (DataList already merges BaseStyledComponent styles); **5 GridView tests fail** (GridView hard-codes `style="border-collapse:collapse;"` without merging — awaiting Issue #17 fix)

**Issue #18 — Image and Label full styling (5+5 tests added to Image/ImageStyleTests.razor and Label/LabelStyleTests.razor):**
- Image: ForeColor, Border (all 3 props together), BorderWidthOnly negative test, AllStyleProperties
- Label: Height, Width, Border (all 3 props together), BorderWidthOnly negative test, AllStyleProperties
- **All 10 pass** — Image and Label already inherit BaseStyledComponent and render `style="@Style"`

**Key discovery:** Border CSS is ALL-OR-NOTHING. The style builder requires `BorderWidth > 0 AND BorderStyle != None/NotSet AND BorderColor != default` — all three simultaneously. If any is missing, no border CSS renders. Tests reflect this correctly.

**Test files modified:**
- `src/BlazorWebFormsComponents.Test/BaseWebFormsComponent/AccessKeyTests.razor` (4→20 tests)
- `src/BlazorWebFormsComponents.Test/BaseWebFormsComponent/ToolTipTests.razor` (+5 special char tests)
- `src/BlazorWebFormsComponents.Test/DataBoundComponent/StyleInheritanceTests.razor` (8→18 tests)
- `src/BlazorWebFormsComponents.Test/Image/ImageStyleTests.razor` (7→12 tests)
- `src/BlazorWebFormsComponents.Test/Label/LabelStyleTests.razor` (7→12 tests)
📌 Team update (2026-03-14): M20 Batch 6 orchestration spawn — Forge designing component health dashboard, Cyclops advancing L1 script fixes, Rogue building L1 test harness — decided by Scribe

### L1 Migration Script Test Harness (2026-07-25)

**10 focused test cases + automated test runner** for `bwfc-migrate.ps1` quality measurement (Issue #29).

**Test harness location:** `migration-toolkit/tests/`
- `inputs/` — 10 .aspx files, each testing one L1 transform category
- `expected/` — 10 .razor expected output files
- `Run-L1Tests.ps1` — automated runner with metrics (pass rate, line accuracy, timing, diffs)

**Baseline metrics:** 7/10 pass (70%), 94.3% line accuracy, 114ms avg per file.

**Three bugs found:**
1. **TC06 — Eval expression partial conversion:** `<%#: Eval("Name") %>` → `<%#: context.Name %>` instead of `@context.Name`. Root cause: the eval regex replaces `Eval("Name")` with `context.Name` inside the `<%#:...%>` delimiters, but doesn't remove the delimiters. The later encoded expression regex `<%:\s*(.+?)\s*%>` doesn't match `<%#:` (different prefix). Likely fix: process the eval-with-hashbang pattern separately or adjust regex to capture and strip the full delimiters.
2. **TC09 — Content wrapper indentation loss:** `<asp:Content>` removal regex `\s*\r?\n?` after the closing `>` consumes leading whitespace of the next line. The first content line loses its indent.
3. **TC10 — ItemType double-add on pre-typed components:** After `ItemType="NS.Class"` → `TItem="Class"` conversion, the auto-add step adds `ItemType="object"` because the negative lookahead `(?![^>]*ItemType=)` no longer finds `ItemType=` (it's now `TItem=`). Fix: also check for `TItem=` in the lookahead.

**Additional discovery:** The script has an uninitialized variable bug — `$script:ExtractedTitleFromContent` accessed in `ConvertFrom-PageDirective` before `ConvertFrom-ContentWrappers` sets it. Manifests with `Set-StrictMode -Version Latest` when processing a standalone .aspx without Title= in the Page directive. All test cases work around this by including `Title="Test"`.


# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Active Decisions & Alerts

📌 **Team update (2026-03-17):** HttpHandlerBase handler tests validated — 94 tests passing, 1 test fixed (HttpMethod_DefaultIsGet bad assumption). No implementation bugs found; adapter architecture verified. Commit 040fbad5 (15 files, 3218 insertions) on feature/httphandler-base. — decided by Rogue

📌 **Team update (2026-03-17):** Rogue wrote 11 bUnit tests for GUID ID rendering (#471). New RadioButton/IDRendering.razor (6 tests), enhanced CheckBox/IDRendering.razor (+3 tests). All tests pass; integrated into regression suite. — decided by Rogue

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

### Ajax Toolkit Extender Tests — Issues #450, #451 (2026-07-25)

**33 bUnit tests for ConfirmButtonExtender (14) and FilteredTextBoxExtender (19) — all pass.** Total suite: 1,628 tests (0 failures).

**ConfirmButtonExtender tests (14):** RendersNoVisibleHTML, TargetControlID binding, ConfirmText default ("Are you sure?") and custom, ConfirmOnFormSubmit default (false) and true, Enabled default (true), Enabled=false skips JS (verifies no import invocation), BehaviorID default (null) and custom, DisplayModalPopupID default (empty), JsModulePath verified via import invocation args, ThrowsWhenTargetControlIDEmpty (InvalidOperationException).

**FilteredTextBoxExtender tests (19):** RendersNoVisibleHTML, FilterType default (Custom) and single values (Numbers, LowercaseLetters, UppercaseLetters), FilterType flags combos (Numbers|Lowercase, AllLetters, AllFlags=7), ValidChars/InvalidChars defaults (empty) and custom, FilterMode default (ValidChars) and InvalidChars, FilterInterval default (250) and custom (100), JsModulePath verified via import invocation args, ThrowsWhenTargetControlIDEmpty, Enabled=false skips JS, BehaviorID default (null).

**Key patterns for Ajax Toolkit extender testing:**
- Use `JSInterop.Mode = JSRuntimeMode.Loose` — extender components call `import` + `createBehavior` returning `IJSObjectReference`, and bUnit 2.x blocks `Setup<IJSObjectReference>` on module interops (throws "Use SetupModule instead"). Loose mode is the correct approach for testing parameter binding without JS behavior.
- Verify JS module path via `JSInterop.Invocations["import"][0].Arguments[0]` — captures the actual import path string.
- Verify Enabled=false skips JS via `JSInterop.Invocations["import"].ShouldBeEmpty()`.
- String parameters in Razor test markup need `@variable` syntax (e.g., `ValidChars="@chars"`), not bare variable names — bare names become string literals for `string`-typed parameters.
- Extenders inherit `ComponentBase` (not `BaseWebFormsComponent`), so `BlazorWebFormsTestContext` works fine as base — extra BWFC services are ignored.
- `TargetControlID` empty check throws `InvalidOperationException` from `OnAfterRenderAsync` — bUnit propagates this synchronously from `Render()`.

**Test files:** `src/BlazorWebFormsComponents.Test/ConfirmButtonExtender/ConfirmButtonExtenderTests.razor` (14 tests), `src/BlazorWebFormsComponents.Test/FilteredTextBoxExtender/FilteredTextBoxExtenderTests.razor` (19 tests).

### ModalPopupExtender & CollapsiblePanelExtender Tests (2026-07-25)

**60 bUnit tests for ModalPopupExtender (28) and CollapsiblePanelExtender (32) — all pass.** Total suite: 1,688 tests (0 failures).

**ModalPopupExtender tests (28):** RendersNoVisibleHTML, TargetControlID binding, PopupControlID default (empty) and custom, BackgroundCssClass default (empty) and custom, OkControlID default (empty) and custom, CancelControlID default (empty) and custom, OnOkScript default (empty) and custom, OnCancelScript default (empty) and custom, DropShadow default (false) and true, Drag default (false) and true, PopupDragHandleControlID default (empty) and custom, Enabled default (true), Enabled=false skips JS, BehaviorID default (null) and custom, JsModulePath verified via import invocation args, ThrowsWhenTargetControlIDEmpty, AllParameters integration test.

**CollapsiblePanelExtender tests (32):** RendersNoVisibleHTML, TargetControlID binding, CollapseControlID default (empty) and custom, ExpandControlID default (empty) and custom, Collapsed default (false) and true, CollapsedSize default (0) and custom (20), ExpandedSize default (0) and custom (300), CollapsedText default (empty) and custom, ExpandedText default (empty) and custom, TextLabelID default (empty) and custom, ExpandDirection default (Vertical) and Horizontal, AutoCollapse default (false) and true, AutoExpand default (false) and true, ScrollContents default (false) and true, Enabled default (true), Enabled=false skips JS, BehaviorID default (null) and custom, JsModulePath verified, ThrowsWhenTargetControlIDEmpty, AllParameters integration test.

**Same patterns as ConfirmButton/FilteredTextBox extender tests:** JSRuntimeMode.Loose for module interop, string params use @variable syntax, enum params use local variable binding, JS module path verified via `JSInterop.Invocations["import"]`, Enabled=false verifies no import call.

**Test files:** `src/BlazorWebFormsComponents.Test/ModalPopupExtender/ModalPopupExtenderTests.razor` (28 tests), `src/BlazorWebFormsComponents.Test/CollapsiblePanelExtender/CollapsiblePanelExtenderTests.razor` (32 tests).



### Component Health Dashboard — Counting Verification Tests (2026-07-25)

**39 tests created in** `src/BlazorWebFormsComponents.Test/Diagnostics/ComponentHealthCountingTests.cs` verifying PRD §5.4 counting algorithm against real BWFC types. All 39 pass.

**Test categories (matching PRD §10 acceptance criteria):**
- AC-1: Button = 8 props, 2 events (PRD said ~7 — PostBackUrl from ButtonBaseComponent counts; see findings)
- AC-2: GridView = 21 props, 10 events (PRD said ~18; all 21 are legitimate)
- AC-3: Repeater = 0 props, 0 events (all RenderFragment templates correctly excluded)
- AC-4: Generic type name stripping (backtick removal) works for GridView, Repeater, ListView, DataList
- AC-5: 15 base class properties (ID, CssClass, BackColor, etc.) verified absent from Button, Label, TextBox, Panel
- AC-6: EventCallback disjointness — no parameter appears in both prop and event lists across 20 components
- AC-7: RenderFragment exclusion — 12 GridView templates excluded; 5 Repeater templates excluded
- AC-8: [Obsolete] exclusion — Button's PostBackUrl override skipped; base obsolete params never counted
- AC-9: AdditionalAttributes excluded across 20 component spot-check
- AC-10: Intermediate base (ButtonBaseComponent) properties counted for Button — Text, CausesValidation, etc.
- AC-11: CascadingParameter exclusion — Coordinator (protected, so excluded by visibility) and CascadedTheme

**Key findings (documented in .squad/decisions/inbox/rogue-counting-findings.md):**
- PRD §2.7 has off-by-one for Button (table shows 8, text says 7)
- GridView actual count is 21 properties, not ~18
- ButtonBaseComponent.Coordinator is `protected` — excluded by visibility before [CascadingParameter] check fires
- Algorithm uses `BWF =` namespace alias to avoid test project folder name conflicts (Button/, Label/, etc.)

**Patterns:** xUnit [Fact]/[Theory], Shouldly assertions, `typeof(BWF.Button)` aliasing, `BwfAssembly` static field for reflection, `GetParameterDetails()` helper returns categorized lists with skip reasons for diagnostic messages.

### Issue #471 — GUID-based ID Fix Tests (2026-03-16)

**11 bUnit tests verifying developer-set IDs render correctly (no GUIDs) for CheckBox, RadioButton, FileUpload, and RadioButtonList.**

**New file: RadioButton/IDRendering.razor (6 tests):**
- RadioButton_WithID_RendersIDOnInput — developer ID used directly on input element
- RadioButton_WithID_LabelForMatchesInputId — label for=developer ID (accessibility)
- RadioButton_WithoutID_RendersGeneratedID — no crash, generated fallback exists
- RadioButton_WithoutID_LabelStillLinked — label-for matches generated ID
- RadioButton_WithID_NoGuidInRenderedId — regex-verified: no GUID pattern in rendered ID
- RadioButton_WithID_NameAttributeUsesGroupNameWhenSet — name=GroupName, not ID

**Enhanced file: CheckBox/IDRendering.razor (+3 tests, now 5 total):**
- CheckBox_WithID_LabelForMatchesInputId — label for=developer ID
- CheckBox_WithID_NoGuidInRenderedId — regex GUID check
- CheckBox_WithoutID_LabelStillLinked — generated ID label-for association

**Pre-existing tests confirmed solid:**
- FileUpload/IdRendering.razor (2 tests) — exact ID and no-ID behavior
- RadioButtonList/StableIds.razor (8 tests) — _0/_1 suffix pattern, name attribute, label-for, multiple layouts

**All 11 new/enhanced tests PASS.** Written proactively ahead of Cyclops's fix. Tests describe expected Web Forms behavior — may need minor adjustments once implementation lands.

**Key patterns:**
- Regex GUID detection: `[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-...` for anti-GUID assertions
- Label-for accessibility: always verify label.for == input.id
- _Imports.razor provides `@inherits BlazorWebFormsTestContext` — no need for explicit @inherits in test files
- `@using Shouldly` added locally when not using _Imports default assertions

### ASHX/AXD Middleware Tests — Issue #423 (2026-03-17)

**46 integration tests for UseBlazorWebFormsComponents middleware (all pass).** Test file: `src/BlazorWebFormsComponents.Test/Middleware/AspxRewriteMiddlewareTests.cs`. Uses `Microsoft.AspNetCore.TestHost` + `TestServer` for full pipeline integration testing — first middleware test file in the project.

**Test categories (46 total):**
- .aspx rewriting regression (5 tests): 301 redirect, Default.aspx→root, query string preservation, subdirectory Default.aspx, aspx works with ashx/axd disabled
- .ashx handler interception (7 tests): 410 Gone default, descriptive body, query string, mixed case (Theory×3), subdirectory path, disabled passthrough
- .ashx custom redirect mappings (6 tests): 301 redirect, Location header, case-insensitive lookup, unmapped returns 410, multiple mappings, query string preservation on redirect
- .axd resource interception (12 tests): 404 for WebResource/ScriptResource/Trace (Theory×3), ChartImg.axd 410 Gone, query strings, mixed case (Theory×3 for 404, Theory×3 for 410), unknown .axd returns 404, disabled passthrough, ChartImg disabled passthrough
- Edge cases (16 tests): ashx/axd substrings passthrough, .html/.api/root passthrough, all disabled passthrough, file-name-only paths, .ashx.bak/.axd.old non-extension paths, 404 empty body, ChartImg descriptive body, subdirectory ChartImg

**Key patterns:** TestServer creates full ASP.NET Core pipeline via `UseBlazorWebFormsComponents` extension. Terminal `app.Run` returns 200 "PASSTHROUGH" — any request reaching it means middleware didn't intercept. `CreateServerAndClient` helper for custom options with proper disposal. Added `Microsoft.AspNetCore.TestHost 10.0.5` package to test csproj.

**Middleware implementation review:** AshxHandlerMiddleware uses `StringComparer.OrdinalIgnoreCase` dictionary for case-insensitive custom mappings. AxdHandlerMiddleware special-cases ChartImg.axd for 410 Gone, all others get 404. Both use `path.EndsWith` with `OrdinalIgnoreCase` — correct for URL path matching.


### HttpHandler Test Coverage (2026-03-23, Issue #473)

**Handler test suite validated (94 tests, all pass):** Tests for HttpHandlerBase, HttpHandlerContext, HttpHandlerRequest, HttpHandlerResponse, HttpHandlerServer, HandlerEndpointExtensions, RequiresSessionStateAttribute. Cyclops created 7 implementation files in src/BlazorWebFormsComponents/Handlers/. Session interrupted during test creation. All files compile cleanly; tests pass after fixing one incorrect default assumption.

**Test fix:** HttpHandlerRequestTests.HttpMethod_DefaultIsGet was wrong — DefaultHttpContext.Request.Method is empty string by default, not "GET". Changed test to explicitly set Method="POST" and verify it returns. Test name changed to HttpMethod_ReturnsHttpMethod. This is a test issue, not an implementation bug.

**Key patterns:**
- DefaultHttpContext doesn't set default HTTP method — always empty string until explicitly set
- Use Microsoft.AspNetCore.TestHost for integration-style handler tests (pattern from existing Middleware tests)
- Project enforces var over explicit types (IDE0007 as error) — always use var declarations
- Test project builds depend on core library compiling — build test project validates implementation

**File paths:**
- Implementation: src/BlazorWebFormsComponents/Handlers/*.cs (7 files)
- Tests: src/BlazorWebFormsComponents.Test/Handlers/*.cs (5 test files)
- Build: dotnet build src\BlazorWebFormsComponents.Test\BlazorWebFormsComponents.Test.csproj validates all
- Run handler tests: dotnet test --filter "FullyQualifiedName~Handlers"

### AngleSharp Parser Performance Benchmark (2026-03-20)

**9 benchmark tests written and passing** for AngleSharp-based AspxParser. Measured parse time, throughput, and GC allocation across 4 input sizes + 4 AngleSharp-specific scenarios (unclosed tags, & entities, single-quote attrs, script blocks). All using Stopwatch + GC.GetTotalAllocatedBytes, 1000 iterations with 50 warmup.

**Key results:**
- Small (252 chars, 8 lines): 0.044ms avg, 22,614 parses/sec, 51.5 KB alloc
- Medium (1,493 chars, 32 lines): 0.254ms avg, 3,933 parses/sec, 121 KB alloc
- Large (4,218 chars, 83 lines): 0.636ms avg, 1,574 parses/sec, 261 KB alloc
- XL stress (18,277 chars, 268 lines, 100+ controls): 3.35ms avg, 299 parses/sec, 926 KB alloc
- AngleSharp-specific scenarios (unclosed tags, entities, quotes, scripts): 0.14–0.21ms, no overhead vs clean inputs

**Conclusion:** Performance is well within acceptable bounds for on-demand .aspx rendering. Even the XL stress test parses in ~3.3ms. Forge's 10-20% slower estimate cannot be directly verified (XDocument code replaced), but absolute perf is a non-issue. Updated evaluation doc with full results table.

**File paths:**
- Benchmark tests: src/BlazorWebFormsComponents.AspxMiddleware.Test/AspxParserBenchmarkTests.cs
- Evaluation doc: dev-docs/aspx-middleware-anglesharp-evaluation.md
- Run benchmarks: `dotnet test src/BlazorWebFormsComponents.AspxMiddleware.Test/ --filter "FullyQualifiedName~Benchmark" -v n`

📌 Team update (2026-03-20): AngleSharp benchmark completed — performance is acceptable (3.3ms for 18KB stress test). No performance-related blockers to merging — decided by Rogue


### BWFC001 Roslyn Analyzer Test Suite (2026-03-20)

**33 tests written for MissingParameterAttributeAnalyzer (BWFC001) + MissingParameterAttributeCodeFixProvider.** P0 blocker from Forge's review resolved.

Positive tests (4): WebControl subclass, CompositeControl subclass, multiple missing properties, indirect descendant.

Negative tests (26 via Theory+Facts): [Parameter] already present, 18 inherited base properties, non-WebControl class, private/protected/internal, static, empty class, wrong-namespace WebControl.

Code fix tests (3): adds [Parameter], adds using directive when missing, no duplicate using.

Bug found and fixed: Analyzer fired on public static properties. Added StaticKeyword skip. Static props cannot be set via Blazor markup.

Infrastructure: Added test project to solution. Created AnalyzerReleases files (RS2008 fix). Pinned CodeAnalysis 4.11.0 in test csproj.

Key paths: src/BlazorWebFormsComponents.Analyzers.Test/MissingParameterAttributeAnalyzerTests.cs, MissingParameterAttributeCodeFixTests.cs

Pattern: Roslyn analyzer tests use CSharpAnalyzerTest with stub types for WebControl/CompositeControl/ParameterAttribute. Markup syntax marks expected diagnostic locations.
📌 Team update (2026-03-20): ASPX middleware gap closure completed by Cyclops + gate review APPROVED by Forge. 100/100 tests passing (67 middleware + 33 analyzer). BWFC001 analyzer ready for analyzer test expansion — decided by Forge, Cyclops

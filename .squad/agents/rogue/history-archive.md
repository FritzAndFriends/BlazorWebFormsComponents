<!-- Archived 2026-03-06 by Scribe -->

### Core Context (2026-02-10 through 2026-02-27)

**M1M3 QA:** Triaged PR #333 Calendar. 71 bUnit tests for Sprint 3 (42 DetailsView + 29 PasswordRecovery). DetailsView is generic DataBoundComponent<ItemType>, PasswordRecovery needs NavigationManager mock.

**M4 Chart QA:** 152 bUnit tests (BunitContext with JSInterop.Mode=Loose). ChartConfigBuilder most testable (pure static). GetPaletteColors internal  tested indirectly. ChartSeriesDataBindingHelper documents binding contract.

**M6 P0 QA:** 44 tests for base class changes (AccessKey, ToolTip, ImageStyle, LabelStyle, StyleInheritance, ValidatorDisplay, SetFocusOnError). Fixed DataList duplicate AccessKey bug. WebColor "LightGray""LightGrey" via ColorTranslator.

**M7 QA:** 24 GridView tests (Selection 7, StyleSubComponents 8, DisplayProperties 9). 41 P2 tests (ListView CRUD 12, DataGrid Styles 11 + Events 3, Menu LevelStyles 7, Panel BackImageUrl 3, Login Orientation 5).

**M9 QA:** 24 tests (ToolTip 20 across 9 controls, CommaSplit 4). Validation messages stored as `Text,ErrorMessage\x1F ValidationGroup`. ToolTip renders as title on outermost element.

**M16 QA:** 18 LoginView/PasswordRecovery OuterStyle tests. 12 ClientIDMode tests (Static 3, Predictable 3, AutoID 2, Inherit 2, Edge Cases 2). Found UseCtl00Prefix regression  fixed via NamingContainer auto-AutoID.

**Post-Bug-Fix Capture Pipeline (2026-02-26):** Re-ran full HTML capture after 14 bug fixes. 132131 divergences, 01 exact match (Literal-3). 11 targeted controls show improvements. Primary blocker: sample data parity, not component bugs.

### Key Test Patterns

- **Validator Display:** EditForm + InputText + RequiredFieldValidator. Static  visibility:hidden, Dynamic  display:none, None  always display:none. SetFocusOnError uses JSInterop.SetupVoid/VerifyInvoke.
- **BaseListControl:** GetItems() applies DataTextFormatString to both static and data-bound items. AppendDataBoundItems=false replaces static items.
- **Menu:** FindAll("li a") to exclude skip-link. JSInterop.Mode = Loose. `Orientation ori = Orientation.Horizontal;` variable pattern avoids Razor collision.
- **Login controls:** AuthenticationStateProvider + NavigationManager mocks. LoginView uses fully-qualified type to avoid ambiguity.
- **GridView styles:** Named RenderFragments (`<RowStyleContent>` etc.) with GridViewRowStyle. AlternatingRowStyle always initialized. AutoGenerateColumns=false renders in two passes.
- **DataGrid styles:** WebColor values must be variables, not string literals.
- **ListView:** HandleCommand via cut.InvokeAsync() for Blazor Dispatcher context.
- **PasswordRecovery:** outer table always includes border-collapse:collapse  use ShouldContain.
- **ClientIDMode:** Button as test component. `@using BlazorWebFormsComponents.Enums` required.
- **Pipeline:** normalize-html.mjs --compare uses RAW files. Normalize both sides first. Sample data alignment is prerequisite for accurate comparison.

 Team update (2026-02-27): Branching workflow directive  feature PRs from personal fork to upstream dev, only devmain on upstream  decided by Jeffrey T. Fritz

 Team update (2026-02-27): Issues must be closed via PR references using 'Closes #N' syntax, no manual closures  decided by Jeffrey T. Fritz


 Team update (2026-02-27): M17 AJAX controls implemented  ScriptManager/Proxy are no-op stubs, Timer shadows Enabled, UpdatePanel uses ChildContent, UpdateProgress renders hidden, Substitution uses Func callback, new AJAX/Migration Helper categories  decided by Cyclops


 Team update (2026-02-27): M17 sample pages created for Timer, UpdatePanel, UpdateProgress, ScriptManager, Substitution. Default.razor filenames. ComponentCatalog already populated  decided by Jubilee

<!-- Summarized 2026-03-02 by Scribe -- covers M17 + Issue #379 -->

### M17 and Issue #379 Test Summary (2026-02-27 through 2026-02-28)

**M17 AJAX tests (47 tests):** 6 test files -- Timer (9), ScriptManager (9), ScriptManagerProxy (4), UpdatePanel (10), UpdateProgress (9), Substitution (6). Fixed Timer duplicate [Parameter] Enabled bug (shadowed base class). Timer tests use C# API Render<Timer>(p => p.Add(...)). No-op stubs tested via cut.Markup.Trim().ShouldBeEmpty(). ScriptReference properties tested as plain C# instantiation.

**M17 Audit fix tests (9 tests):** Covered all 5 fixes -- EnablePartialRendering default true, Scripts collection, CssClass rendering, display:block;visibility:hidden, ScriptReference properties. UpdateProgress CssClass uses conditional null pattern. All 29 ScriptManager/UpdateProgress tests pass.

**LinkButton CssClass tests (8 tests, Issue #379):** Dedicated CssClass.razor -- single class, multiple classes, no/empty CssClass omits attribute, CssClass+ID coexist, PostBackUrl branch, disabled aspNetDisabled, disabled+CssClass. Two render branches tested (PostBackUrl null vs non-null). Edge case: GetCssClassOrNull() uses IsNullOrEmpty not IsNullOrWhiteSpace.

Key patterns: Timer parameter inheritance -- use C# API, not Razor templates. No-op stubs -- test defaults + no-throw. UpdateProgress DynamicLayout=true > display:none, false > display:block;visibility:hidden. UpdatePanel Block > div, Inline > span. LinkButton has two render paths both sharing GetCssClassOrNull().

 Team update (2026-02-27): M17 audit fixes resolved -- 5 fidelity issues fixed. 9 new tests, 1367 total. PR #402 -- decided by Forge, Cyclops

 Team update (2026-02-28): Cyclops fixed MenuItemStyle Font- attributes (SetFontsFromAttributes call in OnInitialized) and CheckBox bare input missing id attribute  may warrant additional test coverage. Issue #379 confirmed already fixed in M15.

<!-- Summarized 2026-03-02 by Scribe -- covers M20 theming + FontInfo tests -->


## Archived 2026-03-12 (entries from 2026-03-06 through 2026-03-11)

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

### L2 Automation Shim Tests (OPP-2,3,5,6) (2026-03-11)

**43 new tests for L2 automation shims (all pass):**

**OPP-2 Unit implicit string conversion (17 tests):** `UnitImplicitConversionTests.cs` — pure C# tests. Covers: "125px"/"%"/"em"/bare integer implicit assignment, empty/null → Unit.Empty, invalid → FormatException, roundtrip for px/%, all 6 non-pixel UnitTypes via Theory, implicit matches explicit Parse, implicit equals Pixel factory.

**OPP-3 ResponseShim (9 tests):** `WebFormsPageBase/ResponseShimTests.razor` — bUnit tests using MockNavigationManager registered as NavigationManager singleton. Covers: ~/X.aspx strips both (→ /X), absolute path passthrough, ~/X strips tilde only, X.aspx strips extension only, case-insensitive ASPX, empty string passthrough, null → NullReferenceException (**BUG documented**), endResponse ignored, deep path ~/A/B/C.aspx. TestPage exposes protected `Response` as `ExposedResponse`.

**OPP-5 ViewState (10 tests):** `WebFormsPageBase/ViewStateTests.razor` — bUnit tests. Covers: set/get string, complex object, integer, KeyNotFoundException on missing key, overwrite, initially empty, multiple keys, ContainsKey true/false, [Obsolete] attribute via reflection. Uses `#pragma warning disable CS0618` for Obsolete access.

**OPP-6 GetRouteUrl (7 tests):** `WebFormsPageBase/GetRouteUrlTests.razor` — bUnit tests with custom `CapturingLinkGenerator` (concrete LinkGenerator subclass using reflection to capture RouteName from generic TAddress). Covers: .aspx stripping, passthrough without .aspx, return value from LinkGenerator, null routeName, case-insensitive ASPX, null parameters default, mixed-case Aspx.

**Bug found:** ResponseShim.Redirect throws NullReferenceException on null URL — `url.StartsWith("~/")` has no null guard. Filed in decisions/inbox.

Key patterns: MockNavigationManager.LastUri for ResponseShim navigation assertions. CapturingLinkGenerator with reflection on TAddress for GetRouteUrl route name verification. bUnit singleton service override (`Services.AddSingleton`) before Render to inject test doubles.

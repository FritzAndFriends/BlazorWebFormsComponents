# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

### 2026-05-15T09:55:50-04:00: ComponentRefCodeBehindTransform Test Audit

**Task:** Write/audit tests for `ComponentRefCodeBehindTransform` field generation.

**Outcome:** Test file at `tests/BlazorWebFormsComponents.Cli.Tests/TransformUnit/ComponentRefCodeBehindTransformTests.cs` already contained comprehensive coverage — 10 tests, all passing.

**Tests verified (all green):**
- `OrderIs220` — transform slot is correct
- `InjectsFieldForLabel` / `InjectsFieldForGenericGridView` — basic and generic field emission
- `InjectsMultipleFields` — all entries in ComponentRefs get declarations
- `InsertsAfterClassOpeningBrace` — positional correctness vs methods
- `SkipsWhenNoComponentRefs` — empty dict returns content unchanged
- `SkipsWhenNoClassFound` — no `partial class` match → passthrough
- `SkipsExistingFieldDeclaration` — duplicate-guard using identifier regex
- `FieldsAreSortedAlphabetically` — deterministic output ordering
- `WorksWithGenericListViewType` — generic type strings preserved verbatim

**Key patterns observed:**
- `CreateMetadata()` factory accepts `Dictionary<string, string>?` for ComponentRefs
- Field format expected: `private {Type} {id} = default!;` (null-forgiving initializer)
- Duplicate detection checks for existing access-modifier + identifier pattern
- `CountOccurrences()` helper used for duplicate-guard assertions

**Key file paths:**
- Transform: `src/BlazorWebFormsComponents.Cli/Transforms/CodeBehind/ComponentRefCodeBehindTransform.cs`
- Tests: `tests/BlazorWebFormsComponents.Cli.Tests/TransformUnit/ComponentRefCodeBehindTransformTests.cs`
- Markup counterpart: `tests/BlazorWebFormsComponents.Cli.Tests/TransformUnit/ComponentRefMarkupTransformTests.cs`

**Build:** `dotnet build tests/BlazorWebFormsComponents.Cli.Tests --nologo` → 0 errors, 9 warnings (pre-existing)
**Test run:** 10/10 passed, 0 failed

### 2026-05-07T13:58:11-04:00: Cart session-key QA

- Added QA coverage for a new `CartSessionKeyTransform` that rewrites cart/basket `Session.Id` usage to a stable session-backed `cart-key` helper using BWFC `SessionShim` semantics.
- Verified the transform is scoped to cart-oriented statements and leaves unrelated `Session.Id` usage untouched in the new unit coverage.
- Full `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo` validation was blocked by unrelated workspace changes in `src\BlazorWebFormsComponents.Cli\Pipeline\PageQuarantineDetector.cs`, which currently fail the CLI project build before the new tests can execute.

<!--  Summarized 2026-02-27 by Scribe  covers M1M16 -->

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


### 2026-04-28: Semantic Pattern Infrastructure Sprint - All Agents

**Task:** Complete semantic pattern infrastructure for BlazorWebFormsComponents semantic pattern catalog.

**Bishop:**
- Implemented pattern-query-details and pattern-action-pages infrastructure
- Wired production and test registration for all patterns
- Added isolated and pipeline regression tests

**Cyclops:**
- Implemented pattern-account-pages infrastructure
- Implemented pattern-master-content-contracts with helper logic
- Added focused concrete tests

**Forge:**
- Performed comprehensive reviewer safety pass
- Approved bounded semantics and manual TODO boundaries
- Special review of authentication and master/content section patterns

**Rogue:**
- QA audit identified missing default registration gap
- Recommended helper and integration test coverage
- Re-check confirmed gap was resolved by Bishop

**Coordinator:**
- Executed full test suite: 486 passed, 0 failed
- Verified all tests passing before archival

**Outcome:** All semantic pattern contracts approved and production-ready.

≡ Team update (2026-05-07): Inbox merged, decisions consolidated — Scribe


### 2026-05-15T14:02:20Z: ComponentRef test validation complete

Confirmed 10/10 ComponentRef tests passing. Test coverage is complete — no further work needed. Field declaration pattern is sound. Bishop's implementation ready for merge.

### 2026-05-16T15:45:00-04:00: WingtipToys Run 88 benchmark

**Task:** Execute a fresh WingtipToys benchmark run against CLI fix set `d591d8d2` and compare to Run 87.

**Outcome:** Run 88 stayed green at 26/26 acceptance tests. The new InnerText stub behavior removed the prior `ShoppingCartTitle` compile break, reducing fresh L1 build errors from 4 to 3; the remaining manual fixes were the `actions` → `_shoppingCartActions` rename, the non-page `Server.MapPath` logging gap in `ExceptionUtility`, and the `ShoppingCartActions.GetCart()` self-instantiation pattern.

**Benchmark notes:**
- `bwfc-migrate.ps1` still resolved the nested `samples\WingtipToys\WingtipToys` root automatically.
- L1 migration stayed at 19 seconds; build repair dropped to 63 seconds.
- Startup smoke checks for `/`, `/ProductList`, `/About`, and `/Account/Login` all returned HTTP 200 before test execution.
- Three of the four target fixes (`Entities`/`DataContext`, EDMX T4 exclusion, BLL namespace alignment) were not directly exercised by WingtipToys, so they still need another benchmark for validation.

### 2026-05-20T21:07:06.347-04:00: Wizard bUnit coverage audit

**Task:** Investigate the Wizard component's bUnit test coverage, execute the existing Wizard-filtered test run, and identify missing scenarios.

**Outcome:** `src\BlazorWebFormsComponents.Test\Wizard\Navigation.razor` is currently the only Wizard-specific test file and contains 16 `[Fact]` tests. The requested `dotnet test src\BlazorWebFormsComponents.Test --nologo --filter "Wizard"` run passed 26 tests across `net8.0`, `net9.0`, and `net10.0`; the broader count indicates the filter also matches non-folder tests such as CreateUserWizard coverage, so component-folder counts should be tracked separately from filter output.

**Coverage notes:**
- Existing Wizard coverage focuses on active-step rendering, next/previous navigation, finish and complete behavior, sidebar visibility/navigation, `AllowReturn=false`, `OnNextButtonClick`, `OnFinishButtonClick`, cancel button visibility, `HeaderText`, and `Visible=false`.
- Missing coverage includes explicit verification for all `WizardStepType` values (`Auto`, `Start`, `Step`), programmatic `ActiveStepIndex` updates and `ActiveStepIndexChanged`, `OnActiveStepChanged`, `OnPreviousButtonClick`, `OnCancelButtonClick`, `OnSideBarButtonClick`, cancel/finish destination navigation, template rendering (`HeaderTemplate`, `SideBarTemplate`, navigation templates), and edge cases such as empty, single-step, null-title, and many-step wizards.
- `Wizard.razor.cs` declares `StartNavigationTemplate`, `StepNavigationTemplate`, and `FinishNavigationTemplate`, but `Wizard.razor` never renders them; this is a high-priority QA gap because new tests should first confirm intended behavior and may expose a product bug.

**Key file paths:**
- Tests: `src\BlazorWebFormsComponents.Test\Wizard\Navigation.razor`
- Component: `src\BlazorWebFormsComponents\Wizard.razor` and `src\BlazorWebFormsComponents\Wizard.razor.cs`
- Step child: `src\BlazorWebFormsComponents\WizardStep.razor` and `src\BlazorWebFormsComponents\WizardStep.razor.cs`
- Enum/events: `src\BlazorWebFormsComponents\Enums\WizardStepType.cs`, `src\BlazorWebFormsComponents\WizardNavigationEventArgs.cs`

### 2026-05-20T21:19:29.902-04:00: Wizard callback and edge-case coverage expansion

**Task:** Add the missing Wizard bUnit coverage for callbacks, step-type behavior, and edge cases without modifying `src\BlazorWebFormsComponents.Test\Wizard\Navigation.razor`.

**Outcome:** Added three new Wizard test files — `src\BlazorWebFormsComponents.Test\Wizard\Callbacks.razor`, `src\BlazorWebFormsComponents.Test\Wizard\StepTypes.razor`, and `src\BlazorWebFormsComponents.Test\Wizard\EdgeCases.razor` — covering callback firing, `WizardStepType` rendering behavior, many-step sidebar output, and null/empty title handling. The requested validation command `dotnet test src\BlazorWebFormsComponents.Test --nologo --filter "Wizard"` passed with 123 total tests, 117 succeeded, and 6 skipped across `net8.0`, `net9.0`, and `net10.0`.

**Architecture / behavior notes:**
- `Wizard.razor.cs` raises `OnActiveStepChanged` and `ActiveStepIndexChanged` only from internal navigation handlers (`HandleNextClick`, `HandlePreviousClick`, `HandleFinishClick`, `HandleSideBarNavigation`), not from parent-driven parameter updates.
- `WizardStepType.Auto` resolves by position: first step becomes `Start`, last non-`Complete` step becomes `Finish`, and middle steps become `Step`.
- Sidebar titles fall back to `Step {index + 1}` only when `Title` is `null`; an empty string renders as empty text because the component uses the null-coalescing operator rather than `string.IsNullOrEmpty`.
- Step registration is add-only via `Wizard.AddStep()`; there is no removal path, so dynamic step removal is not currently supported.

**Known gaps captured as skipped tests:**
- Programmatic `ActiveStepIndex` parameter changes do not currently trigger `ActiveStepIndexChanged`.
- Single-step wizards still render start-step navigation instead of suppressing navigation entirely.
- Dynamic step add/remove updates are not supported by the current registration model.

**Key file paths:**
- New tests: `src\BlazorWebFormsComponents.Test\Wizard\Callbacks.razor`, `src\BlazorWebFormsComponents.Test\Wizard\StepTypes.razor`, `src\BlazorWebFormsComponents.Test\Wizard\EdgeCases.razor`
- Existing baseline test: `src\BlazorWebFormsComponents.Test\Wizard\Navigation.razor`
- Component implementation: `src\BlazorWebFormsComponents\Wizard.razor`, `src\BlazorWebFormsComponents\Wizard.razor.cs`, `src\BlazorWebFormsComponents\WizardStep.razor.cs`

### FormShim & WebFormsForm Tests (Issue #533)

**39 new tests — all passing.** Created 2 test files covering FormShim dual-mode support and WebFormsForm component rendering.

**Test files created:**
- `FormShimTests.cs` (27 tests, all pass): Dual-mode coverage for SSR (IFormCollection) and interactive (Dictionary<string, StringValues>) paths. Tests indexer, GetValues, AllKeys, Count, ContainsKey for both modes plus null/empty. SetFormData mutation tests for interactive mode (populate, replace, multi-value preservation).
- `WebFormsForm/WebFormsFormTests.razor` (12 tests, all pass): bUnit rendering tests — form element renders, default method is POST, Method/Action parameters, ChildContent renders inside form, HtmlAttributes (class, id, data-*), multiple attributes, empty form, nested elements.

**Bug found and fixed:**
- `WebFormsForm.razor` was missing `@inherits ComponentBase`, causing it to inherit `BaseWebFormsComponent` via `_Imports.razor`. Both `BaseWebFormsComponent` and `WebFormsForm` had `[Parameter(CaptureUnmatchedValues = true)]`, causing `ThrowForMultipleCaptureUnmatchedValuesParameters` at render time. Fixed by adding explicit `@inherits ComponentBase`.
- `RequestShim.cs` line 79: `new FormShim(null)` was ambiguous between `FormShim(IFormCollection?)` and `FormShim(Dictionary<string, StringValues>)`. Fixed by casting to `(IFormCollection?)null`.

**Key patterns:**
- FormShim tests are pure C# xUnit (no bUnit needed) — use `new FormCollection(dict)` for SSR mock data.
- WebFormsForm tests use `.razor` bUnit pattern inheriting `BlazorWebFormsTestContext`.
- Any `.razor` component in the main project that should NOT inherit `BaseWebFormsComponent` must have explicit `@inherits ComponentBase` to override the project-level `_Imports.razor`.

≡ Team update (2026-05-21T12:26): Wizard unsupported behaviors remain explicit QA gaps — keep skipped tests for parent-driven `ActiveStepIndex` changes, single-step navigation suppression, and dynamic step add/remove. Tests serve as regression markers while gaps document product deferred behaviors for future implementation — decided by Rogue (per Jeffrey T. Fritz request)

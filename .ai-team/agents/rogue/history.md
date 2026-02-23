# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — ASCX CLI tests (Sprint 3 item) are deprioritized — decided by Jeffrey T. Fritz

📌 Triage (2026-02-10): PR #333 (`copilot/create-calendar-component`) is a regression from `dev`. The PR branch HEAD (`7f45ad9`) is a strict ancestor of `dev` HEAD (`047908d`) — it has zero unique commits. Cyclops committed the Calendar fixes (CalendarSelectionMode enum, Caption/CaptionAlign/UseAccessibleHeader, non-blocking OnDayRender) directly to `dev` in commit `d33e156` instead of to the PR branch. The PR branch still has the old broken code (string-based SelectionMode, missing Caption/CaptionAlign/UseAccessibleHeader, blocking `.GetAwaiter().GetResult()`). Recommendation: close PR #333 — the work is fully on `dev` already; merging the PR as-is would revert the fixes.

📌 Process learning: When fixes for a PR are committed directly to the target branch instead of the feature branch, the PR becomes stale and should be closed rather than merged. Always commit fixes to the feature branch to keep the PR diff clean.
📌 Team update (2026-02-10): Sprint 1 gate review — Calendar (#333) REJECTED (assigned Rogue for triage) — decided by Forge
📌 Team update (2026-02-10): Close PR #333 without merging — all Calendar work already on dev, PR branch has 0 unique commits — decided by Rogue
📌 Team update (2026-02-10): Sprint 2 complete — Localize, MultiView+View, ChangePassword, CreateUserWizard shipped with docs, samples, tests. 709 tests passing. 41/53 components done. — decided by Squad
📌 Team update (2026-02-11): Sprint 3 scope: DetailsView + PasswordRecovery. Chart/Substitution/Xml deferred. 48/53 → target 50/53. — decided by Forge
📌 Team update (2026-02-11): Colossus added as dedicated integration test engineer. Rogue retains bUnit unit tests. — decided by Jeffrey T. Fritz

📌 Sprint 3 QA (2026-02-12): Wrote 71 bUnit tests for DetailsView (42 tests) and PasswordRecovery (29 tests). DetailsView tests cover: auto-generated row rendering, header/footer text and templates, command row buttons (Edit/Delete/New), mode switching (ReadOnly→Edit→Insert→Cancel), paging with page navigation, all events (ModeChanging, ModeChanged, ItemDeleting, ItemDeleted, ItemUpdating, ItemUpdated, ItemInserting, ItemInserted, PageIndexChanging, PageIndexChanged), empty data text/template, CssClass, GridLines, Visible=false. PasswordRecovery tests cover: Step 1 rendering (title, instruction, label, input, submit button, ID, help link/icon), Step 2 flow (question title, answer input, username display), Step 3 success text, full 3-step workflow, event firing (OnVerifyingUser, OnVerifyingAnswer, OnSendingMail, OnUserLookupError, OnAnswerLookupError), failure text on cancel, custom text properties, template overrides (UserNameTemplate, SuccessTemplate). All 797 tests pass (71 new + 726 existing). — Rogue

📌 Test pattern: DetailsView is a generic DataBoundComponent<ItemType> — tests must use `ItemType="Widget"` and provide `Items` parameter as a list. PasswordRecovery requires NavigationManager service registration (`Services.AddSingleton<NavigationManager>(new Mock<NavigationManager>().Object)`). Both follow the .razor test file pattern inheriting BlazorWebFormsTestContext via _Imports.razor. — Rogue

📌 Team update (2026-02-12): Sprint 3 gate review — DetailsView and PasswordRecovery APPROVED. 71 new tests verified. 797 total. — decided by Forge

 Team update (2026-02-12): Milestone 4 planned  Chart component with Chart.js via JS interop. 8 work items, design review required before implementation.  decided by Forge + Squad

📌 Milestone 4 QA (WI-4): Wrote 140 bUnit tests for the Chart component in `ChartTests.cs`. Tests cover: component rendering (canvas in div, width/height style, CssClass, Visible=false), SeriesChartType enum (35 values with Web Forms numbering), ChartPalette enum (12 values), Docking enum (4 values), ChartDashStyle enum (6 values), DataPoint class (defaults, properties, numeric XValue), Axis class (defaults, all properties), ChartConfigBuilder (empty/null series, all 8 supported type mappings: Column→bar, Bar→bar+indexAxis:y, Line→line, Pie→pie, Area→line+fill, Doughnut→doughnut, Point→scatter, StackedColumn→bar+stacked, scatter XY data format, title/legend plugins with docking, axis config with title/min/max/interval/logarithmic, data labels from Label and XValue, series name→dataset label, responsive options, palette color assignment for all 12 palettes, borderWidth, Bar indexAxis on dataset, stacked+axis config merging), 27 unsupported chart types throw NotSupportedException, config snapshot class properties. All 140 tests pass. — Rogue

📌 Test pattern: Chart component tests use `BunitContext` directly (not `BlazorWebFormsTestContext`) with `JSInterop.Mode = JSRuntimeMode.Loose` to handle Chart.js interop calls. ChartConfigBuilder is the most testable part — pure static class, no JS/canvas dependency. bUnit 2.x requires `Render<T>` not `RenderComponent<T>`. `GetPaletteColors` is internal, so palette behavior is tested indirectly via BuildConfig dataset colors. — Rogue

📌 Feature Comparison Audit — Validation Controls + Login Controls (2026-02-12): Created 13 audit documents in `planning-docs/` comparing Web Forms API vs Blazor implementation for CompareValidator, CustomValidator, RangeValidator, RegularExpressionValidator, RequiredFieldValidator, ValidationSummary, ChangePassword, CreateUserWizard, Login, LoginName, LoginStatus, LoginView, PasswordRecovery. — Rogue

📌 Validation controls common gaps: (1) `Display` property (None/Static/Dynamic) missing from all validators — affects layout behavior. (2) `SetFocusOnError` missing from all validators. (3) `ControlToValidate` uses `ForwardRef<InputBase<T>>` instead of string ID — different API shape from Web Forms. (4) `InitialValue` missing from RequiredFieldValidator. (5) `ControlToCompare` missing from CompareValidator. (6) `AccessKey` and `ToolTip` missing from all validators. (7) ValidationSummary is missing `HeaderText`, `ShowMessageBox`, `ShowSummary`, `ShowValidationErrors`, and `ValidationGroup` — significant functional gaps. (8) ValidationSummary error message parsing uses fragile comma-split (`x.Split(',')[1]`). — Rogue

📌 Login control template support status: (1) ChangePassword: `ChangePasswordTemplate` and `SuccessTemplate` both supported as RenderFragment. (2) CreateUserWizard: `CreateUserStep`, `CompleteStep`, `SideBarTemplate`, `HeaderTemplate` all supported — but limited to 2 fixed wizard steps (no arbitrary WizardSteps). (3) Login: `LayoutTemplate` NOT supported (commented out in code). (4) LoginView: `AnonymousTemplate`, `LoggedInTemplate`, `RoleGroups` all supported — best template coverage of all login controls. (5) PasswordRecovery: component NOT FOUND in source tree despite Sprint 3 history references — needs investigation. — Rogue

📌 Login controls inherit BaseWebFormsComponent (not BaseStyledComponent): Login, ChangePassword, CreateUserWizard, LoginView all lack outer-level WebControl style properties (BackColor, CssClass, ForeColor, Width, Height, etc.). Only LoginName and LoginStatus inherit BaseStyledComponent and have full style support. Sub-element styles are handled via CascadingParameters for the composite login controls. — Rogue


 Team update (2026-02-23): AccessKey/ToolTip must be added to BaseStyledComponent  decided by Beast, Cyclops
 Team update (2026-02-23): DataBoundComponent style gap  DataBoundStyledComponent<T> recommended  decided by Forge
 Team update (2026-02-23): DetailsView/PasswordRecovery branch (sprint3) must be merged forward  decided by Forge
 Team update (2026-02-23): Validation Display property gap confirmed migration-blocking  decided by Rogue
 Team update (2026-02-23): ValidationSummary comma-split bug confirmed  immediate fix needed  decided by Rogue
📌 ChartSeries Data Binding Tests: Added 12 new bUnit tests for ChartSeries data binding in `ChartTests.cs`. Tests verify: extracting X/Y values from Items using XValueMember/YValueMembers, numeric X values, decimal Y values, manual Points fallback when Items is null, empty Items producing empty chart, missing XValueMember (null XValue), missing YValueMembers (empty YValues), integer-to-double conversion, Items overriding manual Points, invalid property names handled gracefully. Created `ChartSeriesDataBindingHelper` test helper class that implements expected data binding logic — this documents the expected behavior that `ChartSeries.ToConfig()` must implement (Cyclops's fix). Total Chart tests: 152 (140 original + 12 data binding). — Rogue

📌 Test pattern: Since `ChartSeries.ToConfig()` is `internal`, data binding tests use a helper class `ChartSeriesDataBindingHelper` that implements the expected extraction logic. This helper documents the contract: if Items is not null, extract DataPoints using reflection; if Items is null, fall back to manual Points; handle invalid property names by returning null/empty values. Cyclops should use this same logic in `ToConfig()`. — Rogue



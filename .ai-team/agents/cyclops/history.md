# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

- **Enum pattern:** Every Web Forms enum gets a file in `src/BlazorWebFormsComponents/Enums/`. Use the namespace `BlazorWebFormsComponents.Enums`. Enum values should match the original .NET Framework values and include explicit integer assignments. Older enums use `namespace { }` block syntax; newer ones use file-scoped `namespace;` syntax — either is accepted.
- **Calendar component:** Lives at `src/BlazorWebFormsComponents/Calendar.razor` and `Calendar.razor.cs`. Inherits from `BaseStyledComponent`. Event arg classes (`CalendarDayRenderArgs`, `CalendarMonthChangedArgs`) are defined inline in the `.razor.cs` file.
- **TableCaptionAlign enum already exists** at `src/BlazorWebFormsComponents/Enums/TableCaptionAlign.cs` — reusable across any table-based component (Calendar, Table, GridView, etc.).
- **Blazor EventCallback and sync rendering:** Never use `.GetAwaiter().GetResult()` on `EventCallback.InvokeAsync()` during render — it can deadlock. Use fire-and-forget `_ = callback.InvokeAsync(args)` for render-time event hooks like `OnDayRender`.
- **Pre-existing test infrastructure issue:** The test project on `dev` has a broken `AddXUnit` reference in `BlazorWebFormsTestContext.cs` — this is not caused by component changes.
- **FileUpload must use InputFile internally:** Raw `<input type="file">` with `@onchange` receives `ChangeEventArgs` (no file data). Must use Blazor's `InputFile` component which provides `InputFileChangeEventArgs` with `IBrowserFile` objects. The `@using Microsoft.AspNetCore.Components.Forms` directive is needed in the `.razor` file since `_Imports.razor` only imports `Microsoft.AspNetCore.Components.Web`.
- **Path security in file save operations:** `Path.Combine` silently drops earlier arguments if a later argument is rooted (e.g., `Path.Combine("uploads", "/etc/passwd")` returns `/etc/passwd`). Always use `Path.GetFileName()` to sanitize filenames and validate resolved paths with `Path.GetFullPath()` + `StartsWith()` check.
- **PageService event handler catch pattern:** In `Page.razor.cs`, async event handlers that call `InvokeAsync(StateHasChanged)` should catch `ObjectDisposedException` (not generic `Exception`) — the component may be disposed during navigation while an event is still in flight. This is the standard Blazor pattern for disposed-component safety.
- **Test dead code:** Code scanning flags unused variable assignments in test files. Use `_ = expr` discard for side-effect-only calls, and remove `var` assignments where the result is never asserted.
- **ImageMap base class fix:** ImageMap inherits `BaseStyledComponent` (not `BaseWebFormsComponent`), matching the Web Forms `ImageMap → Image → WebControl` hierarchy. This gives it CssClass, Style, Font, BackColor, etc. The `@inherits` directive in `.razor` must match the code-behind.
- **Instance-based IDs for generated HTML IDs:** Never use `static` counters for internal element IDs (like map names) — they leak across test runs and create non-deterministic output. Use `Guid.NewGuid()` as a field initializer instead.
- **ImageAlign rendering:** `.ToString().ToLower()` on `ImageAlign` enum values produces the correct Web Forms output (`absbottom`, `absmiddle`, `texttop`). No custom mapping needed.
- **Enabled propagation pattern:** When `Enabled=false` on a styled component, interactive child elements (like `<area>` in ImageMap) should render as inactive (nohref, no onclick). Check `Enabled` from `BaseWebFormsComponent` — it defaults to `true`.
- **PasswordRecovery component:** Lives at `src/BlazorWebFormsComponents/LoginControls/PasswordRecovery.razor` and `.razor.cs`. Inherits `BaseWebFormsComponent` (matching ChangePassword/CreateUserWizard pattern, not BaseStyledComponent). Uses 3-step int tracking: 0=UserName, 1=Question, 2=Success. Each step has its own `EditForm` wrapping. Created `SuccessTextStyle` sub-component, `MailMessageEventArgs`, and `SendMailErrorEventArgs` event args classes.
- **Login controls inherit BaseWebFormsComponent, not BaseStyledComponent:** Despite the Web Forms hierarchy (CompositeControl → WebControl), all existing login controls (Login, ChangePassword, CreateUserWizard) inherit `BaseWebFormsComponent` and manage styles via CascadingParameters. New login controls should follow this established pattern.
- **SubmitButtonStyle maps to LoginButtonStyle cascading name:** PasswordRecovery uses `SubmitButtonStyle` as its internal property name but cascades via `Name="LoginButtonStyle"` to reuse the existing `LoginButtonStyle` sub-component. This is the correct approach when the Web Forms property name differs from the existing cascading name.
- **EditForm per step for multi-step login controls:** PasswordRecovery wraps each step in its own `EditForm` (unlike ChangePassword which wraps everything in one). This is necessary because each step has different submit handlers and different model fields being validated.
- **DetailsView component:** Lives at `src/BlazorWebFormsComponents/DetailsView.razor` and `DetailsView.razor.cs`. Inherits `DataBoundComponent<ItemType>` (same as GridView/FormView). Renders a single record as `<table>` with one `<tr>` per field. Auto-generates rows via reflection when `AutoGenerateRows=true`. Supports paging across items, mode switching (ReadOnly/Edit/Insert), and command row with Edit/Delete/New/Update/Cancel buttons.
- **DetailsViewMode enum:** Separate from `FormViewMode` — Web Forms has both as distinct enums with identical values (ReadOnly=0, Edit=1, Insert=2). Created at `src/BlazorWebFormsComponents/Enums/DetailsViewMode.cs` using file-scoped namespace.
- **DetailsView event args:** All event arg classes live in `src/BlazorWebFormsComponents/DetailsViewEventArgs.cs`. Includes `DetailsViewCommandEventArgs`, `DetailsViewDeleteEventArgs`, `DetailsViewDeletedEventArgs`, `DetailsViewInsertEventArgs`, `DetailsViewInsertedEventArgs`, `DetailsViewUpdateEventArgs`, `DetailsViewUpdatedEventArgs`, `DetailsViewModeEventArgs`. These parallel FormView's event args but are separate types, matching Web Forms.
- **DetailsView field abstraction:** Uses `DetailsViewField` abstract base class and `DetailsViewAutoField` internal class for auto-generated fields. Field definitions can be added via `Fields` RenderFragment child content. External field components can register via `AddField`/`RemoveField` methods using a `DetailsViewFieldCollection` cascading value.
- **Data control paging pattern:** DetailsView uses `PageIndex` (zero-based) to index into the `Items` collection. Each page shows one record. Pager row renders numeric page links. `PageChangedEventArgs` is reused from the existing shared class.

📌 Team update(2026-02-10): FileUpload needs InputFile integration — @onchange won't populate file data. Ship-blocking bug. — decided by Forge
📌 Team update (2026-02-10): ImageMap base class must be BaseStyledComponent, not BaseWebFormsComponent — decided by Forge
📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Docs and samples must ship in the same sprint as the component — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Sprint 1 gate review — Calendar (#333) REJECTED (assigned Rogue), FileUpload (#335) REJECTED (assigned Jubilee), ImageMap (#337) APPROVED, PageService (#327) APPROVED — decided by Forge
📌 Team update (2026-02-10): Lockout protocol — Cyclops locked out of Calendar and FileUpload revisions — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Close PR #333 without merging — all Calendar work already on dev, fixes committed directly to dev — decided by Rogue
📌 Team update (2026-02-10): Sprint 2 complete — Localize, MultiView+View, ChangePassword, CreateUserWizard shipped with docs, samples, tests. 709 tests passing. 41/53 components done. — decided by Squad
📌 Team update (2026-02-11): Sprint 3 scope: DetailsView + PasswordRecovery. Chart/Substitution/Xml deferred. 48/53 → target 50/53. — decided by Forge
📌 Team update (2026-02-11): Colossus added as dedicated integration test engineer. Rogue retains bUnit unit tests. — decided by Jeffrey T. Fritz
📌 Team update (2026-02-12): Sprint 3 gate review — DetailsView and PasswordRecovery APPROVED. 50/53 components (94%). — decided by Forge

 Team update (2026-02-12): Milestone 4 planned  Chart component with Chart.js via JS interop. 8 work items, design review required before implementation.  decided by Forge + Squad

- **Chart component architecture (WI-1/2/3):** Chart inherits `BaseStyledComponent`. Uses CascadingValue `"ParentChart"` for child registration (ChartSeries, ChartArea, ChartLegend, ChartTitle). JS interop via ES module `chart-interop.js` with lazy loading in `ChartJsInterop.cs`. `ChartConfigBuilder` is a pure static class converting component model → Chart.js JSON config, testable without browser.
- **Chart file paths:**
  - Enums: `Enums/SeriesChartType.cs` (35 values), `Enums/ChartPalette.cs`, `Enums/Docking.cs`, `Enums/ChartDashStyle.cs`
  - POCOs: `Axis.cs`, `DataPoint.cs`
  - JS: `wwwroot/js/chart.min.js` (PLACEHOLDER), `wwwroot/js/chart-interop.js`
  - C# interop: `ChartJsInterop.cs`
  - Config builder: `ChartConfigBuilder.cs` (+ config snapshot classes)
  - Components: `Chart.razor`/`.cs`, `ChartSeries.razor`/`.cs`, `ChartArea.razor`/`.cs`, `ChartLegend.razor`/`.cs`, `ChartTitle.razor`/`.cs`
- **Chart type mapping:** Web Forms `SeriesChartType.Point` maps to Chart.js `"scatter"`. Web Forms has no explicit "Scatter" enum value — `Point=0` is the equivalent. 8 types supported in Phase 1; unsupported throw `NotSupportedException`.
- **JS interop pattern for Chart:** Uses `IJSRuntime` directly (not the shared `BlazorWebFormsJsInterop` service) because Chart.js interop is chart-specific, not page-level. `ChartJsInterop` lazily imports the ES module and exposes `CreateChartAsync`, `UpdateChartAsync`, `DestroyChartAsync`.
- **BaseStyledComponent already has Width/Height as Unit type:** Chart adds `ChartWidth`/`ChartHeight` as string parameters for CSS dimension styling on the wrapper div, avoiding conflict with the base class Unit properties.
- **Instance-based canvas IDs:** Uses `Guid.NewGuid()` (truncated to 8 chars) for canvas element IDs, consistent with the ImageMap pattern that avoids static counters.


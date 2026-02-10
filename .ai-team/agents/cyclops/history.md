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

📌 Team update (2026-02-10): FileUpload needs InputFile integration — @onchange won't populate file data. Ship-blocking bug. — decided by Forge
📌 Team update (2026-02-10): ImageMap base class must be BaseStyledComponent, not BaseWebFormsComponent — decided by Forge
📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Docs and samples must ship in the same sprint as the component — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Sprint 1 gate review — Calendar (#333) REJECTED (assigned Rogue), FileUpload (#335) REJECTED (assigned Jubilee), ImageMap (#337) APPROVED, PageService (#327) APPROVED — decided by Forge
📌 Team update (2026-02-10): Lockout protocol — Cyclops locked out of Calendar and FileUpload revisions — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Close PR #333 without merging — all Calendar work already on dev, fixes committed directly to dev — decided by Rogue
📌 Team update (2026-02-10): Sprint 2 complete — Localize, MultiView+View, ChangePassword, CreateUserWizard shipped with docs, samples, tests. 709 tests passing. 41/53 components done. — decided by Squad

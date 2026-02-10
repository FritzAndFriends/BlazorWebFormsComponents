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

📌 Team update (2026-02-10): FileUpload needs InputFile integration — @onchange won't populate file data. Ship-blocking bug. — decided by Forge
📌 Team update (2026-02-10): ImageMap base class must be BaseStyledComponent, not BaseWebFormsComponent — decided by Forge
📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Docs and samples must ship in the same sprint as the component — decided by Jeffrey T. Fritz

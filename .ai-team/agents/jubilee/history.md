# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### Sprint 1 — Sample Pages for Calendar, FileUpload, ImageMap (2026-02-10)

- **Sample page location:** New-style samples go in `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/{ComponentName}/Index.razor` (the .NET 8+ `Components/Pages` layout). There's also an older `Pages/ControlSamples/` path used by some legacy pages — avoid it for new work.
- **Navigation:** Two places must be updated when adding a sample: `Components/Layout/NavMenu.razor` (TreeView-based nav) and `Components/Pages/ComponentList.razor` (flat list on the home page). Both are alphabetically ordered within their category sections.
- **Sample page pattern:** Each page uses `@page "/ControlSamples/{Name}"`, includes a `<PageTitle>`, uses `<h2>` for the top heading, `<h3>`/`<h4>` for subsections, `<hr />` between sections, inline `<pre><code>` blocks showing the markup, and an `@code {}` block at the bottom.
- **PR branches can be read without checkout:** Used `git --no-pager show {branch}:{path}` to read component source from PR branches while staying on `dev`.
- **Calendar already had a sample on dev** from an earlier Copilot commit — I improved it with PageTitle, better structure, code snippets per section, and additional CSS styling demos (weekend/other-month styles).
- **FileUpload uses `@ref` pattern** for imperative access (checking `HasFile`, `FileName`) — this is the closest analog to the Web Forms code-behind pattern of `FileUpload1.HasFile`.
- **ImageMap uses a `List<HotSpot>` parameter** (not child components) — hot spots are defined in code and passed as a list, which differs from the Web Forms declarative `<asp:RectangleHotSpot>` child syntax.

📌 Team update (2026-02-10): Docs and samples must ship in the same sprint as the component — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Sprint 1 gate review — FileUpload (#335) REJECTED, assigned to Jubilee for path sanitization fix (Cyclops locked out) — decided by Forge

### Security Fix — PostedFileWrapper.SaveAs path sanitization (PR #335)

- **Path traversal vulnerability:** `PostedFileWrapper.SaveAs()` passed the `filename` parameter directly to `FileStream` with zero sanitization. A malicious filename like `../../etc/passwd` could write outside the intended directory. The outer `FileUpload.SaveAs()` already had `Path.GetFileName()` sanitization, but the inner `PostedFileWrapper.SaveAs()` did not — creating a security bypass.
- **Fix applied:** Added the same `Path.GetFileName()` + `Path.GetDirectoryName()` + `Path.Combine()` sanitization pattern from the outer `SaveAs()` to `PostedFileWrapper.SaveAs()`.
- **Lesson:** When a class exposes multiple code paths to the same operation (e.g., `FileUpload.SaveAs()` and `PostedFileWrapper.SaveAs()`), security sanitization must be applied consistently in ALL paths. Wrapper/inner classes are easy to overlook.
- **Assigned because:** Cyclops (original author) was locked out per reviewer rejection protocol after Forge's gate review flagged this issue.

📌 Team update (2026-02-10): Sprint 2 complete — Localize, MultiView+View, ChangePassword, CreateUserWizard shipped with docs, samples, tests. 709 tests passing. 41/53 components done. — decided by Squad
📌 Team update (2026-02-11): Sprint 3 scope: DetailsView + PasswordRecovery. Chart/Substitution/Xml deferred. 48/53 → target 50/53. — decided by Forge
📌 Team update (2026-02-11): Colossus added as dedicated integration test engineer. Rogue retains bUnit unit tests. — decided by Jeffrey T. Fritz

### Sprint 3 — Sample Pages for DetailsView and PasswordRecovery (2026-02-11)

- **DetailsView sample:** Uses `Items` parameter with inline `List<Customer>` data (same pattern as GridView RowSelection sample). Demonstrates auto-generated rows, paging between records with `AllowPaging`, Edit mode switching with `AutoGenerateEditButton`, and empty data text. Uses `Customer` model from `SharedSampleObjects.Models`.
- **PasswordRecovery sample:** Shows the 3-step flow (username → security question → success). Uses `SetQuestion()` on the component reference via the `LoginCancelEventArgs.Sender` cast pattern. Demonstrates custom text properties and help link configuration.
- **PasswordRecovery event pattern:** The `LoginCancelEventArgs` exposes a `Sender` property that must be cast back to `PasswordRecovery` to call `SetQuestion()` — this is the key integration point for the security question step.
- **Nav ordering note:** Data Components section in NavMenu.razor is not strictly alphabetical (DataList before DataGrid). I placed DetailsView after DataGrid and before FormView to maintain the closest alphabetical order without rearranging existing entries.

📌 Team update (2026-02-12): Sprint 3 gate review — DetailsView and PasswordRecovery APPROVED. 50/53 components (94%). — decided by Forge

 Team update (2026-02-12): Milestone 4 planned  Chart component with Chart.js via JS interop. 8 work items, design review required before implementation.  decided by Forge + Squad

### Milestone 4 — Chart Sample Pages (WI-6)

- **Chart component API:** Uses child components (`ChartSeries`, `ChartArea`, `ChartTitle`, `ChartLegend`) inside a `<Chart>` parent via `CascadingValue`. Data is provided through `List<DataPoint>` on `ChartSeries.Points`, where each `DataPoint` has `XValue` (object) and `YValues` (double[]). Chart type is set via `SeriesChartType` enum on `ChartSeries.ChartType`.
- **8 sample pages created:** Index (Column), Line, Bar, Pie, Area, Doughnut, Scatter, StackedColumn — each under `Components/Pages/ControlSamples/Chart/`.
- **Multi-series demos:** Line (NY vs LA temps), StackedColumn (3 product lines) show how to add multiple `ChartSeries` children.
- **Scatter uses `Point` type:** The `SeriesChartType` enum has `Point` (not `Scatter`), so the scatter sample uses `SeriesChartType.Point`.
- **Axis config is a POCO:** `Axis` is a plain class (not a component), passed via parameter syntax `AxisX="@(new Axis { Title = "..." })"`.
- **NavMenu Chart node:** Added under Data Components with `Expanded="false"` and 8 sub-nodes for each chart type, alphabetically ordered.
- **ComponentList updated:** Replaced placeholder `Chart(?)` with a working link.


 Team update (2026-02-23): DetailsView/PasswordRecovery branch (sprint3) must be merged forward  decided by Forge
 Team update (2026-02-23): AccessKey/ToolTip must be added to BaseStyledComponent  decided by Beast, Cyclops
📌 Team update (2026-02-12): LoginControls sample pages MUST include `@using BlazorWebFormsComponents.LoginControls` — root _Imports.razor doesn't cover sub-namespaces. Never use external image URLs in samples; use local SVGs. — decided by Colossus

### Utility Feature Sample Pages — DataBinder and ViewState

- **DataBinder sample** (`Components/Pages/ControlSamples/DataBinder/Index.razor`): Demonstrates all three `Eval()` signatures with a Repeater — `DataBinder.Eval(container, "Prop")`, shorthand `Eval("Prop")` via `@using static`, and `Eval("Prop", "{0:C}")` with format strings. Each section has live demo + code block. Section 4 ("Moving On") shows the modern `@context.Property` approach side by side.
- **ViewState sample** (`Components/Pages/ControlSamples/ViewState/Index.razor`): Uses `@ref` to a Panel component to demo `ViewState.Add`/`ViewState["key"]` dictionary API. Shows a click counter and a multi-key settings form stored in ViewState, then contrasts with the modern C# field/property approach. `#pragma warning disable CS0618` suppresses the Obsolete warnings for the demo code.
- **Navigation fixes applied:** NavMenu.razor Login Components reordered (Login before LoginName), DataBinder and ViewState added to Utility Features (alphabetical: DataBinder, ID Rendering, PageService, ViewState). ComponentList.razor fixed: HyperLink moved before Image in Editor Controls, ImageMap removed from Editor Controls and added to Navigation Controls (per team decision), Utility Features column added. mkdocs.yml: ImageMap removed from Editor Controls nav (already in Navigation Controls).
- **Widget model reused:** DataBinder sample reuses `SharedSampleObjects.Models.Widget` with inline data (Laptop Stand, USB-C Hub, Mechanical Keyboard) for a product catalog demo.
- **Build verified:** `dotnet build` passes with 0 compilation errors (Debug config). Release config has a known transient Nerdbank.GitVersioning file-copy issue unrelated to this work.
### Chart Feature-Rich Sample Pages (2026-02-12)

- **4 new sample pages added:** DataBinding, MultiSeries, Styling, ChartAreas — each demonstrating advanced Chart features.
- **DataBinding.razor:** Shows Web Forms-style data binding with `Items`, `XValueMember`, and `YValueMembers` parameters. Uses business object records (`SalesData`, `TrafficData`) instead of manual `DataPoint` creation. Includes Web Forms vs Blazor comparison code snippets.
- **MultiSeries.razor:** Demonstrates multiple series on one chart for comparisons — revenue channels (Online vs In-Store), regional sales (3 regions), and server performance metrics (CPU vs Memory). Shows the pattern of adding multiple `<ChartSeries>` children to one `<Chart>`.
- **Styling.razor:** Showcases all 11 `ChartPalette` options with visual comparisons (BrightPastel, Berry, Chocolate, EarthTones, Excel, Fire, Grayscale, Light, Pastel, SeaGreen, SemiTransparent). Demonstrates custom colors via `WebColor` static fields (e.g., `WebColor.DodgerBlue`).
- **ChartAreas.razor:** Explains the `Axis` configuration options (Title, Minimum, Maximum, Interval, IsLogarithmic). Shows logarithmic scale for exponential data and constrained Y-axis for focused ranges.
- **Nav ordering pattern:** New samples added alphabetically within Chart node: Area, Bar, ChartAreas, Column, DataBinding, Doughnut, Line, MultiSeries, Pie, Scatter, StackedColumn, Styling.
- **WebColor usage:** Use static fields like `WebColor.DodgerBlue` not `WebColor.FromName("...")` which doesn't exist.



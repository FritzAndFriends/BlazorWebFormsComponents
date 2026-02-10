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

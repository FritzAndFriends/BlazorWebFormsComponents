# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

- **Doc structure pattern:** Each component doc follows a consistent structure: title → intro paragraph with MS docs link → Features Supported → Features NOT Supported → Web Forms Declarative Syntax → Blazor Razor Syntax (with examples) → HTML Output → Migration Notes (Before/After) → Examples → See Also. Admonitions (`!!! note`, `!!! warning`, `!!! tip`) are used for gotchas and important notes.
- **mkdocs.yml nav is alphabetical:** Components are listed alphabetically within their category sections (Editor Controls, Data Controls, Validation Controls, Navigation Controls, Login Controls, Utility Features).
- **Calendar doc already existed:** The Calendar component doc was already present at `docs/EditorControls/Calendar.md` and in the mkdocs nav — likely created alongside the component PR. No changes needed.
- **PageService doc existed on PR branch but not on dev:** The basepage services branch (`copilot/create-basepage-for-services`) already had a comprehensive `docs/UtilityFeatures/PageService.md`. I created a fresh version on dev that matches the project doc conventions.
- **ImageMap is in Navigation Controls, not Editor Controls:** Despite being image-related, ImageMap is categorized under Navigation Controls in the mkdocs nav, alongside HyperLink, Menu, SiteMapPath, and TreeView.
- **Style migration pattern:** Web Forms used `TableItemStyle` child elements (e.g., `<TitleStyle BackColor="Navy" />`). The Blazor components use CSS class name string parameters (e.g., `TitleStyleCss="my-class"`). This is a key migration note for Calendar, and should be documented for any future components with similar style patterns.
- **Branch naming varies:** PR branches on upstream use `copilot/create-*` naming (not `copilot/fix-*` as referenced in some task descriptions). Always verify branch names via `git ls-remote` or GitHub API.

📌 Team update (2026-02-10): Docs and samples must ship in the same sprint as the component — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Sprint 1 gate review — ImageMap (#337) APPROVED, PageService (#327) APPROVED, ready to merge — decided by Forge
📌 Team update (2026-02-10): Sprint 2 complete — Localize, MultiView+View, ChangePassword, CreateUserWizard shipped with docs, samples, tests. 709 tests passing. 41/53 components done. — decided by Squad

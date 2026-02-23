# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
<!-- ⚠ Summarized 2026-02-12 by Scribe — original entries covered 2026-02-10 through 2026-02-12 -->

### Summary: Milestone 1 PR Review & Planning (2026-02-10)

Reviewed 6 PRs. Calendar (#333): strong but needed CalendarSelectionMode enum. FileUpload (#335): broken data flow — must use InputFile, not raw @onchange. ImageMap (#337): wrong base class (BaseWebFormsComponent → should be BaseStyledComponent), static ID counter leak. PageService (#327): solid DI/service approach. #328 (ASCX CLI) and #309 (VS Snippets): merge conflicts, shelved indefinitely.

Gate review: Calendar REJECTED (assigned Rogue), FileUpload REJECTED (assigned Jubilee, path sanitization needed), ImageMap APPROVED, PageService APPROVED. Cyclops locked out of Calendar/FileUpload. PR #333 closed — work already on dev.

### Summary: Milestone 2 & 3 Planning (2026-02-10–02-11)

- Milestone 2 shipped: Localize, MultiView+View, ChangePassword, CreateUserWizard. 41/53 (77%).
- Status.md reconciliation: actual count was 48/53 (91%) after accounting for merged work.
- Milestone 3 scope: DetailsView + PasswordRecovery. Chart/Substitution/Xml deferred.
- Colossus added as integration test engineer.

### Summary: Milestone 3 Gate Review (2026-02-11)

DetailsView APPROVED: DataBoundComponent<T> inheritance, 10 events, table HTML, DetailsViewMode enum, auto-field generation. PasswordRecovery APPROVED: BaseWebFormsComponent inheritance, 3-step wizard, 6 events, table HTML. Minor non-blocking issues noted. 50/53 components (94%), 797 tests.

**Key patterns confirmed:**
- Login Controls → BaseWebFormsComponent + cascading TableItemStyle/Style
- Data-bound controls → DataBoundComponent<T> with Items parameter
- Login Control events use `On` prefix (project convention)
- Docs + samples must ship with components

### Summary: Chart JS Library Evaluation (2026-02-12)

Evaluated 4 JS libraries for Chart component. D3 rejected (zero built-in charts, XL effort). Chart.js recommended (MIT, ~60KB, 10 types, Blazor wrappers exist). ApexCharts strong alternative (20+ types but 2x bundle). Plotly rejected (3-4MB). Architecture: bundle chart.min.js as static asset + thin JS interop layer. HTML output exception: `<canvas>` instead of `<img>` (justified). Inherit DataBoundComponent<T>. Effort: L. Risks: first JS interop in project, canvas not bUnit-testable, SSR needs special handling. Proceed as Milestone 4.

📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Docs and samples must ship in the same milestone as the component — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Milestone 1 gate review — Calendar (#333) REJECTED, FileUpload (#335) REJECTED, ImageMap (#337) APPROVED, PageService (#327) APPROVED — decided by Forge
📌 Team update (2026-02-10): Lockout protocol — Cyclops locked out of Calendar and FileUpload revisions — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Close PR #333 without merging — all Calendar work already on dev — decided by Rogue
📌 Team update (2026-02-10): Milestone 2 complete — 4 components shipped. 709 tests. 41/53 done. — decided by Squad
📌 Team update (2026-02-10): Milestone 3 plan ratified — DetailsView + PasswordRecovery. Chart/Substitution/Xml deferred. — decided by Forge
📌 Team update (2026-02-11): Colossus added as dedicated integration test engineer. — decided by Jeffrey T. Fritz
📌 Team update (2026-02-11): Milestone 3 gate review — DetailsView APPROVED, PasswordRecovery APPROVED. 50/53 (94%). — decided by Forge
📌 Team update (2026-02-12): Chart component feasibility confirmed — Chart.js recommended via JS interop. Effort: L. Target Milestone 4. — decided by Forge
📌 Team update (2026-02-12): Milestone 4 planned — Chart component with Chart.js via JS interop. 8 work items, design review required before implementation. — decided by Forge + Squad

### Themes and Skins Migration Strategy (2026-02-12)

- Evaluated 5 approaches for migrating Web Forms Themes/Skins to Blazor: CSS Custom Properties, CascadingValue ThemeProvider, Generated CSS Isolation, DI Service, and Hybrid.
- **Recommended CascadingValue ThemeProvider** — only approach that faithfully models both `Theme` (override) and `StyleSheetTheme` (default) semantics, supports SkinID selection, and can set any property (not just CSS-expressible ones).
- CSS-only approaches (1, 3, 5) cannot set non-CSS properties like `Width` (as HTML attribute), `ToolTip`, or `Visible` — which are valid skin properties in Web Forms.
- DI-based approach (4) works functionally but cannot scope themes to a page or subtree, unlike `CascadingValue` which mirrors Web Forms' per-page `@Page Theme=` directive.
- **Known bug:** `BaseWebFormsComponent.SkinID` is typed as `bool` instead of `string`. Must be fixed before any theme implementation.
- The library already uses CascadingParameters extensively (TableItemStyle, LoginControl styles) — ThemeProvider follows the same pattern.
- Implementation is opt-in and non-breaking: no `ThemeProvider` wrapper = no behavior change.
- Strategy is exploratory per Jeff's request — the README exclusion of themes/skins still stands until a decision to implement.

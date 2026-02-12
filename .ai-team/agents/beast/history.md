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
- **Deferred controls doc pattern:** For controls permanently excluded from the library, use `docs/Migration/DeferredControls.md` with per-control sections: What It Did → Why It's Not Implemented → Recommended Alternatives → Migration Example (Before/After). Include a summary table at the end. This is distinct from the component doc pattern — deferred controls don't have Features Supported/Not Supported sections since they have zero Blazor implementation.
- **Migration section nav is semi-alphabetical:** The Migration section in mkdocs.yml keeps "Getting started" and "Migration Strategies" at the top, then remaining entries in alphabetical order.

📌 Team update (2026-02-10): Docs and samples must ship in the same sprint as the component — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Sprint 1 gate review — ImageMap (#337) APPROVED, PageService (#327) APPROVED, ready to merge — decided by Forge
📌 Team update (2026-02-10): Sprint 2 complete — Localize, MultiView+View, ChangePassword, CreateUserWizard shipped with docs, samples, tests. 709 tests passing. 41/53 components done. — decided by Squad
📌 Team update (2026-02-11): Sprint 3 scope: DetailsView + PasswordRecovery. Chart/Substitution/Xml deferred. 48/53 → target 50/53. — decided by Forge
📌 Team update (2026-02-11): Colossus added as dedicated integration test engineer. Rogue retains bUnit unit tests. — decided by Jeffrey T. Fritz
- **PasswordRecovery doc pattern follows ChangePassword:** The PasswordRecovery doc mirrors the ChangePassword.md structure — same "Authentication Integration" warning admonition, same style migration guidance (TableItemStyle → CSS classes via cascading parameters), same emphasis on event-driven architecture. This three-step wizard pattern (UserName → Question → Success) with `@ref` for calling component methods (SetQuestion, SkipToSuccess) is unique among login controls and should be noted for any future wizard-style components.
- **DetailsView doc covers generic component:** DetailsView is generic (`DetailsView<ItemType>`), unlike most other data controls. The doc explicitly calls out the `ItemType` requirement and the reflection-based auto-field generation. The Fields child content pattern with CascadingValue registration is worth noting for any future components that use child component registration.
- **Sprint 3 docs delivered:** DetailsView and PasswordRecovery documentation created with full structure (features, Web Forms syntax, Blazor syntax, HTML output, migration notes, examples, See Also). Added to mkdocs.yml nav (alphabetical) and linked in README.md.

📌 Team update (2026-02-12): Sprint 3 gate review — DetailsView and PasswordRecovery APPROVED. Action item: fix DetailsView docs to replace `DataSource=` with `Items=` in Blazor code samples. — decided by Forge

 Team update (2026-02-12): Milestone 4 planned  Chart component with Chart.js via JS interop. 8 work items, design review required before implementation.  decided by Forge + Squad

- **Chart doc is first JS interop component:** The Chart component is unique in the library — it's the first to use JavaScript interop (Chart.js via ES module import). The doc template needed a new "HTML Output Exception" admonition pattern to explain why `<canvas>` replaces `<img>`. This pattern should be reused for any future components that deviate from identical HTML output.
- **DeferredControls.md updated for partial implementation:** Chart moved from fully-deferred to partially-implemented. The DeferredControls page now has a dual role: documenting controls not implemented at all (Substitution, Xml) AND documenting unsupported sub-features of implemented controls (27 unsupported chart types). This "partially implemented" pattern may apply to future controls.
- **Child component docs pattern:** Chart introduces a multi-component documentation pattern (Chart, ChartSeries, ChartArea, ChartLegend, ChartTitle) with separate parameter tables for each. This nested-component doc approach should be used for any future components with required child components.


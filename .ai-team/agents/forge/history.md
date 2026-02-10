# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

### 2026-02-10 — PR Review & Sprint Planning Session

**PR #333 (Calendar):**
- Strongest of the 6 PRs. Table-based rendering matches Web Forms output. 19 tests. SelectionMode uses string instead of enum — Web Forms uses `CalendarSelectionMode` enum (None/Day/DayWeek/DayWeekMonth). Missing `CalendarSelectionMode` enum in Enums/. Style properties use CSS class strings (`TitleStyleCss`) instead of Web Forms `TableItemStyle` objects — acceptable pragmatic trade-off for Blazor. Missing: `UseAccessibleHeader` property, `Caption`/`CaptionAlign` properties. The `.GetAwaiter().GetResult()` call in `CreateDayRenderArgs` is a blocking anti-pattern but necessary for synchronous rendering. Overall quality is high.

**PR #335 (FileUpload):**
- Inherits `BaseStyledComponent` ✓. Uses `<input type="file">` — correct HTML output. `OnFileChangeInternal` uses raw `ChangeEventArgs` instead of Blazor `InputFile`/`IBrowserFile` pattern — the `@onchange` binding won't actually populate `_currentFile`. This is a broken data flow: files will never be loaded. Has security comments from GitHub Advanced Security about `_currentFiles` readonly and `Path.Combine` traversal risk. `Accept` and `AllowMultiple` attributes correct. Missing: `HasFiles` (plural) property from Web Forms.

**PR #337 (ImageMap):**
- Correctly renders `<img>` + `<map>` + `<area>` HTML structure matching Web Forms. HotSpot hierarchy (HotSpot → RectangleHotSpot/CircleHotSpot/PolygonHotSpot) matches Web Forms class hierarchy exactly. Implements `IImageComponent` interface. Uses `BaseWebFormsComponent` not `BaseStyledComponent` — this is wrong; Web Forms `ImageMap` inherits from `Image` which inherits `WebControl` which has style properties. Static `_mapIdCounter` with `Interlocked.Increment` is thread-safe but will leak across test runs. Missing: `Enabled` property handling for areas.

**PR #327 (PageService):**
- Novel approach — not a direct Web Forms control, but emulates `Page.Title`, `Page.MetaDescription`, `Page.MetaKeywords`. Uses DI service pattern (IPageService) — idiomatic Blazor. Renders `<PageTitle>` and `<HeadContent>` — correct for Blazor 6+. Generic catch clauses flagged by code scanning. Useless variable assignments in tests flagged. Solid architectural approach for the migration use case.

**PR #328 (ASCX CLI Tool):**
- Merge conflicts — NOT mergeable. Draft status. Converts `<%@ Control %>`, `<asp:*>`, `<%: %>`, `<%= %>`, `<%# %>`, `<% %>` blocks. Has `AiAssistant` stub class. No tests visible in the tool project itself. This is a companion tool, not a component — different review criteria. Needs conflict resolution and test coverage before merge.

**PR #309 (VS Snippets):**
- Merge conflicts — NOT mergeable. 13 VS 2022 snippets as VSIX. Not a component — tooling review. Snippets for static imports and component patterns. Useful but needs rebase to resolve conflicts.

**Key Patterns Discovered:**
- Copilot-authored PRs consistently use good XML doc comments
- Components generally follow the project's base class hierarchy correctly
- Calendar uses string-based SelectionMode instead of enum — inconsistent with project enum pattern
- FileUpload has a fundamental data flow bug with `@onchange` not populating file data
- ImageMap should inherit BaseStyledComponent, not BaseWebFormsComponent
- Two PRs (#328, #309) have merge conflicts blocking any merge

**Sprint Planning Decisions:**
- Sprint 1 should focus on landing Calendar (with SelectionMode enum fix) and PageService, plus fixing merge conflicts on tooling PRs
- Sprint 2 should tackle remaining Editor Controls (MultiView/View, Localize) and start Login Controls
- Sprint 3 should cover Data Controls gaps (DetailsView) and documentation/sample catch-up

📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Docs and samples must ship in the same sprint as the component — decided by Jeffrey T. Fritz
📌 Team update (2026-02-10): Sprint plan ratified — 3-sprint roadmap established — decided by Forge

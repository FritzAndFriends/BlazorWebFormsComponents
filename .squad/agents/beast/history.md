# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- ⚠ Summarized 2026-02-27 by Scribe — covers M1–M16 -->

<!-- ⚠ Summarized 2026-03-06 by Scribe — older entries archived -->

### Archived Sessions

- Core Context (2026-02-10 through 2026-02-27)
- Doc Work Summary (2026-02-27 through 2026-03-03)
- Key Team Updates (2026-02-27 through 2026-03-03)
- Migration Report Conventions (2026-03-04)
- Run 5 Benchmark Report (2026-03-05)
- Run 6 Benchmark Report (2026-03-04)
- Render Mode Placement Correction (2026-03-05)

<!-- ⚠ Summarized 2026-03-07 by Scribe — entries from 2026-03-05 through 2026-03-07 (pre-Run 11) archived -->

- WebFormsPageBase & Page System Docs (2026-03-05)
- Skills Cross-Reference Review — 7 files, 16+ fixes (2026-03-06)
- Run 8 Report Enhancement — executive pattern established (2026-03-06)
- Run 9 Skill Fixes — 6 RF items across 4 skill files (2026-03-07)
- Run 9 RCA Documentation — path preservation + CSS verification rules (2026-03-07)

### Issue #438: Deprecation Guidance Docs (Latest)

**Delivered:** Comprehensive deprecation guidance page (`docs/Migration/DeprecationGuidance.md`) covering Web Forms patterns with no Blazor equivalent.

**Content:**
- `runat="server"` → Blazor native components
- `ViewState` → Component fields + scoped services  
- `UpdatePanel` → Blazor's incremental component rendering
- `ScriptManager` → `IJSRuntime` + `HttpClient`
- PostBack events → Component lifecycle + event handlers
- Page lifecycle (`Page_Load`, `Page_Init`) → `OnInitializedAsync`, `OnParametersSetAsync`
- `IsPostBack` → Removed (use `OnInitializedAsync`)
- Server-side control properties → Declarative data binding
- Application/Session state → Singleton/scoped services
- Data binding events (`ItemDataBound`) → Component templates with `@context`

**Format:** Before/after tabbed code examples, migration checklist table, lifecycle mapping.

**Branch:** `squad/438-deprecation-docs` — pushed to FritzAndFriends upstream. Commit 5b17682b.

**Files:** 
- Created `docs/Migration/DeprecationGuidance.md` (23.3 KB, ~400 lines)
- Updated `mkdocs.yml` — added to Migration section navigation

**Design decision:** Placed after "Automated Migration Guide" in nav to catch developers early in their migration journey. Each section pairs Web Forms pattern with clear Blazor alternative, supporting the library's goal of enabling code reuse with minimal markup changes.

**Summary (2026-03-05 through 2026-03-07 pre-Run 11)

WebFormsPageBase docs and Page System rewrite shipped (2026-03-05). Skills cross-reference review found `.ai-team/skills/` drifting behind `migration-toolkit/skills/` — both must be updated together. LoginView is a native BWFC component, never replace with AuthorizeView. Executive report pattern established in Run 8: blockquote bottom line → timeline → screenshots → before/after code. Run 9 skill fixes: cookie auth under Interactive Server, minimal API endpoint templates, enhanced navigation guidance, DisableAntiforgery, ListView GroupItemCount. Run 9 RCA: added Static Asset Path Preservation and CSS Reference Verification rules to migration-standards. Key learning: functional tests passing ≠ migration success — visual regression is ship-blocking.

<!-- ⚠ Summarized 2026-03-11 by Scribe — entries from 2026-03-07 through 2026-03-08 archived -->

- Run 10 Failure Report — Coordinator Process Violation (2026-03-07)
- Run 11 Skill Fixes — Static Assets, ListView Placeholders, Action Links (2026-03-07)

### Summary (2026-03-07 through 2026-03-08)

Run 10: ❌ FAILED — Coordinator violated protocol (hand-editing files, wrong SDK, no Development mode). 20/25 tests passed but process discipline failed. Run 11 skill fixes: added Static Asset Migration Checklist (all common folders to wwwroot/), ListView Template Placeholder Conversion (placeholder elements → @context, #1 failure cause), and Action Links preservation. Team updates: Coordinator must not do domain work; SSR default with InteractiveServer opt-in; LoginControls using required in _Imports.razor; auth via plain HTML forms; DbContext factory-only.

### Migration-tests folder reorganization (2026-03-11)

- **Scope:** Reorganized `dev-docs/migration-tests/` from flat directory (100+ files with inconsistent naming) into hierarchical `project/runNN/` structure.
- **WingtipToys:** 16 run folders (run01–run17, run07 skipped) under `wingtiptoys/`. Merged standalone summary `.md` files into run folders as `summary.md` alongside detailed `REPORT.md`. Renamed lowercase `report.md` → `REPORT.md` in early runs (1–6).
- **ContosoUniversity:** 18 run folders (run01–run18) under `contosouniversity/`. Resolved two numbering collisions: `contosouniversity-run11` (Mar 9) vs `contoso-run11` (Mar 10) and same for run12. Decision: March 9 batch keeps runs 07–12; March 10–11 runs renumbered to 13–18.
- **Lost files:** `contoso-run16/` (new run18) was never committed to git. Files were untracked and accidentally deleted during reorganization. Created placeholder REPORT.md with reconstructed summary data.
- **Duplicate screenshots:** 5 root-level `contoso-*.png` files were byte-identical to `contoso-run11-2026-03-10/` screenshots (now run13). Removed duplicates via `git rm`.
- **README.md:** Completely rewritten with all 18 Contoso runs documented (was only 2), updated all links to new paths, added renumbering details table, and updated Report Archive section.
- **Cross-project docs:** `component-coverage.md` and `css-architecture-analysis.md` remain at migration-tests root.

### Executive Summary & Chart Images (2026-03-11)

- Created `dev-docs/migration-tests/EXECUTIVE-SUMMARY.md` — data-driven summary: 35 benchmark runs, 65 acceptance tests, 45%/61% L1 performance improvements.
- Replaced ASCII charts with PNG images via `generate-charts.py` (matplotlib, 800×400px, 150 DPI). Three charts: WT L1 perf, CU L1 perf, combined improvement bar chart. Script at `dev-docs/migration-tests/images/generate-charts.py` — append new data points and re-run.


📌 Team update (2026-03-11): Run 18 improvement recommendations prioritized by Forge — see decisions.md



📌 Team update (2026-03-11): User directives from Jeff  eliminate Test-UnconvertiblePage, standardize on ItemType, P0-2 approved  see decisions.md

<!-- ⚠ Summarized 2026-03-12 by Scribe — Pipeline enforcement, SelectMethod/SQLite fixes, DB provider detection, exec summary archived -->

- Migration Pipeline Enforcement in Skill Docs (2026-03-11)
- SelectMethod Skills Fix + Run 20 Report Corrections (2025-07-24)
- Database Provider Detection Framing (2025-07-24)
- SelectMethod & SQLite Enforcement in Skill Files (2025-07-24)
- Executive Summary Update — Runs 19-21 (2026-03-12)

### Summary (2026-03-11 through 2026-03-12)

Pipeline enforcement: Added mandatory L1→L2 pipeline section to bwfc-migration and migration-standards SKILL.md. All TItem→ItemType across skill files. SelectMethod fixes: 8+3+4 locations updated from "SelectMethod→Items" to "SelectMethod PRESERVED as SelectHandler<ItemType> delegate." DB provider detection: reframed 3 skill files from reactive ("NEVER SQLite") to proactive ("detect and match original provider"). SQLite enforcement: removed "Prefer SQLite for local dev" root cause, added NEVER/MUST admonitions. Key learning: positive/proactive instructions outperform prohibitions for agent behavior. Exec summary: 38→40 runs, added WT Run 20-21 + CU Run 19 data, regenerated charts.


 Team update (2026-03-11): L2 automation shims (OPP-2, 3, 5, 6) implemented by Cyclops on WebFormsPageBase  Unit implicit string, Response.Redirect shim, ViewState, GetRouteUrl. OPP-1/OPP-4 deferred.  decided by Forge (analysis), Cyclops (implementation)



 Team update (2026-03-11): ItemType renames must cover ALL consumers (tests, samples, docs)  not just component source. CI may only surface first few errors.  decided by Cyclops



 Team update (2026-03-11): WebFormsPageBase now has Response.Redirect shim, ViewState dict, GetRouteUrl, and Unit implicit string conversion. L2 skills should note these patterns compile unchanged on @inherits WebFormsPageBase pages.  decided by Cyclops



 Team update (2026-03-12): L2 automation consolidated  EnumParameter<T> (OPP-1) + WebFormsPageBase shims (OPP-2,3,5,6) all implemented. Rogue: 4 test files need .Value.ShouldBe() fix. Beast: L2 scripts can emit bare enum strings.  decided by Forge (analysis), Cyclops (implementation)

 Team update (2026-03-12): Cookie shims use graceful degradation (Pattern B+), not exceptions  Jeff directive. Document no-op behavior for cookies when HttpContext unavailable.  decided by Jeffrey T. Fritz
 Team update (2026-03-12): PageTitle deduplication  L2 skill must add Page.Title and remove inline <PageTitle>. Never invent title values. Consume BWFC-MIGRATE marker from L1.  decided by Forge (analysis), approved by Jeffrey T. Fritz
 Team update (2026-03-12): Render mode guards on WebFormsPageBase. Document IsHttpContextAvailable escape hatch and render mode behavior for Request/Response/Session shims.  decided by Forge

### UpdatePanel ContentTemplate Documentation Update (2026-03-12 → 2026-03-14)

**Update:** Updated CONTROL-REFERENCE.md to document UpdatePanel's new ContentTemplate RenderFragment support. Later review by Forge approved the enhancement as production-ready.

**Decision:** Surgical update to AJAX Controls section in CONTROL-REFERENCE.md. No other sections affected. All related decisions merged to decisions.md.

📌 Team update (2026-03-14): UpdatePanel ContentTemplate enhancement approved and shipped. Forge review: Web Forms fidelity ✅, HTML output ✅, base class change ✅ acceptable, backward compatibility ✅, migration story ✅, render mode decision ✅ correct, tests ✅ adequate, sample page ✅ excellent. All 8 checklist items pass. Production-ready. Merged to decisions.md with consolidated team notes.

### Run 22 Lessons: Migration Standards & Data Migration Doc Enhancements (2026-03-XX)

Captured three critical Run 22 learnings (39/40 tests passing) in migration skill documentation:

**migration-standards/SKILL.md updates:**
1. **Generated Code Variable Declaration (IDE0007):** Added new subsection documenting that ALL local declarations in generated code MUST use `var`, not explicit types. `.editorconfig` enforces this as a build error. Includes CORRECT/WRONG examples. Applies to both L1-generated scaffolding and L2 Copilot-generated code.
2. **TextBox Binding Timing for Playwright Tests:** Added new subsection documenting BWFC TextBox uses `@onchange` (blur), not `@oninput` (keystroke). Playwright `FillAsync()` doesn't commit binding until blur. Recommended pattern: `BlurAsync()` or `PressAsync("Tab")` after filling, then small delay before submit. Root cause of Run 22 Students page add-student test failure.

**bwfc-data-migration/SKILL.md updates:**
3. **Session State Examples:** Enhanced "Session State Under Interactive Server Mode" section with three concrete, copy-pasteable code patterns: **(A) Minimal API endpoints** (ContosoUniversity student add example with HttpClient), **(B) Scoped Service** (CartService with List<CartItem>), **(C) Database-backed** (UserPreferencesService with IDbContextFactory). All examples use `var` (IDE0007 compliant). Root cause: existing section listed options without code, leaving developers guessing.

**Context:** Run 22 required multiple iterations due to explicit type declarations causing build failures, Playwright test timing issues, and developers implementing session patterns without examples. These doc enhancements codify the patterns to reduce future iteration cycles.

**Files:** migration-toolkit/skills/migration-standards/SKILL.md (lines ~165–199), migration-toolkit/skills/bwfc-data-migration/SKILL.md (lines ~29–95)  
**Decision log:** .squad/decisions/inbox/beast-migration-docs.md

### Ajax Control Toolkit Extender Documentation (2026-03-15)

**Delivered:** Complete documentation suite for first Ajax Control Toolkit extender components in BWFC (ConfirmButtonExtender, FilteredTextBoxExtender).

**Content Structure:**
1. **AjaxToolkit/index.md** — Extender pattern overview explaining non-rendering, target-based architecture, render mode requirements (InteractiveServer), graceful degradation in SSR/static modes
2. **ConfirmButtonExtender.md** — 8.8 KB: browser confirmation dialogs on button click/form submit, includes 5 progressively complex usage examples (basic, form submit, multiple buttons, dynamic messages, destructive action pattern), enum documentation, properties table, migration guidance
3. **FilteredTextBoxExtender.md** — 12.2 KB: character filtering with FilterType flags and FilterMode, covers whitelist/blacklist patterns, includes 7+ realistic examples (phone, currency, SKU, filename blacklist), paste handling behavior, performance tuning, character combination reference table, validation notes

**Key Teaching Insights:**
- **Extender concept clarity:** Emphasized that extenders produce NO HTML — only attach JavaScript behavior. This is the key differentiator from BWFC components that render HTML.
- **Migration simplicity messaging:** "You literally just remove the `ajaxToolkit:` prefix — everything else stays the same!" — directly addressing Jeff's directive to show how BWFC enables continued use of familiar APIs
- **Render mode prominence:** Placed InteractiveServer requirement at top of overview and each component doc to prevent hours of debugging. Included graceful degradation table so developers understand SSR/static behavior.
- **Realistic examples:** Moved beyond abstract to concrete: phone numbers with format, currency with decimals, filenames with blacklist, demonstrating that FilterType flags combine with C# `|` operator
- **Paste behavior clarity:** Documented how FilteredTextBoxExtender strips invalid characters on paste (not a blocking error), with example showing "abc123def" → "123" in Numbers mode

**Files:**
- Created `docs/AjaxToolkit/index.md` (4.8 KB)
- Created `docs/AjaxToolkit/ConfirmButtonExtender.md` (8.8 KB)
- Created `docs/AjaxToolkit/FilteredTextBoxExtender.md` (12.2 KB)
- Updated `mkdocs.yml` — added "Ajax Control Toolkit Extenders" section to navigation

**Branch:** `squad/451-450-confirm-filtered-extenders` — pushed to origin  
**Commit:** 39bf8876 with co-authored-by trailer

**Design Decisions:**
- Placed overview page first in nav to catch developers early on the "extenders don't render HTML" concept
- Character filtering examples use flags table for quick reference (phone, email, currency patterns)
- Troubleshooting sections focus on render mode (most common issue) before TargetControlID mismatches
- All code examples use Blazor razor syntax with `@rendermode InteractiveServer` to model correct usage pattern

**Learnings for future extender docs:**
- Developers unfamiliar with Ajax Control Toolkit will need the "What are extenders?" section; don't skip it
- FilterType enum documentation should include a reference table of common patterns (phone, email, etc.) to reduce copy-paste
- Render mode is a higher-priority troubleshooting item than TargetControlID — place it first

### Ajax Control Toolkit Extender Documentation, Phase 2 (2026-03-15)

**Delivered:** Complete documentation suite for ModalPopupExtender and CollapsiblePanelExtender components.

**Content Structure:**

1. **ModalPopupExtender.md** — 15.6 KB: Modal dialog patterns with overlay backdrop, OK/Cancel actions, drag support, focus trapping, Escape key dismissal. Includes 4 progressively complex examples (basic confirmation, settings dialog with drag/drop shadow, JS callbacks, form dialog). Complete properties table, render mode requirements, graceful degradation notes.

2. **CollapsiblePanelExtender.md** — 20.4 KB: Collapse/expand panels with CSS transitions, separate collapse/expand triggers, auto-collapse/expand on hover, vertical/horizontal animations, scrollable content. Includes 6+ realistic examples (simple toggle, separate buttons, auto-hover, horizontal sidebar, FAQ accordion, scrollable logs, partial visibility). ExpandDirection enum documented with usage patterns.

**Key Teaching Insights:**
- **Modal concept clarity:** Emphasized that modals block interaction with page behind the overlay and require focus management. Different from toast/snackbar notifications.
- **Escape key behavior:** Documented that Escape key executes `OnCancelScript`, matching standard browser modal behavior expectation.
- **Drag handle patterns:** Included header drag pattern example showing visual affordance (gray header with "Settings" text) to teach best practices.
- **FAQ accordion pattern:** Provided complete functional example showing how to generate multiple collapsible panels from a list — a very common real-world use case.
- **Partial visibility with CollapsedSize:** Documented the distinction between `CollapsedSize="0"` (fully hidden) vs. `CollapsedSize="50"` (shows header/preview), with example showing preview pane.
- **Horizontal vs. Vertical:** CollapsiblePanelExtender sidebar example demonstrates ExpandDirection usage for horizontal collapse (width instead of height).

**Files:**
- Created `docs/AjaxToolkit/ModalPopupExtender.md` (15.6 KB)
- Created `docs/AjaxToolkit/CollapsiblePanelExtender.md` (20.4 KB)
- Updated `docs/AjaxToolkit/index.md` — added overview descriptions for both components
- Updated `mkdocs.yml` — added nav entries for both components under Ajax Control Toolkit section

**Branch:** `squad/446-447-modal-collapsible-extenders` — pushed to origin  
**Commit:** f6fafcdd with co-authored-by trailer

**Design Decisions:**
- Placed ModalPopupExtender before CollapsiblePanelExtender in nav (modal is simpler concept to start with)
- Both docs include "Before/After" Web Forms vs. Blazor comparison right after the overview to highlight migration simplicity
- ModalPopupExtender examples progress from simple yes/no dialog → form with fields → JavaScript callbacks (complexity ramp)
- CollapsiblePanelExtender examples progress from toggle button → accordion FAQ → sidebar menu (UI pattern ramp)
- Both docs include "TextLabelID" pattern for updating button text dynamically (e.g., "▶ Show" ↔ "▼ Hide")

**Learnings for future extender docs:**
- Modal docs need to explain overlay behavior and focus trapping — users coming from no-framework experience may not expect this
- CollapsiblePanelExtender ScrollContents property is subtle — pair it with ExpandedSize limit in examples to make the behavior obvious
- FAQ accordion is the most powerful use case for CollapsiblePanelExtender — lead with that to show power
- Drag handle examples should show visual distinction (darker background, cursor change) to teach UX best practices
- Partial visibility pattern (CollapsedSize > 0) is underutilized; showcase it as a way to show "preview" of collapsed content



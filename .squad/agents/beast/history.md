# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10


📌 Team update (2026-03-17): HttpHandlerBase implementation complete (7 files in Handlers/). Returns IEndpointConventionBuilder; Session markers added; build passes 0 errors. — decided by Cyclops
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

### Issue #438: Deprecation Guidance Docs

📌 **Team update (2026-03-17):** #471 & #472 resolved. GUID IDs removed from CheckBox/RadioButton/RadioButtonList; L1 script test suite 100%. 2105 tests passing. — decided by Cyclops

**✅ DELIVERED** — Comprehensive deprecation guidance page covering Web Forms patterns with no Blazor equivalent.

**Session (2026-03-17 by Beast):**
- Created `docs/Migration/DeprecationGuidance.md` — 32 KB, ~600 lines covering 8 deprecation patterns
- Updated `mkdocs.yml` — added "Deprecation Guidance" to Migration navigation section (after "Automated Migration Guide")
- Created decision record: `.squad/decisions/inbox/beast-deprecation-docs.md`

**Content patterns documented:**
- `runat="server"` — Scope marker; remove (Blazor components always server-side)
- `ViewState` — Use component fields + scoped/singleton services instead
- `UpdatePanel` — Blazor incremental rendering makes triggers obsolete; UpdatePanel is now just a CSS-compatible wrapper
- `Page_Load` / `IsPostBack` → `OnInitializedAsync` + event handlers + lifecycle mapping table
- `ScriptManager` — Stub for migration compat; replace with `IJSRuntime` + `HttpClient` + DI
- Server control properties → Reactive data binding (fields, not imperative assignment)
- Application/Session state → Singleton/scoped services
- Data binding events (`ItemDataBound`) → Component templates with `@context`

**Format & Tone:**
- Each pattern: "What It Was" → "Why Deprecated" → "What To Do Instead" + before/after code
- Tabbed markdown for side-by-side comparison of Web Forms vs Blazor
- Lifecycle mapping table (Page_Init → OnInitializedAsync, etc.)
- Empathetic tone — acknowledges these are familiar patterns being left behind
- Audience: Experienced Web Forms developers learning Blazor

**Design decision rationale:**
- Placed after "Automated Migration Guide" in nav — developers run L1 automation first, then encounter these patterns; this doc is their reference
- Each section pairs Web Forms pattern with clear Blazor alternative — supports library goal of enabling code reuse with minimal markup changes
- Comprehensive coverage including derived patterns (e.g., application state → services)

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

### Issue #473 Issue 4: Migrating .ashx Handlers Documentation

**Delivered:** Comprehensive MkDocs page for migrating ASP.NET Web Forms `.ashx` HTTP handlers to Blazor using the new `HttpHandlerBase` feature (Issue #473, Issue 4).

**File created:** `docs/Migration/MigratingAshxHandlers.md` (~33.5 KB, ~800 lines)

**Content structure:**
1. **Overview** — What `.ashx` handlers are, why migration is needed, `HttpHandlerBase` value proposition
2. **Quick Start** — 6-step migration checklist (mechanical changes)
3. **Registration** — Four registration patterns:
   - Explicit path via `MapHandler<T>("/path")` in `Program.cs`
   - Convention-based routing (derive from class name)
   - Multi-path registration with `MapHandler<T>()`
   - Chaining auth/CORS with `.RequireAuthorization()`, `.RequireCors()`
4. **Before/After Examples** — Three fully-worked examples:
   - JSON API handler (GET request, returns JSON with `System.Text.Json`)
   - File download handler (binary response, Content-Disposition, MapPath)
   - Image generation/thumbnail handler (System.Drawing, thumbnail generation)
5. **API Reference** — Complete property/method documentation for:
   - `HttpHandlerContext` (Request, Response, Server, Session, User, Items)
   - `HttpHandlerRequest` (QueryString, Form, Files, HttpMethod, Headers, InputStream, authentication)
   - `HttpHandlerResponse` (ContentType, StatusCode, Write, BinaryWrite, AddHeader, Clear, End [Obsolete])
   - `HttpHandlerServer` (MapPath, HtmlEncode/Decode, UrlEncode/Decode)
6. **Session State** — Using `[RequiresSessionState]` attribute, session configuration, `GetObject<T>()`/`SetObject<T>()` extensions
7. **What's Not Supported** — Clear list with workarounds for:
   - `Response.End()` → use `return`
   - `Server.Transfer()`/`Server.Execute()` → not supported, refactor as service
   - `Application["key"]` → use DI singleton or `IMemoryCache`
   - `context.Cache` → migrate to `IMemoryCache`
   - Complex `Request.Files` scenarios → documented with `IFormFile` equivalent
8. **Interaction with AshxHandlerMiddleware** — How migrated handlers bypass 410 Gone response
9. **Dependency Injection** — Constructor injection support with examples
10. **Testing** — `TestServer`-based unit test example
11. **Common Patterns** — JSON POST handler, authenticated handler with `.RequireAuthorization()`
12. **Troubleshooting** — Common issues and solutions (404, session null, Response.End() warning, DI failures, file uploads, CORS)
13. **Summary** — Quick recap: 6 mechanical changes, familiar API, modern routing, DI support, session state support

**Style & format:**
- Written for Web Forms developers learning Blazor
- Before/after code examples side-by-side or sequential
- Admonitions (!!!note, !!!warning, !!!tip) for important callouts
- Complete, compilable code samples
- Encouraging tone — emphasizes minimal rewrite burden
- Follows existing BWFC doc style (consistent with DeprecationGuidance.md, Strategies.md)

**Navigation updated:** `mkdocs.yml` — added "Migrating .ashx Handlers: Migration/MigratingAshxHandlers.md" after User Controls, before migration readiness checklist

**Design decision:** Placed in Migration section after User Controls and before Readiness checklist, making it discoverable for developers working through migration guides sequentially. Handler migration is a mid-to-late migration concern (after pages/controls are converted) but high-value for API-heavy applications.

**Reference documents:** Built on Forge's `forge-ashx-handler-base-class.md` specification (sections R1-R10, before/after examples in R5, 6-mechanical-changes pattern). Spec defined: explicit path registration, convention-based routing, multi-path chaining, API surface, session state, unsupported patterns, and interaction with existing `AshxHandlerMiddleware`.

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

### README.md: Added Ajax Control Toolkit Components Section (Current)

**Task:** Add promotional section to main README.md highlighting the 14 Ajax Control Toolkit extender/container components available in separate NuGet package.

**Delivered:**
- New top-level `## Ajax Control Toolkit Components` section added after existing "AJAX Controls" subsection
- NuGet badge for `Fritz.BlazorAjaxToolkitComponents` package (color: blue) with stable + prerelease versions
- Clear messaging: "Simply remove the `ajaxToolkit:` prefix and you're ready to go!"
- All 14 components listed with brief descriptions and documentation links
- Link to full documentation at `docs/AjaxToolkit/index.md`
- Section positioned between "Blazor Components for Controls" section end and "We will NOT be converting..." paragraph

**File:** README.md (lines 107–132)  
**Style:** Matches existing component list format and badge pattern  
**Marketing value:** Shows developers that Ajax Control Toolkit migrations are supported and simple  
**Next:** Verify links work in final doc site build



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

### Component Health Dashboard Documentation (Current Session)

**Task:** Create MkDocs documentation page for the Component Health Dashboard per PRD §6.4.

**Delivered:** `docs/dashboard.md` — comprehensive 9 KB documentation page covering:

**Content structure:**
1. **Overview** — Explains what the dashboard measures and why (6 dimensions, 52 tracked components)
2. **How to Access** — Live dashboard at `/dashboard` in sample app; static snapshot in docs
3. **Scoring Model** — 6-dimensional scoring table with rationale:
   - Property Parity (30%) — Most critical: developers need the properties they use
   - Event Parity (15%) — Important but fewer events than properties
   - Has bUnit Tests (20%) — Untested components are unreliable
   - Has Documentation (15%) — Table-stakes for open-source
   - Has Sample Page (10%) — Shows usage but less critical
   - Implementation Status (10%) — Sanity check (Complete/Stub/Deferred)
4. **Reading the Dashboard** — User-focused guidance:
   - Color coding: 🟢 Green (≥90%), 🟡 Yellow (70-89%), 🔴 Red (<70%)
   - Fraction display: "7/8" means 7 of 8 expected properties (why numerator/denominator matters)
   - N/A handling: baseline not yet curated (excluded from weighted average)
   - Binary indicators: ✅/❌ for tests, docs, samples
5. **What Counts (and Doesn't)** — Detailed counting rules from PRD §2:
   - Component-specific properties only (stops at base classes)
   - EventCallback parameters are events, not properties
   - RenderFragments excluded (Blazor infrastructure)
   - Infrastructure parameters excluded (AdditionalAttributes, CascadingParameter, Inject, Obsolete)
6. **Maintaining Baselines** — Operational guidance:
   - When adding a component: add to `dev-docs/reference-baselines.json`
   - When counts seem wrong: verify against MSDN .NET Fx 4.8 API docs
   - Link to PRD for detailed counting rules (§2)
7. **Glossary** — Quick reference for key terms (Expected, Implemented, Parity, Baseline, etc.)
8. **Next Steps** — Action items (run dashboard, improve components, add baselines)

**Key design decisions:**
- **Tone:** Empathetic to developers, focuses on "why" each metric matters
- **Accuracy:** All content derives directly from PRD §§1–7; no interpretation beyond the PRD
- **Accessibility:** Glossary + cross-references to PRD sections for those needing deeper understanding
- **Actionability:** "Maintaining Baselines" section gives developers concrete steps, not abstract guidance
- **Link strategy:** References PRD §2 for detailed counting rules instead of duplicating 40+ lines of rules

**Navigation update:** Added to `mkdocs.yml` as top-level nav entry immediately after "Home," positioning it as a key diagnostic tool alongside component documentation.

**File:** `docs/dashboard.md` (9,010 bytes)  
**Branch:** Ready for commit with co-authored-by trailer

**Style alignment:**
- Matches existing BWFC doc patterns: problem → solution → examples/tables
- Uses Material theme formatting: admonitions, markdown tables, links
- Tone matches Button.md and migration guides: practical, Web Forms-aware, developer-focused

**Learnings:**
- PRD §4.2 weight rationale needed clear translation to "why developers care" (property fidelity = migration success; tests = reliability)
- N/A handling is subtle: developers need to know "missing baseline" ≠ "broken implementation"
- Terminology matters: "component-specific" vs "base class" vs "infrastructure" — glossary prevents confusion
- Reference baselines are the bottleneck: good docs can't overcome missing baseline data (links to MSDN, not invented)
- Partial visibility pattern (CollapsedSize > 0) is underutilized; showcase it as a way to show "preview" of collapsed content

### Ajax Control Toolkit L1 Migration Skill Document (2026-03-XX)

**Delivered:** L1-specific skill document at `.squad/skills/migration-standards/ajax-toolkit-migration.md` covering automated Layer 1 handling of Ajax Control Toolkit controls.

**Content Structure:**
1. **Overview** — ACT as a set of extender and container controls identified by `ajaxToolkit:` prefix
2. **Detection** — Look for Register directive, ToolkitScriptManager, and `<ajaxToolkit:*>` usage
3. **Supported Controls Table** — All 16 known components plus ToolkitScriptManager
4. **Layer 1 Script Behavior** — Three transforms:
   - Remove ToolkitScriptManager entirely
   - Strip prefix on 16 known controls
   - Replace unrecognized ACT controls with TODO comments
5. **Blazor Project Setup** — NuGet package, @using directives, @rendermode, no manual script tags
6. **Migration Example** — Before/after showing ToolkitScriptManager removal + prefix stripping
7. **TargetControlID Resolution** — How extenders find target controls via HTML ID
8. **What's NOT Supported** — Unrecognized controls → manual Layer 2 replacement
9. **Links to comprehensive docs** — Reference to per-component docs and migration guide

**Parent Doc Update:**
Updated `.squad/skills/migration-standards/SKILL.md` to add new section at end:
- Brief intro to ACT companion doc
- Lists 4 key topics covered (detection, L1 automation, project setup, supported components)
- Links to online docs (index.md, migration-guide.md)

**Key Teaching Insights:**
- Ajax Control Toolkit is a special case: extenders are non-rendering (only attach JS behavior), containers are rendering (hold child content)
- L1 script handles ACT mechanically: strip prefix like asp: prefix, remove ToolkitScriptManager (Blazor equivalent: native script loading)
- Render mode is critical — InteractiveServer required for all ACT components
- TargetControlID pattern is platform-agnostic: find by HTML element ID (works same way in Blazor as Web Forms)
- Unrecognized ACT controls are flagged with TODO + manual item log — developers know exactly what needs Layer 2 work

**Design Decision:**
- Placed at `.squad/skills/` (like other skill docs) rather than `docs/` because it's tooling-focused (L1 automation) not user-facing
- Cross-references the user-facing docs (docs/AjaxToolkit/) for comprehensive per-component details
- Emphasizes that L1 is purely mechanical (prefix stripping); L2 is where real work happens (validation, testing, JS interop troubleshooting)

**Files:**

### bwfc-migration Skill: AJAX-TOOLKIT.md Child Document (2026-03-15)

**Delivered:** Complete child skill document `migration-toolkit/skills/bwfc-migration/AJAX-TOOLKIT.md` for Ajax Control Toolkit extender migration patterns within the main bwfc-migration skill.

**Content Structure (20.7 KB, 10 sections):**

1. **Overview** — What the Ajax Control Toolkit is (14 supported extender/container components), why BlazorAjaxToolkitComponents package exists, confirmation that ACT is now covered
2. **Installation** — Four-step setup: NuGet package, @using directives, InteractiveServer render mode, JS auto-loading explanation
3. **Detection** — How to identify ACT usage: Register directives, ToolkitScriptManager, `<ajaxToolkit:*>` prefixed components
4. **Control Translation Table** — All 14 supported controls with types (Extender vs. Container) and brief descriptions
5. **Migration Pattern** — L1 script automation explanation: what the script does (strip prefix, remove ToolkitScriptManager, remove Register directives, preserve properties)
6. **Before/After Examples** — Three progressively complex examples:
   - ConfirmButtonExtender (simplest, event handler conversion only)
   - AutoCompleteExtender (common, requires ServiceMethod callback wiring)
   - TabContainer with TabPanels (container pattern, nested children)
7. **Key Concept: TargetControlID and ID Rendering** — How extenders find targets, why ID attributes matter, common gotchas
8. **Layer 1 Script Automation** — What bwfc-migrate.ps1 does automatically, supported vs. unsupported control handling
9. **Layer 2 Manual Work** — Five post-L1 tasks: NuGet reference, @using directives, ServiceMethod wiring for AutoComplete, TargetControlID verification, unsupported control replacement strategies
10. **Common Scenarios** — Three realistic workflows with zero/minimal Layer 2 work required
11. **Unsupported Controls & Alternatives** — Table showing DragPanelExtender, ResizableControlExtender, etc., with CSS/JS interop alternatives
12. **Render Mode & JavaScript** — Why InteractiveServer required, recommended pattern, graceful degradation
13. **Troubleshooting** — Two common issues (extender not activating, target not found) with diagnostic steps and solutions

**Replaces Outdated Reference:** Removed the "AJAX Toolkit Extenders | Blazor interactivity or JS interop |" line from CONTROL-REFERENCE.md "Not Covered" table (line 306). Added new "### Ajax Control Toolkit Extenders" section below AJAX Controls, with table of 14 supported components and cross-reference to AJAX-TOOLKIT.md.

**Updated Parent Skill (SKILL.md):**
- Layer 2 mandatory read block (lines 144–148): Added AJAX-TOOLKIT.md to three-document reading list with brief description of content
- Reference Documents section (lines 253–257): Added AJAX-TOOLKIT.md entry with description of L1 automation and L2 manual work

**Files Modified:**
- Created `migration-toolkit/skills/bwfc-migration/AJAX-TOOLKIT.md` (20.7 KB)
- Updated `migration-toolkit/skills/bwfc-migration/SKILL.md` — two locations (Layer 2 read block, Reference Documents section)
- Updated `migration-toolkit/skills/bwfc-migration/CONTROL-REFERENCE.md` — replaced "Not Covered" entry with new "Ajax Control Toolkit Extenders" section

**Design Decisions:**
- **Format consistency:** Followed existing child doc format (header, parent skill reference, horizontal rule, then sections) to integrate seamlessly with CODE-TRANSFORMS.md and CONTROL-REFERENCE.md
- **Target audience:** Layer 2 Copilot engineer migrating a real Web Forms app with ACT components. Assumes familiarity with bwfc-migration Layer 1/2 pipeline.
- **Completeness:** Covered all 14 components (Accordion, AccordionPane, AutoCompleteExtender, CalendarExtender, CollapsiblePanelExtender, ConfirmButtonExtender, FilteredTextBoxExtender, HoverMenuExtender, MaskedEditExtender, ModalPopupExtender, NumericUpDownExtender, PopupControlExtender, SliderExtender, TabContainer, TabPanel, ToggleButtonExtender) with migration mechanics and before/after examples
- **ServiceMethod wiring:** Highlighted AutoCompleteExtender's special Layer 2 work (moving from `.asmx` web service to Blazor callback method) with complete code example
- **Error messaging:** Emphasized TargetControlID as a gotcha — most extender bugs trace back to ID mismatches
- **Unsupported controls:** Listed alternatives with difficulty levels (Easy/Medium) to help developers choose replacement strategies (CSS, JS interop, Blazor components)

**Learnings for future skill docs:**
- Child docs inherit scope and audience from parent — AJAX-TOOLKIT.md is "for Copilot engineers doing Layer 2 work," not "for users learning ACT"
- Format consistency (header, parent ref, sections) is critical for skill tooling that cross-references documents
- Before/After examples should progress from trivial (ConfirmButtonExtender — change one attribute signature) to realistic (AutoCompleteExtender — rewrite entire data fetching mechanism)
- L1 automation explanation belongs in skill docs (how the script transforms ACT markup), not in user docs (how to use ACT in Blazor)
- Unsupported control table should include difficulty/complexity guidance to help Copilot choose replacement strategies

**Files:**
- Created `.squad/skills/migration-standards/ajax-toolkit-migration.md` (12.5 KB)
- Updated `.squad/skills/migration-standards/SKILL.md` — added "Ajax Control Toolkit Migration" reference section

**Next Steps (for Cyclops/Rogue):**
- Ensure L1 script uses this skill doc as reference when auditing/enhancing ACT handling
- L2 agents should consult per-component docs when troubleshooting ACT issues






## BaseValidator & BaseCompareValidator Documentation (2026-03-17)

 **Session (2026-03-17 by Beast):**
- Created docs/ValidationControls/BaseValidator.md  6.6 KB comprehensive base class docs covering:
  - Abstract base class overview for all validators
  - Shared properties: ControlToValidate, ControlRef, Display, Text, ErrorMessage, ValidationGroup, Enabled, style properties
  - ForwardRef<InputBase<T>> pattern for Blazor-native field binding
  - Validation lifecycle (EditContext integration, cascading context, registration/validation/cleanup)
  - Child validator references (RequiredFieldValidator, CompareValidator, RangeValidator, RegularExpressionValidator, CustomValidator)
  - Web Forms  Blazor comparison with code examples

- Created docs/ValidationControls/BaseCompareValidator.md  6.4 KB docs for comparison-based validators:
  - Abstract base class extending BaseValidator with type conversion and comparison logic
  - Type property with supported types table (String, Integer, Double, Date, Currency)
  - CultureInvariantValues property explanation with practical examples
  - Type conversion and comparison logic documentation
  - Comprehensive examples (Integer, Date, Currency) with real code samples
  - Web Forms  Blazor syntax comparison
  - Child validator references (CompareValidator, RangeValidator)

- Updated mkdocs.yml  added BaseCompareValidator and BaseValidator alphabetically in Validation Controls section
- Verified MkDocs build: --strict mode passes with no broken links (55.59 seconds build time)

**Pattern Consistency:**
- Followed RequiredFieldValidator.md and CompareValidator.md formatting conventions
- Maintained heading structure: Overview, Properties, Examples, Web Forms comparison, Child references
- Used property tables for enums (Display, Type values)
- Included Microsoft documentation links to original Web Forms classes
- All Blazor code examples shown with EditForm context

**Key Decisions:**
- BaseValidator docs positioned as "framework for all validators" not a user-facing component
- Emphasized ControlRef as Blazor-native approach; ControlToValidate as Web Forms migration bridge
- Type conversion explanation in BaseCompareValidator targets developers migrating numeric/date comparisons
- CultureInvariantValues documentation included practical locale examples (US "." vs European "," decimals)

**Files:**
- Created docs/ValidationControls/BaseValidator.md 
- Created docs/ValidationControls/BaseCompareValidator.md
- Updated mkdocs.yml (Validation Controls section)

# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- ⚠ Summarized 2026-05-16 by Scribe — covers M1–Run11 (Feb–Jul 2026) -->

### Historical Summary (2026-02-10 through 2026-03-06)

**Doc patterns:** Component docs: title → intro (MS link) → Features Supported → NOT Supported → Web Forms syntax → Blazor syntax → HTML Output → Migration Notes → Examples. mkdocs.yml nav alphabetical. Migration section leads with "Getting started" and "Migration Strategies".

**Key doc deliverables:** M1–M3 (PasswordRecovery, DetailsView, Chart), M8 (release polish), M9 (gap audit: FormView, DataGrid, ChangePassword), M10 (Skins & Themes practical guide), M17 (AJAX: Timer, ScriptManager, UpdatePanel, UpdateProgress, Substitution). MasterPages.md created with full architecture + before/after + nesting patterns. Migration Toolkit: README, QUICKSTART, CONTROL-COVERAGE (58 components), METHODOLOGY, CHECKLIST — all self-contained, no repo references.

**Benchmark reports (Runs 4–11):** Pattern locked: exec summary, metrics table, run-over-run comparison, recommendations. Run 4–6: pipeline quality improvements (transforms 309→269). Run 7: first runtime-validated (14/14 acceptance tests pass). Runs 9–11: preservation rate 98.9% (176/178). Convention: BENCHMARK-DATA.md (Bishop) → BENCHMARK-REPORT.md (Beast). Acceptance tests are primary quality gate (Runs 7+).

**Team decisions locked:** WebFormsPageBase is canonical page base class. LoginView is native BWFC (never AuthorizeView). Layer 2 conventions: Button OnClick uses EventArgs, class names match .razor filenames, EF Core wildcards, CartStateService replaces Session, GridView uses explicit TItem. Shims (Request, Response, Session, Server, Cache, ClientScript) are trusted. BWFC data controls never replaced with raw HTML. Page.Request/Response/Session deliberately omitted from migration docs.

**Migration toolkit:** Moved to migration-toolkit/skills/ (end-user distributable). Squad Places artifact published: "What 110+ Web Forms Controls Taught Us About Migration-First Component Design". Run 10–11 achieved 98.9% BWFC preservation and <25 min Layer 2 time.

### 2026-05-16: AutomatedMigrationWithCopilot.md & DepartmentPortal Strategy

**Task:** Write Copilot migration guide and file DepartmentPortal blockers.

**Changes delivered:**
- Created docs/Migration/AutomatedMigrationWithCopilot.md — guide anchored to WingtipToys (26/26 acceptance tests), covering L1 transforms, L2 Copilot patterns, shim table, benchmarks (WingtipToys 26/26, ContosoUniversity 37/40), practical tips
- Updated mkdocs.yml with entry under Migration > Plan
- DepartmentPortal analyzed: 14 pages, 58 standard BWFC controls, 6 code-only server controls (not handled)
- GitHub issues filed: #549 (code-only scaffolder P1), #550 (namespace tag prefix P2), #551 (YARP analyzer future)
- Three labels provisioned: migration-toolkit, nalyzers, uture

**Pattern established:** New migration guides must anchor to proven benchmarks, include end-to-end walkthroughs (prescan → migrate → build → L2 → test → iterate), close with common-mistake tips, link to existing docs (don't duplicate theory).

**Key outcomes:** DepartmentPortal support deferred pending P1/P2 implementation. ContosoUniversity <5 min benchmark target confirmed. Code-only controls migrate via CLI scaffolder (not new components).

≡ Team update (2026-05-16): DepartmentPortal migration blockers (#549, #550, #551) filed — decision merged — decided by Bishop
≡ Team update (2026-05-16): AutomatedMigrationWithCopilot.md pattern established (anchor to benchmarks, walkthrough + tips) — decision merged — decided by Beast

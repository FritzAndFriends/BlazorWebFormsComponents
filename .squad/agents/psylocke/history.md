# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Project Learnings (from import)

- The project has two skill locations: `.squad/skills/` (team-earned skills) and `migration-toolkit/skills/` (shipped migration skills for end users)
- Existing migration skills: bwfc-migration, bwfc-data-migration, bwfc-identity-migration, migration-standards
- Existing team skills: base-class-upgrade, blazor-parameter-aliases, component-documentation, migration-standards, sample-pages, shared-base-extraction, squad-conventions, status-reconciliation, webforms-html-audit
- A known critical failure mode: agents consistently replace BWFC controls with plain HTML during migration (Layer 2 problem). Skills must have mandatory rules to prevent this.
- The migration-toolkit also contains: scripts (bwfc-migrate.ps1), copilot-instructions-template.md, METHODOLOGY.md, CHECKLIST.md, CONTROL-COVERAGE.md
- Skills use SKILL.md format with confidence levels: low, medium, high
- The BWFC component library has 110+ components that must be preserved during migration

## Learnings

### 2026-03-06: Run 7 Skill Updates

**Skills updated:**
- `migration-standards/SKILL.md` — Major update with 6 specific changes from Run 7 and Jeff's directives:
  - `WebFormsPageBase` replaces `ComponentBase` as canonical base class for migrated pages
  - LoginView is now a native BWFC component — removed the old LoginView → AuthorizeView conversion
  - Page_Load → OnInitializedAsync codified as DEFAULT RULE
  - CSS `<link>` elements MUST go to App.razor, not layout
  - MasterPage migration preserves BWFC semantics
  - Fixed "Using Page as Base Class" anti-pattern to show `WebFormsPageBase`
  - Added Runtime Gotchas table (4 issues discovered in benchmarks)

**Skills created:**
- `blazor-auth-migration/SKILL.md` (medium confidence) — Scoped AuthenticationStateProvider + cookie auth pattern. Singleton providers cause session bleed. Discovered in Run 7 Iteration 2.
- `blazor-form-submission/SKILL.md` (low confidence) — Blazor strips onclick from buttons during enhanced navigation. Two patterns: anchor-based POST for auth forms, EditForm for in-component handling. First observation from Run 7 Iteration 3.

**Key patterns codified:**
- `@inherits WebFormsPageBase` in `_Imports.razor` is a scaffold requirement — without it Page.Title, IsPostBack, GetRouteUrl all fail
- Cookie auth registration order matters: AddAuthentication → AddCookie → AddScoped<Provider> → AddScoped<AuthenticationStateProvider>(factory)
- The anchor-based form submit (`<a role="button">`) is a workaround, not necessarily the long-term pattern — marked as low confidence

 Team update (2026-03-06): WebFormsPageBase is the canonical base class for all migrated pages (not ComponentBase). All agents must use WebFormsPageBase  decided by Jeffrey T. Fritz
 Team update (2026-03-06): LoginView is a native BWFC component  do NOT convert to AuthorizeView. Strip asp: prefix only  decided by Jeffrey T. Fritz

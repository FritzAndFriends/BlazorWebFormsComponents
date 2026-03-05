# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Project Learnings (from import)

- The project has two skill locations: `.ai-team/skills/` (team-earned skills) and `migration-toolkit/skills/` (shipped migration skills for end users)
- Existing migration skills: bwfc-migration, bwfc-data-migration, bwfc-identity-migration, migration-standards
- Existing team skills: base-class-upgrade, blazor-parameter-aliases, component-documentation, migration-standards, sample-pages, shared-base-extraction, squad-conventions, status-reconciliation, webforms-html-audit
- A known critical failure mode: agents consistently replace BWFC controls with plain HTML during migration (Layer 2 problem). Skills must have mandatory rules to prevent this.
- The migration-toolkit also contains: scripts (bwfc-migrate.ps1), copilot-instructions-template.md, METHODOLOGY.md, CHECKLIST.md, CONTROL-COVERAGE.md
- Skills use SKILL.md format with confidence levels: low, medium, high
- The BWFC component library has 110+ components that must be preserved during migration

## Learnings

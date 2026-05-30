# ASCX / Custom-Control Migration Planning

**Owner:** Forge (Lead / Web Forms Reviewer)  
**Created:** 2026-05-30  
**Status:** Planning  
**Branch:** `feature/ascx-custom-control-migration`

## Overview

This folder contains planning documents and issue tracking for the ASCX (ASP.NET User Controls) and custom-control migration pipeline. This is a critical gap in the current migration toolkit: user controls and custom base classes cannot yet be migrated automatically, blocking real Web Forms applications that rely on component reuse and custom control frameworks.

## Key Blockers

User controls (`.ascx` files) and custom-control frameworks are pervasive in legacy Web Forms applications and still require iterative transform coverage. Current state:

- 🟡 `.ascx` files run through the shared migration pipeline; baseline template expression rewrites (`<%# Eval(...) %>`) and `<ContentTemplate>` unwrapping are now in P1, with advanced templating still pending
- ❌ Custom HTML controls (`.cs` inheriting from `WebControl`) are not migrated
- 🟡 `FindControl()` readiness is partially automated via `@ref` + backing-field scaffolding; direct callsite rewrites remain manual
- ✅ ASCX property/event descriptors are now analyzed in prescan/runtime detection (`AscxDescriptorAnalyzer`)
- 🟡 ASCX lifecycle/data-binding has baseline transforms (`Page_Load` mapping, `DataBind()` cleanup + `Items` injection), but advanced patterns are still pending
- ✅ Web.config namespace registrations for custom controls are now parsed (`WebConfigAssemblyParser`)

TODO(P1-followup): close remaining gaps for complex `FindControl(...)` callsite rewrites and advanced lifecycle hooks (`OnLoad`/`Page_PreRender`) before marking P1 complete.

## Work Breakdown

This effort is organized into **two high-level streams**:

### Stream 1: Infrastructure (Foundation)
1. **Web.config Parser** — Extract tag/namespace registrations
2. **Custom Base-Class Shim Generator** — Generate compatibility shims for custom WebControl bases
3. **ASCX Descriptor Analyzer** — Parse ASCX properties, events, and dependencies

### Stream 2: Transforms (Automation)
1. **ContentTemplate Unwrapper** — Extract templated markup from ASCX
2. **FindControl-to-@ref Transform** — Convert server-side control queries to Blazor references
3. **ASCX Binding/Lifecycle Transforms** — Rewrite data binding and control lifecycle patterns
4. **Custom-Control Scaffolder** — Generate initial BWFC-based component skeletons

### Stream 3: Tooling (Skills + Testing)
1. **ASCX Skill** — Custom Copilot skill for ASCX migration patterns
2. **Custom-Control Skill** — Custom Copilot skill for WebControl-based components
3. **Integration Tests** — Acceptance tests for ASCX/custom-control benchmark app

## Priority & Scope

| Priority | Item | Stream | Notes |
|----------|------|--------|-------|
| P0 | Web.config tag/namespace parser | 1 (Infrastructure) | Blocks all downstream transforms; unblocks WingtipToys |
| P0 | ASCX descriptor analyzer | 1 (Infrastructure) | Blocks Content/Template unwrapping; enables Layer 1 scanning |
| P1 | ContentTemplate unwrapper | 2 (Transforms) | Blocks findcontrol, binding transforms; enables basic ASCX→component skeleton |
| P1 | FindControl-to-@ref transform | 2 (Transforms) | Enables code-behind rewrite for control queries |
| P2 | Custom base-class shim generator | 1 (Infrastructure) | Blocks custom WebControl adoption; deferred pending P0+P1 analysis |
| P2 | ASCX binding/lifecycle transforms | 2 (Transforms) | Blocks full ASCX migration; needs descriptor analysis first |
| P3 | Custom-control scaffolder | 2 (Transforms) | Deferred: enables generation of BWFC stubs only after descriptors+binding known |
| P3 | ASCX Skill | 3 (Tooling) | Deferred: written after P0–P2 transforms stable |
| P3 | Custom-Control Skill | 3 (Tooling) | Deferred: written after P0–P2 transforms + shim generator stable |
| P3 | Integration tests | 3 (Tooling) | Deferred: written after P0–P2 working end-to-end |

## Acceptance Criteria

When all work is complete:

1. **WingtipToys benchmark run** includes user controls (e.g., `ProductsControl.ascx`, `CartControl.ascx`) that are transformed into Blazor components or scaffolds.
2. **CLI prescan** detects `.ascx` files and reports estimated migration complexity (requires descriptor analysis).
3. **CLI migrate** produces valid Blazor markup for basic ASCX files (property/event mappings, template unwrapping, FindControl rewrites).
4. **No manual editing required** for ASCX files in the P0 happy-path scope (simple controls with properties, events, and templated content).
5. **Layer 2 acceptance tests** pass for transformed user controls when paired with updated code-behind.

## Next Steps

1. ✅ Create feature branch `feature/ascx-custom-control-migration`
2. ✅ Create this planning doc + issue tracking
3. ✅ Create GitHub issues for P0 items (Web.config parser, ASCX descriptor analyzer)
4. ✅ Implement P0 issues in parallel (#555 + #557)
5. → Keep docs and CLI tests current as P1 transforms land
6. → Validate P0 with WingtipToys prescan, then move to P1 transforms

## Validation Cadence on This Branch

- Primary regression command: `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo`
- Run after each control-migration increment (analysis, transform, or scaffolding step)
- Keep docs updated in the same PR as behavior changes

## Related Documents

- [Track: Web.config Parser](./track-webconfig-parser.md)
- [Track: ASCX Descriptor Analyzer](./track-ascx-analyzer.md)
- [Track: ContentTemplate Unwrapper](./track-contenttemplate.md)
- [Track: FindControl Transform](./track-findcontrol.md)

## Team Assignments

- **Forge (Lead/Reviewer)** — Architecture, scope, Web Forms behavior validation
- **Bishop (CLI Developer)** — Transform implementation
- **Cyclops (Component Dev)** — Custom base-class shims
- **Rogue (Test Lead)** — Integration tests
- **Beast (Documentation)** — Migration guides

---

See individual track docs for detailed work breakdown and implementation contracts.

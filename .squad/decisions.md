# Decisions

> Shared team decisions. All agents read this. Only Scribe writes here (by merging from inbox).

# Bishop decision inbox — G3/G4 fixes

- **Date:** 2026-05-08T10:42:43-04:00
- **Owner:** Bishop

## Decision
For benchmark-facing identity scaffolds, the CLI should emit one consistent auth contract end to end:
- account semantic rewrites post to `/Account/LoginHandler` and `/Account/RegisterHandler`
- generated `Program.cs` configures application cookie `LoginPath` and `LogoutPath`
- redirect handler stubs use ASP.NET Core Identity (`SignInManager<IdentityUser>` / `UserManager<IdentityUser>`) and preserve `ReturnUrl` when it is local

For validator typing, `RequiredFieldValidator` should infer its generic `Type` from the validated control when possible (notably `TextBox` -> `string`) and only fall back to `object` when no control hint exists.

## Why
Run 42 showed that mismatched auth contracts caused the only first-pass failure, while blanket validator defaults created avoidable generic warnings. Encoding both decisions in the CLI keeps Layer 1 output runnable without forcing manual post-processing.


# Bishop G6/G7 Fixes

Date: 2026-05-08T11:37:18.862-04:00
Branch: feature/wingtip-next-features-review
PR: #545

## Decisions

1. `PageDirectiveTransform` now emits a source-relative primary `@page` route for nested `.aspx` pages and keeps the filename-only alias as a secondary route when the two differ. Root-level pages still emit a single route, and `Default.aspx`/`Index.aspx` continue to resolve to `/`.
2. `PageQuarantineDetector` now bypasses quarantine for redirect-only action pages when their markup is inert and their code-behind uses `Response.Redirect`, unless identity or payment signals are also present.
3. CLI docs were updated to describe nested route aliasing and the redirect-only quarantine exemption because both behaviors change generated migration output in operator-visible ways.
# Decision: ComponentRefCodeBehindTransform ClassOpenRegex Fix

**Date:** 2026-05-15T09:55:50-04:00  
**Author:** Bishop  
**Status:** Implemented

## Problem

`ComponentRefCodeBehindTransform` (Order 220) was silently skipping ALL field injection because its `ClassOpenRegex` (`partial\s+class\s+\w+\s*\{`) failed to match class declarations that include a base class.

`BaseClassStripTransform` (Order 200) runs **before** our transform and rewrites:
```
public partial class ShoppingCart : System.Web.UI.Page
```
to:
```
public partial class ShoppingCart : WebFormsPageBase
```

The regex `\s*\{` requires only whitespace before `{` — it cannot match `: WebFormsPageBase\n    {`. Result: every `@ref` field was never injected, producing 15+ CS0103 errors per page in Run 80.

## Decision

Change `ClassOpenRegex` from:
```csharp
@"partial\s+class\s+\w+\s*\{"
```
to:
```csharp
@"partial\s+class\s+\w+[^{]*\{"
```

`[^{]*` matches any characters (including `: WebFormsPageBase`, `, IDisposable`, newlines, spaces) up to the first `{`.

## Rationale

- Surgical one-character-class change with no side effects.
- `[^{]*` is the idiomatic pattern for "skip anything before the next `{`" in class-declaration regexes.
- Pattern generalizes to future transforms that need to locate the class body opening brace.

## Rule Established

> Any code-behind transform that locates the class body opening brace via regex **must** use `[^{]*\{` (not `\s*\{`) to account for base classes and interface lists injected by earlier transforms.

## Verification

- 35/35 `ComponentRef` tests pass (8 pre-existing + 3 new regression cases covering `: WebFormsPageBase`, `: Base, IInterface`, and brace-on-next-line variants).
- CLI build clean.


# Decision: Lock GridView TemplateField Preservation with Dual-Layer Regression Tests

**Date:** 2026-05-08T13:02:09-04:00
**Author:** Bishop
**Status:** Proposed

## Decision
Add TemplateField preservation regression coverage at both layers of the CLI test surface:

1. `tests/BlazorWebFormsComponents.Cli.Tests/TransformUnit/GridViewColumnItemTypeTransformTests.cs`
2. `tests/BlazorWebFormsComponents.Cli.Tests/PipelineIntegrationTests.cs`

The tests must prove that `TemplateField` columns survive alongside `BoundField` siblings and when they are the only GridView columns, while preserving nested `TextBox`, `CheckBox`, and inline display-expression content.

## Rationale
TemplateField loss is a high-severity migration failure because it silently drops editable inputs, calculated output, and row-selection controls. The current pipeline behavior depends on multiple ordered markup transforms working together, so a single focused unit test is not enough; we need both transform-level and end-to-end file-migration assertions to keep future ordering changes from regressing Wingtip-style GridViews.

## Files
- `tests/BlazorWebFormsComponents.Cli.Tests/TransformUnit/GridViewColumnItemTypeTransformTests.cs`
- `tests/BlazorWebFormsComponents.Cli.Tests/PipelineIntegrationTests.cs`


# Decision: ComponentRefCodeBehindTransform Test Coverage Complete

**Date:** 2026-05-15
**Author:** Rogue (QA Analyst)
**Status:** Informational — no action required from team

## Finding

The `ComponentRefCodeBehindTransformTests.cs` test file was already fully populated
with 10 well-structured tests covering all required scenarios. All 10 tests pass
against the current transform implementation.

## Test Coverage Map

| Scenario | Test Method | Status |
|---|---|---|
| Basic field generation (Label) | `InjectsFieldForLabel` | ✅ Pass |
| Basic field generation (generic) | `InjectsFieldForGenericGridView` | ✅ Pass |
| Multiple refs | `InjectsMultipleFields` | ✅ Pass |
| Field inserted before methods | `InsertsAfterClassOpeningBrace` | ✅ Pass |
| Empty ComponentRefs → unchanged | `SkipsWhenNoComponentRefs` | ✅ Pass |
| No class found → passthrough | `SkipsWhenNoClassFound` | ✅ Pass |
| No duplicate declarations | `SkipsExistingFieldDeclaration` | ✅ Pass |
| Alphabetical ordering | `FieldsAreSortedAlphabetically` | ✅ Pass |
| Generic ListView type | `WorksWithGenericListViewType` | ✅ Pass |
| Order property | `OrderIs220` | ✅ Pass |

## Field Declaration Contract

The transform emits fields in the form:
```csharp
private {Type} {controlId} = default!;
```

The `= default!;` null-forgiving initializer is mandatory — Blazor `@ref` targets
must be non-nullable fields but are assigned by the framework at render time.

## Recommendation

No further test work needed for this transform. Bishop's implementation is solid.
The duplicate-detection regex (`(?:private|protected|public|internal)\s+\w+.*\b{id}\s*[;=]`)
correctly handles the case where an existing declaration is present.


# 2026-05-16: Route Parameter Collision Dedupe

**Date:** 2026-05-15T11:43:54.133-04:00  
**Author:** Bishop  
**Status:** Established

## Decision

When the migration pipeline leaves multiple `[Parameter]` properties whose names match the same `@page` route token case-insensitively, keep the property whose casing exactly matches the route token if one exists, and remove the other duplicates.

## Why

Blazor treats parameter names case-insensitively, so pairs like `categoryName` and `CategoryName` trigger a runtime duplicate-parameter failure even though they look distinct in generated code. Preferring the exact route-token casing preserves route binding semantics while removing the extra wrapper-generated property.

## Scope

Apply this rule in CLI post-processing for page code-behind after route-parameter promotion, using the paired `.razor` file to discover route tokens.


# 2026-05-16: DepartmentPortal Migration Blocker Issues Filed

**Date:** 2026-05-16T15:22:00-04:00  
**Author:** Bishop  
**Status:** Informational — issues filed, no code changes

## Summary

Three GitHub issues were created in `fritzandfriends/BlazorWebFormsComponents` to track DepartmentPortal migration blockers identified by Forge's gap analysis.

| Issue | # | Labels | Priority |
|-------|---|--------|----------|
| CLI: Add code-only server control scaffolder for DepartmentPortal-style controls | [#549](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/549) | enhancement, migration-toolkit | Tier 1 / P1 |
| CLI: Parse namespace-level tag prefix registrations from Web.config | [#550](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/550) | enhancement, migration-toolkit | Tier 1 / P2 |
| Review analyzers for step-by-step YARP-based incremental migration workflow | [#551](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/551) | enhancement, analyzers, future | Future |

## New Labels Provisioned

Three labels were created to support these and future issues:

- **`migration-toolkit`** — CLI migration pipeline and toolkit enhancements
- **`analyzers`** — Roslyn analyzer work
- **`future`** — Scheduled for future implementation, not immediate

## Design Decisions

### Issue #549 — CodeOnlyControlScaffolder

Base class mapping established for the scaffolder:

| Web Forms Base | BWFC Base Class |
|---------------|-----------------|
| `WebControl` | `BaseStyledComponent` |
| `CompositeControl` | `BaseWebFormsComponent` |
| `DataBoundControl` | `DataBoundComponent<T>` |
| `Control` | `BaseWebFormsComponent` |

This scaffolder must run **before markup transforms** in the pipeline so that `LocalTagNamespaceResolutionTransform` has the emitted stub inventory available during markup resolution.

### Issue #550 — LocalTagNamespaceResolutionTransform

This transform is the markup-side companion to Issue #549. Dependency order: CodeOnlyControlScaffolder runs first (scaffold phase), LocalTagNamespaceResolutionTransform uses the output during markup phase. Both must be registered in `Program.cs` DI and `TestHelpers.CreateDefaultPipeline()` per project conventions.

### Issue #551 — YARP Analyzer Review

Scoped as a spike/review — no implementation expected immediately. Output should be documented in `docs/Analyzers/` or as issue comments. Assigned `future` label to signal post-DepartmentPortal scheduling.


# 2026-05-16: Automated Migration with Copilot Doc Placement and Pattern

**Date:** 2026-05-16T15:22:00-04:00  
**Author:** Beast (Technical Writer)  
**Status:** Established — nav and doc pattern locked

## Decision

`docs/Migration/AutomatedMigrationWithCopilot.md` is placed under the **Plan** subsection of the Migration nav, alongside the existing "Automated Migration Guide" (the older four-step pipeline doc).

```yaml
- Plan:
    - Automated Migration Guide: Migration/AutomatedMigration.md
    - Automated Migration with Copilot: Migration/AutomatedMigrationWithCopilot.md
```

## Rationale

The existing Migration docs divide into theory (Methodology.md), quick path (QuickStart.md), older pipeline (AutomatedMigration.md), and strategy (Strategies.md). A Copilot-specific guide belongs in Plan because it describes *how to plan and execute* a full automated migration — not a specific implementation pattern like MasterPages.md.

The new guide is differentiated from existing docs by:

1. Being anchored to a specific proven benchmark (WingtipToys, 26/26 acceptance tests)
2. Covering the Copilot-specific workflow (prompt patterns, skill file references)
3. Including the prescan → migrate → build → L2 Copilot repair → run → acceptance test → iterate loop
4. Surfacing benchmark numbers (WingtipToys 26/26, ContosoUniversity 37/40)

## Pattern Established

New migration guides at this level of detail should:

- Be anchored to at least one proven benchmark app with acceptance test results
- Include a complete end-to-end walkthrough (not just concepts)
- Close with a tips section covering the most common mistakes
- Link to existing docs for theory and component references rather than duplicating them


# 2026-05-16: User Directive — ContosoUniversity Benchmark Target

**Date:** 2026-05-16T13:10:29-04:00  
**Author:** Jeffrey T. Fritz  
**Status:** Guidance captured

## What

ContosoUniversity is much simpler than WingtipToys and should be migrated in less than 5 minutes. This is the benchmark target.

## Why

User request — captured for team memory.


# 2026-05-16: User Directive — DepartmentPortal Migration Strategy

**Date:** 2026-05-16T15:22:00-04:00  
**Author:** Jeffrey T. Fritz  
**Status:** Guidance captured

## What

Code-only server controls (P1) should migrate using existing shim control classes — we need a migration class in the CLI, not new components. Namespace tag prefix (P2) is confirmed as a real gap. Both should be parked as GitHub issues. Also: generate a migration document about steps to migrate automatically with Copilot, and schedule future work to review analyzers for step-by-step slower migration with YARP.

## Why

User request — captured for team memory. This strategy unlocks DepartmentPortal migration without new component implementations.



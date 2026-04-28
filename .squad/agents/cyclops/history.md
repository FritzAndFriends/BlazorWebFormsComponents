# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!--  Summarized 2026-02-27 by Scribe  covers M1M16 -->


### 2026-04-28: CLI Semantic Pattern Catalog Entries

**Task:** Add the next isolated semantic pattern catalog entries for the migration CLI without touching compile-surface filtering or hand-repairing samples.

**Changes delivered:**
- Added `AccountPagesSemanticPattern` under `src/BlazorWebFormsComponents.Cli/SemanticPatterns/` with ID `pattern-account-pages`.
- Added `MasterContentContractsSemanticPattern` under `src/BlazorWebFormsComponents.Cli/SemanticPatterns/` with ID `pattern-master-content-contracts`.
- Added `SemanticPatternMarkupHelpers` to keep wrapper extraction, `ChildComponents` parameter insertion, and named-region rewrites isolated inside the semantic subsystem.
- Added focused CLI tests in `tests/BlazorWebFormsComponents.Cli.Tests/SemanticPatternConcreteTests.cs`.

**Key behavior:**
- Account-page normalization now rewrites validator-heavy login/register-style markup into SSR-safe `<form method="post">` stubs and preserves explicit `TODO(bwfc-identity)` markers for real auth wiring.
- Master/content normalization now upgrades generated master shells to accept/render `ChildComponents` and rewrites wrapped content pages so named `<Content>` regions move under `<ChildComponents>`.

**Important paths:**
- `src/BlazorWebFormsComponents.Cli/SemanticPatterns/AccountPagesSemanticPattern.cs`
- `src/BlazorWebFormsComponents.Cli/SemanticPatterns/MasterContentContractsSemanticPattern.cs`
- `src/BlazorWebFormsComponents.Cli/SemanticPatterns/SemanticPatternMarkupHelpers.cs`
- `tests/BlazorWebFormsComponents.Cli.Tests/SemanticPatternConcreteTests.cs`

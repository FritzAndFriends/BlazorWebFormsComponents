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


≡ Team update (2026-05-07): Inbox merged, decisions consolidated — Scribe

### 2026-05-17: DetailsView BoundField support on prerender

**Task:** Fix `DetailsView` so explicit `BoundField` definitions work during prerender without falling back to auto-generated rows.

**Changes delivered:**
- Updated `DetailsView<ItemType>` to implement `IColumnCollection<ItemType>` and collect explicit child columns like `GridView` does.
- Mapped collected `IColumn<ItemType>` definitions into `DetailsViewField` adapters so existing row rendering can reuse `BoundField.Render()` / `RenderEdit()`.
- Updated `DetailsView.razor` to cascade `ColumnCollection` for either `<Fields>` content or direct child content.
- Hardened `BaseColumn<ItemType>` registration/disposal with null guards so unsupported or early prerender cases do not throw.
- Added bUnit coverage for explicit field order, direct child-content aliasing, and edit-mode rendering.

**Important paths:**
- `src/BlazorWebFormsComponents/DetailsView.razor`
- `src/BlazorWebFormsComponents/DetailsView.razor.cs`
- `src/BlazorWebFormsComponents/BaseColumn.cs`
- `src/BlazorWebFormsComponents.Test/DetailsView/BoundFields.razor`


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


≡ Team update (2026-05-07): Inbox merged, decisions consolidated — Scribe


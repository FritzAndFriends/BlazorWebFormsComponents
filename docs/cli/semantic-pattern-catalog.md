# Semantic Pattern Catalog

The CLI now has a dedicated **semantic pattern catalog** that runs after the normal markup and code-behind transforms. This gives recurring page-shape rewrites a bounded place to live without turning the flat transform list into a giant procedural migration pass.

Use this catalog for patterns that are more than syntax cleanup:

- query-driven detail pages
- action or redirect-only pages
- simple account and auth forms
- master/content contract normalization

## Where It Runs

The migration pipeline now has three relevant phases for page output:

1. **Markup transforms** make the `.aspx`, `.ascx`, or `.master` file syntactically safe for Blazor/BWFC output.
2. **Code-behind transforms** normalize the paired `.cs` file enough to expose the migration surface.
3. **Semantic pattern catalog** inspects the combined result and applies higher-level rewrites where the page matches a known Web Forms shape.

This means a semantic pattern always sees the post-transform state, not raw Web Forms source.

## Contract

Each catalog entry implements `ISemanticPattern`:

```csharp
public interface ISemanticPattern
{
    string Id { get; }
    int Order { get; }
    SemanticPatternMatch Match(SemanticPatternContext context);
    SemanticPatternResult Apply(SemanticPatternContext context);
}
```

The important rules are:

- **`Id`** must be stable so diagnostics and tests can target it.
- **`Order`** controls deterministic execution within the catalog.
- **`Match()`** should be lightweight and evidence-driven.
- **`Apply()`** can rewrite markup, code-behind, or both.

## Execution Context

`SemanticPatternContext` gives a pattern the information it needs for a bounded rewrite:

- `MigrationContext`
- `SourceFile`
- `FileMetadata`
- current `Markup`
- optional `CodeBehind`
- `MigrationReport`

Patterns should prefer evidence from the transformed markup/code-behind already in the context rather than reparsing the original project through side channels.

## Diagnostics and Report Behavior

When a pattern applies:

- the catalog updates `metadata.MarkupContent` and `metadata.CodeBehindContent`
- the pipeline log records the applied pattern ID and detail text
- `MigrationReport.SemanticPatternsApplied` is incremented

Patterns should emit a clear detail string that explains **why** the match happened or what contract was normalized. If a pattern can only do a partial rewrite safely, it should preserve a runnable output shape and leave explicit manual guidance in the emitted content or report.

## Current Catalog Entries

The first four catalog entries are:

| Order | Pattern | Purpose |
|-------|---------|---------|
| 100 | `QueryDetailsSemanticPattern` | Normalize query-string-driven detail pages toward SSR query-bound output |
| 200 | `MasterContentContractsSemanticPattern` | Keep generated master/content pairs on the valid `MasterPage` / `Content` / `ContentPlaceHolder` contract |
| 300 | `ActionPagesSemanticPattern` | Normalize redirect and action-only pages into usable SSR handler shapes |
| 400 | `AccountPagesSemanticPattern` | Convert simple account/login/register forms to SSR-safe form output with explicit auth TODO boundaries |

## When to Add a Semantic Pattern

Add a new catalog entry when all of these are true:

1. The migration issue is a **recurring page shape**, not a one-off sample repair.
2. The fix needs **both markup and behavioral evidence** after normal transforms.
3. The rewrite should stay isolated from the general transform list.
4. The pattern can produce a better default output without pretending the migration is fully complete.

Do **not** use the catalog for simple token swaps, attribute normalization, or prefix stripping. Those still belong in the normal transform pipeline.

## How to Add a New Pattern

1. Create a new class under `src\BlazorWebFormsComponents.Cli\SemanticPatterns\`.
2. Implement `ISemanticPattern` with a stable `Id`, an `Order`, a small matcher, and a bounded applicator.
3. Register it in **both** places:
   - `src\BlazorWebFormsComponents.Cli\Program.cs`
   - `tests\BlazorWebFormsComponents.Cli.Tests\TestHelpers.cs`
4. Add focused tests:
   - catalog/runtime tests when behavior depends on ordering or pipeline placement
   - concrete pattern tests for the new page shape
5. Update this document when the catalog gains a new first-class entry.

## Testing Expectations

Every new pattern should have:

- at least one **positive** test showing the rewrite that should occur
- at least one **negative** or boundary test showing the matcher does not overreach
- coverage proving the pattern works in the default production/test registration order

Use `SemanticPatternCatalogTests.cs` for catalog mechanics and `SemanticPatternConcreteTests.cs` for pattern-specific behavior.

## Relationship to Compile-Surface Filtering

The semantic pattern catalog improves migrated **page output**. Compile-surface filtering is a separate step that keeps risky legacy support files out of the generated SSR build by quarantining them under `migration-artifacts\compile-surface\`.

Keep these concerns separate:

- **semantic patterns** improve runnable migrated pages
- **compile-surface filtering** prevents unsupported legacy infrastructure from poisoning the generated build

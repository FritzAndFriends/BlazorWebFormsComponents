# Decision: Idempotent Code Fixes for Advisory Analyzers

**Author:** Cyclops  
**Date:** 2026-07-25  
**Context:** BWFC010/BWFC011 implementation

## Decision

When a Roslyn code fix adds a TODO comment but does NOT remove the underlying diagnostic (advisory/Info-severity analyzers), the code fix **must be idempotent** — check for existing TODO in leading trivia before adding.

## Rationale

The Roslyn test framework (and IDE) applies code fixes iteratively until convergence. If a fix adds a comment but the diagnostic persists, the fix runs again. Without an idempotency guard, duplicates accumulate.

## Pattern

```csharp
// In code fix provider, before adding TODO:
foreach (var trivia in node.GetLeadingTrivia())
{
    if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) &&
        trivia.ToString().Contains("TODO: <unique marker>"))
        return document; // Already applied
}
```

Test configuration for persistent diagnostics:
```csharp
FixedState = { ExpectedDiagnostics = { ... } },
NumberOfIncrementalIterations = 2,
NumberOfFixAllIterations = 2,
```

## Scope

Applies to all future BWFC analyzers at Info severity where the code fix doesn't resolve the diagnostic.

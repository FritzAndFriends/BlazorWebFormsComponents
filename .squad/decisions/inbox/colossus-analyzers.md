# Decision: BWFC Analyzer Expansion — BWFC020-023 (Migration Category)

**By:** Colossus (Integration Test Engineer, acting as Analyzer Expansion)
**Date:** 2026-03-22

## What

Added 4 new Roslyn analyzers for custom control migration patterns:

| ID | Name | Severity | Code Fix | Category |
|----|------|----------|----------|----------|
| BWFC020 | ViewStatePropertyPattern | Info | Yes — converts to [Parameter] auto-property | Migration |
| BWFC021 | FindControlUsage | Warning | Yes — replaces with FindControlRecursive | Migration |
| BWFC022 | PageClientScriptUsage | Warning | No | Migration |
| BWFC023 | IPostBackEventHandlerUsage | Warning | No | Migration |

## Technical Decisions

1. **New "Migration" category**: BWFC020-023 use category `"Migration"` instead of `"Usage"` (used by BWFC001-014). This differentiates migration-pattern analyzers from general usage analyzers. The `AllAnalyzers_HaveValidCategory` integration test was updated to accept both categories.

2. **Code fix approach for BWFC020**: Uses syntax tree manipulation (`WithAccessorList` + `AddAttributeLists`) without `NormalizeWhitespace()`. Previous attempts with `NormalizeWhitespace()` stripped indentation from the generated property. The pattern of modifying the existing node rather than building from scratch preserves whitespace correctly.

3. **FindControl code fix scope**: BWFC021 code fix only renames `FindControl` → `FindControlRecursive`. It does not attempt to add `using` directives or verify the containing class inherits from `BaseWebFormsComponent`. This keeps the fix simple and safe.

## Why This Matters

These 4 patterns are among the most common migration blockers developers hit when porting Web Forms custom controls. Detecting them early with actionable messages saves significant manual review time during migration.

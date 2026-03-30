# L1 Migration Script Test Framework Extension

**Date:** 2026-03-17  
**Author:** Colossus (Integration Test Engineer)  
**Status:** Implemented

## Context

The L1 migration script (`migration-toolkit/scripts/bwfc-migrate.ps1`) was enhanced with 6 new capabilities in Phase 1. The test framework at `migration-toolkit/tests/` needed expansion to provide regression coverage for these enhancements.

## Decision

Extended the L1 test suite from 15 to 18 test cases by adding:

1. **TC16-IsPostBackGuard** — Tests GAP-06: IsPostBack guard unwrapping
2. **TC17-BindExpression** — Tests GAP-13: Bind() → @bind-Value transform  
3. **TC18-UrlCleanup** — Tests GAP-20: .aspx URL cleanup in Response.Redirect calls

## Test Case Design Pattern

Each test case consists of:
- **Input:** `TC##-Name.aspx` (markup) + optional `TC##-Name.aspx.cs` (code-behind)
- **Expected:** `TC##-Name.razor` (expected markup output) + optional `TC##-Name.razor.cs` (expected code-behind output)

The test runner (`Run-L1Tests.ps1`) discovers test cases by scanning `inputs/` for `.aspx` files and comparing actual script output to expected output using normalized line-by-line comparison.

## Implementation Notes

- Expected files must match **actual script output** exactly, including:
  - Whitespace/indentation preserved from AST transformations
  - Attributes added by script (e.g., `ItemType="object"`)
  - Standard TODO comment headers
  - Base class removal (`: System.Web.UI.Page` stripped)
- URL cleanup only transforms `Response.Redirect()` arguments, not arbitrary string literals
- Test suite now at 78% pass rate (14/18), 98.2% line accuracy

## Rationale

These test cases provide:
1. Regression protection for Phase 1 enhancements
2. Documentation of expected script behavior through executable examples
3. Foundation for future enhancement testing

## References

- Migration script: `migration-toolkit/scripts/bwfc-migrate.ps1`
- Test runner: `migration-toolkit/tests/Run-L1Tests.ps1`
- Test inputs: `migration-toolkit/tests/inputs/`
- Expected outputs: `migration-toolkit/tests/expected/`

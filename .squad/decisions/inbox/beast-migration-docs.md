# Beast Documentation Updates — Run 22 Lessons Learned

**Date:** 2026-03-XX  
**Source:** ContosoUniversity Run 22 Benchmark (39/40 tests passing)  
**Changes:** 3 migration skill documents enhanced with concrete patterns and requirements

## Decision Summary

Enhanced the migration standards and bwfc-data-migration skill documents with Run 22 learnings. Each fix addresses a root cause that required extra build iterations or test failures during the benchmark.

### Doc Fix 1: `var` Usage Requirement (migration-standards)

**Issue:** Generated code with explicit type declarations caused IDE0007 build errors. `.editorconfig` enforces implicit typing as a build-blocking rule.

**Fix:** Added "Generated Code — Variable Declaration Styles" subsection before Page Lifecycle Mapping. States that:
- All local variable declarations MUST use `var` (implicit typing)
- Explicit types cause IDE0007 build failures
- Applies to both L1-generated and L2 Copilot-generated code
- Includes CORRECT vs WRONG examples

**Location:** migration-standards/SKILL.md, lines ~165–173

### Doc Fix 2: TextBox Binding Timing (migration-standards)

**Issue:** BWFC TextBox uses `@onchange` (blur), not `@oninput` (keystroke). Playwright `FillAsync()` triggers `input` events, but binding doesn't update until blur. Run 22 Students add-student test failed because Playwright filled the field and immediately submitted without blurring.

**Fix:** Added "TextBox Binding Timing for Playwright Tests" subsection after Blazor Enhanced Navigation. Provides:
- Clear explanation of BWFC TextBox behavior vs Web Forms semantics
- Recommended Playwright pattern: `BlurAsync()` or `PressAsync("Tab")` + small delay before submit
- Alternative using keyboard navigation
- Link to Web Forms equivalence (TextChanged also fires on blur)

**Location:** migration-standards/SKILL.md, lines ~157–199

### Doc Fix 3: Session State Examples (bwfc-data-migration)

**Issue:** Existing session state section documented the problem (HttpContext.Session null during WebSocket) and listed three options, but lacked concrete copy-pasteable code examples for ContosoUniversity-style scenarios.

**Fix:** Enhanced the "Session State Under Interactive Server Mode" section with:
- **Option A (Minimal API):** Concrete endpoint example for student add, with HttpClient call from component
- **Option B (Scoped Service):** In-memory CartService with List<CartItem> pattern and Program.cs registration
- **Option C (Database):** UserPreferencesService using IDbContextFactory, async/await patterns
- All examples use `var` for variable declarations (IDE0007 compliant)
- Added context: "For ContosoUniversity-style data modification scenarios"
- Antiforgery warning callout for Option A

**Location:** bwfc-data-migration/SKILL.md, lines ~29–95

## Rationale

Run 22 (39/40 tests) exposed three patterns that, when documented with concrete examples and enforcement notes, reduce future iteration cycles:

1. **IDE0007 enforcement** — Without explicit documentation, L2 agents may generate compliant code but the build fails. Now first-class guidance.
2. **Playwright timing** — The TextBox blur semantics are a BWFC-specific migration trap. Test authors need to know the pattern upfront.
3. **Session state examples** — Listing options without code leaves developers guessing. ContosoUniversity-style examples make the patterns immediately applicable.

These changes do NOT alter the canonical standards — they clarify existing practices with concrete Run 22 learnings.

## Files Modified

| File | Changes | Lines |
|------|---------|-------|
| migration-toolkit/skills/migration-standards/SKILL.md | Added `var` usage subsection + TextBox Playwright timing | ~165–199 |
| migration-toolkit/skills/bwfc-data-migration/SKILL.md | Enhanced Session State section with 3 copy-pasteable examples | ~29–95 |

## Verification

- ✅ Both skill files follow existing markdown structure and formatting
- ✅ Code examples use `var` consistently (IDE0007 compliant)
- ✅ Examples are ContosoUniversity-aligned (StudentDto, SchoolContext, Students.razor)
- ✅ Antiforgery warning callout for minimal API endpoints included
- ✅ No breaking changes to existing guidance

---

**Next:** Append Run 22 learnings to .squad/agents/beast/history.md under "## Learnings"

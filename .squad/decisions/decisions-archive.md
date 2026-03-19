# Archived Decisions

# Team Decisions

## 2026-03-14: Cyclops & Colossus: Students GridView LEFT JOIN + Test Timing

**Agents:** Cyclops (Component Dev), Colossus (Integration Test Engineer)  
**Date:** 2026-03-14  

### Decision 1: GetJoinedTableData LEFT JOIN Fix

**Problem:** `GetJoinedTableData()` used `SelectMany` (INNER JOIN), hiding students without enrollments from GridView.

**Solution:** Replaced with `Students.Include(Enrollments)` loop. Students without enrollments now appear with Count=0 and Date=DateTime.Today.

**Rationale:** Simplest correct approach. No GroupJoin complexity needed. Data loss bug resolved; each student appears exactly once with total enrollment count.

**Impact:** All students visible in GridView regardless of enrollment status. No other files changed. Build clean. Commit d3dc610f.

### Decision 2: Blur + Retry Pattern for BWFC TextBox Playwright Tests

**Problem:** `StudentsPage_AddNewStudentFormWorks` failing due to BWFC TextBox using `@onchange` (blur-triggered) for `TextChanged`, not `@oninput`. Playwright's `FillAsync` triggers `input` events; `change` fires only on blur.

**Solution:** 
1. Explicit blur after last field: `await emailBox.BlurAsync()` + 200ms wait
2. Increased post-click wait: 1000ms (up from 500ms)
3. Retry loop with 3-second deadline, polling every 300ms

**Convention:** Future BWFC TextBox Playwright tests must blur the last field before button submit. BWFC-specific requirement (Web Forms `onchange` semantics).

**Status:** Already implemented from previous session. Verified in place. No new changes needed.

---

## 2026-03-14: Forge Review: UpdatePanel ContentTemplate Enhancement

**Reviewer:** Forge (Lead / Web Forms Reviewer)  
**Related Agent:** Cyclops (implementation), Rogue (tests), Jubilee (samples)  
**Status:** ✅ APPROVED

### Summary

UpdatePanel ContentTemplate enhancement is production-ready. Added `ContentTemplate` RenderFragment parameter, updated base class to `BaseStyledComponent`, verified Web Forms fidelity.

### Verdict Checklist

| Criterion | Result |
|-----------|--------|
| Web Forms fidelity | ✅ PASS — `ContentTemplate` matches System.Web.UI.UpdatePanel property |
| HTML output | ✅ PASS — Renders as `<div>` (Block) or `<span>` (Inline), exactly matching Web Forms |
| Base class change | ✅ ACCEPTABLE — `BaseStyledComponent` is enhancement over expando `class` only; justified & maintains backward compatibility |
| Backward compatibility | ✅ PASS — Existing `<ChildContent>` patterns work; zero breaking changes |
| Migration story | ✅ PASS — L1 script output `<ContentTemplate>` compiles without RZ10012 warnings |
| Render mode decision | ✅ CORRECT — Library components don't force render modes |
| Tests (12 total) | ✅ PASS — 100% pass rate; all scenarios covered |
| Sample page | ✅ EXCELLENT — Reference quality; 6 examples + migration guide |

### Key Technical Validations

1. **ContentTemplate Property:** Confirmed in Microsoft Learn as property of `System.Web.UI.UpdatePanel` (NET Framework 3.5+). Blazor `RenderFragment` is correct equivalent.

2. **UpdatePanel Styling:** Web Forms UpdatePanel inherits from `Control` (not `WebControl`) but accepts expando attributes including `class`. Our `BaseStyledComponent` goes beyond original (adds full style properties) but is justified enhancement: doesn't break compatibility, improves consistency, better migration experience.

3. **Fallback Logic:** `@(ContentTemplate ?? ChildContent)` correctly prioritizes ContentTemplate while maintaining Blazor convention support.

### Team Recognition

- **Cyclops:** Clean implementation, thorough XML docs, correct architectural decisions
- **Rogue:** Comprehensive test coverage (12 tests, all scenarios)
- **Jubilee:** Gold-standard sample page with migration guide
- **All agents:** Perfect adherence to lockout protocol

**Approved by:** Forge  
**Status:** Ready to merge  
**Blocking issues:** None

---


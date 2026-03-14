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

## 2026-03-12: Beast: UpdatePanel ContentTemplate Documentation Update

**Owner:** Beast (Technical Writer)  
**Related:** UpdatePanel component enhancement (L1 migration now supports ContentTemplate cleanly)  

### Decision

Updated `migration-toolkit/skills/bwfc-migration/CONTROL-REFERENCE.md` to document UpdatePanel's new `ContentTemplate` RenderFragment support.

### Background

UpdatePanel now supports a `ContentTemplate` RenderFragment parameter. This enables L1 migration to convert Web Forms `<asp:UpdatePanel>` with `<ContentTemplate>` child elements to clean Blazor markup without RZ10012 warnings.

### Change Summary

**File updated:** `CONTROL-REFERENCE.md` (AJAX Controls section)

1. **Table entry:** Expanded UpdatePanel row to note `<ContentTemplate>` support
2. **New subsection:** Added "UpdatePanel with ContentTemplate" documenting:
   - Before/after code examples (Web Forms → Blazor)
   - Key points: ContentTemplate recognition, BaseStyledComponent inheritance, render mode placement
   - Clarification that render mode is set at app level via `App.razor`, not by UpdatePanel

### No Changes Needed

- `CODE-TRANSFORMS.md` — no ContentTemplate-specific code transforms
- `SKILL.md` — no special UpdatePanel migration notes
- `migration-standards/SKILL.md` — no ContentTemplate warnings to update
- `bwfc-data-migration/SKILL.md` — no UpdatePanel references
- `bwfc-identity-migration/SKILL.md` — ContentTemplate mentioned in context (no update needed)

### Rationale

**Surgical update approach:** Only CONTROL-REFERENCE.md needed changes. This is the canonical control translation reference table, so documenting new capabilities here ensures developers see the capability immediately.

### Impact

- Developers migrating Web Forms `<asp:UpdatePanel>` with `<ContentTemplate>` can proceed confidently with L1 output
- No breaking changes — purely additive documentation
- Aligns with improved component capability (BaseStyledComponent, render fragments)

---

## 2025-07-15: Forge Review: Cyclops's EDMX→EF Core Parser

**Reviewer:** Forge (Lead / Web Forms Reviewer)  
**Status:** ✅ APPROVED

Cyclops delivered a thorough, well-structured EDMX parser that correctly addresses every Run 21 failure. The script cleanly separates SSDL/CSDL/C-S Mapping parsing, produces valid EF Core entity classes with proper data annotations, and generates a complete DbContext with fluent FK configuration. The bwfc-migrate.ps1 integration is clean and handles file skip logic correctly.

### Run 21 Failure Checklist — All Pass

| Requirement | Status | Evidence |
|---|---|---|
| `Cours` has `[Key]` on `CourseID` | ✅ PASS | Cours.cs line 11 |
| `Cours` has `[Table("Courses")]` | ✅ PASS | Cours.cs line 8 — C-S Mapping correctly resolves entity→table mismatch |
| All 4 cascade deletes in `OnModelCreating()` | ✅ PASS | DbContext lines 25, 30, 44, 49 — all `.OnDelete(DeleteBehavior.Cascade)` |
| All FK relationships (HasOne/WithMany/HasForeignKey) | ✅ PASS | 4 complete fluent chains for FK_Courses_Departments, FK_Courses_Instructors, FK_Enrollment_Courses, FK_Enrollment_Students |
| `[Required]` on non-nullable strings | ✅ PASS | CourseName, DepartmentName, FirstName, LastName on all relevant entities |
| `[MaxLength]` on bounded strings | ✅ PASS | All bounded strings annotated (30, 20, 50 as appropriate) |
| `[DatabaseGenerated(Identity)]` on PKs | ✅ PASS | All 5 PK columns: CourseID, DepartmentID, EnrollmentID, InstructorID, StudentID |

### Edge Case Verification

| Check | Status | Notes |
|---|---|---|
| Nullable vs non-nullable | ✅ | `Email` (nullable string) has no `[Required]`; value types like `DateTime BirthDate` are non-nullable without `?` |
| Collection vs single nav props | ✅ | `ICollection<T>` for `*` multiplicity, single `virtual T` for `1` multiplicity — all 8 nav props correct |
| Entity→table name mapping | ✅ | `Cours`→`Courses`, `Department`→`Departments`, `Instructor`→`Instructors`, `Student`→`Students` all resolved; `Enrollment`→`Enrollment` correctly omits `[Table]` |
| bwfc-migrate.ps1 skip logic | ✅ | Skips `*.Designer.cs`, EDMX T4 stem files, and files already generated by converter — 3-layer filter is correct |

### Recommendations (Non-blocking)

These are **not** blocking approval but would improve the parser for broader EDMX files:

1. **Redundant `ToTable()` in OnModelCreating**: The `[Table("X")]` attribute on the class AND `entity.ToTable("X")` in fluent config are both emitted. Functionally harmless (they agree) but could confuse developers reading the output. Consider emitting only the attribute and dropping the fluent call, or vice versa.

2. **Nullable reference types**: With `<Nullable>enable</Nullable>` (default in .NET 6+ templates), nullable string properties like `Email` would produce CS8618 warnings. Consider emitting `string?` for nullable strings. Low priority — the generated code compiles and runs correctly regardless.

3. **Composite key limitation**: The script emits `[Key]` per-property but doesn't emit `[Column(Order=N)]` for composite keys. Not triggered by ContosoUniversity (all single-column PKs), but would fail for EDMX models with composite PKs. Consider adding `HasKey()` fluent config as a fallback when `keyProps.Count > 1`.

4. **`FixedLength` not translated**: `DepartmentName` has `FixedLength="true"` in the CSDL (maps to `nchar`). There's no direct EF Core data annotation equivalent. Would need `entity.Property(e => e.DepartmentName).IsFixedLength()` in fluent config. Minor — column type is inferred from database during migrations.

5. **OnDelete checked only on principal end**: Line 405 reads `$principalEnd.OnDelete` but doesn't check the dependent end. Standard EF6 convention puts OnDelete on the principal, but edge-case EDMX files might differ. Consider checking both ends.

### Script Quality Notes

- **Clean separation of concerns**: 4 well-labeled sections (C-S Mapping, CSDL parse, entity generation, DbContext generation)
- **Type mapping**: Comprehensive EDM→C# type map covering all common types including `Time`→`TimeSpan`
- **Defensive coding**: `Set-StrictMode -Version Latest`, validates input path, `ShouldProcess` support for `-WhatIf`
- **Skip-if-exists guard**: Won't overwrite hand-edited files (line 263-267) — safe for re-runs
- **Good diagnostics**: Summary object returned with entity count, relationship count, cascade count, and warnings

**Decision:** Approve and ship.

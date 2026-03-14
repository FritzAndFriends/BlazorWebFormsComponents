# Decision: GetJoinedTableData LEFT JOIN Fix

**Date:** 2026-07-25
**Author:** Cyclops
**Scope:** `samples/AfterContosoUniversity/BLL/StudentsListLogic.cs`

## Context

`GetJoinedTableData()` used `SelectMany` to flatten Students→Enrollments, which is an INNER JOIN. Students without enrollments were excluded from the GridView entirely. This is a data-loss bug: `InsertNewEntry()` can create a student without an enrollment (when no course is selected), and that student becomes invisible.

## Decision

Replaced `SelectMany` + `GroupBy` with a direct `Students.Include(Enrollments)` query that iterates all students:
- Students WITH enrollments: `Count = Enrollments.Count`, `Date = Min(enrollment dates)`
- Students WITHOUT enrollments: `Count = 0`, `Date = DateTime.Today.ToShortDateString()`

## Rationale

- Simplest correct approach — no GroupJoin complexity needed since we already eager-load enrollments via `Include`
- `DateTime.Today` chosen as default date for zero-enrollment students because there's no meaningful enrollment date to show; today's date signals "just added"
- Return shape (ID, Date, FullName, Email, Count) preserved exactly
- The old GroupBy-on-Date behavior (which could produce multiple rows per student if they enrolled on different dates) was a pre-existing quirk that is now resolved — each student appears exactly once with their total enrollment count

## Impact

- GridView now shows all students regardless of enrollment status
- No other files changed
- Build: 0 errors

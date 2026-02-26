# Decision: M15-10 Data Control Investigation Complete

**Date:** 2026-02-28
**Author:** Forge (Lead / Web Forms Reviewer)
**Task:** M15-10 (GitHub #392)
**Status:** For team review

---

## Context

Performed line-by-line classification of all HTML divergences in DataList, GridView, ListView, and Repeater using post-fix normalized captures. This updates the M13 analysis in `planning-docs/DATA-CONTROL-ANALYSIS.md`.

## Decisions

### 1. Bug Reclassification

3 of 5 M13 bugs are now FIXED (PR #377). 4 bugs remain:
- **BUG-GV-4b (P1):** `UseAccessibleHeader` defaults to `false` in Blazor; WF defaults to `true`. One-line fix.
- **BUG-GV-3 (P1):** `&amp;nbsp;` encoding in empty headers due to ternary expression type resolution. Fix with `@if`/`@else`.
- **BUG-GV-4a (P2):** `<thead>` vs `<tbody>` for header rows — see D-11 proposal below.
- **BUG-DL-2 (P3):** `itemtype` not rendered from generic type parameter.

### 2. Proposed D-11: `<thead>` Header Section

Blazor GridView always renders header rows in `<thead>`. WF GridView defaults to `<tbody>` for header rows (unless `HeaderRow.TableSection = TableRowSection.TableHeader` is explicitly set).

**Options:**
- (A) Register as D-11 intentional divergence — `<thead>` is semantically correct and preferred
- (B) Fix to match WF by putting headers in `<tbody>` when `UseAccessibleHeader=false`

**Forge's recommendation:** Option A. `<thead>` is the correct HTML5 semantic and no real-world migration will break because of this.

### 3. Sample Alignment is Critical Path

22 of 26 findings are sample parity. ListView and Repeater have ZERO component bugs — their entire diff is sample authoring differences. DataList and GridView have minor bugs but the vast majority of their diffs are also sample issues.

**Impact:** Sample alignment would immediately reduce total diff lines from 294 to ~13, revealing only genuine structural issues.

## Artifacts

- Full analysis: `planning-docs/M15-DATA-CONTROL-ANALYSIS.md`
- Prior analysis: `planning-docs/DATA-CONTROL-ANALYSIS.md`

## Who Needs to Know

- **Cyclops:** 3 bug fixes to implement (BUG-GV-4b, BUG-GV-3, BUG-DL-2)
- **Jubilee:** Sample alignment for all 4 data controls is P1
- **Colossus:** 2 normalizer fixes (empty `style=""` stripping, consistent `<div>` wrapper handling)
- **Jeff:** D-11 (`<thead>` vs `<tbody>`) needs decision

— Forge

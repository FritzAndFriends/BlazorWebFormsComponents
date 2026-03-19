# M15-10: Data Control Deep Investigation — Line-by-Line Classification

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-02-28
**Milestone:** M15 (GitHub #392)
**Status:** Complete
**Baseline:** Post-fix capture (after PR #377 merged 14 bug fixes)
**Prior analysis:** `planning-docs/DATA-CONTROL-ANALYSIS.md` (M13)

---

## Executive Summary

This is a line-by-line re-classification of all divergences in the 4 data controls using the **post-fix normalized** HTML captures. The prior M13 analysis identified 5 genuine bugs; 2 of those have been **fixed** in PR #377. This M15 re-assessment reclassifies every diff line against the current codebase.

**Headline finding:** Sample parity remains the overwhelming blocker. Of 294 diff lines across all 4 controls, only 4 are genuine component bugs. The rest are sample authoring differences.

**Updated Summary Table:**

| Control | Diff Lines | Genuine Bugs | Sample Parity | Normalizer | Intentional (D-code) | Fixed Since M13 |
|---------|-----------|-------------|---------------|------------|---------------------|----------------|
| DataList | 106 | 1 | 8 | 0 | 0 | 1 (BUG-DL-3) |
| GridView | 20 | 3 | 4 | 2 | 1 (D-01) | 2 (BUG-GV-1, BUG-GV-2) |
| ListView | 106 | 0 | 5 (+1 WF sample bug) | 0 | 0 | 0 |
| Repeater | 62 | 0 | 5 | 0 | 0 | 0 |
| **Total** | **294** | **4** | **22 (+1)** | **2** | **1** | **3** |

---

## DataList — 106 diff lines

### Summary
- Genuine bugs: 1
- Sample parity: 8
- Intentional: 0
- Normalizer: 0
- Fixed since M13: 1

### Line-by-Line Classification

| Line(s) | WF Content | BZ Content | Category | Description |
|---------|-----------|-----------|----------|-------------|
| WF:1 `id="simpleDataList"` | `id="simpleDataList"` | (absent) | Sample parity | Blazor sample doesn't set an ID. Component supports it via base class. |
| WF:1 `itemtype="SharedSampleObjects.Models.Widget"` | `itemtype="..."` | (absent) | Genuine bug (BUG-DL-2) | WF renders the `ItemType` value as an HTML `itemtype` microdata attribute on the `<table>`. Blazor's `GetItemTypeAttribute()` only checks `AdditionalAttributes`, not the generic `ItemType` type parameter. The component should render `typeof(ItemType).FullName` when the generic parameter is specified. |
| WF:1 `tabindex="1"` | `tabindex="1"` | (absent) | Sample parity | Blazor sample doesn't set `TabIndex`. Component supports it (DataList.razor line 28). |
| WF:1 `title="This is my tooltip"` | `title="..."` | (absent) | Sample parity | Blazor sample doesn't set `ToolTip`. Component supports it (DataList.razor line 29). |
| WF:2-3 | `<caption align="Top">This is my caption</caption>` | (absent) | Sample parity | Blazor sample doesn't set `Caption` or `CaptionAlign`. Component supports both (DataList.razor lines 33-36). |
| WF:4-7 | `<th class="myClass" scope="col" style="font-family:arial...; font-size:x-large; font-weight:bold; font-style:italic; text-decoration:underline overline line-through;">My Widget List</th>` | `<td>Simple Widgets</td>` | Sample parity | WF sample sets `UseAccessibleHeader=true` (→ `<th>` + `scope`), `HeaderStyle.CssClass="myClass"`, and extensive font styling. Blazor sample uses none of these. Different header text ("My Widget List" vs "Simple Widgets"). |
| WF:8-103 (item rows) | Each item: `<td style="background-color:#ffff00; white-space:nowrap;">Name<br/>$Price</td>` alternating with `background-color:#f5deb3` | Each item: `<td style="background-color:#f5deb3;">Name - $Price</td>` (uniform) | Sample parity | WF template uses `<br/>` between name and price; Blazor uses ` - `. WF alternates Yellow/Wheat; Blazor uses Wheat only. WF has `white-space:nowrap` from `Wrap="false"`; Blazor doesn't set Wrap. |
| WF:15,23,31,39,47,55,63,71,79,87,95 | `<td style="color:#ffefd5; background-color:#000000;">Hi! I'm a separator!...</td>` (11 separator rows) | (absent) | Sample parity | Blazor sample omits `SeparatorTemplate` entirely. Component supports it (DataList.razor line 62). |
| WF:103/BZ | `<td>End of Line</td>` | `<td>End of Line</td>` | ✅ Match | Footer template renders identically. |
| WF:1 (no `style=`) | (no border-collapse) | (no border-collapse) | ✅ FIXED (was BUG-DL-3) | PR #377 fixed conditional `border-collapse` in `GetTableStyle()`. Now correctly omits it when `GridLines=None` (default). Verified in current normalized output. |

### Recommendations
- **BUG-DL-2 (itemtype):** Low priority (P3). Fix `GetItemTypeAttribute()` to also return `typeof(ItemType).FullName` when no explicit `itemtype` is in AdditionalAttributes. Rarely impacts real migrations — `itemtype` is a microdata attribute.
- **Sample alignment:** The Blazor DataList sample needs: `ToolTip`, `TabIndex`, `Caption`, `CaptionAlign`, `CellPadding`, `CellSpacing`, `HeaderStyle` with fonts, `UseAccessibleHeader`, `SeparatorTemplate` with `SeparatorStyle`, `AlternatingItemStyle`, `ItemStyle.Wrap="false"`, and matching template markup (`Name<br/>$Price` instead of `Name - $Price`).
- **Achievable match:** Near-exact after sample alignment + BUG-DL-2 fix. Estimated: **>95% match**.

---

## GridView — 20 diff lines

### Summary
- Genuine bugs: 3
- Sample parity: 4
- Intentional: 1 (D-01)
- Normalizer: 2
- Fixed since M13: 2

### Line-by-Line Classification

| Line(s) | WF Content | BZ Content | Category | Description |
|---------|-----------|-----------|----------|-------------|
| WF:1,19 `<div>...</div>` | `<div>` wrapper around `<table>` | (absent) | Normalizer artifact | The `<div data-audit-control="GridView">` wrapper is retained in WF normalized output but stripped in Blazor. The normalizer should handle both consistently. Not a component issue. |
| WF:2 `id="CustomersGridView"` | `id="CustomersGridView"` | (absent) | D-01 + Sample parity | ID mangling (D-01 intentional divergence). Additionally, Blazor sample doesn't set any ID attribute. |
| WF:2 (no `class`) vs BZ `class="table table-striped"` | (no class) | `class="table table-striped"` | Sample parity | Blazor sample adds `CssClass="table table-striped"` which WF sample doesn't have. |
| WF:2 / BZ `style="border-collapse:collapse;"` | `style="border-collapse:collapse;"` | `style="border-collapse:collapse;"` | ✅ FIXED (was BUG-GV-2) | Both now render `border-collapse:collapse;`. Fixed in PR #377. |
| WF:2 (no `rules`/`border`) vs BZ `rules="all" border="1"` | (absent in normalized) | `rules="all" border="1"` | ✅ FIXED (was BUG-GV-1) | Wait — reversed. WF raw has `cellspacing="0" rules="all" border="1"`. Normalized WF also has these. Normalized Blazor also has `rules` and `border` in the style. The default `GridLines.Both` now correctly renders `rules="all" border="1"`. Fixed in PR #377. |
| WF:3-4 `<tbody><tr><th scope="col">` | Header row in `<tbody>` with `<th scope="col">` | Header row in `<thead>` with `<th>` (no scope) | Genuine bug (BUG-GV-4) | **Two issues:** (1) WF puts header row in `<tbody>` by default; Blazor always uses `<thead>`. While `<thead>` is semantically better, it doesn't match WF output. (2) WF defaults `UseAccessibleHeader=true` (since .NET 3.5 SP1), rendering `scope="col"` on `<th>` elements. Blazor defaults `UseAccessibleHeader` to `false` (GridView.razor.cs line 230), omitting `scope`. Default value mismatch. |
| WF:4 `CustomerID` | HeaderText `CustomerID` | HeaderText `ID` | Sample parity | Different `HeaderText` values in the samples. |
| WF:5 `&nbsp;` vs BZ `&amp;nbsp;` | `&nbsp;` (non-breaking space) in empty header cells | `&amp;nbsp;` (HTML-encoded literal) | Genuine bug (BUG-GV-3) | The ternary expression on GridView.razor line 38 — `@(string.IsNullOrEmpty(column.HeaderText) ? (MarkupString)"&nbsp;" : column.HeaderText)` — fails because the ternary resolves to `object`/`string` type, causing Blazor to HTML-encode the `MarkupString`. Fix: use `@if`/`@else` instead of ternary. |
| WF:8,12,16 `<a href="...">Search for ...</a>` vs BZ `<input style="" type="submit" />` | HyperLinkField with Bing search URLs | ButtonField rendering as `<input type="submit">` | Sample parity | WF sample uses `HyperLinkField`; Blazor sample uses `ButtonField`. Different column types. |
| BZ `style=""` on `<input>` | (no empty style) | `style=""` | Normalizer artifact | Empty `style` attribute on ButtonField's `<input>`. Normalizer should strip empty `style=""` attributes. |

### Recommendations
1. **BUG-GV-4a (`<thead>` vs `<tbody>`):** P2. Consider making header section configurable — add `HeaderRow.TableSection` support or match WF default by rendering header in `<tbody>` when `UseAccessibleHeader=false`. However, `<thead>` is semantically correct; consider registering as D-11 intentional divergence instead.
2. **BUG-GV-4b (`UseAccessibleHeader` default):** P1. Change default from `false` to `true` in GridView.razor.cs line 230 to match WF's .NET 3.5 SP1+ default. One-line fix: `public bool UseAccessibleHeader { get; set; } = true;`
3. **BUG-GV-3 (`&amp;nbsp;` encoding):** P1. Replace the ternary on line 38 of GridView.razor with explicit `@if`/`@else` to preserve `MarkupString` rendering. Small, surgical fix.
4. **Normalizer:** Strip empty `style=""` attributes. Strip `<div>` wrapper from data-audit-control consistently.
5. **Sample alignment:** Change Blazor HeaderText `"ID"` → `"CustomerID"`. Replace `ButtonField` with `HyperLinkField`. Remove `CssClass="table table-striped"`.
6. **Achievable match:** Near-exact after sample alignment + bug fixes. Estimated: **>90% match** (remaining gap: `<thead>` vs `<tbody>` structural choice).

---

## ListView — 106 diff lines

### Summary
- Genuine bugs: 0
- Sample parity: 5
- WF sample bug: 1
- Intentional: 0
- Normalizer: 0

### Line-by-Line Classification

| Line(s) | WF Content | BZ Content | Category | Description |
|---------|-----------|-----------|----------|-------------|
| WF:1 `<table>` vs BZ `<table class="table">` | No class | `class="table"` | Sample parity | Blazor sample adds Bootstrap `class="table"` on the outer `<table>`. WF sample has a bare `<table>` in LayoutTemplate. |
| WF:18,28,36,44,52,60,68,76,84,92,100 (separator rows) | `<tr><td colspan="4" style="border-bottom:1px solid #000000;">&nbsp;</td></tr>` (11 separator rows between items) | (absent) | Sample parity | Blazor sample has `ItemSeparatorTemplate` **commented out** with `@*...*@` (Index.razor lines 43-46). Component supports it; this is a sample authoring decision. |
| WF:21,39,57,75,93,111 (AlternatingItem Id col) | `$2.00`, `$4.00`, `$6.00`, `$8.00`, `$10.00`, `$12.00` | `2`, `4`, `6`, `8`, `10`, `12` | WF sample bug | WF sample uses `DataBinder.Eval(Container.DataItem, "Id", "{0:C}")` which formats integer `Id` as currency. This is a bug in the WF sample — `Id` is an integer, not a price. Blazor sample correctly renders `@Item.Id`. |
| WF:5-116 / BZ (common widget data) | Widget names, prices, dates match | Widget names, prices match | ✅ Match | Both samples use the same `Widget.SimpleWidgetList` data source with identical names and prices. |
| Date values | `2/26/2026` | `2/26/2026` | ✅ Match | Dates now match (same capture date in post-fix run). |

### Structural Component Assessment

The ListView component introduces **no structural divergences**. The Blazor `ListView.razor` correctly implements:
- `ItemTemplate` / `AlternatingItemTemplate` with proper alternation
- `ItemSeparatorTemplate` (functional, just commented out in sample)
- `LayoutTemplate` as outer container
- `EmptyDataTemplate` for empty state
- `GroupTemplate` with `GroupItemCount` for grouped layouts

**No genuine bugs found in the ListView component.**

### Recommendations
- **Fix WF sample bug:** Change `DataBinder.Eval(Container.DataItem, "Id", "{0:C}")` to `<%# Item.Id %>` in the WF AlternatingItemTemplate.
- **Sample alignment:** Uncomment Blazor `ItemSeparatorTemplate`. Remove `class="table"` from outer `<table>`. Optionally move `<table>` structure into a `LayoutTemplate` inside the ListView (matching WF pattern).
- **Achievable match:** **Exact match** after sample alignment. Zero component bugs.

---

## Repeater — 62 diff lines

### Summary
- Genuine bugs: 0
- Sample parity: 5
- Intentional: 0
- Normalizer: 0

### Line-by-Line Classification

| Line(s) | WF Content | BZ Content | Category | Description |
|---------|-----------|-----------|----------|-------------|
| WF:1 | `This is a list of widgets` (HeaderTemplate plain text) | (absent — no HeaderTemplate) | Sample parity | WF has HeaderTemplate; Blazor omits it. Component supports HeaderTemplate. |
| WF:2,4,6,8,10,12,14,16,18,20,22,24 | `<li>Widget Name</li>` (simple list items) | `<tr><td>Id</td><td>Name</td><td>$Price</td><td>Date</td></tr>` (table rows, 4 columns) | Sample parity | Completely different template structures. WF: simple `<li>` per item with name only. Blazor: `<tr>` with 4 data columns (Id, Name, Price, LastUpdate). |
| WF:3,5,7,9,11,13,15,17,19,21,23 | `<hr/>` (SeparatorTemplate) | `<tr><td colspan="4"><hr/></td></tr>` (table-wrapped separator) | Sample parity | Both use `<hr/>` but in different structural contexts. WF has bare `<hr/>` between `<li>` items. Blazor wraps `<hr/>` in a table cell. Both are correct for their template structure. |
| WF:25 | `This is the footer of the control` (FooterTemplate plain text) | (absent — no FooterTemplate) | Sample parity | WF has FooterTemplate; Blazor omits it. Component supports FooterTemplate. |
| BZ (entire output) | — | Wrapped in `<table>` element | Sample parity | Blazor sample page adds `<table>` around the Repeater component (Index.razor line 14). This is page-level markup, not component behavior. WF sample has no wrapping element. |

### Structural Component Assessment

The Repeater is the simplest data control — it iterates items with templates and nothing else. Both the WF and Blazor components render `HeaderTemplate → (ItemTemplate|AlternatingItemTemplate with SeparatorTemplate) → FooterTemplate` identically. The 62-line diff is **100% attributable to different sample page authoring**.

The WF sample's use of bare `<li>` elements without a parent `<ul>` or `<ol>` is technically invalid HTML, but that's a WF sample issue, not a control issue.

**No genuine bugs found in the Repeater component.**

### Recommendations
- **Sample alignment:** Rewrite the Blazor Repeater sample to match the WF sample: use `<li>Widget Name</li>` items, bare `<hr/>` separator, header/footer plain text, and no wrapping `<table>`. Alternatively, update both samples to use the same structure.
- **Achievable match:** **Exact match** after sample alignment. Zero component bugs.

---

## Cross-Cutting Findings

### 1. Bugs Fixed Since M13

Three of the 5 bugs identified in the M13 analysis have been **fixed** in PR #377:

| Bug ID | Control | Fix | Status |
|--------|---------|-----|--------|
| BUG-DL-3 | DataList | Conditional `border-collapse` in `GetTableStyle()` based on `GridLines != None` | ✅ Fixed |
| BUG-GV-1 | GridView | Default `GridLines.Both` → `rules="all" border="1"` | ✅ Fixed |
| BUG-GV-2 | GridView | `border-collapse:collapse;` now rendered on table | ✅ Fixed |

### 2. Remaining Genuine Bugs: 4

| Bug ID | Control | Description | Severity | Fix Effort |
|--------|---------|-------------|----------|-----------|
| BUG-DL-2 | DataList | `itemtype` attribute not rendered from generic `ItemType` parameter | P3 (Low) | Small — modify `GetItemTypeAttribute()` |
| BUG-GV-3 | GridView | `&amp;nbsp;` instead of `&nbsp;` in empty header cells (ternary expression type issue) | P1 (Medium) | Small — replace ternary with `@if`/`@else` on line 38 |
| BUG-GV-4a | GridView | Header row in `<thead>` instead of WF's `<tbody>` | P2 (Low) | Medium — structural change or register as D-11 |
| BUG-GV-4b | GridView | `UseAccessibleHeader` defaults to `false`, WF defaults to `true` | P1 (Medium) | Trivial — one-line default value change |

### 3. Normalizer Gaps: 2

| Issue | Description | Fix |
|-------|-------------|-----|
| `<div>` wrapper inconsistency | Audit marker `<div>` retained in WF output but stripped in Blazor | Normalize both sides consistently |
| Empty `style=""` attributes | ButtonField renders `style=""` on `<input>` elements | Strip empty `style` attributes during normalization |

### 4. New Divergence Candidate

| Code | Description | Recommendation |
|------|-------------|----------------|
| D-11 (proposed) | Blazor uses `<thead>` for header rows; WF uses `<tbody>` by default | Register as intentional if team agrees `<thead>` is the better semantic choice. If not, fix GridView to match WF. |

### 5. Sample Parity Remains #1 Blocker

22 of 26 classified findings are sample parity issues. The Blazor samples were authored independently and exercise different features, data formats, and template structures than the WF samples. Until samples are aligned, the diff output is not meaningful for component quality assessment.

**Estimated impact of sample alignment alone:**
- DataList: 106 → ~5 diff lines (just BUG-DL-2)
- GridView: 20 → ~8 diff lines (just remaining bugs + structural `<thead>` choice)
- ListView: 106 → 0 diff lines (exact match)
- Repeater: 62 → 0 diff lines (exact match)

---

## Action Items (Updated from M13)

| # | Action | Owner | Priority | Status |
|---|--------|-------|----------|--------|
| 1 | ~~Fix BUG-DL-3: Conditional border-collapse~~ | ~~Cyclops~~ | ~~P1~~ | ✅ Done (PR #377) |
| 2 | ~~Fix BUG-GV-1: Default GridLines=Both~~ | ~~Cyclops~~ | ~~P1~~ | ✅ Done (PR #377) |
| 3 | ~~Fix BUG-GV-2: Default border-collapse~~ | ~~Cyclops~~ | ~~P1~~ | ✅ Done (PR #377) |
| 4 | Fix BUG-GV-4b: Change `UseAccessibleHeader` default to `true` | Cyclops | P1 | New |
| 5 | Fix BUG-GV-3: Replace ternary with `@if`/`@else` for `&nbsp;` | Cyclops | P1 | Revised |
| 6 | Fix BUG-DL-2: Render `itemtype` from generic type parameter | Cyclops | P3 | Carried |
| 7 | Decision: Register `<thead>` as D-11 or fix to match WF `<tbody>` | Forge | P2 | New |
| 8 | Normalizer: Strip empty `style=""` attributes | Colossus | P2 | New |
| 9 | Normalizer: Consistent `<div>` wrapper stripping | Colossus | P2 | New |
| 10 | Sample alignment: DataList, GridView, ListView, Repeater | Jubilee | P1 | Carried |
| 11 | Fix WF ListView sample: `DataBinder.Eval(..."Id", "{0:C}")` bug | Jubilee | P2 | Carried |

---

## Achievable Match Levels

| Control | Current Match | After Sample Alignment | After All Fixes | Permanently Divergent? |
|---------|-------------|----------------------|-----------------|----------------------|
| DataList | ~5% | ~95% | ~98% | No (minor `itemtype` gap only) |
| GridView | ~30% | ~70% | ~95% | Possibly (`<thead>` vs `<tbody>` if kept) |
| ListView | ~40% | **100%** | **100%** | No |
| Repeater | ~0% | **100%** | **100%** | No |

---

— Forge

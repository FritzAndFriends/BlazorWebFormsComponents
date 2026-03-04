# Tier 2 HTML Audit Findings Report — Milestone 12

**Created:** 2026-02-27
**Author:** Forge (Lead / Web Forms Reviewer)
**Milestone:** M12-05 / M12-06
**Status:** Complete
**Predecessor:** `planning-docs/AUDIT-REPORT-M11.md` (Tier 1 report)

---

## 1. Executive Summary

Milestone 12 expanded the HTML fidelity audit to Tier 2 data controls. WebForms captures grew from 66 variants (24 pages) in M11 to **83 variants across 31 pages**, adding gold-standard HTML for DataList, DataPager, DetailsView, FormView, GridView, ListView, Menu (9 variants), and Repeater. Blazor captures remain at **45 variants** — no new `data-audit-control` markers were added.

The comparison pipeline produced **86 divergence entries** with **0 exact matches**. After de-duplication (4 entries duplicated due to a Hyperlink/HyperLink case-sensitivity bug in the capture pipeline), there are **82 unique entries**.

### Classification Summary

| Category | Entries | Unique | % of 86 |
|----------|---------|--------|---------|
| **Sample Parity** | 26 | 22 | 30% |
| **Genuine Bug** | 23 | 23 | 27% |
| **Missing Blazor Capture** | 37 | 37 | 43% |
| **Intentional Divergence** (standalone) | 0 | 0 | 0% |
| **New Control** (no Blazor component) | 0 | 0 | 0% |
| **Total** | **86** | **82** | **100%** |

> **Note:** Several genuine bug entries also contain intentional divergence components (D-01 ID mangling, D-02/D-08 postback links). These are classified as Genuine Bug because the entry contains structural bugs beyond the intentional divergence. Intentional divergences are noted inline.

### Key Findings

1. **No Tier 1 bugs were fixed since M11.** All 8 genuine bugs identified in M11 (Button, BulletedList, LinkButton, Calendar, CheckBox, FileUpload, Image, RadioButtonList) remain open.
2. **Tier 2 WebForms captures are complete.** DataList, DataPager, DetailsView, FormView, GridView, ListView, and Repeater all have gold-standard HTML captured.
3. **Menu captures are comprehensive.** 9 Menu variants covering List/Table modes, Horizontal/Vertical orientation, and DataSource binding — excellent coverage.
4. **Blazor capture gap widened.** Missing Blazor captures grew from 21 (M11) to 37 (M12) because new WF captures have no Blazor counterparts.
5. **TreeView has significant structural differences** beyond the intentional D-07 divergence.

---

## 2. Comparison with M11

| Metric | M11 | M12 | Change |
|--------|-----|-----|--------|
| WebForms pages attempted | 30 | 31 | +1 |
| WebForms pages succeeded | 24 | 31 | **+7** |
| WebForms control variants captured | 66 | 83 | **+17** |
| Blazor pages with markers | 18 | 18 | — |
| Blazor control variants captured | 45 | 45 | — |
| Diff entries generated | 70 | 86 | +16 |
| Exact matches | 0 | 0 | — |
| Divergent entries | 49 | 49 | — |
| Missing in Blazor | 21 | 37 | **+16** |
| Genuine bugs identified | ~8–10 | 23 entries (10 controls) | Refined |
| Duplicate entries (pipeline bug) | 0 | 4 | +4 |

### What Changed

- **Improved:** 7 WebForms pages that failed with HTTP 500 in M11 (DataList, FormView, GridView, ListView, Menu, Repeater + DataPager/DetailsView newly added) now capture successfully with inline data sources.
- **Unchanged:** All 49 divergent entries from M11 remain — no Blazor component fixes were applied between milestones.
- **New captures:** 17 additional WebForms variants (7 Tier 2 data controls + 9 Menu variants + DataPager), all classified as "Missing Blazor Capture."
- **Regression:** The HyperLink case-sensitivity duplication (noted in M11 §7.4) remains unfixed, now producing 4 phantom entries.

---

## 3. Per-Control Breakdown

### 3.1 Tier 1 Controls (Carried Forward from M11)

| # | Control | Variants | Classification | Notes |
|---|---------|----------|---------------|-------|
| 1 | AdRotator | 1 divergent | **Sample Parity** | Different ad content (Bing vs Microsoft). `<a><img></a>` structure matches. |
| 2 | BulletedList | 3 divergent | **Genuine Bug** | `<ul>` vs `<ol>`, extra `<span>` wrapping, wrong `list-style-type`. |
| 3 | Button | 1 divergent, 4 missing | **Genuine Bug** + Missing Capture | `<button>` vs `<input type="submit">`. 4 WF variants lack Blazor counterparts. |
| 4 | Calendar | 7 divergent | **Genuine Bug** | Missing `style`, `title`, `<tbody>`, `width:14%`, day `title` attrs, full `abbr` names, nav sub-table. Also contains D-02/D-08 (postback links → `cursor:pointer`). |
| 5 | CheckBox | 3 divergent | **Genuine Bug** | Extra wrapping `<span>`, GUID-based IDs. Also contains D-01 (ID format). |
| 6 | CheckBoxList | 2 missing | **Missing Blazor Capture** | No `data-audit-control` markers in Blazor sample. |
| 7 | DropDownList | 6 divergent | **Sample Parity** | `<select>/<option>` structure correct. Differences are item text, `selected` format (`""` vs `"selected"`), missing `class`/`disabled` from different samples. D-01 (ID). |
| 8 | FileUpload | 1 divergent | **Genuine Bug** | GUID fragment leaks as stray attribute (`d71-4b24-8b1d-9049a956fd3b=""`). |
| 9 | HiddenField | 1 divergent | **Sample Parity** | `<input type="hidden">` structure identical. Only `value` and `id` differ. |
| 10 | HyperLink | 4 divergent (×2) | **Sample Parity** | Different URLs, text, attributes. `<a>` structure correct. ⚠️ Duplicated due to Hyperlink/HyperLink case-sensitivity pipeline bug. |
| 11 | Image | 2 divergent | **Sample Parity + Genuine Bug** | Different `src`/`alt` (parity), but Blazor emits empty `longdesc=""` (bug). |
| 12 | ImageButton | 2 missing | **Missing Blazor Capture** | No `data-audit-control` markers. |
| 13 | ImageMap | 1 divergent | **Sample Parity** | `<img usemap>` + `<map>/<area>` structure matches. Different coords/URLs. GUID map name (D-01). |
| 14 | Label | 3 divergent | **Sample Parity** | Variants 1–2: same `<span>` tag, different text/classes. Variant 3: `<label>` vs `<span>` — correct behavior, different features tested (AssociatedControlID). |
| 15 | LinkButton | 3 divergent | **Genuine Bug** | Missing `href` attribute, missing `CssClass` pass-through. |
| 16 | ListBox | 2 missing | **Missing Blazor Capture** | No `data-audit-control` markers. |
| 17 | Literal | 2 divergent, 1 missing | **Sample Parity** | Different text content. Structure (raw HTML pass-through) is correct. |
| 18 | Panel | 3 divergent | **Sample Parity** | `<div>` wrapper correct. Inner content differs entirely. Style pass-through works (Panel-3). |
| 19 | PlaceHolder | 1 divergent | **Sample Parity** | No wrapper element — correct. Different placeholder text. |
| 20 | RadioButton | 3 missing | **Missing Blazor Capture** | No `data-audit-control` markers. |
| 21 | RadioButtonList | 2 divergent | **Genuine Bug** | GUID-based IDs/names instead of stable developer IDs. `<table>` layout structure otherwise matches. |
| 22 | TextBox | 7 missing | **Missing Blazor Capture** | No `data-audit-control` markers. |
| 23 | TreeView | 1 divergent | **Genuine Bug** | Structural differences in tree node rendering beyond D-07 (JS interop). |

### 3.2 Tier 2 Controls (New in M12)

| # | Control | Variants | Classification | Notes |
|---|---------|----------|---------------|-------|
| 24 | DataList | 1 missing | **Missing Blazor Capture** | WF gold-standard captured. No Blazor `data-audit-control` marker. |
| 25 | DataPager | 1 missing | **Missing Blazor Capture** | WF gold-standard captured. No Blazor `data-audit-control` marker. |
| 26 | DetailsView | 1 missing | **Missing Blazor Capture** | WF gold-standard captured. No Blazor `data-audit-control` marker. |
| 27 | FormView | 1 missing | **Missing Blazor Capture** | WF gold-standard captured. No Blazor `data-audit-control` marker. |
| 28 | GridView | 1 missing | **Missing Blazor Capture** | WF gold-standard captured. No Blazor `data-audit-control` marker. |
| 29 | ListView | 1 missing | **Missing Blazor Capture** | WF gold-standard captured. No Blazor `data-audit-control` marker. |
| 30 | Repeater | 1 missing | **Missing Blazor Capture** | WF gold-standard captured. No Blazor `data-audit-control` marker. |

### 3.3 Menu Variants (New in M12)

All 9 Menu variants are classified as **Missing Blazor Capture** — WebForms gold-standard HTML exists but no Blazor `data-audit-control` markers are present.

| Variant | Mode | Orientation | Notes |
|---------|------|-------------|-------|
| Menu-1 | List | Vertical | Default configuration. Blazor Menu supports this mode. |
| Menu-2 | List | Horizontal | Blazor Menu supports this mode. |
| Menu-3 | Table | Vertical | **D-06 applies** — Blazor only implements List mode. |
| Menu-4 | Table | Horizontal | **D-06 applies** — Blazor only implements List mode. |
| Menu-5 | List | Vertical | With submenus/nesting. |
| Menu-6 | List | Horizontal | With submenus/nesting. |
| Menu-7 | Table | Vertical | With submenus. **D-06 applies.** |
| Menu-8 | Table | Horizontal | With submenus. **D-06 applies.** |
| Menu-9 | DataSource | — | DataSource-bound. Structural mode depends on binding configuration. |

> **D-06 Impact:** Menu variants 3, 4, 7, and 8 use `RenderingMode="Table"` which Blazor intentionally does not implement (D-06). Once Blazor captures are added, these 4 variants should be classified as **Intentional Divergence (D-06)**. The remaining 5 List-mode variants should produce comparable HTML and any structural differences would be genuine bugs. See backlog item `d06-menu-table-mode`.

---

## 4. Priority Fix List — Genuine Bugs

Ranked by migration impact (CSS/JS compatibility) and frequency. These are candidates for GitHub Issues.

### P1 — Critical (Wrong HTML Tag)

#### P1-A: Button renders `<button>` instead of `<input type="submit">`

| Field | Value |
|-------|-------|
| **Control** | Button |
| **Variant** | Button-1 |
| **Severity** | 🔴 Critical |
| **Expected (WebForms)** | `<input id="styleButton" style="color:#ffffff; background-color:#0000ff;" type="submit" value="Blue Button" />` |
| **Actual (Blazor)** | `<button accesskey="b" title="Click to submit" type="submit">Click me!</button>` |
| **Root Cause** | Blazor component renders `<button>` container element; WebForms renders `<input type="submit">` void element |
| **CSS Impact** | 🔴 HIGH — `input[type=submit]` selectors won't match `<button>` |
| **JS Impact** | 🟡 MEDIUM — `element.value` works on `<input>` but not `<button>` |
| **Fix** | Change Blazor Button to render `<input type="submit" value="...">` with `Text` mapped to `value` attribute |

#### P1-B: BulletedList renders `<ul>` instead of `<ol>` for numbered lists

| Field | Value |
|-------|-------|
| **Control** | BulletedList |
| **Variants** | BulletedList-1, -2, -3 |
| **Severity** | 🔴 Critical |
| **Expected (WebForms)** | `<ol id="blNumbered" style="list-style-type:decimal;"><li>First</li><li>Second</li></ol>` |
| **Actual (Blazor)** | `<ul style="list-style-type:disc;"><li><span>Item One</span></li></ul>` |
| **Root Cause** | Three bugs: (1) Always renders `<ul>` regardless of `BulletStyle`; (2) Wraps item text in unnecessary `<span>`; (3) Wrong `list-style-type` values |
| **CSS Impact** | 🔴 HIGH — `ol li` selectors won't match; counter-based styling breaks |
| **JS Impact** | 🟢 LOW |
| **Fix** | Render `<ol>` when BulletStyle is numbered; remove `<span>` wrapper; map BulletStyle to correct CSS `list-style-type` |

### P2 — High (Missing Attributes Affecting Layout/Interaction)

#### P2-A: LinkButton missing `href` and `CssClass`

| Field | Value |
|-------|-------|
| **Control** | LinkButton |
| **Variants** | LinkButton-1, -2, -3 |
| **Severity** | 🔴 High |
| **Expected (WebForms)** | `<a class="btn btn-primary" id="LinkButton1">Click Me</a>` |
| **Actual (Blazor)** | `<a>LinkButton1 with Command</a>` |
| **Root Cause** | Blazor LinkButton omits `href` attribute and does not pass `CssClass` to rendered `class` attribute |
| **CSS Impact** | 🔴 HIGH — No link styling without `href`; class-based styling won't apply |
| **JS Impact** | 🟡 MEDIUM — Not keyboard-navigable without `href` |
| **Fix** | Add `href="#"` or `href="javascript:void(0)"`; pass `CssClass` → `class` attribute |

#### P2-B: Calendar missing inline styles, `title`, cell widths, and navigation structure

| Field | Value |
|-------|-------|
| **Control** | Calendar |
| **Variants** | Calendar-1 through Calendar-7 (all) |
| **Severity** | 🔴 High |
| **Expected (WebForms)** | `<table style="border-width:1px; border-style:solid; border-collapse:collapse;" title="Calendar">` with `<td style="width:14%">` cells, `<a title="February 1">` day links, full `abbr="Sunday"` on `<th>`, nested nav `<table>` |
| **Actual (Blazor)** | `<table>` with no `style`, no `title`, abbreviated `abbr="Sun"`, no cell `width`, no day link `title`, flat nav row |
| **Root Cause** | Calendar component does not emit default inline styles, accessibility attributes, or the navigation sub-table structure |
| **CSS Impact** | 🔴 HIGH — Calendar appearance breaks without border styles and column widths |
| **JS Impact** | 🟢 LOW |
| **Fix** | Add `style` and `title` to `<table>`; add `width:14%` to `<td>` cells; use full day names in `abbr`; add `title` to day `<a>` links; restructure nav header to match WF nested table |

#### P2-C: TreeView structural differences

| Field | Value |
|-------|-------|
| **Control** | TreeView |
| **Variants** | TreeView (1) |
| **Severity** | 🔴 High |
| **Expected (WebForms)** | `<div>` node containers with `<table>/<tr>/<td>` layout for expand icons + node text, nested `<div>` children |
| **Actual (Blazor)** | Simplified node structure with different nesting |
| **Root Cause** | TreeView rendering does not replicate the WebForms `<div><table><tr><td>` node layout pattern |
| **CSS Impact** | 🔴 HIGH — CSS targeting TreeView table-based node layout will break |
| **JS Impact** | Intentional (D-07) — JavaScript event model replaced |
| **Fix** | Align node rendering to use `<div><table><tr><td>` structure per WF output |

### P3 — Medium (Structural Wrappers / ID Format)

#### P3-A: CheckBox adds extra wrapping `<span>`

| Field | Value |
|-------|-------|
| **Control** | CheckBox |
| **Variants** | CheckBox-1, -2, -3 |
| **Severity** | 🟡 Medium |
| **Expected (WebForms)** | `<input id="chkTerms" type="checkbox" /><label for="chkTerms">Accept Terms</label>` |
| **Actual (Blazor)** | `<span><input id="[GUID]" type="checkbox" /><label for="[GUID]">I agree to terms</label></span>` |
| **Root Cause** | (1) Extra `<span>` wrapper around checkbox+label pair; (2) GUID-based IDs instead of stable IDs |
| **CSS Impact** | 🟡 MEDIUM — Direct child combinators (`>`) may not match |
| **JS Impact** | 🟡 MEDIUM — GUID IDs are non-deterministic, cannot target by ID |
| **Fix** | Remove wrapping `<span>`; use stable, developer-meaningful IDs |

#### P3-B: RadioButtonList uses GUID-based IDs/names

| Field | Value |
|-------|-------|
| **Control** | RadioButtonList |
| **Variants** | RadioButtonList-1, -2 |
| **Severity** | 🟡 Medium |
| **Expected (WebForms)** | `<input id="RadioButtonList1_0" name="RadioButtonList1" type="radio" value="red" />` |
| **Actual (Blazor)** | `<input id="5f017a2765e34dce9468615893c65b58_0" name="5f017a2765e34dce9468615893c65b58" type="radio" value="red" />` |
| **Root Cause** | Component generates GUID for `name`/`id` grouping instead of using the developer-provided ID |
| **CSS Impact** | 🟡 MEDIUM — Cannot target radio inputs by ID |
| **JS Impact** | 🟡 MEDIUM — Non-deterministic IDs prevent `getElementById` usage |
| **Fix** | Use developer-provided `ID` property for `name`/`id` base; append `_0`, `_1`, etc. per WF convention |

#### P3-C: FileUpload GUID attribute leak

| Field | Value |
|-------|-------|
| **Control** | FileUpload |
| **Variants** | FileUpload (1) |
| **Severity** | 🟡 Medium |
| **Expected (WebForms)** | `<input id="FileUpload1" type="file" />` |
| **Actual (Blazor)** | `<input d71-4b24-8b1d-9049a956fd3b="" type="file" />` |
| **Root Cause** | Blazor's CSS isolation attribute (`b-[hash]`) or a GUID fragment is leaking as an HTML attribute |
| **CSS Impact** | 🟡 MEDIUM — Stray attribute is harmless but indicates a rendering defect |
| **JS Impact** | 🟢 LOW |
| **Fix** | Investigate source of stray GUID attribute; likely a CSS isolation scope marker or broken `@attributes` splatting |

### P4 — Low (Cosmetic)

#### P4-A: Image emits empty `longdesc=""`

| Field | Value |
|-------|-------|
| **Control** | Image |
| **Variants** | Image-1, Image-2 |
| **Severity** | 🟢 Low |
| **Expected (WebForms)** | `<img alt="Banner image" src="banner.png" />` (no `longdesc` when not set) |
| **Actual (Blazor)** | `<img alt="Sample placeholder image" longdesc="" src="placeholder.svg" />` |
| **Root Cause** | Component emits `longdesc=""` even when `DescriptionUrl` is empty/null |
| **CSS Impact** | 🟢 LOW — No visual impact |
| **JS Impact** | 🟢 LOW |
| **Fix** | Only render `longdesc` attribute when `DescriptionUrl` has a non-empty value |

---

## 5. Genuine Bug Summary Table

| # | Control | Variants | Severity | Primary Issue | M11 Status |
|---|---------|----------|----------|--------------|------------|
| 1 | Button | 1 | 🔴 Critical | `<button>` vs `<input type="submit">` | Known (P1) |
| 2 | BulletedList | 3 | 🔴 Critical | `<ul>` vs `<ol>`, extra `<span>`, wrong `list-style-type` | Known (P2) |
| 3 | LinkButton | 3 | 🔴 High | Missing `href`, missing `CssClass` | Known (P3) |
| 4 | Calendar | 7 | 🔴 High | Missing styles, title, cell widths, nav structure, accessibility attrs | Known (P4) |
| 5 | TreeView | 1 | 🔴 High | Structural node layout differences | Known (structural) |
| 6 | CheckBox | 3 | 🟡 Medium | Extra `<span>` wrapper, GUID IDs | Known (P5) |
| 7 | RadioButtonList | 2 | 🟡 Medium | GUID-based IDs/names | Known (P8) |
| 8 | FileUpload | 1 | 🟡 Medium | GUID attribute leak | Known (P6) |
| 9 | Image | 2 | 🟢 Low | Empty `longdesc=""` when not configured | Known (P7) |
| **Total** | | **23** | | **10 controls, 9 distinct bugs** | |

---

## 6. Missing Blazor Capture Inventory

### 6.1 Tier 1 Controls Needing Markers (Carried from M11)

| Control | Missing Variants | Blazor Component Exists? | Action |
|---------|-----------------|-------------------------|--------|
| Button | 4 (Button-2–5) | ✅ Yes | Add `data-audit-control` markers for additional variants |
| CheckBoxList | 2 | ✅ Yes | Add `data-audit-control` markers |
| ImageButton | 2 | ✅ Yes | Add `data-audit-control` markers |
| ListBox | 2 | ✅ Yes | Add `data-audit-control` markers |
| Literal | 1 (Literal-3) | ✅ Yes | Add `data-audit-control` marker for 3rd variant |
| RadioButton | 3 | ✅ Yes | Add `data-audit-control` markers |
| TextBox | 7 | ✅ Yes | Add `data-audit-control` markers |
| **Subtotal** | **21** | | |

### 6.2 Tier 2 Data Controls (New in M12)

| Control | Missing Variants | Blazor Component Exists? | Action |
|---------|-----------------|-------------------------|--------|
| DataList | 1 | ❓ Check | Add Blazor sample page with `data-audit-control` |
| DataPager | 1 | ❓ Check | Add Blazor sample page with `data-audit-control` |
| DetailsView | 1 | ❓ Check | Add Blazor sample page with `data-audit-control` |
| FormView | 1 | ❓ Check | Add Blazor sample page with `data-audit-control` |
| GridView | 1 | ✅ Yes | Add `data-audit-control` marker to existing sample |
| ListView | 1 | ✅ Yes | Add `data-audit-control` marker to existing sample |
| Repeater | 1 | ✅ Yes | Add `data-audit-control` marker to existing sample |
| **Subtotal** | **7** | | |

### 6.3 Menu Variants (New in M12)

| Variants | Missing | Blazor Component Exists? | Action |
|----------|---------|-------------------------|--------|
| Menu-1 through Menu-9 | 9 | ✅ Yes | Add Blazor sample page with 9 variants and `data-audit-control` markers. Note: 4 variants (Menu-3, -4, -7, -8) are Table mode → D-06 applies. |
| **Subtotal** | **9** | | |

**Total Missing Blazor Captures: 37**

---

## 7. Sample Parity Issues

These 22 unique entries (26 including 4 HyperLink duplicates) show divergences caused entirely by different sample content, not component bugs. The `<a>`, `<select>`, `<input>`, `<span>`, `<div>`, and other tag structures are structurally correct.

| Control | Variants | Issue |
|---------|----------|-------|
| AdRotator | 1 | Different ad content (Bing vs Microsoft) |
| DropDownList | 6 | Different item text/values; `selected=""` vs `selected="selected"` (HTML5 compat); missing `class`/`disabled` from different sample configurations |
| HiddenField | 1 | Different `value` and `id` |
| HyperLink | 4 (×2) | Different URLs, text, attributes; ⚠️ duplicated by case-sensitivity bug |
| Image | 2 | Different `src`, `alt` (also has `longdesc` bug — see P4-A) |
| ImageMap | 1 | Different coordinates, URLs, area definitions |
| Label | 3 | Different text, classes; variant 3 correctly uses `<label>` for `AssociatedControlID` |
| Literal | 2 | Different text content |
| Panel | 3 | Completely different inner content; `<div>` wrapper correct |
| PlaceHolder | 1 | Different placeholder text; no wrapper element — correct |

**Recommendation:** Align Blazor sample content with WebForms samples. This would eliminate these 22 false-positive divergences and make the diff report much more actionable. This was also recommended in M11 §7.1 but has not been actioned.

---

## 8. Pipeline Issues

### 8.1 HyperLink Case-Sensitivity Duplication

The WebForms capture pipeline stores HyperLink files under a folder named `Hyperlink` (lowercase L), while the Blazor side uses `HyperLink` (uppercase L). The diff script treats these as separate controls, producing **4 duplicate entries** (inflating the total from 82 unique to 86 reported).

**Fix:** Normalize folder names to consistent casing in the capture pipeline, or add case-insensitive matching to the diff script. This was flagged in M11 §7.4 and remains unfixed.

### 8.2 No New Blazor Markers Since M11

Despite M11 recommending that `data-audit-control` markers be added to 5 Blazor controls (CheckBoxList, ImageButton, ListBox, RadioButton, TextBox), none were added. This keeps 21 Tier 1 comparisons blocked and should be prioritized for M13.

---

## 9. Recommendations for M13

### 9.1 Fix Genuine Bugs (Priority Order)

| Priority | Action | Effort | Impact |
|----------|--------|--------|--------|
| **P1** | Fix Button: render `<input type="submit">` | 3–5 hrs | Unblocks all Button CSS migrations |
| **P1** | Fix BulletedList: `<ol>` for numbered, remove `<span>`, fix `list-style-type` | 1–2 hrs | Fixes 3 variants at once |
| **P2** | Fix LinkButton: add `href`, pass `CssClass` | 1–2 hrs | Fixes 3 variants at once |
| **P2** | Fix Calendar: add styles, title, cell widths, accessibility attrs | 3–5 hrs | Fixes 7 variants at once |
| **P2** | Fix TreeView: align node layout structure | 5–8 hrs | Complex control |
| **P3** | Fix CheckBox: remove wrapping `<span>`, stable IDs | 2–3 hrs | Fixes 3 variants |
| **P3** | Fix RadioButtonList: stable IDs | 1–2 hrs | Fixes 2 variants |
| **P3** | Fix FileUpload: remove stray GUID attribute | 1 hr | Quick fix |
| **P4** | Fix Image: conditional `longdesc` | 30 min | Quick fix |

**Total estimated effort: ~20–30 hours**

### 9.2 Close Blazor Capture Gaps

1. **Add `data-audit-control` markers** to: CheckBoxList, ImageButton, ListBox, RadioButton, TextBox (21 variants unblocked)
2. **Add Blazor sample pages** with markers for: DataList, DataPager, DetailsView, FormView (if components exist)
3. **Add `data-audit-control` markers** to existing Blazor GridView, ListView, Repeater samples
4. **Create Menu sample page** with 9 variant markers (5 List-mode + 4 Table-mode for D-06 documentation)

### 9.3 Align Sample Content

Update Blazor samples to match WebForms sample data (same text, URLs, attribute values). This eliminates ~22 false-positive divergences and allows the diff report to surface only genuine bugs.

### 9.4 Fix Pipeline Bugs

1. Fix HyperLink/Hyperlink case-sensitivity duplication
2. Consider adding `data-audit-variant` attributes to distinguish which WebForms variant maps to which Blazor variant

### 9.5 Re-Run Full Audit

After fixes, re-run the complete Tier 1 + Tier 2 comparison as the M13 final audit. Target: ≥15 exact matches from controls that are currently "Sample Parity" or "fixed Genuine Bug."

---

## 10. Appendix

### 10.1 Raw Data Files

| Path | Contents |
|------|----------|
| `audit-output/normalized/webforms/` | 83 normalized WebForms HTML files across 31 controls |
| `audit-output/normalized/blazor/` | 45 normalized Blazor HTML files across 18 controls |
| `audit-output/diff-report.md` | Full comparison report (86 entries, 564 lines) |

### 10.2 New Tier 2 Captures (M12 Additions)

| Control | WF File | Description |
|---------|---------|-------------|
| DataList | `normalized/webforms/DataList/DataList.html` | `<table>`-based repeating data layout |
| DataPager | `normalized/webforms/DataPager/DataPager.html` | Paging navigation for data controls |
| DetailsView | `normalized/webforms/DetailsView/DetailsView.html` | Single-record detail view |
| FormView | `normalized/webforms/FormView/FormView.html` | Template-based single-record form |
| GridView | `normalized/webforms/GridView/GridView.html` | Tabular data grid |
| ListView | `normalized/webforms/ListView/ListView.html` | Template-based list rendering |
| Menu-1–9 | `normalized/webforms/Menu/Menu-{1..9}.html` | 9 Menu configuration variants |
| Repeater | `normalized/webforms/Repeater/Repeater.html` | Template-based data repeater |

### 10.3 Reference Documents

| Document | Path |
|----------|------|
| M11 Audit Report | `planning-docs/AUDIT-REPORT-M11.md` |
| Divergence Registry | `planning-docs/DIVERGENCE-REGISTRY.md` |

### 10.4 Classification Decision Log

| Entry | Classification | Rationale |
|-------|---------------|-----------|
| DropDownList `selected=""` vs `selected="selected"` | Sample Parity | Both are valid HTML5 boolean attribute forms; functionally equivalent |
| Label-3 `<label>` vs `<span>` | Sample Parity | Blazor correctly renders `<label>` when `AssociatedControlID` is set; WF sample tests a different feature |
| Calendar `cursor:pointer` vs `__doPostBack` | Component of D-02/D-08 | Postback mechanism intentionally divergent; but missing styles are Genuine Bug |
| CheckBox GUID IDs | Genuine Bug (not D-01) | D-01 covers `ctl00_` prefix mangling; GUIDs are a separate issue — non-deterministic, non-addressable |
| Menu Table-mode variants | Missing Blazor Capture (D-06 noted) | Cannot classify as D-06 until Blazor captures exist; currently no comparison possible |
| Image `longdesc=""` | Genuine Bug | WF omits attribute when not set; Blazor should do the same |

---

*— Forge, Lead / Web Forms Reviewer*
*Milestone 12 Audit Complete*

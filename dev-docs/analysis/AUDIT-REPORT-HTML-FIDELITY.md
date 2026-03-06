# BlazorWebFormsComponents HTML Fidelity Audit Report

**Created:** 2026-02-28
**Author:** Forge (Lead / Web Forms Reviewer)
**Milestones:** M11 (Tier 1), M12 (Tier 2), M13 (Tier 3 + Remaining)
**Status:** Final

---

## Executive Summary

The HTML fidelity audit compared rendered HTML output from ASP.NET Web Forms controls (.NET Framework 4.8, IIS Express) against their Blazor equivalents in the BlazorWebFormsComponents library across three milestone waves.

| Metric | Value |
|--------|-------|
| Total unique controls audited | 46 (WebForms), 20 (Blazor with captures) |
| Total comparisons generated | 132 (128 unique + 4 HyperLink case-sensitivity duplicates) |
| Exact HTML matches | 0 |
| Divergent entries (both sides captured) | 49 unique |
| Missing Blazor captures (WF only) | 75 |
| Genuine structural bugs | **9 distinct bugs across 10 controls (23 variant entries)** |
| Sample parity issues | ~22 unique entries |
| Intentional divergences (D-01–D-10) | Overlapping with above; 10 registered |
| New data control divergences (need investigation) | 4 controls (DataList, GridView, ListView, Repeater) |

### Key Takeaways

1. **No Blazor component achieved an exact HTML match** against its WebForms gold standard. However, 7 controls are structurally near-matches with divergences limited to sample content differences.
2. **9 genuine structural bugs** were identified — the most impactful being Button (`<button>` vs `<input type="submit">`), BulletedList (`<ul>` vs `<ol>`), and LinkButton (missing `href`).
3. **75 of 128 unique comparisons (59%)** are blocked by missing Blazor `data-audit-control` markers or missing Blazor sample pages. This is the largest gap in audit coverage.
4. **4 Tier 2 data controls** (DataList, GridView, ListView, Repeater) now have both WF and Blazor captures showing significant structural divergences (33–182 line differences) that require detailed investigation.
5. **No bugs identified in M11 have been fixed** through M12 or M13. All 9 genuine bugs remain open.

---

## Methodology

### Audit Pipeline

1. **Web Forms Gold Standard:** The `BeforeWebForms` sample application served via IIS Express on .NET Framework 4.8. A setup script converted `CodeBehind=` to `CodeFile=` for dynamic compilation and restored NuGet packages.

2. **Blazor Comparison:** The `AfterBlazorServerSide` sample application run as a Blazor Server app.

3. **Playwright Capture:** A Node.js/Playwright script navigated to each sample page, extracted HTML between `data-audit-control="{ControlName}"` marker elements, and saved raw HTML + screenshots to `audit-output/webforms/` and `audit-output/blazor/`.

4. **Normalization Pipeline (19 rules):**
   - Strip `ctl00_`/`ctl00$` ID prefixes and naming container mangling (D-01)
   - Replace `href="javascript:__doPostBack(...)"` with `href="[postback]"` (D-02)
   - Remove `__VIEWSTATE`, `__EVENTVALIDATION`, `__EVENTTARGET` hidden fields (D-03)
   - Remove `WebResource.axd` script/link includes (D-04)
   - Strip TreeView page-level JavaScript and `onclick` handlers (D-07)
   - Strip validator evaluation attributes (D-10)
   - Normalize all `on*` event handler attributes (M12 rule)
   - Normalize form `action` attributes (M12 rule)
   - Normalize table legacy attributes — `cellspacing`, etc. (M12 rule)
   - Normalize whitespace and formatting

5. **Diff Generation:** Normalized HTML compared file-by-file, producing `audit-output/diff-report.md` with 132 entries.

### Three Milestone Waves

| Wave | Milestone | Focus | WF Pages | WF Variants | Blazor Variants |
|------|-----------|-------|----------|-------------|-----------------|
| 1 | M11 | Tier 1 (simple controls) | 24 | 66 | 45 |
| 2 | M12 | Tier 2 (data controls + Menu) | 31 | 83 | 45 |
| 3 | M13 | Tier 3 + remaining (Login, Validators, misc) | 46 | 128 | 49 |

### Classification Approach

Each divergence was classified against:
- The Intentional Divergence Registry (`DIVERGENCE-REGISTRY.md`, D-01 through D-10)
- Sample content differences (same structure, different data)
- Blazor capture availability
- Structural HTML analysis (tag names, nesting, attributes)

---

## Findings by Category

### Genuine Structural Bugs (Priority Fix List)

These are confirmed HTML structure differences where the Blazor component renders different elements, missing attributes, or incorrect nesting compared to the WebForms gold standard. Ranked by migration impact.

#### P1 — Critical (Wrong HTML Tag)

| # | Control | Variants | Bug Description | Severity |
|---|---------|----------|----------------|----------|
| 1 | **Button** | 1 | Blazor renders `<button type="submit">` instead of `<input type="submit" value="...">`. CSS selectors targeting `input[type=submit]` break; `element.value` behavior changes. | 🔴 Critical |
| 2 | **BulletedList** | 3 | Three bugs: (a) Numbered lists render `<ul>` instead of `<ol>`; (b) Item text wrapped in unnecessary `<span>`; (c) Wrong `list-style-type` values (`disc`/`circle` instead of `decimal`/`square`). | 🔴 Critical |

**Button — WebForms vs Blazor:**
```html
<!-- WebForms -->
<input type="submit" value="Blue Button" style="color:white; background-color:blue;" />
<!-- Blazor -->
<button type="submit" title="Click to submit" accesskey="b">Click me!</button>
```

**BulletedList — WebForms vs Blazor (numbered variant):**
```html
<!-- WebForms -->
<ol style="list-style-type:decimal;"><li>First</li><li>Second</li></ol>
<!-- Blazor -->
<ul style="list-style-type: disc;"><li><span>Item One</span></li><li><span>Item Two</span></li></ul>
```

#### P2 — High (Missing Attributes Affecting Layout/Interaction)

| # | Control | Variants | Bug Description | Severity |
|---|---------|----------|----------------|----------|
| 3 | **LinkButton** | 3 | Missing `href` attribute (not keyboard-navigable, no link styling); missing `CssClass` pass-through to `class` attribute. | 🔴 High |
| 4 | **Calendar** | 7 | Missing `style` (border), `title` attribute, `<tbody>` wrapper, `width:14%` on `<td>` cells, day `title` attributes, full `abbr` day names, navigation sub-table structure. | 🔴 High |
| 5 | **TreeView** | 1 | Structural node layout differences: WebForms uses `<div><table><tr><td>` hierarchy for each node; Blazor uses simplified structure. 37 line differences beyond D-07 JS normalization. | 🔴 High |

**LinkButton — WebForms vs Blazor:**
```html
<!-- WebForms -->
<a class="btn btn-primary" href="[postback]">Click Me</a>
<!-- Blazor -->
<a>LinkButton1 with Command</a>
```

#### P3 — Medium (Structural Wrappers / ID Format)

| # | Control | Variants | Bug Description | Severity |
|---|---------|----------|----------------|----------|
| 6 | **CheckBox** | 3 | Extra wrapping `<span>` around checkbox+label pair. GUID-based IDs instead of stable developer-meaningful IDs. | 🟡 Medium |
| 7 | **RadioButtonList** | 2 | GUID-based IDs/names (e.g., `5f017a2765e3...`) instead of developer-provided ID with `_0`, `_1` suffix convention. | 🟡 Medium |
| 8 | **FileUpload** | 1 | GUID fragment leaks as stray HTML attribute (`d71-4b24-8b1d-9049a956fd3b=""`), likely from CSS isolation scope. | 🟡 Medium |

#### P4 — Low (Cosmetic)

| # | Control | Variants | Bug Description | Severity |
|---|---------|----------|----------------|----------|
| 9 | **Image** | 2 | Emits empty `longdesc=""` attribute when `DescriptionUrl` is not set. WebForms omits the attribute entirely. | 🟢 Low |

#### New — Tier 2 Data Controls (Require Investigation)

These controls now have both WebForms and Blazor captures, showing significant structural divergences. The scale of differences suggests these are substantial and need detailed analysis.

| # | Control | Line Differences | Assessment |
|---|---------|-----------------|------------|
| 10 | **DataList** | 110 lines | Large structural divergence — `<table>`-based layout may differ significantly |
| 11 | **GridView** | 33 lines | Moderate divergence — tabular data grid structure needs review |
| 12 | **ListView** | 182 lines | Largest divergence — template-based rendering likely produces very different HTML |
| 13 | **Repeater** | 64 lines | Significant divergence — data repeater structure and template output differ |

> **Action needed:** These 4 data controls require line-by-line analysis to separate genuine bugs from sample parity and intentional divergences (D-01 ID mangling, D-02 postback links). This analysis should be a priority follow-up task.

#### Fix Effort Estimate

| Priority | Count | Controls | Estimated Effort |
|----------|-------|----------|-----------------|
| P1 Critical | 2 bugs | Button, BulletedList | 4–7 hours |
| P2 High | 3 bugs | LinkButton, Calendar, TreeView | 9–15 hours |
| P3 Medium | 3 bugs | CheckBox, RadioButtonList, FileUpload | 4–6 hours |
| P4 Low | 1 bug | Image | 30 min |
| **Investigation** | 4 controls | DataList, GridView, ListView, Repeater | 4–8 hours |
| **Total** | **13** | | **~22–37 hours** |

---

### Sample Parity Issues

These 22 unique entries (26 including 4 HyperLink case-sensitivity duplicates) show divergences caused entirely by different sample content between WebForms and Blazor. The HTML tag structure is correct — only the data values differ. These are **not component bugs**.

| Control | Variants | Nature of Difference |
|---------|----------|---------------------|
| AdRotator | 1 | Different ad content (Bing vs Microsoft), different image paths |
| DropDownList | 6 | Different item text/values; `selected=""` vs `selected="selected"` (functionally equivalent HTML5 forms); missing `class`/`disabled` from different sample configurations |
| HiddenField | 1 | Different `value` attribute content |
| HyperLink | 4 (×2 dupes) | Different URLs, link text, and attribute combinations; `<a>` structure correct |
| Image | 2 | Different `src`/`alt` (also has genuine `longdesc` bug — see P4) |
| ImageMap | 1 | Different coordinates, URLs, area definitions; `<img usemap>` + `<map>/<area>` correct |
| Label | 3 | Different text, classes; variant 3 correctly uses `<label>` for `AssociatedControlID` |
| Literal | 2 | Different text content; raw HTML pass-through working correctly |
| Panel | 3 | Completely different inner content; `<div>` wrapper structure correct |
| PlaceHolder | 1 | Different placeholder text; no wrapper element — correct |

**Recommendation:** Align Blazor sample pages to use identical content as WebForms samples. This eliminates these ~22 false-positive divergences and allows the diff to surface only genuine structural bugs. This was recommended in M11 and M12 but has not been actioned.

---

### Missing Blazor Captures

75 variant entries (59% of all comparisons) have WebForms gold-standard HTML captured but no corresponding Blazor capture. This is the audit's largest coverage gap.

#### Tier 1 — Simple Controls (21 variants, carried from M11)

| Control | Missing Variants | Blazor Component Exists? |
|---------|-----------------|-------------------------|
| Button | 4 (Button-2–5) | ✅ Yes — needs additional `data-audit-control` markers |
| CheckBoxList | 2 | ✅ Yes — needs `data-audit-control` markers |
| ImageButton | 2 | ✅ Yes — needs `data-audit-control` markers |
| ListBox | 2 | ✅ Yes — needs `data-audit-control` markers |
| Literal | 1 (Literal-3) | ✅ Yes — needs marker for 3rd variant |
| RadioButton | 3 | ✅ Yes — needs `data-audit-control` markers |
| TextBox | 7 | ✅ Yes — needs `data-audit-control` markers |

#### Tier 2 — Data Controls (3 variants)

| Control | Missing Variants | Notes |
|---------|-----------------|-------|
| DataPager | 1 | WF gold-standard captured; Blazor sample needs markers |
| DetailsView | 1 | WF gold-standard captured; check if Blazor component exists |
| FormView | 1 | WF gold-standard captured; check if Blazor component exists |

#### Tier 3 — Menu (9 variants)

All 9 Menu variants are missing Blazor captures. WebForms gold-standard HTML exists for all.

| Variants | Mode | D-06 Applies? |
|----------|------|--------------|
| Menu-1, -2, -5, -6, -9 | List mode | No — should be comparable |
| Menu-3, -4, -7, -8 | Table mode | **Yes** — Blazor only implements List mode (D-06) |

#### Tier 3/4 — Newly Captured Controls (42 variants)

These controls had WebForms gold-standard HTML captured for the first time in M13:

| Control | Variants | Category | Notes |
|---------|----------|----------|-------|
| ChangePassword | 2 | Login family (D-09) | Visual shell — auth infrastructure intentionally different |
| CompareValidator | 4 | Validators (D-10) | Validator JS stripped; compare `<span>` output only |
| CreateUserWizard | 2 | Login family (D-09) | Visual shell |
| CustomValidator | 3 | Validators (D-10) | Validator JS stripped |
| Login | 2 | Login family (D-09) | Visual shell |
| LoginName | 2 | Login family (D-09) | Visual shell |
| LoginStatus | 2 | Login family (D-09) | Visual shell |
| LoginView | 1 | Login family (D-09) | Visual shell |
| MultiView | 1 | Infrastructure | Container/view switching control |
| PasswordRecovery | 2 | Login family (D-09) | Visual shell |
| RangeValidator | 4 | Validators (D-10) | Validator JS stripped |
| RegularExpressionValidator | 4 | Validators (D-10) | Validator JS stripped |
| RequiredFieldValidator | 4 | Validators (D-10) | Validator JS stripped |
| SiteMapPath | 2 | Navigation | Breadcrumb-style navigation |
| Table | 3 | Layout | `<table>` structure control |
| ValidationSummary | 4 | Validators (D-10) | Validator JS stripped |

---

### Intentional Divergences (D-01 through D-10)

All 10 registered intentional divergences are documented in `planning-docs/DIVERGENCE-REGISTRY.md`. These are architectural differences between WebForms and Blazor that exist by design and should not be treated as bugs.

| D-Code | Divergence | Controls Affected | Observed in Audit |
|--------|-----------|-------------------|-------------------|
| **D-01** | ID mangling (`ctl00_MainContent_` prefix) | ALL controls | ✅ All 49 divergent entries |
| **D-02** | PostBack links (`__doPostBack` → `@onclick`) | GridView, Calendar, DetailsView, FormView, Menu, TreeView | ✅ Calendar (7), LinkButton, GridView, data controls |
| **D-03** | ViewState hidden fields | Page-level | ✅ Stripped by normalization |
| **D-04** | WebResource.axd URLs | TreeView, Menu | ✅ Stripped by normalization |
| **D-05** | Chart rendering technology (`<img>` vs `<canvas>`) | Chart | ⛔ Excluded from audit entirely |
| **D-06** | Menu RenderingMode=Table | Menu | ⏳ 4 Table-mode variants captured, awaiting Blazor comparison |
| **D-07** | TreeView JavaScript | TreeView | ✅ JS stripped; structural differences remain (genuine bug) |
| **D-08** | Calendar day selection mechanism | Calendar | ✅ Postback → `cursor:pointer` normalized |
| **D-09** | Login control infrastructure | Login, LoginName, LoginStatus, LoginView, ChangePassword, CreateUserWizard, PasswordRecovery | ⏳ WF captured, Blazor captures needed |
| **D-10** | Validator client-side scripts | All validators, ValidationSummary | ⏳ WF captured, Blazor captures needed |

**Note on GUID IDs:** CheckBox, RadioButton, RadioButtonList, and FileUpload generate GUID-based IDs instead of stable developer-meaningful IDs. This is a **separate concern from D-01** (which covers `ctl00_` prefix mangling). GUIDs make HTML non-deterministic and untargetable. This warrants either a new D-11 entry (if intentional) or a bug fix (recommended).

---

## Tier 3 JavaScript Strategy

Per `planning-docs/TIER3-JS-STRATEGY.md`, the existing normalization pipeline is **sufficient** for all Tier 3 controls. No new JS extraction tooling was needed.

| Control | JS Pattern | Normalization Coverage |
|---------|-----------|----------------------|
| Menu | `WebResource.axd` popups, `__doPostBack` | D-02, D-04, M12 `on*` rule |
| TreeView | `TreeView_ToggleNode`, data arrays, `onclick` | D-07, D-04, M12 `on*` rule |
| Calendar | `__doPostBack` for day/week/month selection | D-02, D-08, M12 `on*` rule |

---

## Control-by-Control Status

### Tier 1 — Simple Controls

| # | Control | Tier | WF Captured | WF Variants | Blazor Captured | Compared | Status | Notes |
|---|---------|------|-------------|-------------|-----------------|----------|--------|-------|
| 1 | AdRotator | 1 | ✅ | 1 | ✅ | 1 | Sample Parity | `<a><img>` structure correct |
| 2 | BulletedList | 1 | ✅ | 3 | ✅ | 3 | **🔴 Genuine Bug** | `<ul>`/`<ol>`, `<span>`, `list-style-type` |
| 3 | Button | 1 | ✅ | 5 | ✅ (1) | 1 + 4 missing | **🔴 Genuine Bug** | `<button>` vs `<input type="submit">` |
| 4 | CheckBox | 1 | ✅ | 3 | ✅ | 3 | **🟡 Genuine Bug** | Extra `<span>`, GUID IDs |
| 5 | CheckBoxList | 1 | ✅ | 2 | ❌ | 0 | Missing Blazor | Needs `data-audit-control` markers |
| 6 | DropDownList | 1 | ✅ | 6 | ✅ | 6 | Sample Parity | Structure correct; content differs |
| 7 | FileUpload | 1 | ✅ | 1 | ✅ | 1 | **🟡 Genuine Bug** | GUID attribute leak |
| 8 | HiddenField | 1 | ✅ | 1 | ✅ | 1 | Sample Parity | Near match; value differs |
| 9 | HyperLink | 1 | ✅ | 4 | ✅ | 4 (×2 dupes) | Sample Parity | `<a>` correct; URLs/text differ |
| 10 | Image | 1 | ✅ | 2 | ✅ | 2 | **🟢 Genuine Bug** + Parity | Empty `longdesc=""`; different src/alt |
| 11 | ImageButton | 1 | ✅ | 2 | ❌ | 0 | Missing Blazor | Needs `data-audit-control` markers |
| 12 | ImageMap | 1 | ✅ | 1 | ✅ | 1 | Sample Parity | Structure correct; coords differ |
| 13 | Label | 1 | ✅ | 3 | ✅ | 3 | Sample Parity | Variants test different features |
| 14 | LinkButton | 1 | ✅ | 3 | ✅ | 3 | **🔴 Genuine Bug** | Missing `href`, `CssClass` |
| 15 | ListBox | 1 | ✅ | 2 | ❌ | 0 | Missing Blazor | Needs `data-audit-control` markers |
| 16 | Literal | 1 | ✅ | 3 | ✅ (2) | 2 + 1 missing | Sample Parity | Raw HTML pass-through correct |
| 17 | Panel | 1 | ✅ | 3 | ✅ | 3 | Sample Parity | `<div>` wrapper correct |
| 18 | PlaceHolder | 1 | ✅ | 1 | ✅ | 1 | Sample Parity | No wrapper — correct |
| 19 | RadioButton | 1 | ✅ | 3 | ❌ | 0 | Missing Blazor | Needs `data-audit-control` markers |
| 20 | RadioButtonList | 1 | ✅ | 2 | ✅ | 2 | **🟡 Genuine Bug** | GUID-based IDs/names |
| 21 | TextBox | 1 | ✅ | 7 | ❌ | 0 | Missing Blazor | Needs `data-audit-control` markers |

### Tier 2 — Data Controls

| # | Control | Tier | WF Captured | WF Variants | Blazor Captured | Compared | Status | Notes |
|---|---------|------|-------------|-------------|-----------------|----------|--------|-------|
| 22 | DataList | 2 | ✅ | 1 | ✅ | 1 | **⚠️ Investigate** | 110 line differences |
| 23 | DataPager | 2 | ✅ | 1 | ❌ | 0 | Missing Blazor | WF gold-standard captured |
| 24 | DetailsView | 2 | ✅ | 1 | ❌ | 0 | Missing Blazor | WF gold-standard captured |
| 25 | FormView | 2 | ✅ | 1 | ❌ | 0 | Missing Blazor | WF gold-standard captured |
| 26 | GridView | 2 | ✅ | 1 | ✅ | 1 | **⚠️ Investigate** | 33 line differences |
| 27 | ListView | 2 | ✅ | 1 | ✅ | 1 | **⚠️ Investigate** | 182 line differences |
| 28 | Repeater | 2 | ✅ | 1 | ✅ | 1 | **⚠️ Investigate** | 64 line differences |

### Tier 3 — Complex Controls

| # | Control | Tier | WF Captured | WF Variants | Blazor Captured | Compared | Status | Notes |
|---|---------|------|-------------|-------------|-----------------|----------|--------|-------|
| 29 | Calendar | 3 | ✅ | 7 | ✅ | 7 | **🔴 Genuine Bug** | Missing styles, attrs, nav structure |
| 30 | Menu | 3 | ✅ | 9 | ❌ | 0 | Missing Blazor | 5 List-mode + 4 Table-mode (D-06) |
| 31 | TreeView | 3 | ✅ | 1 | ✅ | 1 | **🔴 Genuine Bug** | Structural node layout differs |
| 32 | SiteMapPath | 3 | ✅ | 2 | ❌ | 0 | Missing Blazor | Breadcrumb navigation |

### Tier 4 — Special Controls

| # | Control | Tier | WF Captured | WF Variants | Blazor Captured | Compared | Status | Notes |
|---|---------|------|-------------|-------------|-----------------|----------|--------|-------|
| 33 | Chart | 4 | ❌ | 0 | ✅ | — | Excluded (D-05) | Permanent arch divergence |
| 34 | MultiView | 4 | ✅ | 1 | ❌ | 0 | Missing Blazor | View switching container |
| 35 | Table | 4 | ✅ | 3 | ❌ | 0 | Missing Blazor | `<table>` structure control |

### Login Family (D-09)

| # | Control | WF Captured | WF Variants | Blazor Captured | Status |
|---|---------|-------------|-------------|-----------------|--------|
| 36 | ChangePassword | ✅ | 2 | ❌ | Missing Blazor |
| 37 | CreateUserWizard | ✅ | 2 | ❌ | Missing Blazor |
| 38 | Login | ✅ | 2 | ❌ | Missing Blazor |
| 39 | LoginName | ✅ | 2 | ❌ | Missing Blazor |
| 40 | LoginStatus | ✅ | 2 | ❌ | Missing Blazor |
| 41 | LoginView | ✅ | 1 | ❌ | Missing Blazor |
| 42 | PasswordRecovery | ✅ | 2 | ❌ | Missing Blazor |

### Validators (D-10)

| # | Control | WF Captured | WF Variants | Blazor Captured | Status |
|---|---------|-------------|-------------|-----------------|--------|
| 43 | CompareValidator | ✅ | 4 | ❌ | Missing Blazor |
| 44 | CustomValidator | ✅ | 3 | ❌ | Missing Blazor |
| 45 | RangeValidator | ✅ | 4 | ❌ | Missing Blazor |
| 46 | RegularExpressionValidator | ✅ | 4 | ❌ | Missing Blazor |
| 47 | RequiredFieldValidator | ✅ | 4 | ❌ | Missing Blazor |
| 48 | ValidationSummary | ✅ | 4 | ❌ | Missing Blazor |

---

## Pipeline Issues

### HyperLink Case-Sensitivity Duplication

The WebForms capture pipeline stores HyperLink under `Hyperlink` (lowercase L), while Blazor uses `HyperLink` (uppercase L). This produces 4 duplicate diff entries (inflating 128 unique to 132 reported). Flagged in M11, confirmed in M12, **still unfixed in M13**.

**Fix:** Normalize folder names to consistent casing, or add case-insensitive matching to the diff script.

### No New Blazor Markers Since M11

Despite repeated recommendations (M11 §7.1, M12 §9.2), no new `data-audit-control` markers have been added to Blazor sample pages. The 21 Tier 1 missing captures from M11 remain blocked.

---

## Recommendations

### 1. Priority Bug Fixes

| Priority | Control | Fix Description | Effort | Impact |
|----------|---------|----------------|--------|--------|
| **P1** | Button | Render `<input type="submit" value="...">` instead of `<button>` | 3–5 hrs | Unblocks all Button CSS migrations |
| **P1** | BulletedList | Render `<ol>` for numbered styles; remove `<span>` wrapper; fix `list-style-type` mapping | 1–2 hrs | Fixes 3 variants |
| **P2** | LinkButton | Add `href="#"` or `href="javascript:void(0)"`; pass `CssClass` → `class` | 1–2 hrs | Fixes 3 variants |
| **P2** | Calendar | Add `style`, `title`, `<tbody>`, `width:14%`, day `title` attrs, full `abbr` names, nav sub-table | 3–5 hrs | Fixes 7 variants |
| **P2** | TreeView | Align node rendering to `<div><table><tr><td>` structure | 5–8 hrs | Complex control |
| **P3** | CheckBox | Remove wrapping `<span>`; use stable developer-meaningful IDs | 2–3 hrs | Fixes 3 variants |
| **P3** | RadioButtonList | Use developer-provided ID for `name`/`id`; append `_0`, `_1` per WF convention | 1–2 hrs | Fixes 2 variants |
| **P3** | FileUpload | Investigate and remove stray GUID attribute leak | 1 hr | Quick fix |
| **P4** | Image | Only render `longdesc` when `DescriptionUrl` has a non-empty value | 30 min | Quick fix |

### 2. Investigate Tier 2 Data Control Divergences

DataList (110 lines), GridView (33 lines), ListView (182 lines), and Repeater (64 lines) now have both WF and Blazor captures showing significant divergences. Perform line-by-line classification to separate genuine bugs from sample parity and D-01/D-02 intentional divergences.

### 3. Close Blazor Capture Gaps (75 missing variants)

**Highest priority (Tier 1 — 21 variants):**
Add `data-audit-control` markers to existing Blazor samples for: CheckBoxList, ImageButton, ListBox, RadioButton, TextBox, Button (variants 2–5), Literal (variant 3).

**Medium priority (Tier 2/3 — 12 variants):**
Add Blazor sample pages with markers for: DataPager, DetailsView, FormView, Menu (9 variants — 5 List-mode comparable, 4 Table-mode D-06).

**Lower priority (Tier 4 + Login + Validators — 42 variants):**
Add Blazor captures for Login family (D-09), Validators (D-10), MultiView, SiteMapPath, Table. These have intentional infrastructure divergences but structural HTML should still be compared.

### 4. Align Sample Content

Update Blazor sample pages to use identical text, URLs, data values, and attribute combinations as WebForms samples. This eliminates ~22 false-positive sample parity entries and makes the diff report directly actionable.

### 5. Fix Pipeline Issues

1. Fix HyperLink/Hyperlink case-sensitivity duplication (4 phantom entries)
2. Consider adding `data-audit-variant` attributes to distinguish WF variant → Blazor variant mapping
3. Add case-insensitive folder matching to the diff script

### 6. Registry Updates

1. **Consider D-11: GUID-Based IDs** — Document or fix the Blazor pattern of generating GUID-based IDs for CheckBox, RadioButton, RadioButtonList, FileUpload
2. **Consider D-12: Boolean Attribute Format** — Document `selected=""` (HTML5) vs `selected="selected"` (XHTML) as a platform difference

### 7. Future Audit Enhancements

1. **Re-run complete audit** after bug fixes — target ≥15 exact matches from near-match controls
2. **Automate regression** — integrate diff report into CI pipeline to catch regressions
3. **Add visual regression** — screenshot comparison in addition to HTML structure comparison

---

## Appendix

### A. Reference Documents

| Document | Path |
|----------|------|
| M11 Audit Report (Tier 1) | `planning-docs/AUDIT-REPORT-M11.md` |
| M12 Audit Report (Tier 2) | `planning-docs/AUDIT-REPORT-M12.md` |
| Intentional Divergence Registry | `planning-docs/DIVERGENCE-REGISTRY.md` |
| Tier 3 JS Strategy | `planning-docs/TIER3-JS-STRATEGY.md` |
| Diff Report (132 comparisons) | `audit-output/diff-report.md` |

### B. Raw Data Locations

| Path | Contents |
|------|----------|
| `audit-output/webforms/{Control}/` | Raw WebForms HTML + screenshots |
| `audit-output/blazor/{Control}/` | Raw Blazor HTML + screenshots |
| `audit-output/normalized/webforms/` | Normalized WebForms HTML (128 files across 46 controls) |
| `audit-output/normalized/blazor/` | Normalized Blazor HTML (49 files across 20 controls) |
| `audit-output/diff-report.md` | Full comparison report (132 entries) |

### C. Classification Summary

| Classification | Unique Entries | % of 128 |
|---------------|---------------|----------|
| Genuine structural bug | 23 (9 bugs, 10 controls) | 18% |
| Sample parity (false positive) | ~22 | 17% |
| Data control divergence (needs investigation) | 4 | 3% |
| Missing Blazor capture | 75 | 59% |
| Pipeline duplicates (HyperLink) | 4 | 3% |

### D. Milestone Progress

| Milestone | Deliverable | Status |
|-----------|------------|--------|
| M11 | Tier 1 capture, normalize, diff, report | ✅ Complete |
| M12 | Tier 2 data control captures, expanded report | ✅ Complete |
| M13 | Tier 3 + remaining captures, JS strategy, final report | ✅ Complete |
| Next | Bug fixes, sample alignment, re-audit | ⏳ Pending |

---

*— Forge, Lead / Web Forms Reviewer*
*Final Comprehensive Audit — M11 + M12 + M13 Complete*

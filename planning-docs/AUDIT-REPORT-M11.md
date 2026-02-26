# Tier 1 HTML Audit Findings Report — Milestone 11

**Created:** 2026-02-26
**Author:** Forge (Lead / Web Forms Reviewer)
**Milestone:** M11-09
**Status:** Complete
**Branch:** `milestone11/html-audit-tier1`

---

## 1. Executive Summary

The Milestone 11 HTML fidelity audit compared rendered HTML output from 30 ASP.NET Web Forms controls (served via IIS Express on .NET Framework 4.8) against their Blazor equivalents in the BlazorWebFormsComponents library. Of the 30 controls attempted, 24 Web Forms pages succeeded (7 returned HTTP 500 due to missing data sources), yielding 66 normalized Web Forms control variants. On the Blazor side, 18 pages with `data-audit-control` markers produced 45 normalized control variants. The comparison pipeline generated 70 diff entries, with **zero exact matches**. However, the zero-match headline is misleading: a significant portion of the divergences are **sample parity issues** (the Web Forms and Blazor samples exercise different content/values, not different HTML structures) or **intentional platform divergences** (D-01 through D-10). After classification, approximately 8–10 controls exhibit genuine structural HTML bugs that should be fixed, while the remainder are either intentional, sample-driven, or require Blazor capture pages to be written before meaningful comparison is possible.

---

## 2. Methodology

### 2.1 Capture Infrastructure

1. **Web Forms Gold Standard:** The `BeforeWebForms` sample application was served via IIS Express on .NET Framework 4.8. A PowerShell setup script (M11-01) converted `CodeBehind=` directives to `CodeFile=` for dynamic compilation, restored NuGet packages, and launched IIS Express.

2. **Blazor Comparison:** The `AfterBlazorServerSide` sample application was run as a Blazor Server app.

3. **Playwright Capture (M11-05):** A Node.js/Playwright script navigated to each sample page, extracted HTML between `data-audit-control="{ControlName}"` marker elements, and saved:
   - Raw HTML to `audit-output/webforms/{Control}/` and `audit-output/blazor/{Control}/`
   - Screenshots alongside the HTML files

4. **Normalization Pipeline (M11-06):** Both HTML captures were normalized before comparison:
   - Stripped `ctl00_`/`ctl00$` ID prefixes and naming container mangling (D-01)
   - Replaced `href="javascript:__doPostBack(...)"` with `href="[postback]"` (D-02)
   - Removed `__VIEWSTATE`, `__EVENTVALIDATION`, `__EVENTTARGET` hidden fields (D-03)
   - Removed `WebResource.axd` script/link includes (D-04)
   - Stripped TreeView page-level JavaScript and `onclick` TreeView handlers (D-07)
   - Stripped validator evaluation attributes (D-10)
   - Normalized whitespace

5. **Diff Generation (M11-08):** Normalized HTML files were compared file-by-file, producing `audit-output/diff-report.md` with 70 entries.

### 2.2 Classification Approach

Each divergence was classified against the Intentional Divergence Registry (`DIVERGENCE-REGISTRY.md`, D-01 through D-10) and further categorized as:

- **Sample parity issue** — The WebForms and Blazor samples test different content, values, or features
- **Tag structure divergence** — Blazor uses fundamentally different HTML elements
- **Missing attribute** — Blazor omits attributes that WebForms emits
- **Intentional divergence** — Covered by a D-XX registry entry
- **Genuine bug** — Structural difference not explained by any of the above

---

## 3. Coverage Summary

### 3.1 Capture Results

| Metric | Count |
|--------|-------|
| Controls attempted (WebForms pages) | 30 |
| WebForms pages succeeded | 24 |
| WebForms pages failed (HTTP 500) | 6 |
| WebForms control variants captured | 66 |
| Blazor pages with markers | 18 |
| Blazor control variants captured | 45 |
| Diff entries generated | 70 |
| Exact matches | 0 |
| Divergent entries | 49 |
| Missing in Blazor (no capture) | 21 |

### 3.2 Per-Control Status

| # | Control | Tier | WF Status | WF Variants | Blazor Status | Blazor Variants | Compared | Verdict |
|---|---------|------|-----------|-------------|---------------|-----------------|----------|---------|
| 1 | AdRotator | 1 | ✅ Captured | 1 | ✅ Captured | 1 | 1 divergent | Sample parity |
| 2 | BulletedList | 1 | ✅ Captured | 3 | ✅ Captured | 3 | 3 divergent | **Tag structure bug** |
| 3 | Button | 1 | ✅ Captured | 5 | ✅ Captured | 1 | 1 divergent, 4 missing | **Tag structure bug** |
| 4 | Calendar | 3 | ✅ Captured | 7 | ✅ Captured | 7 | 7 divergent | **Missing attributes** |
| 5 | Chart | 4 | ❌ HTTP 500 | 0 | ✅ Captured | 1 | — | Excluded (D-05) |
| 6 | CheckBox | 1 | ✅ Captured | 3 | ✅ Captured | 3 | 3 divergent | **Wrapping + sample parity** |
| 7 | CheckBoxList | 1 | ✅ Captured | 2 | ❌ No markers | 0 | 2 missing | Blazor capture needed |
| 8 | DataList | 2 | ❌ HTTP 500 | 0 | ✅ Captured | 1 | — | WF needs data source |
| 9 | DropDownList | 1 | ✅ Captured | 6 | ✅ Captured | 6 | 6 divergent | Sample parity + missing attrs |
| 10 | FileUpload | 1 | ✅ Captured | 1 | ✅ Captured | 1 | 1 divergent | **GUID attribute bug** |
| 11 | FormView | 2 | ❌ HTTP 500 | 0 | ❌ No captures | 0 | — | WF needs data source |
| 12 | GridView | 2 | ❌ HTTP 500 | 0 | ✅ Captured | 1 | — | WF needs data source |
| 13 | HiddenField | 1 | ✅ Captured | 1 | ✅ Captured | 1 | 1 divergent | Near match (sample parity) |
| 14 | HyperLink | 1 | ✅ Captured | 4 | ✅ Captured | 4 | 4 divergent | Sample parity |
| 15 | Image | 1 | ✅ Captured | 2 | ✅ Captured | 2 | 2 divergent | Sample parity + extra attr |
| 16 | ImageButton | 1 | ✅ Captured | 2 | ❌ No markers | 0 | 2 missing | Blazor capture needed |
| 17 | ImageMap | 1 | ✅ Captured | 1 | ✅ Captured | 1 | 1 divergent | Sample parity |
| 18 | Label | 1 | ✅ Captured | 3 | ✅ Captured | 3 | 3 divergent | **Tag bug (Label→label)** |
| 19 | LinkButton | 1 | ✅ Captured | 3 | ✅ Captured | 3 | 3 divergent | **Missing href/class** |
| 20 | ListBox | 1 | ✅ Captured | 2 | ❌ No markers | 0 | 2 missing | Blazor capture needed |
| 21 | ListView | 2 | ❌ HTTP 500 | 0 | ✅ Captured | 1 | — | WF needs data source |
| 22 | Literal | 1 | ✅ Captured | 3 | ✅ Captured | 2 | 2 divergent, 1 missing | Near match (sample parity) |
| 23 | Menu | 3 | ❌ HTTP 500 | 0 | ✅ Captured | 1 | — | WF needs data source |
| 24 | Panel | 1 | ✅ Captured | 3 | ✅ Captured | 3 | 3 divergent | Sample parity |
| 25 | PlaceHolder | 1 | ✅ Captured | 1 | ✅ Captured | 1 | 1 divergent | Near match (sample parity) |
| 26 | RadioButton | 1 | ✅ Captured | 3 | ❌ No markers | 0 | 3 missing | Blazor capture needed |
| 27 | RadioButtonList | 1 | ✅ Captured | 2 | ✅ Captured | 2 | 2 divergent | **GUID IDs** + sample parity |
| 28 | Repeater | 2 | ❌ HTTP 500 | 0 | ✅ Captured | 1 | — | WF needs data source |
| 29 | TextBox | 1 | ✅ Captured | 7 | ❌ No markers | 0 | 7 missing | Blazor capture needed |
| 30 | TreeView | 3 | ✅ Captured | 1 | ✅ Captured | 1 | 1 divergent | **Structural differences** |

> **Note:** HyperLink appears twice in the diff report (as "Hyperlink" and "HyperLink") due to case-sensitivity in folder naming, inflating the total to 70 entries. The actual unique control count is 23 compared + 7 failed = 30.

---

## 4. Findings by Category

### 4.1 Exact or Near Matches

No controls achieved an exact HTML match. However, several controls are **structurally very close** and diverge only because the sample pages test different content (values, text, URLs). After accounting for sample parity differences and intentional D-01 (ID mangling), these controls are near-matches:

| Control | Assessment | Remaining Gap |
|---------|-----------|---------------|
| **HiddenField** | Near match | `<input type="hidden">` structure identical; only `value` and `id` differ (sample parity + D-01) |
| **PlaceHolder** | Near match | No wrapper element in either version; only inner text content differs (sample parity) |
| **Literal** | Near match | Raw text/HTML pass-through identical in concept; content differs (sample parity) |
| **DropDownList** | Structurally close | `<select>/<option>` structure matches; differences are all sample content, plus missing `id` (D-01) and formatting of `selected` attribute (`selected=""` vs `selected="selected"`) |
| **Image** | Structurally close | `<img>` tag structure matches; Blazor adds empty `longdesc=""` attribute when not set; different `src`/`alt` (sample parity) |
| **Panel** | Structurally close | `<div>` wrapper identical in concept; inner content differs entirely (sample parity) |
| **ImageMap** | Structurally close | `<img usemap>` + `<map>/<area>` structure matches; coordinate/href values differ (sample parity); Blazor uses GUID-based map name |

### 4.2 Tag Structure Divergences

These controls use fundamentally different HTML elements between WebForms and Blazor. These are the most impactful divergences for CSS compatibility.

#### 4.2.1 Button: `<input type="submit">` vs `<button>`

**WebForms:** `<input id="..." style="color:#ffffff; background-color:#0000ff;" type="submit" value="Blue Button" />`
**Blazor:** `<button accesskey="b" title="Click to submit" type="submit">Click me!</button>`

- WebForms renders `<input type="submit">` — a self-closing void element with the label in the `value` attribute
- Blazor renders `<button>` — a container element with the label as inner text
- **CSS Impact: HIGH** — `input[type=submit]` selectors will not match `<button>`; padding, font-size, and text styling behave differently between the two elements
- **JS Impact: MEDIUM** — `element.value` works on `<input>` but not `<button>` (use `textContent` instead)
- **Fix difficulty: MEDIUM** — Requires changing the Blazor Button component to render `<input type="submit">` instead of `<button>`, with the `Text` value in the `value` attribute

#### 4.2.2 BulletedList: Numbered Lists Render as `<ul>` Instead of `<ol>`

**WebForms (numbered):** `<ol id="..." style="list-style-type:decimal;"><li>First</li>...</ol>`
**Blazor (numbered):** `<ul style="list-style-type:disc;"><li><span>Item One</span></li>...</ul>`

Three bugs identified:
1. **`<ol>` vs `<ul>`** — When `BulletStyle` is set to a numbered style (Numbered/LowerAlpha/etc.), WebForms correctly renders `<ol>` but Blazor always renders `<ul>`
2. **Extra `<span>` wrapping** — Blazor wraps each list item's text in `<span>`, WebForms does not (except in `HyperLink` display mode)
3. **`list-style-type` mismatch** — Blazor-3 shows `circle` when WebForms-3 shows `square`

- **CSS Impact: HIGH** — `ol li` selectors won't match; counter-based styling breaks
- **Fix difficulty: LOW** — Conditional tag selection based on `BulletStyle`; remove unnecessary `<span>` wrapper

#### 4.2.3 Label (Variant 3): `<span>` vs `<label>`

**WebForms:** `<span id="..."><em>Emphasized</em></span>`
**Blazor:** `<label class="form-label" for="emailInput">Email Address:</label>`

- This is partially a sample parity issue (different content), but the Blazor sample uses `AssociatedControlID`, which correctly renders a `<label>` tag. WebForms does the same when `AssociatedControlID` is set, so the **tag difference is actually correct behavior** — the samples simply test different features.
- **Verdict: Not a bug** — but the samples should be aligned to test the same features.

#### 4.2.4 CheckBox: Extra Wrapping `<span>`

**WebForms:** `<input type="checkbox" /><label for="...">Accept Terms</label>`
**Blazor:** `<span><input type="checkbox" /><label for="...">I agree to terms</label></span>`

- Blazor wraps the checkbox + label pair in a `<span>` that WebForms does not emit
- **CSS Impact: LOW** — The extra `<span>` may affect CSS selectors using direct child combinators (`>`) but is otherwise benign
- **Fix difficulty: LOW** — Remove the wrapping `<span>` from the CheckBox component

### 4.3 Missing Attributes

These controls render the correct HTML elements but are missing attributes that WebForms emits.

#### 4.3.1 Calendar: Multiple Missing Attributes

The Calendar control has the correct `<table>` structure but is missing numerous attributes compared to WebForms:

| Missing Attribute | WebForms Value | CSS Impact |
|------------------|---------------|------------|
| `title` on `<table>` | `"Calendar"` | None (accessibility) |
| `style` on `<table>` | `border-width:1px; border-style:solid; border-collapse:collapse;` | **HIGH** — border rendering differs |
| `<tbody>` wrapper | Present in WebForms | LOW — implicit in browser rendering |
| `style="width:14%"` on `<td>` cells | Present for uniform column width | **MEDIUM** — columns may not be evenly sized |
| `title` on day `<a>` links | `"February 1"`, `"February 2"`, etc. | None (accessibility) |
| `abbr` on `<th>` | Full day name (`"Sunday"`) vs abbreviated (`"Sun"`) | None (accessibility) |
| Navigation header sub-table | WebForms nests a `<table>` in `<td colspan="7">` | **MEDIUM** — layout differs |
| `style="color:#000000"` on day links | Present in WebForms | LOW — inherits in most cases |

- **Overall CSS Impact: HIGH** — Calendar styling is likely to break without border styles and column widths
- **Fix difficulty: MEDIUM** — Many small attribute additions needed across the Calendar component

#### 4.3.2 LinkButton: Missing `href` and CSS Classes

**WebForms:** `<a class="btn btn-primary" href="[postback]" id="...">Click Me</a>`
**Blazor:** `<a>LinkButton1 with Command</a>`

- Blazor LinkButton renders without `href` attribute, making it not keyboard-navigable and not styled as a link by default
- CSS classes from the WebForms declaration are missing
- **CSS Impact: HIGH** — Link styling won't apply without `href`; class-based styling won't work
- **Fix difficulty: LOW** — Add `href="#"` (or `href="javascript:void(0)"`) and pass through `CssClass`

#### 4.3.3 DropDownList: Missing Attributes After Normalization

After removing ID mangling (D-01), DropDownList structure is close but has:
- Missing `class` attribute (variant 5: `class="form-select"` in WebForms, absent in Blazor)
- Missing `disabled="disabled"` (variant 4: WebForms has it, Blazor doesn't in that variant)
- Missing `style` attributes (variant 6: `color`, `background-color`, `width`)
- Boolean attribute format: `selected=""` (Blazor) vs `selected="selected"` (WebForms)

The `selected=""` vs `selected="selected"` difference is cosmetically different but functionally equivalent in HTML5. The other missing attributes indicate the Blazor sample doesn't test the same features.

#### 4.3.4 Image: Extra Empty `longdesc` Attribute

**Blazor adds:** `longdesc=""` when no `DescriptionUrl` is set
**WebForms:** Omits `longdesc` entirely when not set

- **CSS Impact: None**
- **Fix difficulty: LOW** — Only render `longdesc` when the property has a non-empty value

### 4.4 Sample Parity Issues

A substantial portion of the 70 divergences are **false positives** caused by the WebForms and Blazor sample pages testing entirely different content. These are not component bugs — they are audit infrastructure gaps.

| Control | Sample Parity Problem |
|---------|----------------------|
| **AdRotator** | WF: Bing ad with banner.png; Blazor: Microsoft ad with CSharp.png |
| **DropDownList** | WF: "Select.../Option One/Two/Three"; Blazor: "Widget/Gadget/Gizmo/Doohickey" — completely different data sources |
| **HiddenField** | WF: `value="secret-value-123"`; Blazor: `value="initial-secret-value"` |
| **HyperLink** | WF: Links to bing.com; Blazor: Links to github.com — different URLs, text, attributes tested |
| **Image** | WF: banner.png with "Banner image" alt; Blazor: placeholder SVG with different alt |
| **ImageMap** | WF: Links to bing.com/github.com; Blazor: Links to internal pages |
| **Label** (1,2) | WF: "Hello World" and "Styled Label"; Blazor: "Hello, World!" and "Important notice" |
| **Literal** | WF: `This is <b>literal</b> content.`; Blazor: `<b>Literal</b>` |
| **Panel** | All 3 variants have completely different inner content |
| **PlaceHolder** | Different placeholder text |

**Recommendation:** Align the Blazor sample pages to use the same content as the WebForms samples. This would eliminate ~15 false-positive divergences and allow the diff to surface actual structural bugs.

### 4.5 Intentional Divergences

The following divergences observed in the diff report are classified as intentional per the Divergence Registry:

| D-Code | Divergence | Observed In |
|--------|-----------|-------------|
| **D-01** | ID mangling (`MainContent_` prefix) | ALL controls — WebForms IDs have `MainContent_` prefix; Blazor uses direct or GUID IDs |
| **D-02** | PostBack links (`href="[postback]"` vs `cursor:pointer`) | Calendar (all 7 variants), LinkButton |
| **D-05** | Chart rendering technology | Chart excluded from comparison (canvas vs img) |
| **D-07** | TreeView JavaScript | TreeView — `onclick="TreeView_SelectNode(...)"` stripped by normalization |
| **D-08** | Calendar day selection | Calendar — `href="[postback]"` links vs `<a style="cursor:pointer">` event handlers |

**Note on D-01 in CheckBox/RadioButtonList:** While ID mangling is intentional (D-01), the Blazor components are generating **GUID-based IDs** (e.g., `1c0d196d71644cf1b391bc8a93964e15`) instead of developer-meaningful IDs. This is a separate concern from the `ctl00_` prefix stripping — GUIDs make the HTML non-deterministic and harder to target with CSS/JS. This warrants a new divergence entry or a bug fix to use stable, predictable IDs.

---

## 5. Controls Not Captured

### 5.1 WebForms HTTP 500 Failures

Six WebForms pages returned HTTP 500 errors because the controls require data sources (database connections, XML files, or programmatic data binding) that were not configured in the sample app:

| Control | Tier | Failure Reason | M12/M13 Plan |
|---------|------|---------------|--------------|
| **DataList** | 2 | Requires `DataSource` binding | M12 — add inline data source |
| **FormView** | 2 | Requires `DataSource` binding | M12 — add inline data source |
| **GridView** | 2 | Requires `DataSource` binding | M12 — add inline data source |
| **ListView** | 2 | Requires `DataSource` + templates | M12 — add inline data source |
| **Menu** | 3 | Requires `SiteMapDataSource` or items | M13 — add static menu items |
| **Repeater** | 2 | Requires `DataSource` binding | M12 — add inline data source |

**Chart** (Tier 4) also had 0 WebForms files but is permanently excluded from structural comparison per D-05.

### 5.2 Blazor Controls Without Markers

These controls have Blazor sample pages but lacked `data-audit-control` markers, so no normalized HTML could be extracted:

| Control | Variants Missing | Action Needed |
|---------|-----------------|---------------|
| **CheckBoxList** | 2 | Add `data-audit-control` markers to Blazor sample |
| **ImageButton** | 2 | Add `data-audit-control` markers to Blazor sample |
| **ListBox** | 2 | Add `data-audit-control` markers to Blazor sample |
| **RadioButton** | 3 | Add `data-audit-control` markers to Blazor sample |
| **TextBox** | 7 | Add `data-audit-control` markers to Blazor sample |

These 5 controls represent **16 missing variant comparisons** — a significant gap in the Tier 1 audit. All are simple controls expected to have near-match HTML.

---

## 6. Priority Fix List

Ranked by CSS/JS compatibility impact and fix difficulty:

| Priority | Control | Issue | CSS Impact | JS Impact | Difficulty | D-Code |
|----------|---------|-------|------------|-----------|------------|--------|
| **P1** | **Button** | Renders `<button>` instead of `<input type="submit">` | 🔴 HIGH | 🟡 MED | Medium | — |
| **P2** | **BulletedList** | Numbered lists render `<ul>` instead of `<ol>`; extra `<span>` wrapping; wrong `list-style-type` | 🔴 HIGH | 🟢 LOW | Low | — |
| **P3** | **LinkButton** | Missing `href` attribute; missing `CssClass` pass-through | 🔴 HIGH | 🟡 MED | Low | — |
| **P4** | **Calendar** | Missing `style` (border), missing `title`, missing `width:14%` on cells, simplified nav header | 🔴 HIGH | 🟢 LOW | Medium | Partial D-08 |
| **P5** | **CheckBox** | Extra wrapping `<span>` element | 🟡 MED | 🟢 LOW | Low | — |
| **P6** | **FileUpload** | GUID fragment appears as stray HTML attribute (`c2a4-44c1-86ca-...=""`) | 🟡 MED | 🟡 MED | Low | — |
| **P7** | **Image** | Emits empty `longdesc=""` when not configured | 🟢 LOW | 🟢 LOW | Low | — |
| **P8** | **CheckBox/RadioButtonList** | GUID-based IDs instead of stable developer-meaningful IDs | 🟡 MED | 🟡 MED | Medium | Related to D-01 |

### Fix Effort Estimate

| Difficulty | Count | Estimated Effort |
|------------|-------|-----------------|
| Low | 5 fixes (P2, P3, P5, P6, P7) | 1–2 hours each |
| Medium | 3 fixes (P1, P4, P8) | 3–5 hours each |
| **Total** | **8 fixes** | **~20–25 hours** |

---

## 7. Recommendations for M12

### 7.1 Immediate Actions (Before M12 Capture)

1. **Align sample content** — Update the Blazor sample pages to use the same text, URLs, data values, and attribute combinations as the WebForms samples. This will eliminate ~15 false-positive divergences and make the diff report actionable.

2. **Add missing Blazor markers** — Add `data-audit-control` markers to the 5 Blazor controls that were not captured (CheckBoxList, ImageButton, ListBox, RadioButton, TextBox). This closes 16 variant gaps.

3. **Fix P1–P3 bugs** — Button (`<input>` vs `<button>`), BulletedList (`<ol>` vs `<ul>`), and LinkButton (missing `href`) are high-impact, low-to-medium difficulty fixes that should be completed before M12 begins, so the Tier 1 re-audit in M13 shows clean results.

### 7.2 M12 Tier 2 Focus

4. **Add inline data sources** — For DataList, FormView, GridView, ListView, and Repeater, add in-page data sources (static arrays/collections, no database) to the WebForms samples so they render successfully.

5. **Enhance normalization for data controls** — Extend the pipeline to handle `__doPostBack` pager links, sort header links, and auto-generated column IDs (per M12-02).

6. **Capture Menu with static items** — Provide a Menu sample with declarative `<asp:MenuItem>` items (no `SiteMapDataSource`) to unblock comparison.

### 7.3 Registry Updates

7. **Add D-11: GUID-Based IDs** — Document the Blazor pattern of generating GUID-based IDs for CheckBox, RadioButton, RadioButtonList, and FileUpload. Decide whether this is intentional or a bug to fix.

8. **Add D-12: Boolean Attribute Format** — Document `selected=""` (HTML5 boolean) vs `selected="selected"` (XHTML) as an intentional platform difference.

### 7.4 Process Improvements

9. **HyperLink case sensitivity fix** — The WebForms capture folder uses "Hyperlink" (lowercase L) while Blazor uses "HyperLink" (uppercase L), causing duplicate entries in the diff report. Normalize folder naming.

10. **Re-run Tier 1 after fixes** — After P1–P3 fixes and sample alignment, re-run the Tier 1 comparison as part of M13 to establish a corrected baseline.

---

## 8. Appendix

### 8.1 Raw Data Files

All raw capture data, normalized HTML, and diff output are stored in:

| Path | Contents |
|------|----------|
| `audit-output/webforms/{Control}/` | Raw WebForms HTML + screenshots (153 files across 24 controls) |
| `audit-output/blazor/{Control}/` | Raw Blazor HTML + screenshots (125 files across 18+ controls) |
| `audit-output/normalized/webforms/` | Normalized WebForms HTML (66 files) |
| `audit-output/normalized/blazor/` | Normalized Blazor HTML (45 files) |
| `audit-output/diff-report.md` | Full comparison report (70 entries, 524 lines) |

### 8.2 Reference Documents

| Document | Path |
|----------|------|
| Intentional Divergence Registry | `planning-docs/DIVERGENCE-REGISTRY.md` |
| HTML Audit Milestone Plan | `planning-docs/HTML-AUDIT-MILESTONES.md` |

### 8.3 Divergence Classification Summary

| Classification | Count | % of 70 |
|---------------|-------|---------|
| Sample parity (false positive) | ~22 | 31% |
| Intentional divergence (D-01–D-10) | ~12 | 17% |
| Missing Blazor capture | 21 | 30% |
| **Genuine structural bug** | **~15** | **22%** |

### 8.4 Controls by Comparison Verdict

**Near Match (structure correct, content differs):**
HiddenField, PlaceHolder, Literal, DropDownList (structure), Image (structure), Panel (structure), ImageMap (structure)

**Tag Structure Bug (requires code fix):**
Button, BulletedList, LinkButton, CheckBox (wrapping span), FileUpload (GUID attribute)

**Missing Attributes (requires code fix):**
Calendar (multiple), LinkButton (href, class), Image (empty longdesc)

**Not Yet Comparable (needs capture work):**
CheckBoxList, ImageButton, ListBox, RadioButton, TextBox, DataList, FormView, GridView, ListView, Menu, Repeater

---

*— Forge, Lead / Web Forms Reviewer*
*Milestone 11 Audit Complete*

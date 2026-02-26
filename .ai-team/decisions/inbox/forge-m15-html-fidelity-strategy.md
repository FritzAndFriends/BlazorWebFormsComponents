# M15 HTML Fidelity Strategy — Post-PR #377 Assessment

**Date:** 2026-02-28
**Author:** Forge (Lead / Web Forms Reviewer)
**Requested by:** Jeffrey T. Fritz
**Context:** PR #377 merged to upstream/dev — contains M11–M14 deliverables including full HTML fidelity audit (132 comparisons), 14 bug fixes, post-fix re-run (131 divergences, 1 exact match), WebFormsPage component, data alignment, and NamingContainer.

---

## 1. Current State Assessment

### Where We Stand

After PR #377, we have the most complete picture of HTML fidelity this project has ever produced. The numbers tell a clear story:

| Metric | Value | Assessment |
|--------|-------|------------|
| Total comparisons | 132 (128 unique + 4 HyperLink case dupes) | Good coverage of captured controls |
| Exact matches | **1** (Literal-3) | Sobering — but misleading (see below) |
| Verified structural improvements | 11 controls | The 14 bug fixes landed and verified |
| Missing Blazor captures | 64 variants | **The #1 coverage gap** |
| Sample parity false positives | ~22 entries | Noise masking real signal |
| Genuine remaining structural bugs | ~5 controls | Fixable in M15 |
| Data control investigations needed | 4 controls | Unknown severity |

**The "1 exact match" number is misleading.** The vast majority of divergences are caused by **different sample data** between WebForms and Blazor, not by component bugs. If we align the sample data, I estimate **15–20 controls** would achieve exact or near-exact match immediately. The actual HTML structure is correct for most Tier 1 controls — DropDownList, HyperLink, HiddenField, Image (minus `longdesc`), ImageMap, Label, Literal, Panel, PlaceHolder, and AdRotator all render correct tag structure with only content differences.

### Controls by Distance from Pixel-Perfect

**Near-perfect (correct structure, needs sample data alignment only):** ~12 controls
- AdRotator, DropDownList (6 variants), HiddenField, HyperLink (4 variants), Image (1 of 2), ImageMap, Label (3 variants), Literal (2 of 3), Panel (3 variants), PlaceHolder

**Close (1–2 fixable attribute/structural issues):** ~6 controls
- Button (fixed `<input>` — now needs sample parity only), BulletedList (still needs `<ol>` + `list-style-type`), CheckBox (remove `<span>` wrapper), Image (remove empty `longdesc`), LinkButton (add `href` + `class`), FileUpload (GUID attribute leak)

**Moderate (structural work needed):** ~4 controls
- Calendar (73% similarity — missing styles, `title`, navigation header differences), RadioButtonList (GUID IDs), GridView (33 line diff — mixed structural + sample), DataList (110 line diff — needs investigation)

**Far (significant structural divergence):** ~3 controls
- TreeView (deep structural differences in node rendering), ListView (182 line diff — template-based output), Repeater (64 line diff)

**Unknown (missing Blazor captures):** ~20+ controls
- All Login family (7 controls), all Validators (6 controls), Menu (9 variants), TextBox (7 variants), RadioButton (3), CheckBoxList (2), ImageButton (2), ListBox (2), SiteMapPath (2), MultiView, Table (3), DataPager, DetailsView, FormView

---

## 2. Remaining Divergence Analysis

### Category A — Fixable Structural Bugs (5 controls, ~8–14 hours)

These are genuine HTML structure differences where the Blazor component renders wrong elements or missing attributes. All fixable.

| Control | Bug | Effort | Variants Fixed |
|---------|-----|--------|----------------|
| **BulletedList** | Still renders `<ul>` for numbered lists (should be `<ol>`); wrong `list-style-type` values (`disc`/`circle` vs `decimal`/`square`) | 1–2 hrs | 3 |
| **LinkButton** | Missing `href` attribute; missing `CssClass` → `class` pass-through | 1–2 hrs | 3 |
| **CheckBox** | Extra wrapping `<span>` around checkbox+label pair for TextAlign=Right | 1–2 hrs | 3 |
| **Image** | Emits empty `longdesc=""` when `DescriptionUrl` is unset | 30 min | 2 |
| **FileUpload** | Stray GUID fragment leaks as HTML attribute (CSS isolation scope artifact) | 1 hr | 1 |

**Status of PR #377 fixes:** Button (`<input type="submit">`), BulletedList `<span>` removal, LinkButton `href`, Calendar border/`<tbody>`, CheckBox wrapper removal, Image `longdesc`, DataList border-collapse, GridView `rules`/`border`, and TreeView compression were all **fixed and verified** in the post-fix capture. However, BulletedList `<ol>` rendering and `list-style-type` mapping were NOT addressed. LinkButton `class` was NOT addressed. CheckBox may still have issues depending on capture state vs. code state.

### Category B — Sample Parity Issues (NOT component bugs, ~22 entries)

The single highest-value action for improving match rates. These controls render correct HTML structure but use completely different data/text/URLs between WebForms and Blazor samples:

| Control | Nature of Difference |
|---------|---------------------|
| AdRotator | Bing vs Microsoft ad content, different images |
| BulletedList | Apple/Banana/Cherry vs Item One/Two/Three |
| Button | "Blue Button" w/ styling vs "Click me!" no styling |
| CheckBox | "Accept Terms" vs "I agree to terms" |
| DropDownList (×6) | Different option text/values throughout |
| HiddenField | Different `value` attribute |
| HyperLink (×4) | Bing vs GitHub, different text/URLs |
| Image (×2) | Different src/alt |
| ImageMap | Different coordinates/URLs |
| Label (×3) | Different text/classes |
| Literal (×2) | Different text content |
| Panel (×3) | Different inner content |
| PlaceHolder | Different placeholder text |

**Fix approach:** Update Blazor sample pages to mirror exact WebForms content. This is a Jubilee task — mechanical but tedious. Estimated effort: 4–6 hours. Impact: potentially **20+ controls move to exact match** in one sprint.

### Category C — Intentional/Unfixable Divergences (D-01 through D-10)

These are permanent architectural differences. The normalizer already handles them. No action needed.

| D-Code | What It Is | Status |
|--------|-----------|--------|
| D-01 | ID mangling (`ctl00_` prefixes) | ✅ Normalized — stripped before comparison |
| D-02 | `__doPostBack` → `@onclick` | ✅ Normalized — replaced with placeholder |
| D-03 | ViewState hidden fields | ✅ Normalized — stripped |
| D-04 | WebResource.axd URLs | ✅ Normalized — stripped |
| D-05 | Chart `<img>` vs `<canvas>` | ⛔ Excluded from audit entirely |
| D-06 | Menu Table mode (Blazor = List only) | ✅ Documented — 4 variants permanently divergent |
| D-07 | TreeView JS injection | ✅ Normalized — JS stripped |
| D-08 | Calendar `__doPostBack` | ✅ Normalized — postback replaced |
| D-09 | Login control auth infrastructure | ✅ Documented — compare visual shell only |
| D-10 | Validator client-side scripts | ✅ Documented — compare `<span>` only |

**New divergence candidates identified (need decision):**

| Proposed | What It Is | Recommendation |
|----------|-----------|----------------|
| **D-11** | GUID-based IDs for CheckBox/RadioButton/RadioButtonList/FileUpload | **Fix, don't register as intentional.** GUIDs make HTML non-deterministic and untargetable by CSS/JS. Controls should use developer-provided `ID` parameter and append `_0`, `_1` per Web Forms convention. |
| **D-12** | Boolean attribute format: `selected=""` (HTML5) vs `selected="selected"` (XHTML) | **Register as intentional.** Both are valid HTML. Blazor uses HTML5-style; Web Forms uses XHTML-style. Add normalizer rule to treat as equivalent. |
| **D-13** | Calendar previous-month day padding (WF shows Jan 25–31; Blazor starts at Feb 1) | **Fix.** Web Forms Calendar pads the first week with previous month's days. Our Blazor Calendar should do the same — it's visible structural content. |
| **D-14** | Calendar style property pass-through (WF applies inline styles for ForeColor, Font, etc.; Blazor doesn't) | **Fix progressively.** Calendar style application is incomplete — the `<table>` and cell-level styles from `TitleStyle`, `DayStyle`, `OtherMonthDayStyle`, `WeekendDayStyle`, `TodayDayStyle` etc. are not being applied. This is a significant fidelity gap for Calendar specifically. |

### Category D — Normalizer Artifacts (3 items)

| Issue | Impact | Fix |
|-------|--------|-----|
| HyperLink/Hyperlink case-sensitivity duplication | 4 phantom entries in diff report | Case-insensitive folder matching in diff script |
| `selected=""` vs `selected="selected"` | False divergence in DropDownList | Add normalizer rule for boolean HTML attributes |
| Empty `style=""` on some controls | Noise in diffs | Strip empty `style=""` attributes in normalizer |

---

## 3. Prioritized Next Steps — What M15 Should Focus On

Ranked by **impact per unit of effort**:

### 🔴 Tier 1 Priority: Sample Data Alignment (Impact: MASSIVE, Effort: MEDIUM)

**This is the single highest-leverage action.** One sprint of sample alignment work could move us from 1 exact match to 15–20+ exact matches. Every other effort is wasted if the samples don't match — we can't tell what's a bug vs. what's different data.

- Update all Blazor sample pages to use identical text, values, URLs, styles as WebForms samples
- Covers: AdRotator, BulletedList, Button, CheckBox, DropDownList, HiddenField, HyperLink, Image, ImageMap, Label, Literal, Panel, PlaceHolder, LinkButton
- Owner: Jubilee
- Effort: 4–6 hours
- Impact: ~22 false-positive divergences eliminated; true structural bugs surface cleanly

### 🔴 Tier 2 Priority: Fix Remaining Structural Bugs (Impact: HIGH, Effort: LOW-MEDIUM)

Ship the remaining P1–P3 bug fixes that were identified but not addressed in PR #377:

| # | Fix | Effort | Impact |
|---|-----|--------|--------|
| 1 | BulletedList: `<ol>` for numbered + correct `list-style-type` mapping | 1–2 hrs | 3 variants → exact match |
| 2 | LinkButton: Pass `CssClass` → `class` attribute | 1 hr | 3 variants improved |
| 3 | Image: Only emit `longdesc` when non-empty | 30 min | 2 variants → exact match |
| 4 | FileUpload: Remove stray GUID attribute leak | 1 hr | 1 variant → exact match |
| 5 | CheckBox: Remove wrapping `<span>` (verify post-PR #377 state) | 1 hr | 3 variants improved |
| 6 | RadioButtonList: Stable IDs (not GUIDs) | 1–2 hrs | 2 variants improved |

Owner: Cyclops
Total effort: 6–9 hours
Impact: 14+ variants improved, 6+ potentially exact match

### 🟡 Tier 3 Priority: Close Blazor Capture Gaps (Impact: HIGH, Effort: MEDIUM)

64 variants have no Blazor capture. Prioritize by ROI:

**Phase A — Add `data-audit-control` markers to EXISTING Blazor pages (2–3 hrs):**
- TextBox (7 variants), RadioButton (3), CheckBoxList (2), ImageButton (2), ListBox (2), Button (variants 2–5), Literal (variant 3), SiteMapPath (2), Table (3), MultiView (1)
- These controls already have Blazor sample pages — they just lack `data-audit-control` markers
- Impact: ~25 more comparisons enabled

**Phase B — Write new Blazor sample pages for missing controls (6–10 hrs):**
- Menu (5 List-mode variants — 4 Table-mode are permanently divergent per D-06)
- Login family (7 controls × 2 variants = 14 captures) — visual shell comparison
- Validators (6 controls × 3–4 variants = ~23 captures) — `<span>` comparison only
- DataPager, DetailsView, FormView
- Impact: ~42 more comparisons

Owner: Jubilee (samples), Colossus (captures)

### 🟢 Tier 4 Priority: Normalizer Enhancements (Impact: MEDIUM, Effort: LOW)

| Enhancement | Effort | Impact |
|------------|--------|--------|
| Case-insensitive folder matching (kills 4 HyperLink dupes) | 30 min | Cleaner reports |
| Boolean attribute normalization (`selected=""` ↔ `selected="selected"`) | 30 min | DropDownList false positives eliminated |
| Empty `style=""` stripping | 15 min | Noise reduction |
| GUID ID normalization (strip or placeholder) | 1 hr | CheckBox/RadioButtonList/FileUpload noise reduced |

Owner: Cyclops
Total effort: 2–3 hours

### 🟢 Tier 5 Priority: Data Control Deep Investigation (Impact: UNKNOWN, Effort: MEDIUM)

DataList (110 lines), GridView (33 lines), ListView (182 lines), Repeater (64 lines) — all have both captures but show significant divergences. Need line-by-line classification.

**My assessment before investigation:**
- **GridView** (33 lines) — Likely mostly sample parity. The post-fix capture shows correct `<thead>`/`<tbody>` structure, `rules`/`border` attributes. Remaining diff is mostly different column names and content. Probably fixable with sample alignment.
- **DataList** (110 lines) — The `<table>` structure is the right approach, but template rendering likely produces different cell content. Mix of sample parity and template handling differences.
- **Repeater** (64 lines) — Repeater is unusual because it has no default chrome — it renders pure template output. Divergence is almost certainly 100% sample parity.
- **ListView** (182 lines) — Largest divergence. ListView's template model is the most flexible in Web Forms and the hardest to match structurally. This control is likely to have genuine structural differences in how templates are composed.

Owner: Forge (classification), Cyclops (fixes)
Effort: 4–8 hours investigation, unknown fix effort

### 🔵 Tier 6 Priority: Calendar Deep Fix (Impact: HIGH for one control, Effort: HIGH)

Calendar is the most complex control and the closest to pixel-perfect among complex controls (73% similarity on best variant). Remaining gaps:

1. **Previous-month day padding** — WF shows Jan 25–31 in the first row; Blazor starts at Feb 1
2. **Style property pass-through** — `TitleStyle`, `DayStyle`, `OtherMonthDayStyle`, `WeekendDayStyle`, `TodayDayStyle`, `NextPrevStyle`, `SelectorStyle` inline styles not applied
3. **`title` attribute on `<table>`** — WF emits `title="Calendar"`; Blazor doesn't
4. **`id` attribute on `<table>`** — WF emits developer ID; Blazor doesn't (may be NamingContainer related)
5. **`color` attribute on day links** — WF emits `style="color:#000000;"`; Blazor uses `cursor:pointer`
6. **`background-color` on title row** — WF emits `background-color:#c0c0c0`; Blazor doesn't

Owner: Cyclops
Effort: 5–8 hours (touching multiple Razor files and style resolution logic)
Impact: Could push Calendar from 73% to 90%+ similarity

---

## 4. Pixel-Perfect Realism

Let me be blunt about what "pixel-perfect" means for this project.

### Controls That CAN Achieve Exact Normalized Match

With sample alignment + remaining bug fixes, these controls should achieve **100% normalized HTML match**:

| Control | What's Needed | Confidence |
|---------|--------------|------------|
| Literal | ✅ Already exact (1 of 3 variants) | 100% |
| HiddenField | Sample alignment only | 99% |
| PlaceHolder | Sample alignment only | 99% |
| Label | Sample alignment only | 98% |
| Panel | Sample alignment only | 98% |
| AdRotator | Sample alignment only | 95% |
| Image | Remove `longdesc` + sample alignment | 95% |
| HyperLink | Sample alignment only | 95% |
| ImageMap | Sample alignment only | 95% |
| DropDownList | Sample alignment + boolean attr normalizer | 90% |
| BulletedList | `<ol>` fix + `list-style-type` + sample alignment | 90% |
| LinkButton | `href` + `class` fix + sample alignment | 85% |
| Button | Sample alignment only (already fixed to `<input>`) | 85% |

**Realistic target: 13–15 exact matches after M15 (up from 1 today).**

### Controls That Can Achieve Near-Match (>90% normalized similarity)

| Control | Gap | Realistic Ceiling |
|---------|-----|-------------------|
| Calendar | Style pass-through, day padding, title | 90–95% with fixes |
| CheckBox | `<span>` removal, stable IDs | 90–95% with fixes |
| RadioButtonList | Stable IDs | 90% with ID fix |
| FileUpload | GUID attribute removal | 95% |
| GridView | Sample alignment + investigation | 85–95% TBD |

### Controls That Will ALWAYS Have Structural Differences

Be honest. These controls have fundamental architectural gaps that make exact HTML match impossible or impractical:

| Control | Why | Permanent Gap |
|---------|-----|---------------|
| **Chart** | `<canvas>` vs `<img>` — completely different rendering technology | 100% different (D-05) |
| **Menu (Table mode)** | Blazor only implements List mode; 4 Table-mode variants are permanently divergent | Partial (D-06) — List mode is comparable |
| **TreeView** | Web Forms uses `<div><table><tr><td>` nested hierarchy per node; Blazor simplified. This is a deep structural choice unlikely to be changed without major refactor. | 30–50% — structural redesign needed for parity |
| **Calendar** | Will never hit 100% due to `cursor:pointer` vs `color:#000000`, WF `title="Calendar"`, and style granularity differences | Ceiling ~95% with effort |
| **Login controls** | Auth infrastructure divergence (D-09) — visual HTML can match but functional attributes won't | Visual shell match possible; functional attributes diverge |
| **Validators** | Client-side script infrastructure (D-10) — `<span>` output matchable but evaluation attributes won't | `<span>` match possible; JS attributes diverge |

### What "Pixel-Perfect" Realistically Means

For this project, "pixel-perfect" should be defined as:

> **After normalization (stripping intentional divergences D-01 through D-10+), the Blazor component's rendered HTML structure — tag names, nesting, CSS classes, and meaningful attributes — matches the Web Forms gold standard.**

That definition excludes:
- ID values (D-01 — always different)
- Event mechanism attributes (D-02 — `__doPostBack` vs `@onclick`)
- Infrastructure elements (D-03, D-04 — ViewState, WebResource.axd)
- Content data (sample parity — responsibility of the migration developer, not the library)

Under this definition, **I believe 20–25 controls can achieve "pixel-perfect" status** with the work outlined in M15. Another 10–15 can achieve "near-perfect" (>90% structural match). The remaining ~10–15 controls (Chart, TreeView, Menu Table-mode, Login family, Validators) will have documented intentional divergences that are architecturally unavoidable.

---

## 5. Recommended M15 Scope

### Milestone 15: HTML Fidelity Closure

**Branch:** `milestone15/html-fidelity-closure`
**Duration estimate:** 2–3 weeks
**Theme:** Close the gap between audit findings and actual HTML fidelity. Move from 1 exact match to 15+ exact matches.

### Work Items

| # | Work Item | Description | Owner | Size | Priority |
|---|-----------|-------------|-------|------|----------|
| M15-01 | **Sample data alignment** | Update ALL Blazor sample pages to mirror exact WebForms sample content (text, URLs, values, attributes, data items). Use WebForms captures in `audit-output/webforms/` as source of truth. Cover: AdRotator, BulletedList, Button, CheckBox, DropDownList, HiddenField, HyperLink, Image, ImageMap, Label, LinkButton, Literal, Panel, PlaceHolder. | Jubilee | L | 🔴 P0 |
| M15-02 | **BulletedList `<ol>` fix** | Render `<ol>` when `BulletStyle` is Numbered/LowerAlpha/UpperAlpha/LowerRoman/UpperRoman. Fix `list-style-type` CSS mapping: Numbered→decimal, Circle→circle, Disc→disc, Square→square, LowerAlpha→lower-alpha, UpperAlpha→upper-alpha, LowerRoman→lower-roman, UpperRoman→upper-roman. | Cyclops | S | 🔴 P1 |
| M15-03 | **LinkButton `class` pass-through** | Ensure `CssClass` parameter maps to `class` attribute on the rendered `<a>` element. | Cyclops | XS | 🔴 P1 |
| M15-04 | **Image `longdesc` conditional** | Only render `longdesc` attribute when `DescriptionUrl` has a non-empty value. | Cyclops | XS | 🟡 P2 |
| M15-05 | **FileUpload GUID attribute removal** | Investigate and remove stray CSS-isolation-scope GUID that leaks as an HTML attribute. | Cyclops | XS | 🟡 P2 |
| M15-06 | **CheckBox `<span>` removal verification** | Verify the PR #377 fix is complete for all TextAlign variants. If `<span>` wrapper persists for any variant, fix it. | Cyclops | S | 🟡 P2 |
| M15-07 | **Stable IDs for CheckBox/RadioButtonList** | Replace GUID-based IDs with developer-provided `ID` parameter. For RadioButtonList, append `_0`, `_1`, etc. per Web Forms convention. Fix `name` attribute to use control ID. | Cyclops | M | 🟡 P2 |
| M15-08 | **Add `data-audit-control` markers** | Add markers to existing Blazor sample pages for: TextBox (7), RadioButton (3), CheckBoxList (2), ImageButton (2), ListBox (2), Button (variants 2–5), SiteMapPath (2), Table (3), MultiView (1). ~25 new comparisons. | Jubilee | M | 🟡 P2 |
| M15-09 | **Normalizer enhancements** | (a) Case-insensitive folder matching, (b) Boolean attribute normalization, (c) Empty `style=""` stripping, (d) GUID ID normalization. | Cyclops | S | 🟢 P3 |
| M15-10 | **Data control deep investigation** | Line-by-line classification of DataList (110 lines), GridView (33 lines), ListView (182 lines), Repeater (64 lines) divergences. Separate genuine bugs from sample parity and D-01/D-02. File issues for genuine bugs. | Forge | M | 🟢 P3 |
| M15-11 | **Re-run full audit pipeline** | After all fixes and sample alignment, re-run the complete capture + normalize + diff pipeline. Target: ≥15 exact matches. Produce updated diff report. | Colossus | M | 🟢 P3 |
| M15-12 | **Update divergence registry** | Add D-11 through D-14 as appropriate. Document any new divergences discovered during M15 investigation. Update `DIVERGENCE-REGISTRY.md`. | Forge | S | 🟢 P3 |

### Agent Assignments

| Agent | Work Items | Role |
|-------|-----------|------|
| **Forge** | M15-10, M15-12 | Data control investigation, divergence registry, overall review |
| **Cyclops** | M15-02 through M15-07, M15-09 | Bug fixes, normalizer enhancements |
| **Jubilee** | M15-01, M15-08 | Sample alignment, marker insertion |
| **Colossus** | M15-11 | Full pipeline re-run |
| **Rogue** | — | Test updates for any HTML fixes Cyclops makes |
| **Beast** | — | Doc updates post-M15 if new exact matches warrant verification badges |

### Dependencies

```
M15-01 ──→ M15-11  (sample alignment before re-run)
M15-02 through M15-07 ──→ M15-11  (bug fixes before re-run)
M15-09 ──→ M15-11  (normalizer fixes before re-run)
M15-08 ──→ M15-11  (new markers before re-run)
M15-10 ──→ M15-12  (investigation informs registry)
M15-11 ──→ M15-12  (re-run results inform final registry)
```

### Exit Criteria

1. ≥15 controls achieve exact normalized HTML match (up from 1)
2. All Category A structural bugs (BulletedList, LinkButton, Image, FileUpload, CheckBox) are fixed
3. Blazor sample data aligned to WebForms for all Tier 1 controls with existing captures
4. ≥25 new Blazor comparisons enabled via `data-audit-control` markers
5. Data control divergences (DataList, GridView, ListView, Repeater) classified with issues filed for genuine bugs
6. Normalizer enhanced with boolean attribute, case-insensitive matching, and GUID ID handling
7. Updated diff report showing improved match rates
8. Divergence registry updated to D-14

### Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| Sample alignment takes longer than estimated due to complex data binding differences | Medium | Low | Prioritize simple controls first; complex data controls can follow in M16 |
| BulletedList `<ol>` fix breaks existing tests | Low | Medium | Run full test suite before PR; bUnit tests likely cover this |
| GUID ID replacement breaks Blazor event binding | Medium | Medium | Test interactivity after ID stabilization; Blazor may use `@ref` not ID for event wiring |
| Post-fix re-run reveals new regressions | Low | Low | Git bisect to isolate; targeted fix |
| Calendar style pass-through is larger than estimated | High | Medium | Defer Calendar deep fix to M16 if scope grows; capture current similarity as baseline |

---

## 6. Beyond M15 — The Road to Maximum Fidelity

If M15 hits its targets (15+ exact matches, all Tier 1 structural bugs fixed, sample parity for simple controls), the roadmap for M16+ looks like this:

| Milestone | Focus | Expected Outcome |
|-----------|-------|-----------------|
| **M16** | Calendar deep fix (styles, day padding, title), TreeView structural alignment investigation, Menu List-mode Blazor captures | Calendar → 90%+; TreeView assessment; Menu assessed |
| **M17** | Login family visual shell captures + comparison, Validator `<span>` comparison | Coverage expanded to 40+ controls compared |
| **M18** | Data control sample alignment + structural fixes (GridView, DataList, ListView, Repeater) | Data controls assessed and fixed |
| **M19** | CI integration — automated HTML regression in build pipeline | Prevents regression going forward |

The honest bottom line: **This library will never achieve 100% exact HTML match for all controls.** Chart, TreeView, Menu Table-mode, and the event mechanism infrastructure are permanently divergent by design. But for the ~35 controls that represent the everyday migration path (Button, TextBox, Label, DropDownList, GridView, etc.), we should be able to reach 90%+ structural match — and that's what developers migrating from Web Forms actually need.

---

**Decision:** M15 scope as defined above is recommended. Forge endorses this plan.

— Forge, Lead / Web Forms Reviewer

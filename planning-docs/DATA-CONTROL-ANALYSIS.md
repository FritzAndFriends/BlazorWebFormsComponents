# Data Control HTML Divergence Analysis

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-02-27
**Milestone:** M13
**Status:** Complete

---

## Executive Summary

This analysis examines the HTML output of four data controls — DataList, GridView, ListView, and Repeater — comparing the Web Forms (IIS) capture against the Blazor capture. The dominant cause of divergences across all four controls is **Sample Parity** — the Blazor sample pages use materially different templates, data columns, styles, and structure than the Web Forms sample pages. This means the captured diffs mostly reflect sample authoring differences, not component bugs.

However, genuine structural bugs were also found in GridView and DataList, and are documented below.

**Summary Table:**

| Control | Lines Diff | Primary Classification | Genuine Bugs | Sample Parity Issues |
|---------|-----------|----------------------|--------------|---------------------|
| DataList | 110 | Sample Parity + Bugs | 2 | 6 |
| GridView | 33 | Sample Parity + Bugs | 3 | 4 |
| ListView | 182 | Sample Parity (dominant) | 0 | 7 |
| Repeater | 64 | Sample Parity (100%) | 0 | 5 |

---

## 1. DataList — 110 Line Differences

### Classification: **Mix (Sample Parity + Genuine Bugs)**

The Blazor sample is a stripped-down version of the Web Forms sample. Most of the 110-line difference comes from the Blazor sample omitting templates and styles that the Web Forms sample exercises.

### Specific Findings

#### 1.1 Sample Parity Issues (Not Bugs)

| # | Web Forms Renders | Blazor Renders | Verdict |
|---|------------------|---------------|---------|
| SP-DL-1 | `id="simpleDataList"` with `tabindex="1"` and `title="This is my tooltip"` | No `id`, no `tabindex`, no `title` | **Sample Parity** — The Blazor sample omits `ToolTip`, `TabIndex`, and the `id` binding. The component code _does_ support `tabindex` and `title` attributes. |
| SP-DL-2 | `<caption align="Top">This is my caption</caption>` | No caption rendered | **Sample Parity** — The Blazor sample does not set `Caption` or `CaptionAlign`. |
| SP-DL-3 | `cellpadding="2" cellspacing="3"` on `<table>` | No `cellpadding`/`cellspacing` | **Sample Parity** — The Blazor sample does not set `CellPadding` or `CellSpacing`. |
| SP-DL-4 | HeaderTemplate: `<th class="myClass" scope="col" style="font-family:arial black;...">My Widget List</th>` with bold/italic/underline/overline/strikethrough fonts | HeaderTemplate: `<td>Simple Widgets</td>` with no styling | **Sample Parity** — Different header text and no `HeaderStyle`, `UseAccessibleHeader`, or CSS class in the Blazor sample. |
| SP-DL-5 | SeparatorTemplate: `<td style="color:PapayaWhip;background-color:Black;">Hi! I'm a separator!...</td>` between every item | No separator rows at all | **Sample Parity** — The Blazor sample does not include a `SeparatorTemplate`. |
| SP-DL-6 | AlternatingItemStyle alternates between `background-color:Yellow` and `background-color:Wheat` | All items use `background-color:Wheat` uniformly | **Sample Parity** — The WF sample uses `ItemStyle BackColor="Yellow"` + `AlternatingItemStyle BackColor="Wheat"`; the Blazor sample only sets `ItemStyle BackColor="Wheat"` with no alternating style. |

#### 1.2 Genuine Bugs

| # | Web Forms Renders | Blazor Renders | Verdict |
|---|------------------|---------------|---------|
| BUG-DL-1 | Item content on separate lines: `First Widget<br>$7.99` (name and price on separate lines with `<br>`) | Item content concatenated: `First Widget - $7.99` (single line with dash) | **Sample Parity** — This is a template difference in the Blazor sample (`@Item.Name - @Item.Price.ToString("c")` vs. the WF `<%# Item.Name %><br /><%# Item.Price.ToString("c") %>`). Not a component bug. |
| BUG-DL-2 | `itemtype="SharedSampleObjects.Models.Widget"` attribute on `<table>` | No `itemtype` attribute | **Genuine Bug** — Web Forms renders the `ItemType` as an HTML `itemtype` microdata attribute on the outer element. The Blazor `DataList.razor` does not render `ItemType` as an attribute. This is a fidelity gap. |
| BUG-DL-3 | `<table>` has no `style="border-collapse:collapse;"` when cellspacing="3" | Blazor always renders `style="border-collapse:collapse;"` | **Genuine Bug** — The Blazor DataList unconditionally adds `border-collapse:collapse;` to the table (line 27 of DataList.razor). Web Forms only adds `border-collapse:collapse` when `CellSpacing` is 0 (or not set). When `CellSpacing > 0`, Web Forms omits `border-collapse` so the spacing takes effect. The Blazor component ignores the CellSpacing value when deciding about `border-collapse`. |

### Recommendations

1. **BUG-DL-2 (itemtype attribute):** Add `itemtype="@(typeof(ItemType).FullName)"` or similar to the `<table>` element in `DataList.razor` only when `ItemType` is specified. Low priority — `itemtype` is a microdata attribute rarely relied upon in practice.

2. **BUG-DL-3 (border-collapse always on):** In `DataList.razor` line 27, change the `style` attribute to conditionally include `border-collapse:collapse;` only when `CellSpacing` is not set or is zero. Current code: `style="border-collapse:collapse;@CalculatedStyle"`. Fix: only emit `border-collapse:collapse;` when `CellSpacing == 0` (matching Web Forms behavior).

3. **Sample Parity:** The Blazor DataList sample needs significant enhancement to exercise the same features as the Web Forms sample — add `ToolTip`, `TabIndex`, `Caption`, `CaptionAlign`, `CellPadding`, `CellSpacing`, `HeaderStyle`, `UseAccessibleHeader`, `SeparatorTemplate`, `AlternatingItemStyle`, and `ItemStyle` variations. This is a Jubilee task.

---

## 2. GridView — 33 Line Differences

### Classification: **Mix (Sample Parity + Genuine Bugs)**

Despite the low line count, GridView has both sample parity issues and genuine structural divergences.

### Specific Findings

#### 2.1 Sample Parity Issues

| # | Web Forms Renders | Blazor Renders | Verdict |
|---|------------------|---------------|---------|
| SP-GV-1 | `id="MainContent_CustomersGridView"` | No `id` on `<table>` | **D-01 + Sample Parity** — Web Forms mangles the ID (D-01). The Blazor sample doesn't set an ID at all. |
| SP-GV-2 | `<div>` wrapper around `<table>` (Web Forms) | No `<div>` wrapper (Blazor) | **Sample Parity** — The Web Forms page has an explicit `<div data-audit-control="GridView">` wrapper, and the GridView itself is inside a `<div>`. The Blazor sample has `<div data-audit-control>` wrapping the component directly. This is a page-level difference, not a component issue. |
| SP-GV-3 | Last column: `<a href="https://www.bing.com/search?q=...">Search for Virus</a>` (HyperLinkField) | Last column: `<button type="submit"></button>` (ButtonField) | **Sample Parity** — The WF sample uses `HyperLinkField` with `DataNavigateUrlFields`; the Blazor sample uses `ButtonField` with `CommandName`. These are different column types rendering different elements. |
| SP-GV-4 | Header text: `CustomerID` | Header text: `ID` | **Sample Parity** — The Blazor sample uses `HeaderText="ID"` while the WF sample uses `HeaderText="CustomerID"`. |

#### 2.2 Genuine Bugs

| # | Web Forms Renders | Blazor Renders | Verdict |
|---|------------------|---------------|---------|
| BUG-GV-1 | `cellspacing="0" rules="all" border="1"` on `<table>` | No `cellspacing`, no `rules`, no `border` attributes | **Genuine Bug** — Web Forms GridView renders `rules="all"` and `border="1"` by default (GridLines="Both" is the default). The Blazor GridView component does not render the `rules` attribute or `border` when the sample doesn't explicitly set GridLines. The component _does_ support `GetGridLinesRules()` (see GridView.razor line 21), but the default value may not match Web Forms' default of `GridLines.Both`. |
| BUG-GV-2 | `style="border-collapse:collapse;"` on `<table>` | No inline `style` on `<table>` (only `class="table table-striped"`) | **Genuine Bug (partial)** — Web Forms always adds `border-collapse:collapse` on GridView. The Blazor component does not add this. However, this interacts with the CssClass — the Blazor sample uses Bootstrap classes (`table table-striped`) which handle border-collapse differently. This is both a sample parity issue (different CSS class) and a structural gap (missing default `border-collapse`). |
| BUG-GV-3 | `<th scope="col">&nbsp;</th>` for TemplateField and HyperLinkField headers (non-breaking space) | `<th></th>` (empty) for TemplateField; `<button type="submit"></button>` for ButtonField | **Genuine Bug** — When a column has no `HeaderText`, Web Forms renders `&nbsp;` in the `<th>`. The Blazor GridView renders an empty `<th>`. This changes the visual height/spacing of the header row. The `&nbsp;` is intentional in Web Forms to maintain consistent row height. |

### Recommendations

1. **BUG-GV-1 (default GridLines):** Verify that the Blazor GridView's default `GridLines` value is `GridLines.Both` (matching Web Forms). If the default is `None`, change it to `Both` so that `rules="all"` and `border="1"` are rendered by default. Check `GridView.razor.cs` for the default property value.

2. **BUG-GV-2 (border-collapse):** Add `border-collapse:collapse;` to the default style of the GridView `<table>` element. This matches Web Forms behavior. The style should be present even when CssClass is set (CSS class styles and inline styles coexist).

3. **BUG-GV-3 (empty header &nbsp;):** When a column's `HeaderText` is null or empty, render `&nbsp;` in the `<th>` cell. This matches Web Forms behavior and prevents header row collapse.

4. **Sample Parity:** Replace the `ButtonField` in the Blazor sample with a `HyperLinkField` matching the WF sample. Change `HeaderText="ID"` to `HeaderText="CustomerID"`. Remove the `CssClass="table table-striped"` to match the WF sample's default rendering. This is a Jubilee task.

---

## 3. ListView — 182 Line Differences

### Classification: **Sample Parity (dominant)**

The 182-line difference is almost entirely explained by:
1. The Blazor sample has its ItemSeparatorTemplate **commented out** (lines 43-46 of Index.razor: `@*<tr>...*@`).
2. The WF `AlternatingItemTemplate` uses `DataBinder.Eval` with currency format for the Id column (`{0:C}`), rendering `$2.00` for Id=2. The Blazor sample uses `@Item.Id`, rendering `2`. This is a sample bug — the WF sample incorrectly formats the integer Id as currency.
3. The WF output includes `<tr>` separator rows between every item; the Blazor output does not (because the template is commented out).
4. Minor date differences (`2/25/2026` vs `2/26/2026`) from different capture dates.

### Specific Findings

| # | Web Forms Renders | Blazor Renders | Verdict |
|---|------------------|---------------|---------|
| SP-LV-1 | No `class` on `<table>` | `class="table"` on `<table>` | **Sample Parity** — The Blazor sample adds a Bootstrap class not present in the WF sample. |
| SP-LV-2 | Separator rows: `<tr><td colspan="4" style="border-bottom:1px solid #000000;">&nbsp;</td></tr>` between every item pair | No separator rows | **Sample Parity** — The Blazor sample's `ItemSeparatorTemplate` is commented out with `@*...*@`. |
| SP-LV-3 | AlternatingItemTemplate Id column: `$2.00`, `$4.00`, `$6.00`, etc. (formatted as currency) | AlternatingItemTemplate Id column: `2`, `4`, `6`, etc. (integer) | **Sample Bug (WF side)** — The WF AlternatingItemTemplate uses `DataBinder.Eval(Container.DataItem, "Id", "{0:C}")` which formats the integer Id as currency. This is a bug in the WF sample, not in either component. The Blazor sample correctly renders the Id as an integer. |
| SP-LV-4 | `style="border-bottom:1px solid #000000;"` uses hex color | (not rendered) | **Sample Parity** — The normalized WF output converts color name `black` to `#000000`. |
| SP-LV-5 | LayoutTemplate structure: `<table><thead>...<tbody><tr id="itemPlaceHolder">` | Outer `<table class="table"><thead>...<tbody>` with ListView inside tbody | **Sample Parity** — In WF, the LayoutTemplate is inside the ListView control. In Blazor, the table structure is outside the component (the ListView has no LayoutTemplate set, so it uses a default passthrough). This is a different authoring pattern, not a bug — both are valid. |
| SP-LV-6 | Dates: `2/25/2026` | Dates: `2/26/2026` | **Sample Parity** — Different capture dates. |
| SP-LV-7 | `&nbsp;` in separator `<td>` | (not rendered) | **Sample Parity** — Consequence of SP-LV-2 (separator commented out). |

### Structural Component Assessment

The ListView component itself does not introduce structural divergences. The Blazor `ListView.razor` correctly:
- Iterates items with `ItemTemplate` and `AlternatingItemTemplate`
- Supports `ItemSeparatorTemplate` (when not commented out)
- Supports `LayoutTemplate` as a wrapping container
- Supports `EmptyDataTemplate`

**No genuine bugs found in the ListView component.**

### Recommendations

1. **Fix WF sample bug (SP-LV-3):** Change `DataBinder.Eval(Container.DataItem, "Id", "{0:C}")` to `<%# Item.Id %>` in the WF `AlternatingItemTemplate` to match the ItemTemplate pattern.

2. **Sample Parity:** Uncomment the Blazor `ItemSeparatorTemplate`, remove `class="table"` from the outer table, and move the `<table>` structure into a `LayoutTemplate` inside the ListView component (matching the WF pattern). This is a Jubilee task.

---

## 4. Repeater — 64 Line Differences

### Classification: **Sample Parity (100%)**

The Blazor Repeater sample renders **completely different content** than the Web Forms Repeater sample. The WF sample renders a simple `<li>` list with header/footer text and `<hr>` separators. The Blazor sample renders a `<table>` with 4-column rows showing Id, Name, Price, and LastUpdate with alternating styles.

Every single line of the 64-line diff is explained by the completely different template structures. The Repeater component itself is structurally simple (it just renders templates in order) and has no structural bugs.

### Specific Findings

| # | Web Forms Renders | Blazor Renders | Verdict |
|---|------------------|---------------|---------|
| SP-RP-1 | HeaderTemplate: plain text `This is a list of widgets` | No HeaderTemplate (Blazor sample omits it) | **Sample Parity** — Different templates. |
| SP-RP-2 | ItemTemplate: `<li>First Widget</li>` (single name, `<li>` element) | ItemTemplate: `<tr><td>1</td><td>First Widget</td><td>$7.99</td><td>2/26/2026</td></tr>` (table row, 4 columns) | **Sample Parity** — Completely different template structure and data displayed. |
| SP-RP-3 | SeparatorTemplate: `<hr>` (standalone horizontal rule) | SeparatorTemplate: `<tr><td colspan="4"><hr></td></tr>` (hr inside table cell) | **Sample Parity** — Different template structures. The Blazor version is correct for its context (inside a `<table>`). |
| SP-RP-4 | FooterTemplate: plain text `This is the footer of the control` | No FooterTemplate (Blazor sample omits it) | **Sample Parity** — Different templates. |
| SP-RP-5 | No wrapping element (items are bare `<li>` and text nodes) | `<table>` wrapping element (items are `<tr>` rows) | **Sample Parity** — The Blazor page adds a `<table>` around the Repeater. The WF sample has bare items. Note: the WF sample's `<li>` elements without a parent `<ul>` or `<ol>` is technically invalid HTML, but that's a WF sample issue. |

### Structural Component Assessment

The Repeater component is the simplest data control. It renders `HeaderTemplate` → (`ItemTemplate` or `AlternatingItemTemplate` with `SeparatorTemplate`) → `FooterTemplate`. Both the Web Forms and Blazor components do this identically. The component source (`Repeater.razor`) is correct and minimal.

**No genuine bugs found in the Repeater component.**

### Recommendations

1. **Sample Parity:** Rewrite the Blazor Repeater sample to match the Web Forms sample: use `<li>` items, `<hr>` separator, header/footer plain text, and no wrapping `<table>`. This will make the audit comparison meaningful. Alternatively, update both samples to use the same template structure. This is a Jubilee task.

---

## Cross-Cutting Observations

### 1. Sample Parity is the #1 Issue

Across all four controls, **the dominant source of divergence is that the Blazor sample pages use different templates, different data formats, different style attributes, and sometimes completely different column structures** compared to the Web Forms sample pages. This makes the raw diff count (110 + 33 + 182 + 64 = 389 lines) misleading — most of those lines reflect authoring differences, not component bugs.

**Root cause:** The Blazor samples were authored independently from the Web Forms samples, likely to demonstrate different component features. For the HTML fidelity audit to be meaningful, the Blazor samples must be port-accurate copies of the Web Forms samples (same templates, same data, same styles).

### 2. Genuine Bugs Found: 5

| Bug ID | Control | Description | Severity |
|--------|---------|-------------|----------|
| BUG-DL-2 | DataList | Missing `itemtype` attribute on `<table>` | P2 (Low) |
| BUG-DL-3 | DataList | `border-collapse:collapse` always rendered regardless of CellSpacing | P1 (Medium) |
| BUG-GV-1 | GridView | Default GridLines/rules/border may not match Web Forms defaults | P1 (Medium) |
| BUG-GV-2 | GridView | Missing default `border-collapse:collapse` style | P1 (Medium) |
| BUG-GV-3 | GridView | Empty `<th>` instead of `&nbsp;` for columns with no HeaderText | P2 (Low) |

### 3. Blazor Comment Markers

The Blazor captured HTML contains `<!--!-->` comment markers throughout. These are Blazor framework rendering markers (similar to Angular's `<!--ng-container-->`) and are **not a component bug**. They should be stripped by the normalization pipeline. Recommendation: add a normalization rule to strip `<!--!-->` markers.

### 4. No Normalized Blazor Output Exists for Data Controls

The `audit-output/normalized/blazor/` directory does not contain DataList, GridView, ListView, or Repeater subdirectories. Only raw captures exist under `audit-output/blazor/`. The normalization pipeline has not been run on these controls. This must be completed before a meaningful automated diff can be produced.

---

## Action Items

| # | Action | Owner | Priority |
|---|--------|-------|----------|
| 1 | Fix BUG-DL-3: Conditional `border-collapse` in DataList based on CellSpacing | Cyclops | P1 |
| 2 | Fix BUG-GV-1: Verify/fix default GridLines value to match Web Forms (Both) | Cyclops | P1 |
| 3 | Fix BUG-GV-2: Add default `border-collapse:collapse` to GridView table | Cyclops | P1 |
| 4 | Fix BUG-GV-3: Render `&nbsp;` for empty HeaderText in GridView columns | Cyclops | P2 |
| 5 | Fix BUG-DL-2: Add `itemtype` attribute to DataList table element | Cyclops | P2 |
| 6 | Rewrite Blazor DataList sample to match WF sample templates/styles | Jubilee | P1 |
| 7 | Rewrite Blazor GridView sample to match WF sample columns | Jubilee | P1 |
| 8 | Fix Blazor ListView sample: uncomment separator, match WF structure | Jubilee | P1 |
| 9 | Rewrite Blazor Repeater sample to match WF sample templates | Jubilee | P1 |
| 10 | Fix WF ListView sample: `DataBinder.Eval(..."Id", "{0:C}")` is a bug | Jubilee | P2 |
| 11 | Run normalization pipeline on data control captures | Colossus | P1 |
| 12 | Add `<!--!-->` stripping to normalization pipeline | Colossus | P1 |

---

## Appendix: File Locations

| File | Description |
|------|-------------|
| `audit-output/webforms/DataList/DataList.html` | Raw WF capture |
| `audit-output/blazor/DataList/DataList.html` | Raw Blazor capture |
| `audit-output/normalized/webforms/DataList/DataList.html` | Normalized WF (exists) |
| `audit-output/normalized/blazor/DataList/DataList.html` | Normalized Blazor (MISSING) |
| `samples/BeforeWebForms/ControlSamples/DataList/Default.aspx` | WF sample page |
| `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/DataList/Index.razor` | Blazor sample page |
| (same pattern for GridView, ListView, Repeater) | |

— Forge

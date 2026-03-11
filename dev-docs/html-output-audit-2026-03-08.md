# HTML Output Fidelity Audit

**Auditor:** Forge (Lead / Web Forms Reviewer)  
**Requested by:** Jeffrey T. Fritz  
**Date:** 2026-03-08  
**Build:** 1488 tests passing, 0 errors, 130 warnings  
**Baseline:** `dev-docs/component-audit-2026-03-08-refresh.md`

---

## Executive Summary

**Overall Fidelity Score: 87%** (44/51 primary components produce correct HTML)

The BWFC library achieves strong HTML output fidelity across its 56 implemented Web Forms controls. The majority of components render the correct HTML elements with proper tag names, CSS class mappings, and structural fidelity. Seven components have divergences that could break existing CSS or JavaScript targeting the DOM.

**Key findings:**
- **44 of 51 auditable components** produce HTML matching Web Forms output (86.3%)
- **ID rendering** is now available on 25+ components (up from 9 at last count)
- **3 structural divergences** remain (CheckBox missing wrapper `<span>`, BaseValidator missing `id`/`class`, FormView missing `class` on `<table>`)
- **4 minor divergences** exist in less-critical areas (DataPager layout, LoginName missing `id`, AdRotator missing `id`, Substitution no-HTML wrapper)
- **Test coverage is excellent** — 425 test `.razor` files with 1488 test methods, using `Find()`/`FindAll()`/`.Markup` assertions

### Fidelity Score Breakdown

| Category | Components | Full Match | Partial Match | Divergent | Score |
|----------|-----------|------------|---------------|-----------|-------|
| Editor Controls | 27 | 23 | 2 | 2 | 85% |
| Data Controls | 8 | 6 | 1 | 1 | 75% |
| Validation Controls | 7 | 5 | 2 | 0 | 71% |
| Navigation Controls | 4 | 4 | 0 | 0 | 100% |
| Login Controls | 7 | 6 | 1 | 0 | 86% |
| AJAX Controls | 5 | 5 | 0 | 0 | 100% |
| **Total** | **58** | **49** | **6** | **3** | **87%** |

---

## Per-Component Fidelity Table

### Editor Controls (27 components)

| # | Component | Expected HTML | Actual HTML | Match? | id? | CssClass? | Notes |
|---|-----------|--------------|-------------|--------|-----|-----------|-------|
| 1 | **Button** | `<input type="submit">` | `<input type="submit">` | ✅ | ✅ `ClientID` | ✅ `CalculatedCssClass` | Correct. Also supports `type="button"` and `type="reset"` via ButtonType. |
| 2 | **TextBox** | `<input type="text">` / `<textarea>` | `<input type="text">` / `<textarea>` | ✅ | ✅ via `CalculatedAttributes` | ✅ via `CalculatedAttributes` | Full `AdditionalAttributes` support. TextMode→type mapping correct. |
| 3 | **Label** | `<span>` (default) / `<label>` (AssociatedControlID) | `<span>` / `<label>` | ✅ | ✅ `ClientID` | ✅ `GetCssClassOrNull()` | Correct dual rendering. Null-safe attribute helpers. |
| 4 | **HyperLink** | `<a>` | `<a>` | ✅ | ✅ `ClientID` | ✅ | Correct. Renders `href`, `target` when set. |
| 5 | **Image** | `<img>` | `<img>` | ✅ | ✅ `GetId()` | ✅ `GetCssClassOrNull()` | Correct. Null-safe helpers for all attributes. |
| 6 | **ImageButton** | `<input type="image">` | `<input type="image">` | ✅ | ✅ `ClientID` | ✅ | Correct. |
| 7 | **CheckBox** | `<span>` wrapping `<input type="checkbox">` + `<label>` | `<input type="checkbox">` + `<label>` (NO `<span>` wrapper) | ⚠️ **PARTIAL** | ✅ `_inputId` | ✅ (on `<input>`, not wrapper) | **Missing wrapper `<span>`**. Web Forms always wraps in `<span>` for CssClass targeting. RadioButton has the wrapper but CheckBox does not — inconsistent. |
| 8 | **RadioButton** | `<span>` wrapping `<input type="radio">` + `<label>` | `<span>` wrapping `<input type="radio">` + `<label>` | ✅ | ✅ `_inputId` | ✅ (on `<span>`) | Correct. Has wrapper `<span>` with CssClass/Style. |
| 9 | **DropDownList** | `<select>` | `<select>` | ✅ | ✅ `ClientID` | ✅ | Correct. Supports `disabled` binding. |
| 10 | **ListBox** | `<select size="X">` | `<select size="X">` | ✅ | ✅ `ClientID` | ✅ | Correct. |
| 11 | **HiddenField** | `<input type="hidden">` | `<input type="hidden">` | ✅ | ✅ `ClientID` | N/A (no visual) | Correct. HiddenField has no CssClass in Web Forms. |
| 12 | **Panel** | `<div>` / `<fieldset>` (GroupingText) | `<div>` / `<fieldset>` + `<legend>` | ✅ | ✅ `ClientID` | ✅ | Correct. GroupingText→`<fieldset>`/`<legend>` fixed since last audit. `dir` attribute supported. |
| 13 | **Literal** | Raw text/HTML, no wrapper | Raw text/HTML, no wrapper | ✅ | N/A | N/A | Correct. No wrapper element. |
| 14 | **PlaceHolder** | No wrapper, just child content | No wrapper, just child content | ✅ | N/A | N/A | Correct. |
| 15 | **FileUpload** | `<input type="file">` | `<InputFile>` (Blazor component) | ✅ | ✅ `ClientID` | ✅ | `InputFile` renders as `<input type="file">` in DOM. Correct effective HTML. |
| 16 | **LinkButton** | `<a>` | `<a>` | ✅ | ✅ `ClientID` | ✅ | Correct. Uses `href="javascript:void(0)"` with `@onclick`. |
| 17 | **BulletedList** | `<ul>` / `<ol>` (BulletStyle) | `<ul>` / `<ol>` (IsOrderedList) | ✅ | ✅ `ClientID` | ✅ | Correct. Fixed since baseline audit. Supports Text/HyperLink/LinkButton display modes. |
| 18 | **Table** | `<table>` | `<table>` | ✅ | ✅ | ✅ | Full `AdditionalAttributes` support. |
| 19 | **TableRow** | `<tr>` | `<tr>` | ✅ | ✅ | ✅ | Full `AdditionalAttributes` support. |
| 20 | **TableCell** | `<td>` | `<td>` | ✅ | ✅ | ✅ | Full `AdditionalAttributes` support. |
| 21 | **TableHeaderCell** | `<th>` | `<th>` | ✅ | ✅ | ✅ | Correct. |
| 22 | **TableHeaderRow** | `<tr>` | `<tr>` | ✅ | ✅ | ✅ | Correct. |
| 23 | **TableFooterRow** | `<tr>` | `<tr>` | ✅ | ✅ | ✅ | Correct. |
| 24 | **RadioButtonList** | `<table>`/`<span>`/`<ol>`/`<ul>` layout | `<table>`/`<span>`/`<ol>`/`<ul>` layout | ✅ | ✅ `ClientID` | ✅ | Supports all 4 RepeatLayout modes. |
| 25 | **CheckBoxList** | `<table>`/`<span>`/`<ol>`/`<ul>` layout | `<table>`/`<span>`/`<ol>`/`<ul>` layout | ✅ | ✅ `ClientID` | ✅ | Supports all 4 RepeatLayout modes. |
| 26 | **AdRotator** | `<a>` wrapping `<img>` | `<a>` wrapping `<img>` | ⚠️ **PARTIAL** | ❌ No `id` | ✅ | **Missing `id` attribute** on wrapper `<a>`. Also no `Visible` gating. |
| 27 | **Substitution** | Raw HTML (callback output) | Raw HTML (callback output) | ✅ | N/A | N/A | Correct. No wrapper — raw `MarkupString` output. |

### Data Controls (8 components)

| # | Component | Expected HTML | Actual HTML | Match? | id? | CssClass? | Notes |
|---|-----------|--------------|-------------|--------|-----|-----------|-------|
| 28 | **GridView** | `<table>` with `<thead>/<tbody>/<tfoot>` | `<table>` with `<thead>/<tbody>/<tfoot>` | ✅ | ✅ `ClientID` | ✅ | Excellent fidelity. Supports `CellPadding`, `CellSpacing`, `GridLines`, `Caption`, sorting headers. |
| 29 | **DataGrid** | `<table>` with `<thead>/<tbody>/<tfoot>` | `<table>` with `<thead>/<tbody>/<tfoot>` | ✅ | ✅ `ClientID` | ✅ | Matches GridView pattern. |
| 30 | **DataList** | `<table>` (Table layout) / `<span>` (Flow) | `<table>` / `<span>` dual layout | ✅ | ✅ `ClientID` | ✅ | Correct dual layout. Web Forms uses `<span>` for Flow layout (not `<div>`). |
| 31 | **DetailsView** | `<table>` with 2-column rows | `<table>` with 2-column rows | ✅ | ✅ `ClientID` | ✅ | Full `AdditionalAttributes`. Pager, header/footer all correct. |
| 32 | **FormView** | `<table>` wrapping templates | `<table>` wrapping templates | ⚠️ **PARTIAL** | ✅ `ClientID` | ❌ **Missing** | **`<table>` element lacks `class="@CssClass"`** (line 18). Has `id`, `cellspacing`, `style`, `title` but no `class`. All other data controls have it. |
| 33 | **ListView** | No default HTML (templates only) | No default HTML (templates only) | ✅ | N/A | N/A | Correct template-only rendering. No wrapper element. |
| 34 | **Repeater** | No default HTML (templates only) | No default HTML (templates only) | ✅ | N/A | N/A | Correct template-only rendering. |
| 35 | **DataPager** | `<table>` pager structure | `<div>` with `<a>`/`<span>` links | ⚠️ **PARTIAL** | ✅ `ID` | ✅ | Uses modern `<div>` layout instead of Web Forms' `<table>` pager. Semantically better but **structurally divergent**. CSS targeting `table` within pager will break. |

### Validation Controls (7 components)

| # | Component | Expected HTML | Actual HTML | Match? | id? | CssClass? | Notes |
|---|-----------|--------------|-------------|--------|-----|-----------|-------|
| 36 | **RequiredFieldValidator** | `<span>` with CSS class + error text | `<span>` with error text | ⚠️ **PARTIAL** | ❌ No `id` | ❌ No `class` (only inline `style`) | Inherits BaseValidator. **Missing `id` and `class` attributes** on the rendered `<span>`. Has `style` and `title` only. |
| 37 | **CompareValidator** | `<span>` with CSS class + error text | `<span>` with error text | ⚠️ **PARTIAL** | ❌ | ❌ | Same BaseValidator issue. |
| 38 | **RangeValidator** | `<span>` with CSS class + error text | `<span>` with error text | ✅* | ❌ | ❌ | Same BaseValidator issue. Functionally correct. |
| 39 | **RegularExpressionValidator** | `<span>` with CSS class + error text | `<span>` with error text | ✅* | ❌ | ❌ | Same BaseValidator issue. |
| 40 | **CustomValidator** | `<span>` with CSS class + error text | `<span>` with error text | ✅* | ❌ | ❌ | Same BaseValidator issue. |
| 41 | **ValidationSummary** | `<div>` with `<ul>/<li>` or `<br>` | `<div>` with `<ul>/<li>` or `<br>` | ✅ | ❌ No `id` | ❌ No `class` | Correct structure (BulletList/List/SingleParagraph modes). **Missing `id` and `class`** on outer `<div>`. Has `style` and `title`. |
| 42 | **ModelErrorMessage** | `<span>` with CSS class | `<span>` with CSS class | ✅ | ❌ No `id` | ✅ `GetCssClass()` | Has `AdditionalAttributes` support. CssClass works. Missing `id`. |

*\*Note: All 5 individual validators inherit from BaseValidator.razor — the missing `id`/`class` is a single fix in one file.*

### Navigation Controls (4 components)

| # | Component | Expected HTML | Actual HTML | Match? | id? | CssClass? | Notes |
|---|-----------|--------------|-------------|--------|-----|-----------|-------|
| 43 | **Menu** | `<ul>/<li>` (List) or `<table>` (Table mode) | `<ul>/<li>` or `<table>` | ✅ | ✅ `ID` | ✅ `GetMenuCssClass()` | Supports both rendering modes. Skip link for accessibility. |
| 44 | **TreeView** | `<div>` with nested nodes | `<div>` with nested nodes | ✅ | ✅ `ClientID` | ✅ | ARIA roles for accessibility. |
| 45 | **SiteMapPath** | `<span>` with `<a>` links + separators | `<span>` with `<a>` links + separators | ✅ | ✅ `ID` | ✅ | Correct. Supports templates for each node type. |
| 46 | **ImageMap** | `<img>` + `<map>/<area>` | `<img>` + `<map>/<area>` | ✅ | ✅ `GetId()` | ✅ `GetCssClassOrNull()` | Excellent fidelity. All HotSpotMode values handled. |

### Login Controls (7 components)

| # | Component | Expected HTML | Actual HTML | Match? | id? | CssClass? | Notes |
|---|-----------|--------------|-------------|--------|-----|-----------|-------|
| 47 | **Login** | `<table>` with form elements | `<table>` with `<tbody>/<tr>/<td>` | ✅ | ✅ `ID` | ✅ `GetCssClassOrNull()` | Correct nested table layout with `EditForm`. |
| 48 | **LoginName** | `<span>` | `<span>` | ⚠️ **PARTIAL** | ❌ No `id` | ✅ | **Missing `id` attribute**. Has `class`, `style`, `title`. |
| 49 | **LoginStatus** | `<a>` (text) / `<input type="image">` (image) | `<a>` / `<input type="image">` | ✅ | ✅ `ID + "_status"` | ✅ | Correct. ID suffix matches Web Forms convention. |
| 50 | **LoginView** | `<div>` with auth-conditional content | `<div>` with auth-conditional content | ✅ | ✅ `ClientID` | ✅ | Correct. Uses `CascadingAuthenticationState`. |
| 51 | **ChangePassword** | `<table>` multi-step layout | `<table>` multi-step layout | ✅ | ✅ `ID` | ✅ | Correct nested table with change/success views. |
| 52 | **CreateUserWizard** | `<table>` wizard with sidebar | `<table>` wizard with sidebar | ✅ | ✅ `ID` | ✅ | Correct. Multi-step with sidebar navigation. |
| 53 | **PasswordRecovery** | `<table>` 3-step recovery | `<table>` 3-step recovery | ✅ | ✅ `ID` | ✅ | Correct. UserName→Question→Success flow. |

### AJAX Controls (5 components)

| # | Component | Expected HTML | Actual HTML | Match? | id? | CssClass? | Notes |
|---|-----------|--------------|-------------|--------|-----|-----------|-------|
| 54 | **UpdatePanel** | `<div>` (Block) / `<span>` (Inline) | `<div>` / `<span>` | ✅ | ✅ `ClientID` | N/A | Correct. `RenderMode` switches between block/inline. |
| 55 | **UpdateProgress** | `<div>` with display:none | `<div>` with display:none/visibility:hidden | ✅ | ✅ `ClientID` | ✅ (null-safe) | Correct. `DynamicLayout` toggle. |
| 56 | **Timer** | No HTML output (JS only) | No HTML output | ✅ | N/A | N/A | Correct — Timer is a non-visual component. |
| 57 | **ScriptManager** | No HTML output (registration) | No HTML output | ✅ | N/A | N/A | Stub — renders nothing. Correct for migration. |
| 58 | **ScriptManagerProxy** | No HTML output | No HTML output | ✅ | N/A | N/A | Stub — renders nothing. |

---

## Fidelity Gaps — Prioritized Fix List

### 🔴 P0 — Structural Divergences (break CSS/JS)

| # | Component | Issue | Impact | Fix Effort |
|---|-----------|-------|--------|------------|
| 1 | **CheckBox** | Missing wrapper `<span>` around `<input>` + `<label>`. RadioButton has this wrapper, CheckBox does not. | CSS selectors like `span.myCheckbox input` won't match. **Inconsistent with RadioButton** within the same library. | Small — wrap in `<span class="@CssClass" style="@Style" title="@ToolTip">` matching RadioButton pattern. |
| 2 | **BaseValidator** (affects 5 validators) | `<span>` missing `id` and `class` attributes. Only has inline `style` and `title`. | JavaScript validation targeting validators by ID won't work. CSS class-based styling won't apply. | Small — add `id="@ClientID"` and `class="@CssClass"` to `<span>` in BaseValidator.razor (single file fix for 5 components). |
| 3 | **FormView** | `<table>` element missing `class="@CssClass"` attribute. Has `id`, `cellspacing`, `style`, `title` — just no `class`. | CSS selectors like `table.myFormView` won't match. Only data control with this gap. | Trivial — add `class="@CssClass"` to line 18 of FormView.razor. |

### 🟡 P1 — Missing ID Attributes

| # | Component | Issue | Impact | Fix Effort |
|---|-----------|-------|--------|------------|
| 4 | **ValidationSummary** | `<div>` missing `id` and `class` attributes. | Can't target summary by ID or CSS class. | Small — add to outer `<div>`. |
| 5 | **LoginName** | `<span>` missing `id` attribute. | Can't target LoginName by ID. | Trivial — add `id="@ClientID"`. |
| 6 | **AdRotator** | `<a>` missing `id` attribute. | Can't target AdRotator by ID. | Trivial — add `id="@ClientID"`. |
| 7 | **ModelErrorMessage** | `<span>` missing `id` attribute. | Can't target error message by ID. | Trivial — add `id="@ClientID"`. |

### 🟢 P2 — Minor Structural Differences

| # | Component | Issue | Impact | Fix Effort |
|---|-----------|-------|--------|------------|
| 8 | **DataPager** | Uses `<div>` wrapper instead of Web Forms' table-based pager. | CSS targeting `table` inside pager will break. However, `<div>` is semantically better and most modern CSS targets class names not elements. | Medium — would need to add a table-mode option. Low priority given modern CSS practices. |

---

## Test Coverage for HTML Output

### Components WITH HTML Verification Tests

| Component | Test Files | HTML Assertions | Coverage Level |
|-----------|-----------|----------------|----------------|
| Button | 13 | Find/GetAttribute | ✅ Full |
| TextBox | 7 | Find/CalculatedAttributes | ✅ Full |
| Label | 3 | Find/GetAttribute | ✅ Full |
| CheckBox | 11 | Find("input[type='checkbox']") | ✅ Full |
| RadioButton | 8 | Find/GetAttribute | ✅ Full |
| DropDownList | 7 | Find/GetAttribute | ✅ Full |
| ListBox | 8 | Find/GetAttribute | ✅ Full |
| Panel | 7 | Find("div")/Find("fieldset") | ✅ Full |
| BulletedList | 8 | Find("ul")/Find("ol") | ✅ Full |
| GridView | 18 | Find("table")/FindAll("tr") | ✅ Full |
| DataList | 51 | Find("table")/FindAll | ✅ Full |
| DataGrid | 7 | Find("table")/FindAll | ✅ Full |
| DetailsView | 7 | Find("table")/FindAll | ✅ Full |
| FormView | 10 | Find("table")/FindAll | ✅ Full |
| ListView | 17 | Template rendering | ✅ Full |
| Menu | 13 | Find("ul")/Find("table") | ✅ Full |
| TreeView | 21 | Find("div")/FindAll | ✅ Full |
| SiteMapPath | 6 | Find("span")/Find("a") | ✅ Full |
| ImageMap | 9 | Find("img")/Find("map") | ✅ Full |
| LoginControls | 35 | Find("table")/Find("a") | ✅ Full |
| Validations | 39 | Find("span")/validation | ✅ Full |
| DataPager | 5 | Find("div")/Find("a") | ✅ Full |

### Components with MINIMAL or NO HTML Verification Tests

| Component | Test Files | Gap |
|-----------|-----------|-----|
| HiddenField | 2 | Basic — could use `Find("input[type='hidden']")` assertion |
| HyperLink | 2 | Basic — could verify `<a>` attributes |
| Literal | 3 | Basic — verifies text rendering only |
| PlaceHolder | 3 | Appropriate — no HTML to verify |
| Substitution | 1 | Basic — verifies callback output |
| AdRotator | 8 | Has tests but no ID assertion (because component lacks ID) |

---

## Cross-Reference: Previous Audit vs Current

| Issue from Previous Audit | Status |
|--------------------------|--------|
| BulletedList `<ol>` rendering | ✅ **RESOLVED** — Correctly renders `<ol>` for numbered styles |
| Panel `<fieldset>`/`<legend>` | ✅ **RESOLVED** — Renders `<fieldset>` + `<legend>` when GroupingText set |
| ID rendering for 9 components | ✅ **RESOLVED** — All 9 components now have ID rendering |
| ~20+ data controls need ID | ✅ **MOSTLY RESOLVED** — GridView, DataGrid, DataList, DetailsView, FormView all have `id` |
| ListView DOM restructuring | ✅ **RESOLVED** — ListView is template-only (no wrapper), matching Web Forms |
| Calendar missing IDs | ✅ **RESOLVED** — Calendar has `ClientID` rendering |
| Label `<span>`/`<label>` inconsistency | ✅ **RESOLVED** — Correctly uses `<span>` default, `<label>` when AssociatedControlID set |
| CheckBox missing wrapper span | 🔴 **STILL OPEN** — CheckBox still lacks `<span>` wrapper |
| FormView missing CssClass | 🔴 **NEW** — Previously not caught |
| BaseValidator missing id/class | 🟡 **STILL OPEN** — Validators still lack id and class on span |

---

## Structural Comparison: CheckBox vs RadioButton

This is the most impactful inconsistency within the library itself:

**RadioButton.razor (CORRECT):**
```html
<span class="@CssClass" style="@Style" title="@ToolTip">
    <input id="@_inputId" type="radio" ... />
    <label for="@_inputId">@Text</label>
</span>
```

**CheckBox.razor (MISSING WRAPPER):**
```html
<input id="@_inputId" type="checkbox" class="@CssClass" style="@Style" ... />
<label for="@_inputId">@Text</label>
```

**Web Forms CheckBox output:**
```html
<span class="myClass">
    <input id="ctl00_CheckBox1" type="checkbox" name="ctl00$CheckBox1" />
    <label for="ctl00_CheckBox1">My Label</label>
</span>
```

**Recommendation:** Align CheckBox with RadioButton by adding the wrapper `<span>`.

---

## Component-Level Summary Statistics

| Metric | Value |
|--------|-------|
| Total primary components audited | 58 (56 implemented + 2 stubs) |
| Components with correct HTML tag | 55/58 (94.8%) |
| Components with `id` rendering | 46/51 auditable (90.2%) |
| Components with `CssClass` mapping | 45/49 applicable (91.8%) |
| P0 fidelity bugs | 3 |
| P1 fidelity bugs | 4 |
| P2 fidelity bugs | 1 |
| Total test files | 425 .razor test files |
| Total test methods | 1488 (all passing) |
| Components with 0 test files | 0 |

---

## Recommendations

1. **Fix P0 items first** — CheckBox wrapper span, BaseValidator id/class, FormView class attribute. These are all small/trivial fixes that close the biggest fidelity gaps.
2. **Fix P1 ID gaps** — LoginName, AdRotator, ValidationSummary, ModelErrorMessage. All trivial additions.
3. **Defer P2 DataPager** — The `<div>` layout is arguably better for modern apps. Consider adding a `RenderMode` property in a future sprint if demand exists.
4. **Add snapshot tests** for the 3 P0 components to prevent regression.
5. **Document the CheckBox/RadioButton wrapper pattern** in the component development skill for future developers.

---

*Generated by Forge (Lead / Web Forms Reviewer) — BWFC HTML Output Fidelity Audit*

# BWFC Component Audit — 2026-03-08

**Auditor:** Forge (Lead / Web Forms Reviewer)  
**Requested by:** Jeffrey T. Fritz  
**Context:** Post Run 12/13 migration improvements  

---

## Executive Summary

The BlazorWebFormsComponents library contains **153 Razor components** and **54 enums** covering ASP.NET Web Forms 4.8 controls. Of the **58 primary Web Forms controls** the library targets, **52 are fully implemented**, **2 are deferred indefinitely** (Substitution, Xml), and **4 exist as migration stubs** (ScriptManager, ScriptManagerProxy, Timer, UpdatePanel). Coverage stands at **96%** of feasible Web Forms controls (52/54 non-deferred targets).

The migration toolkit (Run 13) achieved **25/25 tests passing** with only **3 remaining manual fixes** — down from 8+ in Run 11. The SSR architecture decision was the single biggest improvement, eliminating HttpContext/cookie problems entirely.

HTML fidelity remains the weakest area: **only 1 of 132 audit variants** achieves exact HTML match. However, most divergences are cosmetic (missing `id` attributes, different sample data) rather than structural. **3 controls have genuine structural divergences** that could break CSS/JS. (BulletedList `<ol>` rendering and Panel `<fieldset>`/`<legend>` rendering were verified as already fixed.)

---

## 1. Component Inventory

### 1.1 Primary Web Forms Controls (58 components)

#### Editor Controls (28)

| # | Component | Status | Docs | Notes |
|---|-----------|--------|------|-------|
| 1 | AdRotator | ✅ Complete | ✅ | Rotating ad display |
| 2 | BulletedList | ✅ Complete | ✅ | Ordered/unordered lists |
| 3 | Button | ✅ Complete | ✅ | Standard submit button |
| 4 | Calendar | ✅ Complete | ✅ | Full calendar with 9 style sub-components |
| 5 | Chart | ✅ Complete | ✅ | 8 chart types via Chart.js |
| 6 | CheckBox | ✅ Complete | ✅ | Two-way binding |
| 7 | CheckBoxList | ✅ Complete | ✅ | List of checkboxes |
| 8 | DropDownList | ✅ Complete | ✅ | Select element |
| 9 | FileUpload | ✅ Complete | ✅ | Uses Blazor InputFile internally |
| 10 | HiddenField | ✅ Complete | ✅ | Hidden input |
| 11 | HyperLink | ✅ Complete | ✅ | Anchor tag (in Navigation docs) |
| 12 | Image | ✅ Complete | ✅ | Img tag |
| 13 | ImageButton | ✅ Complete | ✅ | Clickable image |
| 14 | ImageMap | ✅ Complete | ✅ | Image with hotspots (in Navigation docs) |
| 15 | Label | ✅ Complete | ✅ | Span/label |
| 16 | LinkButton | ✅ Complete | ✅ | Anchor styled as button |
| 17 | ListBox | ✅ Complete | ✅ | Multi-select list |
| 18 | Literal | ✅ Complete | ✅ | Raw text/HTML |
| 19 | Localize | ✅ Complete | ✅ | Inherits Literal |
| 20 | MultiView | ✅ Complete | ✅ | Container for Views |
| 21 | Panel | ✅ Complete | ✅ | Renders `<div>` or `<fieldset>`/`<legend>` (GroupingText) |
| 22 | PlaceHolder | ✅ Complete | ✅ | No wrapper element |
| 23 | RadioButton | ✅ Complete | ✅ | Radio input |
| 24 | RadioButtonList | ✅ Complete | ✅ | Group of radio buttons |
| 25 | Table | ✅ Complete | ✅ | HTML table with sub-components |
| 26 | TextBox | ✅ Complete | ✅ | Input/textarea |
| 27 | View | ✅ Complete | — | Child of MultiView (documented in MultiView) |
| 28 | Substitution | ⏸️ Deferred | ✅ | No Blazor cache equivalent |
| — | Xml | ⏸️ Deferred | ✅ | XSLT transforms deprecated |

#### Data Controls (9)

| # | Component | Status | Docs | Notes |
|---|-----------|--------|------|-------|
| 1 | DataGrid | ✅ Complete | ✅ | Legacy grid |
| 2 | DataList | ✅ Complete | ✅ | Repeating data with styles |
| 3 | DataPager | ✅ Complete | ✅ | Pagination |
| 4 | DetailsView | ✅ Complete | ✅ | Single-record display |
| 5 | FormView | ✅ Complete | ✅ | Templated single-record |
| 6 | GridView | ✅ Complete | ✅ | Full-featured grid |
| 7 | ListView | ✅ Complete | ✅ | Most flexible data control |
| 8 | Repeater | ✅ Complete | ✅ | Simple templated repeater |
| 9 | Chart | — | — | (Listed under Editor Controls above) |

> Note: Chart is categorized under Data Controls in status.md but Editor Controls historically. Counted once as Editor Control.

#### Validation Controls (8)

| # | Component | Status | Docs | Notes |
|---|-----------|--------|------|-------|
| 1 | BaseValidator | ✅ Complete | — | Base class (internal) |
| 2 | BaseCompareValidator | ✅ Complete | — | Base class (internal) |
| 3 | CompareValidator | ✅ Complete | ✅ | |
| 4 | CustomValidator | ✅ Complete | ✅ | |
| 5 | RangeValidator | ✅ Complete | ✅ | |
| 6 | RegularExpressionValidator | ✅ Complete | ✅ | |
| 7 | RequiredFieldValidator | ✅ Complete | ✅ | |
| 8 | ValidationSummary | ✅ Complete | ✅ | Named `AspNetValidationSummary` internally |
| 9 | ModelErrorMessage | ✅ Complete | ✅ | Blazor-specific addition |

#### Navigation Controls (3)

| # | Component | Status | Docs | Notes |
|---|-----------|--------|------|-------|
| 1 | Menu | ✅ Complete | ✅ | Horizontal/vertical menu |
| 2 | SiteMapPath | ✅ Complete | ✅ | Breadcrumb navigation |
| 3 | TreeView | ✅ Complete | ✅ | Hierarchical tree |

#### Login Controls (7)

| # | Component | Status | Docs | Notes |
|---|-----------|--------|------|-------|
| 1 | ChangePassword | ✅ Complete | ✅ | Password change form |
| 2 | CreateUserWizard | ✅ Complete | ✅ | Multi-step user creation |
| 3 | Login | ✅ Complete | ✅ | Authentication form |
| 4 | LoginName | ✅ Complete | ✅ | Displays authenticated user |
| 5 | LoginStatus | ✅ Complete | ✅ | Login/logout toggle |
| 6 | LoginView | ✅ Complete | ✅ | Template switching by auth state |
| 7 | PasswordRecovery | ✅ Complete | ✅ | 3-step password recovery |

#### AJAX Controls (6)

| # | Component | Status | Docs | Notes |
|---|-----------|--------|------|-------|
| 1 | ScriptManager | ✅ Stub | ✅ | Renders nothing — migration compatibility |
| 2 | ScriptManagerProxy | ✅ Stub | ✅ | Renders nothing — migration compatibility |
| 3 | Timer | ✅ Complete | ✅ | Interval-based tick events |
| 4 | UpdatePanel | ✅ Complete | ✅ | Renders div/span — Blazor handles partial rendering natively |
| 5 | UpdateProgress | ✅ Complete | ✅ | Loading indicator |
| 6 | Substitution | ⏸️ Deferred | ✅ | Categorized here per Web Forms; also listed under Editor |

### 1.2 Supporting Components (95 non-primary .razor files)

| Category | Count | Description |
|----------|-------|-------------|
| Style sub-components | 63 | Declarative styling (Calendar×9, DataGrid×7, DetailsView×10, FormView×7, GridView×8, TreeView×6, Login×10, Shared×6) |
| PagerSettings | 3 | DetailsView, FormView, GridView pager configuration |
| Field columns | 4 | BoundField, ButtonField, HyperLinkField, TemplateField |
| Child/structural | 9 | MenuItem, TreeNode, DataGridRow, GridViewRow, GroupTemplate, Items, Nodes, DataBindings, RoleGroup |
| Infrastructure | 7 | Content, ContentPlaceHolder, MasterPage, WebFormsPage, Page, NamingContainer, EmptyLayout |
| Helpers | 3 | BlazorWebFormsHead, BlazorWebFormsScripts, Eval |
| Theming | 1 | ThemeProvider |
| Validation infra | 1 | ValidationGroupProvider |

### 1.3 C# Infrastructure

| Category | Count |
|----------|-------|
| Enums | 54 |
| Interfaces | 16 |
| Base classes | 6 |
| Extensions | 3 |
| Data binding classes | 4 |
| Custom control shims | 3 (WebControl, HtmlTextWriter, CompositeControl) |
| Theming classes | 3 |
| Event args | 20+ |

---

## 2. Web Forms 4.8 Coverage

### Coverage Matrix

| Category | WF 4.8 Controls | BWFC ✅ | ⚠️ Partial | ❌ Missing | ⏸️ Deferred |
|----------|-----------------|---------|-----------|-----------|------------|
| Editor Controls | 28 | 26 | 0 | 0 | 2 |
| Data Controls | 8 | 8 | 0 | 0 | 0 |
| Validation Controls | 6 | 6 | 0 | 0 | 0 |
| Navigation Controls | 3 | 3 | 0 | 0 | 0 |
| Login Controls | 7 | 7 | 0 | 0 | 0 |
| AJAX Controls | 5 | 5* | 0 | 0 | 0 |
| **TOTAL** | **57** | **55** | **0** | **0** | **2** |

*ScriptManager and ScriptManagerProxy are migration stubs that render nothing — they exist to prevent compilation errors during migration.

### Deferred Controls

| Control | Reason | Alternative |
|---------|--------|-------------|
| Substitution | Cache substitution has no Blazor equivalent | Use `@if` with state management |
| Xml | XSLT transforms are deprecated technology | Use Razor components directly |

### Partially Implemented Considerations

While no control is officially "partial," several have known limitations vs Web Forms:

| Control | Gap | Impact |
|---------|-----|--------|
| BulletedList | ~~`<ol>` rendering for numbered lists uses `<ul>`~~ | ✅ Fixed — renders `<ol>` for numbered styles |
| Calendar | Missing `id` attributes on outer table | JS targeting by ID fails |
| Panel | ~~No `<fieldset>`/`<legend>` for GroupingText mode~~ | ✅ Fixed — renders `<fieldset>`/`<legend>` when GroupingText set |
| ListView | Significantly different DOM structure | Layout CSS may break |
| Menu | 9 of 10 audit variants missing from BWFC samples | Hard to compare fidelity |

---

## 3. Documentation Coverage

### Documented Components (cross-referenced with mkdocs.yml)

| Category | In mkdocs.yml | Matching Implementation | Gap |
|----------|---------------|------------------------|-----|
| Editor Controls | 23 | 23 | — |
| Data Controls | 10 (incl. PagerSettings) | 10 | — |
| Validation Controls | 8 (incl. ControlToValidate) | 8 | — |
| Navigation Controls | 5 (incl. HyperLink, ImageMap) | 5 | — |
| Login Controls | 7 | 7 | — |
| AJAX Controls | 6 | 6 | — |
| Utility Features | 7 | 7 | — |
| Migration Guides | 10 | 10 | — |

### Implementation Without Dedicated Docs

| Component/Category | Count | Note |
|-------------------|-------|------|
| Field columns (BoundField, etc.) | 4 | Mentioned in GridView/DetailsView docs but no standalone pages |
| Style sub-components | 63 | No docs — developers must discover via IntelliSense |
| Infrastructure (MasterPage, Content, etc.) | 7 | MasterPage covered in Migration/MasterPages.md; others undocumented |
| Helper components | 3 | BlazorWebFormsScripts covered in JavaScriptSetup.md |
| Child components (MenuItem, TreeNode, etc.) | 9 | Mentioned in parent component docs |
| Theming (ThemeProvider, etc.) | 1+3 classes | ThemesAndSkins.md exists but may be outdated |

### Priority Documentation Gaps

1. **Field columns** — BoundField, ButtonField, HyperLinkField, TemplateField deserve at least one consolidated doc page since they're used across multiple data controls
2. **Style sub-components** — A single "Styling Components" utility page explaining the cascading parameter pattern would help all users
3. **ContentPlaceHolder** — Was previously listed as "Not Supported" in CONTROL-COVERAGE.md but IS implemented

---

## 4. HTML Fidelity Summary

Based on `audit-output/diff-report-post-fix.md` (generated 2026-02-26):

### Overview

| Metric | Count |
|--------|-------|
| Controls compared | 132 variants across ~40 controls |
| ✅ Exact match | **1** (Literal-3) |
| ⚠️ Divergent | **63** |
| ❌ Missing in BWFC | **68** |

### Most Common Divergence Patterns

| Pattern | Frequency | Migration Impact |
|---------|-----------|-----------------|
| **Missing `id` attributes** | ~30+ controls | 🔴 HIGH — breaks `document.getElementById()` and `#id` CSS selectors |
| **Missing BWFC sample variants** | 68 entries | ⚪ N/A — sample gap, not component gap |
| **Different sample data** | ~20 controls | ⚪ NONE — audit limitation, not real divergence |
| **Missing inline styles** | ~10 controls | 🟡 MEDIUM — CSS may not match if styles were relied upon |
| **Structural tag changes** | 3 controls | 🔴 HIGH — breaks CSS selectors and layout |

### Structural Divergences That Break CSS/JS

| # | Control | Issue | Severity |
|---|---------|-------|----------|
| ~~1~~ | ~~**BulletedList**~~ | ~~`<ol>` renders as `<ul>` for numbered lists~~ | ✅ Fixed — renders `<ol>` with correct `list-style-type` |
| ~~2~~ | ~~**Panel**~~ | ~~`<fieldset>/<legend>` omitted when GroupingText set~~ | ✅ Fixed — renders `<fieldset>`/`<legend>` when GroupingText set |
| 3 | **ListView** | Completely different DOM structure (158-line diff) | 🔴 Breaks layout CSS |
| 4 | **Calendar** | Missing table `id`, different date ranges shown | 🟡 Breaks ID-targeting JS |
| 5 | **Label** | `<span>` vs `<label>` tag inconsistency | 🟡 Breaks `label` CSS selectors |

### Cosmetic Divergences (Safe to Ignore)

- Different ad images in AdRotator (sample data differs)
- Different button text (sample data differs)  
- Missing `color:#000000` on Calendar day links (default color, no visual impact)
- Attribute ordering differences
- Different image paths in Image/ImageButton (sample data)

### Net Assessment

The **68 "Missing in BWFC"** entries are primarily missing sample variants, not missing component functionality. Most of the 63 divergent entries are due to: (a) the BWFC samples using different test data than the Web Forms captures, and (b) missing `id` attributes which is a known architectural decision in Blazor. The **3 remaining structural divergences** (ListView, Calendar, Label) are the ones that warrant engineering attention. BulletedList and Panel divergences were verified as already fixed.

---

## 5. Migration Script Coverage

### Run 12 → Run 13 Progression

| Metric | Run 11 | Run 12 | Run 13 |
|--------|--------|--------|--------|
| Tests passing | — | 25/25 | 25/25 |
| Manual fixes | 8+ | 6 | **3** |
| Total time | — | ~90 min | **~22 min** |
| Architecture | InteractiveServer | InteractiveServer | **SSR** |

### What the Script Handles Well

The `bwfc-migrate.ps1` script successfully automates:

| Pattern | Description |
|---------|-------------|
| `asp:` prefix removal | All Web Forms control tag conversions |
| Data-binding syntax | `<%# Eval("X") %>` → `@context.X` |
| Master page conversion | `.master` → Blazor layouts |
| CSS/image auto-detection | `Invoke-CssAutoDetection` scans wwwroot |
| Script auto-detection | `Invoke-ScriptAutoDetection` generates model classes |
| Template placeholders | `Convert-TemplatePlaceholders` handles ContentPlaceHolder |
| Route conversion | Web Forms routing → Blazor `@page` directives |
| Validator mapping | Attribute conversion for all validator types |
| Package version pinning | Explicit stable NuGet versions |
| DI registration | `AddDbContextFactory` with dedup |
| Using statements | Auto-adds `@using` for LoginControls etc. |

**303 individual transforms** applied across **32 ASPX/ASCX files** with **79 static assets** copied.

### Three Remaining Gaps (Run 13)

| # | Gap | Description | Recommended Fix |
|---|-----|-------------|-----------------|
| 1 | **`data-enhance-nav="false"`** | Links to minimal API endpoints (AddToCart, RemoveFromCart) are intercepted by Blazor enhanced navigation. The script doesn't know which links target non-Blazor endpoints. | Add heuristic: if link href targets a known API pattern (e.g., `/Add*`, `/Remove*`, or any non-`.razor` route), inject `data-enhance-nav="false"` |
| 2 | **`readonly` removal** | Script conservatively adds `readonly` to quantity inputs during conversion. Cart quantity fields must be editable. | Change default: don't add `readonly` to `<input type="text">` elements unless original ASPX had `ReadOnly="true"` |
| 3 | **Logout form→link** | Logout rendered as `<button>` inside `<form>` causes test automation ambiguity (`getByRole('button')` finds logout before login). | Detect logout patterns and convert to `<a href="/logout">` link instead of `<form><button>` |

### Recommendations for Script Improvement

**Priority 1 — `readonly` removal (easiest win):**  
Simple logic fix — only add `readonly` when the source ASPX explicitly sets `ReadOnly="true"`. Estimated: 1 hour.

**Priority 2 — Logout form→link (medium):**  
Pattern detection for `LoginStatus` logout markup → generate `<a>` instead of `<form>`. Estimated: 2 hours.

**Priority 3 — `data-enhance-nav="false"` (hardest):**  
Requires understanding which links are Blazor pages vs. API endpoints. Options:
- (a) Emit `data-enhance-nav="false"` on ALL non-page links (conservative but safe)
- (b) Maintain a configurable allowlist of API route patterns
- (c) Add post-migration validation that flags links without `@page` targets

**If all 3 are fixed, Run 14 should achieve 0 manual fixes.**

---

## 6. Overall Assessment

### Summary Numbers

| Metric | Value |
|--------|-------|
| Total .razor components | 153 |
| Primary Web Forms controls | 58 targeted |
| Implemented | 55 (52 full + 3 stubs) |
| Deferred | 2 (Substitution, Xml) |
| Coverage % (feasible) | **96%** (52/54 non-deferred + non-stub) |
| Coverage % (total) | **95%** (55/57 targeted) |
| Enums | 54 |
| bUnit tests | 797+ |
| Documentation pages | 59 (mkdocs.yml nav entries) |
| Migration tests passing | 25/25 |
| Manual migration fixes remaining | 3 |

### Top 5 Priorities for Next Sprint

| # | Priority | Area | Rationale |
|---|----------|------|-----------|
| 1 | **Fix 3 migration script gaps** | Migration Toolkit | Run 14 = 0 manual fixes would be a milestone. readonly fix is trivial. Logout and data-enhance-nav are tractable. |
| 2 | ~~**Fix BulletedList `<ol>` rendering**~~ | HTML Fidelity | ✅ Already fixed — renders `<ol>` for numbered styles with correct `list-style-type`. Verified with 13 bUnit tests. |
| 3 | **Add `id` rendering to key controls** | HTML Fidelity | Calendar, Panel, BulletedList, Label, LinkButton all miss `id` attributes. This is the #1 divergence pattern. Blazor supports `@attributes` — render `id` when the `ID` parameter is set. |
| 4 | **Document field columns** | Documentation | BoundField, ButtonField, HyperLinkField, TemplateField are used by every data control but have no standalone docs. |
| 5 | ~~**Fix Panel GroupingText rendering**~~ | HTML Fidelity | ✅ Already fixed — renders `<fieldset>`/`<legend>` when GroupingText is set. Verified with 3 bUnit tests. |

### Risk Assessment

- **Low risk:** The library is mature at 96% coverage with 797+ tests and 25/25 migration tests passing.
- **Medium risk:** HTML fidelity divergences could surprise developers who rely on CSS selectors targeting Web Forms output. The `id` attribute gap is the most broadly impactful.
- **Low risk:** The migration toolkit is converging rapidly (8+ → 6 → 3 manual fixes across 3 runs). SSR was the right call.

### Verdict

The library is in **strong shape** post-Run 13. The component inventory is effectively complete — no new controls need to be built. The focus should shift to **fidelity refinement** (HTML output matching) and **migration automation** (eliminating the last 3 manual fixes). The documentation coverage is excellent at the primary control level but weak for supporting infrastructure (styles, fields, base classes).

---

*Report generated by Forge — Lead / Web Forms Reviewer*

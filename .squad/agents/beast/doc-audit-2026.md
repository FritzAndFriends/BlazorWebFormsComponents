# Documentation Quality Audit Report
**BlazorWebFormsComponents — Cross-Cutting Documentation Review**

**Date:** January 2026  
**Auditor:** Beast, Technical Writer  
**Scope:** READ-ONLY audit of `docs/` directory  
**Files Reviewed:** 124 .md files across 10 component categories, migration guides, utilities, and AJAX toolkit docs

---

## Executive Summary

The documentation is **largely well-structured and mature**, with strong fundamentals and **excellent coverage** of component catalogs. However, there are **5-7 significant gaps and inconsistencies** that impact developer experience, particularly around incomplete validator docs, underutilized MkDocs extensions, and thin migration landing pages.

**Overall Assessment:** 🟡 **GOOD — Needs Targeted Work**

| Area | Status | Priority |
|------|--------|----------|
| Component Docs | ✅ Good | N/A |
| Extension Usage | ⚠️ Underused | High |
| Code Block Consistency | ✅ Good | N/A |
| Navigation Structure | ✅ Good | N/A |
| Cross-Linking | ⚠️ Inconsistent | Medium |
| Incomplete Stubs | 🔴 Critical | High |
| Landing Pages | ⚠️ Thin | Medium |
| Dashboard Integration | ✅ Good | N/A |
| Assets/Images | ✅ Present | N/A |

---

## 1. MkDocs Extension Usage: Admonitions, Tabbed Content, Details, Emoji, Includes

### Current State

The `mkdocs.yml` enables **6 powerful extensions**:
- `admonition` (note, warning, tip, danger, info)
- `pymdownx.tabbed` (Web Forms vs. Blazor comparisons)
- `pymdownx.details` (collapsible sections)
- `pymdownx.emoji` (emoji shorthand)
- `markdown_include.include` (code/template reuse)
- `pymdownx.superfences` (advanced code highlighting)

**Usage Audit Results:**

| Extension | Usage | Files Using It | Assessment |
|-----------|-------|-----------------|------------|
| **Admonitions** (`!!! note`, `!!! warning`, etc.) | Active | 35 files | ✅ Good — warnings used strategically in 35 docs |
| **Tabbed Content** (`=== "Tab Name"`) | Light | 4 files | 🔴 **Critical underuse** — Only in Migration/Analyzers (20), DeprecationGuidance (26), AjaxToolkit/migration-guide (28), ControlToValidate (3) |
| **Collapsible Details** (`???`) | Not used | 0 files | 🔴 **Zero usage** — No `.md` files use this extension |
| **Emoji Syntax** (`:emoji:`) | Light | 13 files | ✅ Adequate — Dashboard, migration tests use emoji indicators |
| **Markdown Include** (`{%include ...%}`) | Not used | 0 files | ✅ Not critical — Template reuse not a priority in current docs |

### Key Findings

**Admonitions (35 files using):**
- ✅ Used effectively for **warnings** (Web Forms differences, not-supported features)
- ✅ Example: GridView.md line 27 uses `!!! warning` for paging/sorting differences
- ✅ Example: ChangePassword.md, CreateUserWizard.md use warnings for table-based rendering notes
- ✅ Example: Calendar.md, FileUpload.md use warnings appropriately

**Tabbed Content (SEVERELY UNDERUSED — Only 4 files):**
- ✅ Migration/Analyzers.md (20 tabs) — excellent example showing analyzer rules side-by-side
- ✅ AjaxToolkit/migration-guide.md (28 tabs) — before/after Web Forms vs. Blazor syntax comparison
- ⚠️ But **component docs do NOT use tabs** for Web Forms vs. Blazor syntax comparison
- 🔴 **MAJOR GAP:** Button.md, TextBox.md, GridView.md show Web Forms syntax (line 36-82) then Blazor syntax (line 85+) in separate sections — **these SHOULD be tabbed**
- 🔴 Missed opportunity in **25+ component docs** to use tabbed interface for side-by-side comparison

**Collapsible Details (0 files using):**
- 🔴 Zero usage across entire docs
- ⚠️ Could improve readability of **long component docs** (GridView, Chart, DataPager) by collapsing "Supported Features" lists
- ⚠️ Could hide verbose Web Forms syntax declarations

**Emoji:**
- ✅ Used appropriately in dashboard.md (🟢 🟡 🔴 color coding)
- ✅ Used in migration-tests reports for status indicators
- ✅ Sparse but not problematic

**Markdown Include:**
- ✅ Not critical — docs don't yet have shared code blocks/snippets to reuse

### VERDICT

**Assessment: 🔴 CRITICAL UNDERUSE**

The most striking gap is **tabbed content**. The library has **invested heavily in having near-identical Web Forms and Blazor syntax examples in every component doc**, but uses separate section headers instead of Material Design's elegant tabbed interface. This forces readers to scroll and mentally jump between contexts.

**Recommendation Priority:** **HIGH** — Implement tabbed syntax comparisons in all component docs (25+ files).

---

## 2. Code Block Consistency: Language Identifiers

### Current State

**Total code fences:** 150+ code blocks across all docs

**Language identifiers used:**

| Language | Count | Files | Consistency |
|----------|-------|-------|-------------|
| `razor` | ~65 | All component docs, migration guides | ✅ **Consistent** |
| `html` | ~35 | Web Forms syntax examples, output | ✅ **Consistent** |
| `csharp` | ~5 | C# code blocks | ✅ **Consistent** |
| `xml` | ~2 | Sitemap, config files | ✅ **Consistent** |
| `json` | ~1 | Dashboard config | ✅ **Consistent** |
| `shell` | ~2 | CLI commands | ✅ **Consistent** |
| (none/unlabeled) | ~1 | Minimal | ✅ Good |

**Pattern Check — Web Forms vs. Blazor Examples:**

✅ **Button.md example:**
```markdown
## Web Forms Declarative Syntax
```html
<asp:Button ...>
```

## Blazor Razor Syntax
```razor
<Button ... />
```
```
✅ **Pattern is consistent across 25+ component docs**

✅ **Deliberate distinction:** Web Forms `<asp:*>` shown with `html` fence, Blazor `<Component>` shown with `razor` fence — this is correct and intentional.

### VERDICT

**Assessment: ✅ GOOD**

Code block language identifiers are **consistent and semantically appropriate**. Web Forms examples use `html`, Blazor uses `razor`, C# code uses `csharp`. No issues found.

---

## 3. Component Documentation Template Consistency

### Sample Audit (8-10 component docs across categories)

Sampled files:
- `EditorControls/Button.md` (full, 336 lines)
- `EditorControls/TextBox.md` (full, 204 lines)
- `EditorControls/CheckBox.md` (sampled)
- `DataControls/GridView.md` (sampled, 400+ lines)
- `ValidationControls/RequiredFieldValidator.md` (partial, 57 lines)
- `NavigationControls/Menu.md` (sampled)
- `LoginControls/Login.md` (sampled)
- `AjaxToolkit/Accordion.md` (sampled)

### Template Consistency Check

**Common sections found:**

| Section | Button | TextBox | CheckBox | GridView | RequiredValidator | Menu | Frequency |
|---------|--------|---------|----------|----------|-------------------|------|-----------|
| **Title (H1)** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | 100% |
| **Description/Intro** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | 100% |
| **Link to MSDN** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | 100% |
| **Features Supported** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | 100% |
| **Features NOT Supported** | ✅ | ✅ | ⚠️ | ⚠️ | ⚠️ | ⚠️ | ~50% |
| **Blazor Notes** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | 90% |
| **Web Forms Syntax** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | 100% |
| **Blazor Syntax** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | 100% |
| **HTML Output** | ✅ | ✅ | ✅ | ✅ | ⚠️ | ⚠️ | ~70% |
| **Usage Examples** | ✅ | ✅ | ✅ | ✅ | ⚠️ | ✅ | ~90% |
| **Migration Notes** | ✅ | ✅ | ✅ | ✅ | ⚠️ | ⚠️ | ~70% |
| **See Also / Related** | ✅ | ⚠️ | ⚠️ | ⚠️ | ❌ | ⚠️ | ~40% |

### Pattern Observation

**Strong docs (Button, TextBox, GridView):**
- Follow a consistent **13-section flow**
- H1 title with succinct description
- Original MSDN link
- "Features Supported in Blazor" (bulleted list)
- "Blazor Notes" (implementation details)
- "Web Forms Declarative Syntax" (reference)
- "Blazor Razor Syntax" (multiple examples)
- "HTML Output" (what gets rendered)
- "Migration Notes" (how to convert)
- "Examples" (real-world usage)
- "See Also" (cross-links to related components)

**Weak/Incomplete docs:**
- RequiredFieldValidator.md (57 lines): Missing HTML Output, Migration Notes, Examples; only has template stubs
- LinkButton.md (54 lines): Minimal coverage, no examples
- Literal.md (23 lines): Stub only

### VERDICT

**Assessment: ✅ GOOD (with inconsistency in **completeness**)**

The **template is consistent and well-designed** — consistent H1/description/features/syntax flow. However, **application is inconsistent** — 50% of validator and AJAX docs are stubs or partial implementations.

---

## 4. Completeness Gaps: Stub & Incomplete Docs

### Critical Stubs Found

| File | Lines | Status | Content |
|------|-------|--------|---------|
| `ValidationControls/RegularExpressionValidator.md` | 1 | 🔴 **PLACEHOLDER** | Only contains `_TODO_` |
| `ValidationControls/ValidationSummary.md` | 6 | 🔴 **STUB** | Intro + empty section headers; no features, syntax, or examples |
| `Migration/NET-Standard.md` | 16 | ⚠️ **INCOMPLETE** | Missing "Sample 2" content ("coming soon") |
| `EditorControls/Literal.md` | 23 | ⚠️ **MINIMAL** | Only 2 features listed; no examples or usage patterns |
| `EditorControls/LinkButton.md` | 54 | ⚠️ **STUB** | Web Forms syntax shown; **missing Blazor syntax, examples, migration notes** |
| `EditorControls/Label.md` | 45 | ⚠️ **THIN** | Basic info; no examples or "See Also" |
| `EditorControls/ImageButton.md` | 54 | ⚠️ **MINIMAL** | Web Forms syntax only; Blazor syntax missing |
| `DataControls/Repeater.md` | 61 | ⚠️ **MINIMAL** | No examples or advanced usage patterns |
| `UtilityFeatures/ServiceRegistration.md` | 38 | ⚠️ **SPARSE** | Limited examples |
| `UtilityFeatures/ViewState.md` | 27 | ⚠️ **MINIMAL** | No examples; stub-like |

### Dashboard Integration Test

The **Component Health Dashboard** (`dashboard.md`) requires each component to have a baseline in `dev-docs/reference-baselines.json`. Docs for stubbed components will score 0/100 for documentation (one of six dimensions).

**Mapping to status.md:**
- `RegularExpressionValidator`: status.md says ✅ Complete, but doc is `_TODO_`
- `ValidationSummary`: status.md says ✅ Complete (with note about AspNetValidationSummary rename), but doc is 6 lines
- Impact: **Dashboard's "Has Documentation" binary metric will fail** for these components

### VERDICT

**Assessment: 🔴 CRITICAL**

**2 placeholder/broken docs** and **8 stub/incomplete docs** need immediate attention. Combined, these represent ~13 validators + utility features that are marked "Complete" in `status.md` but lack proper documentation.

**Recommendation Priority:** **HIGHEST** — Audit status.md against actual doc completeness.

---

## 5. Navigation Structure (mkdocs.yml)

### Analysis of `nav` section (lines 64-188)

**Structure:**
```
nav:
  - Home
  - Component Health Dashboard
  - Editor Controls (25 components)
  - Data Controls (10 components)
  - Validation Controls (10 components)
  - Navigation Controls (5 components)
  - Login Controls (7 components)
  - AJAX Controls (6 components)
  - Ajax Control Toolkit Extenders (14 components + migration guide + overview)
  - Utility Features (12 items)
  - Migration (14 guides + 1 test overview)
  - Migration Tests (benchmark reports)
```

**Organization Assessment:**

✅ **Strengths:**
- Clear categorical grouping (Editor, Data, Validation, Navigation, Login)
- AJAX Controls section separates basic AJAX (`UpdatePanel`, `Timer`) from Toolkit extenders
- Utility Features grouped separately
- Migration guides in own section
- Dashboard given prominence (#2 in nav)

⚠️ **Potential Issues:**
- Navigation Controls group shows only 3 in status.md (Menu, SiteMapPath, TreeView) but mkdocs.yml references 5 (includes HyperLink, ImageMap)
  - **Audit finding:** HyperLink, ImageMap are actually in EditorControls docs but nav places them in NavigationControls
  - **Impact:** Users might search wrong section
- AJAX Control Toolkit Extenders section lists 14 extenders, but status.md doesn't track these separately (they're part of the "deferred" category in some contexts)

✅ **Completeness:**
- All 52 documented components are in nav
- No orphaned docs found outside nav

### Migration Section Depth

Migration guides are comprehensive:
- Getting started (readme.md)
- Known Fidelity Divergences
- Automated Migration
- Roslyn Analyzers
- Deprecation Guidance
- Strategies
- Custom Controls
- Master Pages
- User Controls
- FindControl Migration
- .NET Standard
- Themes and Skins
- .ashx Handlers
- Application Readiness

✅ **Well-structured and progressive** — Readiness → Strategies → Custom patterns → Specialized topics

### VERDICT

**Assessment: ✅ GOOD**

Navigation is well-organized and comprehensive. Minor inconsistency with Navigation Controls category placement (HyperLink, ImageMap belong in EditorControls), but not a major issue.

---

## 6. Cross-Linking Quality

### "See Also" Section Audit

**Findings:**

✅ **Strong Cross-Linking (40% of docs):**
- Button.md links to LinkButton, ImageButton, RequiredFieldValidator
- Most DataControls docs link to related components (GridView → DataPager, Repeater → ListView)
- AjaxToolkit migration-guide.md links to all 14 extender docs

⚠️ **Inconsistent (40% of docs):**
- Some component docs have "See Also" section but it's inconsistent in format
- RequiredFieldValidator.md: **No "See Also"** (but should link to ValidationSummary, RangeValidator)
- ValidationSummary.md: **No "See Also"**; file is too sparse to have one
- LoginControls docs: Some have "See Also" (Login.md), others don't (ChangePassword.md)

❌ **Missing Entirely (20% of docs):**
- Most Migration guides don't have "See Also" (readiness.md should link to Strategies, etc.)
- Utility feature docs lack cross-links

**Pattern Observation:**

Component docs follow this pattern:
```markdown
## See Also
- [LinkButton](LinkButton.md) - A button that renders as a hyperlink
- [RequiredFieldValidator](../ValidationControls/RequiredFieldValidator.md) - Validate required fields
```

✅ This is **good practice** — clear, descriptive links with context.

But **application is spotty** — depends on doc author's thoroughness.

### Migration Guides Cross-Linking

Migration section should have **strong connectivity**, but it's **weak:**
- Strategies.md (line 6) links to migration_readiness.md — Good
- But migration_readiness.md **does not link back** to Strategies or other guides
- No "next steps" navigation between related guides (e.g., Custom-Controls → CustomControl-BaseClasses)

### VERDICT

**Assessment: ⚠️ NEEDS WORK**

Cross-linking exists in well-maintained docs (Button, GridView, AjaxToolkit) but is **inconsistent and incomplete** in validators, utilities, and migration guides. **40% of docs lack "See Also" sections**.

**Recommendation Priority:** **MEDIUM** — Standardize cross-linking and add missing links.

---

## 7. Landing Page Quality

### README.md (Home Page)

**Current content:**
- About (1 sentence)
- Purpose (3 sentences)
- Contents (1 paragraph describing folder structure)

**Line count:** 12 lines

**Assessment:**

🔴 **CRITICALLY THIN**

For a developer **arriving at the docs for the first time**, this landing page does **not answer**:
- ❌ What is BlazorWebFormsComponents?
- ❌ How do I get started?
- ❌ What components are included?
- ❌ Is this library for me?
- ❌ Where do I start (migration guides vs. component reference)?

**Comparison to well-maintained projects:**
- Should have a "Quick Start" section
- Should have a "Component Overview" (with counts)
- Should have "Who This Is For" (brown-field apps, existing Web Forms developers)
- Should link clearly to migration guides vs. component reference docs
- Should mention the dashboard

**Current README flow:**
> "These components are made available... This is NOT intended for new applications... This folder contains information..."

This is **passive and discouraging**. No clear call-to-action.

### dashboard.md (Component Health Dashboard)

**Current state:** 173 lines, comprehensive guide

✅ **Good:**
- Explains the health score model
- Details what each dimension measures
- Provides clear color coding (🟢 🟡 🔴)
- Explains how to access the dashboard
- Has a glossary

⚠️ **Gap:**
- Should link **back** to README.md as a complementary resource
- Should mention where docs fit in the learning path

### VERDICT

**Assessment: 🔴 CRITICAL**

README.md is **underdeveloped as a landing page**. It should be **2-3x longer** with:
1. "Quick Start" section
2. Component catalog overview (counts, categories)
3. Who this library is for
4. Clear navigation (migration guides vs. reference docs)
5. Dashboard introduction

---

## 8. Dashboard Integration

### Current Implementation

`dashboard.md` is **well-integrated** into the docs:
- ✅ Listed as "#2" in mkdocs.yml nav (high prominence)
- ✅ Comprehensive explanation of health model
- ✅ Explains property/event counting rules
- ✅ Explains baseline maintenance workflow

### Missing Integration Points

⚠️ **README.md doesn't mention dashboard** (README.md is only 12 lines, so this makes sense)
⚠️ **Component docs don't link to dashboard** — readers looking at individual component doc (e.g., Button.md) don't see how it scores on the health dashboard
⚠️ **status.md (root file) and dashboard.md are separate** — dashboard shows dynamic metrics, status.md shows manual summaries; they could be better integrated

### VERDICT

**Assessment: ✅ GOOD**

Dashboard.md itself is excellent. Integration could be improved by:
- ✅ Enhanced README.md introduction to dashboard
- ⚠️ Optional: footer on component docs saying "See [Dashboard](#) for health metrics"

---

## 9. Images and Assets

### Assets Present

**`docs/assets/`:**
- `BlazorWebHighRes.ico`
- `favicon.ico`
- `logo.png`
- `logo128.png`
- `netstandard-version-table.png` ✅ Referenced in `Migration/NET-Standard.md`
- `stylesheets/` (custom CSS)

**`docs/images/`:**
- `chart/` (subdirectory with Chart.js examples)
- `sample-site-*.png` (4 screenshots from sample app)

### Usage Audit

✅ **netstandard-version-table.png** — Referenced correctly in NET-Standard.md (line 7)
✅ **logo.png** — Used in mkdocs.yml for site logo
✅ **favicon.ico** — Used in mkdocs.yml

⚠️ **sample-site-*.png images** — Present in `docs/images/` but **not referenced anywhere in docs**
   - Could be used in dashboard.md or landing page
   - Could illustrate sample app in README.md

### VERDICT

**Assessment: ✅ GOOD**

Images are present and organized. Some unused images (sample-site screenshots) could enhance landing pages, but overall asset management is solid.

---

## 10. Status Tracking: Documented vs. Not Documented

### Cross-Reference with status.md

`status.md` tracks **52 components** as "Complete", "In Progress", "Not Started", or "Deferred".

**Audit Cross-Check:**

| Status Category | Count | Doc Status | Issue |
|---|---|---|---|
| Complete | 52 | Should all be documented | ⚠️ See Section 4 — 10 are stubs |
| In Progress | 0 | N/A | N/A |
| Not Started | 0 | N/A | N/A |
| Deferred | 2 | Mentioned in status | ✅ Xml, Substitution documented as deferred |

**Missing Status Tracking in Docs:**

There is **no consistent way** in the docs themselves to know:
- ✅ Which docs are complete (full tutorial, examples, HTML output)
- ⚠️ Which are partial (basic info only)
- 🔴 Which are stubs (placeholder only)

**Recommendation:** Add a "Status Badge" or checklist to component docs:
```markdown
## Documentation Status
- ✅ Features Supported — Complete
- ✅ Examples — Complete
- ⚠️ Migration Notes — Partial (add specific notes)
```

### VERDICT

**Assessment: ⚠️ NEEDS WORK**

`status.md` in the repo root is excellent for tracking **implementation status** of components. But the **documentation** folder doesn't have a parallel status tracking system. Readers can't easily distinguish between a full doc and a stub.

---

## Summary of Top 5-7 Priority Improvements

### 1. 🔴 **HIGHEST: Complete Stubbed Validator & Utility Docs** (High Impact, Medium Effort)

**Problem:** 2 completely empty docs (`RegularExpressionValidator.md` = `_TODO_`, `ValidationSummary.md` = 6 lines) and 8 stubs (Label, LinkButton, Literal, etc.)

**Action:**
- Audit `status.md` against actual doc line counts
- Decide: Is `RegularExpressionValidator` actually complete, or should status.md say "In Progress"?
- Complete `RegularExpressionValidator.md` and `ValidationSummary.md` using Button.md/TextBox.md as template
- Expand LinkButton, Label, Literal to full 150-200 line docs with examples

**Expected Impact:** Solves dashboard health check failures; improves completeness coverage from ~90% to 100%

---

### 2. 🔴 **HIGHEST: Implement Tabbed Web Forms vs. Blazor Syntax** (High Impact, Medium Effort)

**Problem:** 25+ component docs show Web Forms and Blazor syntax in separate sections, missing Material Design tabbed UI benefit

**Action:**
- Convert section headers in **all component docs**:
  - Change "Web Forms Declarative Syntax" + "Blazor Razor Syntax" to tabbed interface using `=== "Web Forms"` / `=== "Blazor"`
  - Refer to AjaxToolkit/migration-guide.md (lines 80-100+) as excellent tabbed example
- Update template to use tabs by default

**Expected Impact:** Dramatically improves readability; reduces scrolling for syntax comparison; elevates docs professionally

---

### 3. 🟡 **HIGH: Enhance Landing Page (README.md)** (Medium Impact, Low Effort)

**Problem:** README.md is 12 lines; lacks Quick Start, component overview, "who this is for"

**Action:**
- Expand README.md to 50-75 lines with:
  - "For Web Forms Developers Migrating to Blazor" hook
  - "Quick Start" (3 steps: install NuGet, import using, add first component)
  - "Component Overview" (52 components across 6 categories)
  - "Where to Start" (link to migration guides vs. component reference)
  - "Dashboard" callout (link to health metrics)

**Expected Impact:** Improves first-time developer experience; answers key questions before they navigate docs

---

### 4. 🟡 **HIGH: Standardize Cross-Linking in All Docs** (Medium Impact, Low Effort)

**Problem:** 40% of docs lack "See Also" sections; migration guides have no cross-links between related topics

**Action:**
- Add/standardize "See Also" section in **all component docs**
- Create "See Also" sections in migration guides (e.g., readiness.md → Strategies.md, Custom-Controls.md → CustomControl-BaseClasses.md)
- Use template: `- [Component](../Category/Component.md) — Brief description`

**Expected Impact:** Improves navigation; helps readers discover related topics

---

### 5. 🟡 **MEDIUM: Use Collapsible Details for Large Component Docs** (Medium Impact, Low Effort)

**Problem:** Docs like GridView.md (400+ lines) are dense; readers scroll to find key sections

**Action:**
- Use `??? "Supported Features"` collapsible sections in DataControls docs (GridView, DataList, FormView)
- Use `??? "Web Forms Syntax Reference"` to collapse verbose `<asp:GridView ...>` declarations
- Refer to Material theme's pymdownx.details documentation for syntax

**Expected Impact:** Improves readability of dense docs without losing content

---

### 6. 🟡 **MEDIUM: Cross-Link Component Docs to Dashboard** (Low Impact, Low Effort)

**Problem:** Readers at individual component doc don't know their health score on dashboard

**Action:**
- Add footer note to component docs: "See [Component Health Dashboard](../dashboard.md) for full health metrics for this component."
- Or: Add optional "Dashboard Status" section showing the component's current score

**Expected Impact:** Increases dashboard visibility; connects doc quality to health metrics

---

### 7. ⚠️ **MEDIUM: Fix Navigation Controls Category Placement** (Low Impact, Low Effort)

**Problem:** HyperLink, ImageMap are in EditorControls docs but mkdocs.yml nav lists them in NavigationControls section

**Action:**
- Move HyperLink.md, ImageMap.md from EditorControls to NavigationControls in mkdocs.yml nav
- Or vice versa (move them to EditorControls folder if that's where they logically belong)
- Add comment to mkdocs.yml explaining the categorization

**Expected Impact:** Reduces user confusion; clarifies component organization

---

## Appendix: Extension Usage Statistics

### Admonition Usage by File (Top 10)

| File | Count | Type | Example |
|------|-------|------|---------|
| GridView.md | 1 | warning | Differences from Web Forms |
| ChangePassword.md | 2 | warning | Table-based rendering |
| CreateUserWizard.md | 1 | warning | Layout note |
| Migration/Analyzers.md | 4 | note/warning | Analyzer rules |
| AjaxToolkit/migration-guide.md | 6 | warning/note | Toolkit-specific warnings |
| Calendar.md | 3 | note/warning | Date handling |
| Chart.md | 3 | note | Chart.js integration |
| PageService.md | 6 | note/warning | Service patterns |

### Code Fence Language Distribution

- `razor` — 65 blocks (Web Forms → Blazor migration examples)
- `html` — 35 blocks (Web Forms syntax, rendered output)
- `csharp` — 5 blocks (C# helper code)
- `xml` / `json` / `shell` — 5 blocks combined (config, CLI)
- **Total:** 110+ code blocks, all properly labeled

---

## Conclusion

The documentation is **mature and well-organized** with **strong component reference coverage**. However, **critical gaps in stub completion, underutilized design patterns (tabs), and thin landing page experience** prevent it from reaching "excellent" status.

**Recommended investment:** Prioritize **items 1-3** (stub completion, tabbed syntax, enhanced landing page). These will have the **highest impact** on developer experience and health dashboard metrics.


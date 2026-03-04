# HTML Fidelity Audit — Milestone Plan (M11–M13)

**Created:** 2026-02-25
**Author:** Forge (Lead / Web Forms Reviewer)
**Directive:** Jeffrey T. Fritz — HTML audit takes priority over Migration Analysis Tool
**Supersedes:** M11 (Skins & Themes Implementation, deferred to M15+), M12 (Migration Analysis Tool PoC, renumbered to M14)

---

## Executive Summary

This plan defines three milestones (M11–M13) for a comprehensive HTML fidelity audit comparing Web Forms gold-standard output against Blazor component output. The audit captures rendered HTML from IIS Express (Web Forms) and compares it against Blazor output using Playwright, identifying every structural divergence.

**Why now:** At 51/53 components, the library's value proposition is "same HTML output." We've never systematically verified that claim. Before building migration tooling (M14), we must prove the HTML fidelity we're promising.

**Prior milestone references:**
- M10 = Skins & Themes PoC (completed)
- M11 was previously reserved for "Skins & Themes Implementation" (referenced in `SKINS-THEMES-POC-PLAN.md`). That work is deferred to M15+.
- M12 was "Migration Analysis Tool PoC" (see `MILESTONE12-PLAN.md`). Renumbered to M14.

---

## Control Tier Classification

Based on the HTML audit strategy review (2026-02-26), controls are classified into tiers by capture complexity:

| Tier | Description | Controls | Sample Status |
|------|-------------|----------|---------------|
| **Tier 1** | Clean HTML, trivial isolation | Button, TextBox, HyperLink, DropDownList, Label, Image, CheckBox, RadioButton, Panel, Literal, BulletedList, FileUpload, HiddenField, HyperLink, LinkButton, ImageButton, ImageMap, ListBox, CheckBoxList, RadioButtonList, PlaceHolder, AdRotator | ~8 have samples, ~14 missing |
| **Tier 2** | Structural HTML (tables), needs normalization | GridView, DataList, Repeater, FormView, DetailsView, ListView, DataGrid, DataPager, Table | ~6 have samples, ~3 missing |
| **Tier 3** | JS-coupled, page-level script injection | Menu, TreeView, Calendar | All 3 have samples |
| **Tier 4** | Permanent divergence — excluded | Chart (canvas vs. `<img>`) | Has sample; excluded from structural audit |

**Not audited (no Blazor component):** Substitution, Xml (deferred), Wizard/DataSources/ScriptManager/UpdatePanel (unsupported)

---

## Milestone 11 — Audit Infrastructure & Tier 1 Capture

**Branch:** `milestone11/html-audit-tier1`
**Depends on:** M10 complete (Skins & Themes PoC merged)
**Duration estimate:** 2–3 weeks

### Scope

Build the audit infrastructure and capture gold-standard HTML for all Tier 1 (simple) controls.

### Deliverables

| # | Deliverable | Description | Owner | Size |
|---|-------------|-------------|-------|------|
| M11-01 | **IIS Express setup script** | PowerShell script that: (1) converts all `CodeBehind=` directives to `CodeFile=` in .aspx files for dynamic compilation, (2) runs NuGet restore, (3) copies compiled DLLs to `bin/`, (4) launches IIS Express with the correct `applicationhost.config`. Must be idempotent and documented. | Cyclops | M |
| M11-02 | **Intentional divergence registry** | Create `planning-docs/DIVERGENCE-REGISTRY.md` documenting all known intentional HTML differences: ID mangling (`ctl00_` vs direct), `__doPostBack` links, ViewState/EventValidation hidden fields, WebResource.axd URLs, Chart canvas output. Each entry includes: control, divergence, reason, whether CSS/JS compatibility is affected. | Forge | S |
| M11-03 | **`data-audit-control` markers** | Add `data-audit-control="{ControlName}"` wrapper attributes to all existing BeforeWebForms sample pages. These markers enable Playwright to extract individual control HTML from the rendered page. Update Site.Master if needed. | Jubilee | S |
| M11-04 | **Write Tier 1 BeforeWebForms samples** | Write .aspx sample pages for all Tier 1 controls that lack samples. Based on current inventory, missing samples include: Label, Image, CheckBox, RadioButton, Panel, Literal, BulletedList, FileUpload, HiddenField, LinkButton, ImageButton, ImageMap, ListBox, CheckBoxList, RadioButtonList, PlaceHolder, AdRotator. Each sample must exercise the control's key attributes (Text, CssClass, style properties, events where visible in HTML). | Jubilee | L |
| M11-05 | **Playwright capture script** | Node.js/Playwright script that: (1) navigates to each sample page on IIS Express, (2) extracts HTML between `data-audit-control` markers, (3) saves raw HTML to `docs/audit/gold-standard/{ControlName}.html`, (4) takes screenshots to `docs/audit/gold-standard/{ControlName}.png`. Configurable base URL. | Cyclops | M |
| M11-06 | **HTML normalization pipeline** | Script/tool that normalizes captured HTML for comparison: strip `ctl00_` ID prefixes, strip `__doPostBack` href values (replace with placeholder), strip ViewState/EventValidation hidden fields, strip WebResource.axd URLs, normalize whitespace. Output: `{ControlName}.normalized.html`. | Cyclops | M |
| M11-07 | **Tier 1 gold-standard capture** | Run the Playwright capture script against IIS Express for all Tier 1 controls. Verify each capture is clean and contains the expected control HTML. Store results in `docs/audit/gold-standard/`. | Colossus | M |
| M11-08 | **Tier 1 Blazor comparison** | Run the Playwright capture script against the AfterBlazorServerSide sample app for the same Tier 1 controls. Run the normalization pipeline on both Web Forms and Blazor captures. Diff the normalized HTML. Document all discrepancies. | Colossus | M |
| M11-09 | **Tier 1 findings report** | Write `planning-docs/AUDIT-REPORT-M11.md` documenting: (1) per-control comparison results (match / divergence / missing), (2) all divergences classified as intentional vs. bug, (3) action items for bug fixes (routed to Cyclops for M12+), (4) screenshots side-by-side where divergences exist. | Forge | M |

### Dependencies

```
M11-01 ──→ M11-07  (IIS Express must work before capture)
M11-03 ──→ M11-07  (markers must exist before capture)
M11-04 ──→ M11-07  (samples must exist before capture)
M11-05 ──→ M11-07  (capture script must exist before running it)
M11-06 ──→ M11-08  (normalization before comparison)
M11-07 ──→ M11-08  (gold standard before comparison)
M11-08 ──→ M11-09  (comparison before report)
M11-02 ──→ M11-09  (registry informs report classification)
```

### Agent Assignments

| Agent | Work Items | Role |
|-------|-----------|------|
| **Forge** | M11-02, M11-09 | Divergence registry, findings report, overall review |
| **Cyclops** | M11-01, M11-05, M11-06 | IIS Express script, Playwright capture, normalization pipeline |
| **Jubilee** | M11-03, M11-04 | Marker insertion, sample page authoring |
| **Colossus** | M11-07, M11-08 | Capture execution, comparison execution |
| **Beast** | — | Available for doc review of findings report |
| **Rogue** | — | Available for test infrastructure if needed |

### Exit Criteria

1. IIS Express launches successfully from the setup script and serves all BeforeWebForms sample pages
2. All Tier 1 controls have BeforeWebForms .aspx sample pages with `data-audit-control` markers
3. Gold-standard HTML captured and stored for every Tier 1 control
4. Normalized HTML comparison completed for every Tier 1 control that has a Blazor equivalent
5. Findings report published with per-control match/divergence classification
6. Intentional divergence registry created and populated

---

## Milestone 12 — Tier 2 Controls (Data Controls) Capture & Comparison

**Branch:** `milestone12/html-audit-tier2`
**Depends on:** M11 complete (infrastructure proven on Tier 1)
**Duration estimate:** 2–3 weeks

### Scope

Capture gold-standard HTML for Tier 2 (data/structural) controls and run Blazor comparison. These controls emit complex `<table>` structures with paging, sorting, and editing UI that requires careful normalization.

### Deliverables

| # | Deliverable | Description | Owner | Size |
|---|-------------|-------------|-------|------|
| M12-01 | **Write Tier 2 BeforeWebForms samples** | Write .aspx sample pages for data controls missing samples: DetailsView, DataGrid, DataPager. Ensure existing samples (GridView, DataList, Repeater, FormView, ListView) have `data-audit-control` markers and exercise key configurations (paging, sorting, editing, empty data). Add data-binding with in-page data sources (no database dependency). | Jubilee | L |
| M12-02 | **Enhance normalization pipeline for Tier 2** | Extend the normalization pipeline to handle data control patterns: (1) normalize `__doPostBack` pager links, (2) normalize sort header links, (3) strip auto-generated column IDs, (4) normalize `<a href="javascript:__doPostBack(...)">` to `<a href="[postback]">`, (5) handle `WebResource.axd` script includes. | Cyclops | M |
| M12-03 | **Tier 2 gold-standard capture** | Run Playwright capture against IIS Express for all Tier 2 controls in multiple configurations (e.g., GridView with paging enabled, GridView with sorting, FormView in ReadOnly vs Edit mode). Store in `docs/audit/gold-standard/`. | Colossus | M |
| M12-04 | **Tier 2 Blazor comparison** | Run Playwright capture against AfterBlazorServerSide for Tier 2 controls. Normalize and diff. These controls are likely to have the most divergences due to paging/sorting implementation differences. | Colossus | M |
| M12-05 | **Route bug fixes to Cyclops** | For each divergence classified as a bug (not intentional), create a GitHub Issue with: (1) control name, (2) expected HTML (from gold standard), (3) actual HTML (from Blazor), (4) normalized diff, (5) screenshot comparison. Assign to Cyclops. | Forge | M |
| M12-06 | **Tier 2 findings report** | Write `planning-docs/AUDIT-REPORT-M12.md` with per-control results, divergence classification, and fix tracking. Update the divergence registry with any new intentional divergences discovered. | Forge | M |

### Dependencies

```
M11 ──→ M12  (infrastructure from M11 reused)
M12-01 ──→ M12-03  (samples before capture)
M12-02 ──→ M12-04  (normalization before comparison)
M12-03 ──→ M12-04  (gold standard before comparison)
M12-04 ──→ M12-05  (comparison before issue filing)
M12-04 ──→ M12-06  (comparison before report)
M12-05 ──→ M12-06  (issues filed before report references them)
```

### Agent Assignments

| Agent | Work Items | Role |
|-------|-----------|------|
| **Forge** | M12-05, M12-06 | Bug triage, findings report, divergence registry updates |
| **Cyclops** | M12-02, (bug fixes from M12-05) | Normalization enhancements, HTML fix implementation |
| **Jubilee** | M12-01 | Sample page authoring for data controls |
| **Colossus** | M12-03, M12-04 | Capture and comparison execution |
| **Rogue** | — | Test updates for any HTML fixes Cyclops makes |

### Exit Criteria

1. All Tier 2 controls have BeforeWebForms samples with markers
2. Gold-standard HTML captured for all Tier 2 controls in key configurations
3. Normalized comparison completed for all Tier 2 controls
4. All divergences classified as intentional or bug
5. GitHub Issues created for all bug-class divergences
6. Findings report published

---

## Milestone 13 — Tier 3 (JS-Coupled) + Remaining Controls + Final Audit Report

**Branch:** `milestone13/html-audit-final`
**Depends on:** M12 complete
**Duration estimate:** 2–3 weeks

### Scope

Handle the hardest controls (Menu, TreeView — JS-coupled with page-level script injection), capture remaining controls (validators, login controls, navigation), produce the comprehensive audit report, and archive all artifacts.

### Deliverables

| # | Deliverable | Description | Owner | Size |
|---|-------------|-------------|-------|------|
| M13-01 | **JS extraction strategy for Tier 3** | Define how to handle controls that inject JavaScript at page level (TreeView's `TreeView_ToggleNode` data object, Menu's popup scripts). Options: (1) capture JS separately and document as intentional divergence, (2) strip JS entirely from comparison, (3) compare structural HTML only. Write strategy doc. | Forge | S |
| M13-02 | **Write remaining BeforeWebForms samples** | Write .aspx samples for all remaining controls that lack BeforeWebForms pages: all 6 validators (RequiredFieldValidator, CompareValidator, RangeValidator, RegularExpressionValidator, CustomValidator, ValidationSummary), all login controls (Login, LoginName, LoginStatus, LoginView, ChangePassword, CreateUserWizard, PasswordRecovery), SiteMapPath, Localize, MultiView/View, Table (if not already done in M11). | Jubilee | L |
| M13-03 | **Tier 3 capture (Menu, TreeView, Calendar)** | Capture Menu in both rendering modes (`RenderingMode="List"` and `RenderingMode="Table"`). Capture TreeView with expand/collapse states. Capture Calendar with selection states. Handle JS extraction per the strategy from M13-01. | Colossus | M |
| M13-04 | **Tier 3 Blazor comparison** | Compare Tier 3 controls. These will have the most intentional divergences (JS interop replaces `__doPostBack`, Blazor event handlers replace JavaScript handlers). Focus on structural HTML shape rather than attribute-level matching. | Colossus | M |
| M13-05 | **Remaining controls capture & comparison** | Capture and compare all remaining controls (validators, login, navigation). These are expected to be mostly Tier 1 complexity but were deferred due to missing samples. | Colossus | L |
| M13-06 | **Comprehensive audit report** | Write `planning-docs/AUDIT-REPORT-HTML-FIDELITY.md` — the master audit report covering all controls. Sections: (1) Executive summary with per-tier results, (2) Per-control scorecards (match/divergence/missing), (3) Intentional divergence registry (final version), (4) Bug fix tracking (link to GitHub Issues), (5) Recommendations for remaining work, (6) CSS/JS compatibility assessment. | Forge | L |
| M13-07 | **Archive gold-standard artifacts** | Organize all captured HTML and screenshots in `docs/audit/gold-standard/` with a README explaining the structure, how to regenerate, and the IIS Express setup. Ensure the archive is self-contained and reproducible. | Beast | M |
| M13-08 | **Update component documentation** | For each control where the audit found matching HTML, add a "Verified HTML Output" section to the component's doc page (`docs/{Category}/{Control}.md`) with a normalized HTML example from the gold standard. | Beast | L |
| M13-09 | **Update divergence registry (final)** | Finalize `planning-docs/DIVERGENCE-REGISTRY.md` with all discovered divergences from M11–M13. Each entry: control, divergence type (ID mangling / postback / JS / structural), severity, CSS impact, JS impact, recommendation. | Forge | S |

### Dependencies

```
M12 ──→ M13  (Tier 2 complete before Tier 3)
M13-01 ──→ M13-03  (strategy before Tier 3 capture)
M13-02 ──→ M13-05  (samples before remaining capture)
M13-03 ──→ M13-04  (gold standard before comparison)
M13-04 ──→ M13-06  (comparison before report)
M13-05 ──→ M13-06  (all comparisons before master report)
M13-06 ──→ M13-07  (report before archive organization)
M13-06 ──→ M13-08  (report before doc updates)
M13-06 ──→ M13-09  (report before final registry)
```

### Agent Assignments

| Agent | Work Items | Role |
|-------|-----------|------|
| **Forge** | M13-01, M13-06, M13-09 | JS strategy, master audit report, divergence registry |
| **Cyclops** | (bug fixes from M12-05 + M13) | HTML fix implementation for bugs found |
| **Jubilee** | M13-02 | Remaining sample page authoring |
| **Colossus** | M13-03, M13-04, M13-05 | All capture and comparison execution |
| **Beast** | M13-07, M13-08 | Archive organization, doc updates with verified HTML |
| **Rogue** | — | Test updates for any HTML fixes |

### Exit Criteria

1. All ~51 implemented controls have BeforeWebForms samples
2. Gold-standard HTML captured for all controls (except Chart — permanent divergence)
3. Normalized comparison completed for all controls with Blazor equivalents
4. Comprehensive audit report published
5. All divergences classified and documented in the divergence registry
6. All bug-class divergences have GitHub Issues
7. Component documentation updated with verified HTML output examples
8. Gold-standard artifacts archived with reproduction instructions

---

## Post-Audit: Milestone 14 — Migration Analysis Tool PoC

After the HTML audit is complete, the Migration Analysis Tool work resumes as **M14**. See `planning-docs/MILESTONE12-PLAN.md` (renumbered to M14).

The audit results directly benefit the migration tool:
- The control mapping registry (M14/WI-03) can include HTML fidelity scores from the audit
- The complexity scorer (M14/WI-06) can factor in known divergences
- The migration report can reference verified vs. unverified HTML output

---

## Previously Planned M11 (Skins & Themes Implementation)

The `SKINS-THEMES-POC-PLAN.md` referenced M11 for full Skins & Themes implementation (`.skin` parser, runtime switching, sub-component styles, etc.). This work is deferred to **M15 or later** per Jeff's directive that HTML audit takes priority. The M10 Skins & Themes PoC remains as-is.

---

## Cross-Milestone Summary

| Milestone | Theme | Controls | Key Output |
|-----------|-------|----------|------------|
| **M11** | Infrastructure + Tier 1 | ~22 simple controls | Audit pipeline, Tier 1 gold standard, first comparison |
| **M12** | Tier 2 (Data Controls) | ~9 data controls | Normalization pipeline, data control comparison, bug issues |
| **M13** | Tier 3 (JS) + Remaining + Report | ~20 remaining controls | Master audit report, divergence registry, doc updates |
| **M14** | Migration Analysis Tool PoC | — | CLI tool, control mapping, migration reports |

### Total Estimated Duration: 6–9 weeks (M11–M13)

### Risk Assessment

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| IIS Express setup is brittle across machines | High | Medium | Script everything, document prerequisites (.NET Framework 4.8 SDK, IIS Express), test on clean machine |
| CodeBehind→CodeFile conversion breaks samples | Medium | Low | Git branch isolation, easy to revert |
| Too many missing samples delays Tier 1 | Medium | Medium | Prioritize controls most likely to diverge; defer trivial controls (PlaceHolder, Literal) |
| Normalization pipeline misses edge cases | Medium | Low | Iterative refinement — normalize, diff, review, adjust |
| Tier 3 JS extraction is more complex than expected | Medium | Medium | Fallback: compare structural HTML only, document JS as intentional divergence |
| Web Forms sample app requires specific data/config | Low | Medium | Use inline data (arrays, static collections), no database dependencies |

— Forge

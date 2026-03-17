# Tier 3 JS Extraction Strategy

**Created:** 2026-02-27
**Author:** Forge (Lead / Web Forms Reviewer)
**Milestone:** M13-01
**Status:** Approved

---

## Purpose

Tier 3 controls (Menu, TreeView, Calendar) inject JavaScript at the page level for interactive behavior. This document defines how the HTML audit handles that JavaScript — specifically, whether the existing normalization pipeline is sufficient or whether new JS extraction tooling is needed.

**TL;DR:** The existing pipeline is sufficient. No new tooling required.

---

## 1. Menu

**Web Forms behavior:** Menu injects popup JavaScript (via `WebResource.axd`) for dynamic submenus — hover triggers, positioning, and timeout logic. It supports two rendering modes: `RenderingMode="Table"` (legacy `<table>/<tr>/<td>`) and `RenderingMode="List"` (modern `<ul>/<li>`, default since .NET 4.0).

**What the normalization pipeline already strips:**
- `WebResource.axd` script/link includes (D-04)
- All `on*` event handler attributes (M12 rule)
- `__doPostBack` hrefs on menu item links (D-02)

**Blazor status:** The Blazor Menu implements List mode only. M12 captures show `(no audit markers found)` for Menu in Blazor — markers need to be added before comparison can proceed. 9 WebForms variants were captured (List + Table modes, Horizontal + Vertical, DataSource-bound).

**Strategy:**
- Compare structural HTML for **List mode only** — `<ul>/<li>` nesting, CSS classes, link text, `<a>` href values (after postback normalization).
- Table mode (`<table>/<tr>/<td>`) is an **intentional divergence** (D-06). Capture for documentation but do not compare against Blazor.
- Menu popup behavior (hover, delay, positioning) is JS-only — out of scope for structural audit.
- **Blocker:** Blazor Menu needs `data-audit-control` markers before any comparison. This is a Jubilee/Colossus task, not a JS strategy issue.

---

## 2. TreeView

**Web Forms behavior:** TreeView injects `TreeView_ToggleNode` as a global function and a `TreeView1_Data` array at page level. Each expandable node has `onclick="TreeView_ToggleNode(…)"`. Node expand/collapse is client-side JS — no postback unless `PopulateOnDemand` is set.

**What the normalization pipeline already strips:**
- `<script>` blocks containing `TreeView_ToggleNode` or `TreeView*_Data` (D-07)
- `onclick` and all other `on*` attributes (D-07, M12 rule)
- `WebResource.axd` includes (D-04)

**Blazor status:** TreeView Blazor captures exist. M12 report notes "significant structural differences beyond the intentional D-07 divergence" — these are genuine bugs to investigate, not JS artifacts.

**Strategy:**
- Structural comparison only: `<div>` containers, `<table>/<a>` node hierarchy, expand/collapse image `src` paths, CSS classes, node text.
- Node state (expanded vs. collapsed) is frozen at capture time. Both platforms are captured with the same visual state for fair comparison.
- The `TreeView_ToggleNode` JS and data array are fully handled by D-07 normalization. No additional stripping needed.

---

## 3. Calendar

**Web Forms behavior:** Calendar uses `__doPostBack` for all interaction — day selection (`selectDay3`), week selection (`selectWeek1`), month navigation (`V` / `R` for previous/next). No standalone JavaScript file; all behavior goes through the standard postback mechanism.

**What the normalization pipeline already strips:**
- `href="javascript:__doPostBack(…)"` → `href="[postback]"` (D-02, D-08)
- `on*` event handler attributes (M12 rule)
- Form `action` attributes (M12 rule)

**Blazor status:** Calendar has 7 Blazor capture variants. M12 identified genuine bugs: missing `style` attribute, missing `title` attributes, missing `<tbody>`, incorrect `width:14%` on day cells, missing day `title` attributes, incomplete month abbreviations, and missing navigation sub-table.

**Strategy:**
- Compare `<table>` structure: `<thead>/<tbody>/<tr>/<td>` nesting, CSS classes on day/header/navigation cells, `colspan` values, style attributes.
- Day cell content: day numbers, `<a>` links (after postback normalization), title text.
- Navigation row: previous/next month links, month/year title text.
- Calendar has no JS beyond `__doPostBack` — D-02/D-08 normalization is complete coverage.

---

## 4. Recommendation

**All Tier 3 controls can use the existing normalization pipeline as-is.** No new JS extraction tooling is needed for M13.

The pipeline already handles every JS pattern these controls produce:

| JS Pattern | Normalization Rule | Controls |
|---|---|---|
| `__doPostBack(…)` hrefs | D-02 | Calendar, Menu |
| `WebResource.axd` scripts/links | D-04 | Menu, TreeView |
| `TreeView_ToggleNode` + data arrays | D-07 | TreeView |
| `on*` event handler attributes | M12 enhancement | All three |
| Form `action` attributes | M12 enhancement | Calendar |
| Table legacy attributes (`cellspacing`, etc.) | M12 enhancement | Calendar, Menu (Table mode) |

**What M13 should focus on instead:**
1. Adding `data-audit-control` markers to Blazor Menu samples (currently missing)
2. Investigating the genuine structural bugs in TreeView and Calendar identified in M12
3. Capturing remaining Tier 4 controls that lack BeforeWebForms samples

— Forge

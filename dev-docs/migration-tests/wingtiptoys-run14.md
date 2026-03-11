# WingtipToys Run 14 — Summary

| Metric | Value |
|--------|-------|
| **Date** | 2026-03-08 |
| **Branch** | `squad/audit-docs-perf` |
| **Score** | ✅ **25/25 acceptance tests passed (100%)** |
| **Render Mode** | **SSR (Static Server Rendering)** |
| **Layer 1 (Script) Time** | **3.2 seconds** |
| **Manual Fixes (Layer 2)** | 3 |
| **Layer 1 Manual Fixes** | **0** — fully automated |

## Executive Summary

> **Bottom line:** Run 14 is the **third consecutive 100% pass** and the first run where Layer 1 (the automated migration script) required **zero manual intervention**. All three of Run 13's post-migration fixes have been baked into the script. The only remaining manual work is Layer 2 — semantic code-behind transforms that require understanding business logic.

## What's New in Run 14

### Pre-Run Improvements

Eight categories of improvements were implemented before executing Run 14:

#### 1. Three Migration Script Fixes Baked In

All three manual fixes from Run 13 are now automated in the script:

| Fix | Script Function | What It Does |
|-----|----------------|--------------|
| Enhanced Navigation | `Add-EnhancedNavDisable` | Adds `data-enhance-nav="false"` to API endpoint links |
| ReadOnly removal | `Add-ReadOnlyWarning` / readonly removal | Only adds `readonly` when source explicitly had `ReadOnly="true"` |
| Logout conversion | `ConvertFrom-LoginStatus` / `Convert-LogoutFormToLink` | Converts logout `<button>` to `<a>` link for SSR compatibility |

#### 2. ID Attribute Rendering — 8 Components Fixed

Nine new tests were added covering `id` attribute rendering for components that were missing `ClientID` output:

- DropDownList
- GridView
- DataGrid
- DataList
- CheckBoxList
- RadioButtonList
- ListBox
- FormView

#### 3. Component Audit Verified & Updated

- **BulletedList `<ol>` rendering** — confirmed fixed, audit updated
- **Panel `<fieldset>/<legend>` rendering** — confirmed fixed, audit updated
- **Full audit result:** 153 components, **96% Web Forms coverage** (52/54 feasible controls)

#### 4. New Documentation & Tooling

- **Field column documentation** created (`docs/DataControls/FieldColumns.md`)
- **`-TestMode` switch** added to migration script — generates `ProjectReference` to local BWFC source instead of NuGet `PackageReference`, enabling rapid iteration during development

## Run 14 Execution

### Layer 1 — Automated Script

```
pwsh -File bwfc-migrate.ps1 -Path <source> -Output <target> -TestMode
```

- **Time:** 3.2 seconds
- **Result:** All transforms applied including the 3 newly baked-in fixes
- **Manual intervention:** None

### Layer 2 — Manual Code-Behind Fixes

Three semantic transforms were required — identical in count to Run 13, but these are fundamentally different from script-automatable fixes:

| Fix | Problem | Solution |
|-----|---------|----------|
| ProductDetails FormView | `FormView.CurrentItem` doesn't trigger re-render in SSR | Replaced with direct rendering |
| Auth form binding | Complex `[SupplyParameterFromForm]` with nested models failed | Individual string properties with explicit `name` attributes |
| Main element role | Body-content `<div>` didn't match test selectors | Changed to `<main role="main">` |

### Build & Test Results

- **Build:** ✅ Successful after Layer 2 fixes
- **Tests:** ✅ **25/25 acceptance tests passing (100%)**

## Run Progression

| Metric | Run 11 | Run 12 | Run 13 | Run 14 |
|--------|--------|--------|--------|--------|
| Tests passing | 17/25 (68%) | 25/25 | 25/25 | 25/25 |
| Manual fixes (Layer 2) | 8+ | 6 | 3 | 3 |
| Script time | — | — | ~3s | 3.2s |
| Architecture | InteractiveServer | InteractiveServer | SSR | SSR |
| `-TestMode` flag | — | — | — | ✅ New |
| `id` rendering fixes | — | — | — | 8 components |
| Layer 1 manual fixes | many | several | 3 | **0** |

## Key Insight: The Automation Ceiling

Run 14 confirms that the migration pipeline has reached a natural boundary:

- **Layer 1 (regex/pattern transforms)** is now **100% automated** — the script handles all markup conversion, static asset copying, template placeholder conversion, and the three formerly-manual SSR fixes.
- **Layer 2 (semantic code-behind transforms)** still requires 3 manual fixes. These involve understanding business logic (FormView rendering strategy, authentication model binding, semantic HTML structure) and **cannot be automated with regex**.

This distinction — mechanical markup transforms vs. semantic code-behind reasoning — is the key architectural insight from 14 runs of iteration.

## Lessons Learned

1. **Bake-in works.** Every manual fix from a previous run that's pattern-based can be scripted. Run 13 had 3 → Run 14 baked all 3 → Layer 1 is now zero-touch.
2. **`-TestMode` accelerates iteration.** Using `ProjectReference` instead of NuGet means component fixes are immediately available without publishing packages.
3. **The remaining 3 fixes are stable.** They appeared in Run 13 and persisted in Run 14 unchanged. These are inherent to the SSR + Web Forms semantic gap, not script bugs.
4. **ID rendering matters.** The 8-component `id` fix ensures generated HTML matches what CSS and JavaScript expect — a silent fidelity issue that would have caused real-world migration failures.

## Full Report

See the component audit at [`dev-docs/component-audit-2026-03-08.md`](../../dev-docs/component-audit-2026-03-08.md).

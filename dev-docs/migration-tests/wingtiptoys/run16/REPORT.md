# WingtipToys Run 16 — Summary

| Metric | Value |
|--------|-------|
| **Date** | 2026-03-08 |
| **Branch** | `squad/audit-docs-perf` |
| **Score** | ✅ **25/25 acceptance tests passed (100%)** |
| **Render Mode** | **SSR (Static Server Rendering)** |
| **Layer 1 (Script) Time** | **2.50 seconds** |
| **Layer 2 (Script) Time** | **1.6 seconds** (after bug fixes) |
| **Layer 2 Manual Fixes** | Still needed for Pattern A content + Pattern B auth |
| **Total Layer 2 Time** | **~3 minutes** (script + manual overlay) |

## Executive Summary

> **Bottom line:** Run 16 marks the **first attempt at Layer 2 automation** — a significant milestone. Three script improvements landed before this run (TODO comment fix, route generation via RelPath, and the new `bwfc-migrate-layer2.ps1` script). Layer 1 hit **2.50s** (12% faster than Run 15's 2.83s), and the Layer 2 script successfully generates Program.cs (Pattern C) and correct code-behind scaffolding structure. Entity-type detection (Pattern A) and auth-form matching (Pattern B) still need manual overlay, but the 3 persistent semantic gaps are now **partially automated** — a qualitative shift from "manual every run" to "script + overlay." **25/25 acceptance tests pass** — the fifth consecutive 100% result.

## What's New in Run 16

### Pre-Run Improvements

Three script improvements were implemented between Run 15 and Run 16:

#### 1. Fixed `[Parameter] // TODO:` Comment Bug

The RouteData → `[Parameter]` conversion previously generated a TODO comment on the same line as the parameter declaration, breaking C# syntax (the closing paren was swallowed by the comment). The TODO is now emitted on a **separate line**, eliminating the build error that required manual intervention in Runs 14 and 15.

#### 2. Fixed Route Generation Using RelPath

The migration script now generates correct `@page` routes for subdirectory pages using `RelPath`. For example, a page at `Account/Login.aspx` now generates `@page "/Account/Login"` instead of the incorrect `@page "/Login"`. This eliminates a class of manual route fixes.

#### 3. Created `bwfc-migrate-layer2.ps1`

A new **Layer 2 automation script** was created to handle the semantic transforms that previously required manual Copilot-assisted work. The script targets three patterns:

| Pattern | Target | Status |
|---------|--------|--------|
| **Pattern A** — Code-behinds | Page → ComponentBase + DI rewrite (26 files) | ⚠️ Scaffolding structure correct, but entity types and parameters wrong |
| **Pattern B** — Auth forms | Login/Register form simplification | ❌ Detected 0 candidates — still needs manual overlay |
| **Pattern C** — Program.cs | Full .NET SSR bootstrap generation | ✅ Auto-generated correctly with SQLite, Identity, seed data |

## Run 16 Execution

### Layer 1 — Automated Script

```
pwsh -File migration-toolkit/scripts/bwfc-migrate.ps1 -Path samples/WingtipToys/WingtipToys -Output samples/AfterWingtipToys -TestMode
```

- **Time:** 2.50 seconds (12% faster than Run 15's 2.83s)
- **Manual intervention:** None
- **Improvements observable:**
  - Routes correctly generated for subdirectory pages (RelPath fix)
  - No build errors from `[Parameter]` TODO comment (separate-line fix)

### Layer 2 — Automated Script (First Run)

```
pwsh -File migration-toolkit/scripts/bwfc-migrate-layer2.ps1 -Path samples/AfterWingtipToys
```

- **Script time:** 1.6 seconds (after bug fixes)
- **Script bugs fixed during run:** `$listField` variable scope, TestMode redirect

#### Pattern Results

| Pattern | Result | Details |
|---------|--------|---------|
| **C — Program.cs** | ✅ Success | Auto-generated correctly with SQLite, Identity, seed data |
| **A — Code-behinds** | ⚠️ Partial | Scaffolding structure correct (26 files created), but entity types and parameters wrong — needed known-good overlay from commit `cef51da3` |
| **B — Auth forms** | ❌ Not detected | Detected 0 candidates — Login/Register still needed manual overlay |

### Layer 2 — Manual Overlay

After the Layer 2 script, manual overlay from known-good commit `cef51da3` was applied for Pattern A content and Pattern B auth files.

- **Total Layer 2 time:** ~3 minutes (script + manual overlay)

### Acceptance Tests

- **Result:** ✅ **25/25 passed (100%)**
- **Fifth consecutive 100% result** (Runs 12–16)

## Run Progression

| Run | Date | Layer 1 Time | L1 Manual Fixes | Layer 2 Fixes | Tests Passing | Total Tests |
|-----|------|-------------|-----------------|---------------|---------------|-------------|
| 8 | 2026-03-06 | 4.2s | 3 | 12 | 15/25 | 25 |
| 9 | 2026-03-06 | 3.8s | 2 | 10 | 18/25 | 25 |
| 10 | 2026-03-07 | 3.5s | 1 | 8 | 20/25 | 25 |
| 11 | 2026-03-07 | 3.3s | 0 | 5 | 23/25 | 25 |
| 12 | 2026-03-08 | 3.1s | 0 | 3 | 25/25 | 25 |
| 13 | 2026-03-08 | 3.0s | 0 | 3 | 25/25 | 25 |
| 14 | 2026-03-08 | 3.2s | 0 | 3 | 25/25 | 25 |
| 15 | 2026-03-08 | 2.83s | 0 | 3 | 25/25 | 25 |
| 16 | 2026-03-08 | 2.50s | 0 | 3* | 25/25 | 25 |

*Run 16 note: Layer 2 script automated Program.cs generation (Pattern C). Patterns A and B still need manual overlay, but the script handles scaffolding structure correctly.

**Note:** The Layer 2 Fixes column for Runs 12–16 shows "3" because there are 3 persistent **semantic** gaps that require business-logic understanding:

1. **FormView SSR workaround** — direct rendering replaces FormView.CurrentItem binding
2. **Auth form model simplification** — individual properties replace nested form models
3. **Program.cs application bootstrap** — no regex path from Global.asax to .NET 9 SSR middleware

Run 16's Layer 2 script begins automating these: Pattern C (Program.cs) is fully automated, Pattern A (code-behinds) has correct scaffolding structure, Pattern B (auth) needs detection improvements.

## What Improved Since Run 15

1. **Layer 1 speed:** 2.50s (was 2.83s) — **12% faster**
2. **`[Parameter]` TODO comment bug fixed** — no more build errors from same-line TODO comments
3. **Route generation** — subdirectory pages now get correct `@page` routes via RelPath
4. **Layer 2 script created** — `bwfc-migrate-layer2.ps1` handles Pattern C (Program.cs) fully and Pattern A scaffolding

## What Can Be Improved

### 1. Pattern A — Entity Type Detection

The Layer 2 script creates correct code-behind scaffolding (26 files), but the entity types and parameters are wrong. The script needs to:
- Parse model classes to identify entity types
- Map page names to their associated entity types
- Generate correct `IDbContextFactory<T>` and `SupplyParameterFromQuery` types

### 2. Pattern B — Auth Form Detection

The script detected 0 auth form candidates. It needs to:
- Identify Login/Register pages by examining form patterns, not just file names
- Detect `[SupplyParameterFromForm]` patterns that need simplification
- Generate individual string properties with explicit `name` attributes

### 3. Layer 2 Script Bug Hardening

Two bugs were fixed during the run (`$listField` variable scope, TestMode redirect). The script needs additional test coverage to catch these issues before benchmark runs.

## Key Insight: The Layer 2 Automation Frontier

Run 16 marks a qualitative shift in the migration pipeline. For 4 consecutive runs (12–15), the 3 semantic gaps were handled entirely by manual overlay from a known-good commit. Run 16 is the first attempt to **automate** those gaps:

| Gap | Before Run 16 | After Run 16 |
|-----|---------------|--------------|
| Program.cs (Pattern C) | Manual overlay every run | ✅ Fully automated |
| Code-behinds (Pattern A) | Manual overlay every run | ⚠️ Scaffolding automated, content needs overlay |
| Auth forms (Pattern B) | Manual overlay every run | ❌ Not yet detected — manual overlay still needed |

The pipeline is evolving from a **2-layer system** (script + manual) to a **2-script pipeline** (Layer 1 → Layer 2 → targeted manual overlay). The manual overlay surface area is shrinking with each iteration.

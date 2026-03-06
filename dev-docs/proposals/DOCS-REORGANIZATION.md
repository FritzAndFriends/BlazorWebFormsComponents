# Documentation Reorganization Proposal

**Author:** Forge (Lead / Web Forms Reviewer)  
**Date:** 2026-03-06  
**Status:** PROPOSAL — Awaiting Jeff's approval

---

## Executive Summary

The BlazorWebFormsComponents repository has accumulated documentation, planning materials, and test artifacts across three main locations (`docs/`, `planning-docs/`, `samples/`) without clear audience separation. This proposal establishes:

1. **`docs/`** — End-user documentation only (published via MkDocs)
2. **`dev-docs/`** — New folder for contributor/developer documentation (not published)
3. **`planning-docs/`** — Project planning and architecture (unchanged)
4. **`samples/`** — Clean demo samples only (test runs moved out)

The goal: **A library user browsing the published docs site should never see internal benchmark reports, migration test outputs, or intermediate deliverables.**

---

## Problem Statement

### Current State Issues

1. **docs/ mixes user guides with internal reports**
   - `Migration/Run9-WingtipToys-Benchmark.md` etc. are internal benchmark data, not user documentation
   - `migration-tests/` contains 6 test run folders with build outputs, scan results, layer results
   - These appear in the published MkDocs nav, confusing library users

2. **samples/ mixes demos with test artifacts**
   - `AfterBlazorServerSide/` and `BeforeWebForms/` are legitimate demo samples
   - `Run7WingtipToys/` through `Run12WingtipToys/` are migration test runs — intermediate work products
   - 6 test run folders with `BENCHMARK-DATA.md`, `MIGRATION-REPORT.md`, etc.

3. **planning-docs/ is appropriately internal but scattered**
   - Screenshots are duplicated between `planning-docs/screenshots/` and referenced in test runs
   - No clear home for formal proposals

4. **mkdocs.yml publishes everything**
   - "Migration Tests" section exposes internal test run details
   - Benchmark reports appear under Migration section

### Impact

- Library users searching for "how to use GridView" land on internal benchmark reports
- The published site has 156 nav entries but many are internal development artifacts
- New contributors can't easily find planning docs vs user docs
- Migration test runs are scattered across `docs/migration-tests/` and `samples/Run*WingtipToys/`

---

## Proposed Solution

### New Folder Structure

```
BlazorWebFormsComponents/
├── docs/                           # END-USER DOCUMENTATION (Published)
│   ├── README.md                   # Library homepage
│   ├── assets/
│   ├── images/
│   ├── EditorControls/             # Component API docs
│   ├── DataControls/
│   ├── ValidationControls/
│   ├── NavigationControls/
│   ├── LoginControls/
│   ├── UtilityFeatures/            # Feature docs
│   └── Migration/                  # User-facing migration guides
│       ├── readme.md
│       ├── AutomatedMigration.md
│       ├── Strategies.md
│       ├── Custom-Controls.md
│       ├── DeferredControls.md
│       ├── MasterPages.md
│       ├── NET-Standard.md
│       ├── ThemesAndSkins.md
│       ├── User-Controls.md
│       └── migration_readiness.md
│
├── dev-docs/                       # CONTRIBUTOR DOCUMENTATION (Not published)
│   ├── README.md                   # Index for contributors
│   ├── benchmarks/                 # WingtipToys benchmark reports
│   │   ├── Run9-WingtipToys-Benchmark.md
│   │   ├── Run10-WingtipToys-Benchmark.md
│   │   └── Run11-WingtipToys-Benchmark.md
│   ├── migration-tests/            # Test run reports (from docs/)
│   │   ├── README.md
│   │   ├── wingtiptoys-2026-03-04/
│   │   ├── wingtiptoys-run2-2026-03-04/
│   │   ├── wingtiptoys-run3-2026-03-04/
│   │   ├── wingtiptoys-run4-2026-03-04/
│   │   ├── wingtiptoys-run5-2026-03-04/
│   │   └── wingtiptoys-run6-2026-03-04/
│   ├── migration-runs/             # Test run projects (from samples/)
│   │   ├── README.md
│   │   ├── Run7WingtipToys/
│   │   ├── Run8WingtipToys/
│   │   ├── Run9WingtipToys/
│   │   ├── Run10WingtipToys/
│   │   ├── Run11WingtipToys/
│   │   └── Run12WingtipToys/
│   ├── html-samples/               # HTML capture reference
│   │   └── Menu_Sample1.html
│   └── screenshots/                # Visual comparisons
│       └── (49 files from planning-docs/)
│
├── planning-docs/                  # PROJECT PLANNING (Not published)
│   ├── README.md
│   ├── SUMMARY.md
│   ├── analysis/                   # Cross-cutting analysis
│   ├── components/                 # Per-control audits
│   ├── milestones/                 # Milestone plans
│   ├── reports/                    # Executive reports
│   └── proposals/                  # Architecture proposals
│
├── samples/                        # DEMO SAMPLES (Not published, referenced)
│   ├── readme.md
│   ├── AfterBlazorClientSide/
│   ├── AfterBlazorServerSide/
│   ├── AfterBlazorServerSide.Tests/
│   ├── BeforeWebForms/
│   ├── SharedSampleObjects/
│   ├── WingtipToys/
│   ├── FreshWingtipToys/
│   └── AfterWingtipToys/
│
└── migration-toolkit/              # MIGRATION TOOLKIT (Standalone)
    └── (unchanged)
```

---

## Content Migration Plan

### Files to Move

| Current Location | New Location | Reason |
|-----------------|--------------|--------|
| `docs/Migration/Run9-WingtipToys-Benchmark.md` | `dev-docs/benchmarks/` | Internal benchmark |
| `docs/Migration/Run10-WingtipToys-Benchmark.md` | `dev-docs/benchmarks/` | Internal benchmark |
| `docs/Migration/Run11-WingtipToys-Benchmark.md` | `dev-docs/benchmarks/` | Internal benchmark |
| `docs/Migration/WebformsHtml/` | `dev-docs/html-samples/` | HTML capture reference |
| `docs/migration-tests/` (entire folder) | `dev-docs/migration-tests/` | Test run reports |
| `docs/samples/` | DELETE | Appears unused (1 file) |
| `samples/Run7WingtipToys/` | `dev-docs/migration-runs/Run7WingtipToys/` | Test run artifact |
| `samples/Run8WingtipToys/` | `dev-docs/migration-runs/Run8WingtipToys/` | Test run artifact |
| `samples/Run9WingtipToys/` | `dev-docs/migration-runs/Run9WingtipToys/` | Test run artifact |
| `samples/Run10WingtipToys/` | `dev-docs/migration-runs/Run10WingtipToys/` | Test run artifact |
| `samples/Run11WingtipToys/` | `dev-docs/migration-runs/Run11WingtipToys/` | Test run artifact |
| `samples/Run12WingtipToys/` | `dev-docs/migration-runs/Run12WingtipToys/` | Test run artifact |
| `planning-docs/screenshots/` | `dev-docs/screenshots/` | Consolidate visuals |

### Files to Keep In Place

| Location | Content | Reason |
|----------|---------|--------|
| `docs/EditorControls/` | 25 component docs | User-facing API docs |
| `docs/DataControls/` | 10 component docs | User-facing API docs |
| `docs/ValidationControls/` | 8 component docs | User-facing API docs |
| `docs/NavigationControls/` | 5 component docs | User-facing API docs |
| `docs/LoginControls/` | 7 component docs | User-facing API docs |
| `docs/UtilityFeatures/` | 8 feature docs | User-facing feature docs |
| `docs/Migration/*.md` (except Run*) | Migration guides | User-facing guides |
| `planning-docs/analysis/` | 9 analysis docs | Internal planning |
| `planning-docs/components/` | 52 control audits | Internal planning |
| `planning-docs/milestones/` | 11 milestone plans | Internal planning |
| `planning-docs/reports/` | Executive reports | Internal planning |
| `samples/AfterBlazor*/` | Demo projects | Reference samples |
| `samples/BeforeWebForms/` | Original Web Forms | Reference sample |
| `samples/*WingtipToys/` (non-Run*) | Migration targets | Reference samples |

---

## mkdocs.yml Changes

### Remove from nav

```yaml
# DELETE THIS ENTIRE SECTION:
  - Migration Tests:
      - Overview: migration-tests/README.md
      - WingtipToys 2026-03-04: migration-tests/wingtiptoys-2026-03-04/report.md

# DELETE THESE LINES FROM MIGRATION SECTION:
      - Run 9 WingtipToys Benchmark: Migration/Run9-WingtipToys-Benchmark.md
      - Run 10 WingtipToys Benchmark: Migration/Run10-WingtipToys-Benchmark.md
      - Run 11 WingtipToys Benchmark: Migration/Run11-WingtipToys-Benchmark.md
```

### Final Migration Section

```yaml
  - Migration:
      - Getting started: Migration/readme.md
      - Automated Migration Guide: Migration/AutomatedMigration.md
      - Migration Strategies: Migration/Strategies.md
      - Custom Controls: Migration/Custom-Controls.md
      - Deferred Controls: Migration/DeferredControls.md
      - Master Pages: Migration/MasterPages.md
      - .NET Standard to the Rescue: Migration/NET-Standard.md
      - Themes and Skins: Migration/ThemesAndSkins.md
      - User Controls: Migration/User-Controls.md
      - Web Forms Application Migration Readiness: Migration/migration_readiness.md
```

---

## Solution Considerations

### Why dev-docs/ instead of expanding planning-docs/?

- `planning-docs/` has a specific purpose: architecture analysis and milestone planning
- Test run outputs and benchmark data are different — they're ephemeral development artifacts
- Separating them allows `planning-docs/` to stay focused on architectural decisions
- `dev-docs/` can also hold contributor guides, onboarding docs, etc. in the future

### Why move screenshots from planning-docs/?

- Screenshots are visual evidence used in test runs and comparisons
- They belong with the test infrastructure, not the planning documentation
- Consolidating in `dev-docs/screenshots/` keeps all comparison visuals together

### What about the solution file?

- `Run*WingtipToys` projects are likely referenced in `BlazorMeetsWebForms.sln`
- Moving to `dev-docs/migration-runs/` will require updating project paths in the .sln file
- Alternative: Use `.slnf` (solution filter) to exclude test runs from normal builds

### Git considerations

- Use `git mv` to preserve history
- Consider a single reorganization commit vs multiple
- Update any CI/CD that references old paths

---

## Folder Audience Summary

| Folder | Primary Audience | Published? | In .sln? |
|--------|-----------------|------------|----------|
| `docs/` | Library users | ✅ Yes (MkDocs) | ❌ No |
| `dev-docs/` | Contributors, team | ❌ No | Maybe (test runs) |
| `planning-docs/` | Project team | ❌ No | ❌ No |
| `samples/` | Developers (demo) | ❌ No (but referenced) | ✅ Yes |
| `migration-toolkit/` | Migration users | ❌ No | ❌ No |
| `src/` | Contributors | ❌ No | ✅ Yes |

---

## Implementation Steps

### Phase 1: Create Structure (Estimated: 15 min)

1. Create `dev-docs/` folder with subdirectories
2. Create `dev-docs/README.md` with contributor index
3. Create `planning-docs/proposals/` (done)

### Phase 2: Move Content (Estimated: 30 min)

1. Move benchmark reports from `docs/Migration/Run*.md`
2. Move `docs/migration-tests/` to `dev-docs/migration-tests/`
3. Move `docs/Migration/WebformsHtml/` to `dev-docs/html-samples/`
4. Move `samples/Run*WingtipToys/` to `dev-docs/migration-runs/`
5. Move `planning-docs/screenshots/` to `dev-docs/screenshots/`
6. Delete `docs/samples/` (verify unused first)

### Phase 3: Update References (Estimated: 45 min)

1. Update `mkdocs.yml` nav
2. Update `BlazorMeetsWebForms.sln` project paths (if needed)
3. Update any markdown links that reference moved files
4. Update `samples/readme.md` to remove Run* references

### Phase 4: Validate (Estimated: 15 min)

1. Build docs locally with `mkdocs serve`
2. Verify all links work
3. Build solution to confirm project references

---

## Decision Request

**Requesting approval from Jeffrey T. Fritz to proceed with this reorganization.**

Key decisions needed:
1. ✅/❌ Approve creation of `dev-docs/` folder
2. ✅/❌ Approve moving test runs from `samples/` to `dev-docs/`
3. ✅/❌ Approve removing benchmark reports from published nav
4. ✅/❌ Approve moving screenshots from `planning-docs/` to `dev-docs/`

Once approved, Beast can execute the file moves and Jubilee can update any sample references.

---

## Appendix: Current File Counts

| Folder | Files | Subfolders | Notes |
|--------|-------|------------|-------|
| `docs/` | 89 | 12 | Includes ~15 internal artifacts |
| `docs/migration-tests/` | 37 | 6 | All internal test runs |
| `planning-docs/` | 127 | 5 | All internal planning |
| `samples/` | 200+ | 14 | Includes 6 test run folders |
| `samples/Run*WingtipToys/` | ~150 | 6 | Test run projects |

After reorganization:
- `docs/` will have ~74 files (user-facing only)
- `dev-docs/` will have ~100+ files (consolidated internal content)
- `samples/` will have ~50 files (demo samples only)

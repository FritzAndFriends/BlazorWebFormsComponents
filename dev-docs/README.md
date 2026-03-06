# Developer Documentation

> **Internal documentation for contributors and maintainers.**  
> This folder is NOT published to the MkDocs site.

---

## Contents

### `/benchmarks/`

WingtipToys migration benchmark reports measuring transform counts, build attempts, and BWFC control preservation rates.

| File | Description |
|------|-------------|
| `Run9-WingtipToys-Benchmark.md` | Cycle 0 baseline (667 transforms, 173 BWFC instances) |
| `Run10-WingtipToys-Benchmark.md` | Cycle 1 improvements (673 transforms, 172 BWFC instances) |
| `Run11-WingtipToys-Benchmark.md` | Cycle 2 improvements (178 BWFC instances, 98.9% preservation) |

---

### `/migration-tests/`

Test run reports from automated migration testing. Each subfolder contains a `report.md` with metrics, findings, and recommendations.

| Folder | Date | Notes |
|--------|------|-------|
| `wingtiptoys-2026-03-04/` | 2026-03-04 | Initial test run |
| `wingtiptoys-run2-2026-03-04/` | 2026-03-04 | Second iteration |
| `wingtiptoys-run3-2026-03-04/` | 2026-03-04 | Third iteration |
| `wingtiptoys-run4-2026-03-04/` | 2026-03-04 | Fourth iteration |
| `wingtiptoys-run5-2026-03-04/` | 2026-03-04 | Fifth iteration |
| `wingtiptoys-run6-2026-03-04/` | 2026-03-04 | Sixth iteration |

---

### `/migration-runs/`

*Empty* — Old Run7-12 WingtipToys project folders were deleted per Jeff's approval.

---

### `/html-samples/`

HTML capture files from original Web Forms controls. Used as reference for verifying Blazor component HTML output fidelity.

---

### `/screenshots/`

Visual comparison screenshots showing original Web Forms pages vs. migrated Blazor pages. Used in benchmark reports and visual regression testing.

**Contents:**
- `original-*.png` — Screenshots from the original Web Forms application
- `migrated-*.png` — Screenshots from the migrated Blazor application
- `comparison-*.png` — Side-by-side comparison images
- `*.html` — HTML mockups for visual verification

---

## Purpose

This folder separates internal development artifacts from user-facing documentation:

| Folder | Audience | Published? |
|--------|----------|------------|
| `docs/` | Library users | ✅ Yes (MkDocs) |
| `dev-docs/` | Contributors, team | ❌ No |
| `planning-docs/` | Project team | ❌ No |
| `samples/` | Developers (demo) | ❌ No |
| `migration-toolkit/` | Migration users | ❌ No |

---

## Related

- [User documentation](../docs/README.md) — Published MkDocs site
- [Planning documents](../planning-docs/README.md) — Architecture analysis, milestone plans
- [Migration toolkit](../migration-toolkit/README.md) — Standalone migration tooling

# Developer Documentation

> **Internal documentation for contributors and maintainers.**  
> This folder is NOT published to the MkDocs site.

---

## Contents

### `/analysis/`

Cross-cutting architecture analysis including data controls, login/identity, theming, and migration toolkit design documents.

---

### `/audits/`

HTML output comparison audits between Web Forms and Blazor components. Contains:
- `blazor/` — Captured HTML output from Blazor components
- `webforms/` — Captured HTML output from original Web Forms controls
- `normalized/` — Normalized HTML for diff comparison
- `diff-report*.md` — HTML comparison reports

---

### `/benchmarks/`

WingtipToys migration benchmark reports measuring transform counts, build attempts, and BWFC control preservation rates.

| File | Description |
|------|-------------|
| `Run9-WingtipToys-Benchmark.md` | Cycle 0 baseline (667 transforms, 173 BWFC instances) |
| `Run10-WingtipToys-Benchmark.md` | Cycle 1 improvements (673 transforms, 172 BWFC instances) |
| `Run11-WingtipToys-Benchmark.md` | Cycle 2 improvements (178 BWFC instances, 98.9% preservation) |

---

### `/component-specs/`

Per-component analysis and HTML output research (~58 docs). Each document compares the original .NET Framework 4.8 API surface against the Blazor component implementation — covering properties, events, methods, and HTML output.

**Status Categories:**
| Status | Meaning |
|--------|---------|
| ✅ Match | Feature exists and works the same as Web Forms |
| ⚠️ Needs Work | Feature exists but is incomplete or behaves differently |
| 🔴 Missing | Feature does not exist in the Blazor component |
| N/A | Feature is server-side only (ViewState, PostBack, etc.) |

---

### `/html-samples/`

HTML capture files from original Web Forms controls. Used as reference for verifying Blazor component HTML output fidelity.

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

### `/milestones/`

Milestone plans, audits, and post-fix reports tracking project progress across development cycles.

---

### `/proposals/`

Feature proposals and design documents for new functionality.

---

### `/reports/`

Executive reports and migration benchmarks including the WingtipToys migration executive report.

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

This folder consolidates all internal development artifacts:

| Folder | Audience | Published? |
|--------|----------|------------|
| `docs/` | Library users | ✅ Yes (MkDocs) |
| `dev-docs/` | Contributors, team | ❌ No |
| `samples/` | Developers (demo) | ❌ No |
| `migration-toolkit/` | Migration users | ❌ No (ships with NuGet) |

---

## Related

- [User documentation](../docs/README.md) — Published MkDocs site
- [Migration toolkit](../migration-toolkit/README.md) — Standalone migration tooling (end-user distributable)

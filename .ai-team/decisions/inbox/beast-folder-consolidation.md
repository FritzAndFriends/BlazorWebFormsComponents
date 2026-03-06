# Folder Consolidation Decision

**Date:** 2025-07-25  
**Author:** Beast (Technical Writer)  
**Status:** Executed

## Summary

Consolidated development documentation folders per the approved plan, reducing top-level directory sprawl while preserving git history.

## Changes Made

### 1. `audit-output/` → `dev-docs/audits/`
- Moved all HTML output comparison files (blazor/, webforms/, normalized/)
- Moved diff-report*.md files
- Removed empty `audit-output/` folder

### 2. `planning-docs/` → `dev-docs/` (consolidated)
| Source | Destination |
|--------|-------------|
| `planning-docs/analysis/` | `dev-docs/analysis/` |
| `planning-docs/milestones/` | `dev-docs/milestones/` |
| `planning-docs/proposals/` | `dev-docs/proposals/` |
| `planning-docs/components/` | `dev-docs/component-specs/` |
| `planning-docs/reports/` | `dev-docs/reports/` |
| `planning-docs/SUMMARY.md` | `dev-docs/SUMMARY.md` |
| `planning-docs/README.md` | (deleted, dev-docs README updated instead) |

- Removed empty `planning-docs/` folder

### 3. Script Deduplication
- **Deleted:** `scripts/bwfc-migrate.ps1` (50KB)
- **Deleted:** `scripts/bwfc-scan.ps1` (24KB)
- **Kept:** `migration-toolkit/scripts/bwfc-migrate.ps1` (82KB) — more complete, end-user distributable
- **Kept:** `migration-toolkit/scripts/bwfc-scan.ps1` (24KB) — end-user distributable

### 4. Documentation Updates
- Updated `dev-docs/README.md` with new consolidated structure
- Added sections for: analysis, audits, component-specs, milestones, proposals, reports
- Removed references to deleted `planning-docs/` folder

## Rationale

- Reduces top-level directory count
- Consolidates all internal dev documentation in one location
- Eliminates duplicate scripts (migration-toolkit versions are authoritative)
- Preserves git history via `git mv`
- `migration-toolkit/` remains separate as it ships with NuGet package

## Impact

- 910 files affected (mostly renames preserving history)
- 2 files deleted (duplicate scripts)
- 1 file modified (dev-docs README)
- 2 folders removed (audit-output, planning-docs)

## Verification

```powershell
# Folders removed
Test-Path "audit-output"      # False
Test-Path "planning-docs"     # False

# New structure exists
Test-Path "dev-docs/audits"   # True
Test-Path "dev-docs/analysis" # True
Test-Path "dev-docs/component-specs" # True
```

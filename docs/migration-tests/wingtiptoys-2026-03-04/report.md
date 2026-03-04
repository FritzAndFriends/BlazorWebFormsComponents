# WingtipToys Migration Benchmark — 2026-03-04

## Summary

| Metric | Value |
|--------|-------|
| **Source App** | WingtipToys (ASP.NET Web Forms, .NET Framework 4.5) |
| **Pages** | 32 markup files (8 root, 15 Account, 1 Admin, 5 Checkout, 2 Master, 1 UserControl) |
| **Controls** | {to be filled from scan} |
| **BWFC Version** | {latest} |
| **Toolkit Version** | {commit hash} |
| **Total Migration Time** | {to be filled} |
| **Tests Passing** | {to be filled} |

## Methodology

Three-layer migration pipeline:
1. **Layer 1 (Automated):** `bwfc-scan.ps1` + `bwfc-migrate.ps1`
2. **Layer 2 (Copilot-Assisted):** Agent-driven using `bwfc-migration` skill
3. **Layer 3 (Architecture):** EF Core, Identity, routing via `bwfc-data-migration` and `bwfc-identity-migration` skills

## Phase Timing

| Phase | Description | Duration | Files Processed | Notes |
|-------|-------------|----------|-----------------|-------|
| Layer 1a | Scan (`bwfc-scan.ps1`) | — | 32 | — |
| Layer 1b | Mechanical transform (`bwfc-migrate.ps1`) | — | — | — |
| Layer 2 | Structural migration (Copilot + skills) | — | — | — |
| Layer 3a | Data access migration (EF6 → EF Core) | — | — | — |
| Layer 3b | Identity migration | — | — | — |
| Layer 3c | Architecture (Global.asax, routing, etc.) | — | — | — |
| Verification | Build, run, screenshot | — | — | — |
| **TOTAL** | | **—** | | |

## Layer 1a: Project Scan

{scan output will go here}

## Layer 1b: Mechanical Transform

{migrate script output will go here}

## Layer 2: Structural Migration

{per-page migration details}

## Layer 3: Architecture Decisions

{EF Core, Identity, routing details}

## Verification

### Build Results
{build output}

### Screenshots

{screenshots with captions}

## Comparison: Before and After

{side-by-side comparisons}

## Conclusions

{summary findings, time savings, recommendations}

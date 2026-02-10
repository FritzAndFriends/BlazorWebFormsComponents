# Project Context

- **Owner:** Jeffrey T. Fritz (csharpfritz@users.noreply.github.com)
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->

📌 Team update (2026-02-10): PRs #328 (ASCX CLI) and #309 (VS Snippets) shelved indefinitely — ASCX CLI tests (Sprint 3 item) are deprioritized — decided by Jeffrey T. Fritz

📌 Triage (2026-02-10): PR #333 (`copilot/create-calendar-component`) is a regression from `dev`. The PR branch HEAD (`7f45ad9`) is a strict ancestor of `dev` HEAD (`047908d`) — it has zero unique commits. Cyclops committed the Calendar fixes (CalendarSelectionMode enum, Caption/CaptionAlign/UseAccessibleHeader, non-blocking OnDayRender) directly to `dev` in commit `d33e156` instead of to the PR branch. The PR branch still has the old broken code (string-based SelectionMode, missing Caption/CaptionAlign/UseAccessibleHeader, blocking `.GetAwaiter().GetResult()`). Recommendation: close PR #333 — the work is fully on `dev` already; merging the PR as-is would revert the fixes.

📌 Process learning: When fixes for a PR are committed directly to the target branch instead of the feature branch, the PR becomes stale and should be closed rather than merged. Always commit fixes to the feature branch to keep the PR diff clean.
📌 Team update (2026-02-10): Sprint 1 gate review — Calendar (#333) REJECTED (assigned Rogue for triage) — decided by Forge

# Decision: Close PR #333 — Calendar work already on `dev`

**Author:** Rogue (QA Analyst)
**Date:** 2026-02-10
**Status:** Recommendation (pending team approval)
**Affects:** PR #333, branch `copilot/create-calendar-component`, issue #332

## Summary

PR #333 (`copilot/create-calendar-component`) should be **closed without merging**. All Calendar work is already on `dev`, including fixes that the PR branch lacks.

## Evidence

| Check | Result |
|-------|--------|
| PR branch unique commits (not on `dev`) | **0** — branch HEAD `7f45ad9` is a strict ancestor of `dev` HEAD `047908d` |
| `dev` commits not on PR branch (Calendar-related) | **1** — `d33e156` "fix: refactor Calendar to use CalendarSelectionMode enum (#333)" |
| CalendarSelectionMode.cs on `dev` | ✅ Present — proper enum with None/Day/DayWeek/DayWeekMonth |
| CalendarSelectionMode.cs on PR branch | ❌ Missing — does not exist |
| SelectionMode type on `dev` | ✅ `CalendarSelectionMode` enum |
| SelectionMode type on PR branch | ❌ `string` (magic strings: "Day", "None", etc.) |
| Caption/CaptionAlign/UseAccessibleHeader on `dev` | ✅ Present |
| Caption/CaptionAlign/UseAccessibleHeader on PR branch | ❌ Missing |
| OnDayRender invocation on `dev` | ✅ Non-blocking: `_ = OnDayRender.InvokeAsync(args);` |
| OnDayRender invocation on PR branch | ❌ Blocking: `.GetAwaiter().GetResult()` |

## Root Cause

Cyclops committed the Calendar fixes directly to `dev` (commit `d33e156`) instead of pushing them to the `copilot/create-calendar-component` feature branch. This left the PR branch behind `dev` with the old broken code.

## Recommendation

**Close PR #333 without merging.** Rationale:

1. **No unique work on the PR branch** — every commit on the PR branch is already an ancestor of `dev`. Merging would be a no-op after rebase, or actively harmful if merged as-is (reverting the enum fix, Caption support, accessible headers, and non-blocking event handling).
2. **Rebasing is pointless** — after rebase, the PR diff would be empty (0 files changed). There's nothing to review or merge.
3. **Issue #332 is already resolved** — the Calendar component with all fixes is live on `dev`.

## Action Items

- [ ] Close PR #333 with a comment explaining the situation
- [ ] Verify issue #332 can be closed (Calendar component is complete on `dev`)
- [ ] Process note: future fixes for PR review feedback should be committed to the feature branch, not directly to the target branch

## Process Improvement

To prevent this pattern from recurring: when Forge's gate review identifies issues in a PR, the fix commits should go to the **feature branch**, not to `dev`. This keeps the PR as the single source of truth for the change and avoids orphaned PRs.

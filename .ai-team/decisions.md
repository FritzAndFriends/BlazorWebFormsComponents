# Decisions

> Shared team decisions. All agents read this. Only Scribe writes here (by merging from inbox).

<!-- Decisions are appended below by the Scribe after merging from .ai-team/decisions/inbox/ -->

### 2026-02-10: Sample pages use Components/Pages path

**By:** Jubilee
**What:** All new sample pages should be created in `Components/Pages/ControlSamples/{ComponentName}/Index.razor`. The older `Pages/ControlSamples/` path should not be used for new components.
**Why:** The sample app has two page directories — the newer .NET 8+ `Components/Pages/` layout is the standard for new work.

### 2026-02-10: PR merge readiness ratings

**By:** Forge
**What:** PR review ratings established: #333 Calendar (Needs Work — SelectionMode enum), #335 FileUpload (Needs Work — broken data flow), #337 ImageMap (Needs Work — wrong base class), #327 PageService (Ready with minor fixes), #328 ASCX CLI (Risky — conflicts, no tests), #309 VS Snippets (Risky — conflicts).
**Why:** Systematic review of all open PRs to establish sprint priorities and identify blockers.

### 2026-02-10: CalendarSelectionMode must be an enum, not a string (consolidated)

**By:** Forge, Cyclops
**What:** Created `CalendarSelectionMode` enum in `Enums/CalendarSelectionMode.cs` with values None (0), Day (1), DayWeek (2), DayWeekMonth (3). Refactored `Calendar.SelectionMode` from string to enum. Also added `Caption`, `CaptionAlign`, `UseAccessibleHeader` properties. Fixed blocking `.GetAwaiter().GetResult()` call.
**Why:** Web Forms uses `CalendarSelectionMode` as an enum. Project convention requires every Web Forms enum to have a corresponding C# enum in `Enums/`. String-based modes are fragile. Blocking async calls risk deadlocks in Blazor's sync context.

### 2026-02-10: FileUpload must use Blazor InputFile internally (consolidated)

**By:** Forge, Cyclops
**What:** The `@onchange` binding on `<input type="file">` uses `ChangeEventArgs` which does not provide file data in Blazor. FileUpload MUST use Blazor's `<InputFile>` component internally instead of a raw `<input type="file">`. `InputFile` provides proper `InputFileChangeEventArgs` with `IBrowserFile` objects that enable all file operations. Without this, `HasFile` always returns false and `FileBytes`, `FileContent`, `PostedFile`, `SaveAs()` are all broken.
**Why:** Ship-blocking bug — the component cannot function without actual file data access. `InputFile` renders as `<input type="file">` in the DOM so existing tests still pass. Requires `@using Microsoft.AspNetCore.Components.Forms` in the `.razor` file. Any future component needing browser file access must use `InputFile`.

### 2026-02-10: ImageMap base class must be BaseStyledComponent

**By:** Forge
**What:** ImageMap should inherit `BaseStyledComponent`, not `BaseWebFormsComponent`. Web Forms `ImageMap` inherits from `Image` → `WebControl` which has style properties.
**Why:** `BaseWebFormsComponent` is insufficient for controls that need CssClass, Style, and other style properties.

### 2026-02-10: ImageMap categorized under Navigation Controls

**By:** Beast
**What:** ImageMap is categorized under Navigation Controls in the documentation nav, alongside HyperLink, Menu, SiteMapPath, and TreeView.
**Why:** ImageMap's primary purpose is clickable regions for navigation — it aligns with navigation-oriented controls rather than editor/display controls.

### 2026-02-10: Shelve ASCX CLI and VS Snippets indefinitely

**By:** Jeffrey T. Fritz (via Copilot)
**What:** PR #328 (ASCX CLI, issue #18) and PR #309 (VS Snippets, issue #11) removed from sprint plan and shelved indefinitely.
**Why:** Both PRs have merge conflicts and are considered risky. Not worth the effort right now.

### 2026-02-10: Docs and samples must ship with components

**By:** Jeffrey T. Fritz (via Copilot)
**What:** Documentation (Beast) and sample pages (Jubilee) must be delivered in the same sprint as the component they cover — never deferred to a later sprint.
**Why:** Components aren't complete without docs and samples.

### 2026-02-10: Sprint plan — 3-sprint roadmap

**By:** Forge
**What:** Sprint 1: Land & Stabilize current PRs (Calendar enum fix, FileUpload data flow, ImageMap base class, PageService merge). Sprint 2: Editor & Login Controls (MultiView, Localize, ChangePassword, CreateUserWizard). Sprint 3: Data Controls + Tooling + Polish (DetailsView, PasswordRecovery, migration guide, sample updates).
**Why:** Prioritizes getting current PRs mergeable first, then fills biggest control gaps, then invests in tooling and documentation.

### 2026-02-10: Sprint 1 gate review results

**By:** Forge
**What:** Gate review of Sprint 1 PRs: Calendar (#333) REJECTED — branch regressed, dev already has fixes, assigned to Rogue for triage. FileUpload (#335) REJECTED — `PostedFileWrapper.SaveAs()` missing path sanitization, assigned to Jubilee. ImageMap (#337) APPROVED — ready to merge. PageService (#327) APPROVED — ready to merge.
**Why:** Formal gate review to determine merge readiness. Lockout protocol enforced: Cyclops locked out of Calendar and FileUpload revisions.

### 2026-02-10: FileUpload SaveAs path sanitization required

**By:** Forge
**What:** `PostedFileWrapper.SaveAs()` must sanitize file paths to prevent path traversal attacks. `Path.Combine` silently drops earlier arguments if a later argument is rooted. Must use `Path.GetFileName()` and validate resolved paths.
**Why:** Security defect blocking merge of FileUpload (#335).

### 2026-02-10: Lockout protocol — Cyclops locked out of Calendar and FileUpload

**By:** Jeffrey T. Fritz
**What:** Cyclops is locked out of revising Calendar (#333) and FileUpload (#335). Calendar triage assigned to Rogue. FileUpload fix assigned to Jubilee.
**Why:** Lockout protocol enforcement after gate review rejection.

### 2026-02-10: Close PR #333 — Calendar work already on dev

**By:** Rogue
**What:** PR #333 (`copilot/create-calendar-component`) should be closed without merging. All Calendar work including enum fix, Caption/CaptionAlign/UseAccessibleHeader, and non-blocking OnDayRender is already on `dev` (commit `d33e156`). The PR branch has 0 unique commits — merging would be a no-op or actively harmful. Issue #332 is resolved on `dev`.
**Why:** Cyclops committed Calendar fixes directly to `dev` instead of the feature branch, leaving the PR branch behind with old broken code. Rebasing would produce an empty diff. Process note: future PR review fixes should go to the feature branch, not the target branch.

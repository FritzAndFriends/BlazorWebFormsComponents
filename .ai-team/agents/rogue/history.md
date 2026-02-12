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
📌 Team update (2026-02-10): Close PR #333 without merging — all Calendar work already on dev, PR branch has 0 unique commits — decided by Rogue
📌 Team update (2026-02-10): Sprint 2 complete — Localize, MultiView+View, ChangePassword, CreateUserWizard shipped with docs, samples, tests. 709 tests passing. 41/53 components done. — decided by Squad
📌 Team update (2026-02-11): Sprint 3 scope: DetailsView + PasswordRecovery. Chart/Substitution/Xml deferred. 48/53 → target 50/53. — decided by Forge
📌 Team update (2026-02-11): Colossus added as dedicated integration test engineer. Rogue retains bUnit unit tests. — decided by Jeffrey T. Fritz

📌 Sprint 3 QA (2026-02-12): Wrote 71 bUnit tests for DetailsView (42 tests) and PasswordRecovery (29 tests). DetailsView tests cover: auto-generated row rendering, header/footer text and templates, command row buttons (Edit/Delete/New), mode switching (ReadOnly→Edit→Insert→Cancel), paging with page navigation, all events (ModeChanging, ModeChanged, ItemDeleting, ItemDeleted, ItemUpdating, ItemUpdated, ItemInserting, ItemInserted, PageIndexChanging, PageIndexChanged), empty data text/template, CssClass, GridLines, Visible=false. PasswordRecovery tests cover: Step 1 rendering (title, instruction, label, input, submit button, ID, help link/icon), Step 2 flow (question title, answer input, username display), Step 3 success text, full 3-step workflow, event firing (OnVerifyingUser, OnVerifyingAnswer, OnSendingMail, OnUserLookupError, OnAnswerLookupError), failure text on cancel, custom text properties, template overrides (UserNameTemplate, SuccessTemplate). All 797 tests pass (71 new + 726 existing). — Rogue

📌 Test pattern: DetailsView is a generic DataBoundComponent<ItemType> — tests must use `ItemType="Widget"` and provide `Items` parameter as a list. PasswordRecovery requires NavigationManager service registration (`Services.AddSingleton<NavigationManager>(new Mock<NavigationManager>().Object)`). Both follow the .razor test file pattern inheriting BlazorWebFormsTestContext via _Imports.razor. — Rogue

📌 Team update (2026-02-12): Sprint 3 gate review — DetailsView and PasswordRecovery APPROVED. 71 new tests verified. 797 total. — decided by Forge

 Team update (2026-02-12): Milestone 4 planned  Chart component with Chart.js via JS interop. 8 work items, design review required before implementation.  decided by Forge + Squad


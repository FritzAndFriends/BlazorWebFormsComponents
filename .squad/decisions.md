# Decisions

> Shared team decisions. All agents read this. Only Scribe writes here (by merging from inbox).

<!-- Decisions are appended below by the Scribe after merging from .squad/decisions/inbox/ -->




# Decision: Executive Summary Update Pattern

**Date:** 2026-05-17T21:45:23-04:00
**Author:** Beast (Technical Writer)
**Status:** Established

## Decision

When updating the executive summary with new benchmark run data:

1. **Count transforms from source, not docs.** Always grep `AddSingleton<I(Markup|CodeBehind)Transform` in `src/BlazorWebFormsComponents.Cli/Program.cs` for authoritative counts. The copilot-instructions count lags implementation.

2. **Verify screenshot paths before linking.** Check which image files actually exist in the run folder before referencing them. Run 90 had `01–06` images but no `07-cart-with-item.png` — the cart screenshot was `04-shopping-cart.png` instead.

3. **Regenerate charts from generate-charts.py, not by editing SVGs.** The Python file is the single source of truth for chart data. Update it with new data points and rerun — do not hand-edit the SVG output.

4. **The dual-benchmark chart format is the canonical multi-app visual.** Two panels: (1) pipeline phases by app (L1/L2/other stacked bars) and (2) acceptance test stability over recent runs for both apps side by side. Use this for any future multi-benchmark executive summary updates.

5. **Headline framing should reflect what improved most.** When build repair time drops more dramatically than total wall clock (e.g. 3:03 → 1:01 in Run 90), lead with error count reduction and repair time, not just total duration. The total wall clock includes report-writing overhead that does not reflect automation progress.

## Why

This update cycle (Run 90 + CU Run 30) revealed that transform counts in copilot-instructions.md and the exec summary were significantly out of date (24→37 markup, 27→48 code-behind), screenshots were app/run-specific rather than stable, and the single-app headline no longer captured the dual-benchmark story. Encoding these rules prevents the same staleness in future updates.

## Scope

Applies to all future executive summary updates in `dev-docs/migration-tests/EXECUTIVE-SUMMARY.md`.


# Bishop decision inbox — CLI code-behind fixes

- **Date:** 2026-05-17T10:01:20-04:00
- **Owner:** Bishop

## Decision
1. `LegacyHelperStubTransform` must skip page, master, and user-control code-behinds by `FileMetadata.FileType` first, not by `SourceFilePath` suffix checks alone.
2. Keep extension-based fallback checks on both source and output paths, including `.razor.cs`, as belt-and-suspenders protection for renamed code-behind files.
3. When helper stubs are generated, preserve protected method signatures instead of dropping them from the extracted API surface.
4. `DataBindTransform` must consume optional `this.` prefixes for both `DataSource` assignments and `DataBind()` removal, and the `DataBind()` regex must own the whole line so it cannot leave a dangling `this.` token behind.

## Why
`FileMetadata.SourceFilePath` points at the markup file for page migrations, so suffix-only detection silently treated real page code-behinds as standalone helpers whenever they referenced `System.Configuration` or other legacy namespaces. That replaced Contoso page logic with empty stubs and stripped event-handler bodies out of generated `.razor.cs` files.

The companion `DataBindTransform` bug was smaller but still a true pipeline failure: removing only `grv.DataBind();` from `this.grv.DataBind();` leaves invalid C#. These are Layer 1 correctness bugs, so the pipeline has to absorb them before L2 ever sees the output.

## Validation
- Added focused regression tests for `LegacyHelperStubTransform` skip behavior and protected helper methods.
- Added focused regression tests for `DataBindTransform` handling of `this.`-prefixed `DataSource` and `DataBind()` statements.
- Full CLI test suite passed: 818/818.
- Fresh isolated Contoso migration confirmed `Instructors.razor.cs` no longer has a dangling `this.` in `OnAfterRenderAsync`, and `Students.razor.cs` / `Courses.razor.cs` preserved real page code instead of API stubs.


# Decision: Helper-class transforms for MapPath and self-instantiation

**Date:** 2026-05-17T19:20:41-04:00  
**Author:** Bishop  
**Status:** Proposed

## Decision

Teach the CLI helper-file path to fix two recurring non-page migration gaps automatically:

1. Extend `ServerShimTransform` so `FileType.CodeFile` inputs rewrite resolvable `Server.MapPath(...)` and `HttpContext.Current.Server.MapPath(...)` calls to `Path.Combine(AppContext.BaseDirectory, ...)`.
2. Tighten `SelfInstantiationTransform` so it only rewrites self-construction in DI-managed classes, then collapse `new CurrentClass()` helper patterns to `this` and unwrap `using` blocks that would otherwise dispose the current service instance.
3. Add both transforms to `SourceFileCopier` so copied `Logic/` and `BLL/` helper classes receive the same helper-gap repairs as page code-behind.

## Why

WingtipToys and ContosoUniversity both still needed manual Layer 2 cleanup in helper classes after otherwise successful CLI runs. The recurring pain points were path resolution in utilities like `ExceptionUtility` and DI-managed helpers like `ShoppingCartActions` constructing themselves again instead of reusing the current scoped instance.

Fixing these in the CLI removes two benchmark-repeatable manual edits and keeps the migration contract aligned with generated DI registration. The additional copier coverage matters because page-only transforms do not reach standalone `.cs` files.

## Verification

- Added focused transform tests for literal/nested `MapPath`, `HttpContext.Current.Server.MapPath`, literal-backed variable paths, DI-only self-instantiation rewrites, and no-op cases.
- Added `SourceFileCopier` tests proving helper-file rewrites run on copied `Logic/` classes.
- `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo` passes.


# Bishop L1 quality fixes

- **Date:** 2026-05-17T13:32:41-04:00
- **Owner:** Bishop

## Decision
For EDMX-backed migrations, the CLI must treat the converter output as the single source of truth for model artifacts on each run.

That means:
1. normalize excluded source-file paths to full paths before `SourceFileCopier` compares them,
2. exclude any source files whose filenames collide with the EF Core files generated from the EDMX, and
3. delete stale excluded output artifacts (for example `Model1.Context.cs`) when rerunning into an existing `After*` folder.

For malformed legacy markup, the late cleanup pass must auto-close unbalanced child tags before a parent/container closes so emitted `.razor` stays syntactically valid even when the source `.aspx` was lax.

## Why
ContosoUniversity Run 28/29 style reruns can look "fixed" in logs while still leaving duplicate EF artifacts or broken Razor in the output tree because the pipeline never cleaned up stale files or balanced malformed containers. Treating generated EDMX output as authoritative and rebalancing markup in the final cleanup pass makes Layer 1 output much more benchmark-runnable without hand edits.


# Bishop SSR form contract transform

- **Date:** 2026-05-17T19:20:41-04:00
- **Owner:** Bishop

## Decision
The CLI should enforce the Blazor static SSR form contract in one late markup pass shared by both normal markup transforms and semantic-pattern output.

That pass must:
1. detect `<form method="post">`, `<EditForm>`, and `<WebFormsForm>` blocks,
2. add a deterministic filename-derived form name when one is missing (`@formname` for HTML/WebFormsForm, `FormName` for `EditForm`), and
3. inject `<AntiforgeryToken />` as the first child element when it is absent.

## Why
WingtipToys and ContosoUniversity were both paying the same Layer 2 tax on every rerun: forms looked correct in generated `.razor` files but were still missing the SSR postback contract required by Blazor. Centralizing the rule in the CLI removes that mechanical repair step, keeps semantic rewrites and standard transforms aligned, and makes generated form names stable enough to reason about in tests and benchmark diffs.


### MasterPage Migration Bridge — Implementation Contract

**Date:** 2026-04-27
**By:** Forge (Lead Architect)
**Requested by:** Jeffrey T. Fritz
**Status:** PROPOSED

---

## Problem Statement

Run 27 confirms the #1 toolkit gap: master-page conversion does not produce a usable Blazor layout. The generated `Site.razor` retains bundling tags and unconverted `<% %>` markup, requiring 14+ minutes of manual Layer 2 repair focused primarily on layout rewriting. The BWFC library has `MasterPage`, `Content`, and `ContentPlaceHolder` components, but neither the C# CLI migrator nor the PowerShell toolkit emits markup that uses them. The components themselves have a structural gap: `Content` registers with `MasterPage` via `CascadingParameter`, but `MasterPage` never cascades itself (no `<CascadingValue>` wrapping `ChildContent` in the .razor file). This means the Content→ContentPlaceHolder slot-filling mechanism is non-functional at runtime.

---

## Contract: 5 Work Areas

### 1. Component Library (`src/BlazorWebFormsComponents/`)

**1a. Fix MasterPage cascading (P0 — blocking):**
- `MasterPage.razor` must wrap `@ChildContent` in `<CascadingValue Value="this">` so that `Content` and `ContentPlaceHolder` children receive the `[CascadingParameter] MasterPage` they expect.
- Current `.razor` file does NOT cascade `this`; the `Content.razor.cs` `ParentMasterPage` is always `null`.

**1b. MasterPage behavior contract:**
- Renders NO wrapper element (matches Web Forms MasterPage behavior — no `<div>` or `<section>`)
- `Head` RenderFragment → `<HeadContent>` (already implemented, keep)
- `ChildContent` RenderFragment → rendered directly (already implemented, keep)
- `Visible` parameter controls rendering (already implemented, keep)
- `Title` / `MasterPageFile` remain `[Obsolete]` with guidance (already implemented, keep)
- `EmptyLayout` is used via `@layout` to prevent layout recursion (already implemented, keep)

**1c. ContentPlaceHolder behavior contract:**
- Renders content from matching `Content` component if present; otherwise renders `ChildContent` (default content). Already coded in `.razor` — works once cascading is fixed.
- Requires `ID` parameter to match with `Content.ContentPlaceHolderID`.
- No wrapper element.

**1d. Content behavior contract:**
- Registers its `ChildContent` with the parent `MasterPage.ContentSections[ContentPlaceHolderID]`.
- Renders nothing itself (already implemented — `.razor` is empty).
- Requires `ContentPlaceHolderID` parameter.

**1e. NOT in scope:**
- Nested master pages (Web Forms `MasterPageFile` nesting). Document as unsupported; use nested Blazor layouts.
- Runtime dynamic master-page switching. Out of scope.
- `FindControl()` API on master pages. Not applicable in Blazor.

### 2. C# CLI Migrator (`src/BlazorWebFormsComponents.Cli/`)

**2a. MasterPageTransform — change output target (P0):**
- Current: Replaces ALL `<asp:ContentPlaceHolder>` with `@Body` and prepends `@inherits LayoutComponentBase`.
- New behavior for `.master` files: Emit a BWFC bridge layout instead of a raw Blazor layout.
  - Prepend `@inherits LayoutComponentBase` (keep).
  - Replace the PRIMARY ContentPlaceHolder (ID matching `MainContent|ContentPlaceHolder1|BodyContent`) with `@Body`.
  - Replace OTHER ContentPlaceHolders with `<ContentPlaceHolder ID="OriginalID">` (preserving default content between tags).
  - Strip `runat="server"` from `<head>` and `<form>` (keep existing behavior).
  - Extract `<head>` content into `<HeadContent>` block (align with PS toolkit behavior).
  - Add a TODO comment for head content review (keep).

**2b. ContentWrapperTransform — preserve relationships (P1):**
- Current: Strips ALL `<asp:Content>` wrappers, losing the `ContentPlaceHolderID` binding.
- New behavior for `.aspx` child pages:
  - Content targeting the PRIMARY ContentPlaceHolder (MainContent/ContentPlaceHolder1/BodyContent) → strip wrapper, keep inner content (current behavior, correct).
  - Content targeting `HeadContent`/`head`/`TitleContent` → convert to `<HeadContent>...</HeadContent>`.
  - Content targeting OTHER ContentPlaceHolderIDs → convert to `<Content ContentPlaceHolderID="OriginalID">...</Content>` (BWFC component, preserving the relationship).
- This requires `ContentWrapperTransform` to be aware of which ContentPlaceHolder IDs exist. Options:
  - **Option A (recommended):** Maintain a static list of "primary" IDs (`MainContent`, `ContentPlaceHolder1`, `BodyContent`) and "head" IDs (`HeadContent`, `head`, `TitleContent`). Anything else → preserve as `<Content>`.
  - **Option B:** Parse the `.master` file first to extract ContentPlaceHolder IDs and pass them through pipeline context. More accurate, more complex.
  - **Decision:** Start with Option A. It handles >95% of real-world cases. Option B can be added later if edge cases arise.

**2c. New: Add CLI tests for master page transforms (P1):**
- Test `.master` → layout with primary CPH → `@Body` + secondary CPH → `<ContentPlaceHolder>`.
- Test `.aspx` with multiple Content blocks → primary stripped, head converted, others preserved.
- Test self-closing ContentPlaceHolder variants.

### 3. PowerShell Migration Toolkit (`migration-toolkit/scripts/bwfc-migrate.ps1`)

**3a. ConvertFrom-MasterPage — align with CLI path (P1):**
- Current behavior at lines 1536-1562 is mostly correct:
  - Primary CPH IDs → `@Body` ✓
  - Other CPH IDs → TODO comment with BWFC hint ✓
- **Change:** Replace TODO comments for secondary ContentPlaceHolders with actual `<ContentPlaceHolder ID="...">` components instead of comment-only output.
- Replace: `@* TODO: ContentPlaceHolder 'X' — BWFC provides... *@`
- With: `<ContentPlaceHolder ID="X">` (preserving default content between the original tags) `</ContentPlaceHolder>`
- Keep the `Write-ManualItem` hint for developer awareness.

**3b. ConvertFrom-ContentWrappers (child pages) — align with CLI (P1):**
- Lines 1338-1360 handle content extraction. Apply same logic as CLI:
  - Primary CPH → strip wrapper (current behavior ✓)
  - Head/Title CPH → convert to `<HeadContent>` (current behavior ✓)
  - Other CPH → convert to `<Content ContentPlaceHolderID="X">` instead of stripping

**3c. Validation:**
- The `Test-BwfcControlPreservation` function should recognize `<ContentPlaceHolder>` and `<Content>` as valid BWFC components (add to the known-component list if not already present).

### 4. Tests and Samples

**4a. Fix existing tests (P0):**
- The 5 existing test files in `src/BlazorWebFormsComponents.Test/MasterPage/` should continue passing after the cascading fix. They test MasterPage + ContentPlaceHolder rendering but do NOT test Content→ContentPlaceHolder slot filling (because it was broken). Verify no regressions.

**4b. Add Content slot-filling tests (P0):**
- `Content/SlotFilling.razor` — Test that `<Content ContentPlaceHolderID="X">` inside a `<MasterPage>` replaces the default content of `<ContentPlaceHolder ID="X">`.
- `Content/MultipleSlots.razor` — Test multiple Content blocks targeting different ContentPlaceHolders.
- `Content/UnmatchedContent.razor` — Test Content with a ContentPlaceHolderID that doesn't match any ContentPlaceHolder (should be silently ignored, not crash).
- `Content/MixedDefaultAndOverride.razor` — Test one CPH with Content override, another with default content.

**4c. Update sample page (P1):**
- `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/MasterPage/Index.razor` — Add a live demo section that actually renders the `<MasterPage>` + `<ContentPlaceHolder>` + `<Content>` components, not just static code snippets.

**4d. CLI transform tests (P1):**
- Add xUnit tests for `MasterPageTransform` and `ContentWrapperTransform` in the CLI test project.

### 5. Documentation

**5a. Update `docs/Migration/MasterPages.md` (P1):**
- Add a "Bridge Components" section explaining the two-phase approach:
  1. Phase 1 (automated): Toolkit converts `.master` to layout with BWFC `<ContentPlaceHolder>` bridge, converts child `.aspx` to pages with BWFC `<Content>` bridge.
  2. Phase 2 (manual): Developer replaces BWFC bridge components with native Blazor patterns (`@Body`, `@section`).
- Document the `Head` parameter behavior.
- Document limitations (nested masters not supported).

**5b. Update component doc if separate from migration doc (P1).**

---

## Edge Cases and Acceptable Limitations

| Edge Case | Handling | Status |
|-----------|----------|--------|
| Nested master pages (`MasterPageFile` in a master) | Not supported. Document: use nested `@layout` directives. | Acceptable limitation |
| Multiple ContentPlaceHolders with same ID | Last-write-wins in `ContentSections` dictionary. Document as undefined behavior. | Acceptable limitation |
| Content without parent MasterPage | `ParentMasterPage` is null; Content renders nothing. No crash. | Already handled |
| ContentPlaceHolder without parent MasterPage | Renders default `ChildContent`. No crash. | Already handled |
| Dynamic master page switching at runtime | Not supported. Not a real migration scenario. | Acceptable limitation |
| `<head runat="server">` containing `<asp:ContentPlaceHolder ID="HeadContent">` | CLI/PS both extract head metadata into `<HeadContent>` and replace HeadContent CPH. Well-handled. | Already handled |
| Master page with NO primary CPH (only secondary CPHs) | No `@Body` emitted. Layout compiles but renders nothing in Body slot. Add a TODO warning. | P2 enhancement |
| Very large master pages with inline code blocks (`<% %>`) | CLI/PS already flag these as TODO. Not auto-converted. | Acceptable limitation |

---

## Priority Summary

| Priority | Item | Owner Suggestion |
|----------|------|-----------------|
| P0 | Fix MasterPage cascading (`<CascadingValue>`) | Component dev (Cyclops) |
| P0 | Content slot-filling tests | Test dev (Rogue) |
| P0 | Verify existing 5 MasterPage tests still pass | Test dev (Rogue) |
| P1 | CLI MasterPageTransform: secondary CPH → `<ContentPlaceHolder>` | CLI dev (Bishop) |
| P1 | CLI ContentWrapperTransform: secondary Content → `<Content>` | CLI dev (Bishop) |
| P1 | CLI transform tests | CLI dev (Bishop) |
| P1 | PS ConvertFrom-MasterPage: secondary CPH → `<ContentPlaceHolder>` | Toolkit dev (Bishop) |
| P1 | PS content wrapper: secondary Content → `<Content>` | Toolkit dev (Bishop) |
| P1 | Update docs/Migration/MasterPages.md | Doc dev (Beast) |
| P1 | Update sample page with live demo | Sample dev (Jubilee) |
| P2 | Warn when no primary CPH found | CLI/PS dev |

---

## Verification Criteria

1. `dotnet build` succeeds with zero new warnings in BWFC library.
2. All existing MasterPage tests pass (5 files, ~15 tests).
3. New Content slot-filling tests pass (4+ new test files).
4. CLI transforms produce correct output for `.master` with mixed primary/secondary CPHs.
5. PS toolkit produces matching output structure.
6. Next WingtipToys benchmark run (Run 28+) shows reduced Layer 2 repair time for layout.
7. Sample page renders the bridge components live, not just as code snippets.


### 2026-05-30T11:53:20.341-04:00: User directive
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Prefer converting ASCX user controls to Blazor .razor files with .razor.cs backing when feasible, rather than favoring WebControl-based wrappers.
**Why:** User request — captured for team memory


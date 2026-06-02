





# Decision: Executive Summary Update Pattern

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

# Bishop decision inbox — Contoso CRUD transforms\n\n- **Date:** 2026-05-17T00:00:00-04:00\n- **Owner:** Bishop\n\n## Decision\nFor Web Forms data controls that already use model binding, the CLI should preserve `SelectMethod`, `InsertMethod`, `UpdateMethod`, and `DeleteMethod` attributes in migrated BWFC markup instead of rewriting them into new delegate-style handler names or TODO comments.\n\nThe GridView migration path should also emit BWFC `CommandField` columns and preserve `BoundField ReadOnly` so generated CRUD pages stay structurally close to their Web Forms source.\n\n## Why\nBWFC already supports string-based CRUD method resolution at runtime through `DataBoundComponent<T>` and `SelectMethodResolver`, so attribute preservation is the lowest-risk, highest-fidelity migration behavior. Rewriting these attributes adds manual repair noise without improving the runnable output.\n\nContosoUniversity `Students.aspx` is the benchmark proof point: preserving CRUD attributes plus `CommandField`/`ReadOnly` keeps the generated GridView close to the original page and removes several avoidable Layer 2 edits.\n\n## Verification\n- `dotnet test tests\\BlazorWebFormsComponents.Cli.Tests --nologo`\n- `dotnet test src\\BlazorWebFormsComponents.Test --nologo --filter GridView`\n- `dotnet run --project src\\BlazorWebFormsComponents.Cli -- migrate -i samples\\ContosoUniversity\\ContosoUniversity -o samples\\AfterContosoUniversity --overwrite`\n

# Bishop — CU Benchmark Quality Fixes (Run 41, commit 31f927d0)

## Decision: Compile-surface is not a strong quarantine signal for non-quarantinable paths

**Context:** Pages with compile-surface issues (e.g., `HttpContext.Current`, unresolved SelectMethod parameters) were being quarantined even when they contained no payment, identity, or admin-path signals. This caused core university data pages (Students, Courses, Instructors) to be silently stubbed.

**Decision:** Remove "Unresolved compile-surface blockers" from `HasStrongSingleSignal()`. Pages with only compile issues on non-quarantinable paths now receive best-effort output: the transformed code-behind is emitted to the compile surface AND saved as an artifact for L2 review. Only pages on clearly quarantinable paths (Admin/*, Account/*, Checkout/*, Mobile) are quarantined for compile issues alone (via `IsClearlyQuarantinablePath`).

**Rationale:** A page that compiles but needs L2 fixes is more useful than a silent stub. The quarantine mechanism should be reserved for genuinely dangerous emissions (identity, payment, admin with heavy data sources).

---

## Decision: IsEssentialPage() must exclude clearly quarantinable paths

**Context:** Adding "Report", "Dashboard", "Overview" etc. to `IsEssentialPage()` caused `Admin/Reports.aspx` to be treated as essential (bypassing all checks). `IsEssentialPage()` always returns `None` immediately — it bypasses even quarantinable-path signals.

**Decision:** Add a `IsClearlyQuarantinablePath()` guard at the top of `IsEssentialPage()`. Pages in Admin/*, Account/*, Checkout/*, or Mobile paths are never essential regardless of filename keywords. This ensures the essential-page protection applies only to core application pages outside the known-risky path categories.

---

## Decision: EDMX HasKey() required for non-conventional primary key names

**Context:** EF Core discovers primary keys named `Id` or `{TypeName}Id` automatically. EDMX entities often have PKs like `CourseID` where the entity name is `Cours` (truncated by EDMX). `CoursID` (expected by EF Core convention) ≠ `CourseID` → EF Core cannot discover the key automatically.

**Decision:** `BuildEntityModelConfiguration()` now emits `entity.HasKey(e => e.{KeyName})` in `OnModelCreating` when the single PK name does not match the EF Core convention. The `[Key]` data annotation is already emitted per property; `HasKey()` is belt-and-suspenders coverage for non-conventional names. Conventional key names (exact `Id` or `{EntityName}Id` case-insensitive match) continue to omit `HasKey()` to keep the generated context clean.

---

## Decision: BLL DI injection already implemented — verify with tests

**Context:** Concern that BLL files with `new ContosoUniversityEntities()` would not get constructor injection when copied through the pipeline.

**Finding:** `SourceFileCopier` already includes `"DbContextInstantiation"` in its transform allow-list (line 64) and `DbContextInstantiationTransform` correctly handles non-page code files via the constructor injection path. No source change was needed; an end-to-end test was added to prevent regression.


# Bishop — ContosoUniversity Run 25 Timing Note

- **Date:** 2026-05-16
- **CLI fix commit under test:** `d591d8d2`
- **Execution HEAD:** `04972b26`
- **Result:** `37/40` acceptance tests

## Timings

| Phase | Started | Finished | Duration |
|-------|---------|----------|----------|
| L1 Migration | 15:38:51.685 | 15:39:10.728 | 19.04s |
| L2 Initial Build Repair | 15:39:30.287 | 15:44:42.581 | 5m 12.29s |
| First Acceptance Run | 15:47:16.200 | 15:51:32.626 | 4m 16.43s |
| Acceptance-Driven Repair Loop | 15:51:32.626 | 16:01:29.701 | 9m 57.08s |
| Final Acceptance Run | 16:01:29.701 | 16:02:49.058 | 1m 19.36s |
| Screenshots | 16:03:36 | 16:03:39 | ~3s |
| **Total to screenshots** | **15:38:51.685** | **16:03:39** | **24m 47s** |

## Key Outcome

The `d591d8d2` fix set improved the compile surface (no `Model1.Context.cs` duplicate blocker, fewer BLL namespace misses, no InnerText stub cleanup), but the benchmark still missed Jeffrey's <5 minute target because the CLI quarantined `Students`, `Courses`, and `Instructors`, forcing manual L2 page reconstruction.

## Remaining Red Flags

1. Quarantine still catches benchmark-critical pages.
2. Students inline edit remains unstable in the acceptance flow.
3. Instructors sort links still fail two acceptance scenarios.


# Bishop decision inbox — ContosoUniversity Run 27

- **Date:** 2026-05-17T08:13:43-04:00
- **Owner:** Bishop

## Decision
For Contoso-style migrated CRUD pages running in interactive mode:

1. Keep BWFC data controls (`GridView`, `DetailsView`, `DropDownList`) on the page instead of replacing them with raw HTML.
2. Materialize typed page DTOs for benchmark-facing grids/details views rather than binding `object`/anonymous projections directly.
3. Treat interactive GridView edit capture as an open framework gap: current `RowUpdating.NewValues` behavior still does not reliably surface browser-edited values under interactive mode.

## Why
Run 27 reached 40/40 acceptance tests only after the Students page was rebuilt around typed models and simplified BWFC usage. The original `object`-typed DetailsView/GridView composition triggered a prerender-time `BaseColumn<T>` null reference, and the remaining edit-path behavior showed that Request/Form-oriented GridView update plumbing is not yet fully interactive-aware.

## Follow-up
- Product/runtime: add a proper interactive GridView edit-value capture path.
- CLI: prefer typed page DTO generation for complex benchmark pages instead of anonymous-object projections.
- Benchmark guidance: keep BWFC controls, but simplify child-style wrappers and unsupported extender markup during L2 repair.


# Bishop — ContosoUniversity Run 28

**Date:** 2026-05-17T12:29:20-04:00  
**Owner:** Bishop

## Decision

Treat Run 28 as evidence that recent CLI work improved **raw L1 page code-behind preservation**, but not yet end-to-end benchmark quality.

### Established findings
1. `Students.razor.cs`, `Courses.razor.cs`, and `Instructors.razor.cs` now retain meaningful method bodies in raw L1 output instead of collapsing into stub-only pages.
2. The remaining benchmark blocker is concentrated in **Students interactive CRUD behavior** (`Add`, `Clear`, `Delete`) rather than broad page generation; the final run built cleanly and finished 37/40 acceptance tests with only those three failures remaining.
3. BWFC `DetailsView` field child content is still unsafe during prerender because `DetailsView<ItemType>` cascades itself as `ColumnCollection` without implementing `IColumnCollection<ItemType>`, which can null-ref `BaseColumn<T>.OnInitialized()`.

## Why

Run 28 answered the primary benchmark question: recent CLI fixes did improve raw L1 quality for the three audited Contoso pages. However, the benchmark still regressed versus Run 27 because Students interactive form/grid actions are not deterministic enough under Playwright, so the next investment should target BWFC/runtime interaction fidelity instead of more generic L1 page-body preservation work alone.

## Recommended follow-up

1. Investigate BWFC TextBox/Button/GridView interaction timing so Playwright fill/click flows reliably round-trip values before CRUD handlers execute.
2. Fix `DetailsView` field cascading/runtime contracts rather than relying on page-level auto-generated-row workarounds.
3. Keep tracking CLI page-body preservation improvements separately from final benchmark pass rate so L1 gains are not hidden by downstream runtime gaps.


# Bishop — ContosoUniversity Run 29 Decision

**Date:** 2026-05-17T17:58:23-04:00  
**Owner:** Bishop

## Decision
For Contoso-class benchmark pages that keep BWFC data controls under static SSR, the durable postback contract is:

1. preserve the BWFC `GridView` / `DetailsView` surface,
2. use typed page DTOs instead of `ItemType="object"` projections for complex rows/details,
3. wire benchmark forms as standard SSR `<form method="post">` blocks with **both** `<AntiforgeryToken />` and `@formname`, and
4. inject preserved BLL classes through DI rather than instantiating them inline.

## Why
Run 29 returned to 40/40 acceptance tests and cut total benchmark time to ~14 minutes. The decisive repair after the first 35/40 pass was not a data-layer rewrite — it was restoring the SSR form contract so Playwright-driven add/search/clear flows could post successfully.

Typed DTOs (`StudentGridRow`, `StudentSearchResult`) also reconfirmed the earlier Run 27 lesson: benchmark stability improves when BWFC data controls bind concrete models instead of anonymous/object projections. Combined with DI-wired preserved BLL services, this keeps L2 focused on behavior rather than cleanup.

## Follow-up
- Push the SSR form contract left into the CLI/semantic pipeline where possible.
- Keep preserving typed DTO patterns for benchmark-facing data pages.
- Treat unsupported AJAX extenders as candidates for earlier downgrade/removal in L1.


# Bishop — ContosoUniversity Run 30 Decision

**Date:** 2026-05-17T19:57:00-04:00  
**Owner:** Bishop  
**Status:** Proposed for merge into shared decisions

## Decision

For the ContosoUniversity benchmark, treat the following as the current migration reality:

1. **Benchmark quality is green at 40/40, but Layer 1 is not materially better than Run 29.**
   - Raw L1 still starts from 5 build errors.
   - The new SSR form contract transform does not improve Contoso unless the pipeline first emits real `<form>` wrappers on the benchmark pages.

2. **The three new transforms had limited CU-specific impact.**
   - **SSR form contract transform:** no measurable raw-L1 effect on Contoso because generated benchmark pages still contained zero form wrappers before L2.
   - **Identity code-behind quarantine:** effectively no-op on this fixture because the benchmark path has no Account/OWIN page set.
   - **Helper-class transforms:** no visible benchmark gain; Contoso still required manual DI/service cleanup and page-level SSR form rewiring.

3. **The stable Contoso repair pattern remains:**
   - keep BWFC data controls,
   - repair preserved BLL classes in place,
   - add typed page DTOs where object-typed grids/details are unstable,
   - use explicit SSR forms (`method="post"` + `<AntiforgeryToken />` + `@formname`) for Courses/Students postback flows,
   - never switch the app to interactive server render mode.

4. **Operational rule:** invoke `migration-toolkit\scripts\bwfc-migrate.ps1` with `pwsh`, not Windows PowerShell, until the wrapper script is made parser-safe across both shells.

## Why

Run 30 matched Run 29’s final quality outcome (40/40) but did not reduce the raw compile surface or total benchmark time. The remaining path to the sub-5-minute CU target is not more L2 heroics; it is pushing actual benchmark-form emission and preserved-runtime cleanup further left into Layer 1 while keeping the proven L2 stabilization recipe as the fallback.


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


# Bishop — Identity Code-Behind Quarantine

- **Date:** 2026-05-17T19:20:41-04:00
- **Owner:** Bishop

## Decision
Add a late code-behind transform, `IdentityCodeBehindQuarantineTransform`, that targets legacy account page/control code-behind carrying OWIN-era identity signals after the rest of the CLI pipeline has already run.

The transform should:
- run after other code-behind cleanup/fixup passes
- preserve the partial class, inheritance, fields, properties, and `[Inject]` members
- strip `Microsoft.Owin*`, `Microsoft.AspNet.Identity*`, and `Owin` using directives
- replace legacy method bodies with compile-safe stubs when files still contain signals such as `GetOwinContext()`, `IdentityHelper`, `ApplicationUserManager`, or `SignInStatus`

## Why
`PageQuarantineDetector` intentionally keeps happy-path auth pages like `Account/Login.aspx` and `Account/Register.aspx` on the runnable path because the semantic rewrite plus `ProgramCsEmitter`/redirect-handler annotations already emit SSR form handlers for them. Preserving the old OWIN code-behind after that point only reintroduces non-.NET-10 APIs into the compile surface and creates repetitive Layer 2 cleanup.

A dedicated late quarantine transform gives the benchmark path the thin stub shell it actually needs without over-quarantining the page markup or losing query-parameter models and injected services added by earlier transforms.

## Files
- `src/BlazorWebFormsComponents.Cli/Transforms/CodeBehind/IdentityCodeBehindQuarantineTransform.cs`
- `src/BlazorWebFormsComponents.Cli/Program.cs`
- `tests/BlazorWebFormsComponents.Cli.Tests/TestHelpers.cs`
- `tests/BlazorWebFormsComponents.Cli.Tests/TransformUnit/IdentityCodeBehindQuarantineTransformTests.cs`


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


# Bishop Run 89 — WingtipToys benchmark decision

- **Date:** 2026-05-17T17:43:16-04:00
- **Owner:** Bishop
- **Requested by:** Jeffrey T. Fritz
- **Benchmark:** WingtipToys Run 89

## Decision

Run 89 confirms that the current WingtipToys benchmark output still meets the acceptance bar (`26/26` tests), but identity/account preservation is now the main source of Layer 2 regression time.

The CLI currently emits two conflicting migration shapes for login/register flows:

1. simplified SSR form markup plus Core Identity POST handlers in `Program.cs`, and
2. preserved OWIN-era code-behind files that still reference `GetOwinContext`, `IdentityHelper`, `ModelState`, `User`, and other Web Forms / OWIN-only APIs.

For benchmark-facing account pages, once the semantic rewrite chooses the POST-handler contract, the CLI should also replace the corresponding code-behind with a thin query-parameter model (or quarantine it) instead of preserving uncompilable OWIN logic.

## Why

Run 89 passed end to end, but build repair regressed from `1:03` in Run 88 to `3:03` in Run 89 even though the initial visible error count improved from 3 to 2. Most of that extra time went into stubbing `Account\Login`, `Account\Register`, `Account\OpenAuthProviders`, and `Account\RegisterExternalLogin` back to the already-generated Core Identity handler flow.

This is benchmark-significant because the acceptance suite only needs the simplified login/register experience, and the scaffold already has the correct handler endpoints. Preserving stale OWIN code-behind after choosing that migration contract creates avoidable compile churn without improving fidelity.

## Follow-up

1. Add a CLI semantic/post-processing rule that swaps OWIN-era account code-behind for thin query-parameter models whenever the page markup has already been rewritten to POST handler forms.
2. Keep tracking the older Wingtip gaps (`Server.MapPath` in static helpers, self-instantiation cleanup, stale DI variable names) until they are eliminated from fresh output.


# Bishop Run 90 — WingtipToys benchmark decision

- **Date:** 2026-05-17T19:31:21-04:00
- **Owner:** Bishop
- **Requested by:** Jeffrey T. Fritz
- **Benchmark:** WingtipToys Run 90

## Decision

Run 90 confirms that the WingtipToys benchmark remains green at `26/26` acceptance tests, and that the new identity quarantine transform delivered the biggest measurable improvement: Layer 2 build repair dropped from `3:03` in Run 89 to `1:01` in Run 90 because the benchmark no longer required manual OWIN account-page stubbing.

The SSR form contract transform also proved its value on benchmark-critical forms (`Login`, `Register`, `AddToCart`) by auto-emitting `<AntiforgeryToken />` plus stable `@formname` values, but its coverage is not yet complete because the logout form in `Components\Layout\MainLayout.razor` still lacks `@formname`.

The helper transform work is mixed: `ShoppingCartActions.GetCart()` is now correctly self-instantiation-safe in fresh output, but the `Server.MapPath` rewrite in `Logic\ExceptionUtility` still emitted malformed code and required a manual cleanup.

## Why

Run 90 answers the final benchmark question in human terms: the pipeline is now closer to benchmark-runnable immediately after Layer 1, even though the first visible build error count stayed flat at 2. The repair work is narrower and more mechanical than Run 89 — namespace cleanup, one stale identifier, and one malformed helper rewrite — instead of whole-page identity surgery.

That makes the remaining work much clearer for merge readiness: preserve the current identity quarantine behavior, extend SSR form-name coverage to shell/logout forms, and harden helper-class transforms so they cannot emit invalid `Path.Combine`/instance-member mashups.

## Follow-up

1. Extend the SSR form-contract pass so non-page forms such as the logout form in `MainLayout.razor` also receive deterministic `@formname` values.
2. Harden helper-class `Server.MapPath` rewrites with compile-safe output validation, especially for static helper methods.
3. Add a cleanup pass for preserved DI identifiers/usings so stubbed account pages and shopping-cart pages do not retain stale references like missing `using WingtipToys.Logic;` or `actions`.


### 2026-05-16T15:38:27: User directive
**By:** Jeffrey T. Fritz (via Copilot)
**What:** We're building a toolset that works for EVERYONE — not just our sample apps. Every improvement should benefit all users of the migration toolkit, not be tailored to WingtipToys or ContosoUniversity specifics.
**Why:** User request — captured for team memory. This is a core design principle for the CLI and BWFC components.


# Cyclops decision inbox: DetailsView BoundField support

- **Date:** 2026-05-17
- **Owner:** Cyclops

## Decision
`DetailsView<ItemType>` should implement `IColumnCollection<ItemType>` and adapt explicit child `IColumn<ItemType>` definitions into the existing `DetailsViewField` rendering pipeline, instead of inventing a separate BoundField-only path.

## Why
`BoundField<ItemType>` already depends on a cascading `ColumnCollection` parent and already knows how to render read-only and edit-mode content. Reusing that contract keeps `DetailsView` aligned with `GridView`, fixes prerender null references, preserves explicit field ordering, and keeps auto-generated rows as the fallback when no explicit fields are defined.

## Files
- `src/BlazorWebFormsComponents/DetailsView.razor`
- `src/BlazorWebFormsComponents/DetailsView.razor.cs`
- `src/BlazorWebFormsComponents/BaseColumn.cs`
- `src/BlazorWebFormsComponents.Test/DetailsView/BoundFields.razor`


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

### Wizard Web Forms Fidelity Contract — Review Follow-up

**Date:** 2026-05-20T21:07:06.347-04:00
**By:** Forge (Lead / Web Forms Reviewer)
**Requested by:** Jeffrey T. Fritz
**Status:** PROPOSED

---

## Decision

Treat the current `Wizard` component as a partial Web Forms match, not a parity-complete implementation.

Before the team claims Wizard fidelity, the component should:
- render the default header and navigation rows on the outer table structure rather than only inside the nested content table,
- render sidebar navigation with a Web Forms-compatible list/table/button structure instead of flat links and `<br />` separators,
- wire `StartNavigationTemplate`, `StepNavigationTemplate`, and `FinishNavigationTemplate` into the actual navigation area,
- apply `SideBarButtonStyle` to sidebar navigation elements, and
- either implement or remove the misleading `FinishCompleteButtonText` contract from docs/code.

---

## Why

The current component gets the big moving parts right: step registration, event flow, `AllowReturn`, SSR hidden-field restoration, and the full `WizardStepType` enum are present. But the rendered DOM is still materially different from the default ASP.NET Web Forms Wizard output, especially in the sidebar and overall table layout.

That means existing CSS or JavaScript written against the original Web Forms Wizard structure will not reliably carry forward, which violates this repository's core HTML fidelity charter.

---

## Verification

- Review `src/BlazorWebFormsComponents/Wizard.razor` against default Web Forms Wizard layout expectations.
- Review `src/BlazorWebFormsComponents/Wizard.razor.cs` for declared vs. actually rendered parameters/templates.
- Run `dotnet test src\BlazorWebFormsComponents.Test --filter Wizard --nologo` after any Wizard parity changes.
- Add markup-focused tests that assert sidebar and navigation DOM shape, not only step switching behavior.


# Psylocke Decision Inbox: L2 Recipes for Preserved Code-Behind

- **Date:** 2026-05-17
- **Owner:** Psylocke

## Decision

Layer 2 migration guidance should treat preserved code-behind from L1 as repairable source, not disposable scaffolding.

The standard repair sequence is:
1. Remove leftover `System.Web.*` references first.
2. Keep `WebFormsPageBase` shims (`Request`, `Response`, `Session`, `Server`, `Cache`, `ViewState`, `ClientScript`, `IsPostBack`).
3. Map `Page_Load` / `Page_Init` / `Page_PreRender` to Blazor lifecycle methods without renaming business methods.
4. Convert `(object sender, EventArgs e)` handlers to parameterless or BWFC `EventCallback` signatures.
5. Replace `DataBind()` with BWFC `SelectHandler<ItemType>` or `<WebFormsForm>` postback-style CRUD wiring.
6. Register preserved BLL classes in DI as scoped services and keep original database providers (LocalDB stays SQL Server, never SQLite).

## Why

CU Run 28 showed that once CLI output preserves real code-behind, L2 speed depends on having explicit repair recipes for preserved Web Forms patterns. Without them, agents fall back to expensive rewrites, drift namespaces, overuse render modes, and replace working service logic.

## Impact

- L2 agents get a deterministic triage path for preserved `.razor.cs` files.
- CRUD pages stay on BWFC controls with `ItemType` + `SelectHandler<ItemType>` instead of manual HTML or unsupported render-mode shortcuts.
- BLL/service wiring becomes a consistent DI repair pattern across migrations.


# Decision: Wizard QA should separate folder coverage from broad test filters

**Date:** 2026-05-20T21:07:06.347-04:00
**Author:** Rogue
**Status:** Proposed

## Decision
When reporting Wizard component QA coverage, treat the component folder (`src\BlazorWebFormsComponents.Test\Wizard\`) as the authoritative scope for counting Wizard tests, and treat `dotnet test --filter "Wizard"` as a broader validation run that may include other Wizard-named components such as `CreateUserWizard`.

## Why
The requested Wizard-filtered run passed 26 tests across target frameworks, but the Wizard folder itself currently contains a single `.razor` file with 16 `[Fact]` tests. Using the bare filter alone overstates Wizard component coverage and can hide gaps in the actual `Wizard` test surface.

## Impact
- QA reports stay accurate when component names overlap.
- Future audits should list folder files first, then report filtered-run status separately.
- If exact component-only execution is needed, use more specific filters or folder-based counts alongside the run command.


# Rogue decision inbox — WingtipToys Run 88 benchmark

- **Date:** 2026-05-16T15:45:04-04:00
- **Owner:** Rogue
- **Requested by:** Jeffrey T. Fritz
- **Benchmark target:** CLI fix set `d591d8d2`

## Summary

Run 88 remained green at **26/26 acceptance tests passed** from a freshly cleared `samples\AfterWingtipToys` output. The measurable Wingtip improvement from the fix set is that the prior `ShoppingCartTitle` compile error is gone, reducing fresh Layer 1 build errors from **4 to 3** and matching that reduction in manual Layer 2 fixes (**4 → 3**).

## Timing

| Phase | Started | Finished | Duration |
|---|---|---|---|
| Preparation | 15:38:48 | 15:38:57 | 0:09 |
| L1 migration | 15:39:04 | 15:39:23 | 0:19 |
| Build repair | 15:39:34 | 15:40:37 | 1:03 |
| Startup triage | 15:40:37 | 15:41:09 | 0:32 |
| Acceptance tests | 15:41:09 | 15:42:05 | 0:56 |
| Screenshots | 15:42:05 | 15:43:01 | 0:56 |
| Report | 15:43:26 | 15:45:04 | 1:38 |
| **Total** | **15:38:35** | **15:45:04** | **6:29** |

## Comparison to Run 87

- Acceptance tests: **26/26 → 26/26** (no regression)
- L1 build errors: **4 → 3**
- L2 manual fixes: **4 → 3**
- Build repair time: **~90s → 63s**
- Total wall-clock: **~5 min → 6m29s** (overall slower, but the compile-fix critical path improved)

## Remaining Manual Fixes

1. `ShoppingCart.razor.cs` still needs `actions.GetCartItems()` renamed to `_shoppingCartActions.GetCartItems()`.
2. `Logic\ExceptionUtility.cs` still needs a manual non-page `Server.MapPath` replacement.
3. `Logic\ShoppingCartActions.cs` still needs `GetCart()` rewritten to avoid `new ShoppingCartActions()`.

## Interpretation

- **Validated:** InnerText server control property stubs.
- **Not directly exercised by WingtipToys:** `Entities`/`DataContext` suffix handling, EDMX T4 artifact exclusion, and BLL namespace alignment.
- **Recommendation:** Keep Run 88 as proof that the InnerText fix reduced Wingtip Layer 2 work, then validate the other three fixes against a benchmark that includes EDMX/BLL/DataContext-heavy inputs.



# Decision: Scope Wizard Playwright Locators to a Single Sample Instance

**Date:** 2026-05-20T21:19:29.902-04:00
**Author:** Colossus
**Status:** Proposed

## Decision
When writing Playwright coverage for `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/Wizard/Wizard.razor`, scope interactive locators to a single sample container such as `data-audit-control="Wizard-1"` instead of using page-wide `Next`, `Previous`, `Finish`, or sidebar text locators.

## Why
The Wizard sample page renders six separate Wizard demonstrations on the same route, and several of them reuse the same navigation button text and step labels. Page-wide locators would be ambiguous and flaky, while container-scoped locators consistently target the intended sample flow.

## Impact
Future Wizard integration tests should start from the `data-audit-control` wrapper and then locate headings, buttons, and sidebar links inside that container. This keeps smoke coverage broad while making interactive coverage deterministic.


# Decision: Wizard finish button label precedence

**Date:** 2026-05-20T21:19:29.902-04:00
**Author:** Cyclops
**Status:** Proposed

## Decision

For `Wizard`, step-specific navigation templates replace the default button row only for their matching effective step types (`Start`, `Step`, `Finish`).

For the rendered Finish button label, a non-default `FinishCompleteButtonText` value takes precedence, while `FinishButtonText` remains the compatibility fallback when `FinishCompleteButtonText` is left at its default value.

## Why

`FinishCompleteButtonText` was declared but never affected rendering, while existing callers may already rely on `FinishButtonText`. This precedence makes the previously dead Web Forms-style parameter functional without breaking established BWFC usage that customized the finish label through `FinishButtonText`.

## Affected files

- `src/BlazorWebFormsComponents/Wizard.razor`
- `src/BlazorWebFormsComponents/Wizard.razor.cs`
- `src/BlazorWebFormsComponents.Test/Wizard/Navigation.razor`
- `docs/NavigationControls/Wizard.md`


# Rogue decision inbox — Wizard unsupported behaviors remain explicit QA gaps

**Date:** 2026-05-20T21:19:29.902-04:00
**Author:** Rogue
**Requested by:** Jeffrey T. Fritz
**Status:** Proposed

## Decision

Keep Wizard tests for unsupported behaviors explicitly skipped until the component contract is implemented for:
- parent-driven `ActiveStepIndex` changes raising `ActiveStepIndexChanged`,
- single-step wizards suppressing navigation buttons, and
- dynamic step add/remove scenarios updating the registered step list.

## Why

The current `Wizard` implementation only raises `ActiveStepIndexChanged` during its internal click handlers, auto-classifies a single step as `Start` and still renders a Next button, and only supports add-only child-step registration through `AddStep()` with no removal path. Writing active assertions for those behaviors would produce known-failing tests instead of actionable regression coverage.

## Impact

- QA can keep coverage green while still documenting product gaps in executable form.
- Future Wizard implementation work has named tests ready to unskip when the behavior lands.
- Reports can distinguish supported parity from intentionally deferred Wizard behavior.


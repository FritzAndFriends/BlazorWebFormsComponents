



# Decision: Xml and BaseCompareValidator — tracked test gap resolution

**Date:** 2026-06-12T09:56:18-04:00
**Author:** Rogue (QA Analyst)
**Status:** Established

## Xml — no tests; deferred component

`Xml` is listed in `tracked-components.json` with `"status": "Deferred"`. No `Xml.razor` or `Xml.razor.cs` component source files exist in the library. The original `System.Web.UI.WebControls.Xml` control was a niche XSL transformation container with no clear Blazor analogue. **No bUnit tests should be added for Xml until a component implementation exists.** The tracked-tests gap for Xml is intentional; it must remain open until the component ships. No repo change is needed beyond this note.

## BaseCompareValidator — abstract base; covered via property tests

`BaseCompareValidator<InputType>` is an abstract generic base class that cannot be directly instantiated. Its only testable surface beyond what `BaseValidator` already provides is:

- `Type` (ValidationDataType, defaults to `String`)
- `CultureInvariantValues` (bool, defaults to `false`)

Both concrete subclasses (`CompareValidator` and `RangeValidator`) already exercise this base class's `Compare()` logic extensively. To close the tracking gap, targeted property-default and parameter-acceptance tests were added in:

`src/BlazorWebFormsComponents.Test/Validations/BaseCompareValidatorPropertyTests.razor`

These 6 tests use `CompareValidator` and `RangeValidator` as proxies. **No implementation change is needed.** Going forward, `BaseCompareValidator` should be treated as "covered" once these property tests pass.

---

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



---

# Custom control parser + scaffolder handoff (#557, #549, #550, #548)

**Author:** Bishop  
**Date:** 2026-06-10T10:33:04.9872011-04:00  
**Scope:** BWFC CLI parser/runtime/pipeline wiring for custom controls

## Decision

Ship #557 now with production-grade parsing + runtime integration, and land only the starter surface for #549 and #550 so follow-on work can iterate safely without re-plumbing core context.

## Delivered in this slice

1. **#557 (implemented):**
   - `WebConfigAssemblyParser` now aggregates custom control registrations across all `Web.config` files in the source tree.
   - Parser emits `PrefixToNamespaceMap` to normalize prefix resolution.
   - `RuntimeDetector` consumes parser output and exposes prefix map through runtime profile.
   - Added/updated parser + runtime tests.

2. **#549 (started):**
   - Added `CodeOnlyServerControlAnalyzer` to detect code-only controls (`.cs` classes inheriting Web Forms control bases, no `.ascx/.aspx/.master` companion).
   - Added `CodeOnlyControlScaffolder` skeleton emitter.
   - Wired emission into `MigrationPipeline` startup path (after scaffold) to generate placeholder components under `Generated/CodeOnlyControls`.
   - Added scaffolder and pipeline integration tests for starter behavior.

3. **#550 (plumbing only):**
   - Added shared context path: `MigrationContext.CustomControlPrefixToNamespace` and `FileMetadata.CustomControlPrefixToNamespace`.
   - Pipeline now hydrates this map from runtime detection (and from parser when scaffold is skipped).

## Handoff: remaining work

### #549 remaining
- Replace placeholder generated markup/code with base-class-aware templates (`WebControl`, `CompositeControl`, `DataBoundControl`, `Control`) per team design matrix.
- Project detected public properties/events into `[Parameter]`/`EventCallback` members.
- Add deterministic naming/namespace strategy for duplicate class names beyond numeric suffixing.
- Add migration report surface that links generated control stubs back to source registrations and usage sites.

### #550 remaining
- Implement markup transform that uses `FileMetadata.CustomControlPrefixToNamespace` to resolve local/custom prefixes into generated components/usings.
- Coordinate ordering with existing directive/prefix transforms so this runs before generic `AspPrefixTransform` stripping causes information loss.
- Add regression cases for nested `Web.config` overrides and prefix conflicts.

### #548 handoff note
- This slice did not implement #548 behavior.
- If #548 depends on custom control resolution/scaffolding, build directly on the new shared map + detected code-only control descriptors to avoid adding another parser pass.

## Validation executed

- `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo --filter "FullyQualifiedName~WebConfigAssemblyParserTests|FullyQualifiedName~RuntimeDetectorTests|FullyQualifiedName~CodeOnlyControlScaffolderTests|FullyQualifiedName~FullMigration_EmitsCodeOnlyControlSkeletons_AndPopulatesPrefixMap"`
- `dotnet build samples\AfterWingtipToys --nologo` (fails with pre-existing sample compile error in `Logic\ExceptionUtility.cs`)
- `dotnet build samples\AfterBlazorServerSide --nologo` (passes)


---

# WebFormsForm must inherit ComponentBase explicitly

**Author:** Rogue (QA)  
**Date:** 2026-07  
**Scope:** WebFormsForm.razor, RequestShim.cs  
**Issue:** #533

## Decision

Any `.razor` component in the main project that should NOT be a Web Forms control must explicitly declare `@inherits ComponentBase` to override the project-level `_Imports.razor` (which specifies `@inherits BaseWebFormsComponent`).

## Bugs Found

1. **WebFormsForm.razor** — Missing `@inherits ComponentBase` caused it to inherit `BaseWebFormsComponent` via `_Imports.razor`. Both classes had `[Parameter(CaptureUnmatchedValues = true)]`, throwing `ThrowForMultipleCaptureUnmatchedValuesParameters` at render time. Fixed by adding `@inherits ComponentBase`.

2. **RequestShim.cs line 79** — `new FormShim(null)` was ambiguous between `FormShim(IFormCollection?)` and `FormShim(Dictionary<string, StringValues>)` after the dual-mode constructor was added. Fixed by casting to `(IFormCollection?)null`.

## Impact

Both fixes are required for the WebFormsForm component to render at all. Without them, any page using `<WebFormsForm>` crashes at component initialization.



# Documentation Alignment Remediation Checklist

**Date:** 2026-06-12  
**Initiated by:** Beast (Technical Writer)  
**Scope:** Dashboard accuracy, mkdocs.yml coverage, README completeness, docs file naming alignment

---

## EXECUTIVE SUMMARY

Three meta-gaps identified:

1. **Dashboard scope mismatch**: `docs/dashboard.md` claims "52 targeted Web Forms controls" but `tracked-components.json` contains 61 components. The health dashboard needs scope clarification.

2. **Naming misalignment**: 13 catalog items lack one-to-one documentation pages (e.g., `ConfigurationManager` exists as `Phase1-ConfigurationManager.md` under Migration/), and 15 catalog items are missing README doc links.

3. **Coverage distribution**: While 155+ nav entries exist in `mkdocs.yml` and 191 `.md` files exist under `docs/`, many are utility/infrastructure/migration guides that supplement rather than one-to-one map catalog entries.

---

## BUCKET 1: Must-Fix Mismatches

These require action to prevent user confusion and maintain internal consistency.

### 1.1 Dashboard Scope Number Inconsistency

**Problem:**  
`docs/dashboard.md` line 9 states: "The dashboard tracks **52 targeted Web Forms controls**"  
But `dev-docs/tracked-components.json` lists **61 components** across all categories.

**Impact:**  
- Users reading dashboard.md expect tracking of 52 items but might find 61 in actual dashboard
- Health scoring logic vs. inventory mismatch confuses developers
- Non-deterministic: unclear if 52 is intentional subset vs. outdated count

**Suggested Fix:**
- **Option A:** Update dashboard.md line 9 to say "61 targeted Web Forms controls"
- **Option B:** Clarify scope: "52 core Web Forms controls plus 9 infrastructure/utility components" with explanation
- **Recommend Option A** for simplicity (Update `docs/dashboard.md` line 9 only)

**Files to change:**
- `docs/dashboard.md` (line 9)

**Severity:** HIGH — directly impacts documentation accuracy

---

### 1.2 Infrastructure/Utility Components Missing from README Component Lists

**Problem:**  
README.md organizes components by Web Forms category (Editor, Data, Validation, Navigation, Login) but omits Infrastructure/Utility controls that are in tracked-components.json:
- Content
- ContentPlaceHolder
- MasterPage
- NamingContainer
- ScriptManager (stub)
- UpdatePanel

**Impact:**  
- Developers searching README for these components won't find them
- Suggests components are not officially supported when they are tracked
- Incomplete public API inventory

**Suggested Fix:**
Add a new "Infrastructure & Utility Components" section in README.md after Login Controls section (around line 193).

**Files to change:**
- `README.md` (add section, add links to existing docs)

**Severity:** HIGH — README is public-facing primary documentation

---

## BUCKET 2: Needs Naming/Alignment Decision

These items have documentation coverage but naming/location decisions need team input.

### 2.1 Migration Phase/Shim Docs Mapped Under Different Names

**Problem:**  
Several catalog entries map to migration methodology docs but NOT with one-to-one page names:

| Catalog Entry | Actual Doc Location | Type |
|---|---|---|
| `ConfigurationManager` | `docs/Migration/Phase1-ConfigurationManager.md` | Migration guide (present) |
| `FindControl` | `docs/Migration/FindControl-Migration.md` | Migration guide (present) |
| `Session State` | `docs/Migration/Phase2-SessionShim.md` | Migration guide (present) |
| `Server Utilities` | `docs/UtilityFeatures/ServerShim.md` | Utility feature (present) |
| `Request.Form` / Web Forms Request | `docs/UtilityFeatures/RequestShim.md` + `docs/UtilityFeatures/WebFormsForm.md` | Utility features (present, split) |
| `Cache` | `docs/UtilityFeatures/CacheShim.md` | Utility feature (present) |
| `ClientScript` | `docs/Migration/ClientScriptMigrationGuide.md` | Migration guide (present) |

**Decision Needed:**
1. **Should these entries get their own same-name doc pages?**  
   - Pro: Matches ComponentHealthService detection logic (looks for `docs/**/{ComponentName}.md`)
   - Con: Duplicates content that's already well-covered under Migration/Utility sections
   
2. **Or should ComponentHealthService be updated to recognize aliased doc names?**  
   - Pro: Avoids doc duplication
   - Con: Requires code logic changes; harder to maintain

**Current State:**
- ComponentHealthService likely reports these as "missing docs" (false negative)
- mkdocs.yml does NOT have top-level nav entries for `ConfigurationManager.md`, `FindControl.md`, etc.
- All content is present in appropriate guides, just under different organizational names

**Recommended Decision:**
- **Add topic-specific landing pages** under `docs/UtilityFeatures/` and/or `docs/Migration/` with cross-references
- Create lightweight alias pages: `docs/UtilityFeatures/ClientScript.md` → redirects/embeds `docs/Migration/ClientScriptMigrationGuide.md`
- OR: Update ComponentHealthService to recognize these mappings as "documented" without literal filename match

**Files likely to change (depending on decision):**
- `src/BlazorWebFormsComponents/Diagnostics/ComponentHealthService.cs` (detection logic)
- Potential new files: `docs/UtilityFeatures/ClientScript.md`, `docs/UtilityFeatures/ConfigurationManager.md`, etc.
- `docs/Migration/ConfigurationManager-Overview.md` (if adding top-level aliases)
- `mkdocs.yml` (nav entries for new alias pages)

**Severity:** MEDIUM — documentation exists but may not be discoverable by some tools/developers

---

### 2.2 AJAX Toolkit Showcase / AjaxToolkitShowcase Naming

**Problem:**  
Sample catalog likely has an entry "AjaxToolkitShowcase" or similar but no corresponding one-to-one component doc.

**Current Coverage:**
- `docs/AjaxToolkit/index.md` exists (overview)
- Multiple extender docs exist (Accordion.md, AutoCompleteExtender.md, etc.)
- mkdocs.yml has "Ajax Control Toolkit Extenders" nav section with 20+ entries

**Decision Needed:**
1. Is "AjaxToolkitShowcase" a **sample page** (not a component) that shouldn't be tracked in ComponentHealthService?
2. Or should there be a top-level `docs/AjaxToolkit/AjaxToolkitShowcase.md` landing page?

**Recommended Decision:**
- Verify if this is a sample route vs. a tracked component
- If sample route: exclude from ComponentHealthService tracked list
- If component category: create `docs/AjaxToolkit/AjaxToolkitShowcase.md` as overview/index alternative

**Files likely to change:**
- `dev-docs/tracked-components.json` (possibly remove if not a component)
- `docs/AjaxToolkit/AjaxToolkitShowcase.md` (create if needed)
- `src/BlazorWebFormsComponents/Diagnostics/ComponentHealthService.cs` (exclusion logic)

**Severity:** LOW — Toolkit is well-documented; this is more about classification

---

### 2.3 Custom Controls / Custom WebControl Naming

**Problem:**  
Catalog likely tracks "Custom WebControl" as a component, but this is a **migration pattern**, not a trackable BWFC component.

**Current Coverage:**
- `docs/Migration/CustomWebControl.md` exists
- `docs/Migration/Custom-Controls.md` exists
- Both are about how to migrate custom controls, not "the Custom WebControl component"

**Decision Needed:**
1. Should "Custom WebControl" be in `tracked-components.json` at all?
2. Or is it a "feature" (adapter classes) rather than a "component"?

**Recommended Decision:**
- Remove "Custom WebControl" from tracked-components.json or rename it to "CustomWebControl Adapter" with category "Infrastructure/Feature"
- Clarify in dashboard.md that this is a *migration tool/pattern*, not a Blazor component

**Files likely to change:**
- `dev-docs/tracked-components.json` (remove or rename entry)
- `docs/dashboard.md` (clarify feature vs. component definition)

**Severity:** LOW — Editorial/classification issue

---

## BUCKET 3: Nice-to-Have Cleanup

These are low-risk improvements that enhance clarity and consistency.

### 3.1 Update mkdocs.yml to Cross-Reference Aliased Content

**Problem:**  
mkdocs.yml has migration guides but no top-level entries linking directly to infrastructure components.

**Current State:**
```yaml
- Utility Features:
    - Cache: UtilityFeatures/CacheShim.md
    - Server & Path Resolution: UtilityFeatures/ServerShim.md
    - Request: UtilityFeatures/RequestShim.md
```

But Component Catalog may expect:
```yaml
- Utility Features:
    - Cache: UtilityFeatures/Cache.md  (alias/landing page)
    - ConfigurationManager: UtilityFeatures/ConfigurationManager.md  (alias)
    - ClientScript: UtilityFeatures/ClientScript.md  (alias)
```

**Suggested Fix:**
Review mkdocs.yml nav entries and verify naming matches both:
1. What ComponentHealthService looks for (docs/{ComponentName}.md)
2. What catalog entries expect

**Files to change:**
- `mkdocs.yml` (nav section)

**Severity:** LOW — Docs are accessible; this is UX polish

---

### 3.2 Add Brief Component Inventory Doc

**Suggested Improvement:**
Create `docs/ComponentInventory.md` listing:
- All 61 tracked components by category
- Which are "Core" (Editor, Data, Validation, Navigation, Login) vs. "Infrastructure" vs. "Utility"
- Status (Complete, Stub, Deferred) per tracked-components.json

**Purpose:**
- Single source of truth for what's tracked and what's not
- Reduces need to cross-reference catalog/tracked-components.json/dashboard
- Aligns README, dashboard.md, and mkdocs.yml

**Files to add:**
- `docs/ComponentInventory.md` (new)
- `mkdocs.yml` (add entry in nav)

**Severity:** LOW — Nice to have; doesn't fix existing gaps

---

### 3.3 Standardize Naming in tracked-components.json Categories

**Problem:**
tracked-components.json uses category names that may not match mkdocs.yml section names:
- "Editor" vs. "Editor Controls"
- "Data" vs. "Data Controls"
- "Validation" vs. "Validation Controls"
- "Navigation" vs. "Navigation Controls"
- "Login" vs. "Login Controls"
- "Infrastructure" (no corresponding mkdocs section; scattered across Utility Features & AJAX Controls)

**Suggested Fix:**
Align category names in `tracked-components.json` to match mkdocs.yml section names for consistency.

**Files to change:**
- `dev-docs/tracked-components.json` (category name harmonization)

**Severity:** LOW — Cosmetic consistency

---

## SUMMARY TABLE

| Bucket | Item | Priority | Files to Change | Estimated Effort |
|---|---|---|---|---|
| **1: Must-Fix** | Dashboard scope number (52 vs 61) | HIGH | `docs/dashboard.md` | 5 min |
| **1: Must-Fix** | README missing Infrastructure section | HIGH | `README.md` | 15 min |
| **2: Decision** | ConfigurationManager/FindControl/ClientScript aliasing | MEDIUM | Multiple (depends on decision) | 1-2 hours |
| **2: Decision** | AjaxToolkitShowcase classification | MEDIUM | `tracked-components.json`, `ComponentHealthService.cs` | 30 min |
| **2: Decision** | Custom WebControl classification | MEDIUM | `tracked-components.json`, `docs/dashboard.md` | 30 min |
| **3: Nice-to-Have** | Update mkdocs.yml nav cross-refs | LOW | `mkdocs.yml` | 30 min |
| **3: Nice-to-Have** | Create ComponentInventory.md | LOW | New file, `mkdocs.yml` | 45 min |
| **3: Nice-to-Have** | Standardize tracked-components.json categories | LOW | `tracked-components.json` | 20 min |

---

## RECOMMENDED IMMEDIATE ACTIONS (Today)

1. **Fix dashboard.md line 9**: Change "52" → "61" (5 min, resolves confusion)
2. **Add Infrastructure section to README.md**: List Content, ContentPlaceHolder, MasterPage, NamingContainer (15 min, addresses public-facing gap)
3. **Create decision issue**: Schedule 15-min team discussion on Bucket 2 naming/aliasing strategy

---

## TEAM QUESTIONS FOR FRITZ

1. **Dashboard Scope**: Is 52 the *original* intended target, or is 61 the current reality we should track?
2. **Aliasing Strategy**: For migration phase docs (ConfigurationManager, FindControl, ClientScript), do you want:
   - Option A: Same-name landing pages + cross-references (cleaner for tooling)?
   - Option B: Update health-detection logic to recognize aliases (less duplication)?
3. **Infrastructure vs. Component**: Should tracked-components.json include Infrastructure items (Content, ContentPlaceHolder, etc.), or are those "features" not scored?

---

## NEXT STEPS

1. Approve fixes for Bucket 1 (HIGH priority)
2. Schedule brief decision meeting for Bucket 2 items
3. Delegate Bucket 3 cleanup as low-priority / deferred work
4. After decisions, assign implementation tasks to Beast for doc updates


# Gap Analysis Execution Plan: Feature Completion

**Date:** 2026-06-12  
**Prepared by:** Bishop (Migration Tooling Dev)  
**Status:** Recommended for Team Review  
**Scope:** Aligning sample catalog, documentation, Playwright tests, and component health scoring

---

## Executive Summary

The audit identified **13 specific gaps in Playwright test coverage**, **13 catalog entries missing one-to-one documentation pages**, and **15 components missing README links**. These gaps cluster into **three distinct gap types** (metadata, test, content) with different fix strategies and owners. No repository files were modified during this audit—this is an analysis-only snapshot.

---

## Gap Classification

### Gap Type 1: **Test Coverage Gaps** (13 items)
**Nature:** Routes cataloged in `ComponentCatalog.cs` but not explicitly included in `ControlSampleTests.cs` Playwright test suite.

**Affected Routes:**
- `/ControlSamples/Migration/ConfigurationManager`
- `/migration/session`
- `/ControlSamples/NamingContainer`
- `/ControlSamples/ClientScriptShim`
- `/ControlSamples/ScriptManagerProxy`
- `/migration/server-mappath`
- `/migration/cache`
- `/migration/request`
- `/migration/response-redirect`
- `/migration/ispostback`
- `/ControlSamples/PostBackDemo`
- `/migration/findcontrol`
- `/ControlSamples/Migration/CustomWebControl`

**Gap Type:** Test coverage (these pages exist in the sample app but are not explicitly validated by Playwright).  
**Severity:** Medium—the pages render but lack automated smoke test coverage.  
**Owner:** Rogue (Component Test Lead)

---

### Gap Type 2: **Documentation Structure Gaps** (13 items)
**Nature:** Catalog entries without strict one-to-one matching documentation pages (though some concept coverage may exist under different names).

**Affected Catalog Entries:**
- `AjaxToolkitShowcase`
- `ConfigurationManager`
- `Session State`
- `ClientScript`
- `ClientScriptShim`
- `Server Utilities`
- `Request.Form`
- `IsPostBack`
- `PostBack Demo`
- `FindControl`
- `Custom WebControl`
- `BaseProperties`
- `Theming`

**Gap Type:** Documentation metadata (docs exist, but naming doesn't match component health detection rules).  
**Severity:** Low-to-Medium—concepts are documented; the issue is file naming/catalog mismatch.  
**Root Cause:** `ComponentHealthService` detects docs by strict pattern match: `docs/**/{ComponentName}.md`. Pages like `Phase1-ConfigurationManager.md` exist but fail the pattern.  
**Owner:** Beast (Documentation Lead)

---

### Gap Type 3: **README Navigation Gaps** (15 items)
**Nature:** Components missing doc links in the main `README.md` feature list, even if docs exist elsewhere.

**Affected Components:**
- `Content`
- `ContentPlaceHolder`
- `MasterPage`
- `ModelErrorMessage`
- `AjaxToolkitShowcase`
- `ConfigurationManager`
- `NamingContainer`
- `ClientScript`
- `ClientScriptShim`
- `Cache`
- `WebFormsForm`
- `IsPostBack`
- `FindControl`
- `BaseProperties`
- `Theming`

**Gap Type:** Metadata/Navigation (docs link discovery).  
**Severity:** Low—internal/advanced features not prominent in main README; not a blocker for end users.  
**Owner:** Beast (Documentation Lead)

---

## Execution Plan by Phase

### Phase 1: Clarify the Documentation Strategy (Owner: Beast + Jeffrey)
**Duration:** 1 sprint  
**Goal:** Decide whether the team wants one canonical page per catalog entry or looser conceptual coverage.

**Decisions Needed:**
1. **Naming convention rule:** Should every catalog entry have a corresponding `docs/{Category}/{ComponentName}.md` file?
   - Option A: Enforce strict 1:1 mapping (rename/consolidate existing docs)
   - Option B: Accept conceptual coverage under different names (update health-detection logic)

2. **Health scoring rule:** Should `ComponentHealthService` detect docs by:
   - Current: Strict file-name pattern match
   - Alternative: Semantic lookup table (e.g., `"ConfigurationManager" → "docs/Migration/Phase1-ConfigurationManager.md"`)

3. **README.md scope:** Should all 95+ catalog entries be linked in README, or only tier-1 core components?
   - Current: ~90 links; 15 missing
   - Decision: Link all, or define a curated "core" list for visibility?

**Validation Gate:** Team decision recorded in `.squad/decisions.md` with chosen strategy.

---

### Phase 2a: Close Test Coverage Gaps (Owner: Rogue)
**Duration:** 1–2 sprints  
**Goal:** Add 13 missing routes to `ControlSampleTests.cs` Playwright test matrix.

**Action Items:**
1. Add `InlineData` entries for each missing route:
   - `/ControlSamples/Migration/ConfigurationManager`
   - `/migration/session`
   - (… etc., all 13 from Gap Type 1)

2. Verify each route loads without JS errors in Playwright tests.

3. Run full `ControlSampleTests` to ensure no regressions.

**Validation Gate:** All 13 routes added to `InlineData` and passing in `dotnet test` run.  
**Automation:** This can be partially automated: script to extract catalog routes, compare against `InlineData` patterns, emit missing routes.

---

### Phase 2b: Align Documentation Naming (Owner: Beast)
**Duration:** 1–2 sprints  
**Goal:** Either rename/consolidate docs to match catalog names OR update health detection.

**Option A Path (1:1 Mapping):**
1. Create or rename docs files to match strict pattern: `docs/{Category}/{ComponentName}.md`
   - E.g., rename `docs/Migration/Phase1-ConfigurationManager.md` → `docs/UtilityFeatures/ConfigurationManager.md`
   - E.g., create `docs/UtilityFeatures/Session.md` for Session State

2. Update `mkdocs.yml` nav entries to match new names.

3. Run component health dashboard locally; verify all 13 entries now show docs checkmark.

**Option B Path (Semantic Lookup):**
1. Create mapping file: `dev-docs/doc-component-aliases.json`
   ```json
   {
     "ConfigurationManager": "docs/Migration/Phase1-ConfigurationManager.md",
     "Session State": "docs/UtilityFeatures/Request.md",
     ...
   }
   ```

2. Update `ComponentHealthService.DetectDocumentation()` to consult the alias map before falling back to strict pattern match.

3. Run health dashboard; verify all 13 entries now show docs checkmark.

**Validation Gate:** Health dashboard shows all 13 formerly-missing entries with ✅ for "Has Documentation."

---

### Phase 2c: Update README Links (Owner: Beast)
**Duration:** 0.5 sprint  
**Goal:** Add README.md links for all 15 missing components OR update docs/README entry count.

**Action Items:**
1. Decide scope: Link all 95+ catalog entries, or only core components?

2. Extract missing component names from audit.

3. Add links in appropriate section of README.md.

4. Verify links resolve to live documentation pages.

**Validation Gate:** README links match documentation site structure; `grep` confirms all components mentioned have corresponding docs links.

---

## Automation Recommendations

### High-Priority Automation (Owner: Bishop)
These checks can be **automated and run on every commit** to prevent future gaps:

1. **Test Route Coverage Audit** (yearly or per-release):
   - Parse `ComponentCatalog.cs` to extract all routes
   - Parse `ControlSampleTests.cs` to extract all `InlineData` routes
   - Compare; emit missing-route report
   - **Integration:** Add to `.github/workflows/build.yml` as informational check

2. **Documentation Presence Audit** (yearly or per-release):
   - Use `ComponentHealthService` reflection to discover all tracked components
   - Scan `docs/` directory (or consult alias map)
   - Compare; emit missing-docs report
   - **Integration:** Add to `.github/workflows/docs.yml` or as pre-merge check

3. **README Link Audit** (yearly or per-release):
   - Parse `README.md` for component doc links
   - Compare against tracked-components list
   - Emit missing-link report
   - **Integration:** Add to build validation matrix

### Low-Priority / Manual Checks
- Health dashboard Playwright validation (requires running sample app; quarterly or as-needed)
- Catalog-to-test alignment (manual review, but scripted report reduces effort)

### Recommended Script Locations
```
migration-toolkit/scripts/
  - audit-test-coverage.ps1       (test route gaps)
  - audit-docs-coverage.ps1       (doc file gaps)
  - audit-readme-links.ps1        (README link gaps)
  - audit-all-coverage.ps1        (master runner)
```

These can be invoked manually or triggered by CI/CD.

---

## Canonical Sources of Truth (Recommended)

### 1. **Sample Catalog (`ComponentCatalog.cs`)**
- **What it tracks:** Routes, component names, categories, grouping
- **Current status:** Authoritative for "what's in the sample app"
- **Recommendation:** Keep this as the primary inventory. Every route listed here should have:
  - A corresponding Playwright test route
  - A corresponding doc page (or alias entry)
  - A corresponding README link (if tier-1 or marked public)
- **Maintenance:** Update when adding new sample pages; run audit script before PR merge.

### 2. **Documentation Navigation (`mkdocs.yml`)**
- **What it tracks:** Published documentation structure, nav hierarchy
- **Current status:** Derived from `docs/` directory
- **Recommendation:** This is the "published truth." Ensure it stays in sync with actual doc files; use CI to verify no orphaned entries.

### 3. **Component Health Tracking (`tracked-components.json` + `reference-baselines.json`)**
- **What it tracks:** Expected component surface (property/event parity) + implementation status
- **Current status:** Hand-curated baselines; optional tracking file
- **Recommendation:** Adopt `tracked-components.json` as the team's official component inventory for health scoring. Keep in sync with actual implemented components.

### 4. **README.md**
- **What it tracks:** Public feature list + doc link quick-reference
- **Current status:** Manual; prone to drift
- **Recommendation:** This should be derived/generated from `tracked-components.json` or catalog, not hand-maintained. Consider a pre-build step to validate or regenerate it.

### 5. **Test Route Registry (`ControlSampleTests.cs`)**
- **What it tracks:** Playwright test coverage
- **Current status:** Broad but incomplete; includes ~146 unique routes vs. ~95 catalog entries
- **Recommendation:** Treat catalog as source of truth; test registry should be derived. Every catalog route should have a test. Gaps should trigger PR comments.

---

## Validation Gates and Metrics

| Phase | Gate | Measurement | Owner |
|-------|------|-------------|-------|
| **Phase 1** | Documentation strategy decided | Decision recorded in `.squad/decisions.md` | Jeffrey / Beast |
| **Phase 2a** | Test coverage complete | 100% of catalog routes in `ControlSampleTests.cs` | Rogue |
| **Phase 2b** | Docs aligned | Health dashboard: all tracked components show ✅ for docs | Beast |
| **Phase 2c** | README current | Grep confirms all 95+ catalog components have README links (if public tier) | Beast |
| **Ongoing** | Audit automation active | Pre-merge CI check emits coverage report | Bishop |

---

## Effort Estimate

| Phase | Complexity | Effort | Timeline |
|-------|-----------|--------|----------|
| Phase 1 (Strategy) | Low | 4–8 hours | Week 1 |
| Phase 2a (Tests) | Medium | 8–16 hours | Weeks 2–3 |
| Phase 2b (Docs) | Medium–High | 16–24 hours | Weeks 2–4 |
| Phase 2c (README) | Low | 2–4 hours | Week 3 |
| Automation Setup | Low–Medium | 8–12 hours | Week 4 |
| **Total** | — | **38–64 hours** | **~4 weeks** |

---

## Key Decisions to Record

1. **Documentation naming strategy** (1:1 strict mapping vs. semantic aliases)
2. **README.md scope** (all components vs. curated tier-1 list)
3. **Automation priority** (implement all audits, or defer secondary ones)
4. **Ownership clarification** (Beast, Rogue, Bishop roles confirmed for ongoing maintenance)

---

## Risks & Mitigation

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|-----------|
| Documentation naming conflicts (component + doc name collisions) | Low | Medium | Phase 1 strategy decision clarifies naming rules; aliases prevent collisions |
| Audit automation doesn't catch all edge cases | Medium | Low | Start with manual audit templates; refine rules as gaps discovered |
| Playwright test additions slow down CI | Low | Low | Tests are fast; broad test matrix is expected at this repo scale |
| Team resists process change (new audit automation) | Low | Medium | Pilot audits as informational checks; escalate to requirements only if consensus |

---

## Success Criteria

✅ **Phase 1 Complete:** Team strategy recorded; owners assigned.  
✅ **Phase 2a Complete:** 13 missing test routes added; all Playwright tests pass.  
✅ **Phase 2b Complete:** Docs naming aligned; health dashboard shows 100% docs coverage for tracked components.  
✅ **Phase 2c Complete:** README links match catalog scope; no orphaned components.  
✅ **Automation Complete:** Audit scripts in place; pre-merge CI validation active.

---

## Next Actions for Bishop

1. **Now:** Share this plan with Jeffrey (user) and team for feedback.
2. **Decision Point 1:** Wait for Phase 1 strategy clarification (Beast + Jeffrey).
3. **Decision Point 2:** Confirm automation priority before implementing scripts.
4. **Ongoing:** Monitor phases 2a–c for blockers; maintain executor → owner alignment.

---

## Appendix: Audit Summary Statistics

| Metric | Count | Status |
|--------|-------|--------|
| Total Catalog Entries | 95 | ✅ Inventory current |
| Test Routes (Playwright InlineData) | 146 | ✅ Broad; some over-coverage |
| Missing Test Routes | 13 | ⚠️ Gap: Phase 2a |
| Docs Files in `/docs/` | 191 | ✅ Extensive coverage |
| MkDocs Nav Entries | 155 | ✅ Well-structured |
| README Doc Links | 90 | ⚠️ Gap: 15 missing (Phase 2c) |
| Catalog → Docs Name Mismatches | 13 | ⚠️ Gap: Phase 2b |
| Tracked Components (health dashboard) | 52 | ✅ Core set well-defined |

---

**This plan is ready for team review and decision. No repository files have been modified.**


# Bishop Handoff: #550 and #548

- Timestamp: 2026-06-10T10:33:04.9872011-04:00
- Requested by: Jeffrey T. Fritz

## Completed context to carry forward
- #557 is complete: `WebConfigAssemblyParser` now aggregates custom control registrations across discovered `Web.config` files and feeds a shared prefix-to-namespace map through runtime detection.
- #549 skeleton context is in place: `CodeOnlyServerControlAnalyzer` + `CodeOnlyControlScaffolder` starter path is wired in `MigrationPipeline` to emit placeholder components under `Generated/CodeOnlyControls`.

## #550 next implementation steps
- Implement `LocalTagNamespaceResolutionTransform` to consume `FileMetadata.CustomControlPrefixToNamespace` and resolve custom/local tag prefixes before generic prefix stripping.
- Register the transform in both `src\BlazorWebFormsComponents.Cli\Program.cs` and `tests\BlazorWebFormsComponents.Cli.Tests\TestHelpers.cs`.
- Add regression coverage for nested `Web.config` prefix overrides/conflicts and verify generated component/usings resolution across folder boundaries.
- Validate ordering with existing directive/prefix transforms so prefix metadata is preserved and resolved deterministically.

## #548 next implementation steps
- Implement #548 behavior directly on top of the new shared prefix map and code-only control descriptors from #557/#549 (no new parser pass).
- Define required pipeline hook points so #548 logic runs after runtime metadata hydration but before transforms that erase custom-tag intent.
- Add focused tests proving #548 works with both normal scaffold mode and `--skip-scaffold` paths.
- Add a migration report note path that points maintainers from #548 outcomes back to source control registrations and generated placeholders when manual follow-up is required.


# Colossus Test Gate Decisions — Integration Test Coverage Audit

**Decision Requested By**: Colossus (Integration Test Engineer)  
**Context**: Audit of Playwright sample page coverage vs. ComponentCatalog  
**Date**: 2026-06-12  
**Audit Report**: `colossus-remediation-checklist.txt`

---

## FINDINGS SUMMARY

### Playwright Smoke Coverage Gaps (13 Routes)

| Route | Sample Page | Status | Classification |
|-------|-------------|--------|-----------------|
| /ControlSamples/Migration/ConfigurationManager | ✅ Exists | Missing Test | **Tier 0 — Quick Win** |
| /ControlSamples/ClientScriptShim | ❌ Not Found | Missing Both | Tier 1 — High Value |
| /ControlSamples/NamingContainer | ❌ Not Found | Missing Both | Tier 1 — High Value |
| /ControlSamples/PostBackDemo | ❌ Not Found | Missing Both | Tier 1 — High Value |
| /ControlSamples/ScriptManagerProxy | ❌ Not Found | Missing Both | Tier 1 — High Value |
| /migration/cache | ❌ Not Found | Missing Both | Tier 2 — Reference Catalog |
| /migration/findcontrol | ❌ Not Found | Missing Both | Tier 2 — Reference Catalog |
| /migration/ispostback | ❌ Not Found | Missing Both | Tier 2 — Reference Catalog |
| /migration/request | ❌ Not Found | Missing Both | Tier 2 — Reference Catalog |
| /migration/response-redirect | ❌ Not Found | Missing Both | Tier 2 — Reference Catalog |
| /migration/server-mappath | ❌ Not Found | Missing Both | Tier 2 — Reference Catalog |
| /migration/session | ❌ Not Found | Missing Both | Tier 2 — Reference Catalog |

### bUnit Test Gaps (2 Components)

- **Xml** (Editor control, Status: **Deferred**)
  - Tracked in `tracked-components.json`
  - No component implementation yet
  - No bUnit tests

- **BaseCompareValidator** (Validation base class)
  - Tracked in `tracked-components.json`
  - Base class infrastructure component
  - No bUnit tests (CompareValidator tests exist; base class logic untested)

---

## DECISION REQUIRED: TIER PRIORITIZATION

### ❓ Question 1: ConfigurationManager Playwright Test

**What**: Add `[InlineData("/ControlSamples/Migration/ConfigurationManager")]` to ControlSampleTests.cs theory group.

**Options**:
- **A) Add to "Utility Features" theory** (groups with DataBinder, ViewState, etc.)
- **B) Add to "Migration Shim Sample Pages" theory** (groups with /migration/request-form, /migration/webforms-form)
- **C) Create new "Migration Utilities" theory** (separates migration components from basic utilities)

**Recommendation**: **Option B** — /ControlSamples/Migration/ConfigurationManager is a migration shim feature page, not a core utility. Grouping with other migration pages clarifies test organization.

**Impact**: ~1 line change. No implementation needed.

---

### ❓ Question 2: Tier 1 Sample Page Creation (4 pages)

**What**: Decide whether to create sample pages for:
- `/ControlSamples/ClientScriptShim`
- `/ControlSamples/NamingContainer`
- `/ControlSamples/PostBackDemo`
- `/ControlSamples/ScriptManagerProxy`

**Context**:
- `NamingContainer` has existing bUnit tests → public API, likely worth a sample page
- `PostBackDemo` is a migration feature showcase → high value for migration documentation
- `ClientScriptShim` vs. existing `/ControlSamples/ClientScript` — may be duplicate or naming variant
- `ScriptManagerProxy` is infrastructure → validate if already covered by ScriptManager sample

**Options**:
- **A) Create all 4** (complete coverage, ~2-4 hours implementation + tests)
- **B) Create 2 of 4** (NamingContainer + PostBackDemo only; skip AJAX infrastructure)
- **C) Defer to WingtipToys benchmark phase** (validate impact on migration workflow first)
- **D) Create as migration reference code examples** (no dedicated sample page; add to docs instead)

**Recommendation**: **Option B** (NamingContainer + PostBackDemo) — Both have clear use cases:
- **NamingContainer**: Blazor .NET equivalent of Web Forms NamingContainer behavior (page hierarchies, FindControl scoping)
- **PostBackDemo**: Demonstrates IsPostBack, Page.PostBack, and client-side __doPostBack JS interop

**Impact**: ~3-4 hours (2 sample pages + 2 smoke tests + 2 interaction tests)

---

### ❓ Question 3: Tier 2 Migration Reference Catalog (7 pages)

**What**: Decide whether to create `/migration/*` reference pages:
- `/migration/request`
- `/migration/response-redirect`
- `/migration/session`
- `/migration/cache`
- `/migration/findcontrol`
- `/migration/ispostback`
- `/migration/server-mappath`

**Context**:
- These are migration shim reference features (RequestShim, ResponseShim, SessionShim, CacheShim, FindControl, Page.IsPostBack)
- Currently only 2 migration pages exist: `/migration/request-form`, `/migration/webforms-form`
- High value for migration workflows (developers need working examples of shim usage)
- High volume (7 pages = 4-6 hours implementation + tests)

**Options**:
- **A) Create all 7 as reference catalog** (one example page per shim feature, phased over Q2-Q3)
- **B) Create 3 priority features** (session, request, response-redirect; skip cache/findcontrol/ispostback)
- **C) Defer to docs/tutorial** (add migration guide docs instead of sample pages)
- **D) Create as code snippets in docs** (no dedicated sample pages; reference in migration guides)

**Recommendation**: **Option A (phased)** — These are high-value for migration workflows and align with WingtipToys benchmark needs:
- **Phase 1 (M23)**: /migration/session, /migration/response-redirect, /migration/request (critical shims)
- **Phase 2 (M24)**: /migration/cache, /migration/findcontrol (supporting features)
- **Phase 3 (M25)**: /migration/ispostback, /migration/server-mappath (documentation examples)

**Impact**: ~6-8 hours across 3 milestones + ~20 min Playwright tests per page

---

### ❓ Question 4: Xml Component Tests

**What**: Decide on test coverage for **Xml** (deferred control).

**Current Status**:
- Tracked in `tracked-components.json` with status: **Deferred**
- No component implementation
- No bUnit tests

**Options**:
- **A) Keep deferred; add NO tests** (component not yet public; stub tests premature)
- **B) Add placeholder stub tests** (reserve bUnit folder for future implementation)
- **C) Remove from tracked components** (not planning to implement Xml)

**Recommendation**: **Option A** — Keep deferred. **Xml is a data-transformation control with limited Blazor use cases** (XML processing for data binding). Defer tests until component implementation is prioritized.

**Impact**: No change. Return to this decision when Xml component is un-deferred.

---

### ❓ Question 5: BaseCompareValidator Tests

**What**: Decide whether to add bUnit tests for **BaseCompareValidator** (base validation class).

**Current Status**:
- Tracked in `tracked-components.json`
- Base class; no public user-facing sample page
- Existing CompareValidator tests cover derived functionality
- BaseValidator tests exist but BaseCompareValidator logic is untested

**Options**:
- **A) Add unit tests in Validations folder** (BaseCompareValidatorTests.razor covering type comparers + ControlToCompare binding)
- **B) Extend CompareValidator tests** (add BaseCompareValidator property coverage to existing CompareValidator tests)
- **C) Skip — CompareValidator tests sufficient** (derived tests are adequate coverage)

**Recommendation**: **Option A** — Add **BaseCompareValidatorPropertyTests.razor** under `src/BlazorWebFormsComponents.Test/Validations/`:
- **Why**: BaseCompareValidator contains shared validation logic (type comparers, ControlToCompare property binding) that should have explicit tests
- **Coverage**: Verify TypeComparer factory, ControlToCompare property/parameter validation, and base error messages
- **Pattern**: Mirror existing `BaseValidatorPropertyTests.razor` structure

**Impact**: ~1-1.5 hours (1 test file, 8-12 test methods)

---

## TEAM DECISION CHECKLIST

- [ ] **Decision 1 — ConfigurationManager theory group**: Choose A/B/C
- [ ] **Decision 2 — Tier 1 sample pages**: Choose A/B/C/D
- [ ] **Decision 3 — Tier 2 migration catalog**: Choose A/B/C/D
- [ ] **Decision 4 — Xml deferred**: Choose A/B/C
- [ ] **Decision 5 — BaseCompareValidator tests**: Choose A/B/C

---

## IMPLEMENTATION ROADMAP (Assuming Recommended Path)

| Task | Effort | Phase | Owner |
|------|--------|-------|-------|
| Add ConfigurationManager Playwright test | 5 min | M22 | Colossus |
| Create NamingContainer sample + tests | 90 min | M22 | Cyclops + Colossus |
| Create PostBackDemo sample + tests | 90 min | M22 | Cyclops + Colossus |
| Add BaseCompareValidator unit tests | 60 min | M22 | Colossus |
| **Phase 1 Total** | **~4.5 hours** | **M22** | — |
| Create /migration/session + /migration/response-redirect + /migration/request pages + tests | 120 min | M23 | Cyclops + Colossus |
| Create /migration/cache + /migration/findcontrol pages + tests | 100 min | M24 | Cyclops + Colossus |
| Create /migration/ispostback + /migration/server-mappath pages + tests | 100 min | M25 | Cyclops + Colossus |

---

## DECISION DEADLINE

**Requested from**: Jeffrey T. Fritz, Forge, Cyclops  
**Needed by**: 2026-06-13 (before M22 sprint planning)  
**Format**: Reply in this file with decision selections (A/B/C/D) or in `.squad/decisions.md` main file with final ruling.

---

## APPENDIX: AUDIT METRICS

**Catalog Total**: 95 routes  
**Tested Routes**: 146 (includes sub-pages/variants)  
**Missing Smoke Tests**: 13  
**Missing Sample Pages**: 12  
**Existing but Untested**: 1 (ConfigurationManager)  

**bUnit Test Gaps**: 2 (Xml deferred, BaseCompareValidator)  
**Components with Tests**: 85 / 87 tracked (98%)

**Estimated Remediation Effort**:
- Phase 1 (Decisions + High-Priority): 4.5 hours
- Phase 2 (Interactive assertions): 8-10 hours (ongoing)
- Phase 3 (Migration reference catalog): 6-8 hours over 3 milestones
- **Total**: ~20-25 hours over M22-M25


# Decision: Legacy .aspx URL Compatibility in Migrated Blazor Apps

**Date:** 2026-06-10T17:29:42-04:00
**Author:** Cyclops (Component Dev)
**Status:** Implemented in ContosoUniversity; recommended for all generated apps

## Decision

Migrated Blazor apps must serve legacy `.aspx` URLs (e.g. `/Students.aspx`) via
301 permanent redirect to the canonical clean Blazor route (e.g. `/Students`).

### Implementation Pattern

Add the following middleware in the generated `Program.cs`, immediately after
`app.UseHttpsRedirection()` and before `app.MapStaticAssets()`:

```csharp
// Redirect legacy .aspx URLs to clean Blazor routes
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? string.Empty;
    if (path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
    {
        var cleanPath = path[..^5]; // strip .aspx
        if (string.Equals(cleanPath, "/Home", StringComparison.OrdinalIgnoreCase))
            cleanPath = "/";
        context.Response.Redirect(cleanPath + context.Request.QueryString, permanent: true);
        return;
    }
    await next(context);
});
```

### Special Cases

- `/Home.aspx` must map to `/` (root), not `/Home`, because generated `Home.razor`
  declares `@page "/"`. All other pages follow the simple strip-extension rule.
- Query strings are preserved through the redirect so deep-link parameters survive.

## Why

The acceptance test suite already used `.aspx` URLs because they were authored
against the original Web Forms app. Without this redirect, every test that navigated
to `/Home.aspx`, `/Students.aspx`, etc. returned 404 in the migrated app.

More broadly, real users and external sites have bookmarks/links pointing at the
original `.aspx` URLs. Issuing a 301 permanent redirect is the standard web
mechanism to preserve those links while canonicalizing to clean Blazor routes.

## Scope

- **Implemented now:** `samples/AfterContosoUniversity/Program.cs`
- **Recommended next:** `ProgramCsEmitter` in the CLI should emit this middleware
  block automatically for all generated apps (WingtipToys, DepartmentPortal, etc.)
  so the pattern is consistently applied without per-app manual edits.
- **Acceptance tests:** `src/ContosoUniversity.AcceptanceTests/LegacyAspxUrlTests.cs`
  provides the coverage template for the other benchmark suites.


# Forge: Completion Coverage Remediation Plan

**Date:** 2026-06-12  
**Author:** Forge  
**Status:** Proposed  
**Audience:** Beast (Docs), Jubilee (Samples), Rogue (Tests), Jeffrey (Project Owner)

---

## Overview

The audit identified concrete gaps between what we claim to ship (**tracked-components.json: 61 components**), what the Component Health Dashboard tracks (**docs/dashboard.md: 52 controls**), and what actually has complete coverage across Docs / Tests / Samples.

**Snapshot of Coverage:**
- Docs: 60/61 components (98%) — Missing only `Xml`
- Tests: 59/61 components (97%) — Missing `Xml`, `BaseCompareValidator`
- Samples: 57/61 components (93%) — Missing `TextBox`, `Xml`, `BaseValidator`, `BaseCompareValidator`
- Playwright routes: 82/95 catalog routes (86%) — 13 migration/utility routes lack broad coverage

**Key insight:** The dashboard claims "52 targeted controls" but our actual shipping scope is 61 components, creating a perception gap that needs either a scope clarification or a dashboard update.

---

## 1. IMMEDIATE FIXES (Do This Sprint)

### 1.1 Metadata Cleanup (Ownership: Scribe + Forge)

**Action:** Align tracked-components.json to actual shipping scope and update dashboard.md  
**Effort:** 1–2 hours

1. Decide: Do we track **61 components** (current tracked-components.json) or **52 controls** (current dashboard.md claim)?
   - **Recommended:** Adopt 61. The dashboard's "52" is a subset of "50 essential Web Forms controls + 2 infrastructure" that no longer matches our actual output.
   - Update `docs/dashboard.md` line 9: "tracks **61 components**" instead of "52 targeted Web Forms controls"
   - Update the Glossary to clarify: "These 61 include 50 Web Forms controls, 6 validators, 3 infrastructure (Content, ContentPlaceHolder, MasterPage), 2 shims (Xml:Deferred, ScriptManager:Stub)."

2. Document deferred/stub status in dashboard.md decision table:
   - `Xml` → Deferred (marked in tracked-components.json)
   - `ScriptManager` → Stub (marked in tracked-components.json)
   - This clarifies why their scores are lower.

**Owner:** Scribe (or Forge if Scribe unavailable)

---

### 1.2 Route Coverage — Add 13 Missing Playwright Tests (Ownership: Rogue)

**Action:** Extend `ControlSampleTests.cs` to cover all 13 catalog routes  
**Effort:** 2–3 hours (mostly data entry + one test run)

Missing routes (from audit):
```
/ControlSamples/ClientScriptShim
/ControlSamples/Migration/ConfigurationManager
/ControlSamples/Migration/CustomWebControl
/ControlSamples/NamingContainer
/ControlSamples/PostBackDemo
/ControlSamples/ScriptManagerProxy
/migration/cache
/migration/findcontrol
/migration/ispostback
/migration/request
/migration/response-redirect
/migration/server-mappath
/migration/session
```

**Steps:**
1. Grep `ComponentCatalog.cs` for these exact route names — confirm they exist
2. Add `[InlineData(...)]` entries to `ControlSampleTests.cs` (likely in a "Migration Utilities" section after line 224)
3. Run `dotnet test src/AfterBlazorServerSide.Tests --filter "ControlSampleTests"`
4. Confirm all 13 routes load without 404 or exceptions

**Expected outcome:** All 95 catalog routes are now covered by Playwright.

**Owner:** Rogue

---

### 1.3 Sample Page Additions (Ownership: Jubilee)

**Action:** Create sample pages for 4 missing tracked components  
**Effort:** 3–4 hours per component

**Missing sample pages:**
1. `TextBox` — Editor control, should be trivial (mimic Button sample structure)
2. `Xml` — Deferred status; create a stub page explaining deferred status
3. `BaseValidator` — Validation base class; create a concept page showing inheritance hierarchy
4. `BaseCompareValidator` — Validation base class; create a concept page showing inheritance hierarchy

**Steps per component:**
1. Create `Pages/ControlSamples/{ComponentName}/Index.razor` + `.razor.cs`
2. Register in `ComponentCatalog.cs` with the exact route (e.g., `/ControlSamples/TextBox`)
3. For `Xml` and base classes: add a "Status" banner explaining why they're not fully interactive
4. Include a link back to `docs/{ComponentCategory}/{ComponentName}.md`

**Owner:** Jubilee

---

### 1.4 Documentation Completeness (Ownership: Beast)

**Action:** Verify one-to-one doc pages for all 61 tracked components  
**Effort:** 2–3 hours

**Current status:**
- Missing one-to-one doc page: `Xml` only
- Naming mismatches (docs exist but under different names/paths):
  - `ConfigurationManager` → `docs/Migration/Phase1-ConfigurationManager.md` 
  - `FindControl` → `docs/Migration/FindControl-Migration.md`
  - `Server Utilities` → `docs/UtilityFeatures/ServerShim.md`
  - `Request.Form` → possibly split across `RequestShim.md` and `WebFormsForm.md`

**Steps:**
1. Add `Xml.md` to `docs/EditorControls/` explaining deferred status and migration guidance
2. Update `mkdocs.yml` nav entry: `- Xml: EditorControls/Xml.md`
3. Audit the naming-mismatch items: verify docs are discoverable under their tracked names
4. If a naming mismatch exists (e.g., catalog says "ConfigurationManager" but doc is "Phase1-ConfigurationManager"), either:
   - Rename the doc to match the catalog name, OR
   - Create a redirect/symlink in the nav
5. Add all 61 components to `mkdocs.yml` nav (current snapshot shows some are missing from nav even though docs exist)

**Owner:** Beast

---

### 1.5 README Links (Ownership: Beast)

**Action:** Update README.md component list to be exhaustive  
**Effort:** 1–2 hours

**Current state:** README lists ~45 components with doc links; 16 are missing or buried.

**Steps:**
1. Generate an exhaustive list of all 61 components grouped by category
2. Ensure every tracked component in README has a clickable doc link
3. For deferred/stub components, add a note (e.g., `[Xml](docs/EditorControls/Xml.md) (Deferred)`)

**Owner:** Beast

---

## 2. STRUCTURAL DECISIONS REQUIRED (This Week)

### 2.1 Definition: "Source of Truth" for Component Completeness

**Question:** Which inventory should be canonical?

- **Option A (Recommended):** `tracked-components.json` (61 components) is the master inventory. Dashboard.md, mkdocs.yml, ComponentCatalog.cs, and README must all reflect this same scope.
- **Option B:** `ComponentCatalog.cs` (95 entries) is the source of truth. Some catalog entries are meta-concepts (not real components) — dashboard should only track the 61 "true" components.

**Decision for Jeffrey:**  
I recommend **Option A**. The 61 tracked components are the formal product surface. The 95 catalog entries are a mix of components, concept pages, sample scenarios, and migration examples. By declaring tracked-components.json canonical, we ensure:
- Completion checklist is unambiguous: 61 → must have docs, tests, samples
- Health dashboard stays honest: all 61 must be scored
- README and mkdocs.yml can link all 61 + optionally include the extra 34 as "sample scenarios"

**Action:** Update `.squad/decisions/inbox/forge-completion-plan.md` with this decision and have Scribe merge it into `decisions.md`.

---

### 2.2 Documentation Scope: 1:1 vs. Conceptual

**Question:** Must every tracked component have a same-name markdown file in `docs/`?

- **Current state:** Most do (60/61), but some are grouped under concept pages (e.g., "All Validators" under BaseValidator rather than separate pages per validator)
- **Tension:** Health dashboard strictly checks `docs/{ComponentName}.md` filename, so any "conceptual grouping" is flagged as missing docs

**Decision for Beast:**  
I recommend **strict 1:1 mapping**, but with a "hub-and-spoke" structure:
- Every component gets its own `docs/{Category}/{ComponentName}.md` page
- That page can reference a hub page (e.g., `ValidationControls/BaseValidator.md` → linked to from each concrete validator page)
- This ensures the health dashboard detects completeness AND users can find individual components

**Action:** Establish this as a rule in the Contributing Guide / AGENTS.md so future components follow it.

---

### 2.3 Deferred/Stub Handling in Health Dashboard

**Question:** Should deferred components (Xml) and stubs (ScriptManager) count toward completion?

- **Current:** Xml and ScriptManager are in tracked-components.json but have lower baseline/implementation scores
- **Problem:** Unclear if "incomplete" means "not yet implemented" vs. "intentionally deferred"

**Recommendation:** Add a status indicator to the dashboard scoring:
- **Status = "Complete"**: Expected = Implemented (green checkmark for all dimensions)
- **Status = "Stub"**: Intentionally limited scope (show 50% color)
- **Status = "Deferred"**: Not started (show grey/off-white; exclude from portfolio completeness claims)

**Action:** Update `ComponentHealthService.cs` to visually distinguish status tiers in the dashboard. This lets us claim "60/61 components complete or in progress" instead of the weaker "52 complete."

---

## 3. DEFINITION OF DONE / ACCEPTANCE GATES

### For This Remediation Cycle

A feature is **completion-verified** when ALL of these are true:

#### 3.1 Inventory Completeness
- [ ] Component is in `tracked-components.json` (single source of truth)
- [ ] Component is registered in `ComponentCatalog.cs` with a `/ControlSamples/{Name}` route
- [ ] Component name matches across all three files (case-sensitive, no renaming aliases)

#### 3.2 Documentation Completeness
- [ ] Component has a markdown file at `docs/{Category}/{ComponentName}.md`
- [ ] Markdown file is registered in `mkdocs.yml` nav under the correct category section
- [ ] Component has a clickable link in `README.md` component list
- [ ] **For deferred/stub:** Status is clearly marked (e.g., "(Deferred)" or "(Stub)") in all three places

#### 3.3 Sample Page Completeness
- [ ] Component sample page exists at `Pages/ControlSamples/{ComponentName}/Index.razor`
- [ ] Sample page is registered in `ComponentCatalog.cs` with route matching the filename
- [ ] Sample page loads without 404 or unhandled exceptions

#### 3.4 Test Coverage Completeness
- [ ] Component has bUnit tests in `src/BlazorWebFormsComponents.Test/{ComponentName}/`
- [ ] Component has Playwright route coverage in `ControlSampleTests.cs` (route must appear in `[InlineData(...)]`)
- [ ] Both test suites pass: `dotnet test src/BlazorWebFormsComponents.Test` + `dotnet test src/AfterBlazorServerSide.Tests`

#### 3.5 Dashboard Health Score
- [ ] Component appears on `/dashboard` in the sample app
- [ ] Health score is ≥70% (yellow threshold) for production components, ≥50% for stubs
- [ ] **For deferred:** Marked with status indicator and noted in dashboard help text

---

### For Future Component Additions

When adding a new component, the PR checklist must include:

1. **Before merging:**
   - Add component to `tracked-components.json`
   - Create component in `src/BlazorWebFormsComponents/`
   - Add to `ComponentCatalog.cs` with a `/ControlSamples/{Name}` route
   - Add bUnit tests to `src/BlazorWebFormsComponents.Test/{ComponentName}/`
   - Add sample page to `Pages/ControlSamples/{ComponentName}/`
   - Create markdown doc at `docs/{Category}/{ComponentName}.md`
   - Add nav entry to `mkdocs.yml`
   - Add link to `README.md` component list
   - Add reference baseline to `dev-docs/reference-baselines.json`

2. **PR validation:**
   - All 5 test suites pass (component unit, CLI tests, acceptance, etc.)
   - Dashboard shows new component with ≥70% score

3. **After merge:**
   - Verify the live sample site reflects the new component
   - Verify the live docs site has the new page

---

## 4. MAINTAINING COVERAGE GOING FORWARD

### Add a "Completion Status" CI Check

Create a simple PowerShell/bash script that runs after each build and verifies:

```powershell
# Pseudo-code for a completion validator
$tracked = Get-Content tracked-components.json | ConvertFrom-Json
$catalog = Get-Content ComponentCatalog.cs | grep "new(" | measure-object
$docs = Get-ChildItem docs/**/*.md | measure-object
$tests = Get-ChildItem src/BlazorWebFormsComponents.Test/*/  -Directory | measure-object
$samples = Get-ChildItem Pages/ControlSamples/*/ -Directory | measure-object

# Alert if any dimension drops below 95% of tracked count
if ($docs.Count -lt $tracked.components.Count * 0.95) { 
    Write-Error "Docs coverage below threshold: $($docs.Count)/$($tracked.components.Count)" 
}
```

**Owner:** Rogue (integrate into CI) + Scribe (document the rule)

---

## 5. TIMELINE AND PRIORITY

| Priority | Task | Owner | Est. Effort | Deadline |
|----------|------|-------|-------------|----------|
| **P0** | Metadata alignment (tracked-components.json = 61) | Scribe | 1h | End of week |
| **P0** | Add 13 missing Playwright routes | Rogue | 2h | End of week |
| **P1** | Add 4 missing sample pages | Jubilee | 4h | Next week |
| **P1** | Create Xml.md + audit docs naming | Beast | 2h | Next week |
| **P1** | Update README to be exhaustive | Beast | 1h | Next week |
| **P2** | Update dashboard scoring for status tiers | Forge + Bishop | 4h | Sprint +2 |
| **P3** | Implement CI completion validator | Rogue + Scribe | 3h | Sprint +2 |

---

## 6. SUCCESS CRITERIA

**End-of-Sprint "Complete" State:**
- ✅ All 61 tracked components have docs + sample pages + tests
- ✅ All 95 catalog routes have Playwright coverage
- ✅ Dashboard shows 61/61 components (with status labels for deferred/stub)
- ✅ README links all 61 components
- ✅ mkdocs.yml nav exhaustively lists all 61
- ✅ No components appear as "missing" in health dashboard
- ✅ Audit report shows 61/61 docs, 61/61 tests, 61/61 samples (no gap)

---

## 7. TEAM DECISIONS FOR BOARD

**Forge formally proposes the following team decisions:**

### Decision 1: tracked-components.json (61 components) is the canonical product scope
- **Rationale:** Clear, auditable, reflects actual shipping scope
- **Impact:** Dashboard, README, mkdocs.yml, ComponentCatalog must all reflect 61 (not 52 or 95)
- **Who signs:** Jeffrey (Owner)

### Decision 2: Strict 1:1 documentation mapping (one file per component)
- **Rationale:** Ensures health dashboard remains honest; easier for users to find docs
- **Impact:** No more "grouped" concept pages (every validator gets its own page, linked from a hub)
- **Who signs:** Beast (Docs Lead)

### Decision 3: Deferred and Stub components are separately tracked
- **Rationale:** Product roadmap clarity; "60/61 complete + 1 deferred" is better than "60/61 incomplete"
- **Impact:** Dashboard shows status tiers; contributes to portfolio narrative
- **Who signs:** Forge (Reviewer)

---

## Appendix: Audit Data Summary

### What the audit found:
- **tracked-components.json:** 61 components (source of truth)
- **Dashboard.md claim:** 52 controls (outdated)
- **ComponentCatalog.cs:** 95 routes (includes scenarios + samples)
- **docs/ presence:** 60/61 markdown files (missing Xml only)
- **mkdocs.yml nav:** ~48 entries (incomplete; doesn't list all 61)
- **README.md:** ~45 components with links (incomplete)
- **Playwright coverage:** 82/95 routes (13 catalog routes untested)
- **bUnit tests:** 59/61 components (missing Xml, BaseCompareValidator)
- **Sample pages:** 57/61 components (missing TextBox, Xml, BaseValidator, BaseCompareValidator)

### Gap analysis:
| Metric | Current | Target | Gap | Effort |
|--------|---------|--------|-----|--------|
| Tracked components | 61 | 61 | 0 | — |
| Docs coverage | 60/61 | 61/61 | 1 (Xml) | 1h |
| Sample pages | 57/61 | 61/61 | 4 | 4h |
| bUnit tests | 59/61 | 61/61 | 2 | 2h |
| Playwright routes | 82/95 | 95/95 | 13 | 2h |
| README links | ~45 | 61 | 16 | 1h |
| mkdocs.yml nav | ~48 | 61 | 13 | 1h |

**Total immediate effort: ~14 hours across the team**



# WebFormsForm must inherit ComponentBase explicitly

**Author:** Rogue (QA)  
**Date:** 2026-07  
**Scope:** WebFormsForm.razor, RequestShim.cs  
**Issue:** #533

## Decision

Any `.razor` component in the main project that should NOT be a Web Forms control must explicitly declare `@inherits ComponentBase` to override the project-level `_Imports.razor` (which specifies `@inherits BaseWebFormsComponent`).

## Bugs Found

1. **WebFormsForm.razor** — Missing `@inherits ComponentBase` caused it to inherit `BaseWebFormsComponent` via `_Imports.razor`. Both classes had `[Parameter(CaptureUnmatchedValues = true)]`, throwing `ThrowForMultipleCaptureUnmatchedValuesParameters` at render time. Fixed by adding `@inherits ComponentBase`.

2. **RequestShim.cs line 79** — `new FormShim(null)` was ambiguous between `FormShim(IFormCollection?)` and `FormShim(Dictionary<string, StringValues>)` after the dual-mode constructor was added. Fixed by casting to `(IFormCollection?)null`.

## Impact

Both fixes are required for the WebFormsForm component to render at all. Without them, any page using `<WebFormsForm>` crashes at component initialization.


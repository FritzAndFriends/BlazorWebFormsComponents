# Bishop — Migration Tooling & Benchmark Engineer

**Owner:** Jeffrey T. Fritz  
**Role:** CLI pipeline development, WingtipToys migration benchmark automation  

## Run Summary (Runs 34-41)

### Run 41 — ContosoUniversity benchmark quality fixes (commit 31f927d0)
Three targeted CLI fixes to reduce over-quarantine and improve data model coverage:

**Fix 1 — Quarantine tuning (PageQuarantineDetector):**
- Expanded `IsEssentialPage()` with education patterns (Student, Course, Instructor, Department, Enrollment, Faculty, Grade) and general CRUD patterns (Dashboard, Report, Overview, Browse, Search).
- Added IsClearlyQuarantinablePath() guard at the start of `IsEssentialPage()` so Admin/Account/Checkout pages remain quarantinable even when their filename matches an essential keyword (e.g., Admin/Reports.aspx).
- Removed compile-surface from `HasStrongSingleSignal()`: pages with only compile issues on non-quarantinable paths now get best-effort output (emit + artifact) instead of a silent quarantine stub.
- Updated pipeline integration test for LegacyShell.aspx: now expects actual markup emission and no manifest, not a quarantine stub.

**Fix 2 — EDMX HasKey() for non-conventional PKs (EdmxToEfCoreConverter):**
- `BuildEntityModelConfiguration()` now emits `entity.HasKey(e => e.{KeyName})` when key name is not `Id` or `{EntityName}Id` (case-insensitive), matching EF Core discovery convention.
- Example: Cours.CourseID → needs HasKey(); Student.StudentID → conventional, no HasKey().

**Fix 3 — BLL/service DI injection verification:**
- Confirmed existing SourceFileCopier already applies DbContextInstantiation transform to copied source files. Added end-to-end test covering BLL files with inline `new XxxEntities()`.

**Tests:** 810/810 CLI tests pass.

## Run Summary (Runs 34-40)
- **Run 34:** ServerCodeBlockTransform (Order 510), TemplateFieldChildComponentsTransform (Order 620). 25/25 tests. Key: CartSessionStore singleton for SSR persistence, Minimal API for Playwright testing.
- **Run 35:** Gap fixes G1/G3/G8/G10. DisplayExpressionTransform, HttpUtilityRewriteTransform, EfContextConstructorTransform. 25/25 tests.
- **Run 36:** Idiomatic Razor (@expr for simple chains). 25/25 tests, 10:12 runtime.
- **Run 37:** G1/G2/G4 — DisplayExpressionTransform String.Format, ScriptManagerStripTransform, CompileSurfaceStubTransform. 25/25 tests.
- **Run 40:** Modern scaffold (RuntimeDetector/ProgramCsEmitter) validated. 25/25 tests, 21:55 runtime (-43% vs Run 39).

## Key Technical Learnings
- **Blazor Server pre-render/circuit boundary:** Scoped services lost. Use singleton CartSessionStore keyed by session cookie.
- **Playwright NetworkIdle timing:** Resolves before WebSocket connects. Cart mutations must be Minimal API endpoints, not @onclick handlers.
- **Route conflicts:** @page + MapGet clash. Remove @page, let API own route.
- **Transform string escaping:** Avoid C# interpolated strings with brace escaping. Use plain concatenation instead.
- **Width/Height compatibility:** Must accept strings ("500px", "50%") to match Web Forms migration philosophy.
- **HttpUtility ambiguity:** When legacy package also referenced, rewrite to System.Net.WebUtility.

## P0 Gaps Remaining (Run 40 Analysis)
1. **Runtime scaffold automation:** Fresh output needs manual catalog/cart/auth setup. Impact: 5-8 min/run.
2. **Compile-surface debt:** Non-benchmark pages need automated stubbing. Impact: 4-6 min/run.
3. **Templated control emission:** ListView/FormView still malformed on real fixtures. Impact: 2-4 min/run.

## Decisions & Implementations
- Native CLI services: NuGetStaticAssetExtractor, EdmxToEfCoreConverter (replaced PowerShell bridge)
- CLI entrypoints: scan, assets extract, edmx convert
- Transforms: 7 new (DisplayExpression Order 490, EfContext 106, HttpUtility 104, ServerCodeBlock 510, TemplateField 620, ScriptManager 255, CompileSurfaceStub 850)
- BWFC shims: Width/Height string parsing, QueryString.Get(), System.Web.HttpUtility compat, DataBindingAttributeTransform
- Semantic patterns: QueryDetailsSemanticPattern, ActionPagesSemanticPattern, AccountPages, MasterContent
- Test coverage: 588 CLI tests passing

## User Directives Incorporated
- Deprecate PowerShell scripts — CLI is canonical (2026-05-06T09:16)
- Audit ALL BWFC Width/Height attributes for string support (2026-05-06T15:12)
- NEVER replace ListView/FormView/GridView — must use BWFC components (2026-05-07T09:24)

## Learnings
- **2026-05-08T13:02:09-04:00:** GridView template preservation is a pipeline-order contract, not a single-transform concern. The safe sequence is `DisplayExpressionTransform` → `AspPrefixTransform` → `DataBindingAttributeTransform` → `AttributeStripTransform` → `GridViewColumnItemTypeTransform` → `TemplateContextTransform`, because TemplateField columns only stay typed and intact when expressions, inner controls, and `Context="Item"` are normalized in that order.
- **2026-05-08T13:02:09-04:00:** The regression harness for TemplateField bugs now lives in `tests/BlazorWebFormsComponents.Cli.Tests/TransformUnit/GridViewColumnItemTypeTransformTests.cs` and `tests/BlazorWebFormsComponents.Cli.Tests/PipelineIntegrationTests.cs`. Cover both transform-only markup and full `MigrationPipeline.ExecuteAsync()` output so mixed `BoundField`/`TemplateField` grids and TemplateField-only grids cannot silently collapse back to `BoundField` output.
- **2026-05-08T10:42:43-04:00:** Generated account-page stubs are far more benchmark-ready when the semantic rewrite, Program.cs scaffold, and redirect-handler annotator all agree on one POST-based auth contract. Emitting `/Account/LoginHandler` + `/Account/RegisterHandler`, preserving `ReturnUrl`, and configuring the application cookie paths in the scaffold removes a common first-pass auth failure without needing hand-edited Program.cs.
- **2026-05-08T10:42:43-04:00:** `RequiredFieldValidator` generic inference should prefer the validated control's value type instead of a blanket default. In practice, mapping `TextBox` controls to `string` and only falling back to `object` when no control hint exists prevents noisy generic warnings while keeping the transform deterministic.
- **2026-05-08T10:00:31-04:00:** `copilot-instructions.md` now needs explicit migration-tooling guidance: start from the CLI wrapper, preserve BWFC data controls, trust `WebFormsPageBase` shims, and keep transform registration instructions paired across `Program.cs` and `TestHelpers.CreateDefaultPipeline()` so future agents can repair WingtipToys-class migrations without outside help.
- **2026-05-07T13:17:32-04:00:** ListView `GroupTemplate` and `LayoutTemplate` emission is more reliable when the CLI emits explicit `Context` names (`items`, `groups`) instead of leaving raw `@context` placeholders. That keeps generated placeholder markup readable and removes one common manual repair step on Wingtip fixtures.
- **2026-05-07T13:17:32-04:00:** Typed `GridView` columns must inherit the parent grid `ItemType`; leaving `TemplateField` at `ItemType="object"` breaks migrated template expressions like `@Item.Quantity`. A dedicated post-attribute-strip pass that rewrites child BWFC column generics to the grid row type fixes this deterministically.
- **2026-05-07T13:58:11-04:00:** Compile-surface quarantine works best as a post-semantic pipeline decision, not just a code-behind transform. Letting semantic patterns normalize login/action pages first avoids quarantining pages the CLI can already turn into SSR-safe stubs.
- **2026-05-07T13:58:11-04:00:** A quarantine manifest is most useful when it inventories source-relative paths plus detected feature buckets and suggested manual approaches. That turns build-safe placeholders into an actionable backlog for L2/L3 follow-up instead of a silent compile-surface drop.
- **2026-05-08T11:37:18.862-04:00:** Nested `.aspx` pages migrate more safely when the CLI emits the source-relative route first and keeps the filename-only route as an alias. That preserves original URLs like `/Account/Login` for tests, bookmarks, and links without breaking the simpler `/Login` Blazor convention.
- **2026-05-08T11:37:18.862-04:00:** Redirect-only action pages with inert markup should stay on the runnable path unless identity or payment signals are present. `Response.Redirect` plus `WebFormsPageBase` shims is enough for coupon/cart-style handlers, so compile-surface blockers alone are an over-quarantine signal.


### Team Update (2026-05-07T13:17): GridView ItemType transform and ListView context normalization
GridViewColumnItemTypeTransform (Order 705) now propagates parent grid ItemType to child columns. TemplateContextTransform extends to emit explicit Context names for ListViewGroupTemplate/LayoutTemplate. CLI suite 598→603 passing (+5). Commit 1bdbb1f6. Impact: reduces Layer 1 repair surface on typed data-bound pages.

### Team Update (2026-05-07T15:15): Run 41 benchmark results
Run 41 finished green at 25/25 acceptance tests in 47:54 from a fresh output folder, while preserving BWFC `ListView`, `FormView`, and `GridView` on the benchmark path. Key fresh-output findings: quarantine still overreaches into benchmark pages, `MapStaticAssets()` served zero-length migrated images until replaced by `UseStaticFiles()`, and SSR cart POSTs required the full `UseAntiforgery()` + `<AntiforgeryToken />` + `@formname` contract to make quantity updates pass.

## Learnings (Run 41)
- **2026-05-07T15:15:19-04:00:** Fresh Wingtip scaffolds that copy legacy `wwwroot` assets are safer with classic `app.UseStaticFiles()` than `app.MapStaticAssets()`. On Run 41 the latter returned 200 responses with `Content-Length: 0` for logo/catalog images until the middleware was swapped.
- **2026-05-07T15:15:19-04:00:** SSR forms that rely on `Request.Form` need the whole Blazor postback contract: `app.UseAntiforgery()`, an `<AntiforgeryToken />` inside the `<form>`, and an explicit `@formname`. Missing any one of these left `ShoppingCart` quantity updates failing or timing out under Playwright.
- **2026-05-07T15:15:19-04:00:** Compile-surface quarantine should keep a benchmark-path allowlist. Stub-safe handling of Account/Admin/Checkout pages is useful, but quarantining `ProductList`, `AddToCart`, or `ShoppingCart` adds avoidable manual repair work on Wingtip fixtures.
- **2026-05-07T15:38:16-04:00:** Quarantine heuristics work better with a two-tier rule: never quarantine essential product/cart/home/contact/about pages for incidental signals, but still quarantine Account/Admin/Checkout paths or pages with strong blockers such as compile-surface failures. That keeps benchmark flows intact without weakening build safety.
- **2026-05-07T15:38:16-04:00:** Semantic page rewrites that emit raw `<form>` tags should be post-processed centrally so they always carry the SSR contract (`<AntiforgeryToken />` plus a deterministic form name). Doing that in the semantic catalog hardens both account/action pages and any future form-emitting patterns without duplicating logic.
- **2026-05-15T09:55:50-04:00:** `ComponentRefCodeBehindTransform.ClassOpenRegex` was `partial\s+class\s+\w+\s*\{` — this fails to match whenever `BaseClassStripTransform` (Order 200) runs first and rewrites the declaration to `public partial class Foo : WebFormsPageBase`. The base-class/interface list between the class name and `{` breaks the match. Fixed by changing `\s*\{` to `[^{]*\{`. This was the #1 error source in Run 80 (15+ of 28 unique CS0103 errors — `CartList`, `UpdateBtn`, etc.). The fix is surgical: one regex character class change, 3 regression tests added, 35/35 passing.
- **2026-05-15T09:55:50-04:00:** Transform interaction order contracts matter. When a code-behind transform reads a class declaration header, it must account for all transforms with lower Order values that may have modified that header. `ClassOpenRegex`-style patterns should use `[^{]*` not `\s*` between the class name and `{` to survive base-class or interface annotations added by earlier transforms.
- **2026-05-15T11:43:54.133-04:00:** Self-instantiation cleanup is safest as a dedicated post-alignment code-behind pass: after class naming settles, replace only zero-argument `new CurrentClass()` calls with `this` so DI constructor pages and helpers stop re-constructing themselves without touching user variable names or argumentful instantiations.
- **2026-05-15T11:43:54.133-04:00:** BWFC template `Text` bindings need a late markup normalization step after `TemplateContextTransform`. Converting simple `Text="@Item.X"` bindings inside item templates to `Text="@Item.X.ToString()"` fixes `TextBox.Text` string mismatches without disturbing richer expressions or non-template markup.
- **2026-05-15T11:43:54.133-04:00:** Route-parameter collision cleanup should dedupe only true case-insensitive duplicates and keep the property whose casing exactly matches the `@page` token when one exists. That preserves Blazor route binding while removing the extra `[Parameter]` member that triggers duplicate-parameter runtime failures.


≡ Team update (2026-05-07): Inbox merged, decisions consolidated — Scribe


### 2026-05-15T14:02:20Z: ComponentRefCodeBehindTransform regex fix validated

Fixed ClassOpenRegex from partial\s+class\s+\w+\s*\{ to partial\s+class\s+\w+[^{]*\{ to match classes with base classes injected by earlier transforms. Added 3 regression test cases. All 35/35 ComponentRef tests pass, full suite 729/729 green.

**Rule:** Any code-behind transform locating class body opening brace must use [^{]*{ pattern.

### 2026-05-16: Benchmark Pattern Fixes — Contoso/Wingtip Patterns 1/3/6/7

Four automation gaps from ContosoUniversity (Run 24) and WingtipToys benchmark analysis were fixed:

**Pattern 1 — "Entities" suffix DI injection (`DbContextInstantiationTransform`):**  
`ContosoUniversityEntities` (EF6 T4 naming pattern ending in "Entities") was not matched by the transform regex — added `Entities` and `DataContext` to both `NewContextRegex` and `NewContextExprRegex` suffix lists. Same fix covers LINQ-to-SQL `DataContext` subclasses.

**Pattern 3 — EDMX T4 artifact exclusion (`EdmxToEfCoreConverter`):**  
`Model1.Context.cs` and `Model1.Designer.cs` (T4-generated EF6 companion files) were not excluded by `SourceFileCopier`. The EF Core converter generates a new `ContosoUniversityEntities.cs` → CS0101 duplicate type. Fixed by adding both `{stem}.Context.cs` and `{stem}.Designer.cs` to `excludedSourceFiles` alongside the existing `{stem}.cs`.

**Pattern 6 — NamespaceAlign on BLL/source files (`SourceFileCopier`):**  
BLL files copied via `SourceFileCopier` were not having `NamespaceAlignTransform` applied because: (a) `"NamespaceAlign"` was not in the transform allow-list, (b) `FileMetadata` was not populated with `OutputRootPath`/`ProjectNamespace`, and (c) `CopySourceFilesAsync` had no way to receive `projectNamespace`. Fixed all three; `MigrationPipeline` now passes `projectName` as `projectNamespace`.

**Pattern 7 — HTML server control ID string field stubs (`InnerTextRewriteTransform`):**  
`<div id="ShoppingCartTitle" runat="server">` in markup, `ShoppingCartTitle.InnerText = "..."` in code-behind. After InnerText rewrite, `ShoppingCartTitle` was undeclared → CS0103. Transform now collects PascalCase identifiers from `.InnerText`/`.InnerHtml` patterns before rewriting, then injects `private string X = "";` stubs for any that are not already declared. camelCase and `_underscore` identifiers are intentionally excluded.

Added 9 new tests (InnerTextRewrite: 6 new, DbContextInstantiation: 4 new replaced 2, EdmxConverter: 1 new). Full suite: 802/802 green.

## Learnings (2026-05-16 — DepartmentPortal issue triage)

- **2026-05-16T15:22:00-04:00:** `gh issue create` with large PowerShell here-strings (`@"..."`) can hang indefinitely on Windows. The reliable pattern is to write the body to a file in the project directory and use `--body-file` instead. Never use `/tmp`; use a `.squad/` scratch path and clean it up immediately after.
- **2026-05-16T15:22:00-04:00:** The three DepartmentPortal Tier 1 blockers most directly owned by the CLI pipeline are: (P1) code-only control discovery via `CodeOnlyControlScaffolder`, (P2) namespace-level `tagPrefix` parsing in `WebConfigTransformer` + `LocalTagNamespaceResolutionTransform`. These are prerequisites for each other and must be built together — the scaffolder emits the stubs; the resolution transform consumes them.
- **2026-05-16T15:22:00-04:00:** When creating GitHub labels for new issue categories, confirm they don't already exist (`gh label list`) before creating. Three new labels were provisioned: `migration-toolkit`, `analyzers`, `future` — all suitable for ongoing use across CLI and analyzer issues.
- **2026-05-16T16:04:23-04:00:** The four Contoso benchmark fixes in `d591d8d2` reduce first-pass compile cleanup, but they do **not** yet move the end-to-end benchmark under five minutes because compile-surface quarantine still removes `Students`, `Courses`, and `Instructors`. For Contoso-class apps, quarantine remains the biggest lever on total benchmark time.
- **2026-05-16T16:04:23-04:00:** Legacy page-level CSS IDs such as `#ajax`, `#dropList`, and `#grvStudentsData` can shove rebuilt static-SSR controls off-screen enough for Playwright clicks to fail even when the markup is valid. When L2 rebuilds core pages, add explicit layout normalization (or isolate the legacy CSS) before judging interaction failures.
- **2026-05-17T00:00:00-04:00:** Contoso CRUD model-binding attributes (`SelectMethod`, `InsertMethod`, `UpdateMethod`, `DeleteMethod`) should be preserved in migrated markup, not rewritten to new handler names. BWFC already resolves string-based CRUD method names through `DataBoundComponent<T>`/`SelectMethodResolver`, so preserving the original attributes yields cleaner Layer 1 output and keeps CLI expectations aligned with runtime support.
- **2026-05-17T00:00:00-04:00:** GridView CRUD compatibility needs both markup and runtime support: emitting `CommandField`, preserving `BoundField ReadOnly`, and populating `GridViewUpdateEventArgs.Keys/NewValues/OldValues` plus `GridViewDeleteEventArgs.Keys`. Without all three, unchanged Contoso-style handlers still compile poorly even when the markup transform succeeds.
- **2026-05-17T08:13:43-04:00:** Contoso Run 27 reached 40/40 acceptance tests once the Students page stopped binding anonymous `object` projections directly into BWFC data controls. Typed page DTOs (`StudentGridRow`, `StudentSearchResult`) removed a prerender-time `BaseColumn<T>.ParentColumnsCollection` null reference and made the page benchmark-stable.
- **2026-05-17T08:13:43-04:00:** Interactive BWFC GridView edit flows still do not naturally round-trip live edited values into `RowUpdating.NewValues`; the benchmark page needed a compatibility-oriented workaround to satisfy edit assertions. Treat interactive GridView edit capture as an active runtime/product gap, not a solved CLI problem.
- **2026-05-17T10:01:20-04:00:** `LegacyHelperStubTransform` must never decide page/master/control skipping from `SourceFilePath` alone, because code-behind metadata keeps the markup source path (`.aspx`/`.master`/`.ascx`). The reliable contract is `FileMetadata.FileType` first, with extension checks on both source and output paths as a fallback, so Contoso page code-behinds that reference `System.Configuration` keep their real method bodies instead of being replaced by API stubs.
- **2026-05-17T10:01:20-04:00:** `DataBindTransform` regexes must consume optional `this.` prefixes on both `.DataSource = ...` and `.DataBind();` patterns. Otherwise Contoso-style code-behind leaves behind dangling `this.` tokens or misses metadata capture, turning a mechanical cleanup pass into malformed generated C#.
- **2026-05-17T12:29:20-04:00:** Contoso Run 28 confirmed that recent CLI fixes improved raw L1 page code-behind preservation even when the benchmark still regressed end to end. `Students.razor.cs`, `Courses.razor.cs`, and `Instructors.razor.cs` all kept real bodies instead of stub-only shells, so page-body preservation should now be tracked separately from final Playwright pass rate.
- **2026-05-17T12:29:20-04:00:** The remaining Contoso Run 28 failures are concentrated in Students interactive CRUD (`Add`, `Clear`, `Delete`), not broad page generation. Manual browser actions can hit the LocalDB-backed page successfully, but Playwright fill/click flows still miss the expected UI state transitions, which marks this as a BWFC/runtime interaction gap worth prioritizing over more generic CLI emit cleanup.
- **2026-05-17T13:32:41-04:00:** Contoso L1 quality regressions clustered around pipeline bookkeeping, not one broken page. The durable fixes were: normalize EDMX exclusion paths to full paths, exclude source files whose names collide with generated EF Core outputs, and delete stale excluded output artifacts on rerun so old T4 files do not quietly survive in `After*` folders.
- **2026-05-17T13:32:41-04:00:** `MarkupCleanupTransform` needs a late balancing pass that can auto-close open child tags before a parent/container closes. `Courses.aspx` proved that simply removing orphan closers is not enough; malformed legacy markup can still compile in Web Forms but must be rebalanced for Razor emission to stay syntactically valid.

## Learnings (Run 89)
- **2026-05-17T17:43:16-04:00:** When the CLI rewrites Wingtip account pages to SSR `<form method="post">` markup and already emits `/Account/LoginHandler` + `/Account/RegisterHandler` in `Program.cs`, preserving the old OWIN code-behind becomes negative value. The durable fix is to swap those pages to thin query-parameter models (or quarantine the preserved code-behind) as part of the same semantic rewrite so `GetOwinContext`, `IdentityHelper`, `ModelState`, and `User` do not leak back into the compile surface.
- **2026-05-17T17:43:16-04:00:** Visible first-pass build errors can under-report the real Layer 2 cost when a single missing namespace import blocks compilation of deeper preserved-code paths. In Run 89 the initial build showed only two `ShoppingCartActions` errors, but fixing them exposed a larger OWIN-era account backlog; for Wingtip regressions, track both the first visible error count and the total repair categories needed to reach green.
- **2026-05-17T19:20:41-04:00:** SSR form hardening belongs in a late, centralized markup pass instead of benchmark-specific L2 repairs. A single helper/transform that recognizes `<form method="post">`, `<EditForm>`, and `<WebFormsForm>` can deterministically inject the antiforgery token and filename-derived form names for both regular pipeline output and semantic-pattern output.
- **2026-05-17T19:20:41-04:00:** Filename-derived form names make benchmark reruns easier to diff and debug than anonymous counters. Emitting `StudentsForm`, `LoginForm`, or `AddToCartForm` keeps the contract stable across runs while still allowing later suffixing if a page grows multiple SSR forms.
- **2026-05-17T19:20:41-04:00:** Happy-path account pages (`Login`, `Register`) are intentionally exempt from compile-surface quarantine because the CLI already rewrites their markup to post into generated ASP.NET Core Identity handlers. The safe automation pattern is a late code-behind pass that preserves the partial class shell, strips legacy OWIN usings, and stubs method bodies once `GetOwinContext`/`IdentityHelper`-style signals remain.

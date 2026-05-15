# Bishop — Migration Tooling & Benchmark Engineer

**Owner:** Jeffrey T. Fritz  
**Role:** CLI pipeline development, WingtipToys migration benchmark automation  

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


≡ Team update (2026-05-07): Inbox merged, decisions consolidated — Scribe


### 2026-05-15T14:02:20Z: ComponentRefCodeBehindTransform regex fix validated

Fixed ClassOpenRegex from partial\s+class\s+\w+\s*\{ to partial\s+class\s+\w+[^{]*\{ to match classes with base classes injected by earlier transforms. Added 3 regression test cases. All 35/35 ComponentRef tests pass, full suite 729/729 green.

**Rule:** Any code-behind transform locating class body opening brace must use [^{]*{ pattern.

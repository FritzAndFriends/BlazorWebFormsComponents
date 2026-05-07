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

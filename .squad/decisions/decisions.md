# Team Decisions — BlazorWebFormsComponents

## 2026-05-07: WingtipToys Run 40 benchmark success (consolidated)
**By:** Bishop
**Status:** Complete
**Outcome:** 25/25 acceptance tests, 21:55 total time (down from 38:34 in Run 39), committed as a8ba153c

Summary: CLI migrations now emit credible modern scaffolds, BWFC data controls are preserved on benchmark paths, and total runtime dropped by 43%. Next highest-value work: automate benchmark runtime wiring, reduce compile-surface debt, and harden templated data-control emission.

---

### 2026-05-06: CLI compile-surface hardening for migrated apps
**By:** Bishop
**What:** Generated migration projects now disable code-style enforcement in the emitted `.csproj`, the markup pass auto-injects explicit BWFC validator generic arguments (`Type="string"` / `InputType="string"`), and the compiled code-behind pass appends fallback fields/render-methods/event handlers for unresolved markup references before pages are emitted to the compile surface.
**Why:** WingtipToys runs 29-31 regressed because migrated output started compiling more legacy files directly. That exposed style analyzers on copied source, missing validator generic arguments, and dozens of CS0103 missing-member failures when transformed markup referenced names that only existed in quarantined Web Forms artifacts.

---

### 2026-05-06: CLI-only deprecation plan for migration tooling
**By:** Bishop
**What:** Treat `webforms-to-blazor` as the only supported upstream entrypoint. Keep the PowerShell scripts only as short-lived compatibility wrappers, port NuGet asset extraction and EDMX conversion from PowerShell into native C#, and add CLI replacements for readiness scanning, asset extraction, and EDMX conversion.
**Why:** `bwfc-migrate.ps1` already forwards into the CLI, but the CLI still hides two PowerShell runtime dependencies and has no standalone replacement for `bwfc-scan.ps1`. We cannot honestly deprecate the scripts for consumers until the CLI is self-contained and exposes the remaining helper workflows directly.

---

### 2026-05-06: DisplayExpressionTransform emits idiomatic Razor for simple expressions
**Date:** 2026-05-06
**Author:** Bishop (Migration Tooling Dev)
**Status:** Implemented

Update `DisplayExpressionTransform` so simple dotted identifier display expressions emit bare Razor `@expr`, while complex expressions continue to emit `@(expr)`.

**Scope:** Simple expressions match identifier chains such as `Item.ProductName`, `Item.UnitPrice`, `Model.Title`, and `user.Email`. Complex expressions keep parentheses, including method calls, operators, ternaries, indexers, and casts.

**Rationale:** The old transform emitted `@(expr)` for every display expression. That output compiled, but it produced noisy, non-idiomatic Razor for the most common migrated shape: property access on the current item or model. Using bare `@expr` for simple member chains keeps generated Razor closer to what a Blazor developer would naturally write.

**Files Updated:** `src\BlazorWebFormsComponents.Cli\Transforms\Markup\DisplayExpressionTransform.cs`, test data

---

### 2026-05-06: Bishop decisions — G1 / G3 / G8 / G10 gap fixes (consolidated)
**By:** Bishop
**Status:** Implemented & Tested

1. **Normalize Web Forms display expressions before the main expression pass** — Added `DisplayExpressionTransform` at Order 490.
2. **Modernize EF6 DbContext constructors in Layer 1** — Added `EfContextConstructorTransform` at Order 106.
3. **Treat `Server.Transfer`, `Server.GetLastError`, and `Server.ClearError` as supported shim surface** — Extended `ServerShim` with compatibility implementations.
4. **Rewrite `HttpUtility` inline instead of depending on the compatibility shim** — Added `HttpUtilityRewriteTransform` at Order 104.
5. **Apply G3/G10 fixes to copied source files** — Updated `SourceFileCopier` to include the new transforms.
6. **Guard generated projects against the legacy HttpUtility package** — Added explicit package-strip guard in `ProjectScaffolder`.

---

### 2026-05-06: Run 37 gap fixes G1 / G2 / G4 for the CLI pipeline
**Date:** 2026-05-06  
**Author:** Bishop (Migration Tooling Dev)  
**Status:** Implemented & Tested

**G1 display-expression coverage:** `DisplayExpressionTransform` now excludes only `Bind(...)` and `Eval(...)` in its negative lookahead.

**G2 ScriptManager strip pass (Order 255):** Added `ScriptManagerStripTransform` immediately after `MasterPageTransform`. It removes `<asp:ScriptManager>` blocks, `<webopt:bundlereference>`, and placeholder blocks containing `Scripts.Render(...)`.

**G4 compile-surface page stubs (Order 850):** Added `CompileSurfaceStubTransform` for pages not in the happy path (`Login.aspx` / `Register.aspx`). Detection heuristic: lives under `Account\` / `Admin\` or references ASP.NET Identity, OWIN, `OpenAuthProviders`, or payment-service namespaces. Output emits safe stub pages, preserves code-behind artifacts under `migration-artifacts\codebehind\...`, and records `bwfc-compile-surface` manual items.

**Result:** Full CLI suite 588 passing (up from 573 baseline).

---

### 2026-05-06: ServerCodeBlockTransform and TemplateFieldChildComponentsTransform
**Date:** 2026-05-06  
**Author:** Bishop (Migration Tooling Dev)  
**Status:** Implemented & Tested

**Decision 1 — ServerCodeBlockTransform (Order 510):**
Converts Web Forms server-side statement blocks (`<% ... %>`) to Razor equivalents:
- `<% if (cond) { %>` → `@if (cond)\n{`
- `<% foreach/for/while (cond) { %>` → `@foreach/@for/@while (cond)\n{`
- `<% code; %>` → `@{ code; }`

Blocks NOT matched by the negative lookahead (`(?![#:=\-])`) are left to `ExpressionTransform`.

**Order 510 collision with LoginViewTransform:** Both use Order 510. `ServerCodeBlockTransform` is registered **first** so it runs first — intentional, as login view markup may contain server code blocks to convert before restructure.

**Decision 2 — TemplateFieldChildComponentsTransform (Order 620):**
Wraps TemplateField style child elements inside `<ChildComponents>` blocks. Supported style element names: `ItemStyle`, `HeaderStyle`, `FooterStyle`, `AlternatingItemStyle`, `SelectedItemStyle`, `EditItemStyle`.

Uses iterative `do..while` loop to handle multiple TemplateField blocks. Multi-line open/close style tags detected and accumulated correctly.

**Test coverage:** ServerCodeBlockTransform 19 tests, TemplateFieldChildComponentsTransform ~20 tests. Full CLI suite 588 passing.

---

### 2026-05-06: Generated handler stubs for verification-ready migrations
**By:** Bishop
**What:** The CLI now emits deterministic POST-based handler contracts for Wingtip-style account and action pages: account forms post to `/Account/LoginHandler` and `/Account/RegisterHandler`, action-only pages auto-post to `/__bwfc/actions/{PageName}`, and `Program.cs` gets matching minimal API stubs plus antiforgery disablement for those generated forms.
**Why:** Acceptance coverage is moving from transform-only assertions to migrated-app output verification. The generated project needs routable page shells and compile-safe HTTP endpoints so master/content rendering, add-to-cart navigation, and login/register form submissions can be exercised before any app-specific Layer 2 cleanup.

---

### 2026-05-06: Bishop P0 native services — replace PowerShell with C#
**Date:** 2026-05-06
**Owner:** Bishop

Replace the CLI's remaining runtime PowerShell helper dependencies with native C# services and expose matching CLI entrypoints.

**Decision:**
1. Add `src\BlazorWebFormsComponents.Cli\Services\NuGetStaticAssetExtractor.cs` as the native implementation for package static asset discovery.
2. Add `src\BlazorWebFormsComponents.Cli\Services\EdmxToEfCoreConverter.cs` as the native EDMX parser/generator using `System.Xml.Linq`.
3. Rewire `Program.cs` and `MigrationPipeline` to depend on these native services instead of PowerShell bridge execution.
4. Add CLI command surface for `webforms-to-blazor scan`, `webforms-to-blazor assets extract`, and `webforms-to-blazor edmx convert`.
5. Leave legacy PowerShell scripts in place only as deprecated wrappers for the compatibility window.

**Consequences:** Full CLI migration no longer requires spawning `powershell` or `pwsh`. Deprecation banners now point at real CLI commands. Remaining PowerShell retirement narrowed to future wrapper removal.

---

### 2026-05-06: Run 34 Results — Cart Architecture and Playwright Patterns
**From:** Bishop (Migration Tooling Dev)  
**Date:** 2026-05-06  
**Re:** WingtipToys Migration Run 34 benchmark results

**New Transforms Validated:**
- **`ServerCodeBlockTransform` (Order 510):** Converts `<% %>` blocks to `[% %]` notation, preventing RZ9980 parse errors.
- **`TemplateFieldChildComponentsTransform` (Order 620):** Auto-wraps `<ItemStyle>` etc. in `<ChildComponents>` for GridView/TemplateField columns.

Net effect: 2 fewer manual fixes needed.

**Key Architectural Decisions:**

1. **Session-keyed CartSessionStore for Blazor Server:** Use a **singleton `CartSessionStore`** (`ConcurrentDictionary<string, Dictionary<int, CartEntry>>` keyed by a stable cart ID stored in the ASP.NET Core session cookie). Pattern recommended for all state surviving the SSR→circuit transition.

2. **Cart Mutations as Minimal API Endpoints:** Implement cart add/remove as **Minimal API `MapGet` endpoints** that load the session, mutate the `CartSessionStore`, and return `Results.Redirect(...)` (HTTP 302). Makes cart operations pure HTTP round-trips observable by Playwright's `NetworkIdle` waiter.

3. **Avoiding Ambiguous Route Conflicts:** Remove the `@page` directive from `AddToCart.razor` when a Minimal API owns the route. Keep the component file for layout/rendering; only the `@page` directive creates the routing conflict.

**Final Metrics (Run 34):** 25/25 acceptance tests passing, initial build errors 266 → 0, manual fixes eliminated 2.

---

### 2026-05-06: Wingtip Run 35 follow-ups
**Date:** 2026-05-06 16:54:14 -0400
**Author:** Bishop
**Status:** Proposed

**Findings:**
1. Width/Height string handling reduced fresh-run repair immediately.
2. `System.Web.HttpUtility` shim still not safe when migrated apps bring the legacy package — sample hit symbol ambiguity.
3. Layer 1 emits too many compile-surface account/admin/mobile pages with unresolved members.

**Recommended follow-up:**
- CLI should rewrite legacy `HttpUtility` call sites to `System.Net.WebUtility` when migrated project also references the legacy package.
- Add deterministic cleanup for invalid Razor patterns (`@(:...)`, raw `<%#:` blocks).
- Consider benchmark-friendly option to emit placeholder pages for unported account/admin/mobile surfaces.

---

### 2026-05-06: Bishop Run 36 Findings
**Date:** 2026-05-06 18:55:09 -0400  
**Author:** Bishop  
**Status:** Proposed

Run 36 validated the fresh transform set and finished green (25/25 acceptance tests), but exposed repeatable migration-tooling gaps:

1. **DisplayExpressionTransform still incomplete** — `ShoppingCart.razor` still contained raw `<%#:` expression.
2. **EfContextConstructorTransform needs follow-up call-site rewrite** — Generated callers still emitted `new ProductContext()`.
3. **ServerShim compatibility needs overload or paired rewrite** — `Server.Transfer(path, true)` missing.
4. **Master-page shell output remains bottleneck** — Still included unsupported `Scripts.Render(...)` and `webopt:bundlereference`.

**Recommended Follow-up:**
- Extend display-expression cleanup for templated child content.
- Add transform that rewrites `new ProductContext()` call sites.
- Add `Transfer(string, bool)` compatibility or rewrite.
- Continue reducing master-page shell to plain Blazor-safe constructs.

---

### 2026-05-06: Bishop Run 37 Findings (consolidated)
**Date:** 2026-05-06  
**Author:** Bishop

Run 37 priority summary:
1. Follow-up transform for templated display expressions — still emitted raw `<%#:` inside `ShoppingCart.razor`.
2. Automatic repair or quarantine for non-happy-path routable pages (`Account/*`, `Admin/*`, `Checkout/*`).
3. Master-page shell cleanup as first-class toolkit gap — `Site.razor` emitted unsupported bundle/script-manager constructs.
4. Simple dotted-identifier `@expr` output is readability win, but performance depends on invalid Razor removal and compile-surface reduction.

---

### 2026-05-06: Bishop Run 39 Gap Analysis
**Author:** Bishop
**Status:** Analyzed

**P0 — Must fix to keep the benchmark green:**
1. **Acceptance-path runtime wiring** — Fresh output needed manual catalog, cart, auth setup. Fix: Add Wingtip benchmark scaffold mode. Impact: 5-8 min/run.
2. **Compile-surface debt outside benchmark path** — Fresh output shipped long tail of Account/Admin/Checkout/mobile pages. Fix: Expand CLI quarantine/stub. Impact: 4-6 min/run.

**P1 — Important correctness improvements:**
3. **FormView SSR first-render behavior** — `CurrentItem` not available during SSR. Fix: Already done. Impact: 3-4 min.
4. **Session raw-string round-trip** — Page-side cart/session lookups could miss raw string values. Fix: Already done. Impact: 2-3 min.

**P2 — Smaller but worthwhile:**
5. **Generate request-safe session access patterns** — Benchmark pages needed manual repair. Impact: 1-2 min.
6. **Port/process safety during iterative validation** — `MSB3027/MSB3021` lock failures. Impact: 1 min.

**P3 — Nice to have:**
7. **Benchmark-path diagnostics** — Blank SSR or session mismatches. Impact: <1 min.

**Run 38 → Run 39 progress:**
- Acceptance-path data controls no longer replaced by manual HTML.
- `FormView/query-details` gap materially improved.
- Cart path closer to framework-correct behavior.

**Still persisting:**
- Runtime scaffold gap — fresh output still needs manual catalog/cart/auth setup.
- Compile-surface exclusion/quarantine gap.
- Checkout/payment and heavy account/admin surfaces outside automatic coverage.

---

### 2026-05-06: Run 34 shim compatibility and attribute data-binding transform
**Date:** 2026-05-06
**Author:** Bishop (Migration Tooling Dev)
**Status:** Proposed

Run 34 exposed four deterministic migration gaps:
1. BWFC components accepted `Width="500"` but not CSS-style string literals (`Width="500px"`).
2. `RequestShim.QueryString` surfaced `IQueryCollection` (indexer only, no `.Get()`).
3. Migrated code referenced `System.Web.HttpUtility`.
4. CLI handled `<%# ... %>` in content but not in attribute values.

**Decision:**

1. **Width/Height as string-backed parameters** — Use strings, parse internally as `Unit`.
2. **Query string `.Get()` compatibility** — Add `BlazorWebFormsComponents.QueryStringExtensions.Get(string)` extension.
3. **`System.Web.HttpUtility` compatibility shim** — Add static type backing `System.Net.WebUtility`.
4. **Attribute data-binding transform** — Add `DataBindingAttributeTransform` (Order 615) rewriting `<%# ... %>` / `<%= ... %>` in attributes to Razor `@(...)`.

**Rationale:** All four are deterministic compile-surface issues. Leaving to Layer 2 wastes manual time and creates false negatives.

---

### 2026-05-06: Wingtip acceptance split across live-app and CLI build tests
**By:** Colossus
**What:** Wingtip migration regression coverage now lives in two layers: Playwright acceptance tests in `samples/AfterBlazorServerSide.Tests` validate the runnable migrated app output, while `tests/BlazorWebFormsComponents.Cli.Tests` builds scaffolded output after compile-surface quarantine to verify generated-app cleanliness.
**Why:** We needed one layer that proves the committed migrated app actually renders and handles cart/account flows in the browser, and another that proves quarantining legacy compile-surface files still leaves a buildable generated project.

---

### 2026-05-06T09:16: User directive — CLI deprecation
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Deprecate PowerShell migration scripts. Upstream consumers should use the CLI tool (webforms-to-blazor) exclusively. Migrate all functionality from bwfc-migrate.ps1 into the CLI.
**Why:** User request — PS1 scripts are not acceptable for upstream consumers. CLI is the canonical migration tool going forward.

---

### 2026-05-06T15:12: User directive — Width/Height audit
**By:** Jeffrey T. Fritz (via Copilot)
**What:** Review ALL BWFC component attributes for the Width="500" pattern — components should accept string values and parse internally rather than requiring @(500) Razor expressions. This is a systematic audit action item.
**Why:** User request — the BWFC philosophy is that migrated markup compiles unchanged. Integer/Unit attributes that reject string input violate this principle.

---

### 2026-05-07T09:24: User directive — P0 benchmark rule
**By:** Jeffrey T. Fritz (via Copilot)
**What:** NEVER replace ListView, FormView, or GridView with manual HTML/features during migration benchmarks. The BWFC library provides these as Blazor components — they MUST be used. The entire point of BWFC is drop-in replacement. Manual reimplementation defeats the purpose.
**Why:** User request — captured for team memory. This is a P0 benchmark integrity rule.

---

### 2026-05-08: Update Copilot migration instructions with CLI and toolkit guidance
**By:** Bishop (Migration Tooling Dev)
**Status:** Proposed
**Date:** 2026-05-08T10:00:31-04:00

Expand `.github/copilot-instructions.md` so migration-focused agents are steered toward the BWFC CLI pipeline, migration-toolkit wrapper, static SSR scaffold, quarantine boundaries, acceptance suites, and BWFC component preservation rules.

**Why:** The previous instructions were strong for component development but left migration agents without the repo's actual migration operating model. That gap encouraged ad hoc conversions, missed transform registration in tests, and made it easier to rewrite shim-backed code or replace BWFC data controls with manual HTML during benchmark repairs.

**Required Guidance Added:**
- Project structure now includes the CLI project, analyzer projects, migration-toolkit, CLI tests, and benchmark acceptance suites.
- Migration rules now require static SSR, trust in `WebFormsPageBase` shims, BWFC data-control preservation, toolkit-first entry points, and dual transform registration in `Program.cs` plus `TestHelpers.CreateDefaultPipeline()`.
- A new migration CLI and toolkit section documents `MigrationPipeline`, `ProjectScaffolder`, `RuntimeDetector`, `ProgramCsEmitter`, `PageQuarantineDetector`, the five migration-toolkit skills, and the benchmark acceptance-test commands.
- The maintenance matrix now covers CLI transforms, scaffolding/runtime changes, and migration-toolkit workflow changes.

**Expected Outcome:** Future agents should start migrations from the supported toolkit path, preserve benchmark-critical BWFC controls and pages, and keep the CLI runtime, test pipeline, and documentation synchronized while pursuing a fast WingtipToys migration loop.

---

## Summary

**Active Contributors:** Bishop, Colossus, Scribe  
**Last Update:** 2026-05-08T14:42:43Z  
**Next Review:** Post-Bishop-spawn (G3/G4 fixes)

---

### 2026-05-07: Bishop — fix BWFC data-control template emission
**By:** Bishop
**Status:** Complete
**Outcome:** GridView typed columns inherit parent row type, ListView GroupTemplate/LayoutTemplate emit explicit fragment contexts. 603 CLI tests passing (up from 598). Committed as 1bdbb1f6


**Date:** 2026-05-07T13:17:32-04:00  
**By:** Bishop  
**Requested by:** Jeffrey T. Fritz

## Decision
Add a dedicated CLI markup transform to propagate typed `GridView ItemType` values down into child BWFC column components, and extend template-context normalization so `ListView` group/layout placeholders emit explicit fragment contexts.

## What changed
1. Added `GridViewColumnItemTypeTransform` at Order 705 after `AttributeStripTransform`.
2. Rewrote typed `GridView` child `BoundField`, `TemplateField`, `HyperLinkField`, and `ButtonField` tags from `ItemType="object"` to the parent grid item type.
3. Extended `TemplateContextTransform` so `GroupTemplate` emits `Context="items"` and `LayoutTemplate` emits `Context="groups"` when no explicit context exists.
4. Added CLI tests covering Wingtip-style `ShoppingCart` and `ProductList` emission.
5. Updated CLI docs to document the new transform behavior.

## Why
Run 40 showed Layer 1 output still needed manual structural cleanup on flagship BWFC data controls. The worst failure mode was typed `GridView` templates compiling against `object`, which makes generated expressions like `@Item.Quantity` invalid. Explicit ListView placeholder contexts also make the generated template structure trustworthy and easier to inspect.

## Validation
- `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo`
- `dotnet run --project src\BlazorWebFormsComponents.Cli -- convert -i samples\WingtipToys\WingtipToys\ProductList.aspx -o .\bishop-output --overwrite`
- `dotnet run --project src\BlazorWebFormsComponents.Cli -- convert -i samples\WingtipToys\WingtipToys\ProductDetails.aspx -o .\bishop-output --overwrite`
- `dotnet run --project src\BlazorWebFormsComponents.Cli -- convert -i samples\WingtipToys\WingtipToys\ShoppingCart.aspx -o .\bishop-output --overwrite`

## Consequences
Layer 1 now emits cleaner BWFC ListView placeholders and correctly typed GridView columns for Wingtip-style pages, reducing manual repair on the benchmark path. FormView item templates continue to emit valid typed fragments, and the CLI suite now guards this contract.



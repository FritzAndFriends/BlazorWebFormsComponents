# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Learnings

<!--  Summarized 2026-02-27 by Scribe  covers M1M16 -->

<!-- ⚠ Summarized 2026-03-06 by Scribe — older entries archived -->

### Archived Sessions

- Core Context (2026-02-10 through 2026-02-27)
- M17-M20 Wave 1 Context (2026-02-27 through 2026-03-01)
- M20 Theming through Migration Benchmarks (2026-03-01 through 2026-03-04)
- Script & Toolkit Summary (2026-03-02 through 2026-03-04)
- GetRouteUrl, Run 5 & Toolkit Sync Summary (2026-03-04 through 2026-03-05)

<!-- ⚠ Summarized 2026-03-07 by Scribe — entries from 2026-03-05 through 2026-03-07 (pre-Run 11) archived -->

- Run 6 Script Enhancements (2026-03-05)
- @rendermode Scaffold Fix (2026-03-05)
- WebFormsPageBase Implementation (2026-03-05)
- WebFormsPage IPageService Consolidation (2026-03-05)
- LoginView Migration Script Fix (2026-03-06)
- Run 9 Script Fixes — 9 RF items (2026-03-06)
- Layer 2 AfterWingtipToys Build Conversion (2026-03-06)

### Summary (2026-03-05 through 2026-03-07 pre-Run 11)

Run 6: 4 script enhancements (TFM, SelectMethod TODO, wwwroot copy, stubs). @rendermode fix: removed standalone directive from _Imports.razor scaffold — `@rendermode` is a directive *attribute* for component instances only. WebFormsPageBase: `ComponentBase` subclass with `Page => this`, Title/MetaDescription/MetaKeywords delegates, `IsPostBack => false`. WebFormsPage consolidation: merged Page.razor head rendering into WebFormsPage via Option B. LoginView script fix: `<asp:LoginView>` → `<LoginView>` (not AuthorizeView), preserve template names. Run 9: 9 script fixes (Models copy, DbContext transform, EF6→EF Core, redirect detection, Program.cs boilerplate, Page Title extraction, QueryString/RouteData annotations, ListView GroupItemCount, csproj packages). Layer 2: full AfterWingtipToys conversion — key pattern: layout code-behind class name MUST match .razor filename. Auth pages use plain HTML forms with HTTP endpoints.

<!-- summarized 2026-03-11 by Scribe -- entries from 2026-03-07 through 2026-03-08 (Runs 11-13, script fixes) archived -->

- Run 11 -- Complete WingtipToys Migration from Scratch (2026-03-07)
- Run 11 Script Fixes -- Fix 1 (Invoke-ScriptAutoDetection) & Fix 2 (Convert-TemplatePlaceholders) (2026-03-07)
- Run 12 -- Complete WingtipToys Migration from Scratch (2026-03-07)
- LoginView Namespace Fix (2026-03-07)
- Run 13 -- Full WingtipToys Migration Pipeline, 25/25 tests (2026-03-08)

### Summary (2026-03-07 through 2026-03-08)

Run 11: Fresh WingtipToys migration from scratch (105 files, 0 errors). Key patterns: root-level `_Imports.razor` for pages outside `Components/`, partial classes must NOT specify `: ComponentBase` with `@inherits WebFormsPageBase`, auth pages use plain HTML forms to HTTP endpoints. Run 11 script fixes: `Invoke-ScriptAutoDetection` (JS files to wwwroot/Scripts/ with correct dependency order) and `Convert-TemplatePlaceholders` (placeholder elements to `@context`). Run 12: Full pipeline with Layer 2, established dual DbContext pattern (later superseded by factory-only in Run 13). LoginView namespace fix: `@using BlazorWebFormsComponents.LoginControls` required in `_Imports.razor` -- added to script template. Run 13: 25/25 tests passed (100%). Confirmed patterns: SSR default, `data-enhance-nav="false"` for minimal API links, `data-enhance="false"` for auth forms, `AddDbContextFactory` only (no dual registration), middleware order `UseAuthentication -> UseAuthorization -> UseAntiforgery`, logout must use `<a>` not `<button>`.

Team updates (2026-03-07 through 2026-03-08): Coordinator must not perform domain work; FreshWingtipToys must not be committed; migration-standards updated with Static Asset Checklist/ListView Placeholder/Action Links; migration order: fresh project first; SSR default with InteractiveServer opt-in; enhanced nav bypass for minimal API; DbContext factory-only; middleware order confirmed; logout uses link not button.

### ServiceCollectionExtensions Enhancement (2026-03-11)

**Completed:** Enhanced `AddBlazorWebFormsComponents()` with auto-registered services, options pattern, and middleware pipeline.

**Changes:**
1. **`AddHttpContextAccessor()`** now auto-registered — `BaseWebFormsComponent` line 57 injects `IHttpContextAccessor`, so every consumer needed this. It's idempotent, safe to call multiple times.
2. **`BlazorWebFormsComponentsOptions`** class added — configurable behavior via `Action<BlazorWebFormsComponentsOptions>` overload. Currently supports `EnableAspxUrlRewriting` (default: true).
3. **`UseBlazorWebFormsComponents()`** middleware extension on `IApplicationBuilder` — reads options from DI, conditionally enables `.aspx` URL rewriting.
4. **`AspxRewriteMiddleware`** — intercepts `.aspx` requests, issues 301 permanent redirects to clean URLs. `Default.aspx` → `/`, `Students.aspx` → `/Students`, preserves query strings.
5. **FrameworkReference** added to csproj (`Microsoft.AspNetCore.App`) — needed for `AddHttpContextAccessor()` and middleware APIs.
6. **All 3 sample Program.cs files updated** — removed manual `AddHttpContextAccessor()`, added `app.UseBlazorWebFormsComponents()`.

**Suggested future enhancements:** Custom error page middleware, ViewState cleanup, `.ashx`/`WebResource.axd` handling, `~/App_Themes/` mapping, auto-register `CascadingAuthenticationState` for LoginView.

📌 Team update (2026-03-11): Migration tests reorganized — `dev-docs/migration-tests/` now uses `wingtiptoys/runNN/` and `contosouniversity/runNN/` structure. Future run reports must follow this convention. — decided by Beast

### Run 18a/18b/18c — Test-UnconvertiblePage False-Positive Investigation (2026-03-11)

**Run 18a:** ShoppingCart.aspx incorrectly stubbed — `'Checkout'` pattern matched `CheckoutImageBtn` button ID. 6 build errors from `[Parameter]` TODO annotation swallowing property declarations. **Run 18b:** Checkout pattern fixed (path-based `'^Checkout[/\\]'`), but `'PayPal'` pattern still matched image URL/alt text — second false positive. **Run 18c:** ✅ Both patterns fixed. ShoppingCart.razor now contains full GridView markup. Stubs: 5 (Checkout/ only), transforms: 314, timing: 1.51s. Key learning: content-based unconvertible detection must avoid matching URLs/image paths/alt text.

 Run 18c (2026-03-11): ShoppingCart.aspx GridView fix CONFIRMED 
- Both false-positive patterns now fixed: 'Checkout' (Run 18b) and 'PayPal' (Run 18c)
- ShoppingCart.razor now contains full GridView with BoundField/TemplateField markup  NOT a stub
- UnconvertibleStub count: 5 (Checkout/ folder only)  down from 6
- Transforms increased: 303  314 (ShoppingCart now fully processed)
- Layer 1 timing: 1.51s
- Build: same 6 pre-existing errors in ProductDetails/ProductList code-behinds (not ShoppingCart)
- Key learning: content-based unconvertible detection must avoid matching URLs, image paths, alt text  use code-behind analysis or path-based detection for payment/checkout patterns


📌 Team update (2026-03-11): Run 18 improvement recommendations prioritized by Forge — see decisions.md



📌 Team update (2026-03-11): User directives from Jeff  eliminate Test-UnconvertiblePage, standardize on ItemType, P0-2 approved  see decisions.md

### P0 Migration Script Fixes (2026-03-11)

**FIX 1 — P0-1: Eliminated Test-UnconvertiblePage stubbing**
- Gutted `Test-UnconvertiblePage` (line 1233) to always return `$false` — no page is ever stubbed now
- Replaced the call site (formerly line 1397–1406) with a TODO-injection block: Checkout/ pages and Identity/Auth pattern pages get a `@* TODO: ... *@` Razor comment at the top + ManualItem entry, but are ALWAYS fully converted through the BWFC transform pipeline
- PayPal ImageButton in ShoppingCart.aspx now converts normally as a BWFC component
- `New-CompilableStub` function (line 1261) is now dead code but retained for reference

**FIX 2 — P0-2: [Parameter] RouteData TODO annotation bug**
- Changed regex (line 1206) from `\[RouteData\]` to `([ \t]*)\[RouteData\]` to capture leading whitespace
- Replacement now puts TODO comment on a SEPARATE LINE above `[Parameter]`, preserving indentation via `${1}` backreference
- Before: `[Parameter] // TODO: ... {parameter} public string productName)` — the `//` comment swallowed the property declaration
- After: TODO on its own line, then `[Parameter]` on the next line, property type/name preserved
- Eliminates 6 build errors (CS1031, CS1001, CS1026) per project with RouteData parameters

**Patterns observed:**
- PowerShell `[regex]::Replace` with `//` in replacement text is dangerous when the match is on the same line as subsequent code — always put `//` comments on their own line in replacement strings
- Page stubbing destroys converted markup; TODO annotation preserves it while flagging concerns

### Standardize Generic Type Params to ItemType (2026-03-11)

**Completed:** Renamed all remaining `TItemType` and `TItem` generic type parameters to `ItemType` across the BWFC library source.

**Files changed (Group 1 — base classes):**
- `src/BlazorWebFormsComponents/DataBinding/DataBoundComponent.cs` — `TItemType` → `ItemType` (class declaration + 10 internal references)
- `src/BlazorWebFormsComponents/DataBinding/SelectHandler.cs` — `TItemType` → `ItemType` (delegate declaration)
- InsertHandler.cs, UpdateHandler.cs, DeleteHandler.cs — confirmed these files do not exist

**Files changed (Group 2 — list controls):**
- `src/BlazorWebFormsComponents/DataBinding/BaseListControl.cs` — `TItem` → `ItemType` (class declaration, typeparam doc, 2 method usages)
- `src/BlazorWebFormsComponents/BulletedList.razor` — `@typeparam TItem` → `@typeparam ItemType`, `@inherits` updated
- `src/BlazorWebFormsComponents/BulletedList.razor.cs` — `TItem` → `ItemType` (class declaration + typeparam doc)
- `src/BlazorWebFormsComponents/CheckBoxList.razor` — `@typeparam TItem` → `@typeparam ItemType`, `@inherits` updated
- `src/BlazorWebFormsComponents/CheckBoxList.razor.cs` — `TItem` → `ItemType` (class declaration + typeparam doc)
- `src/BlazorWebFormsComponents/DropDownList.razor` — `@typeparam TItem` → `@typeparam ItemType`, `@inherits` updated
- `src/BlazorWebFormsComponents/DropDownList.razor.cs` — `TItem` → `ItemType` (class declaration + typeparam doc)
- `src/BlazorWebFormsComponents/ListBox.razor` — `@typeparam TItem` → `@typeparam ItemType`, `@inherits` updated
- `src/BlazorWebFormsComponents/ListBox.razor.cs` — `TItem` → `ItemType` (class declaration + typeparam doc)
- `src/BlazorWebFormsComponents/RadioButtonList.razor` — `@typeparam TItem` → `@typeparam ItemType`, `@inherits` updated
- `src/BlazorWebFormsComponents/RadioButtonList.razor.cs` — `TItem` → `ItemType` (class declaration + typeparam doc)

**Group 3 verification (already correct):**
- GridView, DataGrid, DataList, Repeater, ListView, DetailsView, FormView — all already use `ItemType`
- BaseColumn, BoundField, ButtonField, TemplateField, HyperLinkField — all already use `ItemType`
- BaseRow, DataGridRow, GridViewRow, GroupTemplate — all already use `ItemType`

**Final grep:** Zero remaining `TItemType` or `TItem` references in library source.
**Build result:** 0 errors (93 pre-existing warnings, all unrelated to this change)


 Team update (2026-03-11): Mandatory L1L2 migration pipeline  no code fixes between layers. Both layers must run in sequence.  decided by Jeffrey T. Fritz

 Team update (2026-03-11): All generic type params standardized to ItemType (not TItem/TItemType) across all BWFC data-bound components.  decided by Jeffrey T. Fritz

 Team update (2026-03-11): P0 script fixes  Test-UnconvertiblePage eliminated (always convert), [Parameter] annotation bug fixed.  decided by Jeffrey T. Fritz

# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents  Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Active Decisions & Alerts

📌 **Team update (2026-03-16):** Forge reviewed Component Health Dashboard PRD; 3 errata items identified before Cyclops implementation. (1) Appendix A: ToolTip base class error. (2) tools/WebFormsPropertyCounter/ doesn't exist—use MSDN curation as Phase 1 primary. (3) Acceptance criterion #9 needs verification (Login controls had 0 bUnit tests as of 2026-03-15). See decisions.md for full details. — decided by Forge

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

<!-- ⚠ Summarized 2026-03-11 by Scribe — entries from 2026-03-11 (ServiceCollectionExtensions through L2 structural transform) archived -->

- ServiceCollectionExtensions Enhancement (2026-03-11)
- Run 18a/18b/18c — Test-UnconvertiblePage False-Positive Investigation (2026-03-11)
- P0 Migration Script Fixes (2026-03-11)
- Standardize Generic Type Params to ItemType (2026-03-11)
- Layer 2 Structural Transform - AfterWingtipToys (2026-03-11)

### Summary (2026-03-11 pre-Run 20)

ServiceCollectionExtensions: Added `AddHttpContextAccessor()` auto-registration, `BlazorWebFormsComponentsOptions` with `EnableAspxUrlRewriting`, `UseBlazorWebFormsComponents()` middleware, `AspxRewriteMiddleware` for `.aspx` → clean URL 301 redirects. Run 18: Fixed `Test-UnconvertiblePage` false positives — 'Checkout' matched button IDs, 'PayPal' matched image URLs. Both now path-based. ShoppingCart.razor restored to full GridView markup. P0 fixes: (1) Eliminated `Test-UnconvertiblePage` — always returns `$false`, pages get TODO annotations instead of stubs. (2) `[Parameter]` regex fix — TODO on separate line, eliminates 6 CS errors. ItemType standardization: renamed all `TItemType`/`TItem` → `ItemType` across 13 files, 0 build errors. L2 AfterWingtipToys: ~60 files transformed, 0 errors, 58 RZ10012 warnings. Key gotchas: class name must match .razor filename, `@inject`/`[Inject]` duplication, `#hexcolor` → `@("...")`, `LoggedInTemplate` has no typed context.

Team updates (2026-03-11): Migration tests reorganized to `project/runNN/`. Mandatory L1→L2 pipeline with no fixes between layers. All generics standardized to `ItemType`. Test-UnconvertiblePage eliminated. Run 18 improvements prioritized by Forge.

### UpdatePanel ContentTemplate Enhancement (2026-03-13)

**Summary:** ContentTemplate RenderFragment parameter added to UpdatePanel; base class changed to BaseStyledComponent; dual syntax support (Web Forms + Blazor); all 24 tests pass.

**Impact:** Eliminates RZ10012 warnings during migration. UpdatePanel now supports full styling capabilities. No breaking changes.

📌 Team update (2026-03-13): UpdatePanel enhancement complete — ContentTemplate parameter + BaseStyledComponent base class + dual syntax support. Cyclops (component), Rogue (12 tests, 10 pass now), Jubilee (sample page + ComponentList update), Colossus (3 interaction tests). All 4 UpdatePanel integration tests passing. Decisions merged to decisions.md.

### Students GridView LEFT JOIN Fix + Test Timing (2026-03-14)

**Summary:** Fixed data-loss bug in `GetJoinedTableData()` by replacing INNER JOIN with LEFT JOIN. Verified Playwright test timing already correct.

**Impact:** Students without enrollments now visible in GridView. Blazor Server form submission timing stable.

📌 Team update (2026-03-14): Students LEFT JOIN fix completed by Cyclops — replaced SelectMany (INNER JOIN) with Students.Include(Enrollments) loop. Students without enrollments appear with Count=0, Date=DateTime.Today. Colossus verified Playwright test timing fixes already in place from previous session. All tests passing. Commit d3dc610f.

### L1 Script Bug Fixes + Test Coverage Expansion (#472)

**Summary:** Fixed 3 bugs in bwfc-migrate.ps1 that caused test failures, added 5 new test cases covering all five L1 patterns from issue #472. Test suite: 7/10 → 15/15 (100%), line accuracy: 94.3% → 100%.

**Bugs fixed:**
1. `ConvertFrom-GetRouteUrl` — `Eval()` regex was global instead of scoped to GetRouteUrl lines. Corrupted `<%#: Eval("Name") %>` expressions by stripping the Eval wrapper but leaving `<%#: %>` delimiters. Fix: only apply Eval→context conversion on lines containing `GetRouteUrl`.
2. `ConvertFrom-ContentWrappers` — `\s*\r?\n?` after `>` consumed leading indentation of the next line. Fix: changed to `[ \t]*\r?\n?` (horizontal whitespace only).
3. `Remove-WebFormsAttributes` — ItemType fallback regex `(?![^>]*ItemType=)` didn't check for `TItem=`, so tags that already had `ItemType` converted to `TItem` got a duplicate `ItemType="object"`. Fix: lookahead now checks `(?![^>]*(?:ItemType|TItem)=)`.

**New test cases:**
- TC11-BoolEnumUnit: boolean lowercase, enum type-qualifying, unit px-stripping
- TC12-DataSourceID: DataSourceID removal + data source control → TODO replacement
- TC13-ResponseRedirect: Response.Redirect → NavigationManager.NavigateTo in code-behind
- TC14-SessionDetect: Session["key"] detection with migration guidance block
- TC15-ViewState: ViewState["key"] detection with private field suggestions

**Test harness enhancements:** Extended Run-L1Tests.ps1 to copy `.aspx.cs` inputs and compare `.razor.cs` expected output, enabling code-behind transform verification.

**Key files:** `migration-toolkit/scripts/bwfc-migrate.ps1`, `migration-toolkit/tests/Run-L1Tests.ps1`, `migration-toolkit/tests/inputs/TC11-TC15*`, `migration-toolkit/tests/expected/TC11-TC15*`

### Run 22 L1 Script Fixes — 5 Migration Toolkit Improvements (2026-03-14)

**Summary:** Implemented 5 script fixes identified from ContosoUniversity Run 22 (39/40 tests, 97.5% pass rate): (1) Strip ContentTemplate wrappers to eliminate RZ10012 warnings, (2) Add dual `@page` routes for home pages (e.g., `/Home` + `/`), (3) Extract PageTitle from TitleContent placeholders, (4) Convert `ID=` to `id=` for HTML compatibility, (5) Add `[DatabaseGenerated(Computed)]` support to EDMX parser.

**Files Modified:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (Fixes 1-4)
- `migration-toolkit/scripts/Convert-EdmxToEfCore.ps1` (Fix 5)

**Key Implementation Details:**
- **Fix 1 (ContentTemplate):** Added after asp: prefix removal in `ConvertFrom-AspPrefix` — strips `<ContentTemplate>` wrapper tags while preserving content. UpdatePanel component accepts ContentTemplate as RenderFragment parameter, but L1 output is cleaner without wrapper tags.
- **Fix 2 (Home Routes):** Added `$isHomePage` detection in `ConvertFrom-PageDirective` for Home.aspx/Default.aspx/Index.aspx — generates both `@page "/Home"` and `@page "/"` directives. Ensures root URL routing works.
- **Fix 3 (TitleContent):** Added regex in `ConvertFrom-ContentWrappers` to extract title from `<asp:Content ContentPlaceHolderID="TitleContent">` blocks. Stored in `$script:ExtractedTitleFromContent`, consumed by `ConvertFrom-PageDirective` as fallback when `Title` attribute absent. Complements existing Title attribute extraction (Run 9 RF-10).
- **Fix 4 (ID → id):** Added in `Remove-WebFormsAttributes` after ItemType processing — broad regex replacement of `ID="value"` to `id="value"`. BWFC components accept both, so safe replacement. Ensures CSS/JS selectors work.
- **Fix 5 (EDMX Computed):** Added `IsComputed` property to entity metadata, generated `[DatabaseGenerated(DatabaseGeneratedOption.Computed)]` annotation in entity files. Mirrors existing `IsIdentity` pattern. Prevents EF Core errors on computed columns.

**Expected Impact:**
- Eliminates 3 RZ10012 warnings (ContentTemplate)
- Fixes 3 HomePageTests (root URL routing)
- Improves browser tab titles (SEO + UX)
- Fixes selector failures from ID casing
- Prevents EF Core computed column errors

**Gotchas:**
- Fix 3 uses script-scoped variable to pass data between functions — must clear `$script:ExtractedTitleFromContent` after use to avoid cross-file pollution
- Fix 2 checks `$isHomePage -and $route -ne '/'` to avoid duplicate `@page "/"` directives
- Fix 1 placed after closing tag removal to avoid interfering with asp: prefix stripping

📌 Team update (2026-03-14): Run 22 L1 script fixes implemented by Cyclops — 5 improvements targeting RZ10012 warnings, home page routing, PageTitle extraction, ID normalization, and EDMX computed properties. Decision document written to `.squad/decisions/inbox/cyclops-l1-script-fixes.md`.



- Run 20 L1 Script Fixes — SelectMethod Preservation + Review Item Noise Reduction (2026-03-12)
- Run 21 — Layer 2 Structural Transform AfterWingtipToys (2026-03-11)
- Layer 2 Structural Transform — AfterContosoUniversity (2026-03-12)
- L1 Script SQLite → SQL Server Fix (2026-03-12)

### Summary (2026-03-11 through 2026-03-12)

Run 20 L1 fixes: `ConvertFrom-SelectMethod` was stripping SelectMethod from markup — fixed to preserve it with TODO for L2 delegate conversion. ContentPlaceHolder and GetRouteUrl review items updated to reference BWFC components. Run 21 WingtipToys L2: 44 files, 0 errors. SelectMethod delegate conversion on 4 data pages (`SelectMethod="@GetProducts"` etc.), all code-behinds to async lifecycle, enum/type Razor escaping (`@GridLines.Vertical`), account pages stubbed for Identity migration. CU L2: 0 errors, 5 models to EF Core, 5 BLL classes with IDbContextFactory, EDMX artifacts deleted, raw SQL → LINQ. L1 script: SQLite package/example replaced with SqlServer/LocalDB.

**Key gotchas (combined):** Code-behind class name MUST match .razor filename. Code-behind files need explicit usings (don't inherit _Imports.razor). Don't duplicate `@inject`/`[Inject]`. `#hexcolor` → `@("#hexcolor")`. `Unit` params are bare integers. BWFC uses `Sorting` EventCallback (not `OnSorting`). `CommandField` doesn't exist — use `ButtonField`. `TextBox Text` expects `string`. `TemplateField` in DetailsView needs `ItemType`. Style sub-components: `<RowStyleContent><GridViewRowStyle .../></RowStyleContent>`. `SortedAscendingCellStyle` not in BWFC. L1 must not strip attributes BWFC supports.


 Team update (2026-03-11): NEVER default to SQLite; SelectMethod MUST be preserved as SelectHandler<ItemType> delegates. All skill files hardened, L1 script now scaffolds SQL Server.  decided by Jeffrey T. Fritz, Beast, Cyclops


 Team update (2026-03-11): ContosoUniversity L2 confirmed SQL Server LocalDB exclusively. Earlier SQLite attempt was rejected. IDbContextFactory pattern with SqlServer provider is canonical.  decided by Cyclops


 Team update (2026-03-11): Database provider guidance reframed  lead with 'detect and match original provider' instead of 'NEVER substitute'. L1 auto-detection connects to L2 verification.  decided by Beast (requested by Jeffrey T. Fritz)

<!-- ⚠ Summarized 2026-03-12 by Scribe — Database provider, ItemType fix, L2 shims, OPP-1 archived -->

- L1 Script — Web.config Database Provider Auto-Detection (2026-03-12)
- Fix: TItem → ItemType in Tests and Samples (2026-03-12)
- L2 Automation Shims — 4 S-sized Library Enhancements (2026-07-25)
- OPP-1: EnumParameter<T> Wrapper Struct (2026-07-25)

### Summary (2026-03-12)

L1 script: Added Find-DatabaseProvider parsing Web.config connectionStrings (3-pass: providerName, connection string patterns, EntityClient inner provider). Maps SqlClient→SqlServer, SQLite→Sqlite, Npgsql→PostgreSQL. Uses GetAttribute() for StrictMode safety. TItem→ItemType: 43 files renamed (36 tests + 7 samples) after CI failures from generic param mismatch. L2 shims (OPP-2,3,5,6): Unit implicit string conversion (Parse all CSS units), ResponseShim (strips ~/ and .aspx), ViewState dict with [Obsolete], GetRouteUrl via LinkGenerator+HttpContextAccessor. OPP-1 EnumParameter<T>: readonly struct with implicit string→enum conversion for 55 files across 46 components. Gotchas: switch expressions need .Value, Shouldly needs .Value.ShouldBe(), nullable enums need separate handling.





 Team update (2026-03-12): ResponseShim.Redirect null URL bug reported by Rogue — throws NullReferenceException instead of ArgumentNullException. Add null guard.  reported by Rogue (QA)

 Team update (2026-03-12): Cookie shims must use graceful degradation (Pattern B+), not exceptions. NullResponseCookies for no-op writes, EmptyRequestCookieCollection for null reads, both with ILogger warnings. CookiesAvailable bool escape hatch.  decided by Jeffrey T. Fritz
 Team update (2026-03-12): PageTitle deduplication  Page.Title via IPageService is single source of truth. Remove inline <PageTitle> from 5 AfterWingtipToys files. Fix Default.razor.cs "Home Page"  "Welcome". L1 script: inject BWFC-MIGRATE marker. L2: consume marker, never invent values.  decided by Forge (analysis), approved by Jeffrey T. Fritz
 Team update (2026-03-12): Render mode guards  add IsHttpContextAvailable and RequireHttpContext() to WebFormsPageBase. Guard GetRouteUrl. HttpContext != null is the guard, RendererInfo for diagnostics only.  decided by Forge

### Run 20 — ContosoUniversity L2 + Phase 3 Build Validation (2026-07-25)

**Scope:** Full Layer 2 structural transform + build validation of ContosoUniversity sample from raw L1 output to 0-error Blazor Server app.

**Build iterations:** 4 (NuGet auth → ProjectReference, style content wrappers, BoundField ItemType, color/enum types → 0 errors)

**Key transforms (39 files touched):**
- 5 EF6 model classes cleaned (removed SuppressMessage, auto-generated comments, added nullable annotations)
- EF Core DbContext rewritten (removed DbModelBuilder, EF6 imports, cleaned to pure EF Core)
- 4 BLL classes fully rewritten with IDbContextFactory<ContosoUniversityEntities> DI pattern, raw SQL replaced with LINQ
- 3 new DTO model classes created (EnrollmentStat, StudentListItem, StudentInfo) — anonymous objects don't work with BWFC data binding
- Program.cs: full DI registration (DbContextFactory + 4 BLL services), SQL Server LocalDB connection
- _Imports.razor: added ContosoUniversity.Models + ContosoUniversity.BLL usings
- App.razor: added @rendermode="InteractiveServer" to HeadOutlet and Routes
- MainLayout.razor: fixed .aspx links → Blazor routes, removed duplicate jQuery CDN ref
- 5 page code-behinds: Page_Load → OnInitialized, injected BLL services via [Inject], SelectMethod delegates matching SelectHandler<ItemType> signature
- 5 page razor files: removed ScriptManager/UpdatePanel wrappers, removed ajaxToolkit, replaced CommandField mentions, fixed all data binding

**Deleted:** Model1.cs, Model1.Designer.cs, Models/Enrollmet_Logic.cs (duplicate), BLL/Students_Logic.cs (duplicate), old BLL classes (4)

**Key gotchas confirmed:**
- GridView/DetailsView style elements MUST use `*Content` wrappers: `<HeaderStyleContent><GridViewHeaderStyle .../></HeaderStyleContent>`. Direct `<HeaderStyle>` is invalid.
- Inner style component names follow pattern: `GridView{StyleName}Style` / `DetailsView{StyleName}Style`
- Colors on WebControl attributes (BackColor, ForeColor, BorderColor) are `WebColor` type — use `WebColor.White`, `WebColor.Black` etc.
- Enum attributes (GridLines, BorderStyle, HorizontalAlign) wrapped in `EnumParameter<T>` — pass as `@("None")` or use static member
- BoundField inside GridView/DetailsView needs EXPLICIT `ItemType="..."` — CascadingTypeParameter doesn't auto-infer for BoundField
- PackageReference to Fritz.BlazorWebFormsComponents fails without GitHub Packages auth — use ProjectReference in dev
- BLL namespace must be consistent: standardized to `ContosoUniversity.BLL` (uppercase matching folder name)
- Anonymous objects from LINQ projections don't work with BWFC data binding — create typed DTOs instead

### EDMX→EF Core Parser Script (2026-07-25)

**Scope:** New standalone `Convert-EdmxToEfCore.ps1` script + integration into `bwfc-migrate.ps1` Models section.

**What was built:**
- `Convert-EdmxToEfCore.ps1` — parses EDMX XML (SSDL/CSDL/C-S Mapping) and generates EF Core entity .cs files with full data annotations + DbContext with `OnModelCreating()` FK relationships and cascade deletes
- Integrated into `bwfc-migrate.ps1` at the Models Copy section: detects `.edmx` files, generates EF Core code, skips EDMX artifacts (`*.Designer.cs`, T4 bootstrap), then proceeds with normal `.cs` copy for any user-added models

**Tested against:** ContosoUniversity `Model1.edmx` (357 lines, 5 entities, 4 associations, all cascade delete). Output: 5 entity files + `ContosoUniversityEntities.cs` DbContext. All annotations correct: `[Key]`, `[DatabaseGenerated(Identity)]`, `[Required]`, `[MaxLength]`, `[Table("Courses")]` for Cours→Courses mismatch, navigation properties (virtual single refs + ICollection).

**Key implementation details:**
- XML parsing uses `XmlNamespaceManager` with `SelectNodes()`/`SelectSingleNode()` — EDMX has 6+ XML namespaces
- `annotation:StoreGeneratedPattern` attribute requires namespace-qualified `GetAttribute()` call
- C-S Mapping is the key to entity→table name mismatches (e.g., `Cours`→`Courses`)
- Entity files skip generation when target .cs already exists (idempotent)
- DbContext groups FK relationships by dependent entity for clean `modelBuilder.Entity<T>()` blocks
- L1 integration uses `$edmxGeneratedFiles` tracking array to prevent the existing .cs copy loop from overwriting generated files

### UpdatePanel ContentTemplate Enhancement (2026-MM-DD)

**Task:** Add `ContentTemplate` RenderFragment parameter to UpdatePanel and verify InteractiveServer render mode approach.

**Changes made:**
1. **Added ContentTemplate parameter** to `UpdatePanel.razor.cs`:
   - `[Parameter] public RenderFragment ContentTemplate { get; set; }`
   - Updated XML doc comments to clarify ContentTemplate is Web Forms equivalent
   - ChildContent remains for Blazor-style syntax

2. **Updated UpdatePanel.razor** to render `ContentTemplate ?? ChildContent`:
   - Both `<ContentTemplate>` and `<ChildContent>` syntaxes now work
   - Eliminates RZ10012 warnings during L1 migration

3. **Changed base class** from `BaseWebFormsComponent` to `BaseStyledComponent`:
   - Web Forms UpdatePanel in .NET 4.0+ supports `class` attribute via Attributes["class"]
   - This gives UpdatePanel access to CssClass, Style, ToolTip, and other styling properties
   - Matches Web Forms behavior more accurately

4. **Added CSS/Style attributes** to rendered markup:
   - Both div and span variants now render `class="@CssClass" style="@Style" title="@ToolTip"`

5. **Render mode decision**: Did NOT add `@attribute [RenderModeInteractiveServer]`
   - Added code comment explaining why: library components should not force render modes
   - Consuming apps control render mode at App.razor or page level
   - While UpdatePanel's purpose was interactivity in Web Forms, in Blazor this is an app-level concern

6. **Updated sample page** (`samples/AfterBlazorServerSide/Components/Pages/ControlSamples/UpdatePanel/Default.razor`):
   - Added "Web Forms ContentTemplate Syntax" section demonstrating `<ContentTemplate>` usage
   - Shows both syntaxes work (ChildContent and ContentTemplate)
   - Added interactive counter for ContentTemplate example

7. **All 24 tests pass** including new TDD tests for ContentTemplate functionality

**Key learnings:**
- UpdatePanel should inherit from BaseStyledComponent, not BaseWebFormsComponent, because .NET 4.0+ Web Forms UpdatePanel supports CSS class attributes
- RenderFragment parameters with `??` fallback pattern enable dual syntax support (Web Forms and Blazor idioms)
- Library components should not force render modes - that's an app-level architectural decision

### L1 Script ContentTemplate Audit (2026-07-25)

**Task:** Verify L1 migration script handles UpdatePanel + ContentTemplate correctly after the ContentTemplate RenderFragment enhancement.

**Findings:**
- L1 script (`bwfc-migrate.ps1`) has **no ContentTemplate stripping logic** — `ContentTemplate` was never an `asp:` prefixed tag, so `ConvertFrom-AspPrefix` never touched it. It passes through L1 untouched. Correct behavior.
- `CODE-TRANSFORMS.md` and `SKILL.md` — no UpdatePanel/ContentTemplate references, no stripping guidance. Clean.
- `CONTROL-REFERENCE.md` — already updated with proper ContentTemplate documentation from previous work. Clean.

**One fix made:** Code-behind TODO header (line 1404) said "UpdatePanel / ScriptManager references → remove" — misleading now that UpdatePanel is a real BWFC component. Split into two lines: ScriptManager → remove, UpdatePanel → BWFC preserved (remove only code-behind API calls).

### M20 Base Class Fixes (2026-03-07)

**Issue #16 — ToolTip promoted to BaseWebFormsComponent:**
- Moved `ToolTip` property from `BaseStyledComponent` to `BaseWebFormsComponent` so all ~40 components get it, not just styled ones.
- Removed duplicate `ToolTip` declarations from `ChartSeries.razor.cs` and `MenuItem.razor.cs` (they inherit from BaseWebFormsComponent).
- `DataPoint.cs` and `TreeNode.razor.cs` keep their own `ToolTip` — DataPoint is a plain class, TreeNode inherits ComponentBase directly.
- `BaseStyledComponent.ApplyThemeSkin` still references `ToolTip` via inheritance — no changes needed there.

**Issues #15, #17, #18 — Already implemented:**
- AccessKey was already in BaseWebFormsComponent (line 139).
- BaseDataBoundComponent already inherits BaseStyledComponent (not BaseWebFormsComponent directly).
- Image and Label already inherit BaseStyledComponent.
- All 1550 tests pass after changes.
📌 Team update (2026-03-14): M20 Batch 6 orchestration spawn — Forge designing component health dashboard, Cyclops advancing L1 script fixes, Rogue building L1 test harness — decided by Scribe

### L1 Script ~40% → ~60% Automation Coverage (#28)

**Summary:** Added 5 new transformation capabilities to `bwfc-migrate.ps1`, pushing L1 automation coverage from ~40% to ~60%.

**New transforms — Markup (Normalize-AttributeValues + Add-DataSourceIDWarning):**
1. **Boolean normalization:** `Visible="True"` → `Visible="true"`, `Enabled="False"` → `Enabled="false"`. Excludes text-content attributes (Text, Title, Value, etc.) to avoid false positives.
2. **Enum type-qualifying:** 18 attribute→enum mappings (GridLines→GridLines, BorderStyle→BorderStyle, TextMode→TextBoxMode, etc.). `GridLines="Both"` → `GridLines="@GridLines.Both"` so Razor evaluates the C# enum directly instead of relying on EnumParameter<T> string parsing.
3. **Unit normalization:** `Width="100px"` → `Width="100"` for Width, Height, BorderWidth, CellPadding, CellSpacing. Only strips "px" — other units (%, em, pt) preserved since they carry distinct meaning.
4. **DataSourceID warnings:** Removes `DataSourceID="..."` attributes and replaces data source control declarations (`<SqlDataSource>`, `<ObjectDataSource>`, etc.) with `@* TODO *@` comments. Uses `(?s)` single-line regex to handle multi-line tags containing `<%$ ... %>` expressions.

**New transforms — Code-behind (enhanced Copy-CodeBehind):**
5. **Response.Redirect → NavigationManager.NavigateTo:** 4 patterns (literal URL, literal+bool, expression, expression+bool). Strips `~/` prefix from literal URLs. Preserves `.aspx` in URLs (AspxRewriteMiddleware handles rewriting). Injects `[Inject] NavigationManager` into the class.
6. **Session["key"] detection:** Collects unique session keys, inserts migration guidance block (ProtectedSessionStorage, scoped service, cascading parameter options).
7. **ViewState["key"] detection:** Collects unique ViewState keys, generates suggested private field declarations, notes the [Obsolete] compatibility shim.

**Pipeline placement:** New markup transforms run after Convert-TemplatePlaceholders and before blank-line cleanup. Code-behind transforms run after existing [RouteData]/[QueryString] conversion and before file write.

**Key gotchas:**
- Data source control regex uses `(?s)` mode because tags span lines and contain `%>` from Web Forms expressions, which breaks `[^>]*` patterns.
- Boolean exclusion list prevents false positives on attributes like Text="True" or Title="False".
- Enum map only includes unambiguous attribute→enum mappings; ambiguous ones like SelectionMode (Calendar vs List) and Mode are skipped.
- Session/ViewState blocks are inserted after the `=====` TODO header marker using LastIndexOf, not prepended, to keep the file well-organized.


### BlazorAjaxToolkitComponents Project Structure (2026-03-14)

**Summary:** Created the BlazorAjaxToolkitComponents class library project at `src/BlazorAjaxToolkitComponents/`. This is the new home for Blazor components emulating ASP.NET Ajax Control Toolkit controls. Project references BlazorWebFormsComponents and includes Microsoft.JSInterop for client-side behaviors.

**Files Created:**
- `BlazorAjaxToolkitComponents.csproj`  Razor class library, net10.0, NuGet metadata matching existing patterns
- `BaseExtenderComponent.cs`  Stub base class with TargetControlID + IJSRuntime injection (full design pending Forge)
- `_Imports.razor`  Common usings
- `README.md`  Project overview

**Key Decisions:**
- Package ID is `BlazorAjaxToolkitComponents` (no Fritz. prefix  distinct from the base library)
- Depends on `BlazorWebFormsComponents` via ProjectReference
- Uses `Microsoft.JSInterop` (not Microsoft.AspNetCore.Components.Web's JS) since toolkit extenders need direct JS interop
- BaseExtenderComponent extends ComponentBase directly (not BaseWebFormsComponent)  extenders are behavioral attachments, not visual controls

 Team update: BlazorAjaxToolkitComponents project created by Cyclops. PR #71 targeting dev. Branch `squad/441-ajax-toolkit-project`. Fixes #441 (M24: Ajax Toolkit Components).
### ConfirmButtonExtender & FilteredTextBoxExtender Implementation (2026-03-15)

**Summary:** Implemented two Ajax Control Toolkit extender components with full JS interop lifecycle. Updated BaseExtenderComponent from minimal stub to production-ready base class. PR #462 targeting upstream dev.

**BaseExtenderComponent changes:**
- Full JS module lifecycle: abstract JsModulePath, JsCreateFunction, GetBehaviorProperties()
- OnAfterRenderAsync initializes behavior on firstRender when Enabled=true
- BehaviorID and Enabled parameters
- IAsyncDisposable with proper cleanup (disposes behavior, then module)
- SSR-safe: catches JSException and JSDisconnectedException during all JS calls
- TargetControlID passed as string to JS for document.getElementById() resolution (pragmatic v1, no BaseWebFormsComponent dependency)

**ConfirmButtonExtender (#451):** Pure C# class (no .razor needed since extenders render no HTML). ConfirmText, ConfirmOnFormSubmit, DisplayModalPopupID parameters. JS intercepts click, shows window.confirm(), prevents event on cancel.

**FilteredTextBoxExtender (#450):** FilterType flags enum (Numbers, LowercaseLetters, UppercaseLetters, Custom), ValidChars, InvalidChars, FilterMode enum, FilterInterval. JS blocks keystrokes on keypress, strips invalid chars on paste with debounced cleanup.

**Key decisions:**
- Extenders are pure .cs classes, not .razor files (they render zero HTML)
- Kept ComponentBase inheritance per task spec (not BaseWebFormsComponent) for lightweight v1
- JS modules use Map-based behavior registry with create/update/dispose exports
- FilterType is [Flags] enum so filter types can be combined (e.g., Numbers | LowercaseLetters)
- BlazorAjaxToolkitComponents project does NOT enable nullable context, so no nullable annotations

Team update: ConfirmButtonExtender and FilteredTextBoxExtender implemented by Cyclops. PR #462 targeting upstream dev. Branch squad/451-450-confirm-filtered-extenders. Fixes #451, #450.

### ModalPopupExtender & CollapsiblePanelExtender Implementation (2026-03-16)
**Summary:** Implemented two more Ajax Control Toolkit extender components following established patterns from ConfirmButton/FilteredTextBox. Both inherit BaseExtenderComponent, use pure .cs classes (no .razor), and follow the JS module lifecycle pattern.

**ModalPopupExtender (#446):** PopupControlID, BackgroundCssClass, OkControlID, CancelControlID, OnOkScript, OnCancelScript, DropShadow, Drag, PopupDragHandleControlID. JS creates overlay backdrop, centers popup with fixed positioning, traps focus within modal, supports Escape key close, and optional mouse-drag repositioning via drag handle.

**CollapsiblePanelExtender (#447):** CollapseControlID, ExpandControlID (same ID = toggle), Collapsed, CollapsedSize, ExpandedSize, CollapsedText, ExpandedText, TextLabelID, ExpandDirection enum (Vertical/Horizontal), AutoCollapse, AutoExpand, ScrollContents. JS uses CSS transitions on height/width with smart initial-state setup (no transition on first paint, then enables animation).

**New enum:** ExpandDirection (Vertical=0, Horizontal=1) in Enums/ folder.

**Key patterns followed:**
- Same module path convention: ./_content/BlazorAjaxToolkitComponents/js/{kebab-name}.js
- Same JS exports: createBehavior, updateBehavior, disposeBehavior with Map-based registry
- OnOkScript/OnCancelScript executed via 
ew Function(script)() with try/catch safety
- CollapsiblePanel measures natural size via scrollHeight when ExpandedSize=0 (auto)
- XML docs file updated with all new members

Team update: ModalPopupExtender and CollapsiblePanelExtender implemented by Cyclops. Branch squad/446-447-modal-collapsible-extenders. Fixes #446, #447.

### ComponentHealthService Implementation (2026-03-16)

**Summary:** Built the core `ComponentHealthService` in `src/BlazorWebFormsComponents/Diagnostics/` per PRD §7. Four files created: `ImplementationStatus.cs` (enum), `ComponentHealthReport.cs` (data model), `ReferenceBaselines.cs` (JSON loader), `ComponentHealthService.cs` (reflection + scoring engine). DI registration via `AddComponentHealthDashboard()` added to `ServiceCollectionExtensions.cs`.

**Key Implementation Details:**
- **Property/Event Counter (§5.4):** Walks inheritance chain with DeclaredOnly, stops at BaseWebFormsComponent/BaseStyledComponent/BaseDataBoundComponent/DataBoundComponent<>. Uses GetGenericTypeDefinition() for generic base matching. Skips [Obsolete], [CascadingParameter], RenderFragment/RenderFragment<T>, AdditionalAttributes, ChildContent, ChildComponents. EventCallback/EventCallback<T> counted as events only.
- **Component Discovery (§5.1-5.2):** Reflects over BWFC assembly, matches against tracked components list. Falls back to hardcoded 56-component list when `dev-docs/tracked-components.json` doesn't exist.
- **File Detection (§7.4):** Scans test project directories/files for component names, docs/ for matching .md files, ComponentCatalog.cs for sample page registration.
- **Score Computation (§4.1):** Weighted average (Props 30%, Events 15%, Tests 20%, Docs 15%, Sample 10%, Status 10%). Missing baselines excluded and weights re-distributed. Scores capped at 100%. 0/0 treated as 1.0.
- **Baselines Loading:** ReferenceBaselines.LoadFromFile() gracefully handles missing/malformed JSON.
- **Project enforces `var` over explicit types** via IDE0007 as error — all code uses var.

**Files Created:**
- `src/BlazorWebFormsComponents/Diagnostics/ImplementationStatus.cs`
- `src/BlazorWebFormsComponents/Diagnostics/ComponentHealthReport.cs`
- `src/BlazorWebFormsComponents/Diagnostics/ReferenceBaselines.cs`
- `src/BlazorWebFormsComponents/Diagnostics/ComponentHealthService.cs`

**Files Modified:**
- `src/BlazorWebFormsComponents/ServiceCollectionExtensions.cs` (added AddComponentHealthDashboard extension)

**Build verified:** 0 errors, 99 pre-existing warnings.


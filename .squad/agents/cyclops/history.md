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

<!-- ⚠ Summarized 2026-03-11 by Scribe — entries from 2026-03-11 (ServiceCollectionExtensions through L2 structural transform) archived -->

- ServiceCollectionExtensions Enhancement (2026-03-11)
- Run 18a/18b/18c — Test-UnconvertiblePage False-Positive Investigation (2026-03-11)
- P0 Migration Script Fixes (2026-03-11)
- Standardize Generic Type Params to ItemType (2026-03-11)
- Layer 2 Structural Transform - AfterWingtipToys (2026-03-11)

### Summary (2026-03-11 pre-Run 20)

ServiceCollectionExtensions: Added `AddHttpContextAccessor()` auto-registration, `BlazorWebFormsComponentsOptions` with `EnableAspxUrlRewriting`, `UseBlazorWebFormsComponents()` middleware, `AspxRewriteMiddleware` for `.aspx` → clean URL 301 redirects. Run 18: Fixed `Test-UnconvertiblePage` false positives — 'Checkout' matched button IDs, 'PayPal' matched image URLs. Both now path-based. ShoppingCart.razor restored to full GridView markup. P0 fixes: (1) Eliminated `Test-UnconvertiblePage` — always returns `$false`, pages get TODO annotations instead of stubs. (2) `[Parameter]` regex fix — TODO on separate line, eliminates 6 CS errors. ItemType standardization: renamed all `TItemType`/`TItem` → `ItemType` across 13 files, 0 build errors. L2 AfterWingtipToys: ~60 files transformed, 0 errors, 58 RZ10012 warnings. Key gotchas: class name must match .razor filename, `@inject`/`[Inject]` duplication, `#hexcolor` → `@("...")`, `LoggedInTemplate` has no typed context.

Team updates (2026-03-11): Migration tests reorganized to `project/runNN/`. Mandatory L1→L2 pipeline with no fixes between layers. All generics standardized to `ItemType`. Test-UnconvertiblePage eliminated. Run 18 improvements prioritized by Forge.

<!-- ⚠ Summarized 2026-03-12 by Scribe — Run 20 fixes, Run 21, CU L2, L1 SQLite fix archived -->

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

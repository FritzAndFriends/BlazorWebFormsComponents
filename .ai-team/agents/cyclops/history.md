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

### Run 6 Script Enhancements (2026-03-05)

4 enhancements to bwfc-migrate.ps1: (1) TFM net8.0->net10.0 + RenderMode using (line 139), (2) SelectMethod TODO->BWFC Items guidance (-120s, line 756), (3) static files->wwwroot/ (line 1103), (4) compilable stubs for unconvertible pages (Test-UnconvertiblePage + New-CompilableStub, lines 907-988). Bug found: @rendermode InteractiveServer in _Imports.razor is invalid in .NET 10. Test-UnconvertiblePage must also scan .aspx.cs code-behind.

Team update (2026-03-04): Run 6 improvement analysis -> decided by Forge
Team update (2026-03-04): @rendermode InteractiveServer in _Imports.razor scaffold is invalid in .NET 10 -- must be removed from bwfc-migrate.ps1 line 164. Also: Test-UnconvertiblePage must scan .aspx.cs code-behind files. -- decided by Forge

### @rendermode Scaffold Fix (2026-03-05)

**Fix applied:** Removed `@rendermode InteractiveServer` standalone directive from _Imports.razor scaffold in both migration-toolkit/scripts/bwfc-migrate.ps1 and scripts/bwfc-migrate.ps1. The `@using static Microsoft.AspNetCore.Components.Web.RenderMode` using directive was kept (correct — enables shorthand `InteractiveServer`). App.razor scaffold already had the correct pattern: `<Routes @rendermode="InteractiveServer" />` and `<HeadOutlet @rendermode="InteractiveServer" />`.

**Lesson:** `@rendermode` is a directive *attribute* that goes on component instances (e.g., `<Routes @rendermode="InteractiveServer" />`), NOT a standalone Razor directive. Placing it as a bare directive in _Imports.razor causes build errors. For global interactivity, apply it to `<Routes>` and `<HeadOutlet>` in App.razor. The `@using static` import in _Imports.razor is the correct way to make `InteractiveServer` available as a shorthand across all pages.
 Team update (2026-03-04): @rendermode InteractiveServer belongs in App.razor, not _Imports.razor  consolidated from Forge, Cyclops, Jeffrey T. Fritz (PR #419)


 Team update (2026-03-04): EF Core must use 10.0.3 (latest .NET 10)  directed by Jeff

### WebFormsPageBase Implementation (2026-03-05)

**WebFormsPageBase:** Created `src/BlazorWebFormsComponents/WebFormsPageBase.cs` — abstract base class inheriting `ComponentBase` (not `BaseWebFormsComponent`). Injects `IPageService` privately, exposes `Title`, `MetaDescription`, `MetaKeywords` as delegate properties. `IsPostBack => false` so `if (!IsPostBack)` compiles and always enters. `Page => this` self-reference enables `Page.Title = "X"` to compile unchanged from Web Forms code-behind. Converted pages use `@inherits WebFormsPageBase` (one line in `_Imports.razor`). Build verified clean (63 pre-existing warnings, 0 errors). Lesson: Pages are top-level containers, not child controls — inheriting `ComponentBase` directly avoids the CascadingValue wrapping and control-tree logic in `BaseWebFormsComponent`.

 Team update (2026-03-04): WebFormsPageBase implemented  decided by Forge, approved by Jeff

### WebFormsPage IPageService Consolidation (2026-03-05)

**WebFormsPage enhanced:** Merged `Page.razor` head-rendering capability into `WebFormsPage`. Added optional `IPageService` resolution via `ServiceProvider.GetService<IPageService>()` — WebFormsPage still works for naming/theming when IPageService is not registered. Subscribes to `TitleChanged`, `MetaDescriptionChanged`, `MetaKeywordsChanged` events in `OnInitialized()`. Renders `<PageTitle>` and `<HeadContent>` before the existing CascadingValue wrapper. Added `RenderPageHead` parameter (default true) to allow opting out. Implements `IDisposable` to unsubscribe from events. `Page.razor` left untouched as standalone option. Build verified clean (70 pre-existing warnings, 0 errors). Lesson: `IServiceProvider` in `BaseWebFormsComponent` is private — child classes needing it must inject their own via `[Inject]`.

� Team update (2026-03-05): WebFormsPage now includes IPageService head rendering (title + meta tags), merging Page.razor capability per Option B consolidation. Layout simplified to single <WebFormsPage> component. Page.razor remains standalone.  decided by Forge, implemented by Cyclops


 Team update (2026-03-06): CRITICAL  Git workflow: feature branches from dev, PRs target dev. NEVER push to or merge into upstream main (production releases only).  directed by Jeff Fritz

 Team update (2026-03-06): CONTROL-COVERAGE.md updated  library ships 153 Razor components (was listed as 58). ContentPlaceHolder reclassified from 'Not Supported' to Infrastructure Controls. Reference updated CONTROL-COVERAGE.md for accurate component inventory.  decided by Forge

� Team update (2026-03-06): LoginView is a native BWFC component  do NOT replace with AuthorizeView in migration guidance. Both migration-standards SKILL.md files (in .ai-team/skills/ and migration-toolkit/skills/) must be kept in sync. WebFormsPageBase patterns corrected in all supporting docs.  decided by Beast

### LoginView Migration Script Fix (2026-03-06)

**Bug fixed:** `ConvertFrom-LoginView` in `migration-toolkit/scripts/bwfc-migrate.ps1` was incorrectly converting `<asp:LoginView>` to `<AuthorizeView>` and renaming `AnonymousTemplate`/`LoggedInTemplate` to `NotAuthorized`/`Authorized`. The BWFC `LoginView` component already accepts `AnonymousTemplate` and `LoggedInTemplate` as `[Parameter] RenderFragment` properties. The script now converts `<asp:LoginView>` to `<LoginView>` and leaves template names untouched. Also updated the RoleGroups manual item to reference the BWFC RoleGroup component.

**Sample fixed:** `samples/AfterWingtipToys/Components/Layout/MainLayout.razor` was using `<AuthorizeView>` with `<NotAuthorized>`/`<Authorized>`. Replaced with `<LoginView>` using `<AnonymousTemplate>`/`<LoggedInTemplate>` and `<LoginName />`. Added `@using BlazorWebFormsComponents.LoginControls` to `_Imports.razor`.

**Pattern:** BWFC LoginView preserves Web Forms template names (AnonymousTemplate, LoggedInTemplate) -- the migration script should only strip the asp: prefix and attributes, never rename child templates.

 Team update (2026-03-06): LoginView must be preserved as BWFC component, not converted to AuthorizeView  decided by Jeff (directive)

### Run 9 Script Fixes (2026-03-06)

**9 fixes implemented in `migration-toolkit/scripts/bwfc-migrate.ps1`** to reduce Layer 2 manual work:

**Functions modified:**
- `New-ProjectScaffold` (lines ~128-238): Added `$SourcePath` parameter, conditional EF Core + Identity package refs (RF-06), and Identity/Session boilerplate in Program.cs (RF-07). Detection logic checks for `Models/`, `Account/`, `Login.aspx`, `Register.aspx` in source.
- `ConvertFrom-PageDirective` (lines ~299-330): Extracts `Title="..."` attribute from `<%@ Page %>` directive before stripping, emits `<PageTitle>` (RF-10).
- `ConvertFrom-GetRouteUrl` (lines ~664-720): After converting route calls, scans for route names in `GetRouteUrlHelper.GetRouteUrl("RouteName", ...)` and emits Write-ManualItem with concrete URL replacement hints (RF-11).
- `Copy-CodeBehind` (lines ~831-970): Added regex transforms for `[QueryString("param")]` → `[SupplyParameterFromQuery(Name = "param")]` and `[RouteData]` → `[Parameter]` with TODO comment (RF-12).

**Functions added:**
- `Test-RedirectHandler`: Detects pages with `Response.Redirect` in code-behind and minimal markup (<100 chars after directive stripping). Flags for minimal API conversion (RF-08).

**Entry point sections added:**
- Models copy section (RF-03/RF-04): After static file copy, detects and copies `Models/*.cs` files. Strips EF6 usings, adds TODO headers. For `*Context.cs` files: replaces EF6 → EF Core using, removes old constructors (including parameterless `base("connectionName")` pattern), adds `DbContextOptions<T>` constructor.
- ListView GroupItemCount detection (RF-13): Before asp: prefix stripping, checks for `<asp:ListView ... GroupItemCount="N"` and emits manual item with BWFC guidance.
- Redirect handler Program.cs annotation (RF-08): After processing, appends TODO comments to Program.cs for detected redirect handler pages.
- `$script:RedirectHandlers` tracking list added to logging region.

**Key patterns:**
- Script uses here-strings (`@"..."@`) for code templates with variable expansion. Conditional blocks are best injected via `.Replace()` after template creation.
- `Test-Path` is used for feature detection before scaffold generation.
- Transform pipeline order matters: RF-13 must check BEFORE `ConvertFrom-AspPrefix` strips the `asp:` prefix.
- DbContext constructor patterns vary: parameterless `() : base("name")`, parameterized `(string x) : base(x)`, etc. Multiple regexes needed.

📬 Team update (2026-03-06): Beast completed 6 skill fixes (RF-01/02/05/09/13/14) across 4 SKILL.md files — complements Cyclops's 9 script fixes. All 15 P0+P1 items done on squad/run8-improvements. — decided by Forge (analysis), implemented by Cyclops + Beast



 Team update (2026-03-06): Run 9 CSS/image failure RCA  script drops bundle refs, Layer 2 changed image paths without moving files. 5 fixes proposed.  decided by Forge

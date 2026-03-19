# Project Context

- **Owner:** Jeffrey T. Fritz
- **Project:** BlazorWebFormsComponents — Blazor components emulating ASP.NET Web Forms controls for migration
- **Stack:** C#, Blazor, .NET, ASP.NET Web Forms, bUnit, xUnit, MkDocs, Playwright
- **Created:** 2026-02-10

## Project Learnings (from import)

- The migration-toolkit lives at `migration-toolkit/` and contains: scripts/, skills/, METHODOLOGY.md, CHECKLIST.md, CONTROL-COVERAGE.md, QUICKSTART.md, README.md, copilot-instructions-template.md
- Primary migration script: `migration-toolkit/scripts/bwfc-migrate.ps1` — handles Layer 1 (automated transform)
- Layer 2 (agent-driven implementation) is where BWFC control replacement failures occur — agents replace asp: controls with plain HTML
- Test-BwfcControlPreservation in bwfc-migrate.ps1 validates that BWFC controls are preserved post-transform
- Test-UnconvertiblePage uses path-based patterns (Checkout\, Account\) and content patterns (SignInManager, UserManager, etc.)
- Sample migration targets: WingtipToys (before/after samples in samples/ directory)
- Run7 is the current gold standard: `samples/Run7WingtipToys/`
- The BWFC library has 110+ components covering Web Forms controls
- Migration must preserve all asp: controls as BWFC components — never flatten to raw HTML

## Core Context

<!-- ⚠ Summarized 2026-03-06 by Scribe — covers Run 9 through Run 11, Cycles 1-2. Originals in history-archive.md -->

**Run 9-11 benchmark progression:** Build attempts 7→3→4, Layer 2 time 45→25→20 min, BWFC instances 173→172→178, preservation 98.9%→92.7%→98.9%. All runs 0 errors, 0 warnings. 28 routable pages, ~32 .razor, ~79 wwwroot throughout.

**Cycle 1 fixes (5 items):** P0-1 ItemType→TItem regex (only list controls get TItem, data controls retain ItemType). P0-2 smart stubs (all markup gets L1 transforms, only code-behinds stubbed). P0-3 base class stripping (`: Page`/`: UserControl`/`: MasterPage` removed). P1-1 validator type params (Type="string"/InputType="string" auto-injected). P1-4 ImageButton warning in Test-BwfcControlPreservation.

**Cycle 2 fixes (6 items):** Convert-EnumAttributes (TextMode→TextBoxMode, Display→@ValidatorDisplay, GridLines→@GridLines). Boolean normalization (True/False → true/false). ControlToValidate/ValidationGroup stripping. ImageButton detection escalated to FAIL. Server-side expression cleanup (Request/Session/Server wrapped in TODO comments). Remove-ItemTypeWithDataSource (strips ItemType/TItem when SelectMethod present).

**Key Layer 2 patterns:** `@inherits WebFormsPageBase` conflicts with `: ComponentBase` — must remove explicit base. Layout files need `: LayoutComponentBase`. `AddHttpContextAccessor()` before `AddBlazorWebFormsComponents()`. Stub model pattern for unavailable types (UserLoginInfo, OrderShipInfo). Stub page cleanup ~60% of L2 effort.

## Learnings

### Cycle 3 Fixes Applied (Bishop) — Script + Auth Services

- **P0.3 (Mock Auth Service Generation):** New `Add-MockAuthService` function in bwfc-migrate.ps1 auto-generates `Services/MockAuthService.cs` (in-memory user store with email/password) and `Services/MockAuthenticationStateProvider.cs` (Blazor auth state provider with login/logout) when Account/Login.aspx is detected in source. Eliminates manual auth scaffolding in Layer 2.
- **P0.1+P0.2 (Program.cs Auth Injection):** `New-ProjectScaffold` now accepts `$HasIdentityPages` parameter and conditionally adds `AddScoped<MockAuthService>()`, `AddScoped<MockAuthenticationStateProvider>()`, `AddCascadingAuthenticationState()`, and `AddAuthorization()` to Program.cs template.
- **P1.4 (LogoutAction Enum):** Added LogoutAction enum conversions: Redirect, RedirectToLoginPage, Refresh → `@LogoutAction.{Value}`.
- **P1.4 (BorderStyle Enum):** Added BorderStyle enum conversions: None, NotSet, Dotted, Dashed, Solid, Double, Groove, Ridge, Inset, Outset → `@BorderStyle.{Value}`.
- **P1.3 (Visible Attribute):** New `Convert-VisibleAttribute` function strips `Visible="true"` (default value), preserves `Visible="false"` for BWFC components.
- **P2.2 (Hex Color Escaping):** New `Convert-HexColors` function escapes hex color values like `BorderColor="#efeeef"` to `BorderColor="@("#efeeef")"` to prevent C# preprocessor directive interpretation in Razor.

### 2026-03-05: Run 12 WingtipToys Migration Benchmark

- **Result:** Build succeeded — 0 errors, 0 warnings, 2 build attempts (best ever — down from 4 in Run 11)
- **Pipeline:** Layer 1 (~3.5s, 365 ops) → Layer 2 (~15min) → Build (~3s, 2 attempts)
- **Output:** 32 .razor, 32 .cs, 79 wwwroot, 28 routable pages, 184 BWFC control instances (27 unique types)
- **Auth services fully functional (P0 complete):**
  - Login.razor.cs: Injects MockAuthenticationStateProvider + MockAuthService, calls LoginAsync, NavigateTo "/" on success
  - Register.razor.cs: Calls CreateUserAsync, auto-login on success, NavigateTo "/Account/Login"
  - MockAuthService: In-memory user store (admin@wingtiptoys.com / Pass@word1 default)
  - LoginStatus: Preserved as BWFC component on MainLayout with `LogoutAction="@LogoutAction.Redirect"`
  - Login/Register markup uses `@bind-Text` for TextBox → code-behind field binding
- **Layer 1 script improvements (6 new features):**
  - Mock auth service generation (conditional on Account/Login.aspx)
  - Program.cs auth injection (conditional)
  - LogoutAction + BorderStyle enum conversions
  - Visible attribute handling (strip true, preserve false)
  - Hex color escaping for Razor safety
- **BWFC preservation rate:** ~100% (184 instances, 27 unique types — up from 178/26 in Run 11)
- **LinkButton newly preserved:** 3 instances (ManageLogins, Checkout pages) — previously flattened

📌 Team update (2026-03-05): Run 12 BENCHMARK complete. All quality gates passed: 0 build errors, ≤3 attempts (2), auth functional, 184 BWFC instances (≥178), ~100% preservation. First run with functional Login/Register. — decided by Bishop

### Fresh Layer 1 Migration Run (Bishop) — AfterWingtipToys

- **Date:** Run executed against cleared `samples/AfterWingtipToys/`
- **Script:** `bwfc-migrate.ps1` with `-Path samples/WingtipToys/WingtipToys -Output samples/AfterWingtipToys`
- **Execution time:** 3.33 seconds
- **Transforms applied:** 366
- **Files processed:** 32 Web Forms files
- **Static files copied:** 80
- **Items needing review:** 46 (14 CodeBlock, 1 ContentPlaceHolder, 2 ControlPreservation, 15 EventHandler, 1 LoginView-Auth, 4 RegisterDirective, 9 SelectMethod)
- **Output:**
  - 35 .razor files, 35 .cs files, 80 wwwroot files, 152 total files
  - Scaffold: WingtipToys.csproj ✓, Program.cs ✓, _Imports.razor ✓, Components/App.razor ✓, Components/Routes.razor ✓
  - Layout: MainLayout.razor + Site.MobileLayout.razor in Components/Layout/
  - Auth: Services/MockAuthService.cs + MockAuthenticationStateProvider.cs auto-generated (Account/Login.aspx detected)
  - Pages: 10 root pages, 15 Account pages, 6 Checkout pages, 1 Admin page
- **Observations:**
  - Task spec listed `-SourcePath`/`-DestinationPath`/`-ProjectName` params — actual params are `-Path`/`-Output` (no ProjectName). Script auto-detects project name from directory.
  - App.razor lives at `Components/App.razor` (Blazor Web App convention), not project root. Task verification should check there.
  - ControlPreservation flagged Site.Master: 1 `<PlaceHolder>` lost (5 asp: tags in → 4 BWFC tags out). Known issue from prior runs.
  - 0 errors during script execution. Clean run.

 Team update (2026-03-06): Layer 2 conventions established  Button OnClick uses EventArgs (not MouseEventArgs), code-behind class names must match .razor filenames exactly, use EF Core wildcard versions for .NET 10, CartStateService replaces Session, GridView needs explicit TItem  decided by Cyclops


 Team update (2026-03-06): Forge produced 8 script improvement recommendations (S1-S8) assigned to Bishop  S1: @inherits WebFormsPageBase in _Imports.razor, S2: AddHttpContextAccessor in Program.cs, S3: : Page  : WebFormsPageBase, S4: @using Enums, S5: Page_Load rename, S6: Cookie auth scaffold, S7: src~/action~ URL conversion, S8: Stub base class. Recommended Cycle 1: S1+S2+S3+S4  decided by Forge
 Team update (2026-03-06): LoginView is native BWFC  migration script must stop converting to AuthorizeView. Strip asp: prefix only, preserve template names  decided by Jeffrey T. Fritz, Forge
 Team update (2026-03-06): WebFormsPageBase is the canonical base class for all migrated pages (not ComponentBase). All agents must use WebFormsPageBase  decided by Jeffrey T. Fritz

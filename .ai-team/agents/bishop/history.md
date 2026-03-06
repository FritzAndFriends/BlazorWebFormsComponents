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

## Learnings

### 2025-07-25: Run 9 WingtipToys Migration Benchmark

- **Result:** Build succeeded — 0 errors, 0 warnings (Run9-specific)
- **Pipeline:** Layer 0 (0.66s) → Layer 1 (4.49s, 667 ops) → Layer 2 (~45min) → Build (6.13s, 7 attempts)
- **Output:** 35 .razor, 46 .cs, 79 wwwroot, 28 routable pages, 173 BWFC control instances (23 unique types)
- **Layer 1 bugs found:**
  - `ItemType→TItem` conversion is wrong for GridView/ListView/FormView/DetailsView (they use `ItemType`, only DropDownList uses `TItem`)
  - Validators missing type params: RequiredFieldValidator needs `Type="string"`, CompareValidator needs `InputType="string"`
  - No `BlazorWebFormsComponents.Validations` using added to `_Imports.razor`
- **Layer 2 patterns:**
  - `@inherits WebFormsPageBase` in `_Imports.razor` conflicts with `: ComponentBase` in code-behinds — must remove `: ComponentBase`
  - Layout files need `: LayoutComponentBase` explicitly
  - Stub page cleanup (17/35 files) is the largest Layer 2 effort — unconverted event handlers, ControlToValidate refs, `<% %>` expressions
  - `AddHttpContextAccessor()` must come BEFORE `AddBlazorWebFormsComponents()` in Program.cs

 Team update (2026-03-05): Run 9 BWFC review APPROVED (98.9% preservation). 2 findings: ImageButtonimg in ShoppingCart (P0), HyperLink dropped in Manage (P2). 3 Layer 1 script bugs documented (ItemType conversion, validator type params, missing Validations using).  decided by Forge, Bishop

### Cycle 1 Fixes Applied (Bishop)

- **P0-1 (ItemType→TItem):** Fixed. Regex now uses `(<(?:DropDownList|ListBox|...)\b[^>]*?)\bItemType=` with Singleline flag so only list controls get TItem. Data controls (GridView, ListView, FormView, DetailsView, DataGrid, DataList, Repeater) retain ItemType. Handles multi-line tags.
- **P0-2 (Smart stub):** Fixed. Removed the early `return` that skipped transforms for Account/Checkout pages. All markup now gets full Layer 1 transforms. Only code-behinds are stubbed (minimal partial class + TODO banner). New `New-StubCodeBehind` function added.
- **P0-3 (Base class stripping):** Fixed. `Copy-CodeBehind` now strips `: Page`, `: System.Web.UI.Page`, `: UserControl`, `: MasterPage` base classes and `using System.Web.*` directives before copying. Avoids CS0263 conflicts with `@inherits WebFormsPageBase`.
- **P1-1 (Validator type params):** Fixed. New `Add-ValidatorTypeParameters` function injects `Type="string"` into RequiredFieldValidator/RegularExpressionValidator/RangeValidator and `InputType="string"` into CompareValidator. Uses negative lookahead to skip tags that already have the attribute.
- **P1-4 (ImageButton warning):** Fixed. `Test-BwfcControlPreservation` now emits a specific warning when source has `asp:ImageButton` and output contains `<img>` tags, flagging silent OnClick event handler loss.

### 2025-07-25: Run 10 WingtipToys Migration Benchmark

- **Result:** Build succeeded — 0 errors, 0 warnings, 3 build attempts (down from 7 in Run 9)
- **Pipeline:** Layer 0 (0.91s) → Layer 1 (3.35s, 673 ops) → Layer 2 (~25min) → Build (~13s, 3 attempts)
- **Output:** 35 .razor, 44 .cs, 79 wwwroot, 28 routable pages, 172 BWFC control instances (26 unique types)
- **P0/P1 fixes validated:** All 5 fixes confirmed working — ItemType correct on data controls, smart stubs fully transform Account/Checkout markup, base class stripping works, validator type params injected, ImageButton warning active.
- **Remaining Layer 2 issues discovered:**
  - TextMode/Display/SetFocusOnError enum values not converted by Layer 1 (TextMode="Email" should be TextMode="TextBoxMode.Email"). Candidate for P2 fix.
  - Empty DropDownList (no Items) can't infer TItem — must be specified explicitly.
  - ManageLogins page uses `Microsoft.AspNet.Identity.UserLoginInfo` as ItemType — needs simplification to stub.
  - Stub page cleanup still ~60% of Layer 2 effort despite P0-2 fix.

### Cycle 2 Fixes Applied (Bishop) — Script-Only (P1/P2)

- **P1-1 + P1-2 + P2-5 (Convert-EnumAttributes):** New function converts Web Forms string enum values to C# enum types. TextMode→TextBoxMode (5 values), Display→@ValidatorDisplay (3 values), GridLines→@GridLines (4 values). Called after Add-ValidatorTypeParameters in the pipeline. Eliminates ~34 manual fixes per run.
- **P1-3 (Boolean normalization):** New `Convert-BooleanAttributes` function uses regex `(?<==")True(?=")` and `(?<==")False(?=")` to lowercase PascalCase boolean attribute values. Prevents subtle C# case-sensitivity build errors.
- **P1-4 (ControlToValidate stripping):** Added `ControlToValidate` and `ValidationGroup` to `$StripAttributes` list. Both are Web Forms-only attributes with no Blazor equivalent.
- **P2-3 (ImageButton FAIL):** Elevated ImageButton→img detection in `Test-BwfcControlPreservation` from ⚠️ WARNING to ❌ FAIL. Stronger signal for Layer 2 to preserve `<ImageButton>` component.
- **P2-4 (Server-side expression cleanup):** Added pre-pass in `ConvertFrom-Expressions` that detects `<%:` / `<%=` expressions referencing `Request`, `Session`, `Server`, `Response`, `ProviderName`, `SuccessMessage`, `OpenID_*`, `ManageMessage`. These are wrapped in `@* TODO: Server-side expression *@` Razor comments instead of `@(Variable)` which won't compile.

 Team update (2026-03-06): Forge reviewed Run 10 preservation: 92.7% (164/177), below 95% threshold. 3 gaps: CheckoutReview DetailsView missing (9 controls), ManageLogins still stub (3 controls), ShoppingCart ImageButton flattened (1 control). Fixing all 3 reaches 97.7%. Layer 1 bugs consolidated into single decision (ItemType, validators, base class).  decided by Forge

 Team update (2026-03-05): User directive from Jeff  stop emitting ItemType/TItem when data source exists (SelectMethod, Items, etc.). Blazor generic type inference handles it. Eliminates #1 recurring build failure class.  decided by Jeffrey T. Fritz

### Cycle 2 Fix: ItemType/TItem Stripping with Data Source (Bishop)

- **Remove-ItemTypeWithDataSource:** New function in bwfc-migrate.ps1 that strips ItemType/TItem attributes when SelectMethod is present on the same tag. Runs before ConvertFrom-SelectMethod in the pipeline. Handles both attribute orderings (ItemType before/after SelectMethod) and both attribute names (ItemType and TItem). Eliminates the #1 recurring build failure class — redundant type parameters.

### 2025-07-25: Run 11 WingtipToys Migration Benchmark

- **Result:** Build succeeded — 0 errors, 0 warnings, 4 build attempts (down from 7 in Run 9, comparable to 3 in Run 10)
- **Pipeline:** Layer 0 (skipped — scan script parse error) → Layer 1 (~3.5s, 354 ops) → Layer 2 (~20min) → Build (~4.4s, 4 attempts)
- **Output:** 32 .razor, 32 .cs, 79 wwwroot, 28 routable pages, 178 BWFC control instances (26 unique types)
- **All 3 P0 gaps from Run 10 closed:**
  - P0-1: CheckoutReview DetailsView — fully preserved with OrderShipInfo stub model, 9 BWFC controls recovered
  - P0-2: ShoppingCart ImageButton — preserved as `<ImageButton>` with OnClick→NavigateTo wiring
  - P0-3: ManageLogins — full ListView + Button + PlaceHolder preserved with UserLoginInfo stub model
- **New capabilities:**
  - ItemType/TItem stripping when SelectMethod present (Layer 1 script fix)
  - AdminPage fully functional with EF Core CRUD (add/remove products)
  - Stub model pattern: create lightweight model classes when original types unavailable (UserLoginInfo, OrderShipInfo)
- **BWFC preservation rate:** ~100% (178/178 controls in output have BWFC components)

📌 Team update (2025-07-25): Forge reviewed Run 11 preservation: 98.9% (176/178 adjusted) — APPROVED. All 3 P0 gaps from Run 10 CLOSED. 1 minor gap: HyperLink conditional visibility in Manage.razor. Cycle 3 priorities shift to functional code-behinds (Login/Register per Jeff's directive). — decided by Forge

📌 Team update (2025-07-25): Jeff directive — Login and Register pages must have functional code-behinds, not just BWFC markup. Mock auth service required. — decided by Jeffrey T. Fritz

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


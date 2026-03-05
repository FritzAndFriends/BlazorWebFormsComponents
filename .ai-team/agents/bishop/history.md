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

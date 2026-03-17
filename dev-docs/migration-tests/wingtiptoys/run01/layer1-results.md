# Layer 1 Results — Automated Scripts

**Date:** 2026-03-04
**Source:** `samples/WingtipToys/WingtipToys/`
**Target:** `samples/FreshWingtipToys/`

## Scan Phase

- **Duration:** 0.9s
- **Files scanned:** 32 (28 .aspx, 2 .ascx, 2 .master)
- **Controls found:** 230 usages across 31 distinct control types
- **Readiness score:** 🟢 100% — all 230 control usages map to BWFC-supported components
- **Key findings:**
  - Most-used controls: Label (44), Content (27), TextBox (22), RequiredFieldValidator (21), Button (17), PlaceHolder (15), ScriptReference (13)
  - No unsupported or unknown controls detected
  - 6 files use data binding expressions
  - No ViewState or Session usage detected in markup (code-behind only)

## Mechanical Transform Phase

- **Duration:** 2.4s
- **Input files:** 32 Web Forms files (28 .aspx, 2 .ascx, 2 .master)
- **Output files:** 33 .razor files (32 from Web Forms + 1 generated `_Imports.razor`)
- **Code-behind files:** 32 .razor.cs files copied with TODO annotations
- **Static files copied:** 79 (CSS, JS, images, fonts)
- **Transforms applied:** 276
- **Items needing manual review:** 18
  - 14 unconverted code blocks (complex data binding expressions like `<%#: String.Format(...)%>`, `<%#: GetRouteUrl(...)%>`, inline `<% %>` blocks)
  - 4 Register directive removals needing component reference verification
- **Transform coverage:** ~40% mechanical (markup transforms complete; code-behind is copied but not transformed)

### Transform Breakdown

| Transform Type | Description |
|---------------|-------------|
| Directive | `<%@ Page %>` → `@page "/route"`, removed Master/Control/Register/Import directives |
| Content | Stripped `asp:Content` wrappers |
| Form | Removed `<form runat="server">` wrappers |
| Expression | Converted comments, Eval() bindings, encoded/unencoded expressions |
| TagPrefix | Removed `asp:` prefix from all control tags |
| Attribute | Stripped `runat="server"`, `AutoEventWireup`, `ViewStateMode`, etc. Converted `ItemType` → `TItem` |
| URL | Converted `~/` references to `/` |
| CodeBehind | Copied all .cs files with TODO annotation headers |
| Scaffold | Generated `.csproj`, `_Imports.razor`, `Program.cs` |

## Build Attempt

- **Result:** ❌ FAIL (338 errors, 48 warnings)
- **NuGet note:** `.csproj` was updated to use ProjectReference to local BWFC library (scaffold generated PackageReference)
- **Target framework:** Updated from generated `net8.0` to `net10.0` to match repo

### Error Categories

| Count | Error | Description |
|------:|-------|-------------|
| 148 | CS0234 | `System.Web.UI` namespace missing — code-behind inherits `System.Web.UI.Page` |
| 54 | CS0234 | `Microsoft.AspNet` namespace missing — ASP.NET Identity v2 references |
| 32 | CS0234 | `WingtipToys.Models` missing — domain model classes not migrated |
| 18 | CS0246 | `Page` base class not found |
| 14 | CS0246 | `Owin` namespace not found |
| 10 | CS0234 | `WingtipToys.Logic` missing — business logic classes not migrated |
| 8 | CS0246 | `ApplicationUserManager` not found — Identity manager types |
| 6 | CS0234 | `System.Web.ModelBinding` missing |
| 6 | CS0234 | `Microsoft.Owin` missing |
| 6 | CS0246 | `IdentityResult` not found |

### Assessment — What Layer 2 Needs to Fix

1. **Code-behind lifecycle conversion** (highest priority): All 32 code-behind files inherit `System.Web.UI.Page` and use Web Forms lifecycle methods (`Page_Load`, `Page_Init`, etc.). These need conversion to Blazor component lifecycle (`OnInitializedAsync`, `OnParametersSetAsync`).

2. **Domain model migration**: `WingtipToys.Models` and `WingtipToys.Logic` namespaces need to be recreated or referenced. The original project has Entity Framework models and business logic classes.

3. **Identity migration**: ASP.NET Identity v2 (`Microsoft.AspNet.Identity`) needs migration to ASP.NET Core Identity (`Microsoft.AspNetCore.Identity`).

4. **Data binding expressions**: 14 complex data binding expressions (format strings, route URLs) need manual Razor conversion.

5. **OWIN middleware**: Startup/authentication pipeline needs conversion from OWIN to ASP.NET Core middleware.

## Summary

Layer 1 successfully performed the mechanical migration in **3.3s total** (0.9s scan + 2.4s transform). The scan confirmed 100% BWFC control coverage. The transform handled all markup-level changes (276 transforms) but the code-behind files — which contain the real application logic — remain as Web Forms code requiring Layer 2 semantic transforms. The 338 build errors are entirely expected: they represent the code-behind and business logic that Layer 1 deliberately defers to Layer 2.

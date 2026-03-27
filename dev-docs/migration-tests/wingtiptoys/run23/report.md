# WingtipToys Migration — Run 23: L1 Script Improvements (2026-03-26)

## Summary

This run focused on improving the L1 migration script (`bwfc-migrate.ps1`) and the BWFC
library packaging to reduce post-migration build errors from **372 → 32** (a **91%
reduction**). The remaining 32 errors are genuine code migration issues requiring L2
skill intervention — not missing infrastructure or namespace resolution failures.

Key innovation: A `.targets` file now ships with the BWFC NuGet package, automatically
providing type aliases (`Page`, `MasterPage`, `ImageClickEventArgs`) and namespace imports
to any project that references the package.

## Changes Made

### 1. BWFC `.targets` File (new — ships with NuGet package)

Added `buildTransitive/Fritz.BlazorWebFormsComponents.targets` and
`build/Fritz.BlazorWebFormsComponents.targets` to the BWFC library project.
When a project references the NuGet package, MSBuild automatically imports:

```xml
<!-- Global usings injected by .targets -->
<Using Include="BlazorWebFormsComponents" />
<Using Include="BlazorWebFormsComponents.LoginControls" />

<!-- Type aliases -->
<Using Alias="Page" Include="BlazorWebFormsComponents.WebFormsPageBase" />
<Using Alias="MasterPage" Include="Microsoft.AspNetCore.Components.LayoutComponentBase" />
<Using Alias="ImageClickEventArgs" Include="Microsoft.AspNetCore.Components.Web.MouseEventArgs" />
```

Also provides opt-in migration-mode warning suppression via
`<BwfcMigrationMode>true</BwfcMigrationMode>`.

**Impact**: Any project referencing BWFC via NuGet automatically gets `Page`, `MasterPage`,
and `ImageClickEventArgs` aliases. No generated code needed for these.

**Note**: `ProjectReference` (local dev) does not auto-import `.targets` files. For local
development, add an explicit `<Import>` to the consumer project.

### 2. GlobalUsings.cs Generation (slimmed down)

The script generates `GlobalUsings.cs` with only Blazor infrastructure usings not covered
by the BWFC `.targets`:

```csharp
global using Microsoft.AspNetCore.Components;       // [Inject], [Parameter], NavigationManager
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Components.Routing;
```

Type aliases and BWFC namespace imports are now handled by the `.targets` file.

### 3. WebFormsShims.cs Removed

The `LoginCancelEventArgs` type that was in `WebFormsShims.cs` already exists in the BWFC
library at `BlazorWebFormsComponents.LoginControls.LoginCancelEventArgs`. GridView event
args (`GridViewUpdateEventArgs`, `GridViewDeleteEventArgs`) were also already in BWFC.
No shim file is needed — the `.targets` provides the namespace imports.

### 4. IdentityShims.cs Generation (conditional, namespace simplified)

When Identity/Account pages are detected, generates compilation stubs in the **project
root namespace** (not a sub-namespace). Types:

- `IdentityUser`, `IdentityResult`, `UserLoginInfo`
- `ApplicationUserManager` (no-op methods matching Identity v2 API surface)
- `ApplicationSignInManager` (no-op methods matching Identity v2 API surface)
- `SignInStatus` enum

A proper delegation layer to ASP.NET Core Identity is tracked in
[issue #525](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/525).

### 5. _Imports.razor Cleanup

- Removed hardcoded `@using BlazorAjaxToolkitComponents` (was always included, rarely needed)
- Added `@using ProjectName.Models` (covers model references in Razor files)
- AJAX toolkit import conditionally added in post-processing only when detected

### 6. Enhanced Using-Stripping in Code-Behind Copy

`Copy-CodeBehind` now strips **all** legacy namespaces (not just `System.Web.*`):

| Namespace Pattern | Was Stripped? | Now Stripped? |
|---|---|---|
| `System.Web.*` | ✅ | ✅ |
| `Microsoft.AspNet.*` | ❌ | ✅ |
| `Microsoft.Owin.*` | ❌ | ✅ |
| `Owin` | ❌ | ✅ |

Same stripping also added to model file copy and business logic file copy paths.

### 7. Fully-Qualified Base Class Replacement (new)

Code-behinds with `class MyPage : System.Web.UI.Page` are now transformed to
`class MyPage : Page`, allowing the `.targets` alias to resolve correctly.

| FQN | Replacement |
|---|---|
| `System.Web.UI.Page` | `Page` (→ `WebFormsPageBase` via alias) |
| `System.Web.UI.MasterPage` | `MasterPage` (→ `LayoutComponentBase` via alias) |
| `System.Web.UI.UserControl` | `ComponentBase` |

## Error Taxonomy: Before vs After

| Error Category | Before | After | Fix |
|---|---|---|---|
| BlazorAjaxToolkitComponents in _Imports | 70 | 0 | Conditional include |
| [Inject] / NavigationManager / SupplyParameterFromQuery | 98 | 0 | GlobalUsings.cs |
| `: Page` / `: MasterPage` base class (FQN) | 44 | 0 | FQN replacement + alias |
| `: Page` / `: MasterPage` base class (unqualified) | 20 | 0 | .targets alias |
| Microsoft.AspNet.* namespace | 64 | 0 | Using stripping |
| Microsoft.Owin namespace | 8 | 0 | Using stripping |
| Identity v2 types | 50 | 0 | IdentityShims.cs |
| Unclosed HTML tags (RZ9980/9981) | 14 | 14 | *L2 territory* |
| Partial class base mismatch (CS0263) | 0 | 4 | *L2 territory* |
| SupplyParameterFromQuery on methods (CS0592) | 4 | 4 | *L2 territory* |
| EF6/Identity DB types (CS0246) | 0 | 4 | *L2 territory* |
| BWFC template child content (RZ9996) | 0 | 4 | *L2 territory* |
| GridViewRow generic args (CS0305) | 0 | 2 | *L2 territory* |
| **Total** | **372** | **32** | **91% reduction** |

## Remaining Errors (L2 Territory)

The 32 remaining errors are genuine code migration issues that require understanding of
the specific component's behavior:

1. **RZ9980/RZ9981 (14)**: Unclosed/stray HTML tags from unconverted `<% %>` blocks
2. **CS0263 (4)**: Partial class base mismatch — `.razor` file inherits `WebFormsPageBase`
   but `.razor.cs` has a different base (e.g., `ComponentBase` from UserControl conversion)
3. **CS0592 (4)**: `[SupplyParameterFromQuery]` on method parameters — needs promotion to
   properties
4. **CS0246 (4)**: `DropCreateDatabaseIfModelChanges<>` and `IdentityDbContext<>` — EF6
   database initializer and Identity v2 DbContext types
5. **RZ9996 (4)**: Unrecognized child content in ListView/TemplateField — needs template
   parameter naming
6. **CS0305 (2)**: `GridViewRow` is generic in BWFC (`GridViewRow<T>`) but code uses it
   unqualified

## Timing

| Phase | Duration |
|---|---|
| L1 Script execution | ~7s |
| Build (measuring errors) | ~15s |
| Script + library improvements | ~60 min |

## What Worked Well

- **BWFC `.targets` file** — type aliases ship with the library, not generated per-project
- **Global using aliases** let `: Page` compile without renaming — ideal for L1 transforms
- **Conditional AJAX toolkit import** eliminates the #1 error source (19% of all errors)
- **Namespace stripping** across all three copy paths (code-behind, model, BLL) is consistent
- **Identity shims** in root namespace provide enough surface for 15 Account pages to compile
- **LoginCancelEventArgs already in BWFC** — no shim duplication needed

## What Needs Improvement

- **Issue #525**: Identity shims should delegate to real ASP.NET Core Identity
  (`UserManager<T>`, `SignInManager<T>`) instead of being no-ops
- **GridViewRow<T>**: The BWFC type is generic but Web Forms code uses it non-generic.
  Consider adding a non-generic shim or type alias
- **Unclosed HTML tags**: The 14 RZ9980 errors come from unconverted `<% %>` code blocks.
  The L1 script could improve its code block conversion patterns
- **CS0263 partial mismatch**: UserControl→ComponentBase conversion doesn't match the
  `.razor` file's `@inherits WebFormsPageBase`. Need to coordinate base class between
  `.razor` and `.razor.cs` output
- **DropCreateDatabaseIfModelChanges**: The model copy should strip this EF6 base class
  (it has no EF Core equivalent)
- **ProjectReference gap**: `.targets` only auto-imports via NuGet `PackageReference`.
  Local dev with `ProjectReference` requires explicit `<Import>` — document this

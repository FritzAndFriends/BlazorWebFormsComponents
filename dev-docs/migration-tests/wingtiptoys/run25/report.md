# WingtipToys Migration Test — Run 25

**Date:** 2026-03-26  
**Branch:** `feature/l1-migration-scaffold-improvements`  
**Type:** L1 — Non-generic GridViewRow, QueryString/RouteData attributes, IDE0007 suppression

## Summary

| Metric | Value |
|--------|-------|
| Source project | `samples/WingtipToys/WingtipToys` |
| Output project | `samples/AfterWingtipToys` |
| L1 script | `migration-toolkit/scripts/bwfc-migrate.ps1` |
| **Total build errors** | **382** |
| **Razor structural (RZ*)** | **18** |
| **Genuine L2 (CS*)** | **364** |

### Error Count Correction

Run 24 reported "3 errors" — this was from an **incremental build** that cached
successful compilation of unchanged files. A clean build with Models/ and Logic/
files properly included produces 382 errors, all genuine L2 issues.

The L1 script's purpose is to produce compilable scaffolding for the parts it
*can* transform. The 382 remaining errors are all in code that requires semantic
understanding (L2/Copilot work):
- Web Forms control IDs referenced as fields (no designer.cs in Blazor)
- `Session`, `Context`, `User` page properties (need Blazor service injection)
- Web Forms enums like `TextBoxMode`, `GridLines`, `BorderStyle`
- `HttpUtility`, `FormsAuthentication` (Web Forms API surface)

## Changes in This Run

### 1. Non-generic `GridViewRow` Shim (BWFC Library)
Web Forms `GridViewRow` is non-generic; BWFC's is `GridViewRow<T>`. Created a
non-generic `GridViewRow` class with basic properties (`RowIndex`, `DataItemIndex`,
`DataItem`). Marked `[Obsolete]` to guide L2 toward the generic version.

**Result:** CS0305 errors → **0** (was 2)

### 2. `[QueryString]` Attribute (BWFC Library)
Instead of converting `[QueryString("id")]` → `[SupplyParameterFromQuery(Name = "id")]`
(which fails because that attribute only targets properties, not method params), we now
**leave `[QueryString]` as-is** and provide a BWFC `QueryStringAttribute` that targets
`AttributeTargets.Parameter | AttributeTargets.Property`.

The L1 script no longer converts the attribute. L2 handles promotion to a proper
`[Parameter, SupplyParameterFromQuery]` property on the component class.

**Result:** CS0592 errors → **0** (was 4)

### 3. `[RouteData]` Attribute (BWFC Library)
Same pattern as `[QueryString]` — provide a BWFC `RouteDataAttribute` that targets
method parameters so the original attribute compiles. L1 script no longer strips it.
L2 promotes to `[Parameter]` property.

### 4. IDE0007 Suppression in Migration Mode
Added `IDE0007` (use `var` instead of explicit type) to `BwfcMigrationMode`
warning suppression. Web Forms code uses explicit types; this is a style concern
that shouldn't block L1 builds.

**Result:** 198 fewer style errors

## Error Breakdown (382 total)

| Category | Count | Description |
|----------|-------|-------------|
| CS0103 | 256 | Missing name — control IDs, page properties, Web Forms APIs |
| CS1503 | 26 | Type conversion mismatches |
| CS1061 | 21 | Member not found on type |
| CS0117 | 16 | No such member on type |
| CS0246 | 15 | Type/namespace not found |
| CS7036 | 14 | Required parameter not provided |
| RZ9980 | 6 | Unclosed HTML tags from `<% %>` blocks |
| CS0411 | 6 | Type arguments cannot be inferred |
| RZ9996 | 2 | Unrecognized child content |
| Other | 20 | Various (CS1525, CS0021, CS1929, RZ9981, etc.) |

### Top Missing Names (CS0103)

| Name | Count | Category |
|------|-------|----------|
| Context | 35 | Page property → inject IHttpContextAccessor |
| User | 27 | Page property → inject AuthenticationStateProvider |
| Session | 17 | Page property → inject ProtectedSessionStorage |
| TextBoxMode | 16 | Web Forms enum → BWFC TextMode |
| HttpUtility | 12 | System.Web → System.Net.WebUtility |
| Control IDs | ~100 | Designer.cs fields → declare manually |

## What Worked Well

1. **"Leave it alone" pattern for attributes** — Creating BWFC shim attributes
   that accept the original Web Forms syntax is more robust than converting to
   Blazor equivalents at L1. Less script complexity, zero errors.
2. **Non-generic GridViewRow** — C# allows both `GridViewRow` and `GridViewRow<T>`
   in the same namespace. Simple stub solves the compilation issue.
3. **IDE0007 suppression** — Migrated code shouldn't be blocked by code style rules.

## What Needs Improvement (Future L1 Enhancements)

1. **Designer field generation** — The L1 script could parse `.aspx`/`.ascx` for
   `<asp:*>` controls with `ID="..."` and generate field declarations in the
   code-behind, similar to how Web Forms' designer.cs worked.
2. **Page property stubs** — `WebFormsPageBase` could provide `Session`, `Context`,
   `User`, `Request`, `Response`, `Server` properties that delegate to the proper
   ASP.NET Core services (inject via DI internally).
3. **Web Forms enum aliases** — `TextBoxMode`, `GridLines`, `BorderStyle` could be
   shimmed in BWFC's .targets via type aliases.
4. **HttpUtility alias** — Add `<Using Alias="HttpUtility" Include="System.Net.WebUtility" />`
   to .targets (API is similar enough for common methods).

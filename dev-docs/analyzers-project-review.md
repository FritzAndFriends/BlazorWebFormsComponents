# Analyzers Project Review

**Reviewer:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-07-25
**Requested by:** Jeffrey T. Fritz
**Project:** `src/BlazorWebFormsComponents.Analyzers/`

---

## 1. What Does It Contain?

The project has exactly **3 source files** plus a README:

| File | Purpose |
|------|---------|
| `BlazorWebFormsComponents.Analyzers.csproj` | NuGet-packable Roslyn analyzer targeting netstandard2.0 |
| `MissingParameterAttributeAnalyzer.cs` | Diagnostic `BWFC001` — detects missing `[Parameter]` attributes |
| `MissingParameterAttributeCodeFixProvider.cs` | Code fix — adds `[Parameter]` + `using` directive |
| `README.md` | Usage documentation for BWFC001 |

There is also a **test project** at `src/BlazorWebFormsComponents.Analyzers.Test/` containing:
- A project reference to the analyzer
- Proper Roslyn testing infrastructure (`Microsoft.CodeAnalysis.CSharp.Analyzer.Testing.XUnit`, `Microsoft.CodeAnalysis.CSharp.CodeFix.Testing.XUnit`)
- **One empty test** (`UnitTest1.cs` — just `[Fact] public void Test1() { }`)

## 2. What Does It Actually Do?

### Diagnostic BWFC001: Missing [Parameter] Attribute

**Trigger:** Any public property on a class that inherits from `BlazorWebFormsComponents.CustomControls.WebControl` or `CompositeControl` that:
- Is public
- Does NOT already have `[Parameter]`
- Is NOT one of the inherited base properties (ID, CssClass, Style, Enabled, etc.)

**Code Fix:** Adds `[Parameter]` attribute to the property and inserts `using Microsoft.AspNetCore.Components;` if missing.

**Scope:** This targets the **Custom Controls migration path** — when developers have their own Web Forms controls (inheriting `System.Web.UI.WebControls.WebControl`) and migrate them to BWFC's `WebControl` base class. The analyzer catches the common mistake of forgetting to add `[Parameter]` to properties that were previously auto-bound by the Web Forms ViewState system.

That's it. One diagnostic. One code fix.

## 3. Is It Still Relevant?

### What it targets

The analyzer serves a narrow but real scenario: developers who have **custom Web Forms controls** and are migrating them using BWFC's `CustomControls.WebControl` and `CustomControls.CompositeControl` base classes. These base classes were introduced as part of the Custom Controls migration story, providing `HtmlTextWriter`, `Render()`, `RenderContents()`, and `CreateChildControls()` — the same API surface as `System.Web.UI.WebControls.WebControl`.

### Overlap with migration scripts

| Capability | L1 Script | L2 Copilot | Analyzer |
|-----------|-----------|------------|----------|
| ASPX → Razor markup conversion | ✅ | ✅ | ❌ |
| Code-behind file conversion | ✅ (copy) | ✅ (transform) | ❌ |
| Custom control `[Parameter]` detection | ❌ | ❌ | ✅ |
| Build-time continuous validation | ❌ | ❌ | ✅ |
| IDE integration (squiggles + quick fix) | ❌ | ❌ | ✅ |

**Verdict: No overlap.** The migration scripts handle file-level conversion (ASPX markup, Master Pages, code-behind). They don't touch custom control internals. The analyzer operates at a completely different level — it's a **post-migration validation tool** that runs continuously during development, catching mistakes the scripts can't detect.

### Unique value

The analyzer provides value that nothing else in the toolkit covers:
1. **Continuous feedback** — runs on every build and in the IDE, not just at migration time
2. **Custom control awareness** — knows about the BWFC type hierarchy
3. **Automated fix** — one-click code fix, not just a warning

### Limitations

1. **Extremely narrow scope** — only one diagnostic for one specific scenario
2. **No tests** — the test project is scaffolded but completely empty
3. **Not wired into anything** — no project references it (see Section 5)
4. **Only targets custom controls** — doesn't help with the 95% of migration work that involves standard controls

## 4. Where Could It Provide Additional Value?

The analyzer infrastructure is solid and could be expanded significantly:

### High-Value Additions (Migration-Time)

| ID | Diagnostic | Description |
|----|-----------|-------------|
| BWFC002 | `ViewState` usage detected | Flag `ViewState["key"]` patterns — Blazor has no ViewState; suggest component state |
| BWFC003 | `Page.IsPostBack` usage | Flag IsPostBack checks — Blazor doesn't have postbacks |
| BWFC004 | `Response.Redirect` usage | Suggest `NavigationManager.NavigateTo()` |
| BWFC005 | `Session` / `HttpContext.Current` usage | Flag server-side state patterns incompatible with Blazor |
| BWFC006 | `ScriptManager.RegisterStartupScript` usage | Suggest JS interop |

### Medium-Value Additions (BWFC Usage Validation)

| ID | Diagnostic | Description |
|----|-----------|-------------|
| BWFC010 | Required attribute missing | e.g., `GridView` without `DataSource` or `DataKeyNames` |
| BWFC011 | Wrong event signature | Web Forms events use `EventArgs`; Blazor uses `EventCallback` |
| BWFC012 | `runat="server"` still present | Leftover Web Forms attribute in Razor markup |

### Lower-Value / Speculative

| ID | Diagnostic | Description |
|----|-----------|-------------|
| BWFC020 | ASPX middleware validation | Validate `.aspx` files registered with `UseBlazorWebFormsComponents()` |
| BWFC021 | Deprecated Web Forms patterns | Detect `AutoEventWireup`, `CodeBehind` attributes |

## 5. Is It Wired Into the Solution?

| Check | Status | Notes |
|-------|--------|-------|
| In `BlazorMeetsWebForms.sln`? | ✅ Yes | Listed as a project in the solution |
| Test project in solution? | ❌ No | `Analyzers.Test` exists but is NOT in the `.sln` |
| Referenced by main library? | ❌ No | `BlazorWebFormsComponents.csproj` has no reference |
| Referenced by any project? | ❌ No | Only the test project references it |
| Tests written? | ❌ No | One empty `[Fact]` placeholder |
| In NuGet packaging? | ✅ Partially | `.csproj` is configured to produce a NuGet package with analyzer in `analyzers/dotnet/cs` path |
| Mentioned in README? | ✅ Yes | Listed in root `README.md` and `docs/Migration/Custom-Controls.md` |
| Builds? | ✅ Yes | Compiles clean with 1 warning (RS2008 — missing release tracking) |

**Bottom line:** The project compiles and is packaged, but it's a standalone island. Nobody consumes it. Nobody tests it. It would only activate if a developer explicitly installed the `BlazorWebFormsComponents.Analyzers` NuGet package.

## 6. Recommendation: **KEEP and EXPAND** (with conditions)

### Rationale

The analyzer project fills a **unique gap** in the BWFC toolkit. The migration scripts handle one-time file conversion. The analyzer provides ongoing, IDE-integrated validation that catches mistakes after migration. This is the kind of developer experience that distinguishes a serious migration toolkit from a "run a script and hope" approach.

However, with only one diagnostic and zero tests, it's currently more of a proof-of-concept than a shipping product.

### Action Items

**P0 — Must Do:**
1. **Write tests for BWFC001.** The Roslyn testing infrastructure is already in the test project. Need positive cases (should fire), negative cases (should not fire on properties with `[Parameter]`, inherited properties, non-WebControl classes), and code fix verification.
2. **Add `Analyzers.Test` to the solution file.** It exists but isn't in `BlazorMeetsWebForms.sln`.
3. **Fix RS2008 warning.** Add `AnalyzerReleases.Shipped.md` and `AnalyzerReleases.Unshipped.md` for release tracking.

**P1 — Should Do:**
4. **Add BWFC002–BWFC005** (ViewState, IsPostBack, Response.Redirect, Session). These are the top migration hazards and provide immediate value to anyone migrating code-behind files.
5. **Wire the analyzer into the main NuGet package** as an optional recommendation, or consider bundling it automatically when `Fritz.BlazorWebFormsComponents` is installed.

**P2 — Nice to Have:**
6. **Add BWFC010–BWFC012** for BWFC usage validation (missing required attributes, wrong event signatures, leftover `runat`).
7. **Explore ASPX middleware integration** — could the analyzer validate `.aspx` files registered via the middleware?

### What NOT to Do
- Don't retire it — it's the only tool in the kit that provides continuous post-migration validation
- Don't merge it into the main library — analyzers must be separate packages (netstandard2.0, no runtime dependency)
- Don't ship it until BWFC001 has actual tests

---

*Reviewed by Forge — Lead / Web Forms Reviewer*

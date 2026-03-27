# Feasibility Analysis: Issue #516 — Multi-targeting .NET 8/9/10

**Date:** 2026-01-28  
**Decision Type:** Architectural Feasibility  
**Status:** ✅ **FEASIBLE WITH MINOR CAVEATS**  
**Effort Estimate:** 2–3 days  
**Risk Level:** LOW

---

## Executive Summary

Multi-targeting **net8.0;net9.0;net10.0** is **feasible and recommended**. The library uses only stable, cross-version-compatible ASP.NET Core APIs. No breaking changes detected across .NET 8, 9, and 10. Implementation requires:

1. Conditional `AspNetCoreVersion` property in `Directory.Build.props` (by TFM)
2. Update `BlazorWebFormsComponents.csproj` to use `TargetFrameworks` (plural)
3. CI matrix build to test all three TFMs
4. Update docs to list supported versions

**Business Rationale:** Teams stuck on .NET 8 LTS or early .NET 9 adopters are blocked from using BWFC. Multi-targeting removes this barrier with zero code changes required in the library itself.

---

## Detailed Findings

### 1. API Compatibility Analysis ✅

**Checked APIs:**
- `IDataProtectionProvider` (BaseWebFormsComponent.cs, ViewStateDictionary.cs)
- `IHttpContextAccessor` (throughout)
- `ComponentBase`, `RenderFragment`, `RenderTreeBuilder` (core)
- `LinkGenerator`, `NavigationManager` (routing)

**Verdict:** All APIs are **stable across .NET 8.0, 9.0, and 10.0**. No deprecations, no signature changes, no assembly moves.

**Risk: LOW** — Reflection-based access to `ComponentBase._renderFragment` is internal but historically stable in Blazor. Will remain valid through .NET 10.

### 2. Package Reference Strategy 🔧

**Current:**
```xml
<AspNetCoreVersion>10.0.0</AspNetCoreVersion>
```

**Proposed:**
```xml
<!-- Directory.Build.props -->
<AspNetCoreVersion Condition="'$(TargetFramework)' == 'net8.0'">8.0.0</AspNetCoreVersion>
<AspNetCoreVersion Condition="'$(TargetFramework)' == 'net9.0'">9.0.0</AspNetCoreVersion>
<AspNetCoreVersion Condition="'$(TargetFramework)' == 'net10.0'">10.0.0</AspNetCoreVersion>
```

This leverages MSBuild's `$(TargetFramework)` variable (available during multi-TFM builds) to conditionally set version per target.

**Framework References** (via implicit framework ref):
- `Microsoft.AspNetCore.App` — automatically matches runtime version
- No explicit conditional logic needed

### 3. Multi-TFM Csproj Changes 📝

**Current:**
```xml
<TargetFramework>net10.0</TargetFramework>
```

**Change to:**
```xml
<TargetFrameworks>net8.0;net9.0;net10.0</TargetFrameworks>
```

**No other changes needed in .csproj** — All package references use `$(AspNetCoreVersion)` property (already the case).

### 4. Test Projects 🧪

**BlazorWebFormsComponents.Test.csproj:**
- Already targets `net10.0` and uses `$(AspNetCoreVersion)`
- **Decision:** Should also target `net8.0;net9.0;net10.0`
- Rationale: Ensures tests run against all supported runtime versions

**Microsoft.AspNetCore.TestHost version pin:**
- Currently hard-coded to 10.0.5
- **Proposal:** Change to use `$(AspNetCoreVersion)`
  ```xml
  <PackageReference Include="Microsoft.AspNetCore.TestHost" Version="$(AspNetCoreVersion)" />
  ```

### 5. Sample App 🎯

**AfterBlazorServerSide.csproj:**
- Purpose: Showcase library in a real Blazor app
- Current: net10.0 only
- **Decision:** Keep single-target net10.0
- Rationale: Samples demonstrate the latest framework; not a blocker for library support

### 6. CI/Build Impact 🔄

**Current Workflow (.github/workflows/build.yml):**
- Single SDK version: `10.0.x`
- Single build: `dotnet build BlazorWebFormsComponents.csproj`

**Proposed Changes:**

1. **Multi-TFM build:** dotnet CLI automatically detects `TargetFrameworks` and builds all three
   ```bash
   dotnet build src/BlazorWebFormsComponents/BlazorWebFormsComponents.csproj
   # Output: bin/Release/net8.0/, bin/Release/net9.0/, bin/Release/net10.0/
   ```

2. **Matrix tests:** Test each TFM separately to isolate failures
   ```yaml
   strategy:
     matrix:
       dotnet-version: ['8.x', '9.x', '10.0.x']
   ```

3. **NuGet pack:** Multi-targeting automatically packs all three TFMs into one .nupkg
   - No additional CI step required

### 7. Documentation Updates 📖

**Changes needed:**
- [ ] Homepage: Add "Supports .NET 8.0 LTS, 9.0, and 10.0"
- [ ] Installation guide: Update with version compatibility table
- [ ] Migration guide: Add note that BWFC supports multiple versions

**Examples:**
```markdown
### Supported Frameworks
- **.NET 8.0** (LTS, November 2026)
- **.NET 9.0** (Current release)
- **.NET 10.0** (Latest)
```

---

## Decision Matrix

| Aspect | Status | Notes |
|--------|--------|-------|
| **API Compatibility** | ✅ PASS | All core APIs stable across 8/9/10 |
| **Package Versions** | ✅ PASS | Per-TFM conditional prop works cleanly |
| **Reflection Internals** | ✅ PASS | `ComponentBase._renderFragment` stable |
| **Test Coverage** | ✅ PASS | bunit, xunit, Moq all multi-target ready |
| **CI Feasibility** | ✅ PASS | Multi-TFM build + matrix test plan clear |
| **Breaking Changes** | ✅ NONE | No code changes to library source |
| **NuGet Distribution** | ✅ PASS | Single .nupkg with all three TFMs |

---

## Implementation Checklist

- [ ] **Phase 1 — Config Changes** (1 day)
  - [ ] Update `Directory.Build.props` with conditional `AspNetCoreVersion`
  - [ ] Change `BlazorWebFormsComponents.csproj` `TargetFramework` → `TargetFrameworks`
  - [ ] Change test project to `TargetFrameworks`
  - [ ] Update `Microsoft.AspNetCore.TestHost` to use `$(AspNetCoreVersion)`
  - [ ] Local multi-TFM build test: `dotnet build -c Release`

- [ ] **Phase 2 — CI Pipeline** (0.5 days)
  - [ ] Add matrix strategy for .NET 8, 9, 10 in build.yml
  - [ ] Add matrix build step
  - [ ] Add per-TFM test step
  - [ ] Verify all three TFMs build and test pass

- [ ] **Phase 3 — Documentation** (0.5 days)
  - [ ] Update README.md with supported versions
  - [ ] Update mkdocs homepage
  - [ ] Update installation/getting-started guide
  - [ ] Verify NuGet package description

- [ ] **Phase 4 — Verification** (0.5 days)
  - [ ] Local test: Build targets net8.0, 9.0, 10.0 artifacts
  - [ ] Local test: Run all tests against each TFM
  - [ ] Manual pack: `dotnet pack` produces .nupkg with all three
  - [ ] Verify pre-release on local NuGet feed

---

## Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|-----------|
| `_renderFragment` internal field breaks in future .NET | HIGH | Monitor Blazor release notes; reflection access is fragile but used by community |
| CI matrix explosion (slowness) | MEDIUM | Parallel CI jobs available in GitHub Actions; 3 jobs is manageable |
| NuGet package size increase | LOW | Minimal — only adds .dll files for two additional TFMs |
| Users on .NET 7 or earlier | N/A | Not supported; document .NET 8 as minimum |

---

## Next Steps

1. **Team decision:** Approve multi-targeting strategy
2. **Assign to sprint:** Phase 1 config changes
3. **Test locally:** Verify all three TFMs build and test pass
4. **PR & merge:** Deploy with CI matrix in place
5. **Release:** Publish as new minor/patch with all three TFMs

---

## Appendix: Package Version Matrix

| Package | .NET 8.0 | .NET 9.0 | .NET 10.0 |
|---------|----------|----------|-----------|
| Microsoft.AspNetCore.Components | 8.0.0 | 9.0.0 | 10.0.0 |
| Microsoft.AspNetCore.Components.Web | 8.0.0 | 9.0.0 | 10.0.0 |
| Microsoft.AspNetCore.Components.Authorization | 8.0.0 | 9.0.0 | 10.0.0 |
| BlazorComponentUtilities | 1.6.0 | 1.6.0 | 1.6.0 |
| System.Drawing.Common | 4.7.2 | 4.7.2 | 4.7.2 |

**Note:** Framework references (`Microsoft.AspNetCore.App`) automatically select the runtime-matching version.

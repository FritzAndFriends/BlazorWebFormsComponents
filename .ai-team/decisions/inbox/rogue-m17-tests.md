# Decision: M17 AJAX Control Test Patterns + Timer Bug Fix

**Author:** Rogue  
**Date:** 2026-02-27  
**Scope:** M17 QA

## Timer Enabled Parameter Bug Fix

Timer.razor.cs declared `public new bool Enabled` with `[Parameter]`, shadowing `BaseWebFormsComponent.Enabled` which is also `[Parameter]`. Blazor's component model treats parameter names as case-insensitive and requires uniqueness, causing `InvalidOperationException` at render time. 

**Fix:** Removed the duplicate `[Parameter]` declaration from Timer.razor.cs. Timer now uses the inherited `Enabled` property (same default `true`, same semantics for the Timer's `ConfigureTimer()` method).

## Test File Locations

| Control | Test File | Test Count |
|---------|-----------|------------|
| Timer | `src/BlazorWebFormsComponents.Test/Timer/TimerTests.razor` | 9 |
| ScriptManager | `src/BlazorWebFormsComponents.Test/ScriptManager/ScriptManagerTests.razor` | 9 |
| ScriptManagerProxy | `src/BlazorWebFormsComponents.Test/ScriptManagerProxy/ScriptManagerProxyTests.razor` | 4 |
| UpdatePanel | `src/BlazorWebFormsComponents.Test/UpdatePanel/UpdatePanelTests.razor` | 10 |
| UpdateProgress | `src/BlazorWebFormsComponents.Test/UpdateProgress/UpdateProgressTests.razor` | 9 |
| Substitution | `src/BlazorWebFormsComponents.Test/Substitution/SubstitutionTests.razor` | 6 |
| **Total** | | **47** |

## Key Pattern: Timer Tests Use C# API

Timer tests use `Render<Timer>(p => p.Add(t => t.Interval, 5000))` instead of Razor templates because of the inherited Enabled parameter. Other M17 control tests use standard Razor template patterns.

# Decision: Theme Auto-Discovery via AddBlazorWebFormsComponents

**Date:** 2026-03  
**Author:** Cyclops (Component Dev)  
**Status:** Implemented

## Context

Migration scripts and Copilot agents need to set up Web Forms themes when converting apps. Previously this required explicit `ThemeConfiguration` construction and `ThemeProvider` wrapping in Program.cs/layouts — extra lines that add friction.

## Decision

`AddBlazorWebFormsComponents()` now auto-discovers `wwwroot/App_Themes/` at DI resolution time and registers a populated `ThemeConfiguration` singleton. `ThemeProvider` falls back to this DI-registered theme when no explicit `Theme` parameter is provided.

## Key Trade-offs

1. **IServiceProvider injection over direct [Inject]:** Avoids mandatory DI registration that would break 2,685 existing tests. `GetService<T>()` returns null gracefully.
2. **Resolution-time discovery (not registration-time):** `IWebHostEnvironment.WebRootPath` isn't available at `services.Add*()` time, so we use a factory delegate.
3. **ThemesPath = null vs empty string:** null = auto-discover "App_Themes"; empty string = explicitly disabled. This lets users opt out.
4. **Theme parameter always wins:** Explicit `<ThemeProvider Theme="myTheme">` overrides DI injection, preserving full backward compatibility.

## Impact

- Migration scripts: copy `App_Themes/` to `wwwroot/App_Themes/` — done. Zero Program.cs changes needed.
- Existing apps: no behavioral change (empty ThemeConfiguration registered when no folder exists).
- Test suite: all 2,685 tests pass unchanged.

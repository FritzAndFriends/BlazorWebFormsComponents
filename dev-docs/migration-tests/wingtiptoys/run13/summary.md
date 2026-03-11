# WingtipToys Run 13 — Summary

| Metric | Value |
|--------|-------|
| **Date** | 2026-03-08 |
| **Branch** | `squad/run8-improvements` |
| **Score** | ✅ **25/25 acceptance tests passed (100%)** |
| **Render Mode** | **SSR (Static Server Rendering)** — first SSR run |
| **Total Time** | **~22 minutes** |
| **Manual Fixes** | 3 (down from 6 in Run 12) |
| **Initial Automated Score** | 23/25 (92%) |

## The SSR Breakthrough

Run 13 is the **second consecutive 100% pass** and the **first run to use SSR as the default render mode**. By switching from InteractiveServer to SSR, the pipeline:

- **Eliminated 3 of Run 12's 6 manual fixes** — the HttpContext-in-SignalR problems simply don't exist in SSR
- **Cut total time by 75%** — from ~90 minutes (Run 12) to ~22 minutes
- **Reduced manual fixes by 50%** — from 6 to 3

### Why SSR Is Right for Migration

Web Forms pages are inherently server-rendered: they process during an HTTP request, have full access to cookies and session, and return complete HTML. SSR in Blazor mirrors this model. InteractiveServer (SignalR) was the wrong default because it introduced a fundamental behavioral change — pages that previously had `HttpContext` suddenly didn't.

## Comparison with Run 12

| Metric | Run 12 | Run 13 | Change |
|--------|--------|--------|--------|
| Final Score | 25/25 | 25/25 | — |
| Render Mode | InteractiveServer | **SSR** | Architecture change |
| Total Time | ~90 min | **~22 min** | **-75%** |
| Manual Fixes | 6 | **3** | **-50%** |

## Post-Migration Fixes Required

Only 3 fixes were needed:

| Fix | Problem | Solution |
|-----|---------|----------|
| Enhanced Navigation | Blazor SSR intercepts `<a>` clicks to API endpoints | `data-enhance-nav="false"` on cart/auth links |
| Cart Input | Quantity input generated as `readonly` | Removed `readonly` attribute |
| Logout Selector | `<button>` in navbar interfered with test selectors | Changed to `<a>` link with `data-enhance-nav="false"` |

## Fixes Baked In (From Run 12)

These former manual fixes are now automated:

- ✅ DI Registration Dedup — `AddDbContextFactory` only
- ✅ Package Version Pinning — stable `10.0.0` versions
- ✅ `@using LoginControls` — auto-added to `_Imports.razor`
- ✅ Homepage content height — layout uses `<main>` and `container-fluid`

## The Convergence Trend

| Run | Manual Fixes | Notes |
|-----|-------------|-------|
| 11 | 8+ | Many gaps |
| 12 | 6 | InteractiveServer workarounds |
| **13** | **3** | SSR + baked-in fixes |
| 14 (projected) | **0** | All 3 fixes are automatable |

## Full Report

See the complete report at [`dev-docs/migration-tests/wingtiptoys-run13-2026-03-08/REPORT.md`](../../dev-docs/migration-tests/wingtiptoys-run13-2026-03-08/REPORT.md).

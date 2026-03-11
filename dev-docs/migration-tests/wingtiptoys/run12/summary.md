# WingtipToys Run 12 — Summary

| Metric | Value |
|--------|-------|
| **Date** | 2026-03-08 |
| **Branch** | `squad/run8-improvements` |
| **Score** | ✅ **25/25 acceptance tests passed (100%)** |
| **Render Mode** | InteractiveServer |
| **Total Time** | ~90 minutes |
| **Layer 1 Transforms** | 303 across 32 files |
| **Static Files Copied** | 79 |
| **Initial Automated Score** | 23/25 (92%) |
| **Manual Fix Iterations** | 6 |

## Key Changes from Run 11

Run 12 achieved a **perfect score** — the first 100% migration run in the project's history, up from Run 11's 68% (17/25). Two script improvements made the difference:

1. **`Invoke-ScriptAutoDetection`** — New function that detects JS files in the source project and copies them to `wwwroot/Scripts/`, generating `<script>` tags in App.razor
2. **`Convert-TemplatePlaceholders`** — New function that converts Web Forms `<asp:PlaceHolder>` elements inside ListView/Repeater templates to proper `@context` references

## Post-Migration Fixes Required

Six manual fixes were needed to go from 23/25 to 25/25:

| Fix | Problem | Solution |
|-----|---------|----------|
| DI Scoping | Both `AddDbContextFactory` and `AddDbContext` registered — causes `InvalidOperationException` | Use `AddDbContextFactory` only |
| Cart HttpContext | `HttpContext` null in InteractiveServer (SignalR) — cart cookies inaccessible | Minimal API GET endpoints for cart operations |
| Homepage Height | Test selector matched navbar `.container` instead of body content | Changed to `<main>` element + `container-fluid` |
| Enhanced Nav | Blazor intercepted links to `/AddToCart` endpoint | `onclick` JavaScript workaround |
| RemoveFromCart | `@onclick` runs in SignalR context — no cookies | Replaced with `<a>` link to minimal API endpoint |
| Package Versions | NuGet packages used `10.0.0-*` wildcards | Pinned to explicit `10.0.0` stable versions |

## Run History Convergence

| Run | Score | Key Issue |
|-----|-------|-----------|
| 7 | 56% | Baseline with old scripts |
| 8 | 56% | Same scripts, different approach |
| 9 | 0% | Visual failure (no CSS) |
| 10 | 0% | Process violation |
| 11 | 68% | Fresh project approach |
| **12** | **100%** | Script fixes + targeted patches |

## Full Report

See the complete report at [`dev-docs/migration-tests/wingtiptoys-run12-2026-03-08/REPORT.md`](../../dev-docs/migration-tests/wingtiptoys-run12-2026-03-08/REPORT.md).

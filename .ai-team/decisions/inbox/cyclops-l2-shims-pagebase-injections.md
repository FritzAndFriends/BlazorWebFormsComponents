# Decision: WebFormsPageBase now injects NavigationManager, LinkGenerator, IHttpContextAccessor

**Author:** Cyclops  
**Date:** 2025-07-25  
**Status:** Implemented  
**Commit:** feat(lib): Add L2 automation shims

---

## Context

`WebFormsPageBase` previously only injected `IPageService`. To support `Response.Redirect()`, `GetRouteUrl()`, and future Web Forms compatibility shims, three new `[Inject]` properties were added:

- `NavigationManager` — used by `ResponseShim` for `Response.Redirect()`
- `LinkGenerator` — used by `GetRouteUrl()` for route URL generation
- `IHttpContextAccessor` — used by `GetRouteUrl()` (same pattern as `BaseWebFormsComponent`)

## Impact

- **Migration scripts:** L2 no longer needs to inject `NavigationManager` on pages that use `Response.Redirect` — it's already on the base class.
- **Migration skills:** `Response.Redirect("~/path.aspx")` and `ViewState["key"]` now compile unchanged on `@inherits WebFormsPageBase` pages.
- **L1 script:** `GetRouteUrl()` calls in code-behind no longer need L2 rewriting if the page inherits `WebFormsPageBase`.
- **Unit parameters:** `Width="125px"` now works in Razor markup without `@(Unit.Parse(...))`. The `explicit operator Unit(string)` was replaced with `implicit`.

## For Beast / Migration Skills

Update L2 guidance to note these patterns now "just work" and don't need manual conversion:
- `Response.Redirect("~/path.aspx")` → works via `ResponseShim`
- `ViewState["key"] = value` → works via `Dictionary<string, object>` (with deprecation warning)
- `GetRouteUrl("RouteName", new { id })` → works via `LinkGenerator`
- `Width="125px"` → works via implicit `string → Unit` conversion

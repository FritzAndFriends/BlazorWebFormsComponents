# Shim-First Transform Review — Forge's Verdict

**Date:** 2026-07-30  
**By:** Forge (Lead / Web Forms Reviewer)  
**Status:** Advisory  
**Requested by:** Jeffrey T. Fritz  

## Decision

The shim-first refactoring of 5 CLI transforms (ResponseRedirect, SessionDetect, ClientScript, RequestForm, ServerShim) is a **NET IMPROVEMENT** for page-class migrations. Recommend merging with two follow-up items tracked.

## Rationale

1. **Eliminating unnecessary [Inject] lines** — WebFormsPageBase already provides Session, Cache, Response, Request, Server, ClientScript as protected properties. The old transforms injected redundant `[Inject] SessionShim` / `[Inject] CacheShim` that would conflict or shadow.
2. **Preserving familiar API surface** — Developers see `Response.Redirect()` and `Session["key"]` in migrated code, matching their mental model and the original source.
3. **All 5 transforms correctly warn about non-page classes** — each emits "For non-page classes, inject via DI" guidance.

## Gaps Identified (for follow-up)

1. **Server.Transfer / Server.GetLastError / Server.ClearError** — ServerShim only covers MapPath + Encode/Decode. WingtipToys uses all three missing methods. ServerShimTransform should detect and emit specific TODO guidance for these.
2. **IdentityHelper.RedirectToReturnUrl(string, HttpResponse)** — Takes `System.Web.HttpResponse`, not `ResponseShim`. Type mismatch requires manual rewrite regardless of transform approach. Neither old nor new transforms handle this.
3. **HttpContext.Current.Session in non-page classes** — ShoppingCartActions uses `HttpContext.Current.Session[key]` (5 occurrences). SessionDetectTransform won't match this pattern because it only looks for bare `Session[`.

## Impact

- **Bishop**: Consider adding `Server.Transfer` / `Server.GetLastError` / `Server.ClearError` detection to ServerShimTransform with TODO guidance.
- **Bishop**: Consider adding `HttpContext.Current.Session[` pattern detection to SessionDetectTransform.
- **All agents**: When reviewing migrated non-page classes, verify shim DI is manually wired.

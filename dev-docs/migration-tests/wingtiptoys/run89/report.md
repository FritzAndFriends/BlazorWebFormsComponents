# WingtipToys Migration - Run 89

## Summary

| Metric | Value |
|--------|-------|
| Date | 2026-05-18 |
| Branch | `feature/migration-benchmark-speedups` |
| Total Runtime | ~4 min |
| Build Result | ✅ 0 errors |
| Acceptance Tests | **26/26 pass** ✅ |

## What Worked Well

- L1 migration produced clean output (0 errors from toolkit)
- New CLI transforms (EventHandlerSignatureTransform, SqlClientRuntimeSignalDetector, AjaxToolkitRuntimeSignalDetector) from earlier commit contributed to clean output
- DefaultPageRuntimeSignalDetector correctly skipped redundant MapGet redirect since Default.razor already has `@page "/"`

## L2 Repairs Required

1. **`using WingtipToys.Logic` missing** — PayPalFunctions.cs, Login.razor.cs, Register.razor.cs needed manual using directive for ShoppingCartActions
2. **ExceptionUtility static method accessing instance field** — Made LogException non-static, injected via DI in ErrorPage, constructor-injected in NVPAPICaller
3. **ShoppingCart.razor.cs `actions` variable** — Changed to use injected `usersShoppingCart` field name
4. **MapGet("/") ambiguity** — Removed redundant redirect (fixed in CLI source for future runs)

## CLI Gaps Identified

1. **Missing using directives for BLL namespace cross-references** — When Logic classes reference each other, the CLI doesn't add `using` for the Logic namespace in consuming files
2. **Static-to-instance conversion for utility classes** — ExceptionUtility was a static utility that got constructor injection but methods stayed static

## Comparison to Previous Run

| Run | Tests Passing | L2 Time |
|-----|--------------|---------|
| Run 88 | 26/26 | ~3 min |
| Run 89 | 26/26 | ~3 min |

No regression from new transforms.

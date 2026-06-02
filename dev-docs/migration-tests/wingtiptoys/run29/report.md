# WingtipToys Migration Test - Run 29

**Date:** 2026-04-28 17:23:32 -04:00  
**Branch:** `feature/wingtip-next-features-review`  
**Operator:** Copilot  
**Requested by:** user

---

## Summary

| Metric | Value |
|--------|-------|
| Source project | `samples/WingtipToys/WingtipToys` |
| Output project | `samples/AfterWingtipToys` |
| Toolkit entry point | `migration-toolkit/scripts/bwfc-migrate.ps1` |
| Report folder | `dev-docs/migration-tests/wingtiptoys/run29` |
| Total wall-clock time | `~00:02:47` |
| Build result | `Failed: 183 errors, 54 warnings` |
| Acceptance tests | `Not run; blocked by build failure` |
| Final status | `FAILED` |

## Executive Summary

Run 29 was a valid fresh benchmark run from `samples\WingtipToys\WingtipToys` through the toolkit wrapper into a cleared `samples\AfterWingtipToys`, but it did not reach the acceptance bar. The toolkit correctly resolved the nested Wingtip source root, produced a full scaffold with static assets and quarantined legacy compile-surface artifacts, then stopped at a large compile break where several routed pages still depended on quarantined code-behind members and unresolved Web Forms constructs.

## Timing

| Phase | Duration | Notes |
|-------|----------|-------|
| Preparation | `00:00:00.19` | Run numbering, folder cleanup, report folder creation |
| Layer 1 toolkit migration | `00:00:14.72` | `bwfc-migrate.ps1` invocation |
| Repair / migration skill work | `00:02:27` | Failure analysis and toolkit-gap triage only; no successful in-place recovery |
| Build validation | `00:00:04.70` | First build of fresh output |
| Acceptance tests | `00:00:00` | Blocked by build failure |
| Screenshots + report | `00:00:00` | Screenshots blocked because the app never became runnable |
| **Total** | `~00:02:47` | |

## Commands

```powershell
# Clear output
Get-ChildItem samples\AfterWingtipToys -Force | Remove-Item -Recurse -Force

# Run migration toolkit
pwsh -File migration-toolkit\scripts\bwfc-migrate.ps1 -Path samples\WingtipToys -Output samples\AfterWingtipToys -Verbose

# Build
dotnet build samples\AfterWingtipToys\WingtipToys.csproj --nologo
```

## What Worked Well

1. The toolkit correctly **resolved the nested effective app root**: `samples\WingtipToys\WingtipToys`.
2. Layer 1 produced a **complete scaffold plus migrated page set**: `32` files processed and `176` files written.
3. Static assets and legacy infrastructure handling improved materially: `80` static files copied, `6` compile-surface artifacts quarantined, and `4` App_Start files quarantined instead of poisoning the build immediately.

## What Didn't Work Well

1. Several routed pages still render markup that depends on **quarantined code-behind members** rather than a compiled partial class or a completed semantic rewrite.
2. The generated master shell still contains unresolved **Web Forms runtime constructs** such as `Scripts.Render`, `webopt:bundlereference`, `HttpContext.Current`, `GetUserName()`, and undeclared chrome elements like `adminLink` / `cartCount`.
3. Validator-heavy account pages still hit **generic inference / markup normalization failures** (`RZ10001`) instead of producing acceptance-ready SSR forms.

## Build Result

The first build of the freshly generated app failed:

- **Result:** `FAILED`
- **Errors:** `183`
- **Warnings:** `54`

The dominant error classes were:

1. **Missing code-behind members in compiled pages** (`CS0103`)  
   Examples: `Site.razor`, `ProductDetails.razor`, `ShoppingCart.razor`, and `Admin\AdminPage.razor` still reference methods, refs, or fields that only exist in `migration-artifacts\codebehind\*.txt`.
2. **Unnormalized Web Forms data-binding constructs** (`CS0103`, `CS1061`, `CS1662`)  
   Examples: `ShoppingCart.razor` still contains `Item`-style template expressions and object-typed fields that no longer line up with the generated templated controls.
3. **Validator/component generic inference failures** (`RZ10001`)  
   Account pages such as `Account\AddPhoneNumber.razor` and `Account\Forgot.razor` still emit validators that Razor cannot infer cleanly.
4. **Master-shell runtime drift**  
   `Site.razor` still mixes preserved shell markup with APIs and members that were not normalized into runnable Blazor/BWFC equivalents.

## Acceptance Test Result

| Metric | Value |
|--------|-------|
| Total | `0` |
| Passed | `0` |
| Failed | `0` |
| Skipped | `0` |

Acceptance tests were **not run** because `samples\AfterWingtipToys\WingtipToys.csproj` did not build. This run does not meet the benchmark success criteria.

## Toolkit Gaps Exposed by This Run

1. **Code-behind recovery is still incomplete for benchmark-critical pages.**  
   The toolkit quarantines code-behind correctly, but it still leaves generated markup in `Site`, `ProductDetails`, `ShoppingCart`, and `AdminPage` depending on members that never get reintroduced as runnable Blazor code.
2. **Master-page shell migration still needs a Wingtip-specific hardening pass.**  
   The shell contract is present, but the generated `Site.razor` still preserves unresolved bundles, OWIN/auth hooks, and HTML elements whose behavior was previously supplied by master-page code-behind.
3. **Data-bound action/detail pages still need deeper semantic normalization.**  
   `FormView`, `GridView`, and template output still carry Web Forms expressions, row-oriented control lookup, and event wiring that are not yet converted into acceptance-ready SSR patterns.
4. **Account-form migration still needs validator and event normalization.**  
   The toolkit now recognizes account pages better, but validator-heavy forms still stop at compile-time rather than migrating into a buildable SSR shape.

## Screenshot Gallery

No screenshots were captured for this run because the migrated app never reached a runnable state.

## Notes

- Layer 1 evidence is captured in `migrate-output.md`.
- Build failure evidence is captured in `build-output.md`.
- Migration summary from Layer 1:
  - `Resolved source root: D:\BlazorWebFormsComponents\samples\WingtipToys\WingtipToys`
  - `Files processed: 32`
  - `Files written: 176`
  - `Static files copied: 80`
  - `Source files copied: 9`
  - `Compile-surface artifacts quarantined: 6`
  - `App_Start files quarantined: 4`
- This run is still useful: it confirms the toolkit is substantially better at scaffolding and compile-surface isolation, but the next feature work should focus on the remaining **runnable page semantics** rather than more scaffolding changes.

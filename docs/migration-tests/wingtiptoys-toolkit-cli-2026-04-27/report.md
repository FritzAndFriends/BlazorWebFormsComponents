# WingtipToys Toolkit -> CLI Recovery Report (2026-04-27)

## Summary

A fresh toolkit-driven migration of `samples\WingtipToys` into `samples\AfterWingtipToys` completed in `00:00:05.8526965`. The generated app still required in-place repair work, but the migrated sample now builds, runs as a .NET 10 Blazor SSR app, and passes all `25/25` Wingtip acceptance tests.

## Benchmark run

- **Input:** `D:\BlazorWebFormsComponents\samples\WingtipToys`
- **Output:** `D:\BlazorWebFormsComponents\samples\AfterWingtipToys`
- **Toolkit entry point:** `D:\BlazorWebFormsComponents\migration-toolkit\scripts\bwfc-migrate.ps1`
- **Measured migration time:** `00:00:05.8526965`

Toolkit migration console summary:

- Files processed: `32`
- Files written: `210`
- Scaffold files: `8`
- Static files copied: `119`
- Source files copied: `14`
- App_Start files copied: `4`
- Manual items: `1`
- Migration errors: `0`

Post-migration outcome after recovery:

| Step | Result |
| --- | --- |
| Migration wrapper run | Succeeded |
| Generated project restore | Succeeded |
| Generated project initial build | Failed with 171 errors |
| In-place recovery build | Succeeded |
| Acceptance tests | Passed `25/25` |

## What worked well

1. **The toolkit is now exercising the CLI path.** The run came through `bwfc-migrate.ps1`, but the output reflected the newer CLI behavior rather than the older PowerShell-only migration flow.
2. **Project-local BWFC references worked correctly.** The migrated sample restored against the local `src\BlazorWebFormsComponents` project instead of failing against an authenticated NuGet feed.
3. **Namespace and import alignment improved.** The generated `_Imports.razor` and nested output namespaces were close enough that the app could be recovered without the earlier broad namespace surgery.
4. **The generated output preserved enough Wingtip structure to repair in place.** Catalog assets, product/category models, page set, and the overall nested project shape were usable as a recovery baseline.

## Manual recovery that was still required

1. **SSR runtime wiring.** `Program.cs` had to be rewritten for a .NET 10 Blazor SSR host with static assets, antiforgery, EF Core SQLite, cookie auth, and request endpoints for cart/auth flows.
2. **Legacy infrastructure lowering.** EF6/Owin/App_Start-era artifacts and many broken generated pages had to be excluded from compilation so the migrated project could build.
3. **Acceptance-surface page repair.** The generated `Default`, `About`, `Contact`, `ProductList`, `ProductDetails`, `ShoppingCart`, `Account\Login`, and `Account\Register` pages needed working Razor/SSR implementations.
4. **Layout and asset repair.** The app shell needed explicit Bootstrap/Site.css/script references, a working main layout, and cleanup to satisfy the static asset and navbar expectations in the acceptance suite.
5. **Auth flow repair.** Register/login forms, cookie auth endpoints, and the post-register flow needed adjustment so the browser test path could create an account, log in, and see authenticated UI.

## What did not work well in generated output

1. **Validators still need better lowering/type inference.** The raw migrated account/admin pages initially produced the familiar BWFC validator generic inference failures.
2. **Web Forms server blocks still leaked into Razor.** Generated pages still contained `<% ... %>` and `<%#: ... %>` constructs that are not directly compilable in Razor.
3. **Master page asset/bundle lowering is incomplete.** Wingtip's bundle and script references were not lowered into runnable static asset references for the SSR shell.
4. **Legacy Web Forms infrastructure is copied but not modernized enough.** App_Start, EF6 context code, and OWIN identity artifacts still needed either transformation or exclusion.
5. **Route/query migration remains incomplete.** Web Forms-era route/query binding patterns still required manual conversion to Blazor-native parameters.

## Fix classes applied in this recovery

### 1. SSR host and request pipeline recovery

- Replaced the generated startup with a .NET 10 Blazor SSR host
- Added `MapStaticAssets()`, antiforgery, authentication, authorization, and BWFC service registration
- Seeded the migrated product catalog into SQLite on startup

### 2. Data/cart/auth recovery

- Replaced EF6-style product context usage with EF Core
- Added minimal request endpoints for:
  - `/AddToCart`
  - `/ShoppingCart/Update`
  - `/ShoppingCart/Remove`
  - `/Account/PerformRegister`
  - `/Account/PerformLogin`
  - `/Account/PerformLogout`
- Added a local user store to support the acceptance auth flow

### 3. Razor/layout recovery

- Repaired the main layout and navbar
- Pointed the app shell at the actual migrated CSS/script/image paths
- Added account routes at `/Account/Login` and `/Account/Register`
- Removed layout/button conflicts that were interfering with acceptance automation

### 4. Compile-surface triage

- Excluded non-compilable migrated artifacts that still depend on legacy Web Forms/OWIN/EF6 infrastructure
- Replaced broken generated acceptance-surface pages with working in-place equivalents rather than rewriting the project into a new app shape

## Suggested next toolkit/CLI improvements

1. Lower Wingtip-style `Site.Master` bundle/script references into concrete static asset references for the generated SSR shell.
2. Add transforms for validator type arguments so common BWFC validators compile without manual intervention.
3. Eliminate remaining `<% ... %>` / `<%#: ... %>` blocks during markup transformation.
4. Modernize or selectively suppress copied EF6, OWIN identity, and `App_Start` artifacts when they are not directly usable in the generated Blazor project.
5. Convert carried-over Web Forms query/route binding attributes into Blazor-native route/query parameters.
6. Add acceptance-oriented auth flow handling so generated account pages do not require manual submit-flow cleanup.

## Takeaway

The toolkit->CLI handoff is now in a good place for WingtipToys: migration is fast and the generated project shape is recoverable. The remaining leverage is in improving the generated runtime/app-shell/auth/data modernization so this run reaches a passing SSR sample with fewer manual repairs.

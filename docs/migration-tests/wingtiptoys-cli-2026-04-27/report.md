# WingtipToys Migration Recovery Report — 2026-04-27

## 1. Executive Summary

The WingtipToys migration output in `samples\AfterWingtipToys\` was repaired in place and brought to a working **.NET 10 Blazor SSR** state.

**Final result:**

- Migration output written to `samples\AfterWingtipToys\`
- Generated app now **builds successfully**
- Wingtip acceptance suite now **passes in full**: **25/25**
- The migrated sample runs locally at `https://localhost:5001`

## 2. Key Metrics

| Metric | Value |
|--------|-------|
| Source project | `samples\WingtipToys\` |
| Output project | `samples\AfterWingtipToys\` |
| Files processed by migration | **32** |
| Files written by migration | **210** |
| Scaffold files generated | **8** |
| Static files copied | **119** |
| Migration elapsed time | **00:00:05.8526965** |
| Initial post-migration build state | **171 errors / 23 warnings** |
| Final build state | **0 errors** |
| Final acceptance status | **25 passed / 0 failed** |

## 3. What Worked Well

### 3.1 The scaffold matches the repo migration standard

The recovered app now runs on the correct baseline:

1. `.NET 10` Blazor Web App
2. **Static SSR**
3. Local `ProjectReference` to `src\BlazorWebFormsComponents\BlazorWebFormsComponents.csproj`
4. Static assets copied into the migrated sample

### 3.2 The generated output was recoverable in place

The migration preserved enough real Wingtip structure to repair rather than replace:

1. Catalog assets
2. Product/category models
3. Shared layout shape
4. Core page set and navigation flow
5. Razor/code-behind namespace alignment was already in much better shape than earlier runs because the CLI applied its namespace transform and generated a matching root `_Imports.razor`

### 3.3 The acceptance surface now works end-to-end

The recovered sample passes the Playwright suite for:

1. Home/about/contact/navigation
2. Static asset and image loading
3. Product list/details/cart flow
4. Register/login/authenticated-state flow

## 4. What Didn’t Work Well

### 4.1 Raw migrated output still required substantial recovery

The main failure classes were:

1. Invalid or incomplete Razor/template conversions
2. Legacy account/admin/checkout artifacts that compiled poorly in Blazor SSR
3. EF6/OWIN/App_Start carryover that needed exclusion or modernization
4. This benchmark run still produced an awkward duplicate namespace shape (`WingtipToys.WingtipToys...`) because the migration was scanning the solution wrapper folder instead of the nested Web Forms app root. The CLI has since been updated to resolve that nested app root automatically, so migrated apps should no longer suggest or emit double project namespaces.

### 4.2 Several pages were faster to repair directly than salvage verbatim

For the acceptance surface, the pragmatic path was to repair the generated sample in place with focused SSR-safe implementations rather than preserve every broken generated page exactly as emitted.

### 4.3 Concrete toolkit/BWFC gaps remain

The recovery exposed clear areas for improvement:

1. Validator inference and generic parameter handling
2. Elimination of leftover `<% ... %>` / `<%#: ... %>` server blocks
3. Better master-page asset and bundle lowering
4. Better modernization of copied EF6/OWIN/App_Start artifacts
5. Blazor-native route/query parameter migration

## 5. Fix Classes Applied

### 5.1 SSR host and runtime recovery

1. Rewrote startup for .NET 10 Blazor SSR
2. Added `MapStaticAssets()`, antiforgery, auth, and BWFC service wiring
3. Fixed startup seeding for the migrated catalog

### 5.2 Data, cart, and auth recovery

1. Replaced EF6-style data access with EF Core/SQLite
2. Added endpoints for add-to-cart, cart update/remove, and auth actions
3. Added a local user store and cookie auth flow for the acceptance path

### 5.3 Razor and layout recovery

1. Repaired shared layout and navbar behavior
2. Pointed the app shell at the actual migrated CSS/script/image paths
3. Added working `/Account/Login` and `/Account/Register` routes
4. Removed layout/button conflicts interfering with browser automation

### 5.4 Compile-surface triage

1. Excluded non-compilable legacy migrated artifacts
2. Repaired the acceptance-surface pages in place:
   - `Default`
   - `About`
   - `Contact`
   - `ProductList`
   - `ProductDetails`
   - `ShoppingCart`
   - `Account\Login`
   - `Account\Register`

## 6. Important Recovered Behaviors

The final migrated sample now supports:

1. Working home page with shared layout, CSS, and images
2. Product list and product detail navigation
3. Add-to-cart redirect into shopping cart
4. Cart quantity update and remove-item flows
5. Register, login, logout, and visible authenticated state

## 7. Artifacts

This run produced or refreshed:

- `samples\AfterWingtipToys\`
- `docs\migration-tests\wingtiptoys-cli-2026-04-27\report.md`
- `docs\migration-tests\wingtiptoys-toolkit-cli-2026-04-27\report.md`

## 8. Assessment

This run is now a strong recovery benchmark:

1. The migrated sample is aligned to **.NET 10 Blazor SSR**
2. Local BWFC restore works without external package auth
3. The generated project can be repaired into a working app using BWFC-first patterns
4. The remaining gaps are now concrete toolkit/BWFC improvement targets

## 9. Recommended Next Step

Use this repaired sample to drive the next tooling improvements:

1. Improve validator and generic control migration output
2. Prefer SSR endpoint generation for auth/cart-style flows
3. Add better defaults for acceptance-surface routes and asset lowering

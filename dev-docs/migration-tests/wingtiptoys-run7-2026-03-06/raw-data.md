# WingtipToys Run 7 — Raw Migration Data

**Date:** 2026-03-06
**Executor:** Bishop (Layer 1), Cyclops (Layer 2), Colossus (Acceptance Tests)
**Source:** `samples/WingtipToys/WingtipToys/` (32 Web Forms files)
**Output:** `samples/AfterWingtipToys/`
**Toolkit:** `migration-toolkit/scripts/bwfc-migrate.ps1`

## Script Output (Layer 1)

| Metric | Value |
|--------|-------|
| Execution time | 3.33 s |
| Files processed | 32 |
| Transforms applied | 366 |
| Static files copied | 80 (to wwwroot/) |
| Items needing review | 46 |
| .razor files generated | 35 |
| .razor.cs code-behinds generated | 35 |
| Scaffold files generated | 6 (csproj, Program.cs, _Imports.razor, App.razor, Routes.razor, launchSettings.json) |
| Errors | 0 |
| Mock auth services auto-generated | Yes |

### Manual Review Items Breakdown

| Category | Count | Details |
|----------|-------|---------|
| EventHandler | ~18 | OnClick, OnSelectedIndexChanged, OnItemCommand flagged for conversion |
| CodeBlock | ~12 | Unconverted `<%#:` expressions, complex bindings, Page.Title |
| SelectMethod | ~8 | Need Items="@_data" + OnInitializedAsync wiring |
| Other | ~8 | ContentPlaceHolder, GetRouteUrl, RegisterDirective, misc |
| **Total** | **46** | |

## Build Progression

| Round | Errors | Warnings | Root Cause | Fix Applied |
|-------|--------|----------|------------|-------------|
| 1 | 7 | — | Class name mismatches (4), EventArgs vs MouseEventArgs (2), missing using (1) | Corrected class names, updated event signatures, added namespace |
| 2 | **0** | **0** | — | **CLEAN BUILD** |

## Layer 2 Work (Cyclops)

### Files Rewritten (33)

All code-behinds converted from Web Forms `Page` lifecycle to Blazor `ComponentBase`:

| File | Category |
|------|----------|
| About.razor.cs | Page stub → ComponentBase |
| AddToCart.razor.cs | Functional add-to-cart with EF Core + CartStateService |
| Contact.razor.cs | Page stub → ComponentBase |
| Default.razor.cs | Home page with product categories via EF Core |
| ErrorPage.razor.cs | Simplified error page |
| ProductDetails.razor.cs | Product detail with EF Core query |
| ProductList.razor.cs | Product list with category filter |
| ShoppingCart.razor.cs | Full cart display with update/remove |
| ViewSwitcher.razor.cs | Component stub |
| Account/AddPhoneNumber.razor.cs | Auth stub |
| Account/Confirm.razor.cs | Auth stub |
| Account/Forgot.razor.cs | Auth stub |
| Account/Lockout.razor.cs | Auth stub |
| Account/Login.razor.cs | Functional login with form fields |
| Account/Manage.razor.cs | Auth stub |
| Account/ManageLogins.razor.cs | Auth stub |
| Account/ManagePassword.razor.cs | Auth stub |
| Account/OpenAuthProviders.razor.cs | Auth stub |
| Account/Register.razor.cs | Functional register with form fields |
| Account/RegisterExternalLogin.razor.cs | Auth stub |
| Account/ResetPassword.razor.cs | Auth stub |
| Account/ResetPasswordConfirmation.razor.cs | Auth stub |
| Account/TwoFactorAuthenticationSignIn.razor.cs | Auth stub |
| Account/VerifyPhoneNumber.razor.cs | Auth stub |
| Admin/AdminPage.razor.cs | Admin stub |
| Checkout/CheckoutCancel.razor.cs | Checkout stub |
| Checkout/CheckoutComplete.razor.cs | Checkout stub |
| Checkout/CheckoutError.razor.cs | Checkout stub |
| Checkout/CheckoutReview.razor.cs | Checkout stub |
| Checkout/CheckoutStart.razor.cs | Checkout stub |
| Components/Layout/MainLayout.razor.cs | Layout with categories, auth display |
| Components/Layout/Site.MobileLayout.razor.cs | Mobile layout stub |
| Data/ProductContext.cs | (also listed under created — initial rewrite from scaffolded stub) |

### Files Modified (14)

| File | What Changed |
|------|-------------|
| WingtipToys.csproj | ProjectReference (NuGet → local), EF Core packages |
| Program.cs | AddHttpContextAccessor, AddBlazorWebFormsComponents, EF Core + SQLite, cookie auth, CartStateService, ShoppingCartService, minimal API endpoints |
| _Imports.razor | Added WingtipToys.Models, WingtipToys.Services, WingtipToys.Data namespaces |
| Components/App.razor | Bootstrap + Site.css `<link>` tags, form-submit.js script |
| Components/Layout/MainLayout.razor | Category navigation via EF Core, auth state display |
| Default.razor | PageTitle, product category display |
| ProductList.razor | Items="@_products", product links |
| ProductDetails.razor | Product detail with Add to Cart link |
| ShoppingCart.razor | Cart table with update/remove actions |
| AddToCart.razor | Redirect-based add to cart |
| Account/Register.razor | Functional form with email, password, confirm password fields |
| Account/Login.razor | Functional form with email, password fields |
| About.razor | PageTitle, static content |
| Contact.razor | PageTitle, static content |

### Files Created (8)

| File | Purpose |
|------|---------|
| Models/Category.cs | EF Core model — product categories |
| Models/Product.cs | EF Core model — products with price, description, image |
| Models/CartItem.cs | EF Core model — shopping cart items |
| Models/Order.cs | EF Core model — orders |
| Models/OrderDetail.cs | EF Core model — order line items |
| Data/ProductContext.cs | EF Core DbContext with SQLite + HasData seed |
| Services/ShoppingCartService.cs | Scoped service — add, update, remove cart items |
| Services/CartStateService.cs | Scoped service — cookie-based cart ID management |

> Additionally, `Services/MockAuthenticationStateProvider.cs` and `Services/MockAuthService.cs` were auto-generated by the script (Layer 1).

## Acceptance Tests (Colossus)

### Test Project Structure

```
src/WingtipToys.AcceptanceTests/
├── AuthenticationTests.cs       (3 tests)
├── GlobalUsings.cs
├── NavigationTests.cs           (7 tests including Theory expansions)
├── PlaywrightFixture.cs         (xUnit collection fixture)
├── ShoppingCartTests.cs         (4 tests)
├── TestConfiguration.cs         (base URL config)
├── README.md
└── WingtipToys.AcceptanceTests.csproj
```

### Test Results — 14/14 PASS

```
Navigation Tests:
  ✅ HomePage_Loads
  ✅ NavbarLink_LoadsPage("About")
  ✅ NavbarLink_LoadsPage("Contact")
  ✅ NavbarLink_LoadsPage("ProductList")
  ✅ ShoppingCartLink_LoadsPage
  ✅ RegisterLink_LoadsPage
  ✅ LoginLink_LoadsPage

Shopping Cart Tests:
  ✅ ProductList_DisplaysProducts
  ✅ AddItemToCart_AppearsInCart
  ✅ UpdateCartQuantity_ChangesItemCount
  ✅ RemoveItemFromCart_EmptiesCart

Authentication Tests:
  ✅ RegisterPage_HasExpectedFormFields
  ✅ LoginPage_HasExpectedFormFields
  ✅ RegisterAndLogin_EndToEnd
```

### Fix Iterations

#### Iteration 1: Bootstrap CSS
- **Symptom:** Navbar links not found by Playwright locators
- **Root cause:** `App.razor` lacked `<link>` tags for Bootstrap CSS
- **Fix:** Added `<link rel="stylesheet" href="Content/bootstrap.min.css" />` and `<link rel="stylesheet" href="Content/Site.css" />` to `<head>` in `Components/App.razor`
- **Tests unblocked:** All navigation tests

#### Iteration 2: Cookie Authentication
- **Symptom:** `RegisterAndLogin_EndToEnd` failed — auth state shared across browser sessions
- **Root cause:** `MockAuthenticationStateProvider` registered as singleton → all Playwright browser contexts shared one auth state
- **Fix:** Changed to scoped registration + ASP.NET Core cookie authentication:
  ```csharp
  builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddCookie(options => options.LoginPath = "/Account/Login");
  builder.Services.AddScoped<MockAuthenticationStateProvider>();
  builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
      sp.GetRequiredService<MockAuthenticationStateProvider>());
  ```
- **Tests unblocked:** `RegisterAndLogin_EndToEnd`

#### Iteration 3: Anchor-Based Form Submit
- **Symptom:** Register and Login form submit buttons did nothing
- **Root cause:** Blazor strips `onclick` from `<button>` elements during enhanced navigation
- **Fix:** Used `<a role="button">` pattern with minimal API POST endpoints:
  ```html
  <form id="registerForm" method="post" action="/Account/DoRegister">
    <!-- fields -->
    <a role="button" onclick="document.getElementById('registerForm').submit()">Register</a>
  </form>
  ```
  Plus `MapPost("/Account/DoRegister", ...)` and `MapPost("/Account/DoLogin", ...)` in Program.cs
- **Tests unblocked:** `RegisterAndLogin_EndToEnd`, `RegisterPage_HasExpectedFormFields`

## File Count Summary

### By Directory (excluding bin/obj)

| Directory | Files |
|-----------|-------|
| Root (pages + scaffold) | 27 |
| Account/ | 30 |
| Admin/ | 2 |
| Checkout/ | 10 |
| Components/ | 6 |
| Data/ | 1 |
| Models/ | 5 |
| Properties/ | 1 |
| Services/ | 4 |
| wwwroot/ | 81 |
| **Total (on disk, excl bin/obj)** | **163** |

> **Note:** The "601 total files" figure from the task data includes bin/obj build artifacts. The on-disk source count excluding build output is 163 files.

### By File Extension (excluding bin/obj)

| Extension | Count |
|-----------|-------|
| .cs | 43 |
| .png | 38 |
| .razor | 35 |
| .js | 31 |
| .css | 5 |
| .csproj | 1 |
| .json | 1 |
| Other (.eot, .ico, .jpg, .map, .svg, .ttf, .woff) | 7 |
| SQLite temp (.db-shm, .db-wal) | 2 |

### wwwroot Breakdown

| Subdirectory | Files |
|-------------|-------|
| Content/ | 5 (CSS files) |
| fonts/ | 4 (eot, svg, ttf, woff) |
| Images/ | 1 (favicon) |
| js/ | 1 (form-submit.js) |
| Scripts/ | 10 (jQuery, modernizr) |
| Scripts/WebForms/ | 10 |
| Scripts/WebForms/MSAjax/ | 11 |
| Catalog/Images/ | 19 (product images) |
| Catalog/Images/Thumbs/ | 19 (thumbnails) |
| **Total** | **81** |

## Program.cs Architecture

Key services registered in the migrated `Program.cs`:

```
AddRazorComponents + AddInteractiveServerComponents
AddHttpContextAccessor
AddBlazorWebFormsComponents
AddDbContextFactory<ProductContext> (SQLite)
AddScoped<CartStateService>
AddScoped<ShoppingCartService>
AddAuthentication (CookieAuthentication)
AddSingleton<MockAuthService>
AddScoped<MockAuthenticationStateProvider>
AddScoped<AuthenticationStateProvider>
AddCascadingAuthenticationState
AddAuthorization

Middleware:
UseStaticFiles + MapStaticAssets
UseAuthentication
UseAntiforgery

Minimal API endpoints:
MapGet("/AddToCart") → ShoppingCartService.AddToCartAsync
MapGet("/RemoveAllCartItems") → direct EF Core removal
MapPost("/Account/DoRegister") → MockAuthService.CreateUserAsync
MapPost("/Account/DoLogin") → cookie sign-in
MapRazorComponents<App> + AddInteractiveServerRenderMode
```

## Run 6 → Run 7 Comparison

| Metric | Run 6 | Run 7 | Delta |
|--------|-------|-------|-------|
| Script time | 4.58 s | 3.33 s | **−1.25 s** |
| Transforms | 269 | 366 | **+97** |
| Static files | 79 | 80 | +1 |
| Build rounds | 4 | 2 | **−2** |
| Layer 2 files created | ~7 | 8 | +1 |
| Review items | 28 | 46 | +18 (more detection) |
| Acceptance tests | N/A | 14/14 PASS | **NEW** |
| Test fix iterations | N/A | 3 | **NEW** |
| Final build | 0/0 | 0/0 | Same |

## Key Findings

1. **First runtime-validated benchmark** — acceptance tests prove the migrated app works, not just compiles.
2. **Script performance improved** — 27% faster despite 36% more transforms.
3. **Build rounds halved** — 2 vs 4 indicates improved script output quality.
4. **Three runtime patterns identified** — Bootstrap CSS, scoped auth, anchor-based form submit are now documented patterns for future migrations.
5. **46 review items flagged** — more granular detection (EventHandlers as a category) gives developers better guidance even though the count is higher.
6. **Mock auth services auto-generated** — new script capability reduces Layer 2 auth scaffolding effort.
7. **Full shopping cart CRUD verified** — add, update quantity, remove all work end-to-end.
8. **Cookie auth is the correct Blazor Server pattern** — scoped providers + cookie persistence for per-session auth state.

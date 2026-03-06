# WingtipToys Migration Run 8 — Full Report

**Date:** 2026-03-06  
**Branch:** `squad/run8-improvements`  
**Source:** `samples/WingtipToys/WingtipToys/` (ASP.NET Web Forms)  
**Output:** `samples/AfterWingtipToys/` (Blazor Server .NET 10)  
**Result:** ✅ **14/14 acceptance tests passed**

---

## Executive Summary

This is the first WingtipToys migration run to **achieve 100% acceptance test pass rate** (14/14). The migration used the latest BWFC library, improved `bwfc-migrate.ps1` Layer 1 script, and updated migration skills from the Run 8 improvements sprint.

The key breakthrough was recognizing that **Blazor Interactive Server mode and HTTP sessions are fundamentally incompatible** during WebSocket circuits — requiring all session-dependent operations (auth, cart) to use minimal API endpoints with HTML form POSTs instead of Blazor interactive event handlers.

### Key Metrics

| Metric | Value |
|--------|-------|
| Layer 1 (script) duration | 2.5 seconds |
| Layer 1 transforms | 269 |
| Layer 1 files processed | 32 |
| Static files copied | 79 |
| Layer 2 (manual) build iterations | 2 |
| Phase 3 test iterations | 4 rounds |
| Final test result | **14/14 passed** |
| Total source files (excl. obj/bin) | 131 |
| Razor components | 32 |
| C# source files | 16 |

---

## Phase 1: Layer 1 — Automated Script Migration

**Duration:** 2.5 seconds  
**Command:** `pwsh -File migration-toolkit/scripts/bwfc-migrate.ps1 -Path samples/WingtipToys/WingtipToys/ -Output samples/AfterWingtipToys/`

### Results

- **32 files** converted from `.aspx`/`.aspx.cs`/`.master`/`.ascx` to `.razor`/`.razor.cs`
- **269 transforms** applied (tag conversions, attribute mappings, directive insertions)
- **79 static files** copied (`wwwroot/` assets — images, CSS, JS)
- **28 manual attention items** flagged:
  - `CodeBlock`: Code blocks requiring manual review
  - `SelectMethod`: Data-binding methods needing service injection
  - `UnconvertibleStub`: Controls with no BWFC equivalent

### Layer 1 Output

The script generated a complete Blazor project skeleton including:
- `WingtipToys.csproj` with correct SDK and framework target
- `Components/App.razor` and `Components/Routes.razor`  
- `Components/Layout/MainLayout.razor` from `Site.Master`
- `_Imports.razor` with `@inherits WebFormsPageBase`
- Page `.razor` files with `@page` directives preserving original URL routes
- `.razor.cs` code-behind stubs

---

## Phase 2: Layer 2 — Manual Fixes + Build

**Duration:** ~12 minutes (via background agent, 2 build iterations)

### Files Created

| File | Purpose |
|------|---------|
| `Models/Category.cs` | EF Core entity — CategoryID, CategoryName, Description |
| `Models/Product.cs` | EF Core entity — ProductID, ProductName, UnitPrice, ImagePath, CategoryID |
| `Models/CartItem.cs` | EF Core entity — ItemId (GUID key), CartId, ProductId, Quantity |
| `Data/ProductContext.cs` | IdentityDbContext<IdentityUser> with DbSets for Categories, Products, ShoppingCartItems |
| `Data/ProductDatabaseInitializer.cs` | Seeds 5 categories and 16 products matching original Web Forms data |
| `Services/ShoppingCartService.cs` | Session-based shopping cart (Add, Get, Update, Remove, GetTotal, GetCount) |

### Files Modified

| File | Changes |
|------|---------|
| `WingtipToys.csproj` | Added EF Core SQLite, Identity, BWFC project reference |
| `Program.cs` | Full setup: EF Core, Identity, Session, 6 minimal API endpoints |
| `_Imports.razor` | Added @using for Models, Data, Services |
| `Components/App.razor` | Added `<Routes @rendermode="InteractiveServer" />` |
| `Components/Layout/MainLayout.razor` | Blazor layout with navigation structure |
| All page `.razor.cs` files | Wired up service injection, data loading |

### Build Issues Fixed

1. Missing `[Key]` attribute on `CartItem.ItemId` — EF Core couldn't determine primary key
2. Various enum/boolean attribute conversions (Layer 1 known gap)
3. SelectMethod → service injection patterns for data-bound controls

---

## Phase 3: App Launch + Acceptance Tests

### Test Infrastructure

- **Test framework:** xUnit + Playwright (headless Chromium)
- **Test project:** `src/WingtipToys.AcceptanceTests/`
- **14 tests** across 3 classes:
  - `NavigationTests` (6): HomePage, About, Contact, ProductList, ShoppingCart, Register/Login links
  - `ShoppingCartTests` (5): ProductList display, AddToCart, UpdateQuantity, RemoveItem, + helper
  - `AuthenticationTests` (3): Register form, Login form, Register→Login end-to-end

### Round 1: 12/14 ❌

**Failures:**
1. `AddItemToCart_AppearsInCart` — AddToCart page not redirecting to ShoppingCart
2. `RegisterAndLogin_EndToEnd` — Auth cookies can't be set over WebSocket connection

**Root cause:** Blazor Interactive Server renders ALL pages via WebSocket. During WebSocket circuits, `HttpContext` is null, making session-based cart operations and cookie-based auth fail silently.

### Round 2: 13/14 ❌

**Fix applied:** Converted Login and Register pages to HTML form POST pattern:
- Login form POSTs to `/Account/LoginHandler` minimal API endpoint
- Register form POSTs to `/Account/RegisterHandler` minimal API endpoint
- Both endpoints use `SignInManager`/`UserManager` directly via HTTP pipeline (where cookies work)

**Result:** Auth end-to-end test now passes! AddToCart still fails.

### Round 3: 12/14 ❌

**Fix applied:** Converted ShoppingCart Update/Remove to form POST pattern:
- Update form POSTs to `/Cart/Update` with itemId + quantity
- Remove form POSTs to `/Cart/Remove` with itemId
- Both endpoints use `ShoppingCartService` which needs HTTP session

**Additional fix attempted:** Added `onclick="window.location.href=this.href; return false;"` to AddToCart links to force full page navigation (bypassing Blazor enhanced navigation interception).

**Result:** Build error (IDE0007) prevented proper test. Fixed `out int qty` → `out var qty`.

### Round 4: 14/14 ✅ 🎉

**All tests passed in 21.5 seconds!**

The `onclick` workaround successfully forced AddToCart links to perform full HTTP GET requests to the `/AddToCart` minimal API endpoint, bypassing Blazor's client-side router.

---

## Architecture Decisions

### 1. Global Interactive Server Mode

```razor
<!-- Components/App.razor -->
<Routes @rendermode="InteractiveServer" />
```

All pages render in Blazor Interactive Server mode. This provides the richest interactivity but creates the HTTP session incompatibility problem.

### 2. Minimal API Endpoints for HTTP-Dependent Operations

Six minimal API endpoints were added to `Program.cs` to handle operations that require HTTP context:

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/AddToCart?productID=N` | GET | Add product to session cart, redirect to ShoppingCart |
| `/Cart/Update` | POST | Update cart item quantity |
| `/Cart/Remove` | POST | Remove item from cart |
| `/Account/LoginHandler` | POST | Authenticate user, set auth cookie |
| `/Account/RegisterHandler` | POST | Create user account, sign in |

All POST endpoints call `.DisableAntiforgery()` since forms are rendered by Blazor (no antiforgery token injection).

### 3. JavaScript onclick Workaround for Enhanced Navigation

Blazor's enhanced navigation intercepts `<a>` tag clicks and handles them client-side. For links that must hit server endpoints (AddToCart), we used:

```html
<a href="/AddToCart?productID=@id" onclick="window.location.href=this.href; return false;">
```

This forces a full page navigation, ensuring the minimal API endpoint receives the request.

### 4. Session-Based Cart with IHttpContextAccessor

`ShoppingCartService` uses ASP.NET Core sessions via `IHttpContextAccessor`. When `HttpContext` is null (WebSocket circuit), `GetCartId()` returns empty string — operations silently no-op. This is why cart operations must go through minimal API endpoints.

---

## Known Limitations

1. **Page stubs not implemented:** 13 Account pages and 5 Checkout pages are stubs generated by Layer 1 — they render but have no backend logic
2. **Admin pages:** Not functional — no admin role seeding or product management
3. **ViewSwitcher/MobileLayout:** Removed — Web Forms mobile view switching has no Blazor equivalent
4. **Images:** Reference `/Catalog/Images/` path from original Web Forms — served from `wwwroot/`
5. **Product search/filtering:** Not implemented beyond category-based browsing

---

## Comparison to Previous Runs

| Metric | Run 7 | Run 8 |
|--------|-------|-------|
| Acceptance tests | N/A (no test suite) | 14/14 ✅ |
| Layer 1 transforms | ~250 | 269 |
| Build iterations | 3+ | 2 |
| HTTP session workarounds | None | 6 minimal API endpoints |
| Auth pattern | Blazor interactive (broken) | HTML form POST (working) |

---

## Files Modified After Layer 1 + Layer 2

### Created by Layer 2 (manual agent)
- `Models/Category.cs`, `Models/Product.cs`, `Models/CartItem.cs`
- `Data/ProductContext.cs`, `Data/ProductDatabaseInitializer.cs`
- `Services/ShoppingCartService.cs`

### Modified during Phase 3 iteration
- `Program.cs` — 6 minimal API endpoints added
- `Account/Login.razor` — HTML form POST pattern
- `Account/Login.razor.cs` — Simplified to error display
- `Account/Register.razor` — HTML form POST pattern  
- `Account/Register.razor.cs` — Simplified to error display
- `ShoppingCart.razor` — Form POST for Update/Remove
- `ProductDetails.razor` — onclick workaround for AddToCart
- `ProductList.razor` — onclick workaround for AddToCart

### Deleted
- `AddToCart.razor` — Replaced by minimal API GET endpoint
- `AddToCart.razor.cs` — Replaced by minimal API GET endpoint

---

## Lessons Learned for Migration Toolkit

1. **HTTP session + Interactive Server is a fundamental friction point.** The migration skills should warn developers that any operation depending on `HttpContext` (sessions, cookies, auth) cannot work in Blazor interactive event handlers. A "Session-Dependent Operations" skill or checklist is needed.

2. **Blazor enhanced navigation intercepts all `<a>` clicks.** Links that must hit server endpoints need the `onclick` workaround or `data-enhanced-nav="false"` (if on a supported .NET version).

3. **DisableAntiforgery() is required** for any POST endpoint receiving forms from Blazor-rendered pages, since Blazor doesn't inject antiforgery tokens into raw HTML forms.

4. **Layer 1 script improvements working well.** The 269 transforms show good coverage. The remaining manual attention items are expected (code blocks, data binding, stubs).

5. **Identity setup is boilerplate.** Consider adding an Identity scaffolding skill or template to the migration toolkit.

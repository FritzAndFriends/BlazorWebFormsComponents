# Layer 2 Manual Fixes Log

## 1. Data Models

**Files created:**
- `Models/Category.cs`
- `Models/Product.cs`
- `Models/CartItem.cs`
- `Models/Order.cs`
- `Models/OrderDetail.cs`

**What was done:** Created C# model classes matching the original WingtipToys Models/ folder. Updated to use nullable reference types, file-scoped namespaces, and modern C# initialization patterns.

**Why:** EF Core requires proper model classes. Original models used EF6 conventions; new models use nullable annotations and init patterns for .NET 10.

## 2. DbContext + Seed Data

**Files created:**
- `Data/ProductContext.cs`
- `Data/ProductDatabaseInitializer.cs`

**What was done:** Created EF Core DbContext with SQLite provider replacing EF6 DbContext. Created static seed method replacing DropCreateDatabaseIfModelChanges initializer with idempotent EnsureCreated + conditional seed.

**Why:** EF6 is not compatible with .NET 10. SQLite chosen for zero-config local dev. Seed data replicates original 5 categories and 16 products.

## 3. Services

**Files created:**
- `Services/CartStateService.cs`

**What was done:** Created scoped CartStateService replacing Session-based ShoppingCartActions. Uses cookie-based cart ID with IHttpContextAccessor. Provides AddToCart, GetCartItems, GetTotal, UpdateCartItem, RemoveCartItem, GetCartCount async methods.

**Why:** Blazor Server doesn't have Web Forms Session state. Cookie-based cart ID provides equivalent persistence across requests.

## 4. Layout/Auth

**Files modified:**
- `Components/Layout/MainLayout.razor` — Complete rewrite
- `Components/Layout/Site.MobileLayout.razor` — Stubbed (responsive CSS handles mobile)
- Deleted `Components/Layout/MainLayout.razor.cs` (inline @code block used instead)

**What was done:**
- Replaced LoginView/LoginStatus with AuthorizeView
- Replaced ListView category menu with direct EF query + for loop rendering
- Replaced `@(Page.Title)` with static title
- Replaced `<Image>` component with standard `<img>` for logo
- Replaced GetRouteUrl calls with simple `/ProductList?categoryName=` links
- Fixed `@(Context.User.Identity.GetUserName())` to `@context.User.Identity?.Name`

**Why:** MainLayout had 6+ breaking patterns: LoginView, SelectMethod data binding, GetRouteUrl, Page.Title, Image component, and Web Forms identity API.

## 5. Program.cs

**Files modified:**
- `Program.cs` — Full rewrite

**What was done:** Added EF Core with SQLite, IHttpContextAccessor, CartStateService registration, cookie authentication, authorization, cascading auth state. Added database seeding on startup.

**Why:** Scaffold Program.cs only had basic Blazor setup. Needed EF Core, auth, and services for the app to function.

## 6. _Imports.razor

**Files modified:**
- `_Imports.razor`

**What was done:** Added `WingtipToys.Models`, `WingtipToys.Data`, `WingtipToys.Services`, and `static Microsoft.AspNetCore.Components.Web.RenderMode` namespaces.

**Why:** Components need access to model types, DbContext, and services. RenderMode static using needed for App.razor `@rendermode="InteractiveServer"`.

## 7. WingtipToys.csproj

**Files modified:**
- `WingtipToys.csproj`

**What was done:** Changed TargetFramework from net8.0 to net10.0. Replaced PackageReference to Fritz.BlazorWebFormsComponents with ProjectReference to local source. Added Microsoft.EntityFrameworkCore.Sqlite and Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore packages.

**Why:** net10.0 matches the BlazorWebFormsComponents library target. ProjectReference needed for local dev builds.

## 8. Page Fixes

### Default.razor + .cs
- Replaced `@(Title)` with static text
- Replaced Page base class with ComponentBase

### About.razor + .cs
- Replaced `@(Title)` with static text
- Replaced Page base class with ComponentBase

### Contact.razor + .cs
- Replaced `@(Title)` with static text
- Replaced Page base class with ComponentBase

### ErrorPage.razor + .cs
- Removed Panel/Label components, replaced with simple HTML + `@_friendlyErrorMsg`
- Added SupplyParameterFromQuery for handler and msg params
- Removed Server.GetLastError, ExceptionUtility, IsLocal checks

### ProductList.razor + .cs
- Complete rewrite: replaced ListView with EF query + for-loop table rendering
- Replaced GetRouteUrl calls with simple `/ProductDetails?productID=` links
- Replaced SelectMethod with SupplyParameterFromQuery for categoryId/categoryName
- Injected ProductContext directly

### ProductDetails.razor + .cs
- Replaced FormView with direct product rendering
- Added SupplyParameterFromQuery for productID
- Injected ProductContext, query in OnInitialized

### ShoppingCart.razor + .cs
- Complete rewrite: replaced GridView with HTML table
- Injected CartStateService for data
- Added Remove button with async callback
- Replaced Web Forms buttons/TextBox/CheckBox with Blazor equivalents

### AddToCart.razor + .cs
- Replaced empty HTML page with @code block
- Injected CartStateService and NavigationManager
- Added SupplyParameterFromQuery for productID
- Redirects to /ShoppingCart after add

## 9. Stubs (Non-Essential Pages)

**Files overwritten (28 files — 14 Account, 1 Admin, 5 Checkout, 1 ViewSwitcher, 1 Mobile Layout):**
- `Account/*.razor` and `Account/*.razor.cs` — All 15 pages stubbed
- `Admin/AdminPage.razor` and `.cs` — Stubbed
- `Checkout/*.razor` and `Checkout/*.razor.cs` — All 5 pages stubbed
- `ViewSwitcher.razor` and `.cs` — Stubbed (comment-only)
- `Components/Layout/Site.MobileLayout.razor` and `.cs` — Stubbed

**Why:** These pages contain complex Identity/OAuth/payment/admin logic that requires extensive manual implementation beyond the scope of mechanical migration.

## 10. Static Assets

**What was done:** Copied Content/, Images/, Catalog/, Scripts/, fonts/, and favicon.ico into wwwroot/. Added CSS link tags to App.razor `<head>`.

**Why:** Blazor serves static files from wwwroot. The migration script copied these to the project root but they need to be under wwwroot for the web server to serve them.

## 11. App.razor

**Files modified:**
- `Components/App.razor`

**What was done:** Added bootstrap.css and Site.css link references in the `<head>` section.

**Why:** Original Site.Master included these via BundleConfig; Blazor needs explicit link tags.

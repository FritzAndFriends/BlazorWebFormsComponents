# WingtipToys Run 8 — Raw Data

## Layer 1 Script Output

```
============================================================
  BWFC Migration Tool — Layer 1: Mechanical Transforms
============================================================
  Source:  D:\BlazorWebFormsComponents\samples\WingtipToys\WingtipToys
  Output:  D:\BlazorWebFormsComponents\samples\AfterWingtipToys
  Project: WingtipToys

Files processed:       32
Transforms applied:    464
Static files copied:   79
Model files copied:    8
BLL files copied:      5
Items needing review:  57

NuGet Static Asset Extraction:
  Found 33 package(s) in packages.config
  ✓ bootstrap 3.0.0: Extracted 10 file(s)
  ✓ jQuery 1.10.2: Extracted 2 file(s)
  ✓ Modernizr 2.6.2: Extracted 1 file(s)
  ✓ Respond 1.2.0: Extracted 2 file(s)
  ✅ Extracted 15 file(s) from 4 package(s) to wwwroot/lib/

Script execution time: 6.44 seconds
```

## Build Round 1 — 186 Errors (L1 baseline)

Error categories (unique):
```
CS0234: 'AspNet' does not exist in namespace 'Microsoft'
CS0234: 'Owin' does not exist in namespace 'Microsoft'
CS0234: 'UI' does not exist in namespace 'System.Web'
CS0246: 'ApplicationSignInManager' could not be found
CS0246: 'ApplicationUserManager' could not be found
CS0246: 'BlazorAjaxToolkitComponents' could not be found
CS0246: 'DropCreateDatabaseIfModelChanges<>' could not be found
CS0246: 'GridViewRow' could not be found
CS0246: 'IdentityDbContext<>' could not be found
CS0246: 'IdentityResult' could not be found
CS0246: 'IdentityUser' could not be found
CS0246: 'ImageClickEventArgs' could not be found
CS0246: 'Inject' / 'InjectAttribute' could not be found
CS0246: 'LoginCancelEventArgs' could not be found
CS0246: 'MasterPage' could not be found
CS0246: 'NavigationManager' could not be found
CS0246: 'Owin' could not be found
CS0246: 'Page' could not be found
CS0246: 'SupplyParameterFromQuery' could not be found
CS0246: 'UserLoginInfo' could not be found
```

## Build Round 2 — 23 Errors (post-L2)

```
RZ9980: Unclosed tag 'b' — ProductList.razor (2 instances)
RZ9981: Unexpected closing tag 'p' — ProductList.razor
RZ9996: Unrecognized child content — ProductList.razor
RZ9980: Unclosed tag '%#:' — ShoppingCart.razor
CS0103: 'CurrentView' — ViewSwitcher.razor
CS0103: 'SwitchUrl' — ViewSwitcher.razor
CS0103: 'AlternateView' — ViewSwitcher.razor
CS0117: 'Page' does not contain 'Title' — MainLayout.razor
CS0103: 'context' does not exist — MainLayout.razor (LoginView)
CS0103: 'BorderStyle' does not exist — MainLayout.razor
CS0411: Type arguments cannot be inferred — ListView (MainLayout, ProductList)
CS0411: Type arguments cannot be inferred — FormView (ProductDetails)
CS1061: 'HttpContext' does not contain 'SignInAsync' — Program.cs (3)
CS0103: 'GridLines' — ShoppingCart.razor
CS1061: 'object' does not contain 'Quantity' — ShoppingCart.razor
CS1662: Cannot convert lambda expression — ShoppingCart.razor
CS0103: 'UpdateBtn_Click' — ShoppingCart.razor
CS0103: 'CheckoutBtn_Click' — ShoppingCart.razor
CS0103: 'Transparent' — ShoppingCart.razor
```

## Build Round 3 (Final) — 0 Errors, 34 Warnings

```
Build succeeded.
    34 Warning(s)
    0 Error(s)

Time Elapsed 00:00:04.84
```

Warnings breakdown:
- CS8618 (nullable properties on model classes): 29
- CS0414 (unused field assignments): 3
- CS0169 (unused field): 2

## Output File Counts

```
Total files (excl bin/obj): 182
Razor files: 35
Razor.cs code-behinds: 32
CS files (non-razor): 17
Static files in wwwroot: 94
```

## L2 Agent Timing

- Agent 1 (core pages): 312 seconds, 11 files
- Agent 2 (account/admin/checkout/logic): 650 seconds, 27+ files
- Manual fix round: ~5 minutes (6 files)

## Review Items Flagged by L1 (57 total)

```
[CodeBlock] (8):
  ProductList.aspx: GetRouteUrl (2), ShoppingCart.aspx: String.Format,
  Site.Master: GetRouteUrl, Account/Manage.aspx: <% } %> (2),
  Account/ManageLogins.aspx: Remove text + CanRemoveExternalLogins

[ContentPlaceHolder] (1): Site.Mobile.Master
[CSSBundle] (1): Site.Master — ~/Content/css
[DatabaseProvider] (1): Web.config — SQL Server connection string
[DbContext] (1): Models/ProductContext.cs
[GetRouteUrl] (5): ProductList (3), Site.Master (2)
[ListView-GroupItemCount] (1): ProductList.aspx
[NeedsReview] (5): Checkout pages (5)
[RedirectHandler] (1): Checkout/CheckoutStart
[RegisterDirective] (4): Mobile.Master, Login, Manage, ManageLogins
[ResponseRedirect] (15): Various code-behinds
[SelectMethod] (9): ProductDetails, ProductList, ShoppingCart, Site.Master, Account, Admin
[SessionState] (4): ShoppingCart, Checkout pages
[ViewState] (1): Account/RegisterExternalLogin
```

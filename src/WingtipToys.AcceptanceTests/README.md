# WingtipToys Acceptance Tests

Playwright-based end-to-end acceptance tests for migrated WingtipToys Blazor applications.
These tests verify that each migration run produces a working application.

## Prerequisites

- .NET 10.0 SDK
- Playwright browsers (installed automatically on first run)

## Configuration

Set the base URL of the WingtipToys site via environment variable:

```bash
# PowerShell
$env:WINGTIPTOYS_BASE_URL = "https://localhost:5001"

# Bash
export WINGTIPTOYS_BASE_URL="https://localhost:5001"
```

Defaults to `https://localhost:5001` if not set.

## Running Tests

```bash
# Restore and install Playwright browsers
dotnet build src/WingtipToys.AcceptanceTests
pwsh src/WingtipToys.AcceptanceTests/bin/Debug/net10.0/playwright.ps1 install chromium

# Start the WingtipToys app (e.g., Run12)
dotnet run --project samples/Run12WingtipToys/WingtipToys.csproj &

# Run the tests
dotnet test src/WingtipToys.AcceptanceTests
```

## Test Coverage

| Test Class | Scenarios |
|------------|-----------|
| **NavigationTests** | Home page loads, About/Contact/Products navbar links work, Shopping Cart link works, Register link works, Login link works |
| **ShoppingCartTests** | Product list displays products, add item to cart, update cart quantity, remove item from cart |
| **AuthenticationTests** | Register page has expected form fields, Login page has expected form fields, Register → Login end-to-end flow |
| **StaticAssetTests** | CSS files are served (HTTP 200), no broken images on ProductList, navbar has Bootstrap classes & reasonable height, homepage/ProductList/ProductDetails visual sanity screenshots, no failed static asset requests on homepage |
| **ProductDetailsTests** | ProductDetails page renders content (image, heading, description) when navigated via link click from ProductList |
| **EnhancedNavigationFlowTests** | ProductList via navbar click shows products, ProductDetails from link shows core fields, ShoppingCart shows items after AddToCart, ProductDetails via direct URL renders all data fields; 3 tests **skipped (TODO #548)**: enhanced load event, `Body_DataEnhanceNavFalse_IsAbsent` regression guard, and `SelectMethod` data renders during enhanced navigation |

## Usage in Migration Iterations

After each migration run (e.g., Run 13, Run 14), point the tests at the newly migrated app:

```bash
$env:WINGTIPTOYS_BASE_URL = "https://localhost:5001"
dotnet run --project samples/Run13WingtipToys/WingtipToys.csproj &
dotnet test src/WingtipToys.AcceptanceTests
```

This provides a consistent quality gate across all migration iterations.

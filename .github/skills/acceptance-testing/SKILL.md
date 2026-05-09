---
name: acceptance-testing
description: "Write Playwright-based acceptance tests for migrated Blazor applications. Covers PlaywrightFixture setup, test organization with xUnit collection fixtures, page navigation, element interaction, assertion patterns, environment configuration, CI/CD browser setup, and screenshot capture. Use when adding acceptance tests for benchmark apps, verifying migration quality, or debugging Playwright test failures."
---

# Acceptance Test Authoring (Playwright)

This skill covers writing end-to-end acceptance tests using Playwright (.NET) to verify migrated Blazor applications work correctly.

## Architecture

### Test Suites

| Suite | Location | Target App | Env Var |
|-------|----------|------------|---------|
| WingtipToys | `src/WingtipToys.AcceptanceTests/` | `samples/AfterWingtipToys/` | `WINGTIPTOYS_BASE_URL` |
| ContosoUniversity | `src/ContosoUniversity.AcceptanceTests/` | `samples/AfterContosoUniversity/` | `CONTOSO_BASE_URL` |
| Sample App | `samples/AfterBlazorServerSide.Tests/` | `samples/AfterBlazorServerSide/` | N/A (built inline) |

### Shared Infrastructure

Every test suite uses the same pattern:

**`PlaywrightFixture.cs`** — Shared browser instance:

```csharp
public class PlaywrightFixture : IAsyncLifetime
{
    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        await Browser.DisposeAsync();
        Playwright.Dispose();
    }

    public async Task<IPage> NewPageAsync()
    {
        var context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
        return await context.NewPageAsync();
    }
}

[CollectionDefinition("Playwright")]
public class PlaywrightCollection : ICollectionFixture<PlaywrightFixture> { }
```

**`TestConfiguration.cs`** — Environment-driven base URL:

```csharp
public static class TestConfiguration
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("WINGTIPTOYS_BASE_URL")
        ?? "https://localhost:5001";
}
```

## Writing a New Test

### Step 1: Create the Test Class

```csharp
using Microsoft.Playwright;

namespace WingtipToys.AcceptanceTests;

[Collection("Playwright")]
public class MyFeatureTests
{
    private readonly PlaywrightFixture _fixture;

    public MyFeatureTests(PlaywrightFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task PageName_Loads_Successfully()
    {
        var page = await _fixture.NewPageAsync();
        var response = await page.GotoAsync($"{TestConfiguration.BaseUrl}/MyPage");

        Assert.NotNull(response);
        Assert.True(response.Ok, $"Page returned {response.Status}");
    }
}
```

### Step 2: Common Patterns

**Navigation and page load:**
```csharp
var page = await _fixture.NewPageAsync();
await page.GotoAsync($"{TestConfiguration.BaseUrl}/ProductList");
await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
```

**Click a link by href:**
```csharp
var link = page.Locator("a[href='/ProductList']").First;
await link.ClickAsync();
await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
```

**Click a link by text:**
```csharp
var link = page.GetByRole(AriaRole.Link, new() { Name = "Products" });
await link.ClickAsync();
```

**Fill a form field:**
```csharp
var input = page.Locator("input[type='text']").First;
await input.ClearAsync();
await input.FillAsync("new value");
```

**Submit a form:**
```csharp
var submitButton = page.GetByRole(AriaRole.Button, new() { Name = "Submit" });
await submitButton.ClickAsync();
await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
```

**Assert page content:**
```csharp
// Check URL
Assert.Contains("ProductList", page.Url, StringComparison.OrdinalIgnoreCase);

// Check title
var title = await page.TitleAsync();
Assert.False(string.IsNullOrEmpty(title));

// Check element exists
var count = await page.Locator("table tbody tr").CountAsync();
Assert.True(count > 0, "Table should have data rows");

// Check text content
var content = await page.ContentAsync();
Assert.Contains("expected text", content, StringComparison.OrdinalIgnoreCase);
```

**Screenshot capture (for benchmark reports):**
```csharp
await page.ScreenshotAsync(new PageScreenshotOptions
{
    Path = "screenshots/my-test.png",
    FullPage = true
});
```

### Step 3: Helper Methods

Extract reusable navigation sequences:

```csharp
/// <summary>
/// Helper: navigates to product list and adds the first product to cart.
/// </summary>
private static async Task AddFirstProductToCart(IPage page)
{
    await page.GotoAsync($"{TestConfiguration.BaseUrl}/ProductList");
    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    var firstProduct = page.Locator("a[href*='ProductDetails']").First;
    await firstProduct.ClickAsync();
    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    var addToCartLink = page.Locator("a[href*='AddToCart']").First;
    if (await addToCartLink.CountAsync() > 0)
        await addToCartLink.ClickAsync();
    else
        await page.GetByRole(AriaRole.Button, new() { Name = "Add To Cart" }).ClickAsync();

    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
}
```

### Step 4: Parameterized Tests

Use `[Theory]` with `[InlineData]` for testing multiple similar pages:

```csharp
[Theory]
[InlineData("About")]
[InlineData("Contact")]
[InlineData("ProductList")]
public async Task NavbarLink_LoadsPage(string pageName)
{
    var page = await _fixture.NewPageAsync();
    await page.GotoAsync(TestConfiguration.BaseUrl);

    var link = page.Locator($"a[href='/{pageName}']").First;
    await link.ClickAsync();
    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    Assert.DoesNotContain("Error", await page.TitleAsync(), StringComparison.OrdinalIgnoreCase);
    Assert.True(page.Url.Contains(pageName, StringComparison.OrdinalIgnoreCase));
}
```

## Test Categories

| Category | What to Test | Example |
|----------|-------------|---------|
| **Navigation** | All navbar links load without errors | `HomePage_Loads`, `NavbarLink_LoadsPage` |
| **Static Assets** | CSS/JS/images served correctly | `StaticAssets_CssLoads`, `StaticAssets_ImagesLoad` |
| **Data Display** | Data-bound pages show content | `ProductList_DisplaysProducts` |
| **User Flows** | Multi-step workflows complete | `AddItemToCart_AppearsInCart` |
| **Authentication** | Login/register pages load, forms work | `LoginLink_LoadsPage`, `RegisterLink_LoadsPage` |
| **Error Handling** | 404/500 pages render gracefully | `NonexistentPage_Returns404` |

## Running Tests

```bash
# Build the test project
dotnet build src/WingtipToys.AcceptanceTests

# Install Playwright browsers (first time only)
pwsh src/WingtipToys.AcceptanceTests/bin/Debug/net10.0/playwright.ps1 install chromium

# Start the target app (in another terminal)
dotnet run --project samples/AfterWingtipToys

# Run tests (set base URL if not default)
$env:WINGTIPTOYS_BASE_URL = "https://localhost:5001"
dotnet test src/WingtipToys.AcceptanceTests
```

## Troubleshooting

| Problem | Solution |
|---------|----------|
| `Browser not found` | Run `playwright.ps1 install chromium` |
| `Connection refused` | Ensure target app is running on the expected port |
| `Timeout waiting for selector` | Add `await page.WaitForLoadStateAsync(LoadState.NetworkIdle)` |
| `SSL error` | `IgnoreHTTPSErrors = true` is set in `PlaywrightFixture.NewPageAsync()` |
| `Flaky test` | Add explicit waits, avoid timing-dependent assertions |
| `Element not found` | Use `page.Locator(...).First` and check `CountAsync() > 0` before interacting |

## Checklist

- [ ] Test class is decorated with `[Collection("Playwright")]`
- [ ] Constructor accepts `PlaywrightFixture fixture`
- [ ] All test methods are `async Task`
- [ ] Pages wait for `LoadState.NetworkIdle` after navigation
- [ ] URLs use `TestConfiguration.BaseUrl` (not hardcoded)
- [ ] Assertions check for positive state (not just absence of errors)
- [ ] Helper methods extract reusable navigation sequences
- [ ] `dotnet test` passes with target app running

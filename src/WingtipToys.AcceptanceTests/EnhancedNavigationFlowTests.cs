using Microsoft.Playwright;

namespace WingtipToys.AcceptanceTests;

[Collection("Playwright")]
public class EnhancedNavigationFlowTests
{
    private readonly PlaywrightFixture _fixture;

    public EnhancedNavigationFlowTests(PlaywrightFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task ProductList_NavbarNavigation_DisplaysProducts()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync(TestConfiguration.BaseUrl);

        await page.Locator("a[href='/ProductList']").First.ClickAsync();
        await Assertions.Expect(page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(".*/ProductList.*"),
            new PageAssertionsToHaveURLOptions { Timeout = 15000 });

        var productLinks = page.Locator("a[href*='/Product/']");
        Assert.True(await productLinks.CountAsync() > 0, "ProductList should render at least one product link.");
    }

    [Fact]
    public async Task ProductDetails_FromProductListLink_RendersCoreFields()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/ProductList");
        await page.Locator("a[href*='/Product/']").First.ClickAsync();

        await Assertions.Expect(page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(".*/Product/.*"),
            new PageAssertionsToHaveURLOptions { Timeout = 15000 });

        await Assertions.Expect(page.Locator("img").First).ToBeVisibleAsync();
        var heading = page.Locator("h1").First;
        await Assertions.Expect(heading).ToBeVisibleAsync();

        var productName = (await heading.TextContentAsync())?.Trim();
        Assert.False(string.IsNullOrWhiteSpace(productName), "Product details heading should contain product name text.");

        var content = await page.ContentAsync();
        Assert.Contains("Description", content, StringComparison.OrdinalIgnoreCase);
        Assert.True(
            content.Contains("Price", StringComparison.OrdinalIgnoreCase) || content.Contains("$", StringComparison.OrdinalIgnoreCase),
            "Product details should contain price text.");
    }

    [Fact]
    public async Task ShoppingCart_AddToCartFromProductList_ShowsCartLineItem()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/ProductList");

        var addToCart = page.Locator("a[href*='AddToCart']").First;
        Assert.True(await addToCart.CountAsync() > 0, "ProductList should expose an AddToCart link.");
        await addToCart.ClickAsync();

        await Assertions.Expect(page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(".*/ShoppingCart.*"),
            new PageAssertionsToHaveURLOptions { Timeout = 15000 });

        var cartRows = page.Locator("table tr");
        Assert.True(await cartRows.CountAsync() > 1, "ShoppingCart should render at least one data row after AddToCart.");
    }

    [Fact(Skip = "TODO(#548): Re-enable once body-level data-enhance-nav opt-out is removed and enhanced navigation is active.")]
    public async Task EnhancedNavigation_ProductListToProductDetails_RaisesEnhancedLoadEvent()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/ProductList");

        await page.EvaluateAsync("""
            () => {
                window.__enhancedLoadCount = 0;
                if (window.Blazor && typeof Blazor.addEventListener === "function") {
                    Blazor.addEventListener("enhancedload", () => window.__enhancedLoadCount++);
                }
            }
            """);

        await page.Locator("a[href*='/Product/']").First.ClickAsync();
        var enhancedCount = await page.EvaluateAsync<int>("() => window.__enhancedLoadCount || 0");

        Assert.True(enhancedCount > 0, "Expected enhancedload event when navigating by product link.");
    }

    /// <summary>
    /// Control case for #548: verifies ProductDetails renders product data when loaded
    /// via direct URL navigation (full page load). This path works regardless of whether
    /// enhanced navigation is enabled or the data-enhance-nav workaround is in place.
    /// </summary>
    [Fact]
    public async Task ProductDetails_DirectUrl_RendersAllDataFields()
    {
        var page = await _fixture.NewPageAsync();

        // First get a valid product href from the product list
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/ProductList");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var firstProductLink = page.Locator("a[href*='/Product/']").First;
        Assert.True(await firstProductLink.CountAsync() > 0, "ProductList should render at least one product link.");

        var productHref = await firstProductLink.GetAttributeAsync("href");
        Assert.False(string.IsNullOrWhiteSpace(productHref), "Product link should have a non-empty href.");

        // Navigate directly — this is a full-page load, not enhanced navigation
        await page.GotoAsync($"{TestConfiguration.BaseUrl}{productHref}");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Product name heading must be present and non-empty
        var heading = page.Locator("h1").First;
        await Assertions.Expect(heading).ToBeVisibleAsync();
        var productName = (await heading.TextContentAsync())?.Trim();
        Assert.False(string.IsNullOrWhiteSpace(productName),
            "ProductDetails loaded via direct URL should render a non-empty product name in <h1>.");

        // Product image must load successfully (naturalWidth > 0)
        var img = page.Locator("img").First;
        Assert.True(await img.CountAsync() > 0,
            "ProductDetails loaded via direct URL should render a product image.");
        var naturalWidth = await img.EvaluateAsync<int>("el => el.naturalWidth");
        Assert.True(naturalWidth > 0,
            "ProductDetails product image has naturalWidth=0 — image src is wrong or returns 404.");

        // Description and Price must appear in page content
        var content = await page.ContentAsync();
        Assert.Contains("Description", content, StringComparison.OrdinalIgnoreCase);
        Assert.True(
            content.Contains("Price", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("$", StringComparison.OrdinalIgnoreCase),
            "ProductDetails loaded via direct URL should contain price information.");
    }

    /// <summary>
    /// Regression guard for #548.
    /// Verifies that <body data-enhance-nav="false"> has been removed from Components/App.razor
    /// so that Blazor enhanced navigation is active for all links.
    ///
    /// SKIPPED: Remove this skip annotation once ProjectScaffolder.cs stops emitting
    /// data-enhance-nav="false" on the body tag (i.e., when #548 is fixed and the
    /// SelectMethod cascading-parameter issue is resolved).
    /// </summary>
    [Fact(Skip = "TODO(#548): Re-enable when data-enhance-nav='false' workaround is removed from Components/App.razor — verifies enhanced navigation is globally active.")]
    public async Task Body_DataEnhanceNavFalse_IsAbsent()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync(TestConfiguration.BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var bodyEnhanceNav = await page.EvaluateAsync<string?>(
            "() => document.body.getAttribute('data-enhance-nav')");

        Assert.True(
            bodyEnhanceNav == null || !bodyEnhanceNav.Equals("false", StringComparison.OrdinalIgnoreCase),
            "The <body> tag has data-enhance-nav=\"false\" — enhanced navigation is globally disabled. " +
            "Remove this attribute from Components/App.razor once issue #548 is resolved.");
    }

    /// <summary>
    /// Enhanced version of <see cref="ProductDetails_FromProductListLink_RendersCoreFields"/>
    /// that additionally verifies enhanced navigation fired AND that SelectMethod-bound
    /// FormView data (name, description, price) renders correctly during the enhanced load.
    ///
    /// This is the key regression test for #548: the symptom was that FormView/ListView
    /// items were empty when navigated to via enhanced navigation because the HostingPage
    /// cascading parameter was null during the enhanced-navigation component lifecycle.
    ///
    /// SKIPPED: Remove this skip once data-enhance-nav="false" workaround is removed
    /// (i.e., when #548 is fixed).
    /// </summary>
    [Fact(Skip = "TODO(#548): Re-enable when data-enhance-nav='false' is removed — verifies SelectMethod data renders correctly during Blazor enhanced navigation.")]
    public async Task EnhancedNavigation_ProductDetails_SelectMethodDataRendersAfterLinkClick()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/ProductList");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Arm the enhancedload counter before the link click
        await page.EvaluateAsync("""
            () => {
                window.__enhancedLoadCount = 0;
                if (window.Blazor && typeof Blazor.addEventListener === "function") {
                    Blazor.addEventListener("enhancedload", () => window.__enhancedLoadCount++);
                }
            }
            """);

        // Click a product link — this should trigger enhanced navigation, not a full reload
        await page.Locator("a[href*='/Product/']").First.ClickAsync();

        await Assertions.Expect(page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(".*/Product/.*"),
            new PageAssertionsToHaveURLOptions { Timeout = 15000 });

        // Assert enhanced navigation actually fired (not a full page reload)
        var enhancedCount = await page.EvaluateAsync<int>("() => window.__enhancedLoadCount || 0");
        Assert.True(enhancedCount > 0,
            "Expected Blazor enhancedload event to fire when clicking a product link. " +
            "If 0, blazor.web.js may not be active or the link triggered a full-page navigation.");

        // Assert FormView SelectMethod data rendered correctly during enhanced nav
        var heading = page.Locator("h1").First;
        await Assertions.Expect(heading).ToBeVisibleAsync();
        var productName = (await heading.TextContentAsync())?.Trim();
        Assert.False(string.IsNullOrWhiteSpace(productName),
            "ProductDetails FormView heading is empty after enhanced navigation. " +
            "This is the #548 symptom: SelectMethod is not invoked when HostingPage cascading parameter is null.");

        var content = await page.ContentAsync();
        Assert.Contains("Description", content, StringComparison.OrdinalIgnoreCase);
        Assert.True(
            content.Contains("Price", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("$", StringComparison.OrdinalIgnoreCase),
            "ProductDetails price is missing after enhanced navigation — #548 symptom: SelectMethod data is empty.");
    }
}

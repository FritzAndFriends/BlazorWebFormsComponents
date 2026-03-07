using Microsoft.Playwright;

namespace WingtipToys.AcceptanceTests;

/// <summary>
/// Verifies that static assets (CSS, images) are served correctly and that
/// Bootstrap styling is applied. These tests catch the Run 9 class of failures:
/// navbar rendering as a bullet list (missing CSS) and product images returning 404.
/// </summary>
[Collection("Playwright")]
public class StaticAssetTests
{
    private readonly PlaywrightFixture _fixture;

    public StaticAssetTests(PlaywrightFixture fixture) => _fixture = fixture;

    // ---------------------------------------------------------------
    // 1. CSS Files Are Served
    // ---------------------------------------------------------------

    [Fact]
    public async Task HomePage_LoadsAtLeastOneCssFile()
    {
        var page = await _fixture.NewPageAsync();

        var cssResponses = new List<IResponse>();
        page.Response += (_, response) =>
        {
            if (response.Url.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
                response.Headers.TryGetValue("content-type", out var ct) &&
                ct.Contains("text/css", StringComparison.OrdinalIgnoreCase))
            {
                cssResponses.Add(response);
            }
        };

        await page.GotoAsync(TestConfiguration.BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.True(cssResponses.Count > 0,
            "Homepage should load at least one CSS file. No .css responses detected — " +
            "CSS bundle references may be broken.");
    }

    [Fact]
    public async Task CssFiles_ReturnHttp200()
    {
        var page = await _fixture.NewPageAsync();

        var failedCss = new List<string>();
        page.Response += (_, response) =>
        {
            if ((response.Url.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
                 response.Headers.TryGetValue("content-type", out var ct) &&
                 ct.Contains("text/css", StringComparison.OrdinalIgnoreCase)) &&
                response.Status != 200)
            {
                failedCss.Add($"{response.Url} → {response.Status}");
            }
        };

        await page.GotoAsync(TestConfiguration.BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.True(failedCss.Count == 0,
            $"CSS files returned non-200 status codes:\n{string.Join("\n", failedCss)}");
    }

    // ---------------------------------------------------------------
    // 2. No Broken Images on Product List
    // ---------------------------------------------------------------

    [Fact]
    public async Task ProductList_AllImagesLoad()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/ProductList");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var images = page.Locator("img");
        var imageCount = await images.CountAsync();

        Assert.True(imageCount > 0,
            "Product list page should contain at least one <img> element");

        var brokenImages = new List<string>();
        for (var i = 0; i < imageCount; i++)
        {
            var img = images.Nth(i);
            var src = await img.GetAttributeAsync("src") ?? "";
            var naturalWidth = await img.EvaluateAsync<int>("el => el.naturalWidth");

            if (string.IsNullOrWhiteSpace(src) || naturalWidth == 0)
            {
                brokenImages.Add(string.IsNullOrWhiteSpace(src)
                    ? $"Image[{i}]: empty src"
                    : $"Image[{i}]: src='{src}' (naturalWidth=0 → 404 or broken)");
            }
        }

        Assert.True(brokenImages.Count == 0,
            $"Broken images found on ProductList:\n{string.Join("\n", brokenImages)}");
    }

    [Fact]
    public async Task ProductList_ImageRequests_ReturnHttp200()
    {
        var page = await _fixture.NewPageAsync();

        var failedImages = new List<string>();
        page.Response += (_, response) =>
        {
            var url = response.Url;
            if ((url.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                 url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                 url.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                 url.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                 url.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ||
                 url.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)) &&
                response.Status != 200)
            {
                failedImages.Add($"{url} → {response.Status}");
            }
        };

        await page.GotoAsync($"{TestConfiguration.BaseUrl}/ProductList");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.True(failedImages.Count == 0,
            $"Image requests returned non-200 status codes on ProductList:\n{string.Join("\n", failedImages)}");
    }

    // ---------------------------------------------------------------
    // 3. Navbar Has Bootstrap Styling
    // ---------------------------------------------------------------

    [Fact]
    public async Task Navbar_HasBootstrapClasses()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync(TestConfiguration.BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Look for a nav element or div with navbar class
        var navbar = page.Locator(".navbar").First;
        var navbarCount = await navbar.CountAsync();

        Assert.True(navbarCount > 0,
            "Homepage should have an element with class 'navbar'. " +
            "If navbar renders as a plain <ul> bullet list, Bootstrap CSS is not loaded.");
    }

    [Fact]
    public async Task Navbar_HasReasonableHeight()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync(TestConfiguration.BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var navbar = page.Locator(".navbar").First;
        if (await navbar.CountAsync() > 0)
        {
            var box = await navbar.BoundingBoxAsync();
            Assert.NotNull(box);
            Assert.True(box.Height >= 30,
                $"Navbar height is {box.Height}px — expected ≥30px. " +
                "A collapsed/unstyled navbar suggests Bootstrap CSS is missing.");
        }
        else
        {
            // Fallback: check for any nav element with links
            var nav = page.Locator("nav").First;
            var navCount = await nav.CountAsync();
            Assert.True(navCount > 0,
                "No .navbar or <nav> element found on homepage. Navigation structure is missing.");

            var box = await nav.BoundingBoxAsync();
            Assert.NotNull(box);
            Assert.True(box.Height >= 30,
                $"Nav element height is {box.Height}px — expected ≥30px with styling.");
        }
    }

    // ---------------------------------------------------------------
    // 4. Visual Sanity Checks (Screenshot + Element Verification)
    // ---------------------------------------------------------------

    [Fact]
    public async Task HomePage_HasStyledMainContent()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync(TestConfiguration.BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Check for a main content area with non-zero dimensions.
        // WingtipToys uses jumbotron, carousel, or a main container.
        var mainContent = page.Locator(".jumbotron, .carousel, [role='main'], main, .container").First;
        var count = await mainContent.CountAsync();
        Assert.True(count > 0,
            "Homepage should have a main content area (.jumbotron, .carousel, [role='main'], main, or .container)");

        var box = await mainContent.BoundingBoxAsync();
        Assert.NotNull(box);
        Assert.True(box.Width > 100 && box.Height > 50,
            $"Main content area is {box.Width}x{box.Height}px — expected a visible, styled region. " +
            "This may indicate CSS is not loaded.");
    }

    [Fact]
    public async Task HomePage_Screenshot_VerifyLayout()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync(TestConfiguration.BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var screenshot = await page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
        Assert.NotNull(screenshot);
        Assert.True(screenshot.Length > 5000,
            $"Homepage screenshot is only {screenshot.Length} bytes — expected a richly styled page. " +
            "An extremely small screenshot suggests the page is mostly blank or CSS is missing.");
    }

    [Fact]
    public async Task ProductList_Screenshot_VerifyImagesAndLayout()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/ProductList");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Verify product images exist and are visible
        var images = page.Locator("img");
        var imageCount = await images.CountAsync();
        Assert.True(imageCount > 0, "Product list should display product images");

        // Check at least one image has meaningful dimensions (not a broken icon)
        var firstImg = images.First;
        var imgBox = await firstImg.BoundingBoxAsync();
        Assert.NotNull(imgBox);
        Assert.True(imgBox.Width > 20 && imgBox.Height > 20,
            $"First product image is {imgBox.Width}x{imgBox.Height}px — expected a visible image, not a broken icon.");

        // Screenshot for visual record
        var screenshot = await page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
        Assert.NotNull(screenshot);
        Assert.True(screenshot.Length > 5000,
            $"ProductList screenshot is only {screenshot.Length} bytes — page may be missing images or CSS.");
    }

    [Fact]
    public async Task ProductDetails_Screenshot_VerifyImageAndStyling()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/ProductList");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Navigate to first product's detail page
        var firstProduct = page.Locator("a[href*='ProductDetails']").First;
        var linkCount = await firstProduct.CountAsync();
        Assert.True(linkCount > 0, "Product list should have at least one product link");

        await firstProduct.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        Assert.Contains("ProductDetails", page.Url, StringComparison.OrdinalIgnoreCase);

        // Verify the product detail image loads
        var detailImage = page.Locator("img").First;
        if (await detailImage.CountAsync() > 0)
        {
            var naturalWidth = await detailImage.EvaluateAsync<int>("el => el.naturalWidth");
            Assert.True(naturalWidth > 0,
                "Product detail image has naturalWidth=0 — image is broken (404 or wrong path).");

            var imgBox = await detailImage.BoundingBoxAsync();
            Assert.NotNull(imgBox);
            Assert.True(imgBox.Width > 30 && imgBox.Height > 30,
                $"Product detail image is {imgBox.Width}x{imgBox.Height}px — too small, may be a broken icon.");
        }

        // Screenshot for visual record
        var screenshot = await page.ScreenshotAsync(new PageScreenshotOptions { FullPage = true });
        Assert.NotNull(screenshot);
        Assert.True(screenshot.Length > 3000,
            $"ProductDetails screenshot is only {screenshot.Length} bytes — page may be unstyled or missing content.");
    }

    // ---------------------------------------------------------------
    // 5. No Failed Static Asset Requests (catch-all)
    // ---------------------------------------------------------------

    [Fact]
    public async Task HomePage_NoFailed_StaticAssetRequests()
    {
        var page = await _fixture.NewPageAsync();

        var failedAssets = new List<string>();
        page.Response += (_, response) =>
        {
            var url = response.Url;
            var isStaticAsset =
                url.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".js", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".woff", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".woff2", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".ico", StringComparison.OrdinalIgnoreCase);

            if (isStaticAsset && response.Status >= 400)
            {
                failedAssets.Add($"[{response.Status}] {url}");
            }
        };

        await page.GotoAsync(TestConfiguration.BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.True(failedAssets.Count == 0,
            $"Static asset requests failed on homepage:\n{string.Join("\n", failedAssets)}");
    }
}

using Microsoft.Playwright;

namespace WingtipToys.AcceptanceTests;

[Collection("Playwright")]
public class ProductDetailsTests
{
    private readonly PlaywrightFixture _fixture;

    public ProductDetailsTests(PlaywrightFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task ProductDetails_ProductSelected_RendersRealContent()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/ProductList");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var productLinks = page.Locator("a[href*='/Product/']");
        var productCount = await productLinks.CountAsync();
        Assert.True(productCount > 0, "Product list should contain at least one product link.");

        await productLinks.First.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var image = page.Locator("img").First;
        Assert.True(await image.CountAsync() > 0, "Product details page should render a product image.");

        var heading = page.Locator("h1").First;
        Assert.True(await heading.CountAsync() > 0, "Product details page should render a product name heading.");

        var productName = (await heading.TextContentAsync())?.Trim();
        Assert.False(string.IsNullOrWhiteSpace(productName), "Product name heading should contain text.");

        var pageContent = await page.ContentAsync();
        Assert.Contains("Description:", pageContent, StringComparison.OrdinalIgnoreCase);
    }
}

using Microsoft.Playwright;

namespace WingtipToys.AcceptanceTests;

/// <summary>
/// Verifies that every top-level navigation link in the WingtipToys navbar
/// resolves to a page that loads without errors.
/// </summary>
[Collection("Playwright")]
public class NavigationTests
{
    private readonly PlaywrightFixture _fixture;

    public NavigationTests(PlaywrightFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task HomePage_Loads()
    {
        var page = await _fixture.NewPageAsync();
        var response = await page.GotoAsync(TestConfiguration.BaseUrl);

        Assert.NotNull(response);
        Assert.True(response.Ok, $"Home page returned {response.Status}");
        // Verify some content loaded
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var title = await page.TitleAsync();
        Assert.False(string.IsNullOrEmpty(title), "Home page should have a title");
    }

    [Theory]
    [InlineData("About")]
    [InlineData("Contact")]
    [InlineData("ProductList")]
    public async Task NavbarLink_LoadsPage(string pageName)
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync(TestConfiguration.BaseUrl);

        // Click the navbar link whose href ends with the page name
        var link = page.Locator($"a[href='/{pageName}']").First;
        await link.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Page should load without a server error
        Assert.DoesNotContain("Error", await page.TitleAsync(), StringComparison.OrdinalIgnoreCase);
        Assert.True(page.Url.Contains(pageName, StringComparison.OrdinalIgnoreCase),
            $"Expected URL to contain '{pageName}' but was '{page.Url}'");
    }

    [Fact]
    public async Task ShoppingCartLink_LoadsPage()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync(TestConfiguration.BaseUrl);

        var cartLink = page.Locator("a[href='/ShoppingCart']").First;
        await cartLink.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.Contains("ShoppingCart", page.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RegisterLink_LoadsPage()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync(TestConfiguration.BaseUrl);

        var registerLink = page.Locator("a[href='/Account/Register']").First;
        await registerLink.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.Contains("Register", page.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LoginLink_LoadsPage()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync(TestConfiguration.BaseUrl);

        var loginLink = page.Locator("a[href='/Account/Login']").First;
        await loginLink.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.Contains("Login", page.Url, StringComparison.OrdinalIgnoreCase);
    }

}

using Microsoft.Playwright;

namespace ContosoUniversity.AcceptanceTests;

/// <summary>
/// Verifies the ContosoUniversity Home page renders static content correctly.
/// Home.aspx is a simple welcome page with no data controls.
/// </summary>
[Collection("Playwright")]
public class HomePageTests
{
    private readonly PlaywrightFixture _fixture;

    public HomePageTests(PlaywrightFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task HomePage_Loads()
    {
        var page = await _fixture.NewPageAsync();
        var response = await page.GotoAsync($"{TestConfiguration.BaseUrl}/Home.aspx");

        Assert.NotNull(response);
        Assert.True(response.Ok, $"Home page returned HTTP {response.Status}");

        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        var title = await page.TitleAsync();
        Assert.False(string.IsNullOrEmpty(title), "Home page should have a title");
    }

    [Fact]
    public async Task HomePage_HasWelcomeText()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Home.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Home page has a #welcomeHeader element or welcome heading
        var welcomeHeader = page.Locator("#welcomeHeader, h1, h2");
        var count = await welcomeHeader.CountAsync();
        Assert.True(count > 0, "Home page should have a welcome heading element");

        var content = await page.ContentAsync();
        Assert.True(
            content.Contains("Welcome", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("Contoso", StringComparison.OrdinalIgnoreCase),
            "Home page should contain 'Welcome' or 'Contoso' text");
    }

    [Fact]
    public async Task HomePage_HasSiteBranding()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Home.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Master page header (#navUp) should render with site branding
        var header = page.Locator("#navUp, header, .navbar");
        var count = await header.CountAsync();
        Assert.True(count > 0,
            "Home page should have a site header/branding area from the master page");
    }

    [Fact]
    public async Task HomePage_HasFooter()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Home.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var footer = page.Locator("footer");
        var count = await footer.CountAsync();
        Assert.True(count > 0, "Home page should have a <footer> element");
    }
}

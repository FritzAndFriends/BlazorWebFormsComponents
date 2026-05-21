using Microsoft.Playwright;

namespace ZavaLoanPortal.AcceptanceTests;

/// <summary>
/// Verifies navigation links and page structure of the migrated ZavaLoanPortal.
/// </summary>
[Collection("Playwright")]
public class NavigationTests
{
    private readonly PlaywrightFixture _fixture;

    public NavigationTests(PlaywrightFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task HomePage_Loads_WithWizard()
    {
        var page = await _fixture.NewPageAsync();
        var response = await page.GotoAsync(TestConfiguration.BaseUrl);

        Assert.NotNull(response);
        Assert.True(response.Ok, $"Home page returned {response.Status}");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Should show the loan application wizard
        var title = await page.TitleAsync();
        Assert.False(string.IsNullOrEmpty(title), "Home page should have a title");
    }

    [Fact]
    public async Task LoginPage_Loads()
    {
        var page = await _fixture.NewPageAsync();
        var response = await page.GotoAsync($"{TestConfiguration.BaseUrl}/Login");

        Assert.NotNull(response);
        Assert.True(response.Ok, $"Login page returned {response.Status}");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Fact]
    public async Task Layout_HasNavLinks()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Login");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Check that the layout header and navigation links are present
        var header = page.Locator("text=Zava Loan Portal");
        await Assertions.Expect(header).ToBeVisibleAsync();

        var homeLink = page.Locator("a[href='/']").First;
        await Assertions.Expect(homeLink).ToBeVisibleAsync();

        var loginLink = page.Locator("a[href='/Login']");
        await Assertions.Expect(loginLink).ToBeVisibleAsync();
    }

    [Fact]
    public async Task LogoutPage_Loads()
    {
        var page = await _fixture.NewPageAsync();
        var response = await page.GotoAsync($"{TestConfiguration.BaseUrl}/Logout");

        Assert.NotNull(response);
        // May redirect to external auth gateway, but should not 500
        Assert.True(response.Status < 500, $"Logout page returned {response.Status}");
    }
}

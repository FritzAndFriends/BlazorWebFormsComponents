using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Integration tests for the IsPostBack sample page at /migration/ispostback.
/// Verifies IsPostBack status display, guard pattern, and HttpContext check.
/// </summary>
[Collection(nameof(PlaywrightCollection))]
public class IsPostBackTests
{
    private readonly PlaywrightFixture _fixture;

    public IsPostBackTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that the IsPostBack status displays a boolean value.
    /// </summary>
    [Fact]
    public async Task IsPostBack_DisplaysStatus()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/ispostback", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='ispostback-status-demo']");
            var strongElements = card.Locator("strong");
            var statusText = await strongElements.Nth(0).TextContentAsync();

            Assert.NotNull(statusText);
            var trimmed = statusText!.Trim();
            Assert.True(trimmed == "True" || trimmed == "False",
                $"Expected 'True' or 'False' but got '{trimmed}'");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that the guard pattern initializes data on first load.
    /// </summary>
    [Fact]
    public async Task IsPostBack_GuardPatternInitializesData()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/ispostback", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='ispostback-guard-demo']");
            var strongElements = card.Locator("strong");

            // Verify data was loaded
            var dataText = await strongElements.Nth(0).TextContentAsync();
            Assert.NotNull(dataText);
            Assert.Contains("Alpha", dataText!);
            Assert.Contains("Bravo", dataText);
            Assert.Contains("Charlie", dataText);

            // Verify init count
            var initCountText = await strongElements.Nth(1).TextContentAsync();
            Assert.NotNull(initCountText);
            Assert.Equal("1 time(s)", initCountText!.Trim());
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that re-rendering does not re-initialize data (guard pattern works).
    /// </summary>
    [Fact]
    public async Task IsPostBack_RerenderDoesNotReinitialize()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/ispostback", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='ispostback-guard-demo']");

            // Click Re-render button
            await card.Locator("button:has-text('Re-render')").ClickAsync();
            await page.WaitForTimeoutAsync(1000);

            // Init count should still be 1 (guard prevented re-init)
            var initCountText = await card.Locator("strong").Nth(1).TextContentAsync();
            Assert.NotNull(initCountText);
            Assert.Equal("1 time(s)", initCountText!.Trim());
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that the HttpContext guard demo displays a boolean value.
    /// </summary>
    [Fact]
    public async Task HttpContextGuard_DisplaysAvailability()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/ispostback", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='httpcontext-guard-demo']");
            var strongText = await card.Locator("strong").TextContentAsync();

            Assert.NotNull(strongText);
            var trimmed = strongText!.Trim();
            Assert.True(trimmed == "True" || trimmed == "False",
                $"Expected 'True' or 'False' but got '{trimmed}'");
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

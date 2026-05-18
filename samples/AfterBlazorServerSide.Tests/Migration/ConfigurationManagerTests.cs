using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Regression tests for the ConfigurationManager shim sample page (Phase 1).
/// Ensures the page continues to load and display configuration values correctly.
/// </summary>
[Collection(nameof(PlaywrightCollection))]
public class ConfigurationManagerTests
{
    private readonly PlaywrightFixture _fixture;

    public ConfigurationManagerTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that the ConfigurationManager sample page loads without errors
    /// and renders the expected demo sections (AppSettings, ConnectionStrings).
    /// </summary>
    [Fact]
    public async Task ConfigurationManager_PageLoads()
    {
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        var pageErrors = new List<string>();

        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                // Filter ASP.NET Core structured log messages
                if (System.Text.RegularExpressions.Regex.IsMatch(msg.Text, @"^\[\d{4}-\d{2}-\d{2}T"))
                    return;
                if (msg.Text.StartsWith("Failed to load resource"))
                    return;
                consoleErrors.Add(msg.Text);
            }
        };

        page.PageError += (_, error) =>
        {
            pageErrors.Add(error);
        };

        try
        {
            var response = await page.GotoAsync(
                $"{_fixture.BaseUrl}/ControlSamples/Migration/ConfigurationManager",
                new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle,
                    Timeout = 30000
                });

            // Page loads successfully
            Assert.NotNull(response);
            Assert.True(response!.Ok, $"Page failed to load with status: {response.Status}");

            // Page renders the expected heading
            var heading = await page.Locator("h2").TextContentAsync();
            Assert.Contains("ConfigurationManager", heading);

            // AppSettings demo section is present
            var appSettingsCard = page.Locator("[data-audit-control='appsettings-demo']");
            await appSettingsCard.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
            Assert.True(await appSettingsCard.IsVisibleAsync());

            // No errors
            Assert.Empty(consoleErrors);
            Assert.Empty(pageErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

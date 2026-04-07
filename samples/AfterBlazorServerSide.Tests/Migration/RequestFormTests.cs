using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Integration tests for the Request.Form shim sample page at /migration/request-form.
/// Verifies page load, demo section rendering, and graceful degradation in interactive mode.
/// </summary>
[Collection(nameof(PlaywrightCollection))]
public class RequestFormTests
{
    private readonly PlaywrightFixture _fixture;

    public RequestFormTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Smoke test — verifies the page loads without errors and displays the expected heading.
    /// </summary>
    [Fact]
    public async Task RequestForm_PageLoads_Successfully()
    {
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();

        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(msg.Text, @"^\[\d{4}-\d{2}-\d{2}T"))
                    return;
                if (msg.Text.StartsWith("Failed to load resource"))
                    return;
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            var response = await page.GotoAsync($"{_fixture.BaseUrl}/migration/request-form", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            Assert.NotNull(response);
            Assert.True(response.Ok, $"Page failed to load with status: {response.Status}");

            // Verify the page heading renders
            var heading = page.Locator("h2").Filter(new() { HasTextString = "Request.Form Migration" });
            await Assertions.Expect(heading).ToBeVisibleAsync();

            // Verify page title
            await Assertions.Expect(page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("Request\\.Form"));

            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Render test — verifies all four data-audit-control demo cards are present.
    /// </summary>
    [Fact]
    public async Task RequestForm_DemoSections_Render()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/request-form", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Wait for Blazor interactive circuit
            await page.WaitForFunctionAsync("typeof window.__doPostBack === 'function'",
                null, new PageWaitForFunctionOptions { Timeout = 10000 });

            // Verify all four demo cards render
            var basicDemo = page.Locator("[data-audit-control='form-basic-demo']");
            await Assertions.Expect(basicDemo).ToBeVisibleAsync();

            var getValuesDemo = page.Locator("[data-audit-control='form-getvalues-demo']");
            await Assertions.Expect(getValuesDemo).ToBeVisibleAsync();

            var metadataDemo = page.Locator("[data-audit-control='form-metadata-demo']");
            await Assertions.Expect(metadataDemo).ToBeVisibleAsync();

            var migrationGuidance = page.Locator("[data-audit-control='form-migration-guidance']");
            await Assertions.Expect(migrationGuidance).ToBeVisibleAsync();

            // Verify card titles
            await Assertions.Expect(basicDemo.Locator(".card-title")).ToContainTextAsync("Basic Field Access");
            await Assertions.Expect(getValuesDemo.Locator(".card-title")).ToContainTextAsync("GetValues");
            await Assertions.Expect(metadataDemo.Locator(".card-title")).ToContainTextAsync("Form Metadata");
            await Assertions.Expect(migrationGuidance.Locator(".card-title")).ToContainTextAsync("Migration Path Summary");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Interactive mode test — verifies Request.Form returns empty/null values gracefully
    /// when running in Blazor Server interactive mode (no HTTP POST available).
    /// </summary>
    [Fact]
    public async Task RequestForm_InteractiveMode_ShowsGracefulDegradation()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/request-form", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Wait for Blazor interactive circuit
            await page.WaitForFunctionAsync("typeof window.__doPostBack === 'function'",
                null, new PageWaitForFunctionOptions { Timeout = 10000 });

            // Basic field access card — should show null indicators
            var basicDemo = page.Locator("[data-audit-control='form-basic-demo']");
            await Assertions.Expect(basicDemo).ToContainTextAsync("(null");

            // GetValues card — should show null indicator
            var getValuesDemo = page.Locator("[data-audit-control='form-getvalues-demo']");
            await Assertions.Expect(getValuesDemo).ToContainTextAsync("(null");

            // Metadata card — Count should be 0, AllKeys should be empty, ContainsKey should be False
            var metadataDemo = page.Locator("[data-audit-control='form-metadata-demo']");
            var metadataTable = metadataDemo.Locator("table");

            // Request.Form.Count should show 0
            var countRow = metadataTable.Locator("tr").Filter(new() { HasTextString = "Request.Form.Count" });
            await Assertions.Expect(countRow.Locator("strong")).ToContainTextAsync("0");

            // Request.Form.AllKeys should show "(empty — no form fields)"
            var allKeysRow = metadataTable.Locator("tr").Filter(new() { HasTextString = "Request.Form.AllKeys" });
            await Assertions.Expect(allKeysRow).ToContainTextAsync("(empty");

            // Request.Form.ContainsKey("username") should show False
            var containsKeyRow = metadataTable.Locator("tr").Filter(new() { HasTextString = "ContainsKey" });
            await Assertions.Expect(containsKeyRow.Locator("strong")).ToContainTextAsync("False");
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

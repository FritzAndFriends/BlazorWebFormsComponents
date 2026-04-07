using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Integration tests for the Request.Form shim sample page at /migration/request-form.
/// This page uses SSR mode via [ExcludeFromInteractiveRouting] so traditional form POSTs work.
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
    /// Smoke test — verifies the SSR page loads without errors and displays the expected heading.
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
    /// Render test — verifies the form input card and migration guidance render on GET.
    /// No results card should appear before form submission.
    /// </summary>
    [Fact]
    public async Task RequestForm_InitialLoad_ShowsFormWithoutResults()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/request-form", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Form input card should be visible
            var formInput = page.Locator("[data-audit-control='form-input']");
            await Assertions.Expect(formInput).ToBeVisibleAsync();

            // Migration guidance card should be visible
            var migrationGuidance = page.Locator("[data-audit-control='form-migration-guidance']");
            await Assertions.Expect(migrationGuidance).ToBeVisibleAsync();
            await Assertions.Expect(migrationGuidance.Locator(".card-title")).ToContainTextAsync("Migration Path Summary");

            // Results card should NOT be visible before form submission
            var results = page.Locator("[data-audit-control='form-results']");
            await Assertions.Expect(results).ToHaveCountAsync(0);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// End-to-end test — fills the form, submits via HTTP POST, and verifies
    /// Request.Form values are read and displayed correctly.
    /// </summary>
    [Fact]
    public async Task RequestForm_FormPost_ShowsSubmittedValues()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/request-form", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Fill in the form fields
            await page.FillAsync("#username", "TestUser");
            await page.FillAsync("#email", "test@example.com");

            // Check "Red" and "Blue" color checkboxes
            await page.CheckAsync("#colorRed");
            await page.CheckAsync("#colorBlue");

            // Submit the form — this triggers a real HTTP POST (SSR page)
            await page.ClickAsync("button[type='submit']");

            // Wait for the page to reload with POST results
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Results card should now be visible
            var results = page.Locator("[data-audit-control='form-results']");
            await Assertions.Expect(results).ToBeVisibleAsync();

            // Verify submitted values appear in the results table
            var resultsTable = results.Locator("table");

            // Username
            var usernameRow = resultsTable.Locator("tr").Filter(new() { HasTextString = "Request.Form[\"username\"]" });
            await Assertions.Expect(usernameRow.Locator("strong")).ToContainTextAsync("TestUser");

            // Email
            var emailRow = resultsTable.Locator("tr").Filter(new() { HasTextString = "Request.Form[\"email\"]" });
            await Assertions.Expect(emailRow.Locator("strong")).ToContainTextAsync("test@example.com");

            // GetValues — should contain Red and Blue
            var colorsRow = resultsTable.Locator("tr").Filter(new() { HasTextString = "GetValues" });
            await Assertions.Expect(colorsRow.Locator("strong")).ToContainTextAsync("Red");
            await Assertions.Expect(colorsRow.Locator("strong")).ToContainTextAsync("Blue");

            // Count should be > 0 (at least username, email, colors, plus antiforgery token)
            var countRow = resultsTable.Locator("tr").Filter(new() { HasTextString = "Request.Form.Count" });
            var countText = await countRow.Locator("strong").TextContentAsync();
            Assert.True(int.Parse(countText!) > 0, "Form count should be greater than 0 after POST");

            // ContainsKey("username") should be True
            var containsRow = resultsTable.Locator("tr").Filter(new() { HasTextString = "ContainsKey" });
            await Assertions.Expect(containsRow.Locator("strong")).ToContainTextAsync("True");

            // Form fields should be pre-filled with submitted values
            var usernameInput = page.Locator("#username");
            await Assertions.Expect(usernameInput).ToHaveValueAsync("TestUser");
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

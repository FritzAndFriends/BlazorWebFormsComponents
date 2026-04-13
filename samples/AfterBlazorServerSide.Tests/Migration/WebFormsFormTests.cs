using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Integration tests for the WebFormsForm interactive demo page at /migration/webforms-form.
/// This page uses INTERACTIVE rendering (SignalR/JS interop), not SSR POST.
/// </summary>
[Collection(nameof(PlaywrightCollection))]
public class WebFormsFormTests
{
    private readonly PlaywrightFixture _fixture;

    public WebFormsFormTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Smoke test — verifies the page loads without errors and displays the expected heading.
    /// </summary>
    [Fact]
    public async Task WebFormsForm_PageLoads_Successfully()
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
            var response = await page.GotoAsync($"{_fixture.BaseUrl}/migration/webforms-form", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            Assert.NotNull(response);
            Assert.True(response.Ok, $"Page failed to load with status: {response.Status}");

            // Verify a heading renders (page should have WebFormsForm or similar heading)
            var heading = page.Locator("h2, h1").First;
            await Assertions.Expect(heading).ToBeVisibleAsync();

            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Render test — verifies the form renders with input fields and no results on initial load.
    /// </summary>
    [Fact]
    public async Task WebFormsForm_InitialLoad_ShowsFormWithoutResults()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/webforms-form", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Form inputs should be visible (name and email)
            var nameInput = page.Locator("input[name='username'], input#username").First;
            await Assertions.Expect(nameInput).ToBeVisibleAsync();

            var emailInput = page.Locator("input[name='email'], input#email").First;
            await Assertions.Expect(emailInput).ToBeVisibleAsync();

            // Submit button should be visible
            var submitButton = page.Locator("button[type='submit'], input[type='submit']").First;
            await Assertions.Expect(submitButton).ToBeVisibleAsync();

            // Results section should NOT be visible before submission
            var results = page.Locator("[data-audit-control='webforms-form-results']").First;
            await Assertions.Expect(results).Not.ToBeVisibleAsync(new() { Timeout = 3000 });
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Interaction test — fills name, email, checks a color checkbox, submits via interactive
    /// Blazor Server (SignalR), and verifies Request.Form values appear in the results section.
    /// The page re-renders in place (no navigation) because it's interactive mode.
    /// </summary>
    [Fact]
    public async Task WebFormsForm_SubmitForm_ShowsRequestFormValues()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/webforms-form", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Wait for Blazor Server interactive circuit — the WebFormsForm component
            // renders data-interactive="true" only after the circuit is established.
            // Without this wait, the prerendered form lacks @onsubmit:preventDefault
            // and clicking submit causes a full-page POST that resets component state.
            await page.WaitForSelectorAsync("form[data-interactive]", new() { Timeout = 10000 });

            // Fill in the name field
            var nameInput = page.Locator("input[name='username'], input#username").First;
            await nameInput.FillAsync("TestUser");
            await page.Keyboard.PressAsync("Tab");

            // Fill in the email field
            var emailInput = page.Locator("input[name='email'], input#email").First;
            await emailInput.FillAsync("test@example.com");
            await page.Keyboard.PressAsync("Tab");

            // Check a color checkbox (look for Red or first available color)
            var colorCheckbox = page.Locator("input[type='checkbox']").First;
            await colorCheckbox.CheckAsync();

            // Click the submit button — interactive mode re-renders in place
            var submitButton = page.Locator("button[type='submit'], input[type='submit']").First;
            await submitButton.ClickAsync();

            // Wait for re-render — the results section should appear after SignalR round-trip
            var resultsLocator = page.Locator("[data-audit-control='webforms-form-results']").First;
            await resultsLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

            // Verify submitted name value appears in results
            var nameResult = page.Locator("text=TestUser");
            await Assertions.Expect(nameResult.First).ToBeVisibleAsync(new() { Timeout = 5000 });

            // Verify submitted email value appears in results
            var emailResult = page.Locator("text=test@example.com");
            await Assertions.Expect(emailResult.First).ToBeVisibleAsync(new() { Timeout = 5000 });
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that the page stays on the same URL after submit (interactive mode, no navigation).
    /// </summary>
    [Fact]
    public async Task WebFormsForm_SubmitForm_StaysOnSamePage()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/webforms-form", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Wait for Blazor Server interactive circuit
            await page.WaitForSelectorAsync("form[data-interactive]", new() { Timeout = 10000 });

            // Fill a field and submit
            var nameInput = page.Locator("input[name='username'], input#username").First;
            await nameInput.FillAsync("NavigationTest");
            await page.Keyboard.PressAsync("Tab");

            var submitButton = page.Locator("button[type='submit'], input[type='submit']").First;
            await submitButton.ClickAsync();

            // Wait for re-render
            await page.WaitForTimeoutAsync(2000);

            // URL should still be the same page (interactive mode, no POST redirect)
            await Assertions.Expect(page).ToHaveURLAsync(
                new System.Text.RegularExpressions.Regex(@"/migration/webforms-form"),
                new() { Timeout = 5000 });
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

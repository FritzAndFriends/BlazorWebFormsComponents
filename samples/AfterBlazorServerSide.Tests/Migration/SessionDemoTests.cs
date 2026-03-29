using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Integration tests for the SessionShim (GAP-04) sample page at /migration/session.
/// Verifies that the Web Forms-compatible Session["key"] pattern works end-to-end
/// in an interactive Blazor Server context.
/// </summary>
[Collection(nameof(PlaywrightCollection))]
public class SessionDemoTests
{
    private readonly PlaywrightFixture _fixture;

    public SessionDemoTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that storing a value via the Session["key"] shim displays the value
    /// in the live demo output area.
    /// </summary>
    [Fact]
    public async Task Session_SetAndGetValue()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/session", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

            // Wait for the interactive card to render
            await page.WaitForSelectorAsync("[data-audit-control='session-setget-demo']",
                new PageWaitForSelectorOptions { Timeout = 10000 });

            // Verify initial state shows "(empty)"
            var setGetCard = page.Locator("[data-audit-control='session-setget-demo']");
            var displayStrong = setGetCard.Locator("strong");
            var initialText = await displayStrong.TextContentAsync();
            Assert.Contains("(empty)", initialText);

            // Type a value into the input and blur to trigger @bind
            var input = setGetCard.Locator("input[type='text']");
            await input.FillAsync("TestUser");
            await input.PressAsync("Tab");

            // Click the Store button
            await setGetCard.Locator("button:has-text('Store in Session')").ClickAsync();
            await page.WaitForTimeoutAsync(500);

            // Verify the stored value appears
            var storedText = await displayStrong.TextContentAsync();
            Assert.Contains("TestUser", storedText);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that storing multiple values increments Session.Count correctly.
    /// Stores a value and clicks increment counter to create 2+ session entries,
    /// then checks the count display.
    /// </summary>
    [Fact]
    public async Task Session_CountIncrementsAfterStore()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/session", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

            await page.WaitForSelectorAsync("[data-audit-control='session-count-demo']",
                new PageWaitForSelectorOptions { Timeout = 10000 });

            // Store a string value (creates "UserName" key)
            var setGetCard = page.Locator("[data-audit-control='session-setget-demo']");
            var input = setGetCard.Locator("input[type='text']");
            await input.FillAsync("Alice");
            await input.PressAsync("Tab");
            await setGetCard.Locator("button:has-text('Store in Session')").ClickAsync();
            await page.WaitForTimeoutAsync(500);

            // Increment the typed counter (creates "ClickCount" key)
            await page.Locator("[data-audit-control='session-typesafe-demo'] button:has-text('Increment Counter')").ClickAsync();
            await page.WaitForTimeoutAsync(500);

            // Now there should be at least 2 items: "UserName" and "ClickCount"
            var countCard = page.Locator("[data-audit-control='session-count-demo']");
            var countText = await countCard.Locator("strong").TextContentAsync();
            Assert.NotNull(countText);

            var count = int.Parse(countText!.Trim());
            Assert.True(count >= 2, $"Expected at least 2 session items, but got {count}");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that Session.Clear() removes all session values and resets the count to 0.
    /// </summary>
    [Fact]
    public async Task Session_ClearRemovesAllValues()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/session", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

            await page.WaitForSelectorAsync("[data-audit-control='session-clear-demo']",
                new PageWaitForSelectorOptions { Timeout = 10000 });

            // Store a value first
            var setGetCard = page.Locator("[data-audit-control='session-setget-demo']");
            var input = setGetCard.Locator("input[type='text']");
            await input.FillAsync("ToClear");
            await input.PressAsync("Tab");
            await setGetCard.Locator("button:has-text('Store in Session')").ClickAsync();
            await page.WaitForTimeoutAsync(500);

            // Verify something was stored
            var countBefore = await page.Locator("[data-audit-control='session-count-demo'] strong").TextContentAsync();
            Assert.NotNull(countBefore);
            Assert.True(int.Parse(countBefore!.Trim()) >= 1, "Expected at least 1 session item before clear");

            // Click Clear
            await page.Locator("[data-audit-control='session-clear-demo'] button:has-text('Clear All Session Values')").ClickAsync();
            await page.WaitForTimeoutAsync(500);

            // Verify count is 0
            var countAfter = await page.Locator("[data-audit-control='session-clear-demo'] strong").TextContentAsync();
            Assert.NotNull(countAfter);
            Assert.Equal("0", countAfter!.Trim());

            // Verify the stored value display shows "(empty)" again
            var displayText = await page.Locator("[data-audit-control='session-setget-demo'] strong").TextContentAsync();
            Assert.Contains("(empty)", displayText);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that the typed counter (Session.Get&lt;int&gt;) increments correctly
    /// when the Increment Counter button is clicked multiple times.
    /// </summary>
    [Fact]
    public async Task Session_TypedCounter()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/session", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

            var counterCard = page.Locator("[data-audit-control='session-typesafe-demo']");
            await counterCard.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });

            var counterDisplay = counterCard.Locator("strong");
            var incrementButton = counterCard.Locator("button:has-text('Increment Counter')");

            // Read the initial counter value
            var initialText = await counterDisplay.TextContentAsync();
            var initialValue = int.Parse(initialText!.Trim());

            // Click increment 3 times
            for (var i = 0; i < 3; i++)
            {
                await incrementButton.ClickAsync();
                await page.WaitForTimeoutAsync(300);
            }

            // Verify the counter increased by 3
            var finalText = await counterDisplay.TextContentAsync();
            var finalValue = int.Parse(finalText!.Trim());
            Assert.Equal(initialValue + 3, finalValue);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that session values persist across navigation in interactive Blazor Server mode.
    /// Stores a value, navigates away to the home page, then navigates back and checks
    /// if the value is still present.
    /// </summary>
    [Fact]
    public async Task Session_PersistsAcrossNavigation()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            // Navigate to session page and store a value
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/session", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

            await page.WaitForSelectorAsync("[data-audit-control='session-setget-demo']",
                new PageWaitForSelectorOptions { Timeout = 10000 });

            var setGetCard = page.Locator("[data-audit-control='session-setget-demo']");
            var input = setGetCard.Locator("input[type='text']");
            await input.FillAsync("PersistMe");
            await input.PressAsync("Tab");
            await setGetCard.Locator("button:has-text('Store in Session')").ClickAsync();
            await page.WaitForTimeoutAsync(500);

            // Confirm value was stored
            var storedText = await setGetCard.Locator("strong").TextContentAsync();
            Assert.Contains("PersistMe", storedText);

            // Navigate away to home page
            await page.GotoAsync(_fixture.BaseUrl, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

            // Navigate back to session page
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/session", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30000
            });

            await page.WaitForSelectorAsync("[data-audit-control='session-setget-demo']",
                new PageWaitForSelectorOptions { Timeout = 10000 });

            // Verify the value persists
            var persistedText = await page.Locator("[data-audit-control='session-setget-demo'] strong").TextContentAsync();
            Assert.Contains("PersistMe", persistedText);
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

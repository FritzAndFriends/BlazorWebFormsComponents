using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Integration tests for the CacheShim sample page at /migration/cache.
/// Verifies Cache["key"] set/get, typed access, removal, and expiration demos.
/// </summary>
[Collection(nameof(PlaywrightCollection))]
public class CacheDemoTests
{
    private readonly PlaywrightFixture _fixture;

    public CacheDemoTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that storing a value via Cache["key"] and retrieving it works.
    /// </summary>
    [Fact]
    public async Task Cache_SetAndGetValue()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/cache", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='cache-setget-demo']");

            // Fill key and value
            var inputs = card.Locator("input[type='text']");
            var keyInput = inputs.Nth(0);
            var valueInput = inputs.Nth(1);

            await keyInput.FillAsync("TestKey");
            await keyInput.PressAsync("Tab");
            await valueInput.FillAsync("TestValue");
            await valueInput.PressAsync("Tab");

            // Store in cache
            await card.Locator("button:has-text('Store in Cache')").ClickAsync();
            await page.WaitForTimeoutAsync(1000);

            // Get from cache
            await card.Locator("button:has-text('Get from Cache')").ClickAsync();
            await page.WaitForTimeoutAsync(1000);

            var output = await card.Locator("strong").TextContentAsync();
            Assert.NotNull(output);
            Assert.Contains("TestValue", output!);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that the typed counter increments correctly using Cache.Get&lt;int&gt;().
    /// </summary>
    [Fact]
    public async Task Cache_TypedCounterIncrements()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/cache", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='cache-typed-demo']");
            await card.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });

            var counterDisplay = card.Locator("strong");
            var incrementButton = card.Locator("button:has-text('Increment Counter')");

            // Read initial value
            var initialText = await counterDisplay.TextContentAsync();
            var initialValue = int.Parse(initialText!.Trim());

            // Click increment 3 times
            for (var i = 0; i < 3; i++)
            {
                await incrementButton.ClickAsync();
                await page.WaitForTimeoutAsync(500);
            }

            // Verify counter increased by 3
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
    /// Verifies that Cache.Remove() evicts an item and subsequent checks return null.
    /// </summary>
    [Fact]
    public async Task Cache_RemoveEvictsItem()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/cache", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='cache-remove-demo']");

            // Store an item
            await card.Locator("button:has-text('Store')").ClickAsync();
            await page.WaitForTimeoutAsync(1000);

            var statusAfterStore = await card.Locator("strong").TextContentAsync();
            Assert.Contains("Stored", statusAfterStore!);

            // Remove the item
            await card.Locator("button:has-text('Remove')").ClickAsync();
            await page.WaitForTimeoutAsync(1000);

            var statusAfterRemove = await card.Locator("strong").TextContentAsync();
            Assert.Contains("Removed", statusAfterRemove!);

            // Check — should be not found
            await card.Locator("button:has-text('Check')").ClickAsync();
            await page.WaitForTimeoutAsync(1000);

            var statusAfterCheck = await card.Locator("strong").TextContentAsync();
            Assert.Contains("Not found", statusAfterCheck!);
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Integration tests for the PostBack demo page at /postback-demo.
/// Verifies GetPostBackEventReference, GetPostBackClientHyperlink,
/// and ScriptManager.GetCurrent startup script registration.
/// </summary>
[Collection(nameof(PlaywrightCollection))]
public class PostBackTests
{
    private readonly PlaywrightFixture _fixture;

    public PostBackTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that clicking the postback button triggers a __doPostBack call
    /// and the result area displays the event target and argument.
    /// </summary>
    [Fact]
    public async Task PostBack_Button_TriggersPostBackEvent()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/PostBackDemo", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Wait for __doPostBack to be bootstrapped by OnAfterRenderAsync
            await page.WaitForFunctionAsync("typeof window.__doPostBack === 'function'",
                null, new PageWaitForFunctionOptions { Timeout = 10000 });

            var button = page.Locator("#postback-button");
            await button.ClickAsync();

            // Wait for Blazor to process the postback and re-render
            var result = page.Locator("#postback-result");
            await Assertions.Expect(result).ToContainTextAsync("PostBack received!",
                new LocatorAssertionsToContainTextOptions { Timeout = 10000 });
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that clicking the postback hyperlink (javascript:__doPostBack)
    /// triggers a postback and the hyperlink result area is updated.
    /// </summary>
    [Fact]
    public async Task PostBackHyperlink_Click_TriggersPostBackEvent()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/PostBackDemo", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Wait for __doPostBack to be bootstrapped by OnAfterRenderAsync
            await page.WaitForFunctionAsync("typeof window.__doPostBack === 'function'",
                null, new PageWaitForFunctionOptions { Timeout = 10000 });

            var link = page.Locator("#postback-link");
            await link.ClickAsync();

            // Wait for Blazor to process the postback and re-render
            var result = page.Locator("#hyperlink-result");
            await Assertions.Expect(result).ToContainTextAsync("Hyperlink PostBack!",
                new LocatorAssertionsToContainTextOptions { Timeout = 10000 });
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that ScriptManager.GetCurrent registers a startup script
    /// that populates the target div on page load.
    /// </summary>
    [Fact]
    public async Task ScriptManager_GetCurrent_RegistersStartupScript()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/PostBackDemo", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Wait for the startup script to execute and populate the target
            var target = page.Locator("#scriptmanager-target");
            await Assertions.Expect(target).Not.ToHaveTextAsync(string.Empty,
                new LocatorAssertionsToHaveTextOptions { Timeout = 10000 });

            var targetText = await target.TextContentAsync();
            Assert.NotNull(targetText);
            Assert.False(string.IsNullOrWhiteSpace(targetText),
                "ScriptManager target should have text content after startup script runs");
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

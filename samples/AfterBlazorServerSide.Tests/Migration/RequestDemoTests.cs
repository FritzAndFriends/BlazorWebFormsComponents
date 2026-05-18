using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Integration tests for the RequestShim sample page at /migration/request.
/// Verifies QueryString parsing, URL display, and Cookies graceful degradation.
/// </summary>
[Collection(nameof(PlaywrightCollection))]
public class RequestDemoTests
{
    private readonly PlaywrightFixture _fixture;

    public RequestDemoTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that Request.Url displays the current URL on the page.
    /// </summary>
    [Fact]
    public async Task Request_UrlDisplaysCurrentUrl()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/request", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='url-demo']");
            var urlText = await card.Locator("strong").TextContentAsync();

            Assert.NotNull(urlText);
            Assert.Contains("/migration/request", urlText!);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that Request.QueryString parses query parameters from the URL.
    /// </summary>
    [Fact]
    public async Task Request_QueryStringParsesParameters()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/request?name=TestUser&id=99", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='querystring-demo']");
            var strongElements = card.Locator("strong");

            var nameText = await strongElements.Nth(0).TextContentAsync();
            var idText = await strongElements.Nth(1).TextContentAsync();

            Assert.Contains("TestUser", nameText!);
            Assert.Contains("99", idText!);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that the Cookies demo renders and shows the IsHttpContextAvailable status.
    /// </summary>
    [Fact]
    public async Task Request_CookiesDemoRendersGracefully()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/request", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='cookies-demo']");
            var strongText = await card.Locator("strong").TextContentAsync();

            // Should display either True or False — just verify it renders
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

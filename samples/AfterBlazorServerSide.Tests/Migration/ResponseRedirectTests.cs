using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Integration tests for the ResponseShim sample page at /migration/response-redirect.
/// Verifies redirect navigation, tilde/aspx stripping, and ResolveUrl output.
/// </summary>
[Collection(nameof(PlaywrightCollection))]
public class ResponseRedirectTests
{
    private readonly PlaywrightFixture _fixture;

    public ResponseRedirectTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that clicking the basic redirect button navigates to the session page.
    /// </summary>
    [Fact]
    public async Task Redirect_NavigatesToSessionPage()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/response-redirect", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Wait for Blazor interactive circuit — OnAfterRenderAsync bootstraps __doPostBack
            await page.WaitForFunctionAsync("typeof window.__doPostBack === 'function'",
                null, new PageWaitForFunctionOptions { Timeout = 10000 });

            var card = page.Locator("[data-audit-control='redirect-basic-demo']");
            var button = card.Locator("button:has-text('Response.Redirect')");

            await button.ClickAsync();

            // forceLoad: true triggers full page navigation
            await page.WaitForURLAsync("**/migration/session", new PageWaitForURLOptions
            {
                WaitUntil = WaitUntilState.Load,
                Timeout = 30000
            });

            Assert.Contains("/migration/session", page.Url);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that the tilde stripping demo shows the cleaned URL.
    /// </summary>
    [Fact]
    public async Task Redirect_TildeStrippingShowsCleanUrl()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/response-redirect", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='redirect-tilde-demo']");
            var strippedText = await card.Locator("strong").TextContentAsync();

            Assert.NotNull(strippedText);
            Assert.Equal("/migration/session", strippedText!.Trim());
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that ResolveUrl strips ~/ and .aspx to produce clean URLs.
    /// </summary>
    [Fact]
    public async Task ResolveUrl_ProducesCleanUrls()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/response-redirect", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='resolveurl-strip-demo']");
            var strongElements = card.Locator("strong");

            var productsUrl = await strongElements.Nth(0).TextContentAsync();
            var sessionUrl = await strongElements.Nth(1).TextContentAsync();
            var logoUrl = await strongElements.Nth(2).TextContentAsync();

            Assert.Equal("/Products", productsUrl!.Trim());
            Assert.Equal("/migration/session", sessionUrl!.Trim());
            Assert.Equal("/images/logo.png", logoUrl!.Trim());
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Integration tests for the ServerShim sample page at /migration/server-mappath.
/// Verifies MapPath, HtmlEncode, UrlEncode, and ResolveUrl demos.
/// </summary>
[Collection(nameof(PlaywrightCollection))]
public class ServerMapPathTests
{
    private readonly PlaywrightFixture _fixture;

    public ServerMapPathTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that Server.MapPath("~/images") resolves and displays a non-empty path.
    /// </summary>
    [Fact]
    public async Task MapPath_DisplaysResolvedPath()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/server-mappath", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var mapPathCard = page.Locator("[data-audit-control='mappath-demo']");
            var pathText = await mapPathCard.Locator("strong").TextContentAsync();

            Assert.NotNull(pathText);
            Assert.True(pathText!.Trim().Length > 0, "MapPath should return a non-empty path");
            Assert.Contains("images", pathText, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that Server.HtmlEncode encodes HTML characters correctly.
    /// </summary>
    [Fact]
    public async Task HtmlEncode_EncodesInput()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/server-mappath", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='htmlencode-demo']");
            var input = card.Locator("input[type='text']");
            await input.FillAsync("<b>Hello</b>");
            await input.PressAsync("Tab");

            await card.Locator("button:has-text('HtmlEncode')").ClickAsync();
            await page.WaitForTimeoutAsync(1000);

            var output = await card.Locator("strong").TextContentAsync();
            Assert.NotNull(output);
            Assert.Contains("&lt;b&gt;", output!);
            Assert.Contains("&lt;/b&gt;", output);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that Server.UrlEncode encodes spaces and special characters.
    /// </summary>
    [Fact]
    public async Task UrlEncode_EncodesInput()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/server-mappath", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='urlencode-demo']");
            var input = card.Locator("input[type='text']");
            await input.FillAsync("hello world & more");
            await input.PressAsync("Tab");

            await card.Locator("button:has-text('UrlEncode')").ClickAsync();
            await page.WaitForTimeoutAsync(1000);

            var output = await card.Locator("strong").TextContentAsync();
            Assert.NotNull(output);
            // URL encoding replaces spaces with + and & with %26
            Assert.Contains("+", output!);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    /// <summary>
    /// Verifies that ResolveUrl("~/Products.aspx") strips ~/ and .aspx to produce /Products.
    /// </summary>
    [Fact]
    public async Task ResolveUrl_StripsAspxAndTilde()
    {
        var page = await _fixture.NewPageAsync();

        try
        {
            await page.GotoAsync($"{_fixture.BaseUrl}/migration/server-mappath", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            var card = page.Locator("[data-audit-control='resolveurl-demo']");
            var output = await card.Locator("strong").TextContentAsync();

            Assert.NotNull(output);
            Assert.Equal("/Products", output!.Trim());
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

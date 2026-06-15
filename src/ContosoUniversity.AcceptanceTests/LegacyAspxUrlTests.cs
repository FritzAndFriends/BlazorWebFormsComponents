using Microsoft.Playwright;

namespace ContosoUniversity.AcceptanceTests;

/// <summary>
/// Verifies that legacy Web Forms .aspx URLs are redirected to the equivalent
/// clean Blazor routes. This preserves shared bookmarks and inbound links
/// that still use the original .aspx paths.
/// </summary>
[Collection("Playwright")]
public class LegacyAspxUrlTests
{
    private readonly PlaywrightFixture _fixture;

    public LegacyAspxUrlTests(PlaywrightFixture fixture) => _fixture = fixture;

    /// <summary>
    /// Each .aspx URL should redirect (301) and ultimately serve a 200 OK page
    /// at the corresponding clean Blazor route.
    /// </summary>
    [Theory]
    [InlineData("/Home.aspx")]
    [InlineData("/About.aspx")]
    [InlineData("/Students.aspx")]
    [InlineData("/Courses.aspx")]
    [InlineData("/Instructors.aspx")]
    public async Task LegacyAspxUrl_ReturnsSuccessfulResponse(string aspxPath)
    {
        var page = await _fixture.NewPageAsync();
        // Playwright follows redirects automatically; response.Ok checks the final response
        var response = await page.GotoAsync($"{TestConfiguration.BaseUrl}{aspxPath}");

        Assert.NotNull(response);
        Assert.True(response.Ok,
            $"Legacy URL '{aspxPath}' did not resolve to a success response. " +
            $"Final HTTP status: {response.Status}. Final URL: {page.Url}");
    }

    /// <summary>
    /// After following the redirect, the browser URL should no longer contain
    /// ".aspx" — confirming the canonical Blazor route was reached.
    /// </summary>
    [Theory]
    [InlineData("/Home.aspx")]
    [InlineData("/About.aspx")]
    [InlineData("/Students.aspx")]
    [InlineData("/Courses.aspx")]
    [InlineData("/Instructors.aspx")]
    public async Task LegacyAspxUrl_RedirectsToCleanRoute(string aspxPath)
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}{aspxPath}");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Assert.DoesNotContain(".aspx", page.Url, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// The redirected page should actually render content — not be blank or an error page.
    /// </summary>
    [Theory]
    [InlineData("/Home.aspx",       "Contoso")]
    [InlineData("/About.aspx",      "Student")]
    [InlineData("/Students.aspx",   "Student")]
    [InlineData("/Courses.aspx",    "Course")]
    [InlineData("/Instructors.aspx","Instructor")]
    public async Task LegacyAspxUrl_PageRendersExpectedContent(string aspxPath, string expectedText)
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}{aspxPath}");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var content = await page.ContentAsync();
        Assert.Contains(expectedText, content, StringComparison.OrdinalIgnoreCase);
    }
}

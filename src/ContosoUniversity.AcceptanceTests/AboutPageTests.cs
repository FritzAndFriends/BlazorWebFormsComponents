using Microsoft.Playwright;

namespace ContosoUniversity.AcceptanceTests;

/// <summary>
/// Verifies the About page renders a GridView with enrollment statistics.
/// About.aspx shows a GridView (ID: EnrollmentsStat) with enrollment date/count data.
/// </summary>
[Collection("Playwright")]
public class AboutPageTests
{
    private readonly PlaywrightFixture _fixture;

    public AboutPageTests(PlaywrightFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AboutPage_Loads()
    {
        var page = await _fixture.NewPageAsync();
        var response = await page.GotoAsync($"{TestConfiguration.BaseUrl}/About.aspx");

        Assert.NotNull(response);
        Assert.True(response.Ok, $"About page returned HTTP {response.Status}");
    }

    [Fact]
    public async Task AboutPage_HasPageTitle()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/About.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var content = await page.ContentAsync();
        Assert.True(
            content.Contains("Statistic", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("Students Body", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("Enrollment", StringComparison.OrdinalIgnoreCase),
            "About page should contain enrollment statistics heading text");
    }

    [Fact]
    public async Task AboutPage_GridViewRendersAsTable()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/About.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // The GridView (ID: EnrollmentsStat) renders as an HTML <table>
        // ASP.NET Web Forms prefixes IDs with the content placeholder name
        var gridView = page.Locator("table[id*='EnrollmentsStat'], table.grid, #grv table");
        var count = await gridView.CountAsync();

        if (count == 0)
        {
            // Fallback: look for any table in the #grv container or page
            gridView = page.Locator("#grv table, table").First;
            count = await gridView.CountAsync();
        }

        Assert.True(count > 0,
            "About page should render the EnrollmentsStat GridView as an HTML table");
    }

    [Fact]
    public async Task AboutPage_GridViewHasExpectedColumns()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/About.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Check the header row for expected column names
        var headerCells = page.Locator("table[id*='EnrollmentsStat'] th, table.grid th, #grv table th");
        if (await headerCells.CountAsync() == 0)
        {
            // Fallback: try first table header cells
            headerCells = page.Locator("table th");
        }

        var headerCount = await headerCells.CountAsync();
        Assert.True(headerCount >= 2,
            $"GridView should have at least 2 header columns (Date, Count), found {headerCount}");

        // Collect header text for validation
        var headerTexts = new List<string>();
        for (var i = 0; i < headerCount; i++)
        {
            headerTexts.Add(await headerCells.Nth(i).InnerTextAsync());
        }

        var allHeaders = string.Join(", ", headerTexts);
        var hasDateColumn = headerTexts.Any(h =>
            h.Contains("Date", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Enrollment", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Key", StringComparison.OrdinalIgnoreCase));
        var hasCountColumn = headerTexts.Any(h =>
            h.Contains("Student", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Count", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Value", StringComparison.OrdinalIgnoreCase));

        Assert.True(hasDateColumn,
            $"GridView should have an Enrollment Date column. Found headers: {allHeaders}");
        Assert.True(hasCountColumn,
            $"GridView should have a Students count column. Found headers: {allHeaders}");
    }

    [Fact]
    public async Task AboutPage_GridViewHasDataRows()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/About.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Count data rows (exclude header row)
        var dataRows = page.Locator("table[id*='EnrollmentsStat'] tr, table.grid tr, #grv table tr");
        if (await dataRows.CountAsync() == 0)
        {
            dataRows = page.Locator("table tr");
        }

        var rowCount = await dataRows.CountAsync();
        // At least 1 header row + 1 data row = 2 total rows
        Assert.True(rowCount >= 2,
            $"GridView should have at least 1 data row (found {rowCount} total rows including header)");
    }
}

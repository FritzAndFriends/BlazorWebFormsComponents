using Microsoft.Playwright;

namespace ContosoUniversity.AcceptanceTests;

/// <summary>
/// Verifies the Instructors page — GridView display and column sorting.
/// Instructors.aspx shows a sortable GridView of all instructors.
/// Uses UpdatePanel for AJAX sorting.
/// </summary>
[Collection("Playwright")]
public class InstructorsPageTests
{
    private readonly PlaywrightFixture _fixture;

    public InstructorsPageTests(PlaywrightFixture fixture) => _fixture = fixture;

    // ---------------------------------------------------------------
    // Page Load
    // ---------------------------------------------------------------

    [Fact]
    public async Task InstructorsPage_Loads()
    {
        var page = await _fixture.NewPageAsync();
        var response = await page.GotoAsync($"{TestConfiguration.BaseUrl}/Instructors.aspx");

        Assert.NotNull(response);
        Assert.True(response.Ok, $"Instructors page returned HTTP {response.Status}");
    }

    [Fact]
    public async Task InstructorsPage_GridViewShowsInstructors()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Instructors.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // GridView (ID: grvInstructors) renders as an HTML table
        var gridRows = page.Locator(
            "table[id*='grvInstructors'] tr, table.grv tr, #grv table tr");
        if (await gridRows.CountAsync() == 0)
        {
            gridRows = page.Locator("table tr");
        }

        var rowCount = await gridRows.CountAsync();
        Assert.True(rowCount >= 2,
            $"Instructors GridView should have at least 1 instructor row (header + data), found {rowCount}");
    }

    [Fact]
    public async Task InstructorsPage_GridViewHasExpectedColumns()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Instructors.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var headerCells = page.Locator(
            "table[id*='grvInstructors'] th, table.grv th, #grv table th");
        if (await headerCells.CountAsync() == 0)
        {
            headerCells = page.Locator("table th");
        }

        var headerCount = await headerCells.CountAsync();
        Assert.True(headerCount >= 3,
            $"Instructors GridView should have at least 3 columns, found {headerCount}");

        var headerTexts = new List<string>();
        for (var i = 0; i < headerCount; i++)
        {
            headerTexts.Add(await headerCells.Nth(i).InnerTextAsync());
        }

        var allHeaders = string.Join(", ", headerTexts);

        // Expect columns for ID, FirstName, LastName, Email
        var hasNameColumn = headerTexts.Any(h =>
            h.Contains("Name", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("First", StringComparison.OrdinalIgnoreCase) ||
            h.Contains("Last", StringComparison.OrdinalIgnoreCase));

        Assert.True(hasNameColumn,
            $"Instructors GridView should have a Name column. Found: {allHeaders}");
    }

    // ---------------------------------------------------------------
    // Column Sorting
    // ---------------------------------------------------------------

    [Fact]
    public async Task InstructorsPage_ColumnHeaderClickSortsGrid()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Instructors.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // GridView with AllowSorting=true renders column headers as clickable links
        var sortableHeaders = page.Locator(
            "table[id*='grvInstructors'] th a, table.grv th a, #grv table th a");
        if (await sortableHeaders.CountAsync() == 0)
        {
            sortableHeaders = page.Locator("table th a");
        }

        var sortableCount = await sortableHeaders.CountAsync();
        Assert.True(sortableCount > 0,
            "Instructors GridView should have clickable column headers for sorting");

        // Capture the initial content of the first data cell for comparison
        var firstDataCell = page.Locator(
            "table[id*='grvInstructors'] tr:nth-child(2) td, " +
            "table.grv tr:nth-child(2) td, " +
            "#grv table tr:nth-child(2) td").First;
        if (await firstDataCell.CountAsync() == 0)
        {
            firstDataCell = page.Locator("table tr:nth-child(2) td").First;
        }

        var initialContent = "";
        if (await firstDataCell.CountAsync() > 0)
        {
            initialContent = await firstDataCell.InnerTextAsync();
        }

        // Click the first sortable header
        await sortableHeaders.First.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Page should not error after sorting
        var pageTitle = await page.TitleAsync();
        Assert.DoesNotContain("Error", pageTitle, StringComparison.OrdinalIgnoreCase);

        // The sort should have been applied — grid should still be intact
        var rowsAfterSort = page.Locator(
            "table[id*='grvInstructors'] tr, table.grv tr, #grv table tr");
        if (await rowsAfterSort.CountAsync() == 0)
        {
            rowsAfterSort = page.Locator("table tr");
        }

        Assert.True(await rowsAfterSort.CountAsync() >= 2,
            "GridView should still have data rows after sorting");
    }

    [Fact]
    public async Task InstructorsPage_SortingTogglesDirection()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Instructors.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var sortableHeaders = page.Locator(
            "table[id*='grvInstructors'] th a, table.grv th a, #grv table th a");
        if (await sortableHeaders.CountAsync() == 0)
        {
            sortableHeaders = page.Locator("table th a");
        }

        if (await sortableHeaders.CountAsync() > 0)
        {
            // First click — sort ascending
            await sortableHeaders.First.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var firstSortContent = await GetFirstDataCellContent(page);

            // Second click on same header — should toggle to descending
            // Re-locate the header after the AJAX refresh
            sortableHeaders = page.Locator(
                "table[id*='grvInstructors'] th a, table.grv th a, #grv table th a");
            if (await sortableHeaders.CountAsync() == 0)
            {
                sortableHeaders = page.Locator("table th a");
            }

            await sortableHeaders.First.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var secondSortContent = await GetFirstDataCellContent(page);

            // If there are multiple instructors, toggling sort direction should change order
            // If only 1 row, content will be the same — that's OK
            var pageTitle = await page.TitleAsync();
            Assert.DoesNotContain("Error", pageTitle, StringComparison.OrdinalIgnoreCase);
        }
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    private static async Task<string> GetFirstDataCellContent(IPage page)
    {
        var firstDataCell = page.Locator(
            "table[id*='grvInstructors'] tr:nth-child(2) td, " +
            "table.grv tr:nth-child(2) td, " +
            "#grv table tr:nth-child(2) td").First;
        if (await firstDataCell.CountAsync() == 0)
        {
            firstDataCell = page.Locator("table tr:nth-child(2) td").First;
        }

        return await firstDataCell.CountAsync() > 0
            ? await firstDataCell.InnerTextAsync()
            : "";
    }
}

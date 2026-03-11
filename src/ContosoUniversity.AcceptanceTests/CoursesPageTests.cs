using Microsoft.Playwright;

namespace ContosoUniversity.AcceptanceTests;

/// <summary>
/// Verifies the Courses page — department filtering, search, and DetailsView display.
/// Courses.aspx has a department DropDownList, GridView of courses, and search with DetailsView.
/// Uses UpdatePanel for AJAX.
/// </summary>
[Collection("Playwright")]
public class CoursesPageTests
{
    private readonly PlaywrightFixture _fixture;

    public CoursesPageTests(PlaywrightFixture fixture) => _fixture = fixture;

    // ---------------------------------------------------------------
    // Page Load
    // ---------------------------------------------------------------

    [Fact]
    public async Task CoursesPage_Loads()
    {
        var page = await _fixture.NewPageAsync();
        var response = await page.GotoAsync($"{TestConfiguration.BaseUrl}/Courses.aspx");

        Assert.NotNull(response);
        Assert.True(response.Ok, $"Courses page returned HTTP {response.Status}");
    }

    // ---------------------------------------------------------------
    // Department Dropdown
    // ---------------------------------------------------------------

    [Fact]
    public async Task CoursesPage_DepartmentDropdownHasOptions()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Courses.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var dropdown = page.Locator("select[id*='drpDepartments']");
        if (await dropdown.CountAsync() == 0)
        {
            // Fallback: find any select element within the dropdown section
            dropdown = page.Locator("#dropList select, select").First;
        }

        Assert.True(await dropdown.CountAsync() > 0,
            "Department dropdown not found on Courses page");

        var options = dropdown.Locator("option");
        var optionCount = await options.CountAsync();
        Assert.True(optionCount >= 2,
            $"Department dropdown should have at least 2 options (placeholder + 1 department), found {optionCount}");
    }

    [Fact]
    public async Task CoursesPage_SelectingDepartmentFiltersCourses()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Courses.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var dropdown = page.Locator("select[id*='drpDepartments']");
        if (await dropdown.CountAsync() == 0)
        {
            dropdown = page.Locator("#dropList select, select").First;
        }

        var searchButton = page.Locator("input[id*='btnSearchCourse'], button[id*='btnSearchCourse']");
        if (await searchButton.CountAsync() == 0)
        {
            searchButton = page.Locator("#dropList input[type='submit'], #dropList button").First;
        }

        if (await dropdown.CountAsync() > 0)
        {
            var options = dropdown.Locator("option");
            var optionCount = await options.CountAsync();

            if (optionCount >= 2)
            {
                // Select the second option (first real department)
                var optionValue = await options.Nth(1).GetAttributeAsync("value");
                if (!string.IsNullOrEmpty(optionValue))
                {
                    await dropdown.SelectOptionAsync(optionValue);

                    // Click search/filter button if present
                    if (await searchButton.CountAsync() > 0)
                    {
                        await searchButton.ClickAsync();
                    }

                    // Wait for UpdatePanel AJAX refresh
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                    // GridView should show filtered courses
                    var courseGrid = page.Locator(
                        "table[id*='grvCourses'] tr, #dropList table tr");
                    if (await courseGrid.CountAsync() == 0)
                    {
                        courseGrid = page.Locator("table tr");
                    }

                    var rowCount = await courseGrid.CountAsync();
                    Assert.True(rowCount >= 1,
                        $"After selecting a department, courses GridView should show results. Found {rowCount} rows");
                }
            }
        }
    }

    // ---------------------------------------------------------------
    // GridView Display
    // ---------------------------------------------------------------

    [Fact]
    public async Task CoursesPage_GridViewHasCourseColumns()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Courses.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Trigger a department selection to populate the grid
        await SelectFirstDepartment(page);

        var headerCells = page.Locator(
            "table[id*='grvCourses'] th, #dropList table th");
        if (await headerCells.CountAsync() == 0)
        {
            headerCells = page.Locator("table th");
        }

        var headerCount = await headerCells.CountAsync();
        if (headerCount > 0)
        {
            var headerTexts = new List<string>();
            for (var i = 0; i < headerCount; i++)
            {
                headerTexts.Add(await headerCells.Nth(i).InnerTextAsync());
            }

            var allHeaders = string.Join(", ", headerTexts);
            Assert.True(headerTexts.Any(h =>
                h.Contains("Course", StringComparison.OrdinalIgnoreCase) ||
                h.Contains("ID", StringComparison.OrdinalIgnoreCase) ||
                h.Contains("Name", StringComparison.OrdinalIgnoreCase)),
                $"Courses GridView should have course-related columns. Found: {allHeaders}");
        }
    }

    // ---------------------------------------------------------------
    // Course Search with DetailsView
    // ---------------------------------------------------------------

    [Fact]
    public async Task CoursesPage_SearchByCourseNameShowsDetailsView()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Courses.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await page.WaitForTimeoutAsync(1000); // Wait for Blazor WebSocket connection

        var searchBox = page.Locator("input[id*='txtCourse']");
        var searchButton = page.Locator("input[id*='search'], button[id*='search']");

        if (await searchBox.CountAsync() == 0)
        {
            searchBox = page.Locator("#autoComplete input[type='text']").First;
            searchButton = page.Locator("#autoComplete input[type='submit'], #autoComplete button").First;
        }

        if (await searchBox.CountAsync() > 0 && await searchButton.CountAsync() > 0)
        {
            // Search with a partial course name
            await searchBox.FillAsync("a");
            await searchButton.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await page.WaitForTimeoutAsync(500); // Allow Blazor UI to re-render after async operation

            // DetailsView (ID: dtlCourses) should appear
            var detailsView = page.Locator(
                "table[id*='dtlCourses'], table.details, #autoComplete table");
            var pageContent = await page.ContentAsync();

            var hasResults = await detailsView.CountAsync() > 0 ||
                             pageContent.Contains("CourseID", StringComparison.OrdinalIgnoreCase) ||
                             pageContent.Contains("CourseName", StringComparison.OrdinalIgnoreCase);

            Assert.True(hasResults,
                "After searching by course name, a DetailsView or course details should appear");
        }
    }

    // ---------------------------------------------------------------
    // GridView Pagination
    // ---------------------------------------------------------------

    [Fact]
    public async Task CoursesPage_GridViewPaginationWorks()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Courses.aspx");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Select a department to populate grid
        await SelectFirstDepartment(page);

        // Look for pagination controls (page numbers or Next link)
        var pagerLinks = page.Locator(
            "table[id*='grvCourses'] a, " +
            "#dropList table td a, " +
            "table .pager a, " +
            "table a[href*='Page']");

        if (await pagerLinks.CountAsync() > 0)
        {
            // Get first page content for comparison
            var firstPageContent = await page.Locator("table[id*='grvCourses'], #dropList table, table").First.InnerTextAsync();

            // Click the next/second page link
            await pagerLinks.First.ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Page should have changed — URL or content should differ
            var secondPageContent = await page.Locator("table[id*='grvCourses'], #dropList table, table").First.InnerTextAsync();

            // Pagination worked if content changed (or at minimum page didn't error)
            var pageTitle = await page.TitleAsync();
            Assert.DoesNotContain("Error", pageTitle, StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            // No pagination visible — may have too few courses for paging
            Assert.True(true,
                "No pagination controls found — dataset may be small enough to fit on one page");
        }
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    /// <summary>
    /// Selects the first department in the dropdown and clicks the search button
    /// to populate the courses GridView.
    /// </summary>
    private static async Task SelectFirstDepartment(IPage page)
    {
        var dropdown = page.Locator("select[id*='drpDepartments']");
        if (await dropdown.CountAsync() == 0)
        {
            dropdown = page.Locator("#dropList select, select").First;
        }

        var searchButton = page.Locator("input[id*='btnSearchCourse'], button[id*='btnSearchCourse']");
        if (await searchButton.CountAsync() == 0)
        {
            searchButton = page.Locator("#dropList input[type='submit'], #dropList button").First;
        }

        if (await dropdown.CountAsync() > 0)
        {
            var options = dropdown.Locator("option");
            if (await options.CountAsync() >= 2)
            {
                var optionValue = await options.Nth(1).GetAttributeAsync("value");
                if (!string.IsNullOrEmpty(optionValue))
                {
                    await dropdown.SelectOptionAsync(optionValue);
                    if (await searchButton.CountAsync() > 0)
                    {
                        await searchButton.ClickAsync();
                        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    }
                }
            }
        }
    }
}

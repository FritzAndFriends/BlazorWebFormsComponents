using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests;

[Collection(nameof(PlaywrightCollection))]
public class ComponentListPageTests
{
    private readonly PlaywrightFixture _fixture;

    public ComponentListPageTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ComponentListPage_Loads_Successfully()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();

        try
        {
            // Act
            var response = await page.GotoAsync($"{_fixture.BaseUrl}/ComponentList");

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Ok, $"Response status: {response.Status}");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task ComponentListPage_HasNoConsoleErrors()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        
        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add(msg.Text);
            }
        };

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ComponentList");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Assert
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task ComponentListPage_HasComponentLinks()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();

        try
        {
            // Act
            await page.GotoAsync($"{_fixture.BaseUrl}/ComponentList");
            var links = await page.QuerySelectorAllAsync("a[href*='ControlSamples']");

            // Assert
            Assert.NotEmpty(links);
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

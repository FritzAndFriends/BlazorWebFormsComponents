using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests;

[Collection(nameof(PlaywrightCollection))]
public class HomePageTests
{
    private readonly PlaywrightFixture _fixture;

    public HomePageTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HomePage_Loads_Successfully()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();

        try
        {
            // Act
            var response = await page.GotoAsync(_fixture.BaseUrl);

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
    public async Task HomePage_HasNoConsoleErrors()
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
            await page.GotoAsync(_fixture.BaseUrl);
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
    public async Task HomePage_HasExpectedTitle()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();

        try
        {
            // Act
            await page.GotoAsync(_fixture.BaseUrl);
            var title = await page.TitleAsync();

            // Assert
            Assert.Contains("Blazor", title);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task HomePage_HasNavigationMenu()
    {
        // Arrange
        var page = await _fixture.NewPageAsync();

        try
        {
            // Act
            await page.GotoAsync(_fixture.BaseUrl);
            var navMenu = await page.QuerySelectorAsync("nav");

            // Assert
            Assert.NotNull(navMenu);
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

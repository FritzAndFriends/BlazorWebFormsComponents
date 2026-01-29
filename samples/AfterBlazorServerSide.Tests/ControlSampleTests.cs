using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests;

[Collection(nameof(PlaywrightCollection))]
public class ControlSampleTests
{
    private readonly PlaywrightFixture _fixture;

    public ControlSampleTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    // Editor Controls
    [Theory]
    [InlineData("/ControlSamples/Button")]
    [InlineData("/ControlSamples/CheckBox")]
    [InlineData("/ControlSamples/HyperLink")]
    [InlineData("/ControlSamples/LinkButton")]
    [InlineData("/ControlSamples/Literal")]
    [InlineData("/ControlSamples/DropDownList")]
    public async Task EditorControl_Loads_WithoutErrors(string path)
    {
        await VerifyPageLoadsWithoutErrors(path);
    }

    // Data Controls
    [Theory]
    [InlineData("/ControlSamples/DataList")]
    [InlineData("/ControlSamples/Repeater")]
    [InlineData("/ControlSamples/ListView")]
    public async Task DataControl_Loads_WithoutErrors(string path)
    {
        await VerifyPageLoadsWithoutErrors(path);
    }

    // Navigation Controls
    [Theory]
    [InlineData("/ControlSamples/TreeView")]
    public async Task NavigationControl_Loads_WithoutErrors(string path)
    {
        await VerifyPageLoadsWithoutErrors(path);
    }

    // Note: Some control samples like GridView, FormView, Validations, LoginControls, and AdRotator
    // don't have Index.razor files and are only accessible through Nav.razor or specific sub-pages.

    private async Task VerifyPageLoadsWithoutErrors(string path)
    {
        // Arrange
        var page = await _fixture.NewPageAsync();
        var consoleErrors = new List<string>();
        var pageErrors = new List<string>();

        page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                consoleErrors.Add($"{path}: {msg.Text}");
            }
        };

        page.PageError += (_, error) =>
        {
            pageErrors.Add($"{path}: {error}");
        };

        try
        {
            // Act
            var response = await page.GotoAsync($"{_fixture.BaseUrl}{path}", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle,
                Timeout = 30000
            });

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Ok, $"Page {path} failed to load with status: {response.Status}");
            Assert.Empty(consoleErrors);
            Assert.Empty(pageErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}

using Microsoft.Playwright;

namespace ZavaLoanPortal.AcceptanceTests;

/// <summary>
/// Verifies the Loan Application Wizard component renders and navigates between steps.
/// </summary>
[Collection("Playwright")]
public class WizardTests
{
    private readonly PlaywrightFixture _fixture;

    public WizardTests(PlaywrightFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task DefaultPage_RedirectsToLogin_WhenNotAuthenticated()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync(TestConfiguration.BaseUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Should redirect to login since user is not authenticated
        Assert.Contains("Login", page.Url, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LoginPage_ShowsAuthGatewayLink()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Login");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var authMessage = page.Locator("text=Authentication is handled by ZavaAuthGateway");
        await Assertions.Expect(authMessage).ToBeVisibleAsync();

        var authLink = page.Locator("text=Go to ZavaAuthGateway Login");
        await Assertions.Expect(authLink).ToBeVisibleAsync();
    }

    [Fact]
    public async Task WizardRenders_WhenAuthenticated()
    {
        var page = await _fixture.NewPageAsync();

        // Authenticate via dev endpoint
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/dev/login");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // The Wizard renders step 1 content: "Customer ID:" label
        var customerIdLabel = page.Locator("text=Customer ID:");
        await Assertions.Expect(customerIdLabel).ToBeVisibleAsync();

        // Verify the Next button is present (Wizard navigation)
        var nextButton = page.Locator("input[value='Next']");
        await Assertions.Expect(nextButton).ToBeVisibleAsync();
    }

    [Fact]
    public async Task WizardNavigation_NextMovesToEmploymentStep()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/dev/login");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Verify we're on step 1 (Personal Info)
        await Assertions.Expect(page.Locator("text=Customer ID:")).ToBeVisibleAsync();

        // Click Next to move to Employment step
        await page.Locator("input[value='Next']").ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Should now show Employment step fields
        var employerField = page.Locator("text=Employer:");
        await Assertions.Expect(employerField).ToBeVisibleAsync();
    }

    [Fact]
    public async Task WizardNavigation_CanReachLoanDetailsStep()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/dev/login");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Step 1 -> Step 2
        await page.Locator("input[value='Next']").ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Step 2 -> Step 3
        await page.Locator("input[value='Next']").ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Should show Loan Details step
        var loanProductField = page.Locator("text=Loan Product:");
        await Assertions.Expect(loanProductField).ToBeVisibleAsync();
    }

    [Fact]
    public async Task WizardNavigation_PreviousGoesBack()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/dev/login");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Go to step 2
        await page.Locator("input[value='Next']").ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Assertions.Expect(page.Locator("text=Employer:")).ToBeVisibleAsync();

        // Go back to step 1
        await page.Locator("input[value='Previous']").ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Should be back on Personal Info
        await Assertions.Expect(page.Locator("text=Customer ID:")).ToBeVisibleAsync();
    }

    [Fact]
    public async Task WizardPage_ShowsLoanApplicationHeader()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/dev/login");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var header = page.Locator("text=New Loan Application");
        await Assertions.Expect(header).ToBeVisibleAsync();
    }
}

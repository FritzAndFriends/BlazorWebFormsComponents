using Microsoft.Playwright;

namespace WingtipToys.AcceptanceTests;

/// <summary>
/// Verifies that the Register and Login pages render correctly
/// with the expected BWFC form controls and are functional.
/// </summary>
[Collection("Playwright")]
public class AuthenticationTests
{
    private readonly PlaywrightFixture _fixture;

    public AuthenticationTests(PlaywrightFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task RegisterPage_HasExpectedFormFields()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Account/Register");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Register page should have email, password, and confirm password fields
        var emailInput = page.Locator("input[type='text'], input[type='email']").First;
        var passwordInputs = page.Locator("input[type='password']");

        Assert.True(await emailInput.CountAsync() > 0, "Register page should have an email/text input");
        Assert.True(await passwordInputs.CountAsync() >= 2, "Register page should have password and confirm password fields");

        // Should have a submit button
        var submitButton = page.GetByRole(AriaRole.Button).First;
        Assert.True(await submitButton.CountAsync() > 0, "Register page should have a submit button");
    }

    [Fact]
    public async Task LoginPage_HasExpectedFormFields()
    {
        var page = await _fixture.NewPageAsync();
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Account/Login");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Login page should have email and password fields
        var emailInput = page.Locator("input[type='text'], input[type='email']").First;
        var passwordInput = page.Locator("input[type='password']").First;

        Assert.True(await emailInput.CountAsync() > 0, "Login page should have an email/text input");
        Assert.True(await passwordInput.CountAsync() > 0, "Login page should have a password input");

        // Should have a login button
        var loginButton = page.GetByRole(AriaRole.Button).First;
        Assert.True(await loginButton.CountAsync() > 0, "Login page should have a login button");
    }

    [Fact]
    public async Task RegisterAndLogin_EndToEnd()
    {
        var page = await _fixture.NewPageAsync();

        // Step 1: Register a new user
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Account/Register");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var testEmail = $"test-{Guid.NewGuid():N}@example.com";
        const string testPassword = "Test@12345";

        var emailInput = page.Locator("input[type='text'], input[type='email']").First;
        await emailInput.FillAsync(testEmail);

        var passwordInputs = page.Locator("input[type='password']");
        if (await passwordInputs.CountAsync() >= 2)
        {
            await passwordInputs.Nth(0).FillAsync(testPassword);
            await passwordInputs.Nth(1).FillAsync(testPassword);
        }

        var registerButton = page.GetByRole(AriaRole.Button).First;
        await registerButton.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Step 2: Log in with the new user
        await page.GotoAsync($"{TestConfiguration.BaseUrl}/Account/Login");
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        emailInput = page.Locator("input[type='text'], input[type='email']").First;
        await emailInput.FillAsync(testEmail);

        var passwordInput = page.Locator("input[type='password']").First;
        await passwordInput.FillAsync(testPassword);

        var loginButton = page.GetByRole(AriaRole.Button).First;
        await loginButton.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Step 3: Verify authenticated state — should see user greeting or manage link
        var pageContent = await page.ContentAsync();
        var isAuthenticated =
            pageContent.Contains("Hello", StringComparison.OrdinalIgnoreCase) ||
            pageContent.Contains("Manage", StringComparison.OrdinalIgnoreCase) ||
            pageContent.Contains("Log out", StringComparison.OrdinalIgnoreCase) ||
            pageContent.Contains(testEmail, StringComparison.OrdinalIgnoreCase);

        Assert.True(isAuthenticated,
            "After login, the page should show an authenticated state (greeting, manage link, or logout)");
    }
}

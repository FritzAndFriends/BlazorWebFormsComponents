using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

[Collection(nameof(WingtipToysPlaywrightCollection))]
public class WingtipToysAcceptanceTests
{
    private readonly WingtipToysPlaywrightFixture _fixture;

    public WingtipToysAcceptanceTests(WingtipToysPlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task HomePage_RendersMasterShellAndMainContent()
    {
        var page = await _fixture.NewPageAsync();
        var consoleErrors = TrackConsoleErrors(page);

        try
        {
            var response = await GotoAsync(page, "/");

            Assert.NotNull(response);
            Assert.True(response!.Ok, $"Response status: {response.Status}");

            await Assertions.Expect(page.Locator(".navbar-brand")).ToHaveTextAsync("Wingtip Toys");
            await Assertions.Expect(page.Locator("#CategoryMenu a")).ToHaveCountAsync(4);
            await Assertions.Expect(page.Locator(".jumbotron")).ToContainTextAsync("Find the perfect transportation toy.");
            await Assertions.Expect(page.Locator("footer")).ToContainTextAsync("2013 - Wingtip Toys");

            var html = await page.ContentAsync();
            Assert.DoesNotContain("<contentplaceholder", html, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("<content ", html, StringComparison.OrdinalIgnoreCase);
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task ProductList_RouteRunsInsideSiteShell()
    {
        var page = await _fixture.NewPageAsync();
        var consoleErrors = TrackConsoleErrors(page);

        try
        {
            var response = await GotoAsync(page, "/ProductList?id=2");

            Assert.NotNull(response);
            Assert.True(response!.Ok, $"Response status: {response.Status}");

            await Assertions.Expect(page.Locator("h1")).ToHaveTextAsync("Cars");
            await Assertions.Expect(page.Locator(".thumbnail")).ToHaveCountAsync(3);
            await Assertions.Expect(page.Locator("a[href='/ProductList']")).ToContainTextAsync("ProductList");
            await Assertions.Expect(page.Locator("a[href='/ShoppingCart']")).ToContainTextAsync("Cart (0)");
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task ProductDetails_AddToCartFlowUpdatesCart()
    {
        var page = await _fixture.NewPageAsync();
        var consoleErrors = TrackConsoleErrors(page);

        try
        {
            var response = await GotoAsync(page, "/ProductDetails?id=1");

            Assert.NotNull(response);
            Assert.True(response!.Ok, $"Response status: {response.Status}");

            await Assertions.Expect(page.Locator("h1")).ToHaveTextAsync("Paper Boat");
            await page.Locator("button:has-text('Add To Cart')").ClickAsync();

            await Assertions.Expect(page).ToHaveURLAsync(new Regex(".*/ShoppingCart(\\?addProductId=1)?$", RegexOptions.IgnoreCase));
            await Assertions.Expect(page.Locator("#ShoppingCartTitle h1")).ToHaveTextAsync("Shopping Cart");
            await Assertions.Expect(page.Locator("tbody")).ToContainTextAsync("Paper Boat");
            await Assertions.Expect(page.Locator("input[name='quantity']")).ToHaveValueAsync("1");
            await Assertions.Expect(page.Locator("a[href='/ShoppingCart']")).ToContainTextAsync("Cart (1)");
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    [Fact]
    public async Task AccountForms_RegisterLoginAndLogoutBehaveLikeRunnableFlows()
    {
        var page = await _fixture.NewPageAsync();
        var consoleErrors = TrackConsoleErrors(page);
        var email = $"wingtip-{Guid.NewGuid():N}@example.com";

        try
        {
            await GotoAsync(page, "/Account/Register");
            await Assertions.Expect(page.Locator("form[action='/Account/PerformRegister']")).ToBeVisibleAsync();

            await page.Locator("#register-email").FillAsync(email);
            await page.Locator("#register-password").FillAsync("pass-123");
            await page.Locator("#register-confirm").FillAsync("different-pass");
            await page.Locator("button:has-text('Register')").ClickAsync();

            await Assertions.Expect(page).ToHaveURLAsync(new Regex(".*/Account/Register\\?error=.*", RegexOptions.IgnoreCase));
            await Assertions.Expect(page.Locator(".text-danger")).ToContainTextAsync("Passwords do not match");

            await GotoAsync(page, "/Account/Register");
            await page.Locator("#register-email").FillAsync(email);
            await page.Locator("#register-password").FillAsync("pass-123");
            await page.Locator("#register-confirm").FillAsync("pass-123");
            await page.Locator("button:has-text('Register')").ClickAsync();

            await Assertions.Expect(page).ToHaveURLAsync(new Regex(".*/Account/Login\\?registered=1$", RegexOptions.IgnoreCase));
            await Assertions.Expect(page.Locator(".text-success")).ToContainTextAsync("Registration succeeded");

            await page.Locator("#login-email").FillAsync(email);
            await page.Locator("#login-password").FillAsync("bad-pass");
            await page.Locator("button:has-text('Log in')").ClickAsync();

            await Assertions.Expect(page).ToHaveURLAsync(new Regex(".*/Account/Login\\?error=.*", RegexOptions.IgnoreCase));
            await Assertions.Expect(page.Locator(".text-danger")).ToContainTextAsync("Invalid email or password.");

            await GotoAsync(page, "/Account/Login");
            await page.Locator("#login-email").FillAsync(email);
            await page.Locator("#login-password").FillAsync("pass-123");
            await page.Locator("button:has-text('Log in')").ClickAsync();

            await Assertions.Expect(page).ToHaveURLAsync(new Regex(".*/?$", RegexOptions.IgnoreCase));
            await Assertions.Expect(page.Locator(".navbar-right")).ToContainTextAsync($"Hello, {email}!");

            await page.Locator("a[href='/Account/Logout']").ClickAsync();
            await Assertions.Expect(page).ToHaveURLAsync(new Regex(".*/?$", RegexOptions.IgnoreCase));
            await Assertions.Expect(page.Locator(".navbar-right")).ToContainTextAsync("Register");
            await Assertions.Expect(page.Locator(".navbar-right")).ToContainTextAsync("Log in");
            Assert.Empty(consoleErrors);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    private async Task<IResponse?> GotoAsync(IPage page, string relativeUrl)
    {
        return await page.GotoAsync($"{_fixture.BaseUrl}{relativeUrl}", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = 30000
        });
    }

    private static List<string> TrackConsoleErrors(IPage page)
    {
        var consoleErrors = new List<string>();

        page.Console += (_, msg) =>
        {
            if (msg.Type == "error" && !Regex.IsMatch(msg.Text, @"^\[\d{4}-\d{2}-\d{2}T"))
            {
                consoleErrors.Add(msg.Text);
            }
        };

        return consoleErrors;
    }
}

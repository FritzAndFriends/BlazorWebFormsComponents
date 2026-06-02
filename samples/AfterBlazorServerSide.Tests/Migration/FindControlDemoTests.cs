using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Integration tests for the FindControl runtime demo page at /migration/findcontrol.
/// Verifies direct lookup, nested search, chained calls, and case-insensitive matching.
/// </summary>
[Collection(nameof(PlaywrightCollection))]
public class FindControlDemoTests
{
	private readonly PlaywrightFixture _fixture;

	public FindControlDemoTests(PlaywrightFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task FindControl_DirectChildLookup_ShowsFound()
	{
		var page = await _fixture.NewPageAsync();

		try
		{
			await page.GotoAsync($"{_fixture.BaseUrl}/migration/findcontrol", new PageGotoOptions
			{
				WaitUntil = WaitUntilState.NetworkIdle,
				Timeout = 30000
			});

			// Wait for the after-render update
			var result = page.Locator("[data-testid='direct-result']");
			await result.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
			var text = await result.TextContentAsync();

			Assert.NotNull(text);
			Assert.Contains("FindControl found the TextBox", text!);
		}
		finally
		{
			await page.CloseAsync();
		}
	}

	[Fact]
	public async Task FindControl_NestedLookup_ShowsFound()
	{
		var page = await _fixture.NewPageAsync();

		try
		{
			await page.GotoAsync($"{_fixture.BaseUrl}/migration/findcontrol", new PageGotoOptions
			{
				WaitUntil = WaitUntilState.NetworkIdle,
				Timeout = 30000
			});

			var result = page.Locator("[data-testid='nested-result']");
			await result.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
			var text = await result.TextContentAsync();

			Assert.NotNull(text);
			Assert.Contains("recursed through InnerPanel", text!);
		}
		finally
		{
			await page.CloseAsync();
		}
	}

	[Fact]
	public async Task FindControl_ChainedLookup_ShowsFound()
	{
		var page = await _fixture.NewPageAsync();

		try
		{
			await page.GotoAsync($"{_fixture.BaseUrl}/migration/findcontrol", new PageGotoOptions
			{
				WaitUntil = WaitUntilState.NetworkIdle,
				Timeout = 30000
			});

			var result = page.Locator("[data-testid='chained-result']");
			await result.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
			var text = await result.TextContentAsync();

			Assert.NotNull(text);
			Assert.Contains("Chained:", text!);
			Assert.Contains("succeeded", text!);
		}
		finally
		{
			await page.CloseAsync();
		}
	}

	[Fact]
	public async Task FindControl_CaseInsensitive_ShowsFound()
	{
		var page = await _fixture.NewPageAsync();

		try
		{
			await page.GotoAsync($"{_fixture.BaseUrl}/migration/findcontrol", new PageGotoOptions
			{
				WaitUntil = WaitUntilState.NetworkIdle,
				Timeout = 30000
			});

			var result = page.Locator("[data-testid='case-result']");
			await result.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
			var text = await result.TextContentAsync();

			Assert.NotNull(text);
			Assert.Contains("mixedcasebox", text!);
			Assert.Contains("MixedCaseBox", text!);
		}
		finally
		{
			await page.CloseAsync();
		}
	}
}

using Microsoft.Playwright;

namespace AfterBlazorServerSide.Tests.Migration;

/// <summary>
/// Integration tests for the Custom WebControl demo page at /ControlSamples/Migration/CustomWebControl.
/// Verifies that WebControl with RenderContents renders correctly in Blazor.
/// </summary>
[Collection(nameof(PlaywrightCollection))]
public class CustomWebControlDemoTests
{
	private readonly PlaywrightFixture _fixture;

	public CustomWebControlDemoTests(PlaywrightFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task StatusBadge_RendersWithDataAttribute()
	{
		var page = await _fixture.NewPageAsync();

		try
		{
			await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/Migration/CustomWebControl", new PageGotoOptions
			{
				WaitUntil = WaitUntilState.NetworkIdle,
				Timeout = 30000
			});

			// StatusBadge should render span elements with data-status attributes
			var badge = page.Locator("[data-status='active']").First;
			await Assertions.Expect(badge).ToBeVisibleAsync();
			await Assertions.Expect(badge).ToContainTextAsync("Online");
		}
		finally
		{
			await page.CloseAsync();
		}
	}

	[Fact]
	public async Task StatusBadge_MultipleInstances_RenderCorrectly()
	{
		var page = await _fixture.NewPageAsync();

		try
		{
			await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/Migration/CustomWebControl", new PageGotoOptions
			{
				WaitUntil = WaitUntilState.NetworkIdle,
				Timeout = 30000
			});

			// Should have three badges with different statuses
			await Assertions.Expect(page.Locator("[data-status='active']")).ToBeVisibleAsync();
			await Assertions.Expect(page.Locator("[data-status='warning']")).ToBeVisibleAsync();
			await Assertions.Expect(page.Locator("[data-status='error']")).ToBeVisibleAsync();
		}
		finally
		{
			await page.CloseAsync();
		}
	}

	[Fact]
	public async Task InfoCard_RendersHeaderAndBody()
	{
		var page = await _fixture.NewPageAsync();

		try
		{
			await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/Migration/CustomWebControl", new PageGotoOptions
			{
				WaitUntil = WaitUntilState.NetworkIdle,
				Timeout = 30000
			});

			// InfoCard should render with card-header and card-body divs
			var header = page.Locator(".card-header").First;
			await Assertions.Expect(header).ToBeVisibleAsync();
			await Assertions.Expect(header).ToContainTextAsync("Migration Complete");

			var body = page.Locator(".card-body").First;
			await Assertions.Expect(body).ToBeVisibleAsync();
			await Assertions.Expect(body).ToContainTextAsync("without code changes");
		}
		finally
		{
			await page.CloseAsync();
		}
	}

	[Fact]
	public async Task Page_HasExpectedTitle()
	{
		var page = await _fixture.NewPageAsync();

		try
		{
			await page.GotoAsync($"{_fixture.BaseUrl}/ControlSamples/Migration/CustomWebControl", new PageGotoOptions
			{
				WaitUntil = WaitUntilState.NetworkIdle,
				Timeout = 30000
			});

			var heading = page.Locator("h2").First;
			await Assertions.Expect(heading).ToContainTextAsync("Custom WebControl Migration");
		}
		finally
		{
			await page.CloseAsync();
		}
	}
}

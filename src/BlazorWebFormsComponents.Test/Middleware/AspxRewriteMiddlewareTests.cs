using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test.Middleware;

/// <summary>
/// Integration tests for the UseBlazorWebFormsComponents middleware pipeline.
/// Covers .aspx rewriting (existing), .ashx handler interception (#423),
/// and .axd resource interception (#423).
///
/// These tests exercise the full middleware pipeline via TestServer, ensuring
/// that UseBlazorWebFormsComponents correctly registers and invokes all
/// URL-handling middleware based on BlazorWebFormsComponentsOptions.
/// </summary>
public class AspxRewriteMiddlewareTests : IDisposable
{
	private readonly TestServer _server;
	private readonly HttpClient _client;

	public AspxRewriteMiddlewareTests()
	{
		_server = CreateTestServer();
		_client = _server.CreateClient();
	}

	public void Dispose()
	{
		_client.Dispose();
		_server.Dispose();
		GC.SuppressFinalize(this);
	}

	#region Helpers

	/// <summary>
	/// Creates a TestServer with the full BWFC middleware pipeline and a
	/// terminal handler that returns 200 OK. Any request reaching the terminal
	/// means the middleware did NOT short-circuit it.
	/// </summary>
	private static TestServer CreateTestServer(Action<BlazorWebFormsComponentsOptions>? configure = null)
	{
		var builder = new WebHostBuilder()
			.ConfigureServices(services =>
			{
				services.AddBlazorWebFormsComponents(configure);
			})
			.Configure(app =>
			{
				app.UseBlazorWebFormsComponents();

				// Terminal: if a request reaches here, no middleware intercepted it
				app.Run(async context =>
				{
					context.Response.StatusCode = StatusCodes.Status200OK;
					await context.Response.WriteAsync("PASSTHROUGH");
				});
			});

		return new TestServer(builder);
	}

	/// <summary>
	/// Creates a disposable server+client pair with custom options for tests
	/// that need non-default configuration.
	/// </summary>
	private static (TestServer server, HttpClient client) CreateServerAndClient(
		Action<BlazorWebFormsComponentsOptions> configure)
	{
		var server = CreateTestServer(configure);
		var client = server.CreateClient();
		return (server, client);
	}

	#endregion

	#region Existing .aspx Rewriting (regression guard)

	[Fact]
	public async Task AspxRequest_Returns301Redirect()
	{
		var response = await _client.GetAsync("/Products.aspx");

		response.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
		response.Headers.Location?.ToString().ShouldBe("/Products");
	}

	[Fact]
	public async Task DefaultAspx_RedirectsToRoot()
	{
		var response = await _client.GetAsync("/Default.aspx");

		response.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
		response.Headers.Location?.ToString().ShouldBe("/");
	}

	[Fact]
	public async Task AspxRequest_PreservesQueryString()
	{
		var response = await _client.GetAsync("/Products.aspx?id=42&sort=name");

		response.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
		response.Headers.Location?.ToString().ShouldBe("/Products?id=42&sort=name");
	}

	[Fact]
	public async Task SubdirectoryDefaultAspx_RedirectsToDirectory()
	{
		var response = await _client.GetAsync("/Admin/Default.aspx");

		response.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
		response.Headers.Location?.ToString().ShouldBe("/Admin");
	}

	[Fact]
	public async Task AspxRewriting_WorksWhenAshxAndAxdDisabled()
	{
		var (server, client) = CreateServerAndClient(opts =>
		{
			opts.EnableAshxHandling = false;
			opts.EnableAxdHandling = false;
		});
		using (server)
		using (client)
		{
			var response = await client.GetAsync("/Products.aspx");

			response.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
			response.Headers.Location?.ToString().ShouldBe("/Products");
		}
	}

	#endregion

	#region .ashx Handler Interception

	[Fact]
	public async Task AshxRequest_Returns410Gone()
	{
		var response = await _client.GetAsync("/MyHandler.ashx");

		response.StatusCode.ShouldBe(HttpStatusCode.Gone);
	}

	[Fact]
	public async Task AshxRequest_ReturnsDescriptiveBody()
	{
		var response = await _client.GetAsync("/MyHandler.ashx");
		var body = await response.Content.ReadAsStringAsync();

		body.ShouldNotBeNullOrWhiteSpace();
	}

	[Fact]
	public async Task AshxRequest_WithQueryString_Returns410()
	{
		var response = await _client.GetAsync("/Export.ashx?format=csv&id=123");

		response.StatusCode.ShouldBe(HttpStatusCode.Gone);
	}

	[Theory]
	[InlineData("/MyHandler.ASHX")]
	[InlineData("/MyHandler.Ashx")]
	[InlineData("/MYHANDLER.ASHX")]
	public async Task AshxRequest_MixedCase_Returns410(string path)
	{
		var response = await _client.GetAsync(path);

		response.StatusCode.ShouldBe(HttpStatusCode.Gone);
	}

	[Fact]
	public async Task AshxRequest_InSubdirectory_Returns410()
	{
		var response = await _client.GetAsync("/api/handlers/Export.ashx");

		response.StatusCode.ShouldBe(HttpStatusCode.Gone);
	}

	[Fact]
	public async Task AshxRequest_WhenDisabled_PassesThrough()
	{
		var (server, client) = CreateServerAndClient(opts => opts.EnableAshxHandling = false);
		using (server)
		using (client)
		{
			var response = await client.GetAsync("/MyHandler.ashx");

			response.StatusCode.ShouldBe(HttpStatusCode.OK);
			var body = await response.Content.ReadAsStringAsync();
			body.ShouldBe("PASSTHROUGH");
		}
	}

	#endregion

	#region .ashx Custom Redirect Mappings

	[Fact]
	public async Task AshxRequest_WithCustomMapping_Returns301Redirect()
	{
		var (server, client) = CreateServerAndClient(opts =>
		{
			opts.AshxRedirectMappings["/MyHandler.ashx"] = "/api/my-handler";
		});
		using (server)
		using (client)
		{
			var response = await client.GetAsync("/MyHandler.ashx");

			response.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
		}
	}

	[Fact]
	public async Task AshxRequest_WithCustomMapping_SetsLocationHeader()
	{
		var (server, client) = CreateServerAndClient(opts =>
		{
			opts.AshxRedirectMappings["/Export.ashx"] = "/api/export";
		});
		using (server)
		using (client)
		{
			var response = await client.GetAsync("/Export.ashx");

			response.Headers.Location?.ToString().ShouldBe("/api/export");
		}
	}

	[Fact]
	public async Task AshxRequest_WithCustomMapping_CaseInsensitiveLookup()
	{
		var (server, client) = CreateServerAndClient(opts =>
		{
			opts.AshxRedirectMappings["/MyHandler.ashx"] = "/api/my-handler";
		});
		using (server)
		using (client)
		{
			// Request uses different casing than the mapping key
			var response = await client.GetAsync("/MYHANDLER.ASHX");

			response.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
			response.Headers.Location?.ToString().ShouldBe("/api/my-handler");
		}
	}

	[Fact]
	public async Task AshxRequest_UnmappedPath_Returns410EvenWhenMappingsExist()
	{
		var (server, client) = CreateServerAndClient(opts =>
		{
			opts.AshxRedirectMappings["/Mapped.ashx"] = "/api/mapped";
		});
		using (server)
		using (client)
		{
			var response = await client.GetAsync("/Unmapped.ashx");

			response.StatusCode.ShouldBe(HttpStatusCode.Gone);
		}
	}

	[Fact]
	public async Task AshxRequest_MultipleCustomMappings_EachWorks()
	{
		var (server, client) = CreateServerAndClient(opts =>
		{
			opts.AshxRedirectMappings["/Handler1.ashx"] = "/api/handler1";
			opts.AshxRedirectMappings["/Handler2.ashx"] = "/api/handler2";
		});
		using (server)
		using (client)
		{
			var response1 = await client.GetAsync("/Handler1.ashx");
			var response2 = await client.GetAsync("/Handler2.ashx");

			response1.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
			response1.Headers.Location?.ToString().ShouldBe("/api/handler1");
			response2.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
			response2.Headers.Location?.ToString().ShouldBe("/api/handler2");
		}
	}

	#endregion

	#region .axd Resource Interception

	[Theory]
	[InlineData("/WebResource.axd")]
	[InlineData("/ScriptResource.axd")]
	[InlineData("/Trace.axd")]
	public async Task AxdRequest_KnownResources_Returns404(string path)
	{
		var response = await _client.GetAsync(path);

		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task ChartImgAxd_Returns410Gone()
	{
		var response = await _client.GetAsync("/ChartImg.axd");

		response.StatusCode.ShouldBe(HttpStatusCode.Gone);
	}

	[Fact]
	public async Task AxdRequest_WithQueryString_Returns404()
	{
		var response = await _client.GetAsync("/WebResource.axd?d=abc123&t=636123456789");

		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task ChartImgAxd_WithQueryString_Returns410()
	{
		var response = await _client.GetAsync("/ChartImg.axd?i=chart_abc.png&g=123");

		response.StatusCode.ShouldBe(HttpStatusCode.Gone);
	}

	[Theory]
	[InlineData("/WEBRESOURCE.AXD")]
	[InlineData("/WebResource.Axd")]
	[InlineData("/webresource.axd")]
	public async Task AxdRequest_MixedCase_Returns404(string path)
	{
		var response = await _client.GetAsync(path);

		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	[Theory]
	[InlineData("/CHARTIMG.AXD")]
	[InlineData("/ChartImg.Axd")]
	[InlineData("/chartimg.axd")]
	public async Task ChartImgAxd_MixedCase_Returns410(string path)
	{
		var response = await _client.GetAsync(path);

		response.StatusCode.ShouldBe(HttpStatusCode.Gone);
	}

	[Fact]
	public async Task AxdRequest_UnknownHandler_Returns404()
	{
		// Unknown .axd handlers should still get 404 (not pass through)
		var response = await _client.GetAsync("/Custom.axd");

		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task AxdRequest_WhenDisabled_PassesThrough()
	{
		var (server, client) = CreateServerAndClient(opts => opts.EnableAxdHandling = false);
		using (server)
		using (client)
		{
			var response = await client.GetAsync("/WebResource.axd");

			response.StatusCode.ShouldBe(HttpStatusCode.OK);
			var body = await response.Content.ReadAsStringAsync();
			body.ShouldBe("PASSTHROUGH");
		}
	}

	[Fact]
	public async Task ChartImgAxd_WhenDisabled_PassesThrough()
	{
		var (server, client) = CreateServerAndClient(opts => opts.EnableAxdHandling = false);
		using (server)
		using (client)
		{
			var response = await client.GetAsync("/ChartImg.axd");

			response.StatusCode.ShouldBe(HttpStatusCode.OK);
			var body = await response.Content.ReadAsStringAsync();
			body.ShouldBe("PASSTHROUGH");
		}
	}

	#endregion

	#region Edge Cases

	[Fact]
	public async Task PathContainingAshxSubstring_PassesThrough()
	{
		// "/flashx-content" contains "ashx" as a substring but doesn't end with .ashx
		var response = await _client.GetAsync("/flashx-content");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact]
	public async Task PathContainingAxdSubstring_PassesThrough()
	{
		// "/taxdata" contains "axd" as a substring but doesn't end with .axd
		var response = await _client.GetAsync("/taxdata");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact]
	public async Task RegularHtmlPath_PassesThrough()
	{
		var response = await _client.GetAsync("/index.html");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact]
	public async Task ApiPath_PassesThrough()
	{
		var response = await _client.GetAsync("/api/products/42");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact]
	public async Task RootPath_PassesThrough()
	{
		var response = await _client.GetAsync("/");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact]
	public async Task AllHandlersDisabled_EverythingPassesThrough()
	{
		var (server, client) = CreateServerAndClient(opts =>
		{
			opts.EnableAspxUrlRewriting = false;
			opts.EnableAshxHandling = false;
			opts.EnableAxdHandling = false;
		});
		using (server)
		using (client)
		{
			var aspxResponse = await client.GetAsync("/page.aspx");
			var ashxResponse = await client.GetAsync("/handler.ashx");
			var axdResponse = await client.GetAsync("/WebResource.axd");

			aspxResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
			ashxResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
			axdResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
		}
	}

	[Fact]
	public async Task AshxFileNameOnly_Returns410()
	{
		var response = await _client.GetAsync("/Handler.ashx");

		response.StatusCode.ShouldBe(HttpStatusCode.Gone);
	}

	[Fact]
	public async Task DotAshxInMiddleOfPath_PassesThrough()
	{
		// Path has .ashx but it's not the final extension
		var response = await _client.GetAsync("/files/report.ashx.bak");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact]
	public async Task DotAxdInMiddleOfPath_PassesThrough()
	{
		var response = await _client.GetAsync("/files/WebResource.axd.old");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	[Fact]
	public async Task AshxCustomMapping_PreservesQueryString()
	{
		var (server, client) = CreateServerAndClient(opts =>
		{
			opts.AshxRedirectMappings["/Export.ashx"] = "/api/export";
		});
		using (server)
		using (client)
		{
			var response = await client.GetAsync("/Export.ashx?format=csv&id=42");

			response.StatusCode.ShouldBe(HttpStatusCode.MovedPermanently);
			response.Headers.Location?.ToString().ShouldBe("/api/export?format=csv&id=42");
		}
	}

	[Fact]
	public async Task ChartImgAxd_InSubdirectory_Returns410()
	{
		var response = await _client.GetAsync("/app/ChartImg.axd");

		response.StatusCode.ShouldBe(HttpStatusCode.Gone);
	}

	[Fact]
	public async Task AxdRequest_ReturnsNoBodyFor404()
	{
		var response = await _client.GetAsync("/WebResource.axd");
		var body = await response.Content.ReadAsStringAsync();

		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
		body.ShouldBeEmpty();
	}

	[Fact]
	public async Task ChartImgAxd_ReturnsDescriptiveBody()
	{
		var response = await _client.GetAsync("/ChartImg.axd");
		var body = await response.Content.ReadAsStringAsync();

		response.StatusCode.ShouldBe(HttpStatusCode.Gone);
		body.ShouldNotBeNullOrWhiteSpace();
	}

	#endregion
}

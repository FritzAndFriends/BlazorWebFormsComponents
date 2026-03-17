using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test.Handlers;

/// <summary>
/// Integration tests for <see cref="HandlerEndpointExtensions"/> using TestServer.
/// Validates MapHandler routing, convention routes, HTTP method handling,
/// session pre-load, and endpoint chaining.
/// </summary>
public class HandlerEndpointExtensionTests : IDisposable
{
	#region Test handler implementations

	private class TestEchoHandler : HttpHandlerBase
	{
		public override async Task ProcessRequestAsync(HttpHandlerContext context)
		{
			context.Response.ContentType = "text/plain";
			var method = context.Request.HttpMethod;
			var name = context.Request.QueryString["name"] ?? "none";
			await context.Response.WriteAsync($"Method={method}, Name={name}");
		}
	}

	private class TestJsonHandler : HttpHandlerBase
	{
		public override async Task ProcessRequestAsync(HttpHandlerContext context)
		{
			context.Response.ContentType = "application/json";
			await context.Response.WriteAsync("{\"status\":\"ok\"}");
		}
	}

	private class TestBinaryHandler : HttpHandlerBase
	{
		public override async Task ProcessRequestAsync(HttpHandlerContext context)
		{
			context.Response.ContentType = "application/octet-stream";
			await context.Response.BinaryWriteAsync(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F });
		}
	}

	[RequiresSessionState]
	private class TestSessionHandler : HttpHandlerBase
	{
		public override async Task ProcessRequestAsync(HttpHandlerContext context)
		{
			context.Response.ContentType = "text/plain";
			context.Session.SetString("test-key", "test-value");
			var value = context.Session.GetString("test-key");
			await context.Response.WriteAsync($"session={value}");
		}
	}

	/// <summary>Convention: FooBarHandler → /FooBar.ashx</summary>
	private class FooBarHandler : HttpHandlerBase
	{
		public override async Task ProcessRequestAsync(HttpHandlerContext context)
		{
			await context.Response.WriteAsync("foobar");
		}
	}

	/// <summary>Convention: DataExportHandler → /DataExport.ashx</summary>
	private class DataExportHandler : HttpHandlerBase
	{
		public override async Task ProcessRequestAsync(HttpHandlerContext context)
		{
			await context.Response.WriteAsync("export");
		}
	}

	/// <summary>Name has no "Handler" suffix — should map to /Processor.ashx</summary>
	private class Processor : HttpHandlerBase
	{
		public override async Task ProcessRequestAsync(HttpHandlerContext context)
		{
			await context.Response.WriteAsync("processed");
		}
	}

	private class TestWriteSyncHandler : HttpHandlerBase
	{
		public override Task ProcessRequestAsync(HttpHandlerContext context)
		{
			context.Response.ContentType = "text/plain";
			context.Response.Write("sync-write");
			return Task.CompletedTask;
		}
	}

	private class TestContentTypeHandler : HttpHandlerBase
	{
		public override async Task ProcessRequestAsync(HttpHandlerContext context)
		{
			context.Response.ContentType = "text/xml";
			await context.Response.WriteAsync("<root/>");
		}
	}

	private class IsReusableTestHandler : HttpHandlerBase
	{
		public override Task ProcessRequestAsync(HttpHandlerContext context)
		{
			return Task.CompletedTask;
		}
	}

	#endregion

	private TestServer _server;
	private HttpClient _client;

	private void CreateServer(
		Action<IEndpointRouteBuilder> configureEndpoints,
		bool useSession = false)
	{
		var builder = new WebHostBuilder()
			.ConfigureServices(services =>
			{
				services.AddRouting();
				if (useSession)
				{
					services.AddDistributedMemoryCache();
					services.AddSession();
				}
			})
			.Configure(app =>
			{
				app.UseRouting();
				if (useSession)
				{
					app.UseSession();
				}
				app.UseEndpoints(configureEndpoints);
			});

		_server = new TestServer(builder);
		_client = _server.CreateClient();
	}

	public void Dispose()
	{
		_client?.Dispose();
		_server?.Dispose();
	}

	#region Explicit path routing

	[Fact]
	public async Task MapHandler_WithExplicitPath_RoutesCorrectly()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<TestJsonHandler>("/api/data.ashx");
		});

		var response = await _client.GetAsync("/api/data.ashx");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		var body = await response.Content.ReadAsStringAsync();
		body.ShouldBe("{\"status\":\"ok\"}");
	}

	[Fact]
	public async Task MapHandler_WithExplicitPath_Returns404ForWrongPath()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<TestJsonHandler>("/api/data.ashx");
		});

		var response = await _client.GetAsync("/wrong/path");

		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	#endregion

	#region Convention routing

	[Fact]
	public async Task MapHandler_Convention_FooBarHandler_MapsToFooBarAshx()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<FooBarHandler>();
		});

		var response = await _client.GetAsync("/FooBar.ashx");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		var body = await response.Content.ReadAsStringAsync();
		body.ShouldBe("foobar");
	}

	[Fact]
	public async Task MapHandler_Convention_DataExportHandler_MapsToDataExportAshx()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<DataExportHandler>();
		});

		var response = await _client.GetAsync("/DataExport.ashx");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		var body = await response.Content.ReadAsStringAsync();
		body.ShouldBe("export");
	}

	[Fact]
	public async Task MapHandler_Convention_NoHandlerSuffix_UsesFullName()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<Processor>();
		});

		var response = await _client.GetAsync("/Processor.ashx");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		var body = await response.Content.ReadAsStringAsync();
		body.ShouldBe("processed");
	}

	#endregion

	#region Multiple paths

	[Fact]
	public async Task MapHandler_WithMultiplePaths_AllRoutesWork()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<TestJsonHandler>("/legacy/data.ashx", "/api/data");
		});

		var response1 = await _client.GetAsync("/legacy/data.ashx");
		var response2 = await _client.GetAsync("/api/data");

		response1.StatusCode.ShouldBe(HttpStatusCode.OK);
		response2.StatusCode.ShouldBe(HttpStatusCode.OK);

		var body1 = await response1.Content.ReadAsStringAsync();
		var body2 = await response2.Content.ReadAsStringAsync();
		body1.ShouldBe("{\"status\":\"ok\"}");
		body2.ShouldBe("{\"status\":\"ok\"}");
	}

	#endregion

	#region Handler receives correct context

	[Fact]
	public async Task Handler_ReceivesQueryStringParameters()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<TestEchoHandler>("/echo.ashx");
		});

		var response = await _client.GetAsync("/echo.ashx?name=alice");

		var body = await response.Content.ReadAsStringAsync();
		body.ShouldContain("Name=alice");
	}

	[Fact]
	public async Task Handler_ReceivesCorrectHttpMethod_Get()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<TestEchoHandler>("/echo.ashx");
		});

		var response = await _client.GetAsync("/echo.ashx");

		var body = await response.Content.ReadAsStringAsync();
		body.ShouldContain("Method=GET");
	}

	[Fact]
	public async Task Handler_ReceivesCorrectHttpMethod_Post()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<TestEchoHandler>("/echo.ashx");
		});

		var response = await _client.PostAsync("/echo.ashx?name=bob", null);

		var body = await response.Content.ReadAsStringAsync();
		body.ShouldContain("Method=POST");
		body.ShouldContain("Name=bob");
	}

	#endregion

	#region All HTTP methods route to handler

	[Theory]
	[InlineData("GET")]
	[InlineData("POST")]
	[InlineData("PUT")]
	[InlineData("DELETE")]
	public async Task MapHandler_AllHttpMethods_RouteToSameHandler(string method)
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<TestEchoHandler>("/echo.ashx");
		});

		var request = new HttpRequestMessage(new HttpMethod(method), "/echo.ashx?name=test");
		var response = await _client.SendAsync(request);

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		var body = await response.Content.ReadAsStringAsync();
		body.ShouldContain($"Method={method}");
	}

	#endregion

	#region Response capabilities

	[Fact]
	public async Task Handler_CanWriteResponse()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<TestWriteSyncHandler>("/write.ashx");
		});

		var response = await _client.GetAsync("/write.ashx");
		var body = await response.Content.ReadAsStringAsync();

		body.ShouldBe("sync-write");
	}

	[Fact]
	public async Task Handler_CanWriteBinaryResponse()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<TestBinaryHandler>("/binary.ashx");
		});

		var response = await _client.GetAsync("/binary.ashx");
		var bytes = await response.Content.ReadAsByteArrayAsync();

		bytes.ShouldBe(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F });
	}

	[Fact]
	public async Task Handler_CanSetContentType()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<TestContentTypeHandler>("/content.ashx");
		});

		var response = await _client.GetAsync("/content.ashx");

		response.Content.Headers.ContentType!.MediaType.ShouldBe("text/xml");
	}

	[Fact]
	public async Task Handler_JsonContentType_IsSetCorrectly()
	{
		CreateServer(endpoints =>
		{
			endpoints.MapHandler<TestJsonHandler>("/json.ashx");
		});

		var response = await _client.GetAsync("/json.ashx");

		response.Content.Headers.ContentType!.MediaType.ShouldBe("application/json");
	}

	#endregion

	#region Session

	[Fact]
	public async Task RequiresSessionState_TriggersSessionLoad()
	{
		CreateServer(
			endpoints =>
			{
				endpoints.MapHandler<TestSessionHandler>("/session.ashx");
			},
			useSession: true);

		var response = await _client.GetAsync("/session.ashx");

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		var body = await response.Content.ReadAsStringAsync();
		body.ShouldBe("session=test-value");
	}

	#endregion

	#region Endpoint chaining

	[Fact]
	public async Task MapHandler_ReturnsEndpointConventionBuilder_ForChaining()
	{
		// Verify that MapHandler returns an IEndpointConventionBuilder
		// that supports chaining methods like RequireAuthorization
		CreateServer(endpoints =>
		{
			var conventionBuilder = endpoints.MapHandler<TestJsonHandler>("/chained.ashx");

			// Should be able to chain — if this compiles and doesn't throw, chaining works
			conventionBuilder.ShouldNotBeNull();
		});

		var response = await _client.GetAsync("/chained.ashx");
		response.StatusCode.ShouldBe(HttpStatusCode.OK);
	}

	#endregion

	#region IsReusable

	[Fact]
	public void IsReusable_DefaultIsFalse()
	{
		var handler = new IsReusableTestHandler();

		handler.IsReusable.ShouldBeFalse();
	}

	#endregion

	#region RequiresSessionStateAttribute

	[Fact]
	public void RequiresSessionStateAttribute_CanBeAppliedToClass()
	{
		var attr = Attribute.GetCustomAttribute(
			typeof(TestSessionHandler),
			typeof(RequiresSessionStateAttribute));

		attr.ShouldNotBeNull();
	}

	[Fact]
	public void RequiresSessionStateAttribute_IsInheritable()
	{
		var attrUsage = Attribute.GetCustomAttribute(
			typeof(RequiresSessionStateAttribute),
			typeof(AttributeUsageAttribute)) as AttributeUsageAttribute;

		attrUsage.ShouldNotBeNull();
		attrUsage!.Inherited.ShouldBeTrue();
	}

	[Fact]
	public void RequiresSessionStateAttribute_NotAllowMultiple()
	{
		var attrUsage = Attribute.GetCustomAttribute(
			typeof(RequiresSessionStateAttribute),
			typeof(AttributeUsageAttribute)) as AttributeUsageAttribute;

		attrUsage.ShouldNotBeNull();
		attrUsage!.AllowMultiple.ShouldBeFalse();
	}

	#endregion
}

using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test.Handlers;

/// <summary>
/// Unit tests for <see cref="HttpHandlerContext"/> — the Web Forms-compatible
/// context wrapper that assembles Request, Response, Server, Session, User, and Items.
/// </summary>
public class HttpHandlerContextTests
{
	private static (HttpHandlerContext context, DefaultHttpContext httpContext) CreateContext()
	{
		var httpContext = new DefaultHttpContext();
		var env = new Mock<IWebHostEnvironment>();
		env.Setup(e => e.WebRootPath).Returns(@"C:\wwwroot");
		env.Setup(e => e.ContentRootPath).Returns(@"C:\app");
		var ctx = new HttpHandlerContext(httpContext, env.Object);
		return (ctx, httpContext);
	}

	[Fact]
	public void Request_IsNotNull()
	{
		var (context, _) = CreateContext();

		context.Request.ShouldNotBeNull();
	}

	[Fact]
	public void Response_IsNotNull()
	{
		var (context, _) = CreateContext();

		context.Response.ShouldNotBeNull();
	}

	[Fact]
	public void Server_IsNotNull()
	{
		var (context, _) = CreateContext();

		context.Server.ShouldNotBeNull();
	}

	[Fact]
	public void User_ReturnsHttpContextUser()
	{
		var (context, httpContext) = CreateContext();

		context.User.ShouldBe(httpContext.User);
	}

	[Fact]
	public void Items_ReturnsHttpContextItems()
	{
		var (context, httpContext) = CreateContext();
		httpContext.Items["key"] = "value";

		context.Items["key"].ShouldBe("value");
	}

	[Fact]
	public void Items_AreMutableThroughContext()
	{
		var (context, httpContext) = CreateContext();

		context.Items["test"] = 42;

		httpContext.Items["test"].ShouldBe(42);
	}

	[Fact]
	public void Session_ThrowsWhenMiddlewareNotConfigured()
	{
		var (context, _) = CreateContext();

		// Accessing Session without session middleware throws InvalidOperationException
		Should.Throw<InvalidOperationException>(() =>
		{
			var _ = context.Session;
		});
	}

	[Fact]
	public void Request_WrapsCorrectHttpRequest()
	{
		var (context, httpContext) = CreateContext();
		httpContext.Request.Method = "DELETE";

		context.Request.HttpMethod.ShouldBe("DELETE");
	}

	[Fact]
	public void Response_WrapsCorrectHttpResponse()
	{
		var (context, httpContext) = CreateContext();

		context.Response.StatusCode = 418;

		httpContext.Response.StatusCode.ShouldBe(418);
	}
}

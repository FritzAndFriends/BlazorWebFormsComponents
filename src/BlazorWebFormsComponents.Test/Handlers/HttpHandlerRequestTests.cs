using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test.Handlers;

/// <summary>
/// Unit tests for <see cref="HttpHandlerRequest"/> — the Web Forms-compatible
/// request adapter wrapping ASP.NET Core's <see cref="HttpRequest"/>.
/// </summary>
public class HttpHandlerRequestTests
{
	private static HttpHandlerRequest CreateRequest(DefaultHttpContext httpContext)
	{
		var env = new Mock<IWebHostEnvironment>();
		env.Setup(e => e.WebRootPath).Returns("C:\\wwwroot");
		env.Setup(e => e.ContentRootPath).Returns("C:\\app");
		var ctx = new HttpHandlerContext(httpContext, env.Object);
		return ctx.Request;
	}

	#region QueryString

	[Fact]
	public void QueryString_ParsesSingleParameter()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.QueryString = new QueryString("?name=alice");

		var request = CreateRequest(httpContext);

		request.QueryString["name"].ShouldBe("alice");
	}

	[Fact]
	public void QueryString_ParsesMultipleParameters()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.QueryString = new QueryString("?a=1&b=2&c=3");

		var request = CreateRequest(httpContext);

		request.QueryString["a"].ShouldBe("1");
		request.QueryString["b"].ShouldBe("2");
		request.QueryString["c"].ShouldBe("3");
	}

	[Fact]
	public void QueryString_ReturnsNullForMissingKey()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.QueryString = new QueryString("?name=alice");

		var request = CreateRequest(httpContext);

		request.QueryString["missing"].ShouldBeNull();
	}

	[Fact]
	public void QueryString_HandlesEmptyQueryString()
	{
		var httpContext = new DefaultHttpContext();

		var request = CreateRequest(httpContext);

		request.QueryString.Count.ShouldBe(0);
	}

	[Fact]
	public void QueryString_HandlesEncodedValues()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.QueryString = new QueryString("?q=hello+world&path=%2Ffoo%2Fbar");

		var request = CreateRequest(httpContext);

		request.QueryString["q"].ShouldBe("hello world");
		request.QueryString["path"].ShouldBe("/foo/bar");
	}

	#endregion

	#region Form

	[Fact]
	public void Form_ReturnsEmptyCollectionWhenNoFormContentType()
	{
		var httpContext = new DefaultHttpContext();

		var request = CreateRequest(httpContext);

		request.Form.Count.ShouldBe(0);
	}

	#endregion

	#region Indexer

	[Fact]
	public void Indexer_ReturnsQueryStringValueFirst()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.QueryString = new QueryString("?key=from-qs");

		var request = CreateRequest(httpContext);

		request["key"].ShouldBe("from-qs");
	}

	[Fact]
	public void Indexer_ReturnsNullWhenKeyNotFound()
	{
		var httpContext = new DefaultHttpContext();

		var request = CreateRequest(httpContext);

		request["missing"].ShouldBeNull();
	}

	#endregion

	#region Headers

	[Fact]
	public void Headers_ReturnsRequestHeaders()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Headers["X-Custom"] = "test-value";

		var request = CreateRequest(httpContext);

		request.Headers["X-Custom"].ShouldBe("test-value");
	}

	[Fact]
	public void Headers_ReturnsMultipleHeaders()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Headers["Accept"] = "text/html";
		httpContext.Request.Headers["X-Request-Id"] = "abc-123";

		var request = CreateRequest(httpContext);

		request.Headers["Accept"].ShouldBe("text/html");
		request.Headers["X-Request-Id"].ShouldBe("abc-123");
	}

	#endregion

	#region HttpMethod / ContentType

	[Fact]
	public void HttpMethod_ReturnsRequestMethod()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Method = "POST";

		var request = CreateRequest(httpContext);

		request.HttpMethod.ShouldBe("POST");
	}

	[Fact]
	public void HttpMethod_ReturnsHttpMethod()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Method = "POST";

		var request = CreateRequest(httpContext);

		request.HttpMethod.ShouldBe("POST");
	}

	[Fact]
	public void ContentType_ReturnsRequestContentType()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.ContentType = "application/json";

		var request = CreateRequest(httpContext);

		request.ContentType.ShouldBe("application/json");
	}

	#endregion

	#region Url / RawUrl

	[Fact]
	public void Url_ReturnsFullUri()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Scheme = "https";
		httpContext.Request.Host = new HostString("example.com");
		httpContext.Request.Path = "/test.ashx";
		httpContext.Request.QueryString = new QueryString("?id=42");

		var request = CreateRequest(httpContext);

		request.Url.ShouldBe(new Uri("https://example.com/test.ashx?id=42"));
	}

	[Fact]
	public void Url_IncludesPathBase()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Scheme = "http";
		httpContext.Request.Host = new HostString("localhost");
		httpContext.Request.PathBase = "/app";
		httpContext.Request.Path = "/test.ashx";

		var request = CreateRequest(httpContext);

		request.Url.ToString().ShouldContain("/app/test.ashx");
	}

	[Fact]
	public void RawUrl_ReturnsPathAndQueryString()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Path = "/test.ashx";
		httpContext.Request.QueryString = new QueryString("?id=42");

		var request = CreateRequest(httpContext);

		request.RawUrl.ShouldBe("/test.ashx?id=42");
	}

	[Fact]
	public void RawUrl_IncludesPathBase()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.PathBase = "/app";
		httpContext.Request.Path = "/test.ashx";

		var request = CreateRequest(httpContext);

		request.RawUrl.ShouldBe("/app/test.ashx");
	}

	[Fact]
	public void RawUrl_WithNoQueryString_ReturnsPathOnly()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Path = "/handler.ashx";

		var request = CreateRequest(httpContext);

		request.RawUrl.ShouldBe("/handler.ashx");
	}

	#endregion

	#region IsSecureConnection

	[Fact]
	public void IsSecureConnection_TrueForHttps()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Scheme = "https";

		var request = CreateRequest(httpContext);

		request.IsSecureConnection.ShouldBeTrue();
	}

	[Fact]
	public void IsSecureConnection_FalseForHttp()
	{
		var httpContext = new DefaultHttpContext();
		httpContext.Request.Scheme = "http";

		var request = CreateRequest(httpContext);

		request.IsSecureConnection.ShouldBeFalse();
	}

	#endregion

	#region InputStream / Cookies / Files

	[Fact]
	public void InputStream_ReturnsRequestBody()
	{
		var httpContext = new DefaultHttpContext();

		var request = CreateRequest(httpContext);

		request.InputStream.ShouldBe(httpContext.Request.Body);
	}

	[Fact]
	public void Cookies_ReturnsRequestCookies()
	{
		var httpContext = new DefaultHttpContext();

		var request = CreateRequest(httpContext);

		request.Cookies.ShouldNotBeNull();
	}

	[Fact]
	public void Files_ReturnsEmptyCollectionWhenNoFormContentType()
	{
		var httpContext = new DefaultHttpContext();

		var request = CreateRequest(httpContext);

		request.Files.Count.ShouldBe(0);
	}

	#endregion
}

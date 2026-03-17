using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test.Handlers;

/// <summary>
/// Unit tests for <see cref="HttpHandlerResponse"/> — the Web Forms-compatible
/// response adapter wrapping ASP.NET Core's <see cref="HttpResponse"/>.
/// </summary>
public class HttpHandlerResponseTests
{
	private static (HttpHandlerResponse response, DefaultHttpContext httpContext, MemoryStream body) CreateResponse()
	{
		var httpContext = new DefaultHttpContext();
		var body = new MemoryStream();
		httpContext.Response.Body = body;

		var env = new Mock<IWebHostEnvironment>();
		env.Setup(e => e.WebRootPath).Returns("C:\\wwwroot");
		env.Setup(e => e.ContentRootPath).Returns("C:\\app");

		var ctx = new HttpHandlerContext(httpContext, env.Object);
		return (ctx.Response, httpContext, body);
	}

	#region Write

	[Fact]
	public void Write_WritesTextToResponseBody()
	{
		var (response, _, body) = CreateResponse();

		response.Write("Hello World");

		body.Position = 0;
		var text = new StreamReader(body).ReadToEnd();
		text.ShouldBe("Hello World");
	}

	[Fact]
	public void Write_MultipleCalls_AppendsText()
	{
		var (response, _, body) = CreateResponse();

		response.Write("Hello");
		response.Write(" World");

		body.Position = 0;
		var text = new StreamReader(body).ReadToEnd();
		text.ShouldBe("Hello World");
	}

	[Fact]
	public async Task WriteAsync_WritesTextToResponseBody()
	{
		var (response, _, body) = CreateResponse();

		await response.WriteAsync("async hello");

		body.Position = 0;
		var text = new StreamReader(body).ReadToEnd();
		text.ShouldBe("async hello");
	}

	#endregion

	#region BinaryWrite

	[Fact]
	public void BinaryWrite_WritesBytesToResponseBody()
	{
		var (response, _, body) = CreateResponse();
		var data = new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F };

		response.BinaryWrite(data);

		body.Position = 0;
		var result = body.ToArray();
		result.ShouldBe(data);
	}

	[Fact]
	public async Task BinaryWriteAsync_WritesBytesToResponseBody()
	{
		var (response, _, body) = CreateResponse();
		var data = new byte[] { 1, 2, 3, 4, 5 };

		await response.BinaryWriteAsync(data);

		body.Position = 0;
		var result = body.ToArray();
		result.ShouldBe(data);
	}

	#endregion

	#region ContentType

	[Fact]
	public void ContentType_GetSet()
	{
		var (response, _, _) = CreateResponse();

		response.ContentType = "application/json";

		response.ContentType.ShouldBe("application/json");
	}

	[Fact]
	public void ContentType_CanBeChangedMultipleTimes()
	{
		var (response, _, _) = CreateResponse();

		response.ContentType = "text/plain";
		response.ContentType = "application/json";

		response.ContentType.ShouldBe("application/json");
	}

	#endregion

	#region StatusCode

	[Fact]
	public void StatusCode_DefaultIs200()
	{
		var (response, _, _) = CreateResponse();

		response.StatusCode.ShouldBe(200);
	}

	[Fact]
	public void StatusCode_GetSet()
	{
		var (response, _, _) = CreateResponse();

		response.StatusCode = 404;

		response.StatusCode.ShouldBe(404);
	}

	[Fact]
	public void StatusCode_CanBeSetTo500()
	{
		var (response, _, _) = CreateResponse();

		response.StatusCode = 500;

		response.StatusCode.ShouldBe(500);
	}

	#endregion

	#region Headers

	[Fact]
	public void AddHeader_AppendsHeader()
	{
		var (response, httpContext, _) = CreateResponse();

		response.AddHeader("X-Custom", "value1");

		httpContext.Response.Headers["X-Custom"].ToString().ShouldBe("value1");
	}

	[Fact]
	public void AppendHeader_AppendsMultipleValues()
	{
		var (response, httpContext, _) = CreateResponse();

		response.AppendHeader("X-Custom", "value1");
		response.AppendHeader("X-Custom", "value2");

		httpContext.Response.Headers["X-Custom"].Count.ShouldBe(2);
	}

	[Fact]
	public void AddHeader_AndAppendHeader_AreFunctionallyEquivalent()
	{
		var (response, httpContext, _) = CreateResponse();

		response.AddHeader("X-First", "a");
		response.AppendHeader("X-Second", "b");

		httpContext.Response.Headers["X-First"].ToString().ShouldBe("a");
		httpContext.Response.Headers["X-Second"].ToString().ShouldBe("b");
	}

	#endregion

	#region Redirect

	[Fact]
	public void Redirect_SetsLocationHeaderAndStatusCode()
	{
		var (response, httpContext, _) = CreateResponse();

		response.Redirect("/new-location");

		httpContext.Response.Headers["Location"].ToString().ShouldBe("/new-location");
		httpContext.Response.StatusCode.ShouldBe(302);
	}

	[Fact]
	public void Redirect_SetsAbsoluteUrl()
	{
		var (response, httpContext, _) = CreateResponse();

		response.Redirect("https://example.com/page");

		httpContext.Response.Headers["Location"].ToString().ShouldBe("https://example.com/page");
	}

	#endregion

	#region Clear

	[Fact]
	public void Clear_ResetsStatusCodeTo200()
	{
		var (response, _, _) = CreateResponse();
		response.StatusCode = 500;

		response.Clear();

		response.StatusCode.ShouldBe(200);
	}

	[Fact]
	public void Clear_ClearsHeaders()
	{
		var (response, httpContext, _) = CreateResponse();
		response.AddHeader("X-Custom", "value");

		response.Clear();

		httpContext.Response.Headers.ContainsKey("X-Custom").ShouldBeFalse();
	}

	#endregion

	#region End

	[Fact]
	public void End_SetsIsEndedFlag()
	{
		var (response, _, _) = CreateResponse();
		response.IsEnded.ShouldBeFalse();

#pragma warning disable CS0618
		response.End();
#pragma warning restore CS0618

		response.IsEnded.ShouldBeTrue();
	}

	[Fact]
	public void End_IsMarkedObsolete()
	{
		var method = typeof(HttpHandlerResponse).GetMethod("End");
		var attrs = method!.GetCustomAttributes(typeof(ObsoleteAttribute), false);

		attrs.Length.ShouldBe(1);
	}

	[Fact]
	public void IsEnded_DefaultIsFalse()
	{
		var (response, _, _) = CreateResponse();

		response.IsEnded.ShouldBeFalse();
	}

	#endregion

	#region Flush

	[Fact]
	public void Flush_DoesNotThrow()
	{
		var (response, _, _) = CreateResponse();

		Should.NotThrow(() => response.Flush());
	}

	[Fact]
	public async Task FlushAsync_DoesNotThrow()
	{
		var (response, _, _) = CreateResponse();

		await Should.NotThrowAsync(() => response.FlushAsync());
	}

	#endregion

	#region OutputStream

	[Fact]
	public void OutputStream_ReturnsResponseBody()
	{
		var (response, _, body) = CreateResponse();

		response.OutputStream.ShouldBe(body);
	}

	#endregion
}

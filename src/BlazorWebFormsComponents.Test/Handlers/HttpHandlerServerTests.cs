using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test.Handlers;

/// <summary>
/// Unit tests for <see cref="HttpHandlerServer"/> — the Web Forms-compatible
/// server utilities adapter wrapping <see cref="IWebHostEnvironment"/>.
/// </summary>
public class HttpHandlerServerTests
{
	private static HttpHandlerServer CreateServer(
		string webRootPath = @"C:\wwwroot",
		string contentRootPath = @"C:\app")
	{
		var httpContext = new DefaultHttpContext();
		var env = new Mock<IWebHostEnvironment>();
		env.Setup(e => e.WebRootPath).Returns(webRootPath);
		env.Setup(e => e.ContentRootPath).Returns(contentRootPath);
		var ctx = new HttpHandlerContext(httpContext, env.Object);
		return ctx.Server;
	}

	#region MapPath

	[Fact]
	public void MapPath_WithTildeSlash_ResolvesRelativeToWebRoot()
	{
		var server = CreateServer(webRootPath: @"C:\wwwroot");

		var result = server.MapPath("~/images/logo.png");

		result.ShouldBe(Path.Combine(@"C:\wwwroot", "images", "logo.png"));
	}

	[Fact]
	public void MapPath_WithoutTildeSlash_ResolvesRelativeToContentRoot()
	{
		var server = CreateServer(contentRootPath: @"C:\app");

		var result = server.MapPath("/data/file.xml");

		result.ShouldBe(Path.Combine(@"C:\app", "data", "file.xml"));
	}

	[Fact]
	public void MapPath_ConvertsForwardSlashesToDirectorySeparator()
	{
		var server = CreateServer(webRootPath: @"C:\wwwroot");

		var result = server.MapPath("~/css/site/main.css");

		// On Windows, should use backslashes
		result.ShouldContain(Path.DirectorySeparatorChar.ToString());
	}

	[Fact]
	public void MapPath_TildeSlash_StripsPrefix()
	{
		var server = CreateServer(webRootPath: @"C:\wwwroot");

		var result = server.MapPath("~/file.txt");

		result.ShouldBe(Path.Combine(@"C:\wwwroot", "file.txt"));
	}

	[Fact]
	public void MapPath_LeadingSlash_IsStripped()
	{
		var server = CreateServer(contentRootPath: @"C:\app");

		var result = server.MapPath("/subfolder/data.json");

		result.ShouldBe(Path.Combine(@"C:\app", "subfolder", "data.json"));
	}

	[Fact]
	public void MapPath_NestedPath_ResolvesCorrectly()
	{
		var server = CreateServer(webRootPath: @"C:\wwwroot");

		var result = server.MapPath("~/css/themes/dark/main.css");

		result.ShouldBe(Path.Combine(@"C:\wwwroot", "css", "themes", "dark", "main.css"));
	}

	#endregion

	#region HtmlEncode / HtmlDecode

	[Fact]
	public void HtmlEncode_EncodesAngleBrackets()
	{
		var server = CreateServer();

		var result = server.HtmlEncode("<script>alert('xss')</script>");

		result.ShouldContain("&lt;");
		result.ShouldContain("&gt;");
		result.ShouldNotContain("<script>");
	}

	[Fact]
	public void HtmlDecode_DecodesEntities()
	{
		var server = CreateServer();

		var result = server.HtmlDecode("&lt;b&gt;bold&lt;/b&gt;");

		result.ShouldBe("<b>bold</b>");
	}

	[Fact]
	public void HtmlEncode_ThenDecode_RoundTrips()
	{
		var server = CreateServer();
		var original = "<div class=\"test\">&amp;</div>";

		var encoded = server.HtmlEncode(original);
		var decoded = server.HtmlDecode(encoded);

		decoded.ShouldBe(original);
	}

	[Fact]
	public void HtmlEncode_HandlesAmpersand()
	{
		var server = CreateServer();

		var result = server.HtmlEncode("A & B");

		result.ShouldContain("&amp;");
	}

	#endregion

	#region UrlEncode / UrlDecode

	[Fact]
	public void UrlEncode_EncodesSpacesAndSpecialChars()
	{
		var server = CreateServer();

		var result = server.UrlEncode("hello world&foo=bar");

		result.ShouldContain("+");
		result.ShouldContain("%26");
	}

	[Fact]
	public void UrlDecode_DecodesEncodedString()
	{
		var server = CreateServer();

		var result = server.UrlDecode("hello+world%26foo%3Dbar");

		result.ShouldBe("hello world&foo=bar");
	}

	[Fact]
	public void UrlEncode_ThenDecode_RoundTrips()
	{
		var server = CreateServer();
		var original = "query=hello world&page=1";

		var encoded = server.UrlEncode(original);
		var decoded = server.UrlDecode(encoded);

		decoded.ShouldBe(original);
	}

	#endregion

	#region Transfer

	[Fact]
	public void Transfer_ThrowsNotSupportedException()
	{
		var server = CreateServer();

		Should.Throw<NotSupportedException>(() => server.Transfer("/other.ashx"));
	}

	[Fact]
	public void Transfer_IncludesPathInExceptionMessage()
	{
		var server = CreateServer();

		var ex = Should.Throw<NotSupportedException>(() => server.Transfer("/target.ashx"));

		ex.Message.ShouldContain("/target.ashx");
	}

	[Fact]
	public void Transfer_IncludesMigrationGuidance()
	{
		var server = CreateServer();

		var ex = Should.Throw<NotSupportedException>(() => server.Transfer("/any.ashx"));

		ex.Message.ShouldContain("Response.Redirect");
		ex.Message.ShouldContain("service class");
	}

	#endregion
}

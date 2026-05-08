using System;
using System.Reflection;
using Shouldly;
using Xunit;

namespace BlazorWebFormsComponents.Test;

public class HttpUtilityTests
{
	private static Type HttpUtilityType => typeof(Unit).Assembly.GetType("System.Web.HttpUtility", throwOnError: true)!;

	private static string? Invoke(string methodName, string value)
		=> (string?)HttpUtilityType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)!.Invoke(null, [value]);

	[Fact]
	public void UrlEncode_EncodesSpacesAndReservedCharacters()
	{
		var encoded = Invoke("UrlEncode", "hello world&foo=bar");

		encoded.ShouldBe("hello+world%26foo%3Dbar");
	}

	[Fact]
	public void UrlDecode_DecodesEncodedString()
	{
		var decoded = Invoke("UrlDecode", "hello+world%26foo%3Dbar");

		decoded.ShouldBe("hello world&foo=bar");
	}

	[Fact]
	public void HtmlEncode_EncodesHtmlCharacters()
	{
		var encoded = Invoke("HtmlEncode", "<div class=\"test\">&</div>");

		encoded.ShouldBe("&lt;div class=&quot;test&quot;&gt;&amp;&lt;/div&gt;");
	}

	[Fact]
	public void HtmlDecode_DecodesHtmlEntities()
	{
		var decoded = Invoke("HtmlDecode", "&lt;strong&gt;Jeff&lt;/strong&gt;");

		decoded.ShouldBe("<strong>Jeff</strong>");
	}
}

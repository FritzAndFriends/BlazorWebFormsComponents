using System.Net;

namespace System.Web;

public static class HttpUtility
{
	public static string? UrlEncode(string? value) => value is null ? null : WebUtility.UrlEncode(value);

	public static string? UrlDecode(string? value) => value is null ? null : WebUtility.UrlDecode(value);

	public static string? HtmlEncode(string? value) => value is null ? null : WebUtility.HtmlEncode(value);

	public static string? HtmlDecode(string? value) => value is null ? null : WebUtility.HtmlDecode(value);
}

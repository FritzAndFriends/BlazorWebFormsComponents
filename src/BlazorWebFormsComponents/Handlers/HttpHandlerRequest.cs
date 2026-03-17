using System;
using System.Collections.Specialized;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace BlazorWebFormsComponents;

/// <summary>
/// Adapter that wraps an ASP.NET Core <see cref="HttpRequest"/> with a Web Forms-compatible
/// API surface. Provides <see cref="NameValueCollection"/>-based access to query string,
/// form data, and headers, plus an indexer that checks QueryString then Form (matching
/// Web Forms <c>HttpRequest</c> behavior).
/// </summary>
public class HttpHandlerRequest
{
	private readonly HttpRequest _request;
	private NameValueCollection _queryString;
	private NameValueCollection _form;
	private NameValueCollection _headers;

	internal HttpHandlerRequest(HttpRequest request)
	{
		_request = request;
	}

	/// <summary>
	/// Gets the query string parameters as a <see cref="NameValueCollection"/>.
	/// Equivalent to <c>HttpRequest.QueryString</c> in Web Forms.
	/// </summary>
	public NameValueCollection QueryString
	{
		get
		{
			if (_queryString == null)
			{
				_queryString = new NameValueCollection();
				foreach (var kvp in _request.Query)
				{
					foreach (var value in kvp.Value)
					{
						_queryString.Add(kvp.Key, value);
					}
				}
			}
			return _queryString;
		}
	}

	/// <summary>
	/// Gets the form data as a <see cref="NameValueCollection"/>.
	/// Returns an empty collection if the request does not have form content type.
	/// Equivalent to <c>HttpRequest.Form</c> in Web Forms.
	/// </summary>
	public NameValueCollection Form
	{
		get
		{
			if (_form == null)
			{
				_form = new NameValueCollection();
				if (_request.HasFormContentType)
				{
					foreach (var kvp in _request.Form)
					{
						foreach (var value in kvp.Value)
						{
							_form.Add(kvp.Key, value);
						}
					}
				}
			}
			return _form;
		}
	}

	/// <summary>
	/// Gets the request headers as a <see cref="NameValueCollection"/>.
	/// Equivalent to <c>HttpRequest.Headers</c> in Web Forms.
	/// </summary>
	public NameValueCollection Headers
	{
		get
		{
			if (_headers == null)
			{
				_headers = new NameValueCollection();
				foreach (var kvp in _request.Headers)
				{
					foreach (var value in kvp.Value)
					{
						_headers.Add(kvp.Key, value);
					}
				}
			}
			return _headers;
		}
	}

	/// <summary>
	/// Gets the request cookies.
	/// Equivalent to <c>HttpRequest.Cookies</c> in Web Forms.
	/// </summary>
	public IRequestCookieCollection Cookies => _request.Cookies;

	/// <summary>
	/// Gets the HTTP method (GET, POST, etc.).
	/// Equivalent to <c>HttpRequest.HttpMethod</c> in Web Forms.
	/// </summary>
	public string HttpMethod => _request.Method;

	/// <summary>
	/// Gets or sets the content type of the request.
	/// Equivalent to <c>HttpRequest.ContentType</c> in Web Forms.
	/// </summary>
	public string ContentType => _request.ContentType;

	/// <summary>
	/// Gets the request body as a <see cref="Stream"/>.
	/// Equivalent to <c>HttpRequest.InputStream</c> in Web Forms.
	/// </summary>
	public Stream InputStream => _request.Body;

	/// <summary>
	/// Gets the full request URL as a <see cref="Uri"/>.
	/// Equivalent to <c>HttpRequest.Url</c> in Web Forms.
	/// </summary>
	public Uri Url
	{
		get
		{
			var req = _request;
			return new Uri($"{req.Scheme}://{req.Host}{req.PathBase}{req.Path}{req.QueryString}");
		}
	}

	/// <summary>
	/// Gets the raw URL (path + query string) of the request.
	/// Equivalent to <c>HttpRequest.RawUrl</c> in Web Forms.
	/// </summary>
	public string RawUrl => $"{_request.PathBase}{_request.Path}{_request.QueryString}";

	/// <summary>
	/// Gets the IP address of the remote client.
	/// Equivalent to <c>HttpRequest.UserHostAddress</c> in Web Forms.
	/// </summary>
	public string UserHostAddress => _request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

	/// <summary>
	/// Gets a value indicating whether the connection uses SSL/TLS.
	/// Equivalent to <c>HttpRequest.IsSecureConnection</c> in Web Forms.
	/// </summary>
	public bool IsSecureConnection => _request.IsHttps;

	/// <summary>
	/// Gets the uploaded files collection. Returns an empty collection if
	/// the request does not have form content type.
	/// Equivalent to <c>HttpRequest.Files</c> in Web Forms.
	/// </summary>
	public IFormFileCollection Files
	{
		get
		{
			if (_request.HasFormContentType)
			{
				return _request.Form.Files;
			}
			return new FormFileCollection();
		}
	}

	/// <summary>
	/// Gets a value from the request by key, checking <see cref="QueryString"/> first,
	/// then <see cref="Form"/>. This matches the Web Forms <c>HttpRequest</c> default
	/// indexer behavior.
	/// </summary>
	/// <param name="key">The key to look up.</param>
	/// <returns>The value if found; otherwise <c>null</c>.</returns>
	public string this[string key]
	{
		get
		{
			var value = QueryString[key];
			if (value != null)
			{
				return value;
			}
			return Form[key];
		}
	}
}

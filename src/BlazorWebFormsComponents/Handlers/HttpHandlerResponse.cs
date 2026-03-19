using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BlazorWebFormsComponents;

/// <summary>
/// Adapter that wraps an ASP.NET Core <see cref="HttpResponse"/> with a Web Forms-compatible
/// API surface. Provides synchronous <see cref="Write(string)"/> and <see cref="BinaryWrite(byte[])"/>
/// methods for migration compatibility, plus header management and the <see cref="End"/> shim.
/// </summary>
/// <remarks>
/// <para>
/// <c>Write</c> and <c>BinaryWrite</c> use sync-over-async internally. This is safe in
/// the ASP.NET Core middleware/endpoint context because there is no <c>SynchronizationContext</c>.
/// For performance-sensitive handlers, use <c>WriteAsync</c> and <c>BinaryWriteAsync</c> instead.
/// </para>
/// </remarks>
public class HttpHandlerResponse
{
	private readonly HttpResponse _response;
	private bool _ended;

	internal HttpHandlerResponse(HttpResponse response)
	{
		_response = response;
	}

	/// <summary>
	/// Gets or sets the content type of the response.
	/// Equivalent to <c>HttpResponse.ContentType</c> in Web Forms.
	/// </summary>
	public string ContentType
	{
		get => _response.ContentType;
		set => _response.ContentType = value;
	}

	/// <summary>
	/// Gets or sets the HTTP status code of the response.
	/// Equivalent to <c>HttpResponse.StatusCode</c> in Web Forms.
	/// </summary>
	public int StatusCode
	{
		get => _response.StatusCode;
		set => _response.StatusCode = value;
	}

	/// <summary>
	/// Gets the response body stream.
	/// Equivalent to <c>HttpResponse.OutputStream</c> in Web Forms.
	/// </summary>
	public Stream OutputStream => _response.Body;

	/// <summary>
	/// Gets a value indicating whether <see cref="End"/> has been called.
	/// In Web Forms, <c>Response.End()</c> throws <c>ThreadAbortException</c> to halt
	/// execution. In BWFC, it sets this flag instead. Migrated code should check
	/// <c>IsEnded</c> and <c>return</c> from <c>ProcessRequestAsync</c>.
	/// </summary>
	public bool IsEnded => _ended;

	/// <summary>
	/// Writes a string to the response body. Uses sync-over-async internally
	/// for Web Forms migration compatibility.
	/// </summary>
	/// <param name="text">The text to write.</param>
	public void Write(string text)
	{
		_response.WriteAsync(text).GetAwaiter().GetResult();
	}

	/// <summary>
	/// Writes a string to the response body asynchronously.
	/// Preferred over <see cref="Write(string)"/> for new or refactored code.
	/// </summary>
	/// <param name="text">The text to write.</param>
	public Task WriteAsync(string text)
	{
		return _response.WriteAsync(text);
	}

	/// <summary>
	/// Writes binary data to the response body. Uses sync-over-async internally
	/// for Web Forms migration compatibility.
	/// </summary>
	/// <param name="data">The byte array to write.</param>
	public void BinaryWrite(byte[] data)
	{
		_response.Body.WriteAsync(data).GetAwaiter().GetResult();
	}

	/// <summary>
	/// Writes binary data to the response body asynchronously.
	/// Preferred over <see cref="BinaryWrite(byte[])"/> for new or refactored code.
	/// </summary>
	/// <param name="data">The byte array to write.</param>
	public Task BinaryWriteAsync(byte[] data)
	{
		return _response.Body.WriteAsync(data).AsTask();
	}

	/// <summary>
	/// Adds a response header. Maps to <c>Headers.Append</c> in ASP.NET Core.
	/// Equivalent to <c>HttpResponse.AddHeader</c> in Web Forms.
	/// </summary>
	/// <param name="name">The header name.</param>
	/// <param name="value">The header value.</param>
	public void AddHeader(string name, string value)
	{
		_response.Headers.Append(name, value);
	}

	/// <summary>
	/// Appends a response header. Identical to <see cref="AddHeader"/> in behavior.
	/// Equivalent to <c>HttpResponse.AppendHeader</c> in Web Forms.
	/// </summary>
	/// <param name="name">The header name.</param>
	/// <param name="value">The header value.</param>
	public void AppendHeader(string name, string value)
	{
		_response.Headers.Append(name, value);
	}

	/// <summary>
	/// Redirects the client to the specified URL with a 302 status code.
	/// Equivalent to <c>HttpResponse.Redirect</c> in Web Forms.
	/// </summary>
	/// <param name="url">The target URL.</param>
	public void Redirect(string url)
	{
		_response.Redirect(url);
	}

	/// <summary>
	/// Clears the response headers and resets the status code to 200.
	/// Equivalent to <c>HttpResponse.Clear</c> in Web Forms.
	/// </summary>
	/// <remarks>
	/// If bytes have already been flushed to the client, this cannot undo them.
	/// In practice, Web Forms handlers call <c>Clear()</c> at the start of
	/// <c>ProcessRequest</c> before any output is written.
	/// </remarks>
	public void Clear()
	{
		_response.Headers.Clear();
		_response.StatusCode = 200;
	}

	/// <summary>
	/// In Web Forms, <c>Response.End()</c> throws <c>ThreadAbortException</c> to halt
	/// execution immediately. In ASP.NET Core, this is not possible.
	/// This shim sets the <see cref="IsEnded"/> flag. Migrated code should be updated
	/// to simply <c>return</c> from <c>ProcessRequestAsync</c> instead.
	/// </summary>
	[Obsolete("Use return from ProcessRequestAsync instead. " +
			  "Response.End() cannot halt execution in ASP.NET Core. " +
			  "This method sets the IsEnded flag but does not throw.")]
	public void End()
	{
		_ended = true;
	}

	/// <summary>
	/// Flushes the response body to the client asynchronously.
	/// Uses sync-over-async for Web Forms API compatibility.
	/// </summary>
	public void Flush()
	{
		_response.Body.FlushAsync().GetAwaiter().GetResult();
	}

	/// <summary>
	/// Flushes the response body to the client asynchronously.
	/// Preferred over <see cref="Flush"/> for new or refactored code.
	/// </summary>
	public Task FlushAsync()
	{
		return _response.Body.FlushAsync();
	}
}

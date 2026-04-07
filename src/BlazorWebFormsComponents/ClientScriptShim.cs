using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorWebFormsComponents;

/// <summary>
/// Compatibility shim for Web Forms <c>Page.ClientScript</c>
/// (<see cref="System.Web.UI.ClientScriptManager"/>).
/// Queues startup scripts, script blocks, and script includes during
/// the component lifecycle and flushes them via <see cref="IJSRuntime"/>
/// in <c>OnAfterRenderAsync</c>.
/// <para>
/// Register as a scoped service so each user/circuit gets its own instance.
/// </para>
/// </summary>
public class ClientScriptShim
{
	private static readonly Regex ScriptTagPattern = new(
		@"^\s*<script[^>]*>\s*|\s*</script>\s*$",
		RegexOptions.IgnoreCase | RegexOptions.Compiled);

	private readonly ILogger<ClientScriptShim> _logger;
	private readonly Dictionary<string, string> _startupScripts = new();
	private readonly Dictionary<string, string> _scriptBlocks = new();
	private readonly Dictionary<string, string> _scriptIncludes = new();
	private readonly HashSet<string> _flushedStartupScripts = new();
	private readonly HashSet<string> _flushedScriptBlocks = new();
	private readonly HashSet<string> _flushedScriptIncludes = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="ClientScriptShim"/> class.
	/// </summary>
	/// <param name="logger">Logger for diagnostics.</param>
	public ClientScriptShim(ILogger<ClientScriptShim> logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	// ─── Startup Scripts ───────────────────────────────────────────────

	/// <summary>
	/// Registers a startup script to be executed after the component renders.
	/// Emulates <c>ClientScriptManager.RegisterStartupScript(Type, String, String, Boolean)</c>.
	/// </summary>
	/// <param name="type">The type of the calling component (used for key scoping).</param>
	/// <param name="key">A unique key identifying this script.</param>
	/// <param name="script">The JavaScript code to execute.</param>
	/// <param name="addScriptTags">
	/// If <c>true</c>, indicates the caller expects script tags to be added.
	/// Any existing <c>&lt;script&gt;</c> wrapper is stripped since execution
	/// is via <see cref="IJSRuntime"/>.
	/// </param>
	public void RegisterStartupScript(Type type, string key, string script, bool addScriptTags)
	{
		var compositeKey = BuildKey(type, key);
		var cleanScript = addScriptTags ? StripScriptTags(script) : script;
		_startupScripts[compositeKey] = cleanScript;
	}

	/// <summary>
	/// Registers a startup script to be executed after the component renders.
	/// Emulates <c>ClientScriptManager.RegisterStartupScript(Type, String, String)</c>.
	/// </summary>
	/// <param name="type">The type of the calling component (used for key scoping).</param>
	/// <param name="key">A unique key identifying this script.</param>
	/// <param name="script">The JavaScript code to execute.</param>
	public void RegisterStartupScript(Type type, string key, string script)
	{
		RegisterStartupScript(type, key, script, addScriptTags: false);
	}

	/// <summary>
	/// Determines whether a startup script with the specified key has already
	/// been registered.
	/// </summary>
	/// <param name="type">The type of the calling component.</param>
	/// <param name="key">The script key.</param>
	/// <returns><c>true</c> if the script is registered; otherwise <c>false</c>.</returns>
	public bool IsStartupScriptRegistered(Type type, string key)
	{
		var k = BuildKey(type, key);
		return _startupScripts.ContainsKey(k) || _flushedStartupScripts.Contains(k);
	}

	// ─── Client Script Blocks ──────────────────────────────────────────

	/// <summary>
	/// Registers a client script block to be executed after the component renders.
	/// Emulates <c>ClientScriptManager.RegisterClientScriptBlock(Type, String, String, Boolean)</c>.
	/// </summary>
	/// <param name="type">The type of the calling component (used for key scoping).</param>
	/// <param name="key">A unique key identifying this script block.</param>
	/// <param name="script">The JavaScript code to execute.</param>
	/// <param name="addScriptTags">
	/// If <c>true</c>, any existing <c>&lt;script&gt;</c> wrapper is stripped.
	/// </param>
	public void RegisterClientScriptBlock(Type type, string key, string script, bool addScriptTags)
	{
		var compositeKey = BuildKey(type, key);
		var cleanScript = addScriptTags ? StripScriptTags(script) : script;
		_scriptBlocks[compositeKey] = cleanScript;
	}

	/// <summary>
	/// Determines whether a client script block with the specified key has
	/// already been registered.
	/// </summary>
	/// <param name="type">The type of the calling component.</param>
	/// <param name="key">The script block key.</param>
	/// <returns><c>true</c> if the script block is registered; otherwise <c>false</c>.</returns>
	public bool IsClientScriptBlockRegistered(Type type, string key)
	{
		var k = BuildKey(type, key);
		return _scriptBlocks.ContainsKey(k) || _flushedScriptBlocks.Contains(k);
	}

	// ─── Client Script Includes ────────────────────────────────────────

	/// <summary>
	/// Registers a client script include (external JS file) to be loaded
	/// after the component renders.
	/// Emulates <c>ClientScriptManager.RegisterClientScriptInclude(String, String)</c>.
	/// </summary>
	/// <param name="key">A unique key identifying this script include.</param>
	/// <param name="url">The URL of the script file to load.</param>
	public void RegisterClientScriptInclude(string key, string url)
	{
		_scriptIncludes[key] = url;
	}

	/// <summary>
	/// Registers a client script include (external JS file) to be loaded
	/// after the component renders.
	/// Emulates <c>ClientScriptManager.RegisterClientScriptInclude(Type, String, String)</c>.
	/// </summary>
	/// <param name="type">The type of the calling component (ignored; key-only deduplication).</param>
	/// <param name="key">A unique key identifying this script include.</param>
	/// <param name="url">The URL of the script file to load.</param>
	public void RegisterClientScriptInclude(Type type, string key, string url)
	{
		RegisterClientScriptInclude(key, url);
	}

	/// <summary>
	/// Determines whether a client script include with the specified key
	/// has already been registered.
	/// </summary>
	/// <param name="key">The script include key.</param>
	/// <returns><c>true</c> if the include is registered; otherwise <c>false</c>.</returns>
	public bool IsClientScriptIncludeRegistered(string key)
	{
		return _scriptIncludes.ContainsKey(key) || _flushedScriptIncludes.Contains(key);
	}

	// ─── Flush ─────────────────────────────────────────────────────────

	/// <summary>
	/// Executes all queued scripts and loads all queued includes via
	/// <see cref="IJSRuntime"/>, then clears the queues.
	/// Called automatically by <see cref="BaseWebFormsComponent"/> in
	/// <c>OnAfterRenderAsync</c>.
	/// </summary>
	/// <param name="jsRuntime">The Blazor JS interop runtime.</param>
	public async Task FlushAsync(IJSRuntime jsRuntime)
	{
		if (_scriptBlocks.Count == 0 && _startupScripts.Count == 0 && _scriptIncludes.Count == 0)
			return;

		// Script blocks execute first (mirrors Web Forms page lifecycle order)
		foreach (var kvp in _scriptBlocks)
		{
			_logger.LogDebug("ClientScriptShim: flushing script block '{Key}'.", kvp.Key);
			await jsRuntime.InvokeVoidAsync("eval", kvp.Value);
		}

		// Startup scripts execute after blocks
		foreach (var kvp in _startupScripts)
		{
			_logger.LogDebug("ClientScriptShim: flushing startup script '{Key}'.", kvp.Key);
			await jsRuntime.InvokeVoidAsync("eval", kvp.Value);
		}

		// Script includes — dynamically append <script> tags
		foreach (var kvp in _scriptIncludes)
		{
			_logger.LogDebug("ClientScriptShim: loading script include '{Key}' from '{Url}'.", kvp.Key, kvp.Value);
			var loaderScript = $"(function(){{var s=document.createElement('script');s.src='{EscapeJsString(kvp.Value)}';document.head.appendChild(s);}})()";
			await jsRuntime.InvokeVoidAsync("eval", loaderScript);
		}

		foreach (var key in _scriptBlocks.Keys) _flushedScriptBlocks.Add(key);
		foreach (var key in _startupScripts.Keys) _flushedStartupScripts.Add(key);
		foreach (var key in _scriptIncludes.Keys) _flushedScriptIncludes.Add(key);

		_scriptBlocks.Clear();
		_startupScripts.Clear();
		_scriptIncludes.Clear();
	}

	// ─── PostBack / Callback Methods ──────────────────────────────────

	/// <summary>
	/// Returns a JavaScript string that triggers a postback, mirroring the
	/// Web Forms <c>ClientScriptManager.GetPostBackEventReference</c> behavior.
	/// The returned string calls <c>__doPostBack(eventTarget, eventArgument)</c>,
	/// which is defined in <c>bwfc-postback.js</c>.
	/// </summary>
	/// <param name="control">The control initiating the postback (used to resolve an ID).</param>
	/// <param name="argument">The event argument string.</param>
	/// <returns>A JavaScript expression string, e.g. <c>__doPostBack('Button1', '')</c>.</returns>
	public string GetPostBackEventReference(object control, string argument)
	{
		var id = ResolveControlId(control);
		return $"__doPostBack('{EscapeJsString(id)}', '{EscapeJsString(argument ?? string.Empty)}')";
	}

	/// <summary>
	/// Returns a <c>javascript:</c> URL that triggers a postback, mirroring
	/// <c>ClientScriptManager.GetPostBackClientHyperlink</c>.
	/// </summary>
	/// <param name="control">The control initiating the postback.</param>
	/// <param name="argument">The event argument string.</param>
	/// <returns>A <c>javascript:__doPostBack(...)</c> URL string.</returns>
	public string GetPostBackClientHyperlink(object control, string argument)
	{
		return $"javascript:{GetPostBackEventReference(control, argument)}";
	}

	/// <summary>
	/// Returns a JavaScript expression that invokes the BWFC callback bridge,
	/// mirroring <c>ClientScriptManager.GetCallbackEventReference</c>.
	/// The returned expression calls <c>__bwfc_callback</c> defined in
	/// <c>bwfc-postback.js</c>.
	/// </summary>
	public string GetCallbackEventReference(object control, string argument,
		string clientCallback, string context, string clientErrorCallback, bool useAsync)
	{
		var id = ResolveControlId(control);
		return $"__bwfc_callback('{EscapeJsString(id)}', '{EscapeJsString(argument ?? string.Empty)}', {clientCallback ?? "null"}, '{EscapeJsString(context ?? string.Empty)}', {clientErrorCallback ?? "null"})";
	}

	// ─── Helpers ───────────────────────────────────────────────────────

	private static string BuildKey(Type type, string key)
	{
		ArgumentNullException.ThrowIfNull(type);
		return $"{type.FullName}:{key}";
	}

	private static string StripScriptTags(string script)
	{
		return ScriptTagPattern.Replace(script, string.Empty).Trim();
	}

	private static string EscapeJsString(string value)
	{
		return value.Replace("\\", "\\\\").Replace("'", "\\'");
	}

	/// <summary>
	/// Resolves a control reference to an ID string suitable for use in
	/// JavaScript postback/callback expressions.
	/// Prefers <see cref="BaseWebFormsComponent.ID"/> when available.
	/// </summary>
	private static string ResolveControlId(object control)
	{
		if (control is BaseWebFormsComponent bwfc && !string.IsNullOrEmpty(bwfc.ID))
			return bwfc.ID;
		if (control is ComponentBase)
			return control.GetType().Name;
		return control?.GetType().Name ?? "unknown";
	}
}

using System;

namespace BlazorWebFormsComponents;

/// <summary>
/// Compatibility shim for <c>System.Web.UI.ScriptManager</c>.
/// Provides the static <see cref="GetCurrent"/> factory method that Web Forms
/// code uses to obtain a <c>ScriptManager</c> instance from the page, and
/// delegates all <c>RegisterXxx</c> calls to <see cref="ClientScriptShim"/>.
/// </summary>
public class ScriptManagerShim
{
	private readonly ClientScriptShim _clientScript;

	/// <summary>
	/// Initializes a new instance backed by the specified <see cref="ClientScriptShim"/>.
	/// </summary>
	public ScriptManagerShim(ClientScriptShim clientScript)
	{
		_clientScript = clientScript ?? throw new ArgumentNullException(nameof(clientScript));
	}

	/// <summary>
	/// Static factory matching the Web Forms <c>ScriptManager.GetCurrent(Page)</c> pattern.
	/// Resolves the <see cref="ClientScriptShim"/> from the page/component instance.
	/// </summary>
	/// <param name="page">
	/// A <see cref="BaseWebFormsComponent"/> or <see cref="WebFormsPageBase"/> instance.
	/// </param>
	/// <returns>A <see cref="ScriptManagerShim"/> wrapping the component's ClientScript.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown when <paramref name="page"/> is not a recognized BWFC type or has no ClientScript.
	/// </exception>
	public static ScriptManagerShim GetCurrent(object page)
	{
		if (page is BaseWebFormsComponent component && component.ClientScript != null)
			return new ScriptManagerShim(component.ClientScript);
		if (page is WebFormsPageBase pageBase && pageBase.ClientScript != null)
			return new ScriptManagerShim(pageBase.ClientScript);

		throw new InvalidOperationException(
			"ScriptManager.GetCurrent() requires a component derived from " +
			"BaseWebFormsComponent or WebFormsPageBase with a registered ClientScriptShim.");
	}

	// ─── Delegated Registration Methods ───────────────────────────────

	/// <inheritdoc cref="ClientScriptShim.RegisterStartupScript(Type, string, string, bool)"/>
	public void RegisterStartupScript(Type type, string key, string script, bool addScriptTags)
		=> _clientScript.RegisterStartupScript(type, key, script, addScriptTags);

	/// <summary>
	/// Overload accepting a control reference (ignored) to match the Web Forms
	/// <c>ScriptManager.RegisterStartupScript(Control, Type, String, String, Boolean)</c> signature.
	/// </summary>
	public void RegisterStartupScript(object control, Type type, string key, string script, bool addScriptTags)
		=> _clientScript.RegisterStartupScript(type, key, script, addScriptTags);

	/// <inheritdoc cref="ClientScriptShim.RegisterClientScriptBlock(Type, string, string, bool)"/>
	public void RegisterClientScriptBlock(Type type, string key, string script, bool addScriptTags)
		=> _clientScript.RegisterClientScriptBlock(type, key, script, addScriptTags);

	/// <inheritdoc cref="ClientScriptShim.RegisterClientScriptInclude(Type, string, string)"/>
	public void RegisterClientScriptInclude(Type type, string key, string url)
		=> _clientScript.RegisterClientScriptInclude(type, key, url);
}

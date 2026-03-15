using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Restricts input in a target TextBox to specified character sets.
/// Filters keystrokes in real-time and strips invalid characters on paste.
/// Emulates the Ajax Control Toolkit FilteredTextBoxExtender.
/// </summary>
public class FilteredTextBoxExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/filtered-textbox-extender.js";

	/// <summary>
	/// The types of characters to allow. Can be combined as flags.
	/// </summary>
	[Parameter]
	public FilterType FilterType { get; set; } = FilterType.Custom;

	/// <summary>
	/// Additional valid characters when FilterType includes Custom.
	/// </summary>
	[Parameter]
	public string ValidChars { get; set; } = string.Empty;

	/// <summary>
	/// Characters to explicitly block.
	/// </summary>
	[Parameter]
	public string InvalidChars { get; set; } = string.Empty;

	/// <summary>
	/// Whether ValidChars or InvalidChars takes precedence.
	/// </summary>
	[Parameter]
	public FilterMode FilterMode { get; set; } = FilterMode.ValidChars;

	/// <summary>
	/// Milliseconds interval for filtering. Default is 250.
	/// </summary>
	[Parameter]
	public int FilterInterval { get; set; } = 250;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		filterType = (int)FilterType,
		validChars = ValidChars,
		invalidChars = InvalidChars,
		filterMode = (int)FilterMode,
		filterInterval = FilterInterval
	};
}

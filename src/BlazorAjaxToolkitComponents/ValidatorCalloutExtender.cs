using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Enhances ASP.NET validators by showing validation messages in a callout/tooltip 
/// bubble instead of inline text. Attaches to a validator control and displays its 
/// error message in a styled popup near the invalid field.
/// Emulates the Ajax Control Toolkit ValidatorCalloutExtender.
/// </summary>
public class ValidatorCalloutExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/validator-callout-extender.js";

	/// <summary>
	/// Width of the callout in pixels. Default is 250.
	/// </summary>
	[Parameter]
	public int Width { get; set; } = 250;

	/// <summary>
	/// CSS class to apply to the invalid input field when validation fails.
	/// </summary>
	[Parameter]
	public string HighlightCssClass { get; set; } = string.Empty;

	/// <summary>
	/// URL to the warning icon image displayed in the callout.
	/// </summary>
	[Parameter]
	public string WarningIconImageUrl { get; set; } = string.Empty;

	/// <summary>
	/// URL to the close button image. If provided, a close button appears in the callout.
	/// </summary>
	[Parameter]
	public string CloseImageUrl { get; set; } = string.Empty;

	/// <summary>
	/// CSS class for the callout container element.
	/// </summary>
	[Parameter]
	public string CssClass { get; set; } = string.Empty;

	/// <summary>
	/// Position of the callout relative to the invalid field.
	/// Default is BottomLeft.
	/// </summary>
	[Parameter]
	public PopupPosition PopupPosition { get; set; } = PopupPosition.BottomLeft;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		width = Width,
		highlightCssClass = HighlightCssClass,
		warningIconImageUrl = WarningIconImageUrl,
		closeImageUrl = CloseImageUrl,
		cssClass = CssClass,
		popupPosition = PopupPosition.ToString()
	};
}

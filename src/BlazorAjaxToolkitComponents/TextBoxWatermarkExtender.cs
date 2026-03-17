using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Displays placeholder/watermark text in a TextBox when it is empty.
/// The watermark text appears in a different style (via WatermarkCssClass)
/// and disappears when the user focuses the field or types.
/// Emulates the Ajax Control Toolkit TextBoxWatermarkExtender.
/// </summary>
public class TextBoxWatermarkExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/textbox-watermark-extender.js";

	/// <summary>
	/// The placeholder/watermark text to display when the TextBox is empty.
	/// </summary>
	[Parameter]
	public string WatermarkText { get; set; } = string.Empty;

	/// <summary>
	/// CSS class applied to the TextBox when showing the watermark.
	/// Typically used to style the watermark text differently (e.g., gray, italic).
	/// </summary>
	[Parameter]
	public string WatermarkCssClass { get; set; } = string.Empty;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		watermarkText = WatermarkText,
		watermarkCssClass = WatermarkCssClass
	};
}

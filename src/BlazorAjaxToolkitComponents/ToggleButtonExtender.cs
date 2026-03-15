using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Replaces a target checkbox with a clickable image that toggles between checked and unchecked states.
/// Supports separate images for checked, unchecked, hover, and disabled states.
/// Emulates the Ajax Control Toolkit ToggleButtonExtender.
/// </summary>
public class ToggleButtonExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/toggle-button-extender.js";

	/// <summary>
	/// The width of the toggle image in pixels.
	/// </summary>
	[Parameter]
	public int ImageWidth { get; set; }

	/// <summary>
	/// The height of the toggle image in pixels.
	/// </summary>
	[Parameter]
	public int ImageHeight { get; set; }

	/// <summary>
	/// URL of the image displayed when the checkbox is unchecked.
	/// </summary>
	[Parameter]
	public string UncheckedImageUrl { get; set; } = string.Empty;

	/// <summary>
	/// URL of the image displayed when the checkbox is checked.
	/// </summary>
	[Parameter]
	public string CheckedImageUrl { get; set; } = string.Empty;

	/// <summary>
	/// Alternate text for the unchecked image (accessibility).
	/// </summary>
	[Parameter]
	public string UncheckedImageAlternateText { get; set; } = string.Empty;

	/// <summary>
	/// Alternate text for the checked image (accessibility).
	/// </summary>
	[Parameter]
	public string CheckedImageAlternateText { get; set; } = string.Empty;

	/// <summary>
	/// URL of the image displayed on hover when the checkbox is checked.
	/// </summary>
	[Parameter]
	public string CheckedImageOverUrl { get; set; } = string.Empty;

	/// <summary>
	/// URL of the image displayed on hover when the checkbox is unchecked.
	/// </summary>
	[Parameter]
	public string UncheckedImageOverUrl { get; set; } = string.Empty;

	/// <summary>
	/// URL of the image displayed when the checkbox is disabled and unchecked.
	/// </summary>
	[Parameter]
	public string DisabledUncheckedImageUrl { get; set; } = string.Empty;

	/// <summary>
	/// URL of the image displayed when the checkbox is disabled and checked.
	/// </summary>
	[Parameter]
	public string DisabledCheckedImageUrl { get; set; } = string.Empty;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		imageWidth = ImageWidth,
		imageHeight = ImageHeight,
		uncheckedImageUrl = UncheckedImageUrl,
		checkedImageUrl = CheckedImageUrl,
		uncheckedImageAlternateText = UncheckedImageAlternateText,
		checkedImageAlternateText = CheckedImageAlternateText,
		checkedImageOverUrl = CheckedImageOverUrl,
		uncheckedImageOverUrl = UncheckedImageOverUrl,
		disabledUncheckedImageUrl = DisabledUncheckedImageUrl,
		disabledCheckedImageUrl = DisabledCheckedImageUrl
	};
}

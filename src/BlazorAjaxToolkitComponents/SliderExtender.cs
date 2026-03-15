using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Attaches range slider behavior to a target input element.
/// Supports horizontal/vertical orientation, bound control synchronization,
/// and customizable rail/handle appearance via CSS classes or image URLs.
/// Emulates the Ajax Control Toolkit SliderExtender.
/// </summary>
public class SliderExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/slider-extender.js";

	/// <summary>
	/// The minimum value of the slider range.
	/// </summary>
	[Parameter]
	public double Minimum { get; set; } = 0;

	/// <summary>
	/// The maximum value of the slider range.
	/// </summary>
	[Parameter]
	public double Maximum { get; set; } = 100;

	/// <summary>
	/// The number of discrete steps in the slider range. 0 means continuous.
	/// </summary>
	[Parameter]
	public int Steps { get; set; }

	/// <summary>
	/// The current value of the slider.
	/// </summary>
	[Parameter]
	public double Value { get; set; }

	/// <summary>
	/// The ID of another element whose value is synchronized with the slider value.
	/// </summary>
	[Parameter]
	public string BoundControlID { get; set; } = string.Empty;

	/// <summary>
	/// The orientation of the slider (Horizontal or Vertical).
	/// </summary>
	[Parameter]
	public SliderOrientation Orientation { get; set; } = SliderOrientation.Horizontal;

	/// <summary>
	/// CSS class applied to the slider rail/track element.
	/// </summary>
	[Parameter]
	public string RailCssClass { get; set; } = string.Empty;

	/// <summary>
	/// CSS class applied to the slider handle element.
	/// </summary>
	[Parameter]
	public string HandleCssClass { get; set; } = string.Empty;

	/// <summary>
	/// URL of an image to use for the slider handle.
	/// </summary>
	[Parameter]
	public string HandleImageUrl { get; set; } = string.Empty;

	/// <summary>
	/// The length of the slider in pixels.
	/// </summary>
	[Parameter]
	public int Length { get; set; }

	/// <summary>
	/// The number of decimal places for the slider value.
	/// </summary>
	[Parameter]
	public int Decimals { get; set; }

	/// <summary>
	/// Tooltip text displayed when hovering over the slider handle.
	/// Use {0} as a placeholder for the current value.
	/// </summary>
	[Parameter]
	public string TooltipText { get; set; } = string.Empty;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		minimum = Minimum,
		maximum = Maximum,
		steps = Steps,
		value = Value,
		boundControlId = BoundControlID,
		orientation = (int)Orientation,
		railCssClass = RailCssClass,
		handleCssClass = HandleCssClass,
		handleImageUrl = HandleImageUrl,
		length = Length,
		decimals = Decimals,
		tooltipText = TooltipText
	};
}

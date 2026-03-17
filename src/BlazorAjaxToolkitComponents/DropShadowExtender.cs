using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Adds a drop shadow effect to an element, giving it a raised/floating appearance.
/// Emulates the Ajax Control Toolkit DropShadowExtender.
/// </summary>
public class DropShadowExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/drop-shadow-extender.js";

	/// <summary>
	/// Shadow opacity from 0.0 (fully transparent) to 1.0 (fully opaque). Default is 0.5.
	/// </summary>
	[Parameter]
	public double Opacity { get; set; } = 0.5;

	/// <summary>
	/// Shadow width in pixels. Default is 5.
	/// </summary>
	[Parameter]
	public int Width { get; set; } = 5;

	/// <summary>
	/// Whether to round the corners of the target element.
	/// </summary>
	[Parameter]
	public bool Rounded { get; set; } = false;

	/// <summary>
	/// Corner radius in pixels when Rounded is true.
	/// </summary>
	[Parameter]
	public int Radius { get; set; } = 0;

	/// <summary>
	/// Whether the shadow should track the element's position if it moves.
	/// </summary>
	[Parameter]
	public bool TrackPosition { get; set; } = false;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		opacity = Opacity,
		width = Width,
		rounded = Rounded,
		radius = Radius,
		trackPosition = TrackPosition
	};
}

using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Applies rounded corners to an element using CSS border-radius.
/// Emulates the Ajax Control Toolkit RoundedCornersExtender.
/// </summary>
public class RoundedCornersExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/rounded-corners-extender.js";

	/// <summary>
	/// Corner radius in pixels. Default is 5.
	/// </summary>
	[Parameter]
	public int Radius { get; set; } = 5;

	/// <summary>
	/// Specifies which corners to round. Default is All.
	/// </summary>
	[Parameter]
	public BoxCorners Corners { get; set; } = BoxCorners.All;

	/// <summary>
	/// Background color for the rounded area (e.g., "#FFFFFF").
	/// If not specified, the element's existing background is preserved.
	/// </summary>
	[Parameter]
	public string Color { get; set; } = "";

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		radius = Radius,
		corners = (int)Corners,
		color = Color
	};
}

using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Adds numeric up/down spinner buttons to a target TextBox.
/// Supports min/max range, step increments, and cycling through a reference value list.
/// Emulates the Ajax Control Toolkit NumericUpDownExtender.
/// </summary>
public class NumericUpDownExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/numeric-updown-extender.js";

	/// <summary>
	/// The width of the spinner control in pixels.
	/// </summary>
	[Parameter]
	public int Width { get; set; } = 100;

	/// <summary>
	/// The minimum allowed value.
	/// </summary>
	[Parameter]
	public double Minimum { get; set; } = double.MinValue;

	/// <summary>
	/// The maximum allowed value.
	/// </summary>
	[Parameter]
	public double Maximum { get; set; } = double.MaxValue;

	/// <summary>
	/// The amount to increment or decrement on each step. Default is 1.
	/// </summary>
	[Parameter]
	public double Step { get; set; } = 1;

	/// <summary>
	/// Semicolon-separated list of values to cycle through instead of numeric stepping.
	/// When set, the up/down buttons cycle through these values in order.
	/// </summary>
	[Parameter]
	public string RefValues { get; set; } = string.Empty;

	/// <summary>
	/// URL of the web service to call for the up action.
	/// </summary>
	[Parameter]
	public string ServiceUpPath { get; set; } = string.Empty;

	/// <summary>
	/// Web service method name for the up action.
	/// </summary>
	[Parameter]
	public string ServiceUpMethod { get; set; } = string.Empty;

	/// <summary>
	/// URL of the web service to call for the down action.
	/// </summary>
	[Parameter]
	public string ServiceDownPath { get; set; } = string.Empty;

	/// <summary>
	/// Web service method name for the down action.
	/// </summary>
	[Parameter]
	public string ServiceDownMethod { get; set; } = string.Empty;

	/// <summary>
	/// An arbitrary string value passed to the web service methods.
	/// </summary>
	[Parameter]
	public string Tag { get; set; } = string.Empty;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		width = Width,
		minimum = Minimum,
		maximum = Maximum,
		step = Step,
		refValues = RefValues,
		serviceUpPath = ServiceUpPath,
		serviceUpMethod = ServiceUpMethod,
		serviceDownPath = ServiceDownPath,
		serviceDownMethod = ServiceDownMethod,
		tag = Tag
	};
}

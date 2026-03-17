using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Allows users to resize an element by dragging its edges or a resize handle.
/// Emulates the Ajax Control Toolkit ResizableControlExtender.
/// </summary>
public class ResizableControlExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/resizable-control-extender.js";

	/// <summary>
	/// CSS class for the resize handle element (usually positioned at the bottom-right corner).
	/// </summary>
	[Parameter]
	public string HandleCssClass { get; set; } = string.Empty;

	/// <summary>
	/// CSS class applied to the target element while it is being resized.
	/// </summary>
	[Parameter]
	public string ResizableCssClass { get; set; } = string.Empty;

	/// <summary>
	/// Minimum width constraint in pixels. Default is 0 (no minimum).
	/// </summary>
	[Parameter]
	public int MinimumWidth { get; set; } = 0;

	/// <summary>
	/// Minimum height constraint in pixels. Default is 0 (no minimum).
	/// </summary>
	[Parameter]
	public int MinimumHeight { get; set; } = 0;

	/// <summary>
	/// Maximum width constraint in pixels. 0 means no maximum.
	/// </summary>
	[Parameter]
	public int MaximumWidth { get; set; } = 0;

	/// <summary>
	/// Maximum height constraint in pixels. 0 means no maximum.
	/// </summary>
	[Parameter]
	public int MaximumHeight { get; set; } = 0;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		handleCssClass = HandleCssClass,
		resizableCssClass = ResizableCssClass,
		minimumWidth = MinimumWidth,
		minimumHeight = MinimumHeight,
		maximumWidth = MaximumWidth,
		maximumHeight = MaximumHeight
	};
}

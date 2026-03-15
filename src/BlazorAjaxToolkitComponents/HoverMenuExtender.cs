using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Displays a popup panel when the user hovers over a target control.
/// Supports configurable show/hide delays, positional placement, and hover CSS styling.
/// Emulates the Ajax Control Toolkit HoverMenuExtender.
/// </summary>
public class HoverMenuExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/hover-menu-extender.js";

	/// <summary>
	/// The ID of the panel element to display as a hover menu.
	/// </summary>
	[Parameter]
	public string PopupControlID { get; set; } = string.Empty;

	/// <summary>
	/// The position of the popup relative to the target control.
	/// </summary>
	[Parameter]
	public PopupPosition PopupPosition { get; set; } = PopupPosition.Right;

	/// <summary>
	/// Horizontal offset in pixels from the calculated position.
	/// </summary>
	[Parameter]
	public int OffsetX { get; set; }

	/// <summary>
	/// Vertical offset in pixels from the calculated position.
	/// </summary>
	[Parameter]
	public int OffsetY { get; set; }

	/// <summary>
	/// Delay in milliseconds before showing the popup after mouse enters the target.
	/// </summary>
	[Parameter]
	public int PopDelay { get; set; }

	/// <summary>
	/// Delay in milliseconds before hiding the popup after mouse leaves the target and popup.
	/// </summary>
	[Parameter]
	public int HoverDelay { get; set; } = 300;

	/// <summary>
	/// CSS class applied to the target control while the hover menu is visible.
	/// </summary>
	[Parameter]
	public string HoverCssClass { get; set; } = string.Empty;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		popupControlId = PopupControlID,
		popupPosition = (int)PopupPosition,
		offsetX = OffsetX,
		offsetY = OffsetY,
		popDelay = PopDelay,
		hoverDelay = HoverDelay,
		hoverCssClass = HoverCssClass
	};
}

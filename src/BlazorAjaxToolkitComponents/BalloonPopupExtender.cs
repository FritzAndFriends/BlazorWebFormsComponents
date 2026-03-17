using BlazorAjaxToolkitComponents.Enums;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Displays a balloon/tooltip-style popup with a pointer arrow when the user
/// hovers over, clicks, or focuses on a target element. Supports customizable
/// appearance including style, size, shadow, and scrollbar settings.
/// Emulates the Ajax Control Toolkit BalloonPopupExtender.
/// </summary>
public class BalloonPopupExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/balloon-popup-extender.js";

	/// <summary>
	/// The ID of the element to show as the balloon content.
	/// </summary>
	[Parameter]
	public string BalloonPopupControlID { get; set; } = string.Empty;

	/// <summary>
	/// The position of the balloon relative to the target control.
	/// </summary>
	[Parameter]
	public BalloonPosition Position { get; set; } = BalloonPosition.Auto;

	/// <summary>
	/// The visual style of the balloon popup.
	/// </summary>
	[Parameter]
	public BalloonStyle BalloonStyle { get; set; } = BalloonStyle.Rectangle;

	/// <summary>
	/// The size preset for the balloon popup.
	/// </summary>
	[Parameter]
	public BalloonSize BalloonSize { get; set; } = BalloonSize.Medium;

	/// <summary>
	/// Whether to display a drop shadow on the balloon.
	/// </summary>
	[Parameter]
	public bool UseShadow { get; set; } = true;

	/// <summary>
	/// The scrollbar behavior for the balloon content.
	/// </summary>
	[Parameter]
	public ScrollBars ScrollBars { get; set; } = ScrollBars.Auto;

	/// <summary>
	/// Whether to display the balloon on mouse hover.
	/// </summary>
	[Parameter]
	public bool DisplayOnMouseOver { get; set; } = true;

	/// <summary>
	/// Whether to display the balloon when the target receives focus.
	/// </summary>
	[Parameter]
	public bool DisplayOnFocus { get; set; }

	/// <summary>
	/// Whether to display the balloon when the target is clicked.
	/// </summary>
	[Parameter]
	public bool DisplayOnClick { get; set; }

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
	/// URL to custom CSS for styling the balloon popup.
	/// Only used when BalloonStyle is Custom.
	/// </summary>
	[Parameter]
	public string CustomCssUrl { get; set; } = string.Empty;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		balloonPopupControlId = BalloonPopupControlID,
		position = (int)Position,
		balloonStyle = (int)BalloonStyle,
		balloonSize = (int)BalloonSize,
		useShadow = UseShadow,
		scrollBars = (int)ScrollBars,
		displayOnMouseOver = DisplayOnMouseOver,
		displayOnFocus = DisplayOnFocus,
		displayOnClick = DisplayOnClick,
		offsetX = OffsetX,
		offsetY = OffsetY,
		customCssUrl = CustomCssUrl
	};
}

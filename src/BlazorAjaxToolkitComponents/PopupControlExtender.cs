using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Attaches a popup panel to a target control, displaying it on click.
/// Lighter than ModalPopupExtender — no overlay, no focus trap.
/// Supports positional placement, commit property/script, and outside-click dismissal.
/// Emulates the Ajax Control Toolkit PopupControlExtender.
/// </summary>
public class PopupControlExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/popup-control-extender.js";

	/// <summary>
	/// The ID of the panel element to display as a popup.
	/// </summary>
	[Parameter]
	public string PopupControlID { get; set; } = string.Empty;

	/// <summary>
	/// The position of the popup relative to the target control.
	/// </summary>
	[Parameter]
	public PopupPosition Position { get; set; } = PopupPosition.Bottom;

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
	/// The property name on the target control to set when the popup is committed.
	/// Typically "value" for input elements.
	/// </summary>
	[Parameter]
	public string CommitProperty { get; set; } = string.Empty;

	/// <summary>
	/// JavaScript to execute when the popup is committed (a value is selected).
	/// </summary>
	[Parameter]
	public string CommitScript { get; set; } = string.Empty;

	/// <summary>
	/// The ID of this extender control for programmatic access.
	/// </summary>
	[Parameter]
	public string ExtenderControlID { get; set; } = string.Empty;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		popupControlId = PopupControlID,
		position = (int)Position,
		offsetX = OffsetX,
		offsetY = OffsetY,
		commitProperty = CommitProperty,
		commitScript = CommitScript,
		extenderControlId = ExtenderControlID
	};
}

using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Displays a target element as a modal popup with an overlay backdrop.
/// Supports OK/Cancel buttons, focus trapping, drag support, and Escape key dismissal.
/// Emulates the Ajax Control Toolkit ModalPopupExtender.
/// </summary>
public class ModalPopupExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/modal-popup-extender.js";

	/// <summary>
	/// The ID of the element to display as a modal popup.
	/// </summary>
	[Parameter]
	public string PopupControlID { get; set; } = string.Empty;

	/// <summary>
	/// CSS class applied to the overlay backdrop behind the modal.
	/// </summary>
	[Parameter]
	public string BackgroundCssClass { get; set; } = string.Empty;

	/// <summary>
	/// The ID of the control that confirms and closes the modal (OK action).
	/// </summary>
	[Parameter]
	public string OkControlID { get; set; } = string.Empty;

	/// <summary>
	/// The ID of the control that cancels and closes the modal.
	/// </summary>
	[Parameter]
	public string CancelControlID { get; set; } = string.Empty;

	/// <summary>
	/// JavaScript to execute when the modal is dismissed via the OK control.
	/// </summary>
	[Parameter]
	public string OnOkScript { get; set; } = string.Empty;

	/// <summary>
	/// JavaScript to execute when the modal is dismissed via the Cancel control or Escape key.
	/// </summary>
	[Parameter]
	public string OnCancelScript { get; set; } = string.Empty;

	/// <summary>
	/// Whether to add a drop shadow effect to the popup.
	/// </summary>
	[Parameter]
	public bool DropShadow { get; set; }

	/// <summary>
	/// Whether the popup can be dragged by the user.
	/// </summary>
	[Parameter]
	public bool Drag { get; set; }

	/// <summary>
	/// The ID of the element used as a drag handle when Drag is enabled.
	/// If not set, the entire popup is draggable.
	/// </summary>
	[Parameter]
	public string PopupDragHandleControlID { get; set; } = string.Empty;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		popupControlId = PopupControlID,
		backgroundCssClass = BackgroundCssClass,
		okControlId = OkControlID,
		cancelControlId = CancelControlID,
		onOkScript = OnOkScript,
		onCancelScript = OnCancelScript,
		dropShadow = DropShadow,
		drag = Drag,
		popupDragHandleControlId = PopupDragHandleControlID
	};
}

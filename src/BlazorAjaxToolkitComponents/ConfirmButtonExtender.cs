using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Attaches a confirmation dialog to a target button.
/// When the user clicks the target, a browser confirm() prompt appears.
/// If the user cancels, the click event is suppressed.
/// Emulates the Ajax Control Toolkit ConfirmButtonExtender.
/// </summary>
public class ConfirmButtonExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/confirm-button-extender.js";

	/// <summary>
	/// The confirmation message displayed to the user.
	/// </summary>
	[Parameter]
	public string ConfirmText { get; set; } = "Are you sure?";

	/// <summary>
	/// Whether to confirm on form submit instead of button click.
	/// </summary>
	[Parameter]
	public bool ConfirmOnFormSubmit { get; set; }

	/// <summary>
	/// Optional ID of a ModalPopup to display instead of browser confirm().
	/// Not implemented in v1 — reserved for future use.
	/// </summary>
	[Parameter]
	public string DisplayModalPopupID { get; set; } = string.Empty;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		confirmText = ConfirmText,
		confirmOnFormSubmit = ConfirmOnFormSubmit,
		displayModalPopupId = DisplayModalPopupID
	};
}

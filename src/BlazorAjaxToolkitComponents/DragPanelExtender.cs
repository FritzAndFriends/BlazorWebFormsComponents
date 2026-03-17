using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Makes a panel or element draggable by its title bar or handle.
/// Users can click and drag the panel to reposition it on the page.
/// Emulates the Ajax Control Toolkit DragPanelExtender.
/// </summary>
public class DragPanelExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/drag-panel-extender.js";

	/// <summary>
	/// The ID of an optional element that acts as the drag handle.
	/// If not set, the entire target panel is draggable.
	/// </summary>
	[Parameter]
	public string DragHandleID { get; set; } = string.Empty;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		dragHandleId = DragHandleID
	};
}

using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Adds collapsible behavior to a target panel, toggling between collapsed and expanded states
/// with smooth CSS transitions. Supports separate collapse/expand triggers and dynamic label text.
/// Emulates the Ajax Control Toolkit CollapsiblePanelExtender.
/// </summary>
public class CollapsiblePanelExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/collapsible-panel-extender.js";

	/// <summary>
	/// The ID of the element that triggers collapse.
	/// </summary>
	[Parameter]
	public string CollapseControlID { get; set; } = string.Empty;

	/// <summary>
	/// The ID of the element that triggers expand. Can be the same as CollapseControlID for toggle behavior.
	/// </summary>
	[Parameter]
	public string ExpandControlID { get; set; } = string.Empty;

	/// <summary>
	/// Whether the panel starts in the collapsed state.
	/// </summary>
	[Parameter]
	public bool Collapsed { get; set; }

	/// <summary>
	/// The height (or width for horizontal) in pixels when collapsed. 0 means fully hidden.
	/// </summary>
	[Parameter]
	public int CollapsedSize { get; set; }

	/// <summary>
	/// The height (or width for horizontal) in pixels when expanded. 0 means auto-size.
	/// </summary>
	[Parameter]
	public int ExpandedSize { get; set; }

	/// <summary>
	/// Text displayed in the TextLabelID element when the panel is collapsed.
	/// </summary>
	[Parameter]
	public string CollapsedText { get; set; } = string.Empty;

	/// <summary>
	/// Text displayed in the TextLabelID element when the panel is expanded.
	/// </summary>
	[Parameter]
	public string ExpandedText { get; set; } = string.Empty;

	/// <summary>
	/// The ID of the element whose text content changes to reflect collapsed/expanded state.
	/// </summary>
	[Parameter]
	public string TextLabelID { get; set; } = string.Empty;

	/// <summary>
	/// The direction of the collapse/expand animation.
	/// </summary>
	[Parameter]
	public ExpandDirection ExpandDirection { get; set; } = ExpandDirection.Vertical;

	/// <summary>
	/// Whether to automatically collapse the panel when the mouse leaves.
	/// </summary>
	[Parameter]
	public bool AutoCollapse { get; set; }

	/// <summary>
	/// Whether to automatically expand the panel when the mouse enters.
	/// </summary>
	[Parameter]
	public bool AutoExpand { get; set; }

	/// <summary>
	/// Whether to enable scrolling within the panel when content overflows.
	/// </summary>
	[Parameter]
	public bool ScrollContents { get; set; }

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		collapseControlId = CollapseControlID,
		expandControlId = ExpandControlID,
		collapsed = Collapsed,
		collapsedSize = CollapsedSize,
		expandedSize = ExpandedSize,
		collapsedText = CollapsedText,
		expandedText = ExpandedText,
		textLabelId = TextLabelID,
		expandDirection = (int)ExpandDirection,
		autoCollapse = AutoCollapse,
		autoExpand = AutoExpand,
		scrollContents = ScrollContents
	};
}

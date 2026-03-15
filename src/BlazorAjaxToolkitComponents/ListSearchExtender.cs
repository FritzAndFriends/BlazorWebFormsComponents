using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Enables search/filter functionality on a ListBox or DropDownList.
/// As the user types in the search field, items that don't match are filtered
/// or the selection jumps to the first matching item.
/// Emulates the Ajax Control Toolkit ListSearchExtender.
/// </summary>
public class ListSearchExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/list-search-extender.js";

	/// <summary>
	/// The placeholder text shown in the search input when it is empty.
	/// </summary>
	[Parameter]
	public string PromptText { get; set; } = "Type to search";

	/// <summary>
	/// CSS class applied to the search prompt text.
	/// </summary>
	[Parameter]
	public string PromptCssClass { get; set; } = string.Empty;

	/// <summary>
	/// Specifies where the search prompt is displayed relative to the list control.
	/// </summary>
	[Parameter]
	public PromptPosition PromptPosition { get; set; } = PromptPosition.Top;

	/// <summary>
	/// Indicates whether the list items are sorted.
	/// When true, enables optimized binary search for StartsWith pattern.
	/// </summary>
	[Parameter]
	public bool IsSorted { get; set; } = false;

	/// <summary>
	/// Specifies how search text matches against list items.
	/// Contains matches anywhere in the text; StartsWith matches only from the beginning.
	/// </summary>
	[Parameter]
	public QueryPattern QueryPattern { get; set; } = QueryPattern.Contains;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		promptText = PromptText,
		promptCssClass = PromptCssClass,
		promptPosition = PromptPosition.ToString().ToLowerInvariant(),
		isSorted = IsSorted,
		queryPattern = QueryPattern.ToString().ToLowerInvariant()
	};
}

using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Provides typeahead/autocomplete functionality for a target TextBox.
/// Fetches suggestions via a Blazor EventCallback and renders a dropdown list
/// with keyboard navigation support.
/// Emulates the Ajax Control Toolkit AutoCompleteExtender.
/// </summary>
public class AutoCompleteExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/autocomplete-extender.js";

	/// <summary>
	/// The URL of the web service providing completion data (for JS-based fetching).
	/// </summary>
	[Parameter]
	public string ServicePath { get; set; } = string.Empty;

	/// <summary>
	/// The web service method name to call for completion data.
	/// </summary>
	[Parameter]
	public string ServiceMethod { get; set; } = string.Empty;

	/// <summary>
	/// Minimum number of characters typed before suggestions are fetched. Default is 3.
	/// </summary>
	[Parameter]
	public int MinimumPrefixLength { get; set; } = 3;

	/// <summary>
	/// Maximum number of suggestions to display. Default is 10.
	/// </summary>
	[Parameter]
	public int CompletionSetCount { get; set; } = 10;

	/// <summary>
	/// Time in milliseconds between keystrokes before fetching suggestions.
	/// </summary>
	[Parameter]
	public int CompletionInterval { get; set; } = 1000;

	/// <summary>
	/// CSS class applied to the completion list dropdown container.
	/// </summary>
	[Parameter]
	public string CompletionListCssClass { get; set; } = string.Empty;

	/// <summary>
	/// CSS class applied to each item in the completion list.
	/// </summary>
	[Parameter]
	public string CompletionListItemCssClass { get; set; } = string.Empty;

	/// <summary>
	/// CSS class applied to the highlighted item in the completion list.
	/// </summary>
	[Parameter]
	public string CompletionListHighlightedItemCssClass { get; set; } = string.Empty;

	/// <summary>
	/// Characters that delimit separate completion entries within the same text box.
	/// </summary>
	[Parameter]
	public string DelimiterCharacters { get; set; } = string.Empty;

	/// <summary>
	/// Whether to automatically select the first item in the completion list.
	/// </summary>
	[Parameter]
	public bool FirstRowSelected { get; set; }

	/// <summary>
	/// When true, shows only the current word in each completion list item
	/// instead of the full value.
	/// </summary>
	[Parameter]
	public bool ShowOnlyCurrentWordInCompletionListItem { get; set; }

	/// <summary>
	/// Client-side script invoked when an item is selected from the completion list.
	/// </summary>
	[Parameter]
	public string OnClientItemSelected { get; set; } = string.Empty;

	/// <summary>
	/// Client-side script invoked before the completion list is populated.
	/// </summary>
	[Parameter]
	public string OnClientPopulating { get; set; } = string.Empty;

	/// <summary>
	/// Client-side script invoked after the completion list is populated.
	/// </summary>
	[Parameter]
	public string OnClientPopulated { get; set; } = string.Empty;

	/// <summary>
	/// Blazor callback for requesting completion data server-side.
	/// The string parameter is the current prefix typed by the user.
	/// </summary>
	[Parameter]
	public EventCallback<string> CompletionDataRequested { get; set; }

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		servicePath = ServicePath,
		serviceMethod = ServiceMethod,
		minimumPrefixLength = MinimumPrefixLength,
		completionSetCount = CompletionSetCount,
		completionInterval = CompletionInterval,
		completionListCssClass = CompletionListCssClass,
		completionListItemCssClass = CompletionListItemCssClass,
		completionListHighlightedItemCssClass = CompletionListHighlightedItemCssClass,
		delimiterCharacters = DelimiterCharacters,
		firstRowSelected = FirstRowSelected,
		showOnlyCurrentWordInCompletionListItem = ShowOnlyCurrentWordInCompletionListItem,
		onClientItemSelected = OnClientItemSelected,
		onClientPopulating = OnClientPopulating,
		onClientPopulated = OnClientPopulated
	};
}

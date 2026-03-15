using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Applies an input mask to a target TextBox, restricting and formatting user input
/// according to a mask pattern. Supports number, date, time, and custom masks.
/// Emulates the Ajax Control Toolkit MaskedEditExtender.
/// </summary>
public class MaskedEditExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/masked-edit-extender.js";

	/// <summary>
	/// The mask pattern string (e.g., "999-999-9999" for phone numbers).
	/// 9 = digit, L = letter, $ = digit or space, C = any character, A = letter or digit.
	/// </summary>
	[Parameter]
	public string Mask { get; set; } = string.Empty;

	/// <summary>
	/// The type of mask to apply.
	/// </summary>
	[Parameter]
	public MaskType MaskType { get; set; } = MaskType.None;

	/// <summary>
	/// The direction of text input within the mask.
	/// </summary>
	[Parameter]
	public InputDirection InputDirection { get; set; } = InputDirection.LeftToRight;

	/// <summary>
	/// The character displayed for unfilled mask positions. Default is "_".
	/// </summary>
	[Parameter]
	public string PromptCharacter { get; set; } = "_";

	/// <summary>
	/// Whether to enable browser autocomplete on the target input.
	/// </summary>
	[Parameter]
	public bool AutoComplete { get; set; }

	/// <summary>
	/// The value to use for autocomplete when AutoComplete is enabled.
	/// </summary>
	[Parameter]
	public string AutoCompleteValue { get; set; } = string.Empty;

	/// <summary>
	/// Additional characters allowed beyond the mask definition.
	/// </summary>
	[Parameter]
	public string Filtered { get; set; } = string.Empty;

	/// <summary>
	/// Whether to remove the mask characters when the input loses focus,
	/// showing only the raw value.
	/// </summary>
	[Parameter]
	public bool ClearMaskOnLostFocus { get; set; } = true;

	/// <summary>
	/// Whether to clear the text when it does not match the mask on lost focus.
	/// </summary>
	[Parameter]
	public bool ClearTextOnInvalid { get; set; }

	/// <summary>
	/// Whether to accept AM/PM designator for time masks.
	/// </summary>
	[Parameter]
	public bool AcceptAMPM { get; set; }

	/// <summary>
	/// How negative values are displayed for number masks.
	/// </summary>
	[Parameter]
	public AcceptNegative AcceptNegative { get; set; } = AcceptNegative.None;

	/// <summary>
	/// How currency symbols are displayed for number masks.
	/// </summary>
	[Parameter]
	public DisplayMoney DisplayMoney { get; set; } = DisplayMoney.None;

	/// <summary>
	/// CSS class applied when the input has focus.
	/// </summary>
	[Parameter]
	public string OnFocusCssClass { get; set; } = string.Empty;

	/// <summary>
	/// CSS class applied when the input contains an invalid value.
	/// </summary>
	[Parameter]
	public string OnInvalidCssClass { get; set; } = string.Empty;

	/// <summary>
	/// CSS class applied when the input contains a negative value.
	/// </summary>
	[Parameter]
	public string OnBlurCssNegative { get; set; } = string.Empty;

	/// <summary>
	/// The culture name used for formatting (e.g., "en-US").
	/// </summary>
	[Parameter]
	public string CultureName { get; set; } = string.Empty;

	/// <summary>
	/// Whether to show a tooltip with error information when validation fails.
	/// </summary>
	[Parameter]
	public bool ErrorTooltipEnabled { get; set; }

	/// <summary>
	/// CSS class applied to the error tooltip.
	/// </summary>
	[Parameter]
	public string ErrorTooltipCssClass { get; set; } = string.Empty;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		mask = Mask,
		maskType = (int)MaskType,
		inputDirection = (int)InputDirection,
		promptCharacter = PromptCharacter,
		autoComplete = AutoComplete,
		autoCompleteValue = AutoCompleteValue,
		filtered = Filtered,
		clearMaskOnLostFocus = ClearMaskOnLostFocus,
		clearTextOnInvalid = ClearTextOnInvalid,
		acceptAMPM = AcceptAMPM,
		acceptNegative = (int)AcceptNegative,
		displayMoney = (int)DisplayMoney,
		onFocusCssClass = OnFocusCssClass,
		onInvalidCssClass = OnInvalidCssClass,
		onBlurCssNegative = OnBlurCssNegative,
		cultureName = CultureName,
		errorTooltipEnabled = ErrorTooltipEnabled,
		errorTooltipCssClass = ErrorTooltipCssClass
	};
}

using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Displays a visual indicator of password strength as the user types in a password TextBox.
/// Evaluates passwords against configurable rules (length, character requirements) and shows feedback.
/// Emulates the Ajax Control Toolkit PasswordStrength extender.
/// </summary>
public class PasswordStrength : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/password-strength.js";

	/// <summary>
	/// Specifies where to display the strength indicator relative to the password field.
	/// Default is RightSide.
	/// </summary>
	[Parameter]
	public DisplayPosition DisplayPosition { get; set; } = DisplayPosition.RightSide;

	/// <summary>
	/// Specifies the type of strength indicator: Text labels or BarIndicator.
	/// Default is Text.
	/// </summary>
	[Parameter]
	public StrengthIndicatorType StrengthIndicatorType { get; set; } = StrengthIndicatorType.Text;

	/// <summary>
	/// The preferred minimum password length. Default is 10.
	/// </summary>
	[Parameter]
	public int PreferredPasswordLength { get; set; } = 10;

	/// <summary>
	/// The minimum number of numeric characters required. Default is 0.
	/// </summary>
	[Parameter]
	public int MinimumNumericCharacters { get; set; } = 0;

	/// <summary>
	/// The minimum number of symbol characters required. Default is 0.
	/// </summary>
	[Parameter]
	public int MinimumSymbolCharacters { get; set; } = 0;

	/// <summary>
	/// The minimum number of uppercase characters required. Default is 0.
	/// </summary>
	[Parameter]
	public int MinimumUpperCaseCharacters { get; set; } = 0;

	/// <summary>
	/// The minimum number of lowercase characters required. Default is 0.
	/// </summary>
	[Parameter]
	public int MinimumLowerCaseCharacters { get; set; } = 0;

	/// <summary>
	/// Whether the password must contain both uppercase and lowercase characters.
	/// Default is false.
	/// </summary>
	[Parameter]
	public bool RequiresUpperAndLowerCaseCharacters { get; set; } = false;

	/// <summary>
	/// Semicolon-delimited strength level descriptions.
	/// Example: "Very Poor;Weak;Average;Strong;Excellent"
	/// </summary>
	[Parameter]
	public string TextStrengthDescriptions { get; set; } = string.Empty;

	/// <summary>
	/// Semicolon-delimited CSS class names for each strength level.
	/// Aligns with TextStrengthDescriptions by index.
	/// </summary>
	[Parameter]
	public string StrengthStyles { get; set; } = string.Empty;

	/// <summary>
	/// CSS class for the bar indicator border.
	/// </summary>
	[Parameter]
	public string BarBorderCssClass { get; set; } = string.Empty;

	/// <summary>
	/// Custom calculation weightings for strength scoring (advanced).
	/// Format depends on implementation requirements.
	/// </summary>
	[Parameter]
	public string CalculationWeightings { get; set; } = string.Empty;

	/// <summary>
	/// CSS class applied to the text display element.
	/// </summary>
	[Parameter]
	public string TextCssClass { get; set; } = string.Empty;

	/// <summary>
	/// CSS class for the help handle/icon.
	/// </summary>
	[Parameter]
	public string HelpHandleCssClass { get; set; } = string.Empty;

	/// <summary>
	/// Position of the help handle relative to the password field.
	/// Default is RightSide.
	/// </summary>
	[Parameter]
	public DisplayPosition HelpHandlePosition { get; set; } = DisplayPosition.RightSide;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		displayPosition = DisplayPosition.ToString(),
		strengthIndicatorType = StrengthIndicatorType.ToString(),
		preferredPasswordLength = PreferredPasswordLength,
		minimumNumericCharacters = MinimumNumericCharacters,
		minimumSymbolCharacters = MinimumSymbolCharacters,
		minimumUpperCaseCharacters = MinimumUpperCaseCharacters,
		minimumLowerCaseCharacters = MinimumLowerCaseCharacters,
		requiresUpperAndLowerCaseCharacters = RequiresUpperAndLowerCaseCharacters,
		textStrengthDescriptions = TextStrengthDescriptions,
		strengthStyles = StrengthStyles,
		barBorderCssClass = BarBorderCssClass,
		calculationWeightings = CalculationWeightings,
		textCssClass = TextCssClass,
		helpHandleCssClass = HelpHandleCssClass,
		helpHandlePosition = HelpHandlePosition.ToString()
	};
}

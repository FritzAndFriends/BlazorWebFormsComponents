using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Provides visual feedback animations when content is updating.
/// Emulates the Ajax Control Toolkit UpdatePanelAnimationExtender.
/// Apply CSS classes and fade effects during and after content updates.
/// </summary>
public class UpdatePanelAnimationExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/update-panel-animation-extender.js";

	/// <summary>
	/// Whether to complete the "updating" animation before starting the "updated" animation.
	/// Default is false.
	/// </summary>
	[Parameter]
	public bool AlwaysFinishOnUpdatingAnimation { get; set; } = false;

	/// <summary>
	/// CSS class to apply during update/loading state.
	/// </summary>
	[Parameter]
	public string OnUpdatingCssClass { get; set; } = string.Empty;

	/// <summary>
	/// CSS class to apply after update completes.
	/// </summary>
	[Parameter]
	public string OnUpdatedCssClass { get; set; } = string.Empty;

	/// <summary>
	/// Duration in seconds for the fade-in effect after update completes. Default is 0.3.
	/// </summary>
	[Parameter]
	public double FadeInDuration { get; set; } = 0.3;

	/// <summary>
	/// Duration in seconds for the fade-out effect before update starts. Default is 0.3.
	/// </summary>
	[Parameter]
	public double FadeOutDuration { get; set; } = 0.3;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		alwaysFinishOnUpdatingAnimation = AlwaysFinishOnUpdatingAnimation,
		onUpdatingCssClass = OnUpdatingCssClass,
		onUpdatedCssClass = OnUpdatedCssClass,
		fadeInDuration = FadeInDuration,
		fadeOutDuration = FadeOutDuration
	};
}

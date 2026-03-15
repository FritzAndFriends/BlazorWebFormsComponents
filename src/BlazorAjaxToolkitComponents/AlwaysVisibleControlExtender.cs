using BlazorAjaxToolkitComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorAjaxToolkitComponents;

/// <summary>
/// Keeps a control visible in a fixed position on screen even when the user scrolls.
/// Useful for toolbars, help buttons, or notification areas.
/// Emulates the Ajax Control Toolkit AlwaysVisibleControlExtender.
/// </summary>
public class AlwaysVisibleControlExtender : BaseExtenderComponent
{
	private const string MODULE_PATH =
		"./_content/BlazorAjaxToolkitComponents/js/always-visible-control-extender.js";

	/// <summary>
	/// The number of pixels from the horizontal edge.
	/// </summary>
	[Parameter]
	public int HorizontalOffset { get; set; } = 0;

	/// <summary>
	/// The number of pixels from the vertical edge.
	/// </summary>
	[Parameter]
	public int VerticalOffset { get; set; } = 0;

	/// <summary>
	/// Which horizontal edge to position the control against.
	/// </summary>
	[Parameter]
	public HorizontalSide HorizontalSide { get; set; } = HorizontalSide.Left;

	/// <summary>
	/// Which vertical edge to position the control against.
	/// </summary>
	[Parameter]
	public VerticalSide VerticalSide { get; set; } = VerticalSide.Top;

	/// <summary>
	/// Duration of the position animation in seconds.
	/// </summary>
	[Parameter]
	public float ScrollEffectDuration { get; set; } = 0.1f;

	/// <summary>
	/// Whether to animate position changes.
	/// </summary>
	[Parameter]
	public bool UseAnimation { get; set; } = true;

	protected override string JsModulePath => MODULE_PATH;

	protected override string JsCreateFunction => "createBehavior";

	protected override object GetBehaviorProperties() => new
	{
		horizontalOffset = HorizontalOffset,
		verticalOffset = VerticalOffset,
		horizontalSide = HorizontalSide.ToString().ToLowerInvariant(),
		verticalSide = VerticalSide.ToString().ToLowerInvariant(),
		scrollEffectDuration = ScrollEffectDuration,
		useAnimation = UseAnimation
	};
}

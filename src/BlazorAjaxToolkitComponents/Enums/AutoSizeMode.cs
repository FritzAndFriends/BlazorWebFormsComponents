namespace BlazorAjaxToolkitComponents.Enums;

/// <summary>
/// Specifies how the Accordion automatically sizes its content panes.
/// </summary>
public enum AutoSizeMode
{
	/// <summary>
	/// No automatic sizing. Panes use their natural height.
	/// </summary>
	None = 0,

	/// <summary>
	/// Panes are sized to fill the available space in the Accordion container.
	/// </summary>
	Fill = 1,

	/// <summary>
	/// Panes are limited to the Accordion container height but do not expand to fill it.
	/// </summary>
	Limit = 2
}

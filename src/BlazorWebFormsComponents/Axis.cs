namespace BlazorWebFormsComponents;

/// <summary>
/// Represents an axis in a ChartArea. This is a POCO configuration class, not a component.
/// </summary>
public class Axis
{
	/// <summary>
	/// Gets or sets the title of the axis.
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// Gets or sets the minimum value of the axis.
	/// </summary>
	public double? Minimum { get; set; }

	/// <summary>
	/// Gets or sets the maximum value of the axis.
	/// </summary>
	public double? Maximum { get; set; }

	/// <summary>
	/// Gets or sets the interval of the axis.
	/// </summary>
	public double? Interval { get; set; }

	/// <summary>
	/// Gets or sets whether the axis uses a logarithmic scale.
	/// </summary>
	public bool IsLogarithmic { get; set; }
}

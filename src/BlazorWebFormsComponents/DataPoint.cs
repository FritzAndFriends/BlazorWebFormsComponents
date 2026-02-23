namespace BlazorWebFormsComponents;

/// <summary>
/// Represents a data point in a chart series. Matches the ASP.NET Web Forms DataPoint class.
/// </summary>
public class DataPoint
{
	/// <summary>
	/// Gets or sets the X value of the data point.
	/// </summary>
	public object XValue { get; set; }

	/// <summary>
	/// Gets or sets the Y values of the data point.
	/// </summary>
	public double[] YValues { get; set; } = [];

	/// <summary>
	/// Gets or sets the label of the data point.
	/// </summary>
	public string Label { get; set; }

	/// <summary>
	/// Gets or sets the color of the data point.
	/// </summary>
	public WebColor Color { get; set; }

	/// <summary>
	/// Gets or sets the tooltip of the data point.
	/// </summary>
	public string ToolTip { get; set; }

	/// <summary>
	/// Gets or sets whether the value is shown as a label.
	/// </summary>
	public bool IsValueShownAsLabel { get; set; }
}

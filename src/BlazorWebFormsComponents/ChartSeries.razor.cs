using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents;

public partial class ChartSeries : BaseWebFormsComponent
{
	[CascadingParameter(Name = "ParentChart")]
	public Chart ParentChart { get; set; }

	/// <summary>
	/// Gets or sets the name of the series.
	/// </summary>
	[Parameter]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the chart type for this series.
	/// </summary>
	[Parameter]
	public SeriesChartType ChartType { get; set; } = SeriesChartType.Column;

	/// <summary>
	/// Gets or sets the name of the chart area this series belongs to.
	/// </summary>
	[Parameter]
	public string ChartArea { get; set; }

	/// <summary>
	/// Gets or sets the field name for X values when data-binding.
	/// </summary>
	[Parameter]
	public string XValueMember { get; set; }

	/// <summary>
	/// Gets or sets the field names for Y values when data-binding.
	/// </summary>
	[Parameter]
	public string YValueMembers { get; set; }

	/// <summary>
	/// Gets or sets the color of the series.
	/// </summary>
	[Parameter]
	public WebColor Color { get; set; }

	/// <summary>
	/// Gets or sets the border width of the series.
	/// </summary>
	[Parameter]
	public int BorderWidth { get; set; }

	/// <summary>
	/// Gets or sets whether the series is visible in the legend.
	/// </summary>
	[Parameter]
	public bool IsVisibleInLegend { get; set; } = true;

	/// <summary>
	/// Gets or sets the name of the legend this series is associated with.
	/// </summary>
	[Parameter]
	public string Legend { get; set; }

	/// <summary>
	/// Gets or sets the marker style for the series.
	/// </summary>
	[Parameter]
	public string MarkerStyle { get; set; }

	/// <summary>
	/// Gets or sets the tooltip for the series.
	/// </summary>
	[Parameter]
	public string ToolTip { get; set; }

	/// <summary>
	/// Gets or sets the rendering order of this series in mixed charts.
	/// Lower values are drawn last (on top). When null, an automatic order
	/// is assigned based on chart type (line/area = 0, bar/column = 1).
	/// </summary>
	[Parameter]
	public int? Order { get; set; }

	/// <summary>
	/// Gets or sets the data points for this series.
	/// </summary>
	[Parameter]
	public List<DataPoint> Points { get; set; } = new();

	/// <summary>
	/// Gets or sets the data source items for data-binding.
	/// </summary>
	[Parameter]
	public IEnumerable<object> Items { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		ParentChart?.RegisterSeries(this);
	}

	internal ChartSeriesConfig ToConfig()
	{
		var config = new ChartSeriesConfig
		{
			Name = Name,
			ChartType = ChartType,
			Color = Color,
			BorderWidth = BorderWidth,
			IsVisibleInLegend = IsVisibleInLegend,
			ChartArea = ChartArea,
			Order = Order
		};

		// If Items is provided, extract data points from it
		if (Items != null && !string.IsNullOrEmpty(YValueMembers))
		{
			config.Points = ExtractDataPointsFromItems();
		}
		else
		{
			// Fall back to manually-specified Points
			config.Points = Points;
		}

		return config;
	}

	private List<DataPoint> ExtractDataPointsFromItems()
	{
		var result = new List<DataPoint>();
		if (Items == null)
		{
			return result;
		}

		// Parse YValueMembers (can be comma-separated for multi-value charts)
		var yMembers = YValueMembers?.Split(',', StringSplitOptions.RemoveEmptyEntries)
			.Select(m => m.Trim())
			.ToArray() ?? Array.Empty<string>();

		if (yMembers.Length == 0)
		{
			return result;
		}

		foreach (var item in Items)
		{
			if (item == null)
			{
				continue;
			}

			var dataPoint = new DataPoint();
			var itemType = item.GetType();

			// Extract X value if XValueMember is specified
			if (!string.IsNullOrEmpty(XValueMember))
			{
				var xProperty = itemType.GetProperty(XValueMember, BindingFlags.Public | BindingFlags.Instance);
				if (xProperty != null)
				{
					dataPoint.XValue = xProperty.GetValue(item);
				}
			}

			// Extract Y values based on YValueMembers
			var yValues = new List<double>();
			foreach (var yMember in yMembers)
			{
				var yProperty = itemType.GetProperty(yMember, BindingFlags.Public | BindingFlags.Instance);
				if (yProperty != null)
				{
					var rawValue = yProperty.GetValue(item);
					if (rawValue != null && TryConvertToDouble(rawValue, out var yValue))
					{
						yValues.Add(yValue);
					}
					else
					{
						yValues.Add(0.0);
					}
				}
				else
				{
					yValues.Add(0.0);
				}
			}

			dataPoint.YValues = yValues.ToArray();
			result.Add(dataPoint);
		}

		return result;
	}

	private static bool TryConvertToDouble(object value, out double result)
	{
		result = 0.0;
		if (value == null)
		{
			return false;
		}

		// Handle common numeric types directly
		switch (value)
		{
			case double d:
				result = d;
				return true;
			case float f:
				result = f;
				return true;
			case int i:
				result = i;
				return true;
			case long l:
				result = l;
				return true;
			case decimal dec:
				result = (double)dec;
				return true;
			case short s:
				result = s;
				return true;
			case byte b:
				result = b;
				return true;
			default:
				// Fallback: try Convert.ToDouble
				try
				{
					result = Convert.ToDouble(value);
					return true;
				}
				catch
				{
					return false;
				}
		}
	}
}

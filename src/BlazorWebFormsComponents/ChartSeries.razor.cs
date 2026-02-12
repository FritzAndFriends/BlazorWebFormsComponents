using System.Collections.Generic;
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
		return new ChartSeriesConfig
		{
			Name = Name,
			ChartType = ChartType,
			Points = Points,
			Color = Color,
			BorderWidth = BorderWidth,
			IsVisibleInLegend = IsVisibleInLegend,
			ChartArea = ChartArea
		};
	}
}

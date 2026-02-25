using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents;

public partial class ChartArea : BaseWebFormsComponent
{
	[CascadingParameter(Name = "ParentChart")]
	public Chart ParentChart { get; set; }

	/// <summary>
	/// Gets or sets the unique name of the chart area.
	/// </summary>
	[Parameter]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the background color of the chart area.
	/// </summary>
	[Parameter]
	public WebColor AreaBackColor { get; set; }

	/// <summary>
	/// Gets or sets the X axis configuration.
	/// </summary>
	[Parameter]
	public Axis AxisX { get; set; }

	/// <summary>
	/// Gets or sets the Y axis configuration.
	/// </summary>
	[Parameter]
	public Axis AxisY { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		ParentChart?.RegisterChartArea(this);
	}

	internal ChartAreaConfig ToConfig()
	{
		return new ChartAreaConfig
		{
			Name = Name,
			BackColor = AreaBackColor,
			AxisX = AxisX,
			AxisY = AxisY
		};
	}
}

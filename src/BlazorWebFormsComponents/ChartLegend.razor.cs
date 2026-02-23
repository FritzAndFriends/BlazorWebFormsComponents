using System.Threading.Tasks;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents;

public partial class ChartLegend : BaseWebFormsComponent
{
	[CascadingParameter(Name = "ParentChart")]
	public Chart ParentChart { get; set; }

	/// <summary>
	/// Gets or sets the unique name of the legend.
	/// </summary>
	[Parameter]
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets where the legend is docked.
	/// </summary>
	[Parameter]
	public Docking? LegendDocking { get; set; }

	/// <summary>
	/// Gets or sets the alignment of the legend.
	/// </summary>
	[Parameter]
	public string Alignment { get; set; }

	/// <summary>
	/// Gets or sets the title of the legend.
	/// </summary>
	[Parameter]
	public string Title { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		ParentChart?.RegisterLegend(this);
	}

	internal ChartLegendConfig ToConfig()
	{
		return new ChartLegendConfig
		{
			Name = Name,
			Docking = LegendDocking,
			Title = Title
		};
	}
}

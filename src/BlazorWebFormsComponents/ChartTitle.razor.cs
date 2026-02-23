using System.Threading.Tasks;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents;

public partial class ChartTitle : BaseWebFormsComponent
{
	[CascadingParameter(Name = "ParentChart")]
	public Chart ParentChart { get; set; }

	/// <summary>
	/// Gets or sets the text of the title.
	/// </summary>
	[Parameter]
	public string Text { get; set; }

	/// <summary>
	/// Gets or sets the alignment of the title.
	/// </summary>
	[Parameter]
	public string Alignment { get; set; }

	/// <summary>
	/// Gets or sets where the title is docked.
	/// </summary>
	[Parameter]
	public Docking? TitleDocking { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		ParentChart?.RegisterTitle(this);
	}

	internal ChartTitleConfig ToConfig()
	{
		return new ChartTitleConfig
		{
			Text = Text,
			Docking = TitleDocking
		};
	}
}

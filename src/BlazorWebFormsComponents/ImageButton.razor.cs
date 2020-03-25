using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
namespace BlazorWebFormsComponents
{	public partial class ImageButton : ButtonBaseComponent, IImageComponent
	{
		[Parameter]
		public string AlternateText { get; set; }

		[Parameter]
		public string DescriptionUrl { get; set; } = string.Empty;

		[Parameter]
		public ImageAlign ImageAlign { get; set; }

		[Parameter]
		public string ImageUrl { get; set; }
	}
}

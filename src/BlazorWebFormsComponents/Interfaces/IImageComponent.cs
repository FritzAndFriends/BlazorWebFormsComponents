using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.Interfaces
{
	public interface IImageComponent
	{
		[Parameter]
		string AlternateText { get; set; }

		[Parameter]
		string DescriptionUrl { get; set; }

		[Parameter]
		EnumParameter<ImageAlign> ImageAlign { get; set; }

		[Parameter]
		string ImageUrl { get; set; }
	}
}

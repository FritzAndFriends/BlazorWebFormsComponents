using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class Literal : BaseWebFormsComponent, ITextComponent
	{
		[Parameter]
		public LiteralMode Mode { get; set; }

		[Parameter]
		public string Text { get; set; }
	}
}

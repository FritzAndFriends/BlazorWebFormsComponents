using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class Label : BaseStyledComponent, ITextComponent
	{
		[Parameter]
		public string Text { get; set; }

		[Parameter]
		public string AssociatedControlID { get; set; }
	}
}

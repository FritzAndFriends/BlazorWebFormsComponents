using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class Repeater<ItemType> : BaseModelBindingComponent<ItemType>
	{

		[Parameter]
		public RenderFragment<ItemType> ItemTemplate { get; set; }

		[Parameter]
		public RenderFragment<ItemType> AlternatingItemTemplate { get; set; }

		[Parameter]
		public RenderFragment HeaderTemplate { get; set; }

		[Parameter]
		public RenderFragment FooterTemplate { get; set; }

		[Parameter]
		public RenderFragment SeparatorTemplate { get; set; }

	}
}

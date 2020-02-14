using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{

	public partial class FormView<ItemType> : BaseModelBindingComponent<ItemType> where ItemType : class, new()
	{

		[Parameter]
		public RenderFragment<ItemType>	ItemTemplate { get; set; }

		public ItemType CurrentItem { get; set; } = null;

		//protected override Task OnParametersSetAsync()
		//{
		//	return base.OnParametersSetAsync();
		//}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{

			if (firstRender)
			{
				if ((CurrentItem is null) && Items != null && Items.Any()) CurrentItem = Items.FirstOrDefault();
			}

			await base.OnAfterRenderAsync(firstRender);

		}

	}

}

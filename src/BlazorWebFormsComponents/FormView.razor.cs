using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{

	public partial class FormView<ItemType> : BaseModelBindingComponent<ItemType>
	{

		[Parameter]
		public RenderFragment<ItemType>	ItemTemplate { get; set; }

		public ItemType CurrentItem { get; set; }

		protected override Task OnParametersSetAsync()
		{

			if (CurrentItem == null && Items != null) CurrentItem = Items.FirstOrDefault();

			return base.OnParametersSetAsync();
		}

	}

}

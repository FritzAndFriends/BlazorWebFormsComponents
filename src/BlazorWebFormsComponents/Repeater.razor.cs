using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{
	public partial class Repeater : BaseModelBindingComponent<dynamic>
	{

		[Parameter]
		public RenderFragment<dynamic> ItemTemplate { get; set; }

		[Parameter]
		public RenderFragment<dynamic> AlternatingItemTemplate { get; set; }

		[Parameter]
		public RenderFragment HeaderTemplate { get; set; }

		[Parameter]
		public RenderFragment FooterTemplate { get; set; }

		[Parameter]
		public RenderFragment SeparatorTemplate { get; set; }

	}
}

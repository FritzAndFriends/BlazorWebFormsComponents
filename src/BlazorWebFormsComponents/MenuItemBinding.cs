using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	public class MenuItemBinding : ComponentBase
	{

		[CascadingParameter(Name = "ParentMenu")]
		public Menu ParentMenu { get; set; }

		[Parameter]
		public string DataMember { get; set; }

		[Parameter]
		public string ImageUrlField { get; set; }

		[Parameter]
		public string NavigateUrlField { get; set; }

		[Parameter]
		public string TargetField { get; set; }

		[Parameter]
		public string TextField { get; set; }

		[Parameter]
		public string ToolTipField { get; set; }

		[Parameter]
		public string ValueField { get; set; }

		protected override Task OnParametersSetAsync()
		{

			ParentMenu.AddMenuItemBinding(this);

			return base.OnParametersSetAsync();
		}

		public override string ToString()
		{

			return $"<MenuItemBinding DataMember=\"{DataMember}\" TextField=\"{TextField}\" NavigateUrlField=\"{NavigateUrlField}\"/>";

		}

	}

}

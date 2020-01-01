using BlazorWebFormsComponents.Enum;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Text;

namespace BlazorWebFormsComponents
{
	public partial class DataList<ItemType> : BaseModelBindingComponent<ItemType>
	{

		[Parameter]
		public RenderFragment HeaderTemplate { get; set; }

		[CascadingParameter(Name = "HeaderStyle")]
		private TableItemStyle HeaderStyle { get; set; } = new TableItemStyle();

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[Parameter]
		public RenderFragment<ItemType> ItemTemplate { get; set; }

		[Parameter]
		public RepeatLayout RepeatLayout { get; set; } = BlazorWebFormsComponents.Enum.RepeatLayout.Table;

		protected override void HandleUnknownAttributes()
		{

			if (AdditionalAttributes.ContainsKey("HeaderStyle-BackColor")) {
				HeaderStyle.BackColor = (Color)AdditionalAttributes["HeaderStyle-BackColor"];
			}

			base.HandleUnknownAttributes();
		}

	}

}

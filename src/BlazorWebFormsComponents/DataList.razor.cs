using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BlazorWebFormsComponents
{
	public partial class DataList<ItemType> : BaseModelBindingComponent<ItemType>
	{

		[Parameter]
		public RenderFragment HeaderTemplate { get; set; }

		[Parameter]
		public RenderFragment FooterTemplate { get; set; }

		[CascadingParameter(Name = "HeaderStyle")]
		private TableItemStyle HeaderStyle { get; set; } = new TableItemStyle();

		[CascadingParameter(Name = "FooterStyle")]
		private TableItemStyle FooterStyle { get; set; } = new TableItemStyle();

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[Parameter]
		public RenderFragment<ItemType> ItemTemplate { get; set; }

		[Parameter]
		public RepeatLayout RepeatLayout { get; set; } = BlazorWebFormsComponents.Enums.RepeatLayout.Table;

		[Parameter]
		public bool ShowHeader { get; set; } = true;

		[Parameter]
		public bool ShowFooter { get; set; } = true;

		[Parameter]
		public string Style { get; set; }

		[Parameter]
		public string ToolTip { get; set; }

		[Parameter]
		public bool UseAccessibleHeader { get; set; } = false;


		protected override void HandleUnknownAttributes()
		{

			if (AdditionalAttributes?.Count > 0)
			{

				HeaderStyle.FromUnknownAttributes(AdditionalAttributes, "HeaderStyle-");
				FooterStyle.FromUnknownAttributes(AdditionalAttributes, "FooterStyle-");

			}

			base.HandleUnknownAttributes();
		}

	}

}

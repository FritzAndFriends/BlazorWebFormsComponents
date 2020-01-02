using BlazorWebFormsComponents.Enum;
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

			if (AdditionalAttributes?.Count > 0)
			{

				var headerStyles = AdditionalAttributes.Keys
					.Where(k => k.StartsWith("HeaderStyle-"))
					.Select(k => new KeyValuePair<string, object>(k.Replace("HeaderStyle-", ""), AdditionalAttributes[k]));

				if (AdditionalAttributes.ContainsKey("HeaderStyle-BackColor"))
				{
					HeaderStyle.BackColor = AdditionalAttributes["HeaderStyle-BackColor"].GetColorFromHtml();
				}

				if (AdditionalAttributes.ContainsKey("HeaderStyle-ForeColor"))
				{
					HeaderStyle.ForeColor = AdditionalAttributes["HeaderStyle-ForeColor"].GetColorFromHtml();
				}

			}

			base.HandleUnknownAttributes();
		}

	}

}

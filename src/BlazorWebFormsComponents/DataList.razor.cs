using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Net.Cache;
using System.Text;

namespace BlazorWebFormsComponents
{
	public partial class DataList<ItemType> : BaseModelBindingComponent<ItemType>, IHasStyle
	{

		[Parameter]
		public string AccessKey { get; set; }

		[Parameter]
		public string Caption { get; set; }

		[Parameter]
		public VerticalAlign CaptionAlign { get; set; } = VerticalAlign.NotSet;

		[Parameter]
		public int CellPadding { get; set; }

		[Parameter]
		public int CellSpacing { get; set; }

		[Parameter]
		public GridLines GridLines { get; set; } = GridLines.None;

		private static readonly Dictionary<GridLines, string?> _GridLines = new Dictionary<GridLines, string?> {
			{GridLines.None, null },
			{GridLines.Horizontal, "rows" },
			{GridLines.Vertical, "cols" },
			{GridLines.Both, "both" }
		};
		protected string? CalculatedGridLines {  get {

				return _GridLines[this.GridLines];

		} }

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

		[CascadingParameter(Name = "ItemStyle")]
		private TableItemStyle ItemStyle { get; set; } = new TableItemStyle();

		[Parameter]
		public RenderFragment<ItemType> ItemTemplate { get; set; }

		[Parameter]
		public RenderFragment<ItemType> AlternatingItemTemplate { get; set; }

		[CascadingParameter(Name = "AlternatingItemStyle")]
		private TableItemStyle AlternatingItemStyle { get; set; } = new TableItemStyle();


		[Parameter]
		public RepeatLayout RepeatLayout { get; set; } = BlazorWebFormsComponents.Enums.RepeatLayout.Table;

		[CascadingParameter(Name = "SeparatorStyle")]
		private TableItemStyle SeparatorStyle { get; set; } = new TableItemStyle();


		[Parameter]
		public RenderFragment SeparatorTemplate { get; set; }

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

		protected string CalculatedStyle { get; set; }

		[Parameter] public WebColor BackColor { get; set; }
		[Parameter] public WebColor BorderColor { get; set; }
		[Parameter]public BorderStyle BorderStyle { get; set; }
		[Parameter]public Unit BorderWidth { get; set; }
		[Parameter]public string CssClass { get; set; }
		[Parameter]public bool Font_Bold { get; set; }
		[Parameter]public bool Font_Italic { get; set; }
		[Parameter]public string Font_Names { get; set; }
		[Parameter]public bool Font_Overline { get; set; }
		[Parameter]public FontUnit Font_Size { get; set; }
		[Parameter]public bool Font_Strikeout { get; set; }
		[Parameter]public bool Font_Underline { get; set; }
		[Parameter]public WebColor ForeColor { get; set; }
		[Parameter]public Unit Height { get; set; }
		[Parameter] public Unit Width { get; set; }

		protected override void HandleUnknownAttributes()
		{

			if (AdditionalAttributes?.Count > 0)
			{

				HeaderStyle.FromUnknownAttributes(AdditionalAttributes, "HeaderStyle-");
				FooterStyle.FromUnknownAttributes(AdditionalAttributes, "FooterStyle-");
				ItemStyle.FromUnknownAttributes(AdditionalAttributes, "ItemStyle-");
				AlternatingItemStyle.FromUnknownAttributes(AdditionalAttributes, "AlternatingItemStyle-");
				SeparatorStyle.FromUnknownAttributes(AdditionalAttributes, "SeparatorStyle-");

			}

			base.HandleUnknownAttributes();
		}

		protected override void OnInitialized()
		{

			this.SetFontsFromAttributes(AdditionalAttributes);

			CalculatedStyle = this.ToStyle().AddStyle(Style).NullIfEmpty();

			base.OnInitialized();
		}

	}

}

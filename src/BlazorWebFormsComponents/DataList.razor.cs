using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents
{
	public partial class DataList<ItemType> : BaseModelBindingComponent<ItemType>, IHasStyle
	{
		private static readonly Dictionary<DataListEnum, string?> _GridLines = new Dictionary<DataListEnum, string?> {
			{DataListEnum.None, null },
			{DataListEnum.Horizontal, "rows" },
			{DataListEnum.Vertical, "cols" },
			{DataListEnum.Both, "both" }
		};
		protected string CalculatedStyle { get; set; }
		protected string? CalculatedGridLines { get => _GridLines[this.GridLines]; }
		[Parameter] public string AccessKey { get; set; }
		[Parameter] public string Caption { get; set; }
		[Parameter] public VerticalAlign CaptionAlign { get; set; } = VerticalAlign.NotSet;
		[Parameter] public int CellPadding { get; set; }
		[Parameter] public int CellSpacing { get; set; }
		[Parameter] public DataListEnum GridLines { get; set; } = DataListEnum.None;
		[Parameter] public RenderFragment HeaderTemplate { get; set; }
		[Parameter] public RenderFragment FooterTemplate { get; set; }
		[CascadingParameter(Name = "HeaderStyle")] private TableItemStyle HeaderStyle { get; set; } = new TableItemStyle();
		[CascadingParameter(Name = "FooterStyle")] private TableItemStyle FooterStyle { get; set; } = new TableItemStyle();
		[Parameter] public RenderFragment ChildContent { get; set; }
		[CascadingParameter(Name = "ItemStyle")] private TableItemStyle ItemStyle { get; set; } = new TableItemStyle();
		[Parameter] public RenderFragment<ItemType> ItemTemplate { get; set; }
		[Parameter] public RenderFragment<ItemType> AlternatingItemTemplate { get; set; }
		[CascadingParameter(Name = "AlternatingItemStyle")] private TableItemStyle AlternatingItemStyle { get; set; } = new TableItemStyle();
		[Parameter] public RepeatLayout RepeatLayout { get; set; } = BlazorWebFormsComponents.Enums.RepeatLayout.Table;
		[Parameter] public DataListEnum RepeatDirection { get; set; } = BlazorWebFormsComponents.Enums.DataListEnum.Vertical;
		[Parameter] public int RepeatColumns { get; set; } = 1;
		[CascadingParameter(Name = "SeparatorStyle")] private TableItemStyle SeparatorStyle { get; set; } = new TableItemStyle();
		[Parameter] public RenderFragment SeparatorTemplate { get; set; }
		[Parameter] public bool ShowHeader { get; set; } = true;
		[Parameter] public bool ShowFooter { get; set; } = true;
		[Parameter] public string Style { get; set; }
		[Parameter] public string ToolTip { get; set; }
		[Parameter] public bool UseAccessibleHeader { get; set; } = false;
		[Parameter] public WebColor BackColor { get; set; }
		[Parameter] public WebColor BorderColor { get; set; }
		[Parameter] public BorderStyle BorderStyle { get; set; }
		[Parameter] public Unit BorderWidth { get; set; }
		[Parameter] public string CssClass { get; set; }
		[Parameter] public bool Font_Bold { get; set; }
		[Parameter] public bool Font_Italic { get; set; }
		[Parameter] public string Font_Names { get; set; }
		[Parameter] public bool Font_Overline { get; set; }
		[Parameter] public FontUnit Font_Size { get; set; }
		[Parameter] public bool Font_Strikeout { get; set; }
		[Parameter] public bool Font_Underline { get; set; }
		[Parameter] public WebColor ForeColor { get; set; }
		[Parameter] public Unit Height { get; set; }
		[Parameter] public Unit Width { get; set; }

		private IList<ItemType> ElementIndex(int columns, IEnumerable<ItemType> items, DataListEnum direction)
		{
			var itemList = items.ToList();
			if (direction == DataListEnum.Horizontal)
			{
				return itemList;
			}

			var count = itemList.Count();
			var fullRows = count / columns;
			var extraRowItemCount = count % columns;
			var rowMax = fullRows + ((extraRowItemCount > 0) ? 1 : 0);

			var iter = 0;
			var returnList = new ItemType[count];
			for (var colC = 0; colC < columns; colC++)
			{
				if (colC == extraRowItemCount && extraRowItemCount != 0)
				{
					rowMax--;
				}

				for (var rowC = 0; rowC < rowMax; rowC++)
				{
					var pos = (rowC * columns) + colC;
					returnList[pos] = itemList[iter];
					iter++;
				}

			}

			return returnList;
		}

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

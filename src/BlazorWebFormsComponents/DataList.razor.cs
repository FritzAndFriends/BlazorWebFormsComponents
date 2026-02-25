using BlazorComponentUtilities;
using BlazorWebFormsComponents.DataBinding;
using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents
{
	public partial class DataList<ItemType> : DataBoundComponent<ItemType>, IDataListStyleContainer
	{
		private static readonly Dictionary<DataListEnum, string?> _GridLines = new Dictionary<DataListEnum, string?> {
			{DataListEnum.None, null },
			{DataListEnum.Horizontal, "rows" },
			{DataListEnum.Vertical, "cols" },
			{DataListEnum.Both, "both" }
		};
		protected string CalculatedStyle { get; set; }
		protected string? CalculatedGridLines { get => _GridLines[this.GridLines]; }
		[Parameter] public string Caption { get; set; }
		[Parameter] public VerticalAlign CaptionAlign { get; set; } = VerticalAlign.NotSet;
		[Parameter] public int CellPadding { get; set; }
		[Parameter] public int CellSpacing { get; set; }
		[Parameter] public DataListEnum GridLines { get; set; } = DataListEnum.None;
		[Parameter] public RenderFragment HeaderTemplate { get; set; }
		[Parameter] public RenderFragment FooterTemplate { get; set; }
		public TableItemStyle HeaderStyle { get; internal set; } = new TableItemStyle();
		public TableItemStyle FooterStyle { get; internal set; } = new TableItemStyle();
		[Parameter] public RenderFragment HeaderStyleContent { get; set; }
		[Parameter] public RenderFragment FooterStyleContent { get; set; }
		public TableItemStyle ItemStyle { get; internal set; } = new TableItemStyle();
		[Parameter] public RenderFragment<ItemType> ItemTemplate { get; set; }
		[Parameter] public RenderFragment<ItemType> AlternatingItemTemplate { get; set; }
		public TableItemStyle AlternatingItemStyle { get; internal set; } = new TableItemStyle();
		[Parameter] public RepeatLayout RepeatLayout { get; set; } = BlazorWebFormsComponents.Enums.RepeatLayout.Table;
		[Parameter] public DataListEnum RepeatDirection { get; set; } = BlazorWebFormsComponents.Enums.DataListEnum.Vertical;
		[Parameter] public int RepeatColumns { get; set; } = 1;
		public TableItemStyle SeparatorStyle { get; internal set; } = new TableItemStyle();
		[Parameter] public RenderFragment SeparatorTemplate { get; set; }
		[Parameter] public RenderFragment ItemStyleContent { get; set; }
		[Parameter] public RenderFragment AlternatingItemStyleContent { get; set; }
		[Parameter] public RenderFragment SeparatorStyleContent { get; set; }
		[Parameter] public bool ShowHeader { get; set; } = true;
		[Parameter] public bool ShowFooter { get; set; } = true;
		[Parameter] public new string Style { get; set; }
		[Parameter] public string ToolTip { get; set; }
		[Parameter] public bool UseAccessibleHeader { get; set; } = false;
		[Parameter]
		public EventCallback<DataListItemEventArgs> OnItemDataBound { get; set; }

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

			var currentItemIdx = 0;
			var returnList = new ItemType[count];
			for (var col = 0; col < columns; col++)
			{
				if (col == extraRowItemCount && extraRowItemCount != 0)
				{
					rowMax--;
				}

				for (var row = 0; row < rowMax; row++)
				{
					var pos = (row * columns) + col;
					returnList[pos] = itemList[currentItemIdx];
					currentItemIdx++;
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

		protected virtual void ItemDataBound(DataListItemEventArgs e)
		{
			OnItemDataBound.InvokeAsync(e);
		}
	}
}

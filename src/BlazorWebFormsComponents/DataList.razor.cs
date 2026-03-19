using BlazorComponentUtilities;
using BlazorWebFormsComponents.DataBinding;
using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents
{
	public partial class DataList<ItemType> : DataBoundComponent<ItemType>, IDataListStyleContainer
	{
		public DataList()
		{
			BubbledEvent += DataList_BubbledEvent;
		}

		private void DataList_BubbledEvent(object sender, System.EventArgs e)
		{
			if (e is CommandEventArgs cmd)
			{
				var args = new DataListCommandEventArgs(cmd.CommandName, cmd.CommandArgument, null) { Sender = cmd.Sender ?? this };

				// Fire ItemCommand for all commands
				var itemCommandHandler = OnItemCommand.HasDelegate ? OnItemCommand : ItemCommand;
				if (itemCommandHandler.HasDelegate) { _ = itemCommandHandler.InvokeAsync(args); }

				// Dispatch to specific command events based on CommandName
				switch (cmd.CommandName?.ToLowerInvariant())
				{
					case "select":
						var selHandler = OnSelectedIndexChanged.HasDelegate ? OnSelectedIndexChanged : SelectedIndexChanged;
						if (selHandler.HasDelegate) { _ = selHandler.InvokeAsync(EventArgs.Empty); }
						break;
					case "edit":
						var editHandler = OnEditCommand.HasDelegate ? OnEditCommand : EditCommand;
						if (editHandler.HasDelegate) { _ = editHandler.InvokeAsync(args); }
						break;
					case "update":
						var updateHandler = OnUpdateCommand.HasDelegate ? OnUpdateCommand : UpdateCommand;
						if (updateHandler.HasDelegate) { _ = updateHandler.InvokeAsync(args); }
						break;
					case "delete":
						var deleteHandler = OnDeleteCommand.HasDelegate ? OnDeleteCommand : DeleteCommand;
						if (deleteHandler.HasDelegate) { _ = deleteHandler.InvokeAsync(args); }
						break;
					case "cancel":
						var cancelHandler = OnCancelCommand.HasDelegate ? OnCancelCommand : CancelCommand;
						if (cancelHandler.HasDelegate) { _ = cancelHandler.InvokeAsync(args); }
						break;
				}
			}
		}

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
		[Parameter] public bool UseAccessibleHeader { get; set; } = false;
		[Parameter]
		public EventCallback<DataListItemEventArgs> OnItemDataBound { get; set; }

		/// <summary>
		/// Web Forms migration alias (bare name) for OnItemDataBound.
		/// </summary>
		[Parameter]
		public EventCallback<DataListItemEventArgs> ItemDataBound { get; set; }

		/// <summary>
		/// Occurs when a command button within the DataList is clicked.
		/// </summary>
		[Parameter]
		public EventCallback<DataListCommandEventArgs> ItemCommand { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemCommand.
		/// </summary>
		[Parameter]
		public EventCallback<DataListCommandEventArgs> OnItemCommand { get; set; }

		/// <summary>
		/// Occurs when the selected index changes.
		/// </summary>
		[Parameter]
		public EventCallback<EventArgs> SelectedIndexChanged { get; set; }

		/// <summary>
		/// Web Forms migration alias for SelectedIndexChanged.
		/// </summary>
		[Parameter]
		public EventCallback<EventArgs> OnSelectedIndexChanged { get; set; }

		/// <summary>
		/// Occurs when an Edit command is raised.
		/// </summary>
		[Parameter]
		public EventCallback<DataListCommandEventArgs> EditCommand { get; set; }

		/// <summary>
		/// Web Forms migration alias for EditCommand.
		/// </summary>
		[Parameter]
		public EventCallback<DataListCommandEventArgs> OnEditCommand { get; set; }

		/// <summary>
		/// Occurs when an Update command is raised.
		/// </summary>
		[Parameter]
		public EventCallback<DataListCommandEventArgs> UpdateCommand { get; set; }

		/// <summary>
		/// Web Forms migration alias for UpdateCommand.
		/// </summary>
		[Parameter]
		public EventCallback<DataListCommandEventArgs> OnUpdateCommand { get; set; }

		/// <summary>
		/// Occurs when a Delete command is raised.
		/// </summary>
		[Parameter]
		public EventCallback<DataListCommandEventArgs> DeleteCommand { get; set; }

		/// <summary>
		/// Web Forms migration alias for DeleteCommand.
		/// </summary>
		[Parameter]
		public EventCallback<DataListCommandEventArgs> OnDeleteCommand { get; set; }

		/// <summary>
		/// Occurs when a Cancel command is raised.
		/// </summary>
		[Parameter]
		public EventCallback<DataListCommandEventArgs> CancelCommand { get; set; }

		/// <summary>
		/// Web Forms migration alias for CancelCommand.
		/// </summary>
		[Parameter]
		public EventCallback<DataListCommandEventArgs> OnCancelCommand { get; set; }

		/// <summary>
		/// Occurs when an item is created in the DataList control.
		/// </summary>
		[Parameter]
		public EventCallback<DataListItemEventArgs> ItemCreated { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemCreated.
		/// </summary>
		[Parameter]
		public EventCallback<DataListItemEventArgs> OnItemCreated { get; set; }

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

		protected override void OnParametersSet()
		{
			base.OnParametersSet();
			FireItemCreatedEvents();
		}

		private void FireItemCreatedEvents()
		{
			if (Items == null || !Items.Any()) return;

			var handler = OnItemCreated.HasDelegate ? OnItemCreated : ItemCreated;
			if (!handler.HasDelegate) return;

			foreach (var item in Items)
			{
				_ = handler.InvokeAsync(new DataListItemEventArgs(item) { Sender = this });
			}
		}

		protected string GetItemTypeAttribute()
		{
			if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("itemtype", out var value))
				return value?.ToString();
			return null;
		}

		protected string GetTableStyle()
		{
			var borderCollapse = GridLines != DataListEnum.None ? "border-collapse:collapse;" : null;
			var combined = borderCollapse + CalculatedStyle;
			return string.IsNullOrEmpty(combined) ? null : combined;
		}

		protected virtual void ItemDataBoundInternal(DataListItemEventArgs e)
		{
			var handler = OnItemDataBound.HasDelegate ? OnItemDataBound : ItemDataBound;
			handler.InvokeAsync(e);
		}
	}
}

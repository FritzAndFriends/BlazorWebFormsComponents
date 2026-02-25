using BlazorWebFormsComponents.DataBinding;
using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Blazor version of WebForms DataGrid control
	/// </summary>
	/// <typeparam name="ItemType"></typeparam>
	public partial class DataGrid<ItemType> : DataBoundComponent<ItemType>, IRowCollection<ItemType>, IColumnCollection<ItemType>, IDataGridStyleContainer
	{

		/// <summary>
		///	Specify if the DataGrid component will autogenerate its columns
		/// </summary>
		[Parameter] public bool AutoGenerateColumns { get; set; } = true;

		/// <summary>
		/// Text to show when there are no items to show
		/// </summary>
		[Parameter] public string EmptyDataText { get; set; }

		/// <summary>
		/// Not supported yet
		/// </summary>
		[Parameter] public string DataKeyField { get; set; }

		/// <summary>
		/// Show or hide the header row
		/// </summary>
		[Parameter] public bool ShowHeader { get; set; } = true;

		/// <summary>
		/// Show or hide the footer row
		/// </summary>
		[Parameter] public bool ShowFooter { get; set; } = false;

		/// <summary>
		/// Enable or disable paging
		/// </summary>
		[Parameter] public bool AllowPaging { get; set; } = false;

		/// <summary>
		/// Enable or disable sorting
		/// </summary>
		[Parameter] public bool AllowSorting { get; set; } = false;

		/// <summary>
		/// The number of items to display per page
		/// </summary>
		[Parameter] public int PageSize { get; set; } = 10;

		/// <summary>
		/// The current page index
		/// </summary>
		[Parameter] public int CurrentPageIndex { get; set; } = 0;

		/// <summary>
		/// Gets or sets the index of the selected item. -1 means no item is selected.
		/// </summary>
		[Parameter] public int SelectedIndex { get; set; } = -1;

		/// <summary>
		/// Gets or sets the index of the item being edited. -1 means no item is being edited.
		/// </summary>
		[Parameter] public int EditItemIndex { get; set; } = -1;

		#region Display Properties

		/// <summary>
		/// Gets or sets the text to render in a caption element at the top of the table.
		/// </summary>
		[Parameter] public string Caption { get; set; }

		/// <summary>
		/// Gets or sets the horizontal or vertical position of the caption element.
		/// </summary>
		[Parameter] public TableCaptionAlign CaptionAlign { get; set; } = TableCaptionAlign.NotSet;

		/// <summary>
		/// Gets or sets the cell padding for the table. -1 means the attribute is not rendered.
		/// </summary>
		[Parameter] public int CellPadding { get; set; } = -1;

		/// <summary>
		/// Gets or sets the cell spacing for the table. -1 means the attribute is not rendered.
		/// </summary>
		[Parameter] public int CellSpacing { get; set; } = -1;

		/// <summary>
		/// Gets or sets the grid line style for the table.
		/// </summary>
		[Parameter] public GridLines GridLines { get; set; } = GridLines.None;

		/// <summary>
		/// Gets or sets whether header cells render with th scope="col" for accessibility.
		/// </summary>
		[Parameter] public bool UseAccessibleHeader { get; set; }

		#endregion

		#region TableItemStyle Properties (IDataGridStyleContainer)

		/// <summary>
		/// Gets or sets the style applied to alternating items.
		/// </summary>
		public TableItemStyle AlternatingItemStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to items.
		/// </summary>
		public TableItemStyle ItemStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to the header.
		/// </summary>
		public TableItemStyle HeaderStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to the footer.
		/// </summary>
		public TableItemStyle FooterStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to the pager.
		/// </summary>
		public TableItemStyle PagerStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to the selected item.
		/// </summary>
		public TableItemStyle SelectedItemStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to the item being edited.
		/// </summary>
		public TableItemStyle EditItemStyle { get; internal set; } = new TableItemStyle();

		#endregion

		#region Style RenderFragment Parameters

		/// <summary>
		/// Content for the AlternatingItemStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment AlternatingItemStyleContent { get; set; }

		/// <summary>
		/// Content for the ItemStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment ItemStyleContent { get; set; }

		/// <summary>
		/// Content for the HeaderStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment HeaderStyleContent { get; set; }

		/// <summary>
		/// Content for the FooterStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment FooterStyleContent { get; set; }

		/// <summary>
		/// Content for the PagerStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment PagerStyleContent { get; set; }

		/// <summary>
		/// Content for the SelectedItemStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment SelectedItemStyleContent { get; set; }

		/// <summary>
		/// Content for the EditItemStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment EditItemStyleContent { get; set; }

		#endregion

		#region Events (WI-45)

		/// <summary>
		/// Fires when the page index changes.
		/// </summary>
		[Parameter] public EventCallback<DataGridPageChangedEventArgs> PageIndexChanged { get; set; }

		/// <summary>
		/// Fires when a sort header is clicked.
		/// </summary>
		[Parameter] public EventCallback<DataGridSortCommandEventArgs> SortCommand { get; set; }

		/// <summary>
		/// Fires when an item row is created.
		/// </summary>
		[Parameter] public EventCallback<DataGridItemEventArgs> ItemCreated { get; set; }

		/// <summary>
		/// Fires after an item is data-bound.
		/// </summary>
		[Parameter] public EventCallback<DataGridItemEventArgs> ItemDataBound { get; set; }

		/// <summary>
		/// Fires when the selected index changes.
		/// </summary>
		[Parameter] public EventCallback SelectedIndexChanged { get; set; }

		#endregion

		///<inheritdoc/>
		public List<IColumn<ItemType>> ColumnList { get; set; } = new List<IColumn<ItemType>>();

		/// <summary>
		/// The Rows of the DataGrid
		/// </summary>
		public List<IRow<ItemType>> Rows { get => RowList; set => RowList = value; }

		///<inheritdoc/>
		public List<IRow<ItemType>> RowList { get; set; } = new List<IRow<ItemType>>();

		#region Templates
		/// <summary>
		/// The columns template of the DataGrid
		/// </summary>
		[Parameter] public RenderFragment Columns { get; set; }

		/// <summary>
		/// The ChildContent of the DataGrid
		/// </summary>
		[Parameter] public RenderFragment ChildContent { get; set; }
		#endregion

		/// <summary>
		/// Gets the total number of pages based on item count and page size.
		/// </summary>
		public int PageCount => Items != null && AllowPaging && PageSize > 0
			? (int)Math.Ceiling((double)Items.Count() / PageSize)
			: 1;

		/// <summary>
		/// Gets the items for the current page, or all items if paging is disabled.
		/// </summary>
		protected IEnumerable<ItemType> PagedItems
		{
			get
			{
				if (Items == null) return Enumerable.Empty<ItemType>();
				if (!AllowPaging) return Items;
				return Items.Skip(CurrentPageIndex * PageSize).Take(PageSize);
			}
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			if (AutoGenerateColumns)
			{
				DataGridColumnGenerator.GenerateColumns(this);
			}
		}

		[Parameter]
		public EventCallback<DataGridCommandEventArgs> OnItemCommand { get; set; }

		[Parameter]
		public EventCallback<DataGridCommandEventArgs> OnEditCommand { get; set; }

		[Parameter]
		public EventCallback<DataGridCommandEventArgs> OnCancelCommand { get; set; }

		[Parameter]
		public EventCallback<DataGridCommandEventArgs> OnUpdateCommand { get; set; }

		[Parameter]
		public EventCallback<DataGridCommandEventArgs> OnDeleteCommand { get; set; }

		/// <summary>
		/// Navigates to the specified page index and fires PageIndexChanged.
		/// </summary>
		protected async Task GoToPage(int newPageIndex)
		{
			if (Items == null) return;
			CurrentPageIndex = newPageIndex;
			var args = new DataGridPageChangedEventArgs(newPageIndex);
			await PageIndexChanged.InvokeAsync(args);
			StateHasChanged();
		}

		/// <summary>
		/// Fires the SortCommand event for the specified sort expression.
		/// </summary>
		internal async Task Sort(string sortExpression)
		{
			var args = new DataGridSortCommandEventArgs(sortExpression, this);
			await SortCommand.InvokeAsync(args);
			StateHasChanged();
		}

		/// <summary>
		/// Gets the HTML rules attribute value corresponding to the GridLines setting.
		/// </summary>
		internal string GetGridLinesRules()
		{
			return GridLines switch
			{
				GridLines.Horizontal => "rows",
				GridLines.Vertical => "cols",
				GridLines.Both => "all",
				_ => null
			};
		}

		/// <summary>
		/// Gets the CSS style for the caption element based on CaptionAlign.
		/// </summary>
		internal string GetCaptionStyle()
		{
			return CaptionAlign switch
			{
				TableCaptionAlign.Top => "caption-side:top",
				TableCaptionAlign.Bottom => "caption-side:bottom",
				TableCaptionAlign.Left => "text-align:left",
				TableCaptionAlign.Right => "text-align:right",
				_ => null
			};
		}

		/// <summary>
		/// Gets the effective style for a data row. Priority: Edit > Selected > Alternating > Item.
		/// </summary>
		internal TableItemStyle GetRowStyle(int rowIndex)
		{
			if (rowIndex == EditItemIndex && EditItemStyle != null)
				return EditItemStyle;
			if (rowIndex == SelectedIndex && SelectedItemStyle != null)
				return SelectedItemStyle;
			if (rowIndex % 2 == 1 && AlternatingItemStyle != null)
				return AlternatingItemStyle;
			return ItemStyle;
		}

		///<inheritdoc/>
		public void AddColumn(IColumn<ItemType> column)
		{
			ColumnList.Add(column);
			StateHasChanged();
		}

		///<inheritdoc/>
		public void RemoveColumn(IColumn<ItemType> column)
		{
			ColumnList.Remove(column);
			StateHasChanged();
		}

		///<inheritdoc/>
		public void RemoveRow(IRow<ItemType> row)
		{
			Rows.Remove(row);
			StateHasChanged();
		}

		///<inheritdoc/>
		public void AddRow(IRow<ItemType> row)
		{
			Rows.Add(row);
			StateHasChanged();
		}

	}
}

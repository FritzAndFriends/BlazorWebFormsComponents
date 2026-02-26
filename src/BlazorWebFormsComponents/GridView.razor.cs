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
	/// Blazor version of WebForms GridView control
	/// </summary>
	/// <typeparam name="ItemType"></typeparam>
	public partial class GridView<ItemType> : DataBoundComponent<ItemType>, IRowCollection<ItemType>, IColumnCollection<ItemType>, IGridViewStyleContainer, IPagerSettingsContainer
	{

		/// <summary>
		///	Specify if the GridView component will autogenerate its columns
		/// </summary>
		[Parameter] public bool AutoGenerateColumns { get; set; } = true;

		/// <summary>
		/// Text to show when there are no items to show
		/// </summary>
		[Parameter] public string EmptyDataText { get; set; }

		/// <summary>
		/// Not supported yet
		/// </summary>
		[Parameter] public string DataKeyNames { get; set; }

		/// <summary>
		/// Gets or sets the index of the row to edit. -1 means no row is being edited.
		/// </summary>
		[Parameter] public int EditIndex { get; set; } = -1;

		/// <summary>
		/// Gets or sets the index of the currently selected row. -1 means no row is selected.
		/// </summary>
		[Parameter] public int SelectedIndex { get; set; } = -1;

		/// <summary>
		/// Gets the data item of the currently selected row.
		/// </summary>
		public ItemType SelectedRow
		{
			get
			{
				if (SelectedIndex < 0 || Items == null) return default;
				var items = PagedItems.ToList();
				return SelectedIndex < items.Count ? items[SelectedIndex] : default;
			}
		}

		/// <summary>
		/// Gets the value of the DataKeyNames field for the currently selected row.
		/// </summary>
		public object SelectedValue
		{
			get
			{
				var row = SelectedRow;
				if (row == null || string.IsNullOrEmpty(DataKeyNames)) return null;
				var keyField = DataKeyNames.Split(',')[0].Trim();
				var prop = typeof(ItemType).GetProperty(keyField);
				return prop?.GetValue(row);
			}
		}

		/// <summary>
		/// Gets or sets whether a Select command link is automatically rendered in each row.
		/// </summary>
		[Parameter] public bool AutoGenerateSelectButton { get; set; }

		/// <summary>
		/// Occurs before the selected index changes. Can be cancelled.
		/// </summary>
		[Parameter] public EventCallback<GridViewSelectEventArgs> SelectedIndexChanging { get; set; }

		/// <summary>
		/// Occurs after the selected index has changed.
		/// </summary>
		[Parameter] public EventCallback<int> SelectedIndexChanged { get; set; }

		#region TableItemStyle Properties (IGridViewStyleContainer)

		/// <summary>
		/// Gets or sets the style applied to data rows.
		/// </summary>
		public TableItemStyle RowStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to alternating data rows.
		/// </summary>
		public TableItemStyle AlternatingRowStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to the header row.
		/// </summary>
		public TableItemStyle HeaderStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to the footer row.
		/// </summary>
		public TableItemStyle FooterStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to the empty data row.
		/// </summary>
		public TableItemStyle EmptyDataRowStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to the pager row.
		/// </summary>
		public TableItemStyle PagerStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to the row being edited.
		/// </summary>
		public TableItemStyle EditRowStyle { get; internal set; } = new TableItemStyle();

		/// <summary>
		/// Gets or sets the style applied to the selected row.
		/// </summary>
		public TableItemStyle SelectedRowStyle { get; internal set; } = new TableItemStyle();

		#endregion

		#region Style RenderFragment Parameters

		/// <summary>
		/// Content for the RowStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment RowStyleContent { get; set; }

		/// <summary>
		/// Content for the AlternatingRowStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment AlternatingRowStyleContent { get; set; }

		/// <summary>
		/// Content for the HeaderStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment HeaderStyleContent { get; set; }

		/// <summary>
		/// Content for the FooterStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment FooterStyleContent { get; set; }

		/// <summary>
		/// Content for the EmptyDataRowStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment EmptyDataRowStyleContent { get; set; }

		/// <summary>
		/// Content for the PagerStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment PagerStyleContent { get; set; }

		/// <summary>
		/// Content for the EditRowStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment EditRowStyleContent { get; set; }

		/// <summary>
		/// Content for the SelectedRowStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment SelectedRowStyleContent { get; set; }

		#endregion

		#region PagerSettings

		/// <summary>
		/// Gets the pager settings for the GridView.
		/// </summary>
		public PagerSettings PagerSettings { get; internal set; } = new PagerSettings();

		/// <summary>
		/// Content for the PagerSettings sub-component.
		/// </summary>
		[Parameter] public RenderFragment PagerSettingsContent { get; set; }

		#endregion

		#region Display Properties

		/// <summary>
		/// Gets or sets whether the header row is displayed. Default is true.
		/// </summary>
		[Parameter] public bool ShowHeader { get; set; } = true;

		/// <summary>
		/// Gets or sets whether a footer row is displayed. Default is false.
		/// </summary>
		[Parameter] public bool ShowFooter { get; set; }

		/// <summary>
		/// Gets or sets whether the header row is rendered when the data source is empty. Default is false.
		/// </summary>
		[Parameter] public bool ShowHeaderWhenEmpty { get; set; }

		/// <summary>
		/// Gets or sets the text to render in a caption element at the top of the table.
		/// </summary>
		[Parameter] public string Caption { get; set; }

		/// <summary>
		/// Gets or sets the horizontal or vertical position of the caption element in a table.
		/// </summary>
		[Parameter] public TableCaptionAlign CaptionAlign { get; set; } = TableCaptionAlign.NotSet;

		/// <summary>
		/// Gets or sets the template to display when the data source is empty.
		/// When set, this takes precedence over EmptyDataText.
		/// </summary>
		[Parameter] public RenderFragment EmptyDataTemplate { get; set; }

		/// <summary>
		/// Gets or sets the grid line style for the table (renders the rules attribute).
		/// </summary>
		[Parameter] public GridLines GridLines { get; set; } = GridLines.Both;

		/// <summary>
		/// Gets or sets whether header cells render with th scope="col" for accessibility.
		/// </summary>
		[Parameter] public bool UseAccessibleHeader { get; set; }

		/// <summary>
		/// Gets or sets the cell padding for the table. -1 means the attribute is not rendered.
		/// </summary>
		[Parameter] public int CellPadding { get; set; } = -1;

		/// <summary>
		/// Gets or sets the cell spacing for the table. -1 means the attribute is not rendered.
		/// </summary>
		[Parameter] public int CellSpacing { get; set; } = -1;

		#endregion

		/// <summary>
		/// Enables or disables sorting for the GridView
		/// </summary>
		[Parameter] public bool AllowSorting { get; set; }

		/// <summary>
		/// The current sort direction
		/// </summary>
		[Parameter] public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

		/// <summary>
		/// The current sort expression (column name)
		/// </summary>
		[Parameter] public string SortExpression { get; set; }

		/// <summary>
		/// Fires before sort is applied. Can be cancelled.
		/// </summary>
		[Parameter] public EventCallback<GridViewSortEventArgs> Sorting { get; set; }

		/// <summary>
		/// Fires after sort is applied
		/// </summary>
		[Parameter] public EventCallback<GridViewSortEventArgs> Sorted { get; set; }

		/// <summary>
		/// Gets or sets whether paging is enabled.
		/// </summary>
		[Parameter] public bool AllowPaging { get; set; }

		/// <summary>
		/// Gets or sets the number of items to display per page.
		/// </summary>
		[Parameter] public int PageSize { get; set; } = 10;

		/// <summary>
		/// Gets or sets the current page index (zero-based).
		/// </summary>
		[Parameter] public int PageIndex { get; set; }

		/// <summary>
		/// Occurs after the page index has changed.
		/// </summary>
		[Parameter] public EventCallback<PageChangedEventArgs> PageIndexChanged { get; set; }

		///<inheritdoc/>
		public List<IColumn<ItemType>> ColumnList { get; set; } = new List<IColumn<ItemType>>();

		/// <summary>
		/// The Rows of the GridView
		/// </summary>
		public List<IRow<ItemType>> Rows { get => RowList; set => RowList = value; }

		///<inheritdoc/>
		public List<IRow<ItemType>> RowList { get; set; } = new List<IRow<ItemType>>();

		#region Templates
		/// <summary>
		/// The columns template of the GridView
		/// </summary>
		[Parameter] public RenderFragment Columns { get; set; }

		/// <summary>
		/// The ChildContent of the GridView
		/// </summary>
		[Parameter] public RenderFragment ChildContent { get; set; }
		#endregion
		protected override void OnInitialized()
		{
			base.OnInitialized();
			if (AutoGenerateColumns)
			{
				GridViewColumnGenerator.GenerateColumns(this);
			}
		}

		[Parameter]
		public EventCallback<GridViewCommandEventArgs> OnRowCommand { get; set; }

		/// <summary>
		/// Occurs when a row's Edit button is clicked, but before the row enters edit mode.
		/// </summary>
		[Parameter] public EventCallback<GridViewEditEventArgs> RowEditing { get; set; }

		/// <summary>
		/// Occurs when a row's Update button is clicked, but before the row is updated.
		/// </summary>
		[Parameter] public EventCallback<GridViewUpdateEventArgs> RowUpdating { get; set; }

		/// <summary>
		/// Occurs when a row's Delete button is clicked, but before the row is deleted.
		/// </summary>
		[Parameter] public EventCallback<GridViewDeleteEventArgs> RowDeleting { get; set; }

		/// <summary>
		/// Occurs when a row's Cancel button is clicked, but before the row exits edit mode.
		/// </summary>
		[Parameter] public EventCallback<GridViewCancelEditEventArgs> RowCancelingEdit { get; set; }

		/// <summary>
		/// Gets the total number of pages based on item count and page size.
		/// </summary>
		public int TotalPages => Items != null && AllowPaging && PageSize > 0
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
				return Items.Skip(PageIndex * PageSize).Take(PageSize);
			}
		}

		/// <summary>
		/// Navigates to the specified page index.
		/// </summary>
		protected async Task GoToPage(int newPageIndex)
		{
			if (Items == null) return;

			var oldPageIndex = PageIndex;
			var totalPages = TotalPages;
			var startRowIndex = newPageIndex * PageSize;

			var args = new PageChangedEventArgs(newPageIndex, oldPageIndex, totalPages, startRowIndex);

			PageIndex = args.NewPageIndex;
			await PageIndexChanged.InvokeAsync(args);
			StateHasChanged();
		}

		/// <summary>
		/// Initiates a sort operation for the specified sort expression
		/// </summary>
		internal async Task Sort(string sortExpression)
		{
			var newDirection = (sortExpression == SortExpression && SortDirection == SortDirection.Ascending)
				? SortDirection.Descending
				: SortDirection.Ascending;

			var args = new GridViewSortEventArgs(sortExpression, newDirection);
			await Sorting.InvokeAsync(args);
			if (args.Cancel) return;

			SortExpression = args.SortExpression;
			SortDirection = args.SortDirection;
			await Sorted.InvokeAsync(args);
			StateHasChanged();
		}

		/// <summary>
		/// Puts the specified row into edit mode.
		/// </summary>
		internal async Task EditRow(int rowIndex)
		{
			var args = new GridViewEditEventArgs(rowIndex);
			await RowEditing.InvokeAsync(args);
			if (args.Cancel) return;
			EditIndex = args.NewEditIndex;
			StateHasChanged();
		}

		/// <summary>
		/// Fires the RowUpdating event for the specified row and exits edit mode.
		/// </summary>
		internal async Task UpdateRow(int rowIndex)
		{
			var args = new GridViewUpdateEventArgs(rowIndex);
			await RowUpdating.InvokeAsync(args);
			if (args.Cancel) return;
			EditIndex = -1;
			StateHasChanged();
		}

		/// <summary>
		/// Fires the RowDeleting event for the specified row.
		/// </summary>
		internal async Task DeleteRow(int rowIndex)
		{
			var args = new GridViewDeleteEventArgs(rowIndex);
			await RowDeleting.InvokeAsync(args);
		}

		/// <summary>
		/// Cancels edit mode for the specified row.
		/// </summary>
		internal async Task CancelEdit(int rowIndex)
		{
			var args = new GridViewCancelEditEventArgs(rowIndex);
			await RowCancelingEdit.InvokeAsync(args);
			if (args.Cancel) return;
			EditIndex = -1;
			StateHasChanged();
		}

		/// <summary>
		/// Selects the specified row.
		/// </summary>
		internal async Task SelectRow(int rowIndex)
		{
			var args = new GridViewSelectEventArgs(rowIndex);
			await SelectedIndexChanging.InvokeAsync(args);
			if (args.Cancel) return;
			SelectedIndex = args.NewSelectedIndex;
			await SelectedIndexChanged.InvokeAsync(SelectedIndex);
			StateHasChanged();
		}

		/// <summary>
		/// Gets whether the auto-generated command column should be displayed.
		/// </summary>
		internal bool ShowCommandColumn => AutoGenerateSelectButton || RowEditing.HasDelegate || RowUpdating.HasDelegate || RowDeleting.HasDelegate || RowCancelingEdit.HasDelegate;

		/// <summary>
		/// Gets the total column count including the auto-generated command column.
		/// </summary>
		internal int TotalColumnCount => ColumnList.Count + (ShowCommandColumn ? 1 : 0);

		/// <summary>
		/// Gets whether the data source has items.
		/// </summary>
		internal bool HasData => Items != null && Items.Any();

		/// <summary>
		/// Gets whether the header row should be rendered based on ShowHeader, data presence, and ShowHeaderWhenEmpty.
		/// </summary>
		internal bool ShouldRenderHeader => ShowHeader && (HasData || ShowHeaderWhenEmpty);

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
		/// Gets the border attribute value. WebForms renders border="1" when GridLines is not None.
		/// </summary>
		internal string GetGridLinesBorder()
		{
			return GridLines != GridLines.None ? "1" : null;
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
		/// Gets the effective style for a data row, considering RowStyle, AlternatingRowStyle, EditRowStyle, and SelectedRowStyle.
		/// </summary>
		internal TableItemStyle GetRowStyle(int rowIndex)
		{
			if (rowIndex == EditIndex && EditRowStyle != null)
				return EditRowStyle;
			if (rowIndex == SelectedIndex && SelectedRowStyle != null)
				return SelectedRowStyle;
			if (rowIndex % 2 == 1 && AlternatingRowStyle != null)
				return AlternatingRowStyle;
			return RowStyle;
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

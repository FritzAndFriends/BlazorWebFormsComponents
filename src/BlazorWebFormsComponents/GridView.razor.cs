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
	public partial class GridView<ItemType> : DataBoundComponent<ItemType>, IRowCollection<ItemType>, IColumnCollection<ItemType>
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
		/// Gets or sets the style applied to the row being edited.
		/// </summary>
		[Parameter] public TableItemStyle EditRowStyle { get; set; }

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
		/// Gets whether the auto-generated command column should be displayed.
		/// </summary>
		internal bool ShowCommandColumn => RowEditing.HasDelegate || RowUpdating.HasDelegate || RowDeleting.HasDelegate || RowCancelingEdit.HasDelegate;

		/// <summary>
		/// Gets the total column count including the auto-generated command column.
		/// </summary>
		internal int TotalColumnCount => ColumnList.Count + (ShowCommandColumn ? 1 : 0);

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

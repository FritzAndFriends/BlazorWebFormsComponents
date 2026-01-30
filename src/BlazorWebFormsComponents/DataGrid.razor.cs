using BlazorWebFormsComponents.DataBinding;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Blazor version of WebForms DataGrid control
	/// </summary>
	/// <typeparam name="ItemType"></typeparam>
	public partial class DataGrid<ItemType> : DataBoundComponent<ItemType>, IRowCollection<ItemType>, IColumnCollection<ItemType>
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
		/// The css class of the DataGrid
		/// </summary>
		[Parameter] public string CssClass { get; set; }

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

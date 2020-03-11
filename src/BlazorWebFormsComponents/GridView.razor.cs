using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Blazor version of WebForms GridView control
	/// </summary>
	/// <typeparam name="ItemType"></typeparam>
  public partial class GridView<ItemType> : BaseModelBindingComponent<ItemType>, IRowCollection, IColumnCollection
  {

		/// <summary>
		///	Specify if the GridView component will autogenerate its columns
		/// </summary>
		[Parameter] public bool AutogenerateColumns { get; set; } = true;

		/// <summary>
		/// Text to show when there are no items to show
		/// </summary>
		[Parameter] public string EmptyDataText { get; set; }

		/// <summary>
		/// Not supported yet
		/// </summary>
		[Parameter] public string DataKeyNames { get; set; }

		/// <summary>
		/// The css class of the GridView
		/// </summary>
		[Parameter] public string CssClass { get; set; }

		///<inheritdoc/>
		public List<IColumn> ColumnList { get; set; } = new List<IColumn>();

		/// <summary>
		/// The Rows of the GridView
		/// </summary>
		public List<IRow> Rows { get => RowList; set => RowList = value; }

		///<inheritdoc/>
		public List<IRow> RowList { get; set; } = new List<IRow>();

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
			if (AutogenerateColumns)
			{
				GridViewColumnGenerator.GenerateColumns(this);
			}
		}

		///<inheritdoc/>
		public void AddColumn(IColumn column)
		{
			ColumnList.Add(column);
			StateHasChanged();
		}

		///<inheritdoc/>
		public void RemoveColumn(IColumn column)
		{
			ColumnList.Remove(column);
			StateHasChanged();
		}

		///<inheritdoc/>
		public void RemoveRow(IRow row)
		{
			Rows.Remove(row);
			StateHasChanged();
		}

		///<inheritdoc/>
		public void AddRow(IRow row)
		{
			Rows.Add(row);
			StateHasChanged();
		}

	}
}

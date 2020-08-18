using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
  /// <summary>
  /// Blazor version of WebForms GridView control
  /// </summary>
  /// <typeparam name="ItemType"></typeparam>
  public partial class GridView<ItemType> : BaseModelBindingComponent<ItemType>, IRowCollection<ItemType>, IColumnCollection<ItemType>
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
	  if (AutogenerateColumns)
	  {
		GridViewColumnGenerator.GenerateColumns(this);
	  }
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

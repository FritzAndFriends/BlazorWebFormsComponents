using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Components;
using BlazorWebFormsComponents.Extensions;

namespace BlazorWebFormsComponents
{

  public abstract class BaseModelBindingComponent<ItemType> : BaseDataBindingComponent
  {

	// Cheer 300 svavablount 15/12/19 
	// Cheer 200 nothing_else_matters 15/12/19 

	public delegate IQueryable<ItemType> SelectHandler(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount);

	/// <summary>
	/// Data retrieval method to databind the collection to
	/// </summary>
	[Parameter]
	public SelectHandler SelectMethod { get; set; }

	[Parameter]
	public IEnumerable<ItemType> Items
	{
	  get { return ItemsList; }
	  set { ItemsList = value?.ToList(); }
	}

	protected List<ItemType> ItemsList { get; set; }

	[Parameter]
	public object DataSource
	{
	  get { return Items; }
	  set
	  {
		if (!typeof(IEnumerable).IsAssignableFrom(value.GetType()) && value.GetType() != typeof(DataSet) && value.GetType() != typeof(DataTable))
		{
		  throw new InvalidOperationException("The DataSource must have an object of type IEnumerable, DataSet or DataTable.");
		}

		TryBindToDataSet(ref value);
		TryBindToDataTable(ref value);

		Items = value as IEnumerable<ItemType>;
		this.StateHasChanged();
	  }
	}

	protected override void OnAfterRender(bool firstRender)
	{

	  if (firstRender && SelectMethod != null)
	  {

		// Model Binding
		int totalRowCount;
		Items = SelectMethod(int.MaxValue, 0, "", out totalRowCount);

	  }

	  base.OnAfterRender(firstRender);

	}

	private static void TryBindToDataSet(ref object value)
	{
	  if (value.GetType() == typeof(DataSet))
	  {
		var dataSet = (value as DataSet);
		if (dataSet.Tables.Count > 0)
		{
		  value = (dataSet.Tables[0] as object as DataTable).AsDynamicEnumerable();
		}
		else
		{
		  value = Enumerable.Empty<ItemType>();
		}
	  }
	}

	private static void TryBindToDataTable(ref object value)
	{
	  if (value.GetType() == typeof(DataTable))
	  {
		value = (value as DataTable).AsDynamicEnumerable();
	  }
	}

  }

}

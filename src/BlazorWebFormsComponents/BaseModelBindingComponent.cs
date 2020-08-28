using Microsoft.AspNetCore.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
				Items = GetDataSource(value);
				StateHasChanged();
			}
		}

		private IEnumerable<ItemType> GetDataSource(object dataSource)
		{
			if (dataSource is IEnumerable<ItemType> enumerableOfItemType)
			{
				// If data source is already the right type, return it
				return enumerableOfItemType;
			}

			if (dataSource is IListSource listSource)
			{
				// Check for DataTable and DataSet
				return GetListSourceData(listSource);
			}

			throw new InvalidOperationException($"The DataSource must implement IEnumerable<{typeof(ItemType).FullName}> (such as most list types) or IListSource (such as DataSet or DataTable).");
		}

		private IEnumerable<ItemType> GetListSourceData(IListSource listSource)
		{
			if (typeof(ItemType) != typeof(object))
			{
				throw new InvalidOperationException("Binding to an IListSource (such as DataSet or DataTable) requires that 'ItemType' be set to 'object'.");
			}

			var list = listSource.GetList();
			if (list == null)
			{
				throw new InvalidOperationException("The list returned by the IListSource (such as DataSet or DataTable) cannot be null.");
			}

			if (!listSource.ContainsListCollection)
			{
				// If this is the actual list, get it, and return it
				return list.OfType<object>() as IEnumerable<ItemType>;
			}
			else
			{
				// If not, then this is a list of lists, so find the list within

				if (list is ITypedList typedList)
				{
					var propDescs = typedList.GetItemProperties(new PropertyDescriptor[0]);
					if (propDescs == null || propDescs.Count == 0)
					{
						throw new InvalidOperationException("The list returned by the IListSource (such as DataSet or DataTable) has no members. For example, the DataSet might have no DataTables in it.");
					}

					var listProperty = propDescs[0];
					if (listProperty != null)
					{
						var listRow = list[0];
						var innerList = listProperty.GetValue(listRow);

						if ((innerList != null) && (innerList is IEnumerable innerListEnumerable))
						{
							return innerListEnumerable.OfType<object>() as IEnumerable<ItemType>;
						}
					}

					throw new InvalidOperationException("The list returned by the IListSource (such as DataSet or DataTable) has no members. For example, the DataSet might have no DataTables in it.");
				}

				return null;
			}
		}

		protected override void OnAfterRender(bool firstRender)
		{
			if (firstRender && SelectMethod != null)
			{
				// Model Binding
				Items = SelectMethod(int.MaxValue, 0, "", out var totalRowCount);
			}

			base.OnAfterRender(firstRender);
		}
	}
}

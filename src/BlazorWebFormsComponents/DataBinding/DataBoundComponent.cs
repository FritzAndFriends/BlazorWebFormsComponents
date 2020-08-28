using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System;

namespace BlazorWebFormsComponents.DataBinding
{
	public class DataBoundComponent<TItemType> : BaseDataBoundComponent
	{
		[Parameter]
		public string DataMember { get; set; }

		[Parameter]
		public SelectHandler<TItemType> SelectMethod { get; set; }

		[Parameter]
		public IEnumerable<TItemType> Items
		{
			get { return ItemsList; }
			set { ItemsList = value?.ToList(); }
		}

		protected List<TItemType> ItemsList { get; set; }

		[Parameter]
		public override object DataSource
		{
			get { return Items; }
			set
			{
				Items = GetDataSource(value);
				StateHasChanged();
			}
		}

		private IEnumerable<TItemType> GetDataSource(object dataSource)
		{
			if (dataSource is IEnumerable<TItemType> enumerableOfItemType)
			{
				// If data source is already the right type, return it
				return enumerableOfItemType;
			}

			if (dataSource is IListSource listSource)
			{
				// Check for DataTable and DataSet
				return GetListSourceData(listSource);
			}

			throw new InvalidOperationException($"The DataSource must implement IEnumerable<{typeof(TItemType).FullName}> (such as most list types) or IListSource (such as DataSet or DataTable).");
		}

		private IEnumerable<TItemType> GetListSourceData(IListSource listSource)
		{
			if (typeof(TItemType) != typeof(object))
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
				return list.OfType<object>() as IEnumerable<TItemType>;
			}
			else
			{
				// If not, then this is a list of lists, so find the list within

				if (list is ITypedList typedList)
				{
					var propDescs = typedList.GetItemProperties(Array.Empty<PropertyDescriptor>());
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
							return innerListEnumerable.OfType<object>() as IEnumerable<TItemType>;
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

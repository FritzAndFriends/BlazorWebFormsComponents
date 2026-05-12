using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace BlazorWebFormsComponents.DataBinding
{
	public class DataBoundComponent<ItemType> : BaseDataBoundComponent
	{
		[Parameter]
		public string DataMember { get; set; }

		/// <summary>
		/// String-based method name for data retrieval, matching the ASP.NET Web Forms
		/// <c>SelectMethod="GetProducts"</c> pattern. The named method is resolved via
		/// reflection on the hosting <see cref="WebFormsPageBase"/> page.
		/// Supports methods returning IEnumerable&lt;T&gt;, IQueryable&lt;T&gt;, List&lt;T&gt;, etc.
		/// </summary>
		[Parameter]
		public string SelectMethod { get; set; }

		/// <summary>
		/// String-based method name for inserting items, matching the ASP.NET Web Forms
		/// <c>InsertMethod="InsertItem"</c> pattern.
		/// </summary>
		[Parameter]
		public string InsertMethod { get; set; }

		/// <summary>
		/// String-based method name for updating items, matching the ASP.NET Web Forms
		/// <c>UpdateMethod="UpdateItem"</c> pattern.
		/// </summary>
		[Parameter]
		public string UpdateMethod { get; set; }

		/// <summary>
		/// String-based method name for deleting items, matching the ASP.NET Web Forms
		/// <c>DeleteMethod="DeleteItem"</c> pattern.
		/// </summary>
		[Parameter]
		public string DeleteMethod { get; set; }

		/// <summary>
		/// Delegate-based select method with pagination support.
		/// Prefer string-based <see cref="SelectMethod"/> for Web Forms compatibility.
		/// </summary>
		[Parameter]
		public SelectHandler<ItemType> SelectMethodDelegate { get; set; }

		/// <summary>
		/// Simplified delegate-based select method without the out parameter.
		/// Prefer string-based <see cref="SelectMethod"/> for Web Forms compatibility.
		/// </summary>
		[Parameter]
		public SimpleSelectHandler<ItemType> SelectItems { get; set; }

		[Parameter]
		public SelectHandlerAsync<ItemType> SelectMethodAsync { get; set; }

		[Parameter]
		public IEnumerable<ItemType> Items
		{
			get { return ItemsList; }
			set { ItemsList = value?.ToList(); }
		}

		protected List<ItemType> ItemsList { get; set; }

		/// <summary>
		/// The hosting WebFormsPageBase, received via cascading parameter.
		/// Used to resolve string-based SelectMethod/InsertMethod/UpdateMethod/DeleteMethod
		/// via reflection, exactly as ASP.NET Web Forms resolves model-binding methods.
		/// </summary>
		[CascadingParameter(Name = WebFormsPageBase.CascadingParameterName)]
		protected WebFormsPageBase HostingPage { get; set; }

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

		private IEnumerable<ItemType> GetDataSource(object dataSource)
		{
			if (dataSource is IEnumerable<ItemType> enumerableOfItemType)
			{
				return enumerableOfItemType;
			}

			if (dataSource is IListSource listSource)
			{
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
				return list.OfType<object>() as IEnumerable<ItemType>;
			}
			else
			{
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
			base.OnAfterRender(firstRender);
		}

		/// <summary>
		/// Ensures that string-based SelectMethod has a valid hosting page to resolve against.
		/// </summary>
		private void RequireHostingPage(string methodAttributeName, string methodName)
		{
			if (HostingPage == null)
			{
				throw new InvalidOperationException(
					$"The data-bound component '{GetType().Name}' has {methodAttributeName}=\"{methodName}\" " +
					$"but no hosting page was found. String-based method resolution requires the page to " +
					$"inherit from WebFormsPageBase, which cascades itself to child components. " +
					$"Either make your page inherit from WebFormsPageBase, or use the Items parameter " +
					$"with direct data binding instead.");
			}
		}

		protected override void OnParametersSet()
		{
			base.OnParametersSet();

			// Priority: SelectMethodAsync (handled in OnParametersSetAsync) > string SelectMethod > delegate SelectItems > delegate SelectMethodDelegate
			if (SelectMethodAsync == null)
			{
				if (!string.IsNullOrEmpty(SelectMethod))
				{
					RequireHostingPage(nameof(SelectMethod), SelectMethod);
					Items = SelectMethodResolver.InvokeSelectMethod<ItemType>(HostingPage, SelectMethod);
				}
				else if (SelectItems != null)
				{
					Items = SelectItems(int.MaxValue, 0, "");
				}
				else if (SelectMethodDelegate != null)
				{
					Items = SelectMethodDelegate(int.MaxValue, 0, "", out var totalRowCount);
				}
			}
		}

		protected override async Task OnParametersSetAsync()
		{
			await base.OnParametersSetAsync();

			if (SelectMethodAsync != null)
			{
				var result = await SelectMethodAsync(int.MaxValue, 0, "");
				Items = result;
			}
			else if (!string.IsNullOrEmpty(SelectMethod) && HostingPage != null)
			{
				// Check if the method on the page is async
				if (SelectMethodResolver.HasMethod(HostingPage, SelectMethod))
				{
					var method = HostingPage.GetType().GetMethod(SelectMethod,
						System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
					if (method != null && typeof(Task).IsAssignableFrom(method.ReturnType))
					{
						Items = await SelectMethodResolver.InvokeSelectMethodAsync<ItemType>(HostingPage, SelectMethod);
					}
				}
			}
		}

		/// <summary>
		/// Re-invokes the appropriate select method to refresh data after CRUD operations.
		/// </summary>
		protected void RefreshSelectMethod()
		{
			if (SelectMethodAsync != null)
			{
				_ = RefreshSelectMethodAsync();
			}
			else if (!string.IsNullOrEmpty(SelectMethod) && HostingPage != null)
			{
				Items = SelectMethodResolver.InvokeSelectMethod<ItemType>(HostingPage, SelectMethod);
				StateHasChanged();
			}
			else if (SelectItems != null)
			{
				var result = SelectItems(int.MaxValue, 0, "");
				Items = result;
				StateHasChanged();
			}
			else if (SelectMethodDelegate != null)
			{
				Items = SelectMethodDelegate(int.MaxValue, 0, "", out var totalRowCount);
				StateHasChanged();
			}
		}

		private async Task RefreshSelectMethodAsync()
		{
			var result = await SelectMethodAsync(int.MaxValue, 0, "");
			Items = result;
			StateHasChanged();
		}

		/// <summary>
		/// Invokes the InsertMethod on the hosting page, if configured.
		/// </summary>
		protected async Task InvokeInsertMethodAsync(params object[] args)
		{
			if (string.IsNullOrEmpty(InsertMethod)) return;
			RequireHostingPage(nameof(InsertMethod), InsertMethod);
			await SelectMethodResolver.InvokeActionMethodAsync(HostingPage, InsertMethod, args);
			RefreshSelectMethod();
		}

		/// <summary>
		/// Invokes the UpdateMethod on the hosting page, if configured.
		/// </summary>
		protected async Task InvokeUpdateMethodAsync(params object[] args)
		{
			if (string.IsNullOrEmpty(UpdateMethod)) return;
			RequireHostingPage(nameof(UpdateMethod), UpdateMethod);
			await SelectMethodResolver.InvokeActionMethodAsync(HostingPage, UpdateMethod, args);
			RefreshSelectMethod();
		}

		/// <summary>
		/// Invokes the DeleteMethod on the hosting page, if configured.
		/// </summary>
		protected async Task InvokeDeleteMethodAsync(params object[] args)
		{
			if (string.IsNullOrEmpty(DeleteMethod)) return;
			RequireHostingPage(nameof(DeleteMethod), DeleteMethod);
			await SelectMethodResolver.InvokeActionMethodAsync(HostingPage, DeleteMethod, args);
			RefreshSelectMethod();
		}
	}
}

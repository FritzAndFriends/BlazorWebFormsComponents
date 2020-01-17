using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents
{

	public abstract class BaseModelBindingComponent<ItemType> : BaseWebFormsComponent {

		// Cheer 300 svavablount 15/12/19 
		// Cheer 200 nothing_else_matters 15/12/19 

		public delegate IQueryable<ItemType> SelectHandler(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount); 

		/// <summary>
		/// Data retrieval method to databind the collection to
		/// </summary>
		[Parameter]
		public SelectHandler SelectMethod { get; set; }

    [Parameter]
    public IEnumerable<ItemType> Items { get; set; }

    [Parameter]
    public IEnumerable<ItemType> DataSource
    {
      get { return Items; }
      set { 
        Items = value;
        this.StateHasChanged();
      }
    }

		protected override void OnAfterRender(bool firstRender)
		{

			if (firstRender && SelectMethod != null) {

				// Model Binding
				int totalRowCount;
				Items = SelectMethod(int.MaxValue, 0, "", out totalRowCount);

			}

			base.OnAfterRender(firstRender);

		}

		#region Data Binding Events

		[Parameter]
		public EventCallback<EventArgs> OnDataBinding { get; set; }

		[Parameter]
		public EventCallback<EventArgs> OnDataBound { get; set; }

		[Parameter]
		public EventCallback<ListViewItemEventArgs> OnItemDataBound { get; set; }

		#endregion

	}

}

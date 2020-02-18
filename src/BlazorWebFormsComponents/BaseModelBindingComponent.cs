﻿using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
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
    public IEnumerable<ItemType> Items {
			get { return ItemsList; }
			set { ItemsList = value?.ToList(); }
		}

		protected List<ItemType> ItemsList { get; set; }

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

	}

}

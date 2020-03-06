using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Collections.Generic;

namespace BlazorWebFormsComponents.GridView
{
  public partial class GridView<ItemType> : BaseModelBindingComponent<ItemType>
  {
		[Parameter] public bool AutogenerateColumns { get; set; } = true;

		[Parameter] public string EmptyDataText { get; set; }

		[Parameter] public string DataKeyNames { get; set; }

		[Parameter] public string CssClass { get; set; }

		[Parameter] public RenderFragment Columns { get; set; }

		public List<IColumn<ItemType>> GridColumns { get; } = new List<IColumn<ItemType>>();

		[Parameter] public RenderFragment ChildContent { get; set; }

		protected override void OnInitialized()
		{
			base.OnInitialized();
			if (AutogenerateColumns)
			{
				GridViewColumnGenerator.AutogenerateColumns(this);
			}
		}


		public void AddColumn(IColumn<ItemType> column)
		{
			GridColumns.Add(column);
			StateHasChanged();
		}

		public void RemoveColumn(IColumn<ItemType> column)
		{
			GridColumns.Remove(column);
			StateHasChanged();
		}
	}
}

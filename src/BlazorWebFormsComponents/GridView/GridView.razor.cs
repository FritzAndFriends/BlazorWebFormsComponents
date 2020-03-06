using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Collections.Generic;

namespace BlazorWebFormsComponents.GridView
{
  public partial class GridView<ItemType> : BaseModelBindingComponent<ItemType>
  {
		[Parameter] public bool AutogenerateColumns { get; set; }

		[Parameter] public string EmptyDataText { get; set; }

		[Parameter] public string DataKeyNames { get; set; }

		[Parameter] public string CssClass { get; set; }

		[Parameter] public RenderFragment Columns { get; set; }

		public List<IColumn<ItemType>> GridColumns { get; } = new List<IColumn<ItemType>>();

		[Parameter] public RenderFragment ChildContent { get; set; }

		[CascadingParameter(Name = "Host")] public BaseWebFormsComponent HostComponent { get; set; }

		protected override void OnInitialized()
		{
			HostComponent = this;
			base.OnInitialized();
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

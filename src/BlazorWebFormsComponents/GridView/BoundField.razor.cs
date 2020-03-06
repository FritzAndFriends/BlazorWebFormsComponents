using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.GridView
{
	public partial class BoundField<ItemType> :  ComponentBase, IColumn<ItemType>
	{
		[CascadingParameter(Name = "GridView")]
		public GridView<ItemType> GridView { get; set; }

		[Parameter]
		public string HeaderText { get; set; }

		[Parameter]
		public string DataField { get; set; }

		[Parameter]
		public string DataFormatString { get; set; } = null;

		public void Dispose()
		{
			GridView.RemoveColumn(this);
		}

		protected override void OnInitialized()
		{
			GridView.AddColumn(this);
		}
	}
}

using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.GridView
{
  public partial class TemplateField<ItemType> : ComponentBase, IColumn<ItemType>
  {
		[CascadingParameter(Name = "GridView")]
		public GridView<ItemType> GridView { get; set; }

		[Parameter]
		public string HeaderText { get; set; }

		[Parameter]
		public RenderFragment<ItemType> ItemTemplate { get; set; }


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

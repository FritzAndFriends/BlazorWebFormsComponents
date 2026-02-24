using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class DataGridItemStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "ParentDataGrid")]
		protected IDataGridStyleContainer ParentDataGrid { get; set; }

		protected override void OnInitialized()
		{
			if (ParentDataGrid != null)
			{
				theStyle = ParentDataGrid.ItemStyle;
			}
			base.OnInitialized();
		}
	}
}

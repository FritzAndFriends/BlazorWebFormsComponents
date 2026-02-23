using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class GridViewRowStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "ParentGridView")]
		protected IGridViewStyleContainer ParentGridView { get; set; }

		protected override void OnInitialized()
		{
			if (ParentGridView != null)
			{
				theStyle = ParentGridView.RowStyle;
			}
			base.OnInitialized();
		}
	}
}

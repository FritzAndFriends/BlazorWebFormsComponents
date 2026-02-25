using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class GridViewHeaderStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "ParentGridView")]
		protected IGridViewStyleContainer ParentGridView { get; set; }

		protected override void OnInitialized()
		{
			if (ParentGridView != null)
			{
				theStyle = ParentGridView.HeaderStyle;
			}
			base.OnInitialized();
		}
	}
}

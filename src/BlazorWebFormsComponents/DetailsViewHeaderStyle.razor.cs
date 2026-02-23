using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class DetailsViewHeaderStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "ParentDetailsView")]
		protected IDetailsViewStyleContainer ParentDetailsView { get; set; }

		protected override void OnInitialized()
		{
			if (ParentDetailsView != null)
			{
				theStyle = ParentDetailsView.HeaderStyle;
			}
			base.OnInitialized();
		}
	}
}

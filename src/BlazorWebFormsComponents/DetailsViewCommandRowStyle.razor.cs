using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class DetailsViewCommandRowStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "ParentDetailsView")]
		protected IDetailsViewStyleContainer ParentDetailsView { get; set; }

		protected override void OnInitialized()
		{
			if (ParentDetailsView != null)
			{
				theStyle = ParentDetailsView.CommandRowStyle;
			}
			base.OnInitialized();
		}
	}
}

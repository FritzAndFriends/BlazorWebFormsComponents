using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class FormViewPagerStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "ParentFormView")]
		protected IFormViewStyleContainer ParentFormView { get; set; }

		protected override void OnInitialized()
		{
			if (ParentFormView != null)
			{
				theStyle = ParentFormView.PagerStyle;
			}
			base.OnInitialized();
		}
	}
}

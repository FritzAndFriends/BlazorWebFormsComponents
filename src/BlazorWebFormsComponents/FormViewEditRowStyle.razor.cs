using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class FormViewEditRowStyle : UiTableItemStyle
	{
		[CascadingParameter(Name = "ParentFormView")]
		protected IFormViewStyleContainer ParentFormView { get; set; }

		protected override void OnInitialized()
		{
			if (ParentFormView != null)
			{
				theStyle = ParentFormView.EditRowStyle;
			}
			base.OnInitialized();
		}
	}
}

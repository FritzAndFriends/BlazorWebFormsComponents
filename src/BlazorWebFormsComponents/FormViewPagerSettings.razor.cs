using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class FormViewPagerSettings : UiPagerSettings
	{
		[CascadingParameter(Name = "ParentFormView")]
		protected IPagerSettingsContainer ParentFormView { get; set; }

		protected override void OnInitialized()
		{
			if (ParentFormView != null)
			{
				theSettings = ParentFormView.PagerSettings;
			}
			base.OnInitialized();
		}
	}
}

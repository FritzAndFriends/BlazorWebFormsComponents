using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class DetailsViewPagerSettings : UiPagerSettings
	{
		[CascadingParameter(Name = "ParentDetailsView")]
		protected IPagerSettingsContainer ParentDetailsView { get; set; }

		protected override void OnInitialized()
		{
			if (ParentDetailsView != null)
			{
				theSettings = ParentDetailsView.PagerSettings;
			}
			base.OnInitialized();
		}
	}
}

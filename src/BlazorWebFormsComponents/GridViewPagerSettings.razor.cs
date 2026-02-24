using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class GridViewPagerSettings : UiPagerSettings
	{
		[CascadingParameter(Name = "ParentGridView")]
		protected IPagerSettingsContainer ParentGridView { get; set; }

		protected override void OnInitialized()
		{
			if (ParentGridView != null)
			{
				theSettings = ParentGridView.PagerSettings;
			}
			base.OnInitialized();
		}
	}
}

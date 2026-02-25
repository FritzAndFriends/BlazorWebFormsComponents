using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Abstract base component for PagerSettings sub-components.
	/// Follows the same CascadingParameter pattern as UiTableItemStyle.
	/// </summary>
	public abstract class UiPagerSettings : ComponentBase
	{
		protected PagerSettings theSettings { get; set; }

		[Parameter]
		public PagerButtons Mode { get; set; } = PagerButtons.Numeric;

		[Parameter]
		public int PageButtonCount { get; set; } = 10;

		[Parameter]
		public string FirstPageText { get; set; } = "...";

		[Parameter]
		public string LastPageText { get; set; } = "...";

		[Parameter]
		public string NextPageText { get; set; } = ">";

		[Parameter]
		public string PreviousPageText { get; set; } = "<";

		[Parameter]
		public string FirstPageImageUrl { get; set; }

		[Parameter]
		public string LastPageImageUrl { get; set; }

		[Parameter]
		public string NextPageImageUrl { get; set; }

		[Parameter]
		public string PreviousPageImageUrl { get; set; }

		[Parameter]
		public PagerPosition Position { get; set; } = PagerPosition.Bottom;

		[Parameter]
		public bool Visible { get; set; } = true;

		protected override void OnInitialized()
		{
			if (theSettings != null)
			{
				theSettings.Mode = Mode;
				theSettings.PageButtonCount = PageButtonCount;
				theSettings.FirstPageText = FirstPageText;
				theSettings.LastPageText = LastPageText;
				theSettings.NextPageText = NextPageText;
				theSettings.PreviousPageText = PreviousPageText;
				theSettings.FirstPageImageUrl = FirstPageImageUrl;
				theSettings.LastPageImageUrl = LastPageImageUrl;
				theSettings.NextPageImageUrl = NextPageImageUrl;
				theSettings.PreviousPageImageUrl = PreviousPageImageUrl;
				theSettings.Position = Position;
				theSettings.Visible = Visible;
			}
		}
	}
}

using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents
{
	public partial class Panel : BaseStyledComponent
	{
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[Parameter]
		public string DefaultButton { get; set; }

		[Parameter]
		public ContentDirection Direction { get; set; } = ContentDirection.NotSet;

		[Parameter]
		public string GroupingText { get; set; }

		[Parameter]
		public HorizontalAlign HorizontalAlign { get; set; } = HorizontalAlign.NotSet;

		[Parameter]
		public ScrollBars ScrollBars { get; set; } = ScrollBars.None;

		[Parameter]
		public bool Wrap { get; set; } = true;

		private string DirectionAttr => Direction switch
		{
			ContentDirection.LeftToRight => "ltr",
			ContentDirection.RightToLeft => "rtl",
			_ => null
		};

		private string ComputedStyle => BuildStyle();

		private string BuildStyle()
		{
			var baseStyle = Style;
			var styles = new List<string>();

			if (!string.IsNullOrEmpty(baseStyle))
				styles.Add(baseStyle);

			// Add HorizontalAlign
			if (HorizontalAlign != HorizontalAlign.NotSet)
			{
				var alignValue = HorizontalAlign.ToString().ToLowerInvariant();
				styles.Add($"text-align:{alignValue}");
			}

			// Add ScrollBars
			var overflowStyle = ScrollBars switch
			{
				ScrollBars.Horizontal => "overflow-x:scroll;overflow-y:hidden",
				ScrollBars.Vertical => "overflow-x:hidden;overflow-y:scroll",
				ScrollBars.Both => "overflow:scroll",
				ScrollBars.Auto => "overflow:auto",
				_ => null
			};
			if (overflowStyle != null)
				styles.Add(overflowStyle);

			// Add Wrap
			if (!Wrap)
				styles.Add("white-space:nowrap");

			var result = string.Join(";", styles.Where(s => !string.IsNullOrEmpty(s)));
			return string.IsNullOrEmpty(result) ? null : result;
		}
	}
}

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
		public EnumParameter<ContentDirection> Direction { get; set; } = ContentDirection.NotSet;

		[Parameter]
		public string GroupingText { get; set; }

		[Parameter]
		public EnumParameter<HorizontalAlign> HorizontalAlign { get; set; } = Enums.HorizontalAlign.NotSet;

		[Parameter]
		public EnumParameter<ScrollBars> ScrollBars { get; set; } = Enums.ScrollBars.None;

		[Parameter]
		public string BackImageUrl { get; set; }

		[Parameter]
		public bool Wrap { get; set; } = true;

		private string DirectionAttr => Direction.Value switch
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
			if (HorizontalAlign.Value != Enums.HorizontalAlign.NotSet)
			{
				var alignValue = HorizontalAlign.ToString().ToLowerInvariant();
				styles.Add($"text-align:{alignValue}");
			}

			// Add ScrollBars
			var overflowStyle = ScrollBars.Value switch
			{
				Enums.ScrollBars.Horizontal => "overflow-x:scroll;overflow-y:hidden",
				Enums.ScrollBars.Vertical => "overflow-x:hidden;overflow-y:scroll",
				Enums.ScrollBars.Both => "overflow:scroll",
				Enums.ScrollBars.Auto => "overflow:auto",
				_ => null
			};
			if (overflowStyle != null)
				styles.Add(overflowStyle);

			// Add BackImageUrl
			if (!string.IsNullOrEmpty(BackImageUrl))
				styles.Add($"background-image:url({BackImageUrl})");

			// Add Wrap
			if (!Wrap)
				styles.Add("white-space:nowrap");

			var result = string.Join(";", styles.Where(s => !string.IsNullOrEmpty(s)));
			return string.IsNullOrEmpty(result) ? null : result;
		}
	}
}

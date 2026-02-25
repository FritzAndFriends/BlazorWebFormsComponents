using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents an HTML table control.
	/// Emulates the ASP.NET Web Forms Table control.
	/// </summary>
	public partial class Table : BaseStyledComponent
	{
		/// <summary>
		/// Gets or sets the child content of the table (rows).
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// Gets or sets the caption text for the table.
		/// </summary>
		[Parameter]
		public string Caption { get; set; }

		/// <summary>
		/// Gets or sets the alignment of the caption.
		/// </summary>
		[Parameter]
		public TableCaptionAlign CaptionAlign { get; set; } = TableCaptionAlign.NotSet;

		/// <summary>
		/// Gets or sets the cell padding in pixels.
		/// </summary>
		[Parameter]
		public int CellPadding { get; set; } = -1;

		/// <summary>
		/// Gets or sets the cell spacing in pixels.
		/// </summary>
		[Parameter]
		public int CellSpacing { get; set; } = -1;

		/// <summary>
		/// Gets or sets which grid lines are displayed.
		/// </summary>
		[Parameter]
		public GridLines GridLines { get; set; } = GridLines.None;

		/// <summary>
		/// Gets or sets the horizontal alignment of the table.
		/// </summary>
		[Parameter]
		public HorizontalAlign HorizontalAlign { get; set; } = HorizontalAlign.NotSet;

		/// <summary>
		/// Gets or sets the background image URL for the table.
		/// </summary>
		[Parameter]
		public string BackImageUrl { get; set; }

		/// <summary>
		/// Gets the cellpadding attribute value, or null if not set.
		/// </summary>
		protected int? CellPaddingValue => CellPadding >= 0 ? CellPadding : null;

		/// <summary>
		/// Gets the cellspacing attribute value, or null if not set.
		/// </summary>
		protected int? CellSpacingValue => CellSpacing >= 0 ? CellSpacing : null;

		/// <summary>
		/// Gets the caption-side CSS value.
		/// </summary>
		protected string CaptionSideStyle => CaptionAlign switch
		{
			TableCaptionAlign.Top => "caption-side: top",
			TableCaptionAlign.Bottom => "caption-side: bottom",
			_ => null
		};

		/// <summary>
		/// Gets the rules attribute for grid lines (deprecated HTML, but matches Web Forms output).
		/// </summary>
		protected string RulesAttribute => GridLines switch
		{
			GridLines.Horizontal => "rows",
			GridLines.Vertical => "cols",
			GridLines.Both => "all",
			_ => null
		};

		/// <summary>
		/// Gets the border attribute for the table.
		/// </summary>
		protected int? BorderAttribute => GridLines != GridLines.None ? 1 : null;

		/// <summary>
		/// Gets the combined style string.
		/// </summary>
		protected string CombinedStyle
		{
			get
			{
				var styles = new List<string>();

				var baseStyle = Style;
				if (!string.IsNullOrEmpty(baseStyle))
					styles.Add(baseStyle);

				if (HorizontalAlign != HorizontalAlign.NotSet)
				{
					var marginStyle = HorizontalAlign switch
					{
						HorizontalAlign.Left => "margin-right: auto",
						HorizontalAlign.Center => "margin-left: auto; margin-right: auto",
						HorizontalAlign.Right => "margin-left: auto",
						_ => null
					};
					if (marginStyle != null)
						styles.Add(marginStyle);
				}

				if (!string.IsNullOrEmpty(BackImageUrl))
				{
					styles.Add($"background-image: url('{BackImageUrl}')");
				}

				// Modern CSS for grid lines (in addition to deprecated attributes for compatibility)
				if (GridLines != GridLines.None)
				{
					styles.Add("border-collapse: collapse");
				}

				return styles.Count > 0 ? string.Join("; ", styles) : null;
			}
		}
	}
}

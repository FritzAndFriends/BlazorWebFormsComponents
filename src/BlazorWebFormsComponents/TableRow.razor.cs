using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents a row in a Table control.
	/// Emulates the ASP.NET Web Forms TableRow control.
	/// </summary>
	public partial class TableRow : BaseStyledComponent
	{
		/// <summary>
		/// Gets or sets the child content of the row (cells).
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// Gets or sets the section of the table where this row belongs.
		/// </summary>
		[Parameter]
		public TableRowSection TableSection { get; set; } = TableRowSection.TableBody;

		/// <summary>
		/// Gets or sets the horizontal alignment of the row content.
		/// </summary>
		[Parameter]
		public HorizontalAlign HorizontalAlign { get; set; } = HorizontalAlign.NotSet;

		/// <summary>
		/// Gets or sets the vertical alignment of the row content.
		/// </summary>
		[Parameter]
		public VerticalAlign VerticalAlign { get; set; } = VerticalAlign.NotSet;

		/// <summary>
		/// Gets the alignment style for the row.
		/// </summary>
		protected string AlignmentStyle
		{
			get
			{
				var styles = new List<string>();

				if (HorizontalAlign != HorizontalAlign.NotSet)
				{
					var align = HorizontalAlign switch
					{
						HorizontalAlign.Left => "left",
						HorizontalAlign.Center => "center",
						HorizontalAlign.Right => "right",
						HorizontalAlign.Justify => "justify",
						_ => null
					};
					if (align != null)
						styles.Add($"text-align: {align}");
				}

				if (VerticalAlign != VerticalAlign.NotSet)
				{
					var valign = VerticalAlign switch
					{
						VerticalAlign.Top => "top",
						VerticalAlign.Middle => "middle",
						VerticalAlign.Bottom => "bottom",
						_ => null
					};
					if (valign != null)
						styles.Add($"vertical-align: {valign}");
				}

				return styles.Count > 0 ? string.Join("; ", styles) : null;
			}
		}

		/// <summary>
		/// Gets the combined style string.
		/// </summary>
		protected string CombinedStyle
		{
			get
			{
				var baseStyle = Style;
				var alignStyle = AlignmentStyle;

				if (!string.IsNullOrEmpty(baseStyle) && !string.IsNullOrEmpty(alignStyle))
					return $"{baseStyle}; {alignStyle}";
				if (!string.IsNullOrEmpty(baseStyle))
					return baseStyle;
				if (!string.IsNullOrEmpty(alignStyle))
					return alignStyle;
				return null;
			}
		}
	}
}

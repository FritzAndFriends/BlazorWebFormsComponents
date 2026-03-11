using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents a cell in a Table control.
	/// Emulates the ASP.NET Web Forms TableCell control.
	/// </summary>
	public partial class TableCell : BaseStyledComponent
	{
		/// <summary>
		/// Gets or sets the child content of the cell.
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// Gets or sets the number of columns the cell spans.
		/// </summary>
		[Parameter]
		public int ColumnSpan { get; set; } = 0;

		/// <summary>
		/// Gets or sets the number of rows the cell spans.
		/// </summary>
		[Parameter]
		public int RowSpan { get; set; } = 0;

		/// <summary>
		/// Gets or sets the horizontal alignment of the cell content.
		/// </summary>
		[Parameter]
		public EnumParameter<HorizontalAlign> HorizontalAlign { get; set; } = Enums.HorizontalAlign.NotSet;

		/// <summary>
		/// Gets or sets the vertical alignment of the cell content.
		/// </summary>
		[Parameter]
		public EnumParameter<VerticalAlign> VerticalAlign { get; set; } = Enums.VerticalAlign.NotSet;

		/// <summary>
		/// Gets or sets a value indicating whether the cell content wraps.
		/// </summary>
		[Parameter]
		public bool Wrap { get; set; } = true;

		/// <summary>
		/// Gets or sets the ID of the associated header cell for accessibility.
		/// </summary>
		[Parameter]
		public string AssociatedHeaderCellID { get; set; }

		/// <summary>
		/// Gets or sets the text content of the cell.
		/// </summary>
		[Parameter]
		public string Text { get; set; }

		/// <summary>
		/// Gets the colspan attribute value, or null if not set.
		/// </summary>
		protected int? ColSpanValue => ColumnSpan > 0 ? ColumnSpan : null;

		/// <summary>
		/// Gets the rowspan attribute value, or null if not set.
		/// </summary>
		protected int? RowSpanValue => RowSpan > 0 ? RowSpan : null;

		/// <summary>
		/// Gets the alignment style for the cell.
		/// </summary>
		protected string AlignmentStyle
		{
			get
			{
				var styles = new List<string>();

				if (HorizontalAlign.Value != Enums.HorizontalAlign.NotSet)
				{
					var align = HorizontalAlign.Value switch
					{
						Enums.HorizontalAlign.Left => "left",
						Enums.HorizontalAlign.Center => "center",
						Enums.HorizontalAlign.Right => "right",
						Enums.HorizontalAlign.Justify => "justify",
						_ => null
					};
					if (align != null)
						styles.Add($"text-align: {align}");
				}

				if (VerticalAlign.Value != Enums.VerticalAlign.NotSet)
				{
					var valign = VerticalAlign.Value switch
					{
						Enums.VerticalAlign.Top => "top",
						Enums.VerticalAlign.Middle => "middle",
						Enums.VerticalAlign.Bottom => "bottom",
						_ => null
					};
					if (valign != null)
						styles.Add($"vertical-align: {valign}");
				}

				if (!Wrap)
				{
					styles.Add("white-space: nowrap");
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

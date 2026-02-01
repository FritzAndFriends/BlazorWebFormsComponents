using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents a header cell in a Table control.
	/// Emulates the ASP.NET Web Forms TableHeaderCell control.
	/// </summary>
	public partial class TableHeaderCell : BaseStyledComponent
	{
		/// <summary>
		/// Gets or sets the child content of the header cell.
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
		public HorizontalAlign HorizontalAlign { get; set; } = HorizontalAlign.NotSet;

		/// <summary>
		/// Gets or sets the vertical alignment of the cell content.
		/// </summary>
		[Parameter]
		public VerticalAlign VerticalAlign { get; set; } = VerticalAlign.NotSet;

		/// <summary>
		/// Gets or sets a value indicating whether the cell content wraps.
		/// </summary>
		[Parameter]
		public bool Wrap { get; set; } = true;

		/// <summary>
		/// Gets or sets the scope of the header cell (row or column).
		/// </summary>
		[Parameter]
		public TableHeaderScope Scope { get; set; } = TableHeaderScope.NotSet;

		/// <summary>
		/// Gets or sets the abbreviated text for the header cell.
		/// </summary>
		[Parameter]
		public string AbbreviatedText { get; set; }

		/// <summary>
		/// Gets or sets the text content of the header cell.
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
		/// Gets the scope attribute value, or null if not set.
		/// </summary>
		protected string ScopeValue => Scope switch
		{
			TableHeaderScope.Row => "row",
			TableHeaderScope.Column => "col",
			_ => null
		};

		/// <summary>
		/// Gets the alignment style for the cell.
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

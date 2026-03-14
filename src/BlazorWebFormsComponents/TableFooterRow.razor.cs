using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents a footer row in a Table control.
	/// Emulates the ASP.NET Web Forms TableFooterRow control.
	/// </summary>
	public partial class TableFooterRow : BaseStyledComponent
	{
		/// <summary>
		/// Gets or sets the child content of the row (cells).
		/// </summary>
		[Parameter]
		public RenderFragment ChildContent { get; set; }

		/// <summary>
		/// Gets the section of the table where this row belongs (always TableFooter).
		/// </summary>
		public TableRowSection TableSection => TableRowSection.TableFooter;

		/// <summary>
		/// Gets or sets the horizontal alignment of the row content.
		/// </summary>
		[Parameter]
		public EnumParameter<HorizontalAlign> HorizontalAlign { get; set; } = Enums.HorizontalAlign.NotSet;

		/// <summary>
		/// Gets or sets the vertical alignment of the row content.
		/// </summary>
		[Parameter]
		public EnumParameter<VerticalAlign> VerticalAlign { get; set; } = Enums.VerticalAlign.NotSet;

		/// <summary>
		/// Gets the alignment style for the row.
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

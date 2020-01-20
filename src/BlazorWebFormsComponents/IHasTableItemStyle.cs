using BlazorWebFormsComponents.Enums;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BlazorWebFormsComponents
{

	public interface IHasTableItemStyle : IHasLayoutTableItemStyle, IHasFontStyle { }

	public interface IHasStyle : IHasLayoutStyle, IHasFontStyle { }

	public interface IHasLayoutTableItemStyle : IHasLayoutStyle
	{

		HorizontalAlign HorizontalAlign { get; set; }

		VerticalAlign VerticalAlign { get; set; }

		bool Wrap { get; set; }

	}

	public interface IHasLayoutStyle
	{

		// Cheer 1042 codingbandit 07/1/20 
		// Cheer 100 ramblinggeek 07/1/20 


		WebColor BackColor { get; set; }

		WebColor BorderColor { get; set; }

		BorderStyle BorderStyle { get; set; }

		Unit BorderWidth { get; set; }

		string CssClass { get; set; }

		WebColor ForeColor { get; set; }

		Unit Height { get; set; }

		Unit Width { get; set; }

	}

	public interface IHasFontStyle
	{


		bool Font_Bold { get; set; }

		bool Font_Italic { get; set; }

		string Font_Names { get; set; }

		bool Font_Overline { get; set; }

		FontUnit Font_Size { get; set; }

		bool Font_Strikeout { get; set; }

		bool Font_Underline { get; set; }


	}


	public static class IHasStyleExtensions
	{

		public static void CopyTo(this IHasStyle source, IHasStyle destination)
		{

			destination.BackColor = source.BackColor;
			destination.BorderColor = source.BorderColor;
			destination.BorderStyle = source.BorderStyle;
			destination.BorderWidth = source.BorderWidth;
			destination.CssClass = source.CssClass;
			destination.ForeColor = source.ForeColor;
			destination.Height = source.Height;
			destination.Width = source.Width;
			destination.Font_Bold = source.Font_Bold;
			destination.Font_Italic = source.Font_Italic;
			destination.Font_Names = source.Font_Names;
			destination.Font_Overline = source.Font_Overline;
			destination.Font_Size = source.Font_Size;
			destination.Font_Strikeout = source.Font_Strikeout;
			destination.Font_Underline = source.Font_Underline;

		}

		public static string ToStyleString(this IHasTableItemStyle hasStyle)
		{

			return ToStyleString(hasStyle, new StringBuilder()).ToString();

		}

		public static StringBuilder ToStyleString(this IHasTableItemStyle hasStyle, StringBuilder sb)
		{

			((IHasStyle)hasStyle).ToStyleString(sb);

			if (!hasStyle.Wrap) sb.Append("white-space:nowrap;");

			return sb;

		}

		public static string ToStyleString(this IHasStyle hasStyle)
		{

			return ToStyleString(hasStyle, new StringBuilder()).ToString();

		}

		public static StringBuilder ToStyleString(this IHasStyle hasStyle, StringBuilder sb)
		{

			if (hasStyle.BackColor != default(WebColor)) sb.Append($"background-color:{ColorTranslator.ToHtml(hasStyle.BackColor.ToColor()).Trim()};");
			if (hasStyle.ForeColor != default(WebColor)) sb.Append($"color:{ColorTranslator.ToHtml(hasStyle.ForeColor.ToColor())};");
			if (hasStyle.BorderStyle != BorderStyle.None && hasStyle.BorderStyle != BorderStyle.NotSet && hasStyle.BorderWidth.Value > 0 && hasStyle.BorderColor != default(WebColor))
			{

				sb.Append($"border:{hasStyle.BorderWidth.ToString()} {hasStyle.BorderStyle.ToString().ToLowerInvariant()} {ColorTranslator.ToHtml(hasStyle.BorderColor.ToColor())};");

			}

			if (hasStyle.Font_Bold) sb.Append("font-weight:bold;");
			if (hasStyle.Font_Italic) sb.Append("font-style:italic;");
			if (!string.IsNullOrEmpty(hasStyle.Font_Names)) sb.Append($"font-family:{hasStyle.Font_Names};");
			if (hasStyle.Font_Size != FontUnit.Empty) sb.Append($"font-size:{hasStyle.Font_Size.ToString()};");
			if (hasStyle.Font_Underline || hasStyle.Font_Overline || hasStyle.Font_Strikeout)
			{
				sb.Append("text-decoration:");

				var td = new StringBuilder();
				if (hasStyle.Font_Underline) td.Append("underline ");
				if (hasStyle.Font_Overline) td.Append("overline ");
				if (hasStyle.Font_Strikeout) td.Append("line-through");
				sb.Append(td.ToString().Trim());
				sb.Append(";");
			}

			return sb;


		}

		public static void SetFontsFromAttributes(this IHasFontStyle hasStyle, Dictionary<string, object> additionalAttributes)
		{

			if (additionalAttributes != null)
			{
				if (additionalAttributes.ContainsKey("Font-Bold")) hasStyle.Font_Bold = bool.Parse(additionalAttributes["Font-Bold"].ToString());
				if (additionalAttributes.ContainsKey("Font-Italic")) hasStyle.Font_Italic = bool.Parse(additionalAttributes["Font-Italic"].ToString());
				if (additionalAttributes.ContainsKey("Font-Names")) hasStyle.Font_Names = additionalAttributes["Font-Names"].ToString();
				if (additionalAttributes.ContainsKey("Font-Overline")) hasStyle.Font_Overline = bool.Parse(additionalAttributes["Font-Overline"].ToString());
				if (additionalAttributes.ContainsKey("Font-Size")) hasStyle.Font_Size = FontUnit.Parse(additionalAttributes["Font-Size"].ToString());
				if (additionalAttributes.ContainsKey("Font-Strikeout")) hasStyle.Font_Strikeout = bool.Parse(additionalAttributes["Font-Strikeout"].ToString());
				if (additionalAttributes.ContainsKey("Font-Underline")) hasStyle.Font_Underline = bool.Parse(additionalAttributes["Font-Underline"].ToString());
			}


		}

	}


}

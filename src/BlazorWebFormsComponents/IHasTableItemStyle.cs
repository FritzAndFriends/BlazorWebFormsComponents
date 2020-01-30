using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using System;
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

		public static StyleBuilder ToStyle(this IHasTableItemStyle hasStyle) =>
			((IHasStyle)hasStyle).ToStyle().AddStyle("white-space", "nowrap", !hasStyle.Wrap);


		public static StyleBuilder ToStyle(this IHasStyle hasStyle) => 
			StyleBuilder.Empty().AddStyle("background-color", () => ColorTranslator.ToHtml(hasStyle.BackColor.ToColor()).Trim(),
							when: hasStyle.BackColor != default(WebColor))

					.AddStyle("color", () => ColorTranslator.ToHtml(hasStyle.ForeColor.ToColor()).Trim(),
							when: hasStyle.ForeColor != default(WebColor))

					.AddStyle("border", v => v.AddValue(hasStyle.BorderWidth.ToString())
						.AddValue(hasStyle.BorderStyle.ToString().ToLowerInvariant())
						.AddValue(() => ColorTranslator.ToHtml(hasStyle.BorderColor.ToColor()), HasBorders(hasStyle)),
							when: HasBorders(hasStyle))

					.AddStyle("font-weight", "bold", hasStyle.Font_Bold)
					.AddStyle("font-style", "italic", hasStyle.Font_Italic)
					.AddStyle("font-family", hasStyle.Font_Names, !string.IsNullOrEmpty(hasStyle.Font_Names))
					.AddStyle("font-size", hasStyle.Font_Size.ToString(), hasStyle.Font_Size != FontUnit.Empty)
					.AddStyle("text-decoration", v => v.AddValue("underline", hasStyle.Font_Underline)
						.AddValue("overline", hasStyle.Font_Overline)
						.AddValue("line-through", hasStyle.Font_Strikeout)
						, HasTextDecorations(hasStyle));

		private static bool HasTextDecorations(IHasStyle hasStyle) =>
				hasStyle.Font_Underline ||
				hasStyle.Font_Overline ||
				hasStyle.Font_Strikeout;

		private static bool HasBorders(IHasStyle hasStyle) =>
						hasStyle.BorderStyle != BorderStyle.None &&
						hasStyle.BorderStyle != BorderStyle.NotSet &&
						hasStyle.BorderWidth.Value > 0 &&
						hasStyle.BorderColor != default(WebColor);

		public static void SetFontsFromAttributes(this IHasFontStyle hasStyle, Dictionary<string, object> additionalAttributes)
		{

			if (additionalAttributes != null)
			{
				hasStyle.Font_Bold =        additionalAttributes.GetValue("Font-Bold", bool.Parse, hasStyle.Font_Bold);
				hasStyle.Font_Italic =      additionalAttributes.GetValue("Font-Italic", bool.Parse, hasStyle.Font_Italic);
				hasStyle.Font_Underline =   additionalAttributes.GetValue("Font-Underline", bool.Parse, hasStyle.Font_Underline);

				hasStyle.Font_Names =       additionalAttributes.GetValue("Font-Names", a => a, hasStyle.Font_Names);
				hasStyle.Font_Overline =    additionalAttributes.GetValue("Font-Overline", bool.Parse, hasStyle.Font_Overline);
				hasStyle.Font_Size =        additionalAttributes.GetValue("Font-Size", FontUnit.Parse, hasStyle.Font_Size);
				hasStyle.Font_Strikeout =   additionalAttributes.GetValue("Font-Strikeout", bool.Parse, hasStyle.Font_Strikeout);
			}

		}

		public static T GetValue<T>(this Dictionary<string, object> additionalAttributes, string key, Func<string, T> parser, T defaultValue) =>
				additionalAttributes.TryGetValue(key, out var x) ? parser(x.ToString()) : defaultValue;


	}

}

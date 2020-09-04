using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using System;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	public static class HasStyleExtensions
	{
		public static void CopyTo(this IStyle source, IStyle destination)
		{

			destination.BackColor = source.BackColor;
			destination.BorderColor = source.BorderColor;
			destination.BorderStyle = source.BorderStyle;
			destination.BorderWidth = source.BorderWidth;
			destination.CssClass = source.CssClass;
			destination.ForeColor = source.ForeColor;
			destination.Height = source.Height;
			destination.Width = source.Width;
			destination.Font.Bold = source.Font.Bold;
			destination.Font.Italic = source.Font.Italic;
			destination.Font.Names = source.Font.Names;
			destination.Font.Overline = source.Font.Overline;
			destination.Font.Size = source.Font.Size;
			destination.Font.Strikeout = source.Font.Strikeout;
			destination.Font.Underline = source.Font.Underline;

		}

		public static StyleBuilder ToStyle(this IHasTableItemStyle hasStyle) =>
			((IStyle)hasStyle).ToStyle().AddStyle("white-space", "nowrap", !hasStyle.Wrap);


		public static StyleBuilder ToStyle(this IStyle hasStyle) =>
			StyleBuilder.Empty().AddStyle("background-color", () => ColorTranslator.ToHtml(hasStyle.BackColor.ToColor()).Trim(),
							when: hasStyle.BackColor != default(WebColor))

					.AddStyle("color", () => ColorTranslator.ToHtml(hasStyle.ForeColor.ToColor()).Trim(),
							when: hasStyle.ForeColor != default(WebColor))

					.AddStyle("border", v => v.AddValue(hasStyle.BorderWidth.ToString())
						.AddValue(hasStyle.BorderStyle.ToString().ToLowerInvariant())
						.AddValue(() => ColorTranslator.ToHtml(hasStyle.BorderColor.ToColor()), HasBorders(hasStyle)),
							when: HasBorders(hasStyle))

					.AddStyle("font-weight", "bold", hasStyle.Font.Bold)
					.AddStyle("font-style", "italic", hasStyle.Font.Italic)
					.AddStyle("font-family", hasStyle.Font.Names, !string.IsNullOrEmpty(hasStyle.Font.Names))
					.AddStyle("font-size", hasStyle.Font.Size.ToString(), hasStyle.Font.Size != FontUnit.Empty)
					.AddStyle("text-decoration", v => v.AddValue("underline", hasStyle.Font.Underline)
						.AddValue("overline", hasStyle.Font.Overline)
						.AddValue("line-through", hasStyle.Font.Strikeout)
						, HasTextDecorations(hasStyle));

		private static bool HasTextDecorations(IStyle hasStyle) =>
				hasStyle.Font.Underline ||
				hasStyle.Font.Overline ||
				hasStyle.Font.Strikeout;

		private static bool HasBorders(IStyle hasStyle) =>
						hasStyle.BorderStyle != BorderStyle.None &&
						hasStyle.BorderStyle != BorderStyle.NotSet &&
						hasStyle.BorderWidth.Value > 0 &&
						hasStyle.BorderColor != default(WebColor);

		public static void SetFontsFromAttributes(this IFontStyle hasStyle, Dictionary<string, object> additionalAttributes)
		{
			if (additionalAttributes != null)
			{
				hasStyle.Font.Bold = additionalAttributes.GetValue("Font-Bold", bool.Parse, hasStyle.Font.Bold);
				hasStyle.Font.Italic = additionalAttributes.GetValue("Font-Italic", bool.Parse, hasStyle.Font.Italic);
				hasStyle.Font.Underline = additionalAttributes.GetValue("Font-Underline", bool.Parse, hasStyle.Font.Underline);

				hasStyle.Font.Names = additionalAttributes.GetValue("Font-Names", a => a, hasStyle.Font.Names);
				hasStyle.Font.Overline = additionalAttributes.GetValue("Font-Overline", bool.Parse, hasStyle.Font.Overline);
				hasStyle.Font.Size = additionalAttributes.GetValue("Font-Size", FontUnit.Parse, hasStyle.Font.Size);
				hasStyle.Font.Strikeout = additionalAttributes.GetValue("Font-Strikeout", bool.Parse, hasStyle.Font.Strikeout);
			}
		}

		public static T GetValue<T>(this Dictionary<string, object> additionalAttributes, string key, Func<string, T> parser, T defaultValue) =>
				additionalAttributes.TryGetValue(key, out var x) ? parser(x.ToString()) : defaultValue;


	}

}

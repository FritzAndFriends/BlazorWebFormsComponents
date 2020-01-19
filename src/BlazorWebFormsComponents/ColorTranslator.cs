using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace BlazorWebFormsComponents
{

	/// <include file='doc\ColorTranslator.uex' path='docs/doc[@for="ColorTranslator"]/*' />
	/// <devdoc>
	///    Translates colors to and from GDI+ <see cref='System.Drawing.Color'/> objects.
	/// </devdoc>
	internal sealed class ColorTranslator
	{

		private static Hashtable htmlSysColorTable;

		// not creatable...
		//
		private ColorTranslator()
		{
		}

		/// <include file='doc\ColorTranslator.uex' path='docs/doc[@for="ColorTranslator.FromHtml"]/*' />
		/// <devdoc>
		///    Translates an Html color representation to
		///    a GDI+ <see cref='System.Drawing.Color'/>.
		/// </devdoc>
		public static WebColor FromHtml(string htmlColor)
		{

			var c = WebColor.Empty;

			// empty color
			if ((htmlColor == null) || (htmlColor.Length == 0))
				return c;

			// #RRGGBB or #RGB
			if ((htmlColor[0] == '#') &&
					((htmlColor.Length == 7) || (htmlColor.Length == 4)))
			{

				if (htmlColor.Length == 7)
				{
					c = Color.FromArgb(Convert.ToInt32(htmlColor.Substring(1, 2), 16),
														 Convert.ToInt32(htmlColor.Substring(3, 2), 16),
														 Convert.ToInt32(htmlColor.Substring(5, 2), 16));
				}
				else
				{
					var r = Char.ToString(htmlColor[1]);
					var g = Char.ToString(htmlColor[2]);
					var b = Char.ToString(htmlColor[3]);

					c = Color.FromArgb(Convert.ToInt32(r + r, 16),
														 Convert.ToInt32(g + g, 16),
														 Convert.ToInt32(b + b, 16));
				}
			}

			// special case. Html requires LightGrey, but .NET uses LightGray
			if (c.IsEmpty && String.Equals(htmlColor, "LightGrey", StringComparison.OrdinalIgnoreCase))
			{
				c = Color.LightGray;
			}

			// System color
			if (c.IsEmpty)
			{
				if (htmlSysColorTable == null)
				{
					InitializeHtmlSysColorTable();
				}

				var o = htmlSysColorTable[htmlColor.ToLower(CultureInfo.InvariantCulture)];
				if (o != null)
				{
					c = (Color)o;
				}
			}

			// resort to type converter which will handle named colors
			if (c.IsEmpty)
			{
				c = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(htmlColor);
			}

			return c;
		}

		/// <include file='doc\ColorTranslator.uex' path='docs/doc[@for="ColorTranslator.ToHtml"]/*' />
		/// <devdoc>
		///    <para>
		///       Translates the specified <see cref='System.Drawing.Color'/> to an Html string color representation.
		///    </para>
		/// </devdoc>
		public static string ToHtml(Color c)
		{
			var colorString = String.Empty;

			if (c.IsEmpty)
				return colorString;

			if (c.IsSystemColor)
			{
				switch (c.ToKnownColor())
				{
					case KnownColor.ActiveBorder: colorString = "activeborder"; break;
					case KnownColor.GradientActiveCaption:
					case KnownColor.ActiveCaption: colorString = "activecaption"; break;
					case KnownColor.AppWorkspace: colorString = "appworkspace"; break;
					case KnownColor.Desktop: colorString = "background"; break;
					case KnownColor.Control: colorString = "buttonface"; break;
					case KnownColor.ControlLight: colorString = "buttonface"; break;
					case KnownColor.ControlDark: colorString = "buttonshadow"; break;
					case KnownColor.ControlText: colorString = "buttontext"; break;
					case KnownColor.ActiveCaptionText: colorString = "captiontext"; break;
					case KnownColor.GrayText: colorString = "graytext"; break;
					case KnownColor.HotTrack:
					case KnownColor.Highlight: colorString = "highlight"; break;
					case KnownColor.MenuHighlight:
					case KnownColor.HighlightText: colorString = "highlighttext"; break;
					case KnownColor.InactiveBorder: colorString = "inactiveborder"; break;
					case KnownColor.GradientInactiveCaption:
					case KnownColor.InactiveCaption: colorString = "inactivecaption"; break;
					case KnownColor.InactiveCaptionText: colorString = "inactivecaptiontext"; break;
					case KnownColor.Info: colorString = "infobackground"; break;
					case KnownColor.InfoText: colorString = "infotext"; break;
					case KnownColor.MenuBar:
					case KnownColor.Menu: colorString = "menu"; break;
					case KnownColor.MenuText: colorString = "menutext"; break;
					case KnownColor.ScrollBar: colorString = "scrollbar"; break;
					case KnownColor.ControlDarkDark: colorString = "threeddarkshadow"; break;
					case KnownColor.ControlLightLight: colorString = "buttonhighlight"; break;
					case KnownColor.Window: colorString = "window"; break;
					case KnownColor.WindowFrame: colorString = "windowframe"; break;
					case KnownColor.WindowText: colorString = "windowtext"; break;
				}
			}
			else if (c.IsNamedColor)
			{
				if (c == Color.LightGray)
				{
					// special case due to mismatch between Html and enum spelling
					colorString = "LightGrey";
				}
				else
				{
					colorString = c.Name;
				}
			}
			else
			{
				colorString = "#" + c.R.ToString("X2", null) +
														c.G.ToString("X2", null) +
														c.B.ToString("X2", null);
			}

			return colorString;
		}

		private static void InitializeHtmlSysColorTable()
		{
			htmlSysColorTable = new Hashtable(26);
			htmlSysColorTable["activeborder"] = Color.FromKnownColor(KnownColor.ActiveBorder);
			htmlSysColorTable["activecaption"] = Color.FromKnownColor(KnownColor.ActiveCaption);
			htmlSysColorTable["appworkspace"] = Color.FromKnownColor(KnownColor.AppWorkspace);
			htmlSysColorTable["background"] = Color.FromKnownColor(KnownColor.Desktop);
			htmlSysColorTable["buttonface"] = Color.FromKnownColor(KnownColor.Control);
			htmlSysColorTable["buttonhighlight"] = Color.FromKnownColor(KnownColor.ControlLightLight);
			htmlSysColorTable["buttonshadow"] = Color.FromKnownColor(KnownColor.ControlDark);
			htmlSysColorTable["buttontext"] = Color.FromKnownColor(KnownColor.ControlText);
			htmlSysColorTable["captiontext"] = Color.FromKnownColor(KnownColor.ActiveCaptionText);
			htmlSysColorTable["graytext"] = Color.FromKnownColor(KnownColor.GrayText);
			htmlSysColorTable["highlight"] = Color.FromKnownColor(KnownColor.Highlight);
			htmlSysColorTable["highlighttext"] = Color.FromKnownColor(KnownColor.HighlightText);
			htmlSysColorTable["inactiveborder"] = Color.FromKnownColor(KnownColor.InactiveBorder);
			htmlSysColorTable["inactivecaption"] = Color.FromKnownColor(KnownColor.InactiveCaption);
			htmlSysColorTable["inactivecaptiontext"] = Color.FromKnownColor(KnownColor.InactiveCaptionText);
			htmlSysColorTable["infobackground"] = Color.FromKnownColor(KnownColor.Info);
			htmlSysColorTable["infotext"] = Color.FromKnownColor(KnownColor.InfoText);
			htmlSysColorTable["menu"] = Color.FromKnownColor(KnownColor.Menu);
			htmlSysColorTable["menutext"] = Color.FromKnownColor(KnownColor.MenuText);
			htmlSysColorTable["scrollbar"] = Color.FromKnownColor(KnownColor.ScrollBar);
			htmlSysColorTable["threeddarkshadow"] = Color.FromKnownColor(KnownColor.ControlDarkDark);
			htmlSysColorTable["threedface"] = Color.FromKnownColor(KnownColor.Control);
			htmlSysColorTable["threedhighlight"] = Color.FromKnownColor(KnownColor.ControlLight);
			htmlSysColorTable["threedlightshadow"] = Color.FromKnownColor(KnownColor.ControlLightLight);
			htmlSysColorTable["window"] = Color.FromKnownColor(KnownColor.Window);
			htmlSysColorTable["windowframe"] = Color.FromKnownColor(KnownColor.WindowFrame);
			htmlSysColorTable["windowtext"] = Color.FromKnownColor(KnownColor.WindowText);
		}
	}

}

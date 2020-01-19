using System.Drawing;

namespace BlazorWebFormsComponents
{
	public class WebColor
	{
		private Color _color = Color.Empty;

		public static readonly WebColor Empty = new WebColor(Color.Empty);
		public static readonly WebColor Transparent = new WebColor(Color.Transparent);
		public static readonly WebColor AliceBlue = new WebColor(Color.AliceBlue);
		public static readonly WebColor AntiqueWhite = new WebColor(Color.AntiqueWhite);
		public static readonly WebColor Aqua = new WebColor(Color.Aqua);
		public static readonly WebColor Aquamarine = new WebColor(Color.Aquamarine);
		public static readonly WebColor Azure = new WebColor(Color.Azure);
		public static readonly WebColor Beige = new WebColor(Color.Beige);
		public static readonly WebColor Bisque = new WebColor(Color.Bisque);
		public static readonly WebColor Black = new WebColor(Color.Black);
		public static readonly WebColor BlanchedAlmond = new WebColor(Color.BlanchedAlmond);
		public static readonly WebColor Blue = new WebColor(Color.Blue);
		public static readonly WebColor BlueViolet = new WebColor(Color.BlueViolet);
		public static readonly WebColor Brown = new WebColor(Color.Brown);
		public static readonly WebColor BurlyWood = new WebColor(Color.BurlyWood);
		public static readonly WebColor CadetBlue = new WebColor(Color.CadetBlue);
		public static readonly WebColor Chartreuse = new WebColor(Color.Chartreuse);
		public static readonly WebColor Chocolate = new WebColor(Color.Chocolate);
		public static readonly WebColor Coral = new WebColor(Color.Coral);
		public static readonly WebColor CornflowerBlue = new WebColor(Color.CornflowerBlue);
		public static readonly WebColor Cornsilk = new WebColor(Color.Cornsilk);
		public static readonly WebColor Crimson = new WebColor(Color.Crimson);
		public static readonly WebColor Cyan = new WebColor(Color.Cyan);
		public static readonly WebColor DarkBlue = new WebColor(Color.DarkBlue);
		public static readonly WebColor DarkCyan = new WebColor(Color.DarkCyan);
		public static readonly WebColor DarkGoldenrod = new WebColor(Color.DarkGoldenrod);
		public static readonly WebColor DarkGray = new WebColor(Color.DarkGray);
		public static readonly WebColor DarkGreen = new WebColor(Color.DarkGreen);
		public static readonly WebColor DarkKhaki = new WebColor(Color.DarkKhaki);
		public static readonly WebColor DarkMagenta = new WebColor(Color.DarkMagenta);
		public static readonly WebColor DarkOliveGreen = new WebColor(Color.DarkOliveGreen);
		public static readonly WebColor DarkOrange = new WebColor(Color.DarkOrange);
		public static readonly WebColor DarkOrchid = new WebColor(Color.DarkOrchid);
		public static readonly WebColor DarkRed = new WebColor(Color.DarkRed);
		public static readonly WebColor DarkSalmon = new WebColor(Color.DarkSalmon);
		public static readonly WebColor DarkSeaGreen = new WebColor(Color.DarkSeaGreen);
		public static readonly WebColor DarkSlateBlue = new WebColor(Color.DarkSlateBlue);
		public static readonly WebColor DarkSlateGray = new WebColor(Color.DarkSlateGray);
		public static readonly WebColor DarkTurquoise = new WebColor(Color.DarkTurquoise);
		public static readonly WebColor DarkViolet = new WebColor(Color.DarkViolet);
		public static readonly WebColor DeepPink = new WebColor(Color.DeepPink);
		public static readonly WebColor DeepSkyBlue = new WebColor(Color.DeepSkyBlue);
		public static readonly WebColor DimGray = new WebColor(Color.DimGray);
		public static readonly WebColor DodgerBlue = new WebColor(Color.DodgerBlue);
		public static readonly WebColor Firebrick = new WebColor(Color.Firebrick);
		public static readonly WebColor FloralWhite = new WebColor(Color.FloralWhite);
		public static readonly WebColor ForestGreen = new WebColor(Color.ForestGreen);
		public static readonly WebColor Fuchsia = new WebColor(Color.Fuchsia);
		public static readonly WebColor Gainsboro = new WebColor(Color.Gainsboro);
		public static readonly WebColor GhostWhite = new WebColor(Color.GhostWhite);
		public static readonly WebColor Gold = new WebColor(Color.Gold);
		public static readonly WebColor Goldenrod = new WebColor(Color.Goldenrod);
		public static readonly WebColor Gray = new WebColor(Color.Gray);
		public static readonly WebColor Green = new WebColor(Color.Green);
		public static readonly WebColor GreenYellow = new WebColor(Color.GreenYellow);
		public static readonly WebColor Honeydew = new WebColor(Color.Honeydew);
		public static readonly WebColor HotPink = new WebColor(Color.HotPink);
		public static readonly WebColor IndianRed = new WebColor(Color.IndianRed);
		public static readonly WebColor Indigo = new WebColor(Color.Indigo);
		public static readonly WebColor Ivory = new WebColor(Color.Ivory);
		public static readonly WebColor Khaki = new WebColor(Color.Khaki);
		public static readonly WebColor Lavender = new WebColor(Color.Lavender);
		public static readonly WebColor LavenderBlush = new WebColor(Color.LavenderBlush);
		public static readonly WebColor LawnGreen = new WebColor(Color.LawnGreen);
		public static readonly WebColor LemonChiffon = new WebColor(Color.LemonChiffon);
		public static readonly WebColor LightBlue = new WebColor(Color.LightBlue);
		public static readonly WebColor LightCoral = new WebColor(Color.LightCoral);
		public static readonly WebColor LightCyan = new WebColor(Color.LightCyan);
		public static readonly WebColor LightGoldenrodYellow = new WebColor(Color.LightGoldenrodYellow);
		public static readonly WebColor LightGreen = new WebColor(Color.LightGreen);
		public static readonly WebColor LightGray = new WebColor(Color.LightGray);
		public static readonly WebColor LightPink = new WebColor(Color.LightPink);
		public static readonly WebColor LightSalmon = new WebColor(Color.LightSalmon);
		public static readonly WebColor LightSeaGreen = new WebColor(Color.LightSeaGreen);
		public static readonly WebColor LightSkyBlue = new WebColor(Color.LightSkyBlue);
		public static readonly WebColor LightSlateGray = new WebColor(Color.LightSlateGray);
		public static readonly WebColor LightSteelBlue = new WebColor(Color.LightSteelBlue);
		public static readonly WebColor LightYellow = new WebColor(Color.LightYellow);
		public static readonly WebColor Lime = new WebColor(Color.Lime);
		public static readonly WebColor LimeGreen = new WebColor(Color.LimeGreen);
		public static readonly WebColor Linen = new WebColor(Color.Linen);
		public static readonly WebColor Magenta = new WebColor(Color.Magenta);
		public static readonly WebColor Maroon = new WebColor(Color.Maroon);
		public static readonly WebColor MediumAquamarine = new WebColor(Color.MediumAquamarine);
		public static readonly WebColor MediumBlue = new WebColor(Color.MediumBlue);
		public static readonly WebColor MediumOrchid = new WebColor(Color.MediumOrchid);
		public static readonly WebColor MediumPurple = new WebColor(Color.MediumPurple);
		public static readonly WebColor MediumSeaGreen = new WebColor(Color.MediumSeaGreen);
		public static readonly WebColor MediumSlateBlue = new WebColor(Color.MediumSlateBlue);
		public static readonly WebColor MediumSpringGreen = new WebColor(Color.MediumSpringGreen);
		public static readonly WebColor MediumTurquoise = new WebColor(Color.MediumTurquoise);
		public static readonly WebColor MediumVioletRed = new WebColor(Color.MediumVioletRed);
		public static readonly WebColor MidnightBlue = new WebColor(Color.MidnightBlue);
		public static readonly WebColor MintCream = new WebColor(Color.MintCream);
		public static readonly WebColor MistyRose = new WebColor(Color.MistyRose);
		public static readonly WebColor Moccasin = new WebColor(Color.Moccasin);
		public static readonly WebColor NavajoWhite = new WebColor(Color.NavajoWhite);
		public static readonly WebColor Navy = new WebColor(Color.Navy);
		public static readonly WebColor OldLace = new WebColor(Color.OldLace);
		public static readonly WebColor Olive = new WebColor(Color.Olive);
		public static readonly WebColor OliveDrab = new WebColor(Color.OliveDrab);
		public static readonly WebColor Orange = new WebColor(Color.Orange);
		public static readonly WebColor OrangeRed = new WebColor(Color.OrangeRed);
		public static readonly WebColor Orchid = new WebColor(Color.Orchid);
		public static readonly WebColor PaleGoldenrod = new WebColor(Color.PaleGoldenrod);
		public static readonly WebColor PaleGreen = new WebColor(Color.PaleGreen);
		public static readonly WebColor PaleTurquoise = new WebColor(Color.PaleTurquoise);
		public static readonly WebColor PaleVioletRed = new WebColor(Color.PaleVioletRed);
		public static readonly WebColor PapayaWhip = new WebColor(Color.PapayaWhip);
		public static readonly WebColor PeachPuff = new WebColor(Color.PeachPuff);
		public static readonly WebColor Peru = new WebColor(Color.Peru);
		public static readonly WebColor Pink = new WebColor(Color.Pink);
		public static readonly WebColor Plum = new WebColor(Color.Plum);
		public static readonly WebColor PowderBlue = new WebColor(Color.PowderBlue);
		public static readonly WebColor Purple = new WebColor(Color.Purple);
		public static readonly WebColor Red = new WebColor(Color.Red);
		public static readonly WebColor RosyBrown = new WebColor(Color.RosyBrown);
		public static readonly WebColor RoyalBlue = new WebColor(Color.RoyalBlue);
		public static readonly WebColor SaddleBrown = new WebColor(Color.SaddleBrown);
		public static readonly WebColor Salmon = new WebColor(Color.Salmon);
		public static readonly WebColor SandyBrown = new WebColor(Color.SandyBrown);
		public static readonly WebColor SeaGreen = new WebColor(Color.SeaGreen);
		public static readonly WebColor SeaShell = new WebColor(Color.SeaShell);
		public static readonly WebColor Sienna = new WebColor(Color.Sienna);
		public static readonly WebColor Silver = new WebColor(Color.Silver);
		public static readonly WebColor SkyBlue = new WebColor(Color.SkyBlue);
		public static readonly WebColor SlateBlue = new WebColor(Color.SlateBlue);
		public static readonly WebColor SlateGray = new WebColor(Color.SlateGray);
		public static readonly WebColor Snow = new WebColor(Color.Snow);
		public static readonly WebColor SpringGreen = new WebColor(Color.SpringGreen);
		public static readonly WebColor SteelBlue = new WebColor(Color.SteelBlue);
		public static readonly WebColor Tan = new WebColor(Color.Tan);
		public static readonly WebColor Teal = new WebColor(Color.Teal);
		public static readonly WebColor Thistle = new WebColor(Color.Thistle);
		public static readonly WebColor Tomato = new WebColor(Color.Tomato);
		public static readonly WebColor Turquoise = new WebColor(Color.Turquoise);
		public static readonly WebColor Violet = new WebColor(Color.Violet);
		public static readonly WebColor Wheat = new WebColor(Color.Wheat);
		public static readonly WebColor White = new WebColor(Color.White);
		public static readonly WebColor WhiteSmoke = new WebColor(Color.WhiteSmoke);
		public static readonly WebColor Yellow = new WebColor(Color.Yellow);
		public static readonly WebColor YellowGreen = new WebColor(Color.YellowGreen);

		public bool IsEmpty { get { return _color.IsEmpty; } }

		public WebColor(string colorString)
		{
			if (string.IsNullOrEmpty(colorString))
				_color = Color.Empty;
			else if (colorString.StartsWith("#"))
				_color = ColorTranslator.FromHtml(colorString);
			else
				_color = Color.FromName(colorString);
		}

		public WebColor(Color color)
		{
			_color = color;
		}

		public static implicit operator WebColor(string colorString) => new WebColor(colorString);

		public Color ToColor() => _color;

		public static implicit operator WebColor(Color c)
		{
			return new WebColor(c);
		}

		public static implicit operator Color(WebColor c)
		{
			return c._color;
		}
	}
}

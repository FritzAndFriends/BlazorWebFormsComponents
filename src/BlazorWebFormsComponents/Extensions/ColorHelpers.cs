using System;
using System.Collections.Generic;
using System.Text;

namespace System.Drawing
{
	public static class ColorHelpers
	{

		public static Color GetColorFromHtml(this object obj)
		{

			if (obj is Color) return (Color)obj;
			if (!(obj is string)) return Color.Empty;

			var theString = obj.ToString();
			if (theString.Contains("#")) return ColorTranslator.FromHtml(theString);
			return Color.FromName(theString);

		}

	}
}

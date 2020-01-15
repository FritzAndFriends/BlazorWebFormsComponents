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

namespace BlazorWebFormsComponents
{
    public static class ColorHelpers
    {
        public static WebColor GetWebColorFromHtml(this object obj)
		{
			if (obj is WebColor) return (WebColor)obj;
			if (!(obj is string)) return WebColor.Empty;

			var theString = obj.ToString();
			return new WebColor(theString);
		}
    }
}
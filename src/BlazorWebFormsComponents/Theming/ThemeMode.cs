namespace BlazorWebFormsComponents.Theming
{
	/// <summary>
	/// Controls how theme skins interact with explicit property values.
	/// Matches ASP.NET Web Forms Page.Theme vs Page.StyleSheetTheme behavior.
	/// </summary>
	public enum ThemeMode
	{
		/// <summary>
		/// Theme sets default values. Explicit component property values take precedence.
		/// Equivalent to Web Forms Page.StyleSheetTheme.
		/// </summary>
		StyleSheetTheme = 0,

		/// <summary>
		/// Theme overrides all property values, even explicitly set ones.
		/// Equivalent to Web Forms Page.Theme. This matches the most common Web Forms usage.
		/// </summary>
		Theme = 1
	}
}

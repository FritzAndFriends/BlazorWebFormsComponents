namespace BlazorWebFormsComponents
{

	/// <summary>
	/// A collection of the names of the JavaScript scripts available in
	/// the project so that we do not pass magic strings around
	/// </summary>
	internal static class JsScripts
	{

		/// <summary>
		/// Scripts that run in the Page JavaScript object
		/// </summary>
		internal static class Page
		{

			private const string _Base = "bwfc.Page.";

			/// <summary>
			/// Script to be run after the page is rendered
			/// </summary>
			public static readonly string OnAfterRender = string.Concat(_Base, "OnAfterRender");

		}

	}
}

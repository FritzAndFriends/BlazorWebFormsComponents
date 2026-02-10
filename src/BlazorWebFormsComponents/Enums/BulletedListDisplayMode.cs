namespace BlazorWebFormsComponents.Enums
{
	/// <summary>
	/// Specifies the display mode of a BulletedList control.
	/// </summary>
	public enum BulletedListDisplayMode
	{
		/// <summary>
		/// The list items are displayed as plain text.
		/// </summary>
		Text = 0,

		/// <summary>
		/// The list items are displayed as hyperlinks that navigate to a URL.
		/// </summary>
		HyperLink = 1,

		/// <summary>
		/// The list items are displayed as link buttons that post back to the server.
		/// </summary>
		LinkButton = 2
	}
}

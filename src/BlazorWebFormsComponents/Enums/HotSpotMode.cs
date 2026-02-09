namespace BlazorWebFormsComponents.Enums
{
	/// <summary>
	/// Specifies the behaviors of a HotSpot object in an ImageMap control when the HotSpot is clicked.
	/// </summary>
	public enum HotSpotMode
	{
		/// <summary>
		/// The HotSpot does not have any behavior.
		/// </summary>
		Inactive,

		/// <summary>
		/// The HotSpot navigates to a URL.
		/// </summary>
		Navigate,

		/// <summary>
		/// The HotSpot generates a postback to the server.
		/// </summary>
		PostBack
	}
}

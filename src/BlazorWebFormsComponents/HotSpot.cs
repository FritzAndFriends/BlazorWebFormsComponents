using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Implements the basic functionality common to all hot spot shapes.
	/// </summary>
	public abstract class HotSpot
	{
		/// <summary>
		/// Gets or sets the alternate text to display for a HotSpot object in an ImageMap control when the image is unavailable or renders to a browser that does not support images.
		/// </summary>
		public string AlternateText { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the behavior of a HotSpot object in an ImageMap control when the HotSpot is clicked.
		/// </summary>
		public HotSpotMode HotSpotMode { get; set; } = HotSpotMode.Navigate;

		/// <summary>
		/// Gets or sets the URL to navigate to when a HotSpot object is clicked.
		/// </summary>
		public string NavigateUrl { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the name of the HotSpot object to pass in the event data when the HotSpot is clicked.
		/// </summary>
		public string PostBackValue { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the target window or frame in which to display the Web page content linked to when the HotSpot object is clicked.
		/// </summary>
		public string Target { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the tab index of the HotSpot object.
		/// </summary>
		public short TabIndex { get; set; }

		/// <summary>
		/// Gets or sets the AccessKey that allows you to quickly navigate to the HotSpot object.
		/// </summary>
		public string AccessKey { get; set; } = string.Empty;

		/// <summary>
		/// Gets the shape type for this hot spot.
		/// </summary>
		/// <returns>The shape type (rect, circle, or poly)</returns>
		public abstract string GetShapeType();

		/// <summary>
		/// Gets the coordinates for this hot spot as a comma-separated string.
		/// </summary>
		/// <returns>The coordinates string</returns>
		public abstract string GetCoordinates();
	}
}

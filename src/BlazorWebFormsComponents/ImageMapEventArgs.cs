using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the Click event of an ImageMap control.
	/// </summary>
	public class ImageMapEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the PostBackValue associated with the HotSpot object in the ImageMap control that was clicked.
		/// </summary>
		public string PostBackValue { get; }

		/// <summary>
		/// Initializes a new instance of the ImageMapEventArgs class.
		/// </summary>
		/// <param name="postBackValue">The string assigned to the PostBackValue property of the HotSpot object that was clicked.</param>
		public ImageMapEventArgs(string postBackValue)
		{
			PostBackValue = postBackValue;
		}
	}
}

using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the <see cref="SiteMapPath.ItemCreated"/> and
	/// <see cref="SiteMapPath.ItemDataBound"/> events.
	/// </summary>
	public class SiteMapNodeItemEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the <see cref="SiteMapNode"/> associated with the event.
		/// </summary>
		public SiteMapNode Item { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SiteMapNodeItemEventArgs"/> class.
		/// </summary>
		/// <param name="item">The site map node associated with the event.</param>
		public SiteMapNodeItemEventArgs(SiteMapNode item)
		{
			Item = item;
		}
	}
}

using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for Menu item events.
	/// </summary>
	public class MenuEventArgs : EventArgs
	{
		public MenuEventArgs(MenuItem item)
		{
			Item = item;
		}

		/// <summary>
		/// Gets the menu item associated with the event.
		/// </summary>
		public MenuItem Item { get; }

		/// <summary>
		/// The component that raised this event.
		/// </summary>
		public object Sender { get; set; }
	}
}

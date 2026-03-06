using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemCreated and ItemDataBound events of the Repeater control.
	/// </summary>
	public class RepeaterItemEventArgs : EventArgs
	{
		public RepeaterItemEventArgs(object item)
		{
			Item = item;
		}

		/// <summary>
		/// Gets the data item associated with the Repeater item.
		/// </summary>
		public object Item { get; }

		/// <summary>
		/// The component that raised this event.
		/// </summary>
		public object Sender { get; set; }
	}
}

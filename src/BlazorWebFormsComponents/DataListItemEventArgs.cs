using System;

namespace BlazorWebFormsComponents
{
	public class DataListItemEventArgs : EventArgs
	{
		public DataListItemEventArgs(object item)
		{
			Item = item;
		}

		public object Item { get; }

		/// <summary>
		/// The component that raised this event
		/// </summary>
		public object Sender { get; set; }
	}
}

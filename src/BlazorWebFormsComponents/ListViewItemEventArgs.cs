using System;

namespace BlazorWebFormsComponents
{
	public class ListViewItemEventArgs : EventArgs
	{
		public ListViewItemEventArgs(ListViewItem item)
		{
			Item = item;
		}

		public ListViewItem Item { get; }

		/// <summary>
		/// The component that raised this event
		/// </summary>
		public object Sender { get; set; }
	}
}

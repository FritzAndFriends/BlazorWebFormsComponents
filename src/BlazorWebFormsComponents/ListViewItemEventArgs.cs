using System;

namespace BlazorWebFormsComponents
{
	public class ListViewItemEventArgs : EventArgs
	{
		public ListViewItemEventArgs(object item)
		{
			Item = item;
		}

		public object Item { get; }
	}
}

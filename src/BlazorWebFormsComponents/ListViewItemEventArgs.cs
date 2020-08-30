using System;

namespace BlazorWebFormsComponents
{

	public class ListViewItemEventArgs : EventArgs
	{

		public ListViewItemEventArgs(object item)
		{
			this.Item = item;
		}

		object Item { get; set; }

	}

}

using System;
using System.Collections.Specialized;

namespace BlazorWebFormsComponents
{
	public class GridViewDeleteEventArgs : EventArgs
	{
		public int RowIndex { get; }
		public bool Cancel { get; set; }
		public IOrderedDictionary Keys { get; set; } = new OrderedDictionary();

		public GridViewDeleteEventArgs(int rowIndex)
		{
			RowIndex = rowIndex;
		}

		/// <summary>
		/// The component that raised this event.
		/// </summary>
		public object Sender { get; set; }
	}
}

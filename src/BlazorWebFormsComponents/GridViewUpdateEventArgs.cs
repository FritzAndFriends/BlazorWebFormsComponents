using System;
using System.Collections.Specialized;

namespace BlazorWebFormsComponents
{
	public class GridViewUpdateEventArgs : EventArgs
	{
		public int RowIndex { get; }
		public bool Cancel { get; set; }
		public IOrderedDictionary Keys { get; set; } = new OrderedDictionary();
		public IOrderedDictionary NewValues { get; set; } = new OrderedDictionary();
		public IOrderedDictionary OldValues { get; set; } = new OrderedDictionary();

		public GridViewUpdateEventArgs(int rowIndex)
		{
			RowIndex = rowIndex;
		}

		/// <summary>
		/// The component that raised this event.
		/// </summary>
		public object Sender { get; set; }
	}
}

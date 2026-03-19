using System;

namespace BlazorWebFormsComponents
{
	public class GridViewUpdateEventArgs : EventArgs
	{
		public int RowIndex { get; }
		public bool Cancel { get; set; }

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

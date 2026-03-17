using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the RowDataBound and RowCreated events of the GridView control.
	/// </summary>
	public class GridViewRowEventArgs : EventArgs
	{
		public GridViewRowEventArgs(object row, int rowIndex)
		{
			Row = row;
			RowIndex = rowIndex;
		}

		/// <summary>
		/// Gets the data item for the row that raised the event.
		/// </summary>
		public object Row { get; }

		/// <summary>
		/// Gets the index of the row that raised the event.
		/// </summary>
		public int RowIndex { get; }

		/// <summary>
		/// The component that raised this event.
		/// </summary>
		public object Sender { get; set; }
	}
}

using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemCreated and ItemDataBound events of the DataGrid control.
	/// </summary>
	public class DataGridItemEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the item associated with the event.
		/// </summary>
		public object Item { get; }

		/// <summary>
		/// Creates a new instance of DataGridItemEventArgs.
		/// </summary>
		public DataGridItemEventArgs(object item)
		{
			Item = item;
		}
	}
}

using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemInserting event of the ListView control.
	/// </summary>
	public class ListViewInsertEventArgs : EventArgs
	{
		public ListViewInsertEventArgs()
		{
		}

		/// <summary>
		/// Gets or sets a value indicating whether the event should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }

		/// <summary>
		/// Gets the item to be inserted.
		/// </summary>
		public object Item { get; set; }
	}
}

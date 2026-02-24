using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemDeleting event of the ListView control.
	/// </summary>
	public class ListViewDeleteEventArgs : EventArgs
	{
		public ListViewDeleteEventArgs(int itemIndex)
		{
			ItemIndex = itemIndex;
		}

		/// <summary>
		/// Gets the index of the item being deleted.
		/// </summary>
		public int ItemIndex { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the event should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }
	}
}

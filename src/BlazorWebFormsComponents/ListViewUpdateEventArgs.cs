using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemUpdating event of the ListView control.
	/// </summary>
	public class ListViewUpdateEventArgs : EventArgs
	{
		public ListViewUpdateEventArgs(int itemIndex)
		{
			ItemIndex = itemIndex;
		}

		/// <summary>
		/// Gets the index of the item being updated.
		/// </summary>
		public int ItemIndex { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the event should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }
	}
}

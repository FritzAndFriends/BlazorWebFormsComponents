using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemEditing event of the ListView control.
	/// </summary>
	public class ListViewEditEventArgs : EventArgs
	{
		public ListViewEditEventArgs(int newEditIndex)
		{
			NewEditIndex = newEditIndex;
		}

		/// <summary>
		/// Gets or sets the index of the item being edited.
		/// </summary>
		public int NewEditIndex { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the event should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }
	}
}

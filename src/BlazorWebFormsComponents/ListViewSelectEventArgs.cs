using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the SelectedIndexChanging event of the ListView control.
	/// </summary>
	public class ListViewSelectEventArgs : EventArgs
	{
		public ListViewSelectEventArgs(int newSelectedIndex)
		{
			NewSelectedIndex = newSelectedIndex;
		}

		/// <summary>
		/// Gets or sets the index of the new item to select in the ListView control.
		/// </summary>
		public int NewSelectedIndex { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the selection should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }
	}
}

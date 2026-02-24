using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the SelectedIndexChanging event of a GridView.
	/// </summary>
	public class GridViewSelectEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the index of the new row to select.
		/// </summary>
		public int NewSelectedIndex { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the selection should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }

		public GridViewSelectEventArgs(int newSelectedIndex)
		{
			NewSelectedIndex = newSelectedIndex;
		}
	}
}

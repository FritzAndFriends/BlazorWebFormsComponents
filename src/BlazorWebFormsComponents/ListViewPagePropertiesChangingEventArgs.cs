using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the PagePropertiesChanging event of the ListView control.
	/// </summary>
	public class ListViewPagePropertiesChangingEventArgs : EventArgs
	{
		public ListViewPagePropertiesChangingEventArgs(int startRowIndex, int maximumRows)
		{
			StartRowIndex = startRowIndex;
			MaximumRows = maximumRows;
		}

		/// <summary>
		/// Gets the index of the first item of the page.
		/// </summary>
		public int StartRowIndex { get; }

		/// <summary>
		/// Gets the maximum number of items to display on each page.
		/// </summary>
		public int MaximumRows { get; }
	}
}

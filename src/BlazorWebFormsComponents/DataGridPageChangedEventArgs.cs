using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the PageIndexChanged event of the DataGrid control.
	/// </summary>
	public class DataGridPageChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the index of the new page.
		/// </summary>
		public int NewPageIndex { get; set; }

		/// <summary>
		/// Creates a new instance of DataGridPageChangedEventArgs.
		/// </summary>
		public DataGridPageChangedEventArgs(int newPageIndex)
		{
			NewPageIndex = newPageIndex;
		}
	}
}

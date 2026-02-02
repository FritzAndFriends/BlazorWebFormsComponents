using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the PageIndexChanging and PageIndexChanged events of pageable controls.
	/// </summary>
	public class PageChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the index of the new page.
		/// </summary>
		public int NewPageIndex { get; set; }

		/// <summary>
		/// Gets the index of the previous page.
		/// </summary>
		public int OldPageIndex { get; }

		/// <summary>
		/// Gets the total number of pages.
		/// </summary>
		public int TotalPages { get; }

		/// <summary>
		/// Gets the index of the first row on the new page.
		/// </summary>
		public int StartRowIndex { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the event should be canceled.
		/// </summary>
		public bool Cancel { get; set; }

		/// <summary>
		/// Creates a new instance of PageChangedEventArgs.
		/// </summary>
		public PageChangedEventArgs(int newPageIndex, int oldPageIndex, int totalPages, int startRowIndex)
		{
			NewPageIndex = newPageIndex;
			OldPageIndex = oldPageIndex;
			TotalPages = totalPages;
			StartRowIndex = startRowIndex;
		}
	}
}

using BlazorWebFormsComponents.Enums;
using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the Sorting event of the ListView control.
	/// </summary>
	public class ListViewSortEventArgs : EventArgs
	{
		public ListViewSortEventArgs(string sortExpression, SortDirection sortDirection)
		{
			SortExpression = sortExpression;
			SortDirection = sortDirection;
		}

		/// <summary>
		/// Gets or sets the expression used to sort the items in the ListView control.
		/// </summary>
		public string SortExpression { get; set; }

		/// <summary>
		/// Gets or sets the direction in which to sort the ListView control.
		/// </summary>
		public SortDirection SortDirection { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the sort operation should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }
	}
}

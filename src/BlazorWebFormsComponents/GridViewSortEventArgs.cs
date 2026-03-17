using BlazorWebFormsComponents.Enums;
using System;

namespace BlazorWebFormsComponents
{
	public class GridViewSortEventArgs : EventArgs
	{
		public string SortExpression { get; set; }
		public SortDirection SortDirection { get; set; }
		public bool Cancel { get; set; }

		public GridViewSortEventArgs(string sortExpression, SortDirection sortDirection)
		{
			SortExpression = sortExpression;
			SortDirection = sortDirection;
		}

		/// <summary>
		/// The component that raised this event.
		/// </summary>
		public object Sender { get; set; }
	}
}

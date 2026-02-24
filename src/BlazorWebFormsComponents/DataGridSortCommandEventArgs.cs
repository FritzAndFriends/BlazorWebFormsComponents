using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the SortCommand event of the DataGrid control.
	/// </summary>
	public class DataGridSortCommandEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the sort expression associated with the column.
		/// </summary>
		public string SortExpression { get; }

		/// <summary>
		/// Gets the source of the command.
		/// </summary>
		public object CommandSource { get; }

		/// <summary>
		/// Creates a new instance of DataGridSortCommandEventArgs.
		/// </summary>
		public DataGridSortCommandEventArgs(string sortExpression, object commandSource)
		{
			SortExpression = sortExpression;
			CommandSource = commandSource;
		}
	}
}

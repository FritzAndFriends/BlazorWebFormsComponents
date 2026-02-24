using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemDeleted event of the ListView control.
	/// </summary>
	public class ListViewDeletedEventArgs : EventArgs
	{
		public ListViewDeletedEventArgs(int affectedRows, Exception exception)
		{
			AffectedRows = affectedRows;
			Exception = exception;
		}

		/// <summary>
		/// Gets or sets the number of rows affected by the delete operation.
		/// </summary>
		public int AffectedRows { get; set; }

		/// <summary>
		/// Gets the exception, if any, that was raised during the delete operation.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the exception was handled.
		/// </summary>
		public bool ExceptionHandled { get; set; }
	}
}

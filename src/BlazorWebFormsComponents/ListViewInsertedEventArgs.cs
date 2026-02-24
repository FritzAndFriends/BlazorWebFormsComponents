using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemInserted event of the ListView control.
	/// </summary>
	public class ListViewInsertedEventArgs : EventArgs
	{
		public ListViewInsertedEventArgs(int affectedRows, Exception exception)
		{
			AffectedRows = affectedRows;
			Exception = exception;
		}

		/// <summary>
		/// Gets or sets the number of rows affected by the insert operation.
		/// </summary>
		public int AffectedRows { get; set; }

		/// <summary>
		/// Gets the exception, if any, that was raised during the insert operation.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the exception was handled.
		/// </summary>
		public bool ExceptionHandled { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the inserted values were cleared.
		/// </summary>
		public bool KeepInsertedValues { get; set; }
	}
}

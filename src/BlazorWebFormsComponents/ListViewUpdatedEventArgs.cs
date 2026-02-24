using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemUpdated event of the ListView control.
	/// </summary>
	public class ListViewUpdatedEventArgs : EventArgs
	{
		public ListViewUpdatedEventArgs(int affectedRows, Exception exception)
		{
			AffectedRows = affectedRows;
			Exception = exception;
		}

		/// <summary>
		/// Gets or sets the number of rows affected by the update operation.
		/// </summary>
		public int AffectedRows { get; set; }

		/// <summary>
		/// Gets the exception, if any, that was raised during the update operation.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the exception was handled.
		/// </summary>
		public bool ExceptionHandled { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the row should stay in edit mode.
		/// </summary>
		public bool KeepInEditMode { get; set; }
	}
}

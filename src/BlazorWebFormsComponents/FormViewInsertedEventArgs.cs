using System;
using System.Collections.Specialized;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemInserted (past tense) event of the FormView control.
	/// Distinct from FormViewInsertEventArgs which is used for the ItemInserting event.
	/// </summary>
	public class FormViewInsertedEventArgs : EventArgs
	{
		public FormViewInsertedEventArgs(int affectedRows, Exception exception = null)
		{
			AffectedRows = affectedRows;
			Exception = exception;
		}

		/// <summary>
		/// Gets the number of rows affected by the insert operation.
		/// </summary>
		public int AffectedRows { get; set; }

		/// <summary>
		/// Gets the exception, if any, that was raised during the insert operation.
		/// </summary>
		public Exception Exception { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the exception was handled.
		/// </summary>
		public bool ExceptionHandled { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the FormView should remain in insert mode.
		/// </summary>
		public bool KeepInInsertMode { get; set; }

		/// <summary>
		/// Gets or sets the values of the inserted record.
		/// </summary>
		public IOrderedDictionary Values { get; set; }

		/// <summary>
		/// The component that raised this event.
		/// </summary>
		public object Sender { get; set; }
	}
}

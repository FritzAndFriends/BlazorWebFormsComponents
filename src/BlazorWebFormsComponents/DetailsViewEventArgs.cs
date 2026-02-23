using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemCommand event of the DetailsView control.
	/// </summary>
	public class DetailsViewCommandEventArgs : CommandEventArgs
	{
		public DetailsViewCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
			: base(originalArgs)
		{
			CommandSource = commandSource;
		}

		/// <summary>
		/// Gets the source of the command.
		/// </summary>
		public object CommandSource { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the event has been handled.
		/// </summary>
		public bool Handled { get; set; }
	}

	/// <summary>
	/// Provides data for the ItemDeleting event of the DetailsView control.
	/// </summary>
	public class DetailsViewDeleteEventArgs : EventArgs
	{
		public DetailsViewDeleteEventArgs(int rowIndex)
		{
			RowIndex = rowIndex;
		}

		/// <summary>
		/// Gets the index of the row being deleted.
		/// </summary>
		public int RowIndex { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the event should be canceled.
		/// </summary>
		public bool Cancel { get; set; }
	}

	/// <summary>
	/// Provides data for the ItemDeleted event of the DetailsView control.
	/// </summary>
	public class DetailsViewDeletedEventArgs : EventArgs
	{
		public DetailsViewDeletedEventArgs(int affectedRows, Exception exception)
		{
			AffectedRows = affectedRows;
			Exception = exception;
		}

		/// <summary>
		/// Gets the number of rows affected by the delete operation.
		/// </summary>
		public int AffectedRows { get; }

		/// <summary>
		/// Gets the exception, if any, that was raised during the delete operation.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the exception was handled.
		/// </summary>
		public bool ExceptionHandled { get; set; }
	}

	/// <summary>
	/// Provides data for the ItemInserting event of the DetailsView control.
	/// </summary>
	public class DetailsViewInsertEventArgs : EventArgs
	{
		public DetailsViewInsertEventArgs(object commandArgument)
		{
			CommandArgument = commandArgument;
		}

		/// <summary>
		/// Gets the command argument for the insert operation.
		/// </summary>
		public object CommandArgument { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the event should be canceled.
		/// </summary>
		public bool Cancel { get; set; }
	}

	/// <summary>
	/// Provides data for the ItemInserted event of the DetailsView control.
	/// </summary>
	public class DetailsViewInsertedEventArgs : EventArgs
	{
		public DetailsViewInsertedEventArgs(int affectedRows, Exception exception)
		{
			AffectedRows = affectedRows;
			Exception = exception;
		}

		/// <summary>
		/// Gets the number of rows affected by the insert operation.
		/// </summary>
		public int AffectedRows { get; }

		/// <summary>
		/// Gets the exception, if any, that was raised during the insert operation.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the exception was handled.
		/// </summary>
		public bool ExceptionHandled { get; set; }
	}

	/// <summary>
	/// Provides data for the ItemUpdating event of the DetailsView control.
	/// </summary>
	public class DetailsViewUpdateEventArgs : EventArgs
	{
		public DetailsViewUpdateEventArgs(object commandArgument)
		{
			CommandArgument = commandArgument;
		}

		/// <summary>
		/// Gets the command argument for the update operation.
		/// </summary>
		public object CommandArgument { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the event should be canceled.
		/// </summary>
		public bool Cancel { get; set; }
	}

	/// <summary>
	/// Provides data for the ItemUpdated event of the DetailsView control.
	/// </summary>
	public class DetailsViewUpdatedEventArgs : EventArgs
	{
		public DetailsViewUpdatedEventArgs(int affectedRows, Exception exception)
		{
			AffectedRows = affectedRows;
			Exception = exception;
		}

		/// <summary>
		/// Gets the number of rows affected by the update operation.
		/// </summary>
		public int AffectedRows { get; }

		/// <summary>
		/// Gets the exception, if any, that was raised during the update operation.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the exception was handled.
		/// </summary>
		public bool ExceptionHandled { get; set; }
	}

	/// <summary>
	/// Provides data for the ModeChanging event of the DetailsView control.
	/// </summary>
	public class DetailsViewModeEventArgs : EventArgs
	{
		public DetailsViewModeEventArgs(Enums.DetailsViewMode mode, bool cancelingEdit)
		{
			NewMode = mode;
			CancelingEdit = cancelingEdit;
		}

		/// <summary>
		/// Gets or sets the new mode for the DetailsView control.
		/// </summary>
		public Enums.DetailsViewMode NewMode { get; set; }

		/// <summary>
		/// Gets a value indicating whether the mode change is a result of canceling an edit operation.
		/// </summary>
		public bool CancelingEdit { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the event should be canceled.
		/// </summary>
		public bool Cancel { get; set; }
	}
}

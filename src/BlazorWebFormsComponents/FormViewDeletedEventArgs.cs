using System;
using System.Collections.Specialized;

namespace BlazorWebFormsComponents
{
	public class FormViewDeletedEventArgs : EventArgs {

		public FormViewDeletedEventArgs(int affectedRows, Exception ex)
		{
			this.AffectedRows = affectedRows;
			this.Exception = ex;
		}

		public int AffectedRows { get; internal set; }

		public Exception Exception { get; }

		public bool ExceptionHandled { get; set; }

		public IOrderedDictionary Keys { get; internal set; }

		public IOrderedDictionary Values { get; internal set; }

		/// <summary>
		/// The component that raised this event
		/// </summary>
		public object Sender { get; set; }


	}

}

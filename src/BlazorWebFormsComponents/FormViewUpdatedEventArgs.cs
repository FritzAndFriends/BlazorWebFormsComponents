using System;
using System.Collections.Specialized;

namespace BlazorWebFormsComponents {

	public class FormViewUpdatedEventArgs : EventArgs {

		public FormViewUpdatedEventArgs(int affectedRows, Exception e = null)
		{

			this.AffectedRows = affectedRows;
			this.Exception = e;

		}

		public int AffectedRows { get; set; }

		public Exception Exception { get; set; }

		public bool ExceptionHandled { get; set; }

		public bool KeepInEditMode { get; set; }

		public IOrderedDictionary Keys { get; set; }

		public IOrderedDictionary OldValues { get; set; }

		public IOrderedDictionary NewValues { get; set; }


	}

}

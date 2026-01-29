using System;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BlazorWebFormsComponents
{
	public class FormViewDeleteEventArgs : CancelEventArgs {

		public FormViewDeleteEventArgs(int rowIndex)
		{
			this.RowIndex = rowIndex;
		}

		public IOrderedDictionary Keys { get; internal set; }

		public int RowIndex { get; }

		public IOrderedDictionary Values { get; internal set; }

		/// <summary>
		/// The component that raised this event
		/// </summary>
		public object Sender { get; set; }

	}

}

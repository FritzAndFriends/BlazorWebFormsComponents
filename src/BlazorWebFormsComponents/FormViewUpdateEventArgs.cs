using System;
using System.Collections.Specialized;


namespace BlazorWebFormsComponents
{

	/// <summary>
	/// Provides data for the ItemUpdating event.
	/// </summary>
	public class FormViewUpdateEventArgs : EventArgs
	{

		public FormViewUpdateEventArgs(object commandArgument)
		{

			this.CommandArgument = commandArgument;

		}

		public bool Cancel { get; set; }

		public object CommandArgument { get; set; }

		public IOrderedDictionary Keys { get; set; }

		public IOrderedDictionary OldValues { get; set; }

		public IOrderedDictionary NewValues { get; set; }


	}

}

using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemCommand event of the ListView control.
	/// </summary>
	public class ListViewCommandEventArgs : CommandEventArgs
	{
		public ListViewCommandEventArgs(string commandName, object commandArgument)
			: base(commandName, commandArgument)
		{
		}

		public ListViewCommandEventArgs(CommandEventArgs args)
			: base(args)
		{
		}

		/// <summary>
		/// Gets the source item in which the command was raised.
		/// </summary>
		public object Item { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the event was handled.
		/// </summary>
		public bool Handled { get; set; }
	}
}

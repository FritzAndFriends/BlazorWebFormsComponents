using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemCommand event of the Repeater control.
	/// </summary>
	public class RepeaterCommandEventArgs : EventArgs
	{
		public RepeaterCommandEventArgs(string commandName, object commandArgument, object item)
		{
			CommandName = commandName;
			CommandArgument = commandArgument;
			Item = item;
		}

		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public string CommandName { get; }

		/// <summary>
		/// Gets the argument for the command.
		/// </summary>
		public object CommandArgument { get; }

		/// <summary>
		/// Gets the data item associated with the Repeater item that raised the event.
		/// </summary>
		public object Item { get; }

		/// <summary>
		/// The component that raised this event.
		/// </summary>
		public object Sender { get; set; }
	}
}

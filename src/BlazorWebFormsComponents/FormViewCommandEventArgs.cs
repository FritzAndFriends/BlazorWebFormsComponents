using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemCommand event of the FormView control.
	/// </summary>
	public class FormViewCommandEventArgs : CommandEventArgs
	{
		public FormViewCommandEventArgs(string commandName, object commandArgument)
			: base(commandName, commandArgument)
		{
		}

		public FormViewCommandEventArgs(CommandEventArgs args)
			: base(args)
		{
		}
	}
}

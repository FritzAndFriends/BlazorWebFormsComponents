using System;

namespace BlazorWebFormsComponents
{

	public class CommandEventArgs : EventArgs
	{

		public CommandEventArgs(string commandName, object commandArgument)
		{

			CommandName = commandName;
			CommandArgument = commandArgument;

		}

		public CommandEventArgs(CommandEventArgs args)
		{

			this.CommandName = args.CommandName;
			this.CommandArgument = args.CommandArgument;

		}

		public string CommandName { get; set; }

		public object CommandArgument { get; set; }

	}

}

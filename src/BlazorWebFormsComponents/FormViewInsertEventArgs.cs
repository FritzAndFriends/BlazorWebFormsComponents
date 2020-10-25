namespace BlazorWebFormsComponents
{
	public class FormViewInsertEventArgs {

		public FormViewInsertEventArgs(string commandArgument)
		{
			CommandArgument = commandArgument;
		}

		public object CommandArgument { get; }

	}

}

namespace BlazorWebFormsComponents
{
	public class FormViewInsertEventArgs {

		public FormViewInsertEventArgs(string commandArgument)
		{
			CommandArgument = commandArgument;
		}

		public object CommandArgument { get; }

		/// <summary>
		/// The component that raised this event
		/// </summary>
		public object Sender { get; set; }

	}

}

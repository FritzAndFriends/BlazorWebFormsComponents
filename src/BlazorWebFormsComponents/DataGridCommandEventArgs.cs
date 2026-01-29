namespace BlazorWebFormsComponents
{
	public class DataGridCommandEventArgs
	{

		public object CommandArgument { get; set; }

		public string CommandName { get; set; }

		public object CommandSource { get; set; }

		public bool Handled { get; set; }

		public object Item { get; set; }

	}
}

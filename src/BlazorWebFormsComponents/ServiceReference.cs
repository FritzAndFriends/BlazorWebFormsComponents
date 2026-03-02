namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents a reference to a web service, used by ScriptManagerProxy for migration compatibility.
	/// </summary>
	public class ServiceReference
	{
		public string Path { get; set; }

		public bool InlineScript { get; set; }
	}
}

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents a reference to a script file, used by ScriptManagerProxy for migration compatibility.
	/// </summary>
	public class ScriptReference
	{
		public string Name { get; set; }

		public string Path { get; set; }

		public string Assembly { get; set; }
	}
}

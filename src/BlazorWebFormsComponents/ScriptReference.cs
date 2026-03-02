using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents a reference to a script file, used by ScriptManager and ScriptManagerProxy for migration compatibility.
	/// </summary>
	public class ScriptReference
	{
		public string Name { get; set; }

		public string Path { get; set; }

		public string Assembly { get; set; }

		public ScriptMode ScriptMode { get; set; } = ScriptMode.Auto;

		public bool NotifyScriptLoaded { get; set; } = true;

		public string ResourceUICultures { get; set; }
	}
}

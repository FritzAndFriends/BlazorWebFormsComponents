using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Migration stub for System.Web.UI.ScriptManager.
	/// Renders no visible output — exists for markup compatibility during migration.
	/// </summary>
	public partial class ScriptManager : BaseWebFormsComponent
	{
		[Parameter]
		public bool EnablePartialRendering { get; set; }

		[Parameter]
		public bool EnablePageMethods { get; set; }

		[Parameter]
		public ScriptMode ScriptMode { get; set; } = ScriptMode.Auto;

		[Parameter]
		public int AsyncPostBackTimeout { get; set; } = 90;

		[Parameter]
		public bool EnableCdn { get; set; }

		[Parameter]
		public bool EnableScriptGlobalization { get; set; }

		[Parameter]
		public bool EnableScriptLocalization { get; set; }
	}
}

using System.Collections.Generic;
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
		public bool EnablePartialRendering { get; set; } = true;

		[Parameter]
		public bool EnablePageMethods { get; set; }

		[Parameter]
		public EnumParameter<ScriptMode> ScriptMode { get; set; } = Enums.ScriptMode.Auto;

		[Parameter]
		public int AsyncPostBackTimeout { get; set; } = 90;

		[Parameter]
		public bool EnableCdn { get; set; }

		[Parameter]
		public bool EnableScriptGlobalization { get; set; }

		[Parameter]
		public bool EnableScriptLocalization { get; set; }

		[Parameter]
		public List<ScriptReference> Scripts { get; set; } = new();
	}
}

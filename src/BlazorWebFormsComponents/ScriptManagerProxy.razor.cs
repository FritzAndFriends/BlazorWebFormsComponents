using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Migration stub for System.Web.UI.ScriptManagerProxy.
	/// Used in content pages that reference a ScriptManager in a master page.
	/// Renders no visible output.
	/// </summary>
	public partial class ScriptManagerProxy : BaseWebFormsComponent
	{
		[Parameter]
		public List<ScriptReference> Scripts { get; set; } = new();

		[Parameter]
		public List<ServiceReference> Services { get; set; } = new();
	}
}

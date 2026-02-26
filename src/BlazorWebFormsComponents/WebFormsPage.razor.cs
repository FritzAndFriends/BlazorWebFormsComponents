using BlazorWebFormsComponents.Theming;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents;

/// <summary>
/// Unified legacy Web Forms support wrapper. Combines NamingContainer (ID mangling)
/// and ThemeProvider (skin/theme cascading) into a single component that mirrors
/// System.Web.UI.Page — the root of every Web Forms page.
///
/// Place in MainLayout.razor wrapping @Body to give all pages naming scope and theming,
/// or use per-page for area-specific configuration.
/// </summary>
public partial class WebFormsPage : NamingContainer
{
	/// <summary>
	/// Optional theme configuration to cascade to all child components.
	/// When null, theming is effectively disabled (child components skip theme application).
	/// </summary>
	[Parameter]
	public ThemeConfiguration Theme { get; set; }
}

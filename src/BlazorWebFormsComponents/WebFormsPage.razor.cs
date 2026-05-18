using BlazorWebFormsComponents.Theming;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents;

/// <summary>
/// Unified legacy Web Forms support wrapper. Combines NamingContainer (ID mangling),
/// ThemeProvider (skin/theme cascading), and optional page head rendering into a single component.
/// </summary>
public partial class WebFormsPage : NamingContainer
{
	/// <summary>
	/// Optional theme configuration to cascade to all child components.
	/// When null, theming is effectively disabled (child components skip theme application).
	/// </summary>
	[Parameter]
	public ThemeConfiguration Theme { get; set; }

	/// <summary>
	/// When true (the default), renders the shared <see cref="Page"/> component so page title
	/// and meta tags come from the same shim surface used by migrated page code-behind.
	/// </summary>
	[Parameter]
	public bool RenderPageHead { get; set; } = true;
}

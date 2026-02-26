using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents;

/// <summary>
/// Establishes a naming scope for child components, equivalent to ASP.NET Web Forms INamingContainer.
/// Child controls get IDs prefixed with this container's ID, separated by underscores.
/// Renders no HTML of its own — it is a purely structural component.
/// </summary>
public partial class NamingContainer : BaseWebFormsComponent
{
	[Parameter]
	public RenderFragment ChildContent { get; set; }

	/// <summary>
	/// When true, prepends "ctl00" to the naming hierarchy for full Web Forms compatibility.
	/// For example, a Button with ID="MyButton" inside a NamingContainer with ID="MainContent"
	/// and UseCtl00Prefix=true would get ClientID="ctl00_MainContent_MyButton".
	/// Automatically sets ClientIDMode to AutoID for backward compatibility.
	/// </summary>
	[Parameter]
	public bool UseCtl00Prefix { get; set; }

	protected override void OnParametersSet()
	{
		base.OnParametersSet();

		// UseCtl00Prefix implies AutoID mode so children inherit ctl00 behavior
		if (UseCtl00Prefix && ClientIDMode == ClientIDMode.Inherit)
		{
			ClientIDMode = ClientIDMode.AutoID;
		}
	}
}

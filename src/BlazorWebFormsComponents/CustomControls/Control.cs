namespace BlazorWebFormsComponents.CustomControls
{
	/// <summary>
	/// Compatibility class for migrated ASP.NET controls that inherit from
	/// System.Web.UI.Control directly (without styling support).
	/// Maps to the root control base class in Web Forms.
	/// </summary>
	/// <remarks>
	/// Usage: change <c>using System.Web.UI;</c> to
	/// <c>using BlazorWebFormsComponents.CustomControls;</c> and
	/// classes inheriting from Control will compile against this class.
	/// For controls that need styling (BackColor, CssClass, etc.), use
	/// <see cref="WebControl"/> instead.
	/// </remarks>
	public abstract class Control : BaseWebFormsComponent
	{
	}
}

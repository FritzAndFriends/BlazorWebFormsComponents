namespace BlazorWebFormsComponents.CustomControls
{
	/// <summary>
	/// Compatibility class for migrated ASP.NET User Controls (.ascx).
	/// Maps to System.Web.UI.UserControl — provides the same base surface
	/// as BaseStyledComponent plus lifecycle auto-wiring so that
	/// Page_Init, Page_Load, Page_PreRender, and Page_Unload methods
	/// are automatically called at the correct Blazor lifecycle points.
	/// </summary>
	/// <remarks>
	/// Usage: change <c>using System.Web.UI;</c> to
	/// <c>using BlazorWebFormsComponents.CustomControls;</c> and
	/// the ASCX code-behind will compile against this class.
	/// </remarks>
	public abstract class UserControl : BaseStyledComponent
	{
	}
}

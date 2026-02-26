namespace BlazorWebFormsComponents.Enums;

/// <summary>
/// Specifies how a Menu control renders HTML.
/// </summary>
public enum MenuRenderingMode
{
	/// <summary>
	/// The menu renders using unordered list (ul/li) elements. Default in ASP.NET 4.0+.
	/// </summary>
	List,

	/// <summary>
	/// The menu renders using table (table/tr/td) elements. Legacy pre-4.0 default.
	/// </summary>
	Table
}

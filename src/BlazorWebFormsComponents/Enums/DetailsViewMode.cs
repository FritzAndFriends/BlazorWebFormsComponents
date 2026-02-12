namespace BlazorWebFormsComponents.Enums;

/// <summary>
/// Represents the different data-entry modes of a DetailsView control.
/// </summary>
public enum DetailsViewMode
{
	/// <summary>
	/// The DetailsView is in read-only display mode.
	/// </summary>
	ReadOnly = 0,

	/// <summary>
	/// The DetailsView is in edit mode, allowing the user to update an existing record.
	/// </summary>
	Edit = 1,

	/// <summary>
	/// The DetailsView is in insert mode, allowing the user to add a new record.
	/// </summary>
	Insert = 2
}

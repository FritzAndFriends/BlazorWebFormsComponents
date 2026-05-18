using System.Collections.Specialized;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Non-generic base for data control fields, providing the Web Forms
	/// <c>ExtractValuesFromCell</c> pattern used by <c>GridViewRow.Cells</c>.
	/// Mirrors System.Web.UI.WebControls.DataControlField for migration compatibility.
	/// </summary>
	public abstract class DataControlField
	{
		/// <summary>
		/// The header text of the field.
		/// </summary>
		public string HeaderText { get; set; } = string.Empty;

		/// <summary>
		/// Whether the field is visible.
		/// </summary>
		public bool Visible { get; set; } = true;

		/// <summary>
		/// Extracts field values from a cell into the provided dictionary.
		/// Mirrors System.Web.UI.WebControls.DataControlField.ExtractValuesFromCell.
		/// </summary>
		/// <param name="dictionary">The dictionary to populate with extracted values.</param>
		/// <param name="cell">The cell containing the values.</param>
		/// <param name="rowState">The state of the row.</param>
		/// <param name="includeReadOnly">Whether to include read-only field values.</param>
		public abstract void ExtractValuesFromCell(
			IOrderedDictionary dictionary,
			DataControlFieldCell cell,
			DataControlRowState rowState,
			bool includeReadOnly);
	}
}

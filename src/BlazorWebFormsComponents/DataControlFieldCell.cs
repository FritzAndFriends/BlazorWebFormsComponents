using System.Collections.Specialized;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Represents a cell in a data-bound control row, wrapping a column and its
	/// associated row data. Mirrors System.Web.UI.WebControls.DataControlFieldCell
	/// for migration compatibility with the <c>GridViewRow.Cells</c> pattern.
	/// </summary>
	public class DataControlFieldCell
	{
		/// <summary>
		/// The field that owns this cell.
		/// </summary>
		public DataControlField ContainingField { get; }

		/// <summary>
		/// Whether the cell is visible.
		/// </summary>
		public bool Visible => ContainingField?.Visible ?? true;

		/// <summary>
		/// Gets the display text for the cell, matching the Web Forms <c>Cells[i].Text</c> pattern.
		/// </summary>
		public string Text
		{
			get
			{
				if (ContainingField == null)
					return string.Empty;

				var values = new OrderedDictionary();
				ContainingField.ExtractValuesFromCell(values, this, DataControlRowState.Normal, includeReadOnly: true);
				if (values.Count == 0)
					return string.Empty;

				return values[0]?.ToString() ?? string.Empty;
			}
		}

		/// <summary>
		/// Creates a new DataControlFieldCell wrapping the specified field.
		/// </summary>
		public DataControlFieldCell(DataControlField containingField)
		{
			ContainingField = containingField;
		}
	}
}

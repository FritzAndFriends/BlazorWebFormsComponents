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
		/// Creates a new DataControlFieldCell wrapping the specified field.
		/// </summary>
		public DataControlFieldCell(DataControlField containingField)
		{
			ContainingField = containingField;
		}
	}
}

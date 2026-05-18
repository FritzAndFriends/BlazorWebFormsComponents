using System;

namespace BlazorWebFormsComponents;

/// <summary>
/// Non-generic compatibility shim for Web Forms <c>System.Web.UI.WebControls.GridViewRow</c>.
/// <para>
/// Web Forms code often passes <c>GridViewRow</c> without a type parameter (e.g., in
/// helper methods like <c>GetValues(GridViewRow row)</c>). The BWFC Blazor component is
/// generic (<c>GridViewRow&lt;T&gt;</c>), so this non-generic class bridges the gap.
/// </para>
/// <para>
/// Instances are created automatically via the implicit operator on <c>GridViewRow&lt;T&gt;</c>.
/// </para>
/// </summary>
public class GridViewRow
{
	private readonly object _genericRow;

	/// <summary>
	/// Creates an empty GridViewRow (for compile compatibility).
	/// </summary>
	public GridViewRow()
	{
	}

	/// <summary>
	/// Creates a GridViewRow wrapping a generic <c>GridViewRow&lt;T&gt;</c> instance.
	/// Used by the implicit operator.
	/// </summary>
	internal GridViewRow(object genericRow)
	{
		_genericRow = genericRow;

		if (genericRow != null)
		{
			var type = genericRow.GetType();
			RowIndex = (int)(type.GetProperty("RowIndex")?.GetValue(genericRow) ?? 0);
			DataItemIndex = (int)(type.GetProperty("DataItemIndex")?.GetValue(genericRow) ?? 0);
			DataItem = type.GetProperty("DataItem")?.GetValue(genericRow);

			// Extract RowState
			var rowStateObj = type.GetProperty("RowState")?.GetValue(genericRow);
			if (rowStateObj is DataControlRowState rs) RowState = rs;

			// Extract Cells
			var cellsObj = type.GetProperty("Cells")?.GetValue(genericRow);
			if (cellsObj is DataControlFieldCellCollection cells) Cells = cells;
		}
	}

	public int RowIndex { get; set; }
	public int DataItemIndex { get; set; }
	public object DataItem { get; set; } = null!;

	/// <summary>
	/// Gets the state of the row (Normal, Edit, Selected, etc.).
	/// </summary>
	public DataControlRowState RowState { get; set; }

	/// <summary>
	/// Gets the cells in this row.
	/// </summary>
	public DataControlFieldCellCollection Cells { get; set; }

	/// <summary>
	/// Finds a control by ID, delegating to the wrapped generic row.
	/// Enables the Web Forms pattern: <c>((TextBox)row.FindControl("X")).Text</c>
	/// </summary>
	public BaseWebFormsComponent FindControl(string controlId)
	{
		if (_genericRow == null) return null;

		var method = _genericRow.GetType().GetMethod("FindControl", new[] { typeof(string) });
		return method?.Invoke(_genericRow, new object[] { controlId }) as BaseWebFormsComponent;
	}
}


using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// The row of a GridView
	/// </summary>
	public partial class GridViewRow<ItemType> : BaseRow<ItemType>
	{
		/// <summary>
		/// The data item Index
		/// </summary>
		[Parameter] public int DataItemIndex { get; set; }

		/// <summary>
		/// The row index
		/// </summary>
		[Parameter] public int RowIndex { get; set; }

		/// <summary>
		/// The columns of the Row
		/// </summary>
		[Parameter] public List<IColumn<ItemType>> Columns { get; set; }

		/// <summary>
		/// Whether this row is in edit mode
		/// </summary>
		[Parameter] public bool IsEditing { get; set; }

		/// <summary>
		/// Style applied when the row is in edit mode
		/// </summary>
		[Parameter] public TableItemStyle EditRowStyle { get; set; }

		/// <summary>
		/// Style applied to this row (RowStyle or AlternatingRowStyle)
		/// </summary>
		[Parameter] public TableItemStyle RowStyle { get; set; }

		/// <summary>
		/// Whether this row is currently selected
		/// </summary>
		[Parameter] public bool IsSelected { get; set; }

		/// <summary>
		/// Reference to the parent GridView
		/// </summary>
		[Parameter] public GridView<ItemType> GridView { get; set; }
	}
}

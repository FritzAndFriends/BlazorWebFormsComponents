using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// The row of a DataGrid
	/// </summary>
	public partial class DataGridRow<ItemType> : BaseRow<ItemType>
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
		/// The style applied to this row.
		/// </summary>
		[Parameter] public TableItemStyle RowStyle { get; set; }
	}
}

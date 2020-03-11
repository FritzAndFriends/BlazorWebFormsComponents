using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{ 
	/// <summary>
	/// The row of a GridView
	/// </summary>
  public partial class GridViewRow: BaseRow
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
		[Parameter] public List<IColumn> Columns { get; set; }

	}
}

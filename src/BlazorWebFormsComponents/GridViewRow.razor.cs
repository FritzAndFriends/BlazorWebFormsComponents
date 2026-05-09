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

		/// <summary>
		/// The form naming context for this row, cascaded to child controls.
		/// Provides the naming prefix (e.g., "CartList$ctl04") so child controls
		/// can generate Web Forms-compatible form field names.
		/// </summary>
		private FormNamingContext _formNamingContext;

		protected override void OnParametersSet()
		{
			base.OnParametersSet();

			// Build the naming context: GridView's UniqueID + ctl{nn} for this row
			var gridUniqueId = GridView?.UniqueID ?? GridView?.ID;
			var ctlId = FormNamingContext.GetRowCtlId(DataItemIndex);

			var prefix = string.IsNullOrEmpty(gridUniqueId)
				? ctlId
				: $"{gridUniqueId}${ctlId}";

			_formNamingContext = new FormNamingContext(prefix, DataItemIndex);
		}

		/// <summary>
		/// Sets the form naming context on a column so BoundField and other column types
		/// can generate row-scoped form field names during RenderEdit.
		/// </summary>
		private void SetColumnNamingContext(IColumn<ItemType> column)
		{
			if (column is BaseColumn<ItemType> baseCol)
			{
				baseCol.CurrentFormNamingContext = _formNamingContext;
			}
		}
	}
}

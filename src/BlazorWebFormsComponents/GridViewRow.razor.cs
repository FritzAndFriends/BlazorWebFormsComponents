using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using System;
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
		/// The form data dictionary for this row, passed down from GridView.
		/// Contains only the form values relevant to this row (keyed by control ID).
		/// </summary>
		[Parameter] public IDictionary<string, string> FormRowData { get; set; }

		/// <summary>
		/// Gets the state of the row (Normal, Edit, Selected, etc.).
		/// Mirrors System.Web.UI.WebControls.GridViewRow.RowState.
		/// </summary>
		public DataControlRowState RowState
		{
			get
			{
				var state = DataControlRowState.Normal;
				if (RowIndex % 2 == 1) state |= DataControlRowState.Alternate;
				if (IsEditing) state |= DataControlRowState.Edit;
				if (IsSelected) state |= DataControlRowState.Selected;
				return state;
			}
		}

		/// <summary>
		/// Gets the cells in this row, wrapping each column as a <see cref="DataControlFieldCell"/>.
		/// Mirrors System.Web.UI.WebControls.GridViewRow.Cells for migration compatibility.
		/// </summary>
		public DataControlFieldCellCollection Cells
		{
			get
			{
				if (_cells == null)
				{
					var cellList = new List<DataControlFieldCell>();
					if (Columns != null)
					{
						foreach (var column in Columns)
						{
							DataControlField dcf;
							if (column is BoundField<ItemType> bf)
								dcf = new BoundDataControlField<ItemType>(bf, DataItem);
							else
								dcf = new TemplateDataControlField(column.HeaderText);

							cellList.Add(new DataControlFieldCell(dcf));
						}
					}
					_cells = new DataControlFieldCellCollection(cellList);
				}
				return _cells;
			}
		}
		private DataControlFieldCellCollection _cells;

		/// <summary>
		/// Implicit conversion to the non-generic <see cref="GridViewRow"/> shim,
		/// enabling Web Forms method signatures like <c>GetValues(GridViewRow row)</c>
		/// to accept <c>GridView.Rows[i]</c> without an explicit cast.
		/// </summary>
		public static implicit operator GridViewRow(GridViewRow<ItemType> source)
		{
			if (source == null) return null;
			return new GridViewRow(source);
		}

		/// <summary>
		/// The form naming context for this row, cascaded to child controls.
		/// Provides the naming prefix (e.g., "CartList$ctl04") so child controls
		/// can generate Web Forms-compatible form field names.
		/// </summary>
		private FormNamingContext _formNamingContext;

		protected override void OnParametersSet()
		{
			base.OnParametersSet();

			// Invalidate cached cells when parameters change
			_cells = null;

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

		/// <summary>
		/// Finds a control by ID. When form data is available, returns a proxy control
		/// populated from form POST data (enabling the Web Forms FindControl pattern).
		/// Falls back to the live component tree when no form data is present.
		/// This enables Web Forms code-behind like:
		///   TextBox qty = (TextBox)CartList.Rows[i].FindControl("PurchaseQuantity");
		///   int quantity = Convert.ToInt16(qty.Text);
		/// to compile and work unchanged after migration.
		/// </summary>
		public override BaseWebFormsComponent FindControl(string controlId)
		{
			if (string.IsNullOrEmpty(controlId)) return null;

			// Get the registered control type from the naming context
			var controlType = _formNamingContext?.GetControlType(controlId);

			// When form data is available, create a proxy with form values
			// This takes priority over live components because the caller wants
			// the submitted form values, not the rendered component state
			if (FormRowData != null && controlType != null)
			{
				FormRowData.TryGetValue(controlId, out var formValue);

				if (controlType == typeof(TextBox))
				{
					return new TextBox { ID = controlId, Text = formValue ?? string.Empty };
				}

				if (controlType == typeof(CheckBox))
				{
					// Checkbox form values: present means checked, absent means unchecked.
					// Value is "on" or "true" when checked.
					var isChecked = formValue != null &&
						(formValue.Equals("on", StringComparison.OrdinalIgnoreCase) ||
						 formValue.Equals("true", StringComparison.OrdinalIgnoreCase));
					return new CheckBox { ID = controlId, Checked = isChecked };
				}
			}

			// If we have form data but no registered type, check for a value
			if (FormRowData != null && FormRowData.TryGetValue(controlId, out var fallbackValue))
			{
				return new TextBox { ID = controlId, Text = fallbackValue };
			}

			// Fall back to the live component tree
			return base.FindControl(controlId);
		}
	}
}

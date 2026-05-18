using System.Collections.Specialized;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A DataControlField backed by a BoundField column. Extracts the bound
	/// property value from the row's DataItem into the dictionary keyed by DataField.
	/// </summary>
	internal class BoundDataControlField<ItemType> : DataControlField
	{
		private readonly BoundField<ItemType> _column;
		private readonly object _dataItem;

		public BoundDataControlField(BoundField<ItemType> column, object dataItem)
		{
			_column = column;
			_dataItem = dataItem;
			HeaderText = column.HeaderText ?? string.Empty;
		}

		public override void ExtractValuesFromCell(
			IOrderedDictionary dictionary,
			DataControlFieldCell cell,
			DataControlRowState rowState,
			bool includeReadOnly)
		{
			if (_column.DataField == null || _dataItem == null) return;
			if (_column.ReadOnly && !includeReadOnly) return;

			// Walk the property chain (e.g., "Product.ProductName")
			var properties = _column.DataField.Split('.');
			var value = _dataItem;
			foreach (var property in properties)
			{
				if (value == null) break;
				value = DataBinder.GetPropertyValue(value, property);
			}

			dictionary[_column.DataField] = value;
		}
	}

	/// <summary>
	/// A DataControlField backed by a TemplateField column. Template fields
	/// contain arbitrary content and don't extract values automatically.
	/// </summary>
	internal class TemplateDataControlField : DataControlField
	{
		public TemplateDataControlField(string headerText)
		{
			HeaderText = headerText ?? string.Empty;
		}

		public override void ExtractValuesFromCell(
			IOrderedDictionary dictionary,
			DataControlFieldCell cell,
			DataControlRowState rowState,
			bool includeReadOnly)
		{
			// Template fields don't auto-extract values.
			// FindControl is the intended pattern for accessing template controls.
		}
	}
}

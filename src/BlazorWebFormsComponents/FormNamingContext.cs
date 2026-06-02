using System;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides the naming container context for form controls inside data-bound rows.
	/// Cascaded from GridViewRow (and similar row containers) to child controls so they
	/// can generate Web Forms-compatible UniqueID values for HTML name attributes.
	///
	/// In Web Forms, a TextBox with ID="PurchaseQuantity" inside the 3rd data row of a
	/// GridView with ID="CartList" would get UniqueID = "CartList$ctl04$PurchaseQuantity"
	/// (ctl02 = header, ctl03 = first data row, ctl04 = second data row, etc.).
	///
	/// The Prefix property carries the parent portion (e.g., "CartList$ctl04") so child
	/// controls only need to append "$" + their own ID.
	///
	/// Controls self-register their type during rendering so that FindControl can create
	/// correctly-typed proxy instances populated from form POST data.
	/// </summary>
	public class FormNamingContext
	{
		/// <summary>
		/// The naming container prefix including the parent grid's UniqueID and the
		/// row's ctl identifier. Example: "CartList$ctl04"
		/// </summary>
		public string Prefix { get; }

		/// <summary>
		/// The zero-based data item index of the row within the data source.
		/// </summary>
		public int RowIndex { get; }

		/// <summary>
		/// Registry of control IDs to their concrete types, populated during render
		/// as each control registers itself. Used by FindControl to create properly-typed
		/// proxy instances from form data.
		/// </summary>
		private readonly Dictionary<string, Type> _controlTypes = new(StringComparer.OrdinalIgnoreCase);

		public FormNamingContext(string prefix, int rowIndex)
		{
			Prefix = prefix;
			RowIndex = rowIndex;
		}

		/// <summary>
		/// Registers a control's type so FindControl can create the correct proxy type.
		/// Called by controls (TextBox, CheckBox, etc.) during their parameter set phase.
		/// </summary>
		/// <param name="controlId">The control's ID (e.g., "PurchaseQuantity")</param>
		/// <param name="controlType">The concrete control type (e.g., typeof(TextBox))</param>
		public void RegisterControl(string controlId, Type controlType)
		{
			if (!string.IsNullOrEmpty(controlId) && controlType != null)
			{
				_controlTypes[controlId] = controlType;
			}
		}

		/// <summary>
		/// Gets the registered type for a control ID, or null if not registered.
		/// </summary>
		public Type GetControlType(string controlId)
		{
			if (string.IsNullOrEmpty(controlId)) return null;
			return _controlTypes.TryGetValue(controlId, out var type) ? type : null;
		}

		/// <summary>
		/// Generates the full UniqueID for a child control within this naming context.
		/// </summary>
		/// <param name="controlId">The control's own ID (e.g., "PurchaseQuantity")</param>
		/// <returns>The full form name (e.g., "CartList$ctl04$PurchaseQuantity"), or null if controlId is empty</returns>
		public string GetChildUniqueID(string controlId)
		{
			return ComponentIdGenerator.GetUniqueIDWithContext(Prefix, controlId);
		}

		/// <summary>
		/// Generates the ctl identifier for a Web Forms row.
		/// Web Forms uses ctl00 for the page naming container, then data rows start at ctl02
		/// (ctl01 is typically the header row). We follow the same convention.
		/// </summary>
		/// <param name="dataItemIndex">Zero-based index of the data item</param>
		/// <returns>The ctl identifier (e.g., "ctl02" for the first data row)</returns>
		public static string GetRowCtlId(int dataItemIndex)
		{
			// Web Forms convention: header is ctl01, first data row is ctl02
			var ctlNumber = dataItemIndex + 2;
			return $"ctl{ctlNumber:D2}";
		}
	}
}

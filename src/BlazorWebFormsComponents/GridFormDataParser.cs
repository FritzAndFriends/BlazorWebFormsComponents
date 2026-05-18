using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Parses form POST data from controls inside GridView rows.
	/// Web Forms naming convention uses '$' as separator:
	///   GridView1$ctl02$PurchaseQuantity
	///   GridView1$ctl03$RemoveItem
	///
	/// This parser extracts the row index and control name from each form key
	/// and returns structured data grouped by row.
	/// </summary>
	public static class GridFormDataParser
	{
		// Matches: {gridId}$ctl{nn}${controlName}
		// Group 1: gridId, Group 2: ctl number (2+ digits), Group 3: controlName
		private static readonly Regex CtlPattern = new Regex(
			@"^(.+)\$ctl(\d{2,})\$(.+)$",
			RegexOptions.Compiled);

		/// <summary>
		/// Parses form data for a specific grid, extracting row-level field values.
		/// </summary>
		/// <param name="formData">The form data dictionary (e.g., from Request.Form)</param>
		/// <param name="gridId">The grid's ID or UniqueID to filter on</param>
		/// <returns>Dictionary mapping row index to a dictionary of field name → value</returns>
		public static Dictionary<int, Dictionary<string, string>> Parse(
			IDictionary<string, StringValues> formData,
			string gridId)
		{
			var result = new Dictionary<int, Dictionary<string, string>>();

			if (formData == null || string.IsNullOrEmpty(gridId))
				return result;

			var prefix = gridId + "$ctl";

			foreach (var kvp in formData)
			{
				// Quick prefix check before regex
				if (!kvp.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
					continue;

				var match = CtlPattern.Match(kvp.Key);
				if (!match.Success)
					continue;

				var matchedGridId = match.Groups[1].Value;
				if (!string.Equals(matchedGridId, gridId, StringComparison.OrdinalIgnoreCase))
					continue;

				var ctlNumber = int.Parse(match.Groups[2].Value);
				var controlName = match.Groups[3].Value;

				// Convert ctl number back to data item index (ctl02 = row 0, ctl03 = row 1, etc.)
				var rowIndex = ctlNumber - 2;
				if (rowIndex < 0)
					continue;

				if (!result.TryGetValue(rowIndex, out var rowData))
				{
					rowData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
					result[rowIndex] = rowData;
				}

				// Use the first value from StringValues
				rowData[controlName] = kvp.Value.ToString();
			}

			return result;
		}

		/// <summary>
		/// Gets the form values for a specific row in a grid.
		/// </summary>
		/// <param name="formData">The form data dictionary</param>
		/// <param name="gridId">The grid's ID</param>
		/// <param name="rowIndex">The zero-based data row index</param>
		/// <returns>Dictionary of control name → value for that row, or empty dictionary</returns>
		public static Dictionary<string, string> GetRowValues(
			IDictionary<string, StringValues> formData,
			string gridId,
			int rowIndex)
		{
			var allRows = Parse(formData, gridId);
			return allRows.TryGetValue(rowIndex, out var rowData)
				? rowData
				: new Dictionary<string, string>();
		}

		/// <summary>
		/// Gets a single form value for a specific control in a specific row.
		/// </summary>
		/// <param name="formData">The form data dictionary</param>
		/// <param name="gridId">The grid's ID</param>
		/// <param name="rowIndex">The zero-based data row index</param>
		/// <param name="controlName">The control's ID (e.g., "PurchaseQuantity")</param>
		/// <returns>The form value, or null if not found</returns>
		public static string GetValue(
			IDictionary<string, StringValues> formData,
			string gridId,
			int rowIndex,
			string controlName)
		{
			var ctlId = FormNamingContext.GetRowCtlId(rowIndex);
			var key = $"{gridId}${ctlId}${controlName}";

			if (formData != null && formData.TryGetValue(key, out var values))
				return values.ToString();

			return null;
		}
	}
}

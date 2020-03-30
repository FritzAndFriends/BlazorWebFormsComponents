using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Generic collection of IRow
	/// </summary>
  public interface IRowCollection
  {
		/// <summary>
		/// The list of IRows
		/// </summary>
		List<IRow> RowList { get; set; }

		/// <summary>
		/// Adds a IRow to the collection
		/// </summary>
		/// <param name="row"></param>
		void AddRow(IRow row);

		/// <summary>
		/// Deletes an IRow from the collection
		/// </summary>
		/// <param name="row"></param>
		void RemoveRow(IRow row);
	}
}

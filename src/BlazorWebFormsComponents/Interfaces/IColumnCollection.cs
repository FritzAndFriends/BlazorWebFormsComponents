using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Generic column collection interface
	/// </summary>
  public interface IColumnCollection
  {
		/// <summary>
		/// The list of IColumns
		/// </summary>
		List<IColumn> ColumnList { get; set; }

		/// <summary>
		/// Adds an IColumn to the collection
		/// </summary>
		/// <param name="column"></param>
		void AddColumn(IColumn column);

		/// <summary>
		/// Removes an IColumn from the collection
		/// </summary>
		/// <param name="column"></param>
		void RemoveColumn(IColumn column);
	}
}

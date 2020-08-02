using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Generic column collection interface
	/// </summary>
  public interface IColumnCollection<ItemType>
  {
		/// <summary>
		/// The list of IColumns
		/// </summary>
		List<IColumn<ItemType>> ColumnList { get; set; }

		/// <summary>
		/// Adds an IColumn to the collection
		/// </summary>
		/// <param name="column"></param>
		void AddColumn(IColumn<ItemType> column);

		/// <summary>
		/// Removes an IColumn from the collection
		/// </summary>
		/// <param name="column"></param>
		void RemoveColumn(IColumn<ItemType> column);
	}
}

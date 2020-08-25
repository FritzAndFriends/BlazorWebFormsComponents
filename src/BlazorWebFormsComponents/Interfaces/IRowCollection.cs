using System.Collections.Generic;

namespace BlazorWebFormsComponents.Interfaces
{
	/// <summary>
	/// Generic collection of IRow
	/// </summary>
	public interface IRowCollection<ItemType>
	{
		/// <summary>
		/// The list of IRows
		/// </summary>
		List<IRow<ItemType>> RowList { get; set; }

		/// <summary>
		/// Adds a IRow to the collection
		/// </summary>
		/// <param name="row"></param>
		void AddRow(IRow<ItemType> row);

		/// <summary>
		/// Deletes an IRow from the collection
		/// </summary>
		/// <param name="row"></param>
		void RemoveRow(IRow<ItemType> row);
	}
}

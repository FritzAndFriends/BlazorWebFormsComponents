using System.Collections;
using System.Collections.Generic;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A collection of <see cref="DataControlFieldCell"/> objects representing the cells
	/// in a data-bound row. Provides indexer access matching the Web Forms
	/// <c>GridViewRow.Cells</c> pattern.
	/// </summary>
	public class DataControlFieldCellCollection : IReadOnlyList<DataControlFieldCell>
	{
		private readonly List<DataControlFieldCell> _cells;

		internal DataControlFieldCellCollection(List<DataControlFieldCell> cells)
		{
			_cells = cells ?? new List<DataControlFieldCell>();
		}

		/// <summary>
		/// Gets the cell at the specified index.
		/// </summary>
		public DataControlFieldCell this[int index] => _cells[index];

		/// <summary>
		/// Gets the number of cells in the collection.
		/// </summary>
		public int Count => _cells.Count;

		public IEnumerator<DataControlFieldCell> GetEnumerator() => _cells.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _cells.GetEnumerator();
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DATA = System.Data;

namespace BlazorWebFormsComponents.Data
{

	public class DataTable : IEnumerable<DataRow>
	{

		private DATA.DataTable _WrappedTable;

		public IEnumerator<DataRow> GetEnumerator()
		{
			yield return _WrappedTable.Rows.GetEnumerator() as DATA.DataRow;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			yield return _WrappedTable.Rows.GetEnumerator();
		}

		public static implicit operator DataTable(DATA.DataTable table) {

			return new DataTable { _WrappedTable = table };

		}

	}

}

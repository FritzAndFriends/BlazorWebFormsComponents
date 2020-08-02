using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DATA = System.Data;

namespace BlazorWebFormsComponents.Data
{

	public class DataTable : IEnumerable<DataRow>
	{
		public IEnumerator<DataRow> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

}

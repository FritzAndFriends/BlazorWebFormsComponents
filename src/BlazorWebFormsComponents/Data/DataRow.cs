using System.Dynamic;
using DATA = System.Data;

namespace BlazorWebFormsComponents.Data
{
	public class DataRow : DynamicObject {

		private DATA.DataRow _Row;

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return base.TryGetMember(binder, out result);
		}

		public static implicit operator DataRow(DATA.DataRow row)
		{

			return new DataRow
			{
				_Row = row
			};

		}

	}

}

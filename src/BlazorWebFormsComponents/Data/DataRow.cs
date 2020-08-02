using System.Dynamic;

namespace BlazorWebFormsComponents.Data
{
	public class DataRow : DynamicObject {

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return base.TryGetMember(binder, out result);
		}

	}

}

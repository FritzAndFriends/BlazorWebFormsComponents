using System.Linq;

namespace BlazorWebFormsComponents.DataBinding
{
	public delegate IQueryable<TItemType> SelectHandler<TItemType>(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount);
}

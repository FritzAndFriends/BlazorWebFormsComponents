using System.Linq;

namespace BlazorWebFormsComponents.DataBinding
{
	public delegate IQueryable<ItemType> SelectHandler<ItemType>(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount);
}

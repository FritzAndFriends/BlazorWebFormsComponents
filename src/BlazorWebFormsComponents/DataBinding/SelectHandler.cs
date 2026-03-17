using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents.DataBinding
{
	public delegate IQueryable<TItemType> SelectHandler<TItemType>(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount);

	public delegate IEnumerable<TItemType> SimpleSelectHandler<TItemType>(int maxRows, int startRowIndex, string sortByExpression);

	public delegate Task<IEnumerable<TItemType>> SelectHandlerAsync<TItemType>(int maxRows, int startRowIndex, string sortByExpression);
}

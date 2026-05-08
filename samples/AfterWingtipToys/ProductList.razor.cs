using BlazorWebFormsComponents;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ProductList
{
	private ListView<Product> productList = default!;

	private IQueryable<Product> GetProductsQueryDetails_SelectMethod(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
	{
		using var db = new ProductContext();
		var query = db.Products.AsQueryable();

		if (CategoryId.HasValue && CategoryId.Value > 0)
		{
			query = query.Where(p => p.CategoryID == CategoryId.Value);
		}

		if (!string.IsNullOrEmpty(CategoryName))
		{
			query = query.Where(p => p.Category != null && string.Compare(p.Category.CategoryName, CategoryName, StringComparison.OrdinalIgnoreCase) == 0);
		}

		var items = query.ToList();
		totalRowCount = items.Count;
		return items.AsQueryable();
	}
}
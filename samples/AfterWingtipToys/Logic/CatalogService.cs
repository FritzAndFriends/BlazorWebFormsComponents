using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public sealed class CatalogService(IDbContextFactory<ProductContext> dbFactory)
{
    public async Task<IReadOnlyList<Category>> GetCategoriesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Categories
            .AsNoTracking()
            .OrderBy(category => category.CategoryName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(int? categoryId, string? categoryName)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var query = db.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .AsQueryable();

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(product => product.CategoryID == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            query = query.Where(product => product.Category != null &&
                product.Category.CategoryName == categoryName);
        }

        return await query
            .OrderBy(product => product.ProductName)
            .ToListAsync();
    }

    public async Task<Product?> GetProductAsync(int? productId, string? productName)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var query = db.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .AsQueryable();

        if (productId.HasValue && productId.Value > 0)
        {
            return await query.FirstOrDefaultAsync(product => product.ProductID == productId.Value);
        }

        if (!string.IsNullOrWhiteSpace(productName))
        {
            return await query.FirstOrDefaultAsync(product => product.ProductName == productName);
        }

        return null;
    }
}

using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Services;

public class CategoryNavigationService(IDbContextFactory<ProductContext> dbFactory)
{
    public async Task<IReadOnlyList<Category>> GetCategoriesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.Categories
            .AsNoTracking()
            .OrderBy(category => category.CategoryName)
            .ToListAsync();
    }
}

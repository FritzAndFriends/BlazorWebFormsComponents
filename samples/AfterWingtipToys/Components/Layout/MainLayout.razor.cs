using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys.Components.Layout;

public partial class MainLayout
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    private List<Category> _categories = new();

    protected override async Task OnInitializedAsync()
    {
        using var db = DbFactory.CreateDbContext();
        _categories = await db.Categories.ToListAsync();
    }
}

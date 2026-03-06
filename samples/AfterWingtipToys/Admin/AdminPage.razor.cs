using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys.Admin;

public partial class AdminPage : ComponentBase
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    private List<Category> _categories = new();
    private List<Product> _products = new();

    protected override async Task OnInitializedAsync()
    {
        using var db = await DbFactory.CreateDbContextAsync();
        _categories = await db.Categories.ToListAsync();
        _products = await db.Products.ToListAsync();
    }

    private Task AddProductButton_Click(EventArgs e) => Task.CompletedTask;
    private Task RemoveProductButton_Click(EventArgs e) => Task.CompletedTask;
}

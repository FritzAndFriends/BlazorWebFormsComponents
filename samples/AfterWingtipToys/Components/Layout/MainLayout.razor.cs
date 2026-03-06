using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
    [Inject] private ShoppingCartService CartService { get; set; } = default!;

    private List<Category> _categories = new();
    private int _cartCount;

    protected override async Task OnInitializedAsync()
    {
        using var db = await DbFactory.CreateDbContextAsync();
        _categories = await db.Categories.ToListAsync();
        _cartCount = await CartService.GetCountAsync();
    }
}



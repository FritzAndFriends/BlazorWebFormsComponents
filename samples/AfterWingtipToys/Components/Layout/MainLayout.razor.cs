using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] private ProductContext Db { get; set; } = default!;
    [Inject] private ShoppingCartService Cart { get; set; } = default!;

    private List<Category>? categories;
    private int cartCount;

    protected override async Task OnInitializedAsync()
    {
        categories = await Db.Categories.ToListAsync();
        cartCount = Cart.GetCount();
    }
}

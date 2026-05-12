using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class RemoveItem : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] public ProductContext Db { get; set; } = default!;
    [Parameter, SupplyParameterFromQuery(Name = "cartItemId")] public string? CartItemId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (!string.IsNullOrEmpty(CartItemId))
        {
            var item = await Db.ShoppingCartItems.FirstOrDefaultAsync(c => c.ItemId == CartItemId);
            if (item != null)
            {
                Db.ShoppingCartItems.Remove(item);
                await Db.SaveChangesAsync();
            }
        }

        Response.Redirect("/ShoppingCart");
    }
}
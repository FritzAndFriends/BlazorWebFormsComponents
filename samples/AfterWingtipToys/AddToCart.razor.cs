using Microsoft.AspNetCore.Components;
using WingtipToys.Services;

namespace WingtipToys;

public partial class AddToCart : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] private ShoppingCartService Cart { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "productID")]
    public int? ProductID { get; set; }

    protected override void OnInitialized()
    {
        if (ProductID.HasValue && ProductID > 0)
        {
            Cart.AddToCart(ProductID.Value);
        }
        Nav.NavigateTo("/ShoppingCart", forceLoad: true);
    }
}

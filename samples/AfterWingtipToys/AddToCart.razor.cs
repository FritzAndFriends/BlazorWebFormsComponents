using Microsoft.AspNetCore.Components;
using WingtipToys.Services;

namespace WingtipToys
{
    public partial class AddToCart : ComponentBase
    {
        [Inject]
        private CartStateService CartService { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [SupplyParameterFromQuery]
        public int productID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await CartService.AddToCartAsync(productID);
            Navigation.NavigateTo("/ShoppingCart");
        }
    }
}

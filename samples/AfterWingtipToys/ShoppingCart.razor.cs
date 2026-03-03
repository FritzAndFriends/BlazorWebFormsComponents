using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys
{
    public partial class ShoppingCart : ComponentBase
    {
        [Inject]
        private CartStateService CartService { get; set; } = default!;

        public List<CartItem> CartItems { get; set; } = new();
        public string CartTotal { get; set; } = "$0.00";

        private Dictionary<string, int> _quantities = new();
        private Dictionary<string, bool> _removals = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadCartAsync();
        }

        private async Task LoadCartAsync()
        {
            CartItems = await CartService.GetCartItemsAsync();
            var total = await CartService.GetTotalAsync();
            CartTotal = total.ToString("c");

            _quantities.Clear();
            _removals.Clear();
            foreach (var item in CartItems)
            {
                _quantities[item.ItemId] = item.Quantity;
                _removals[item.ItemId] = false;
            }
        }

        public string GetQuantity(string itemId)
            => _quantities.TryGetValue(itemId, out var qty) ? qty.ToString() : "0";

        public void OnQuantityChanged(string itemId, string value)
        {
            if (int.TryParse(value, out var qty) && qty > 0)
                _quantities[itemId] = qty;
        }

        public bool IsMarkedForRemoval(string itemId)
            => _removals.TryGetValue(itemId, out var marked) && marked;

        public void OnRemovalChanged(string itemId, bool value)
        {
            _removals[itemId] = value;
        }

        public async Task OnUpdateCart(MouseEventArgs args)
        {
            foreach (var kvp in _removals.Where(r => r.Value))
            {
                await CartService.RemoveCartItemAsync(kvp.Key);
            }

            foreach (var kvp in _quantities)
            {
                if (!_removals.GetValueOrDefault(kvp.Key))
                {
                    await CartService.UpdateCartItemAsync(kvp.Key, kvp.Value);
                }
            }

            await LoadCartAsync();
        }
    }
}


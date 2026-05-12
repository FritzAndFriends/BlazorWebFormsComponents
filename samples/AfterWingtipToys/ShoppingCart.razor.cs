using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class ShoppingCart
    {
        [Inject] public ProductContext Db { get; set; } = default!;

        private GridView<CartItem> CartList = default!;
        private TextBox PurchaseQuantity = default!;
        private CheckBox Remove = default!;
        private Label LabelTotalText = default!;
        private Label lblTotal = default!;
        private Button UpdateBtn = default!;
        private ImageButton CheckoutImageBtn = default!;
        private string OrderTotalText = 0m.ToString("C");

        private List<CartItem> cartItems = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadCart();
        }

        private async Task LoadCart()
        {
            var cartId = GetCartId();
            cartItems = await Db.ShoppingCartItems
                .Where(c => c.CartId == cartId)
                .Include(c => c.Product)
                .ToListAsync();

            var total = cartItems.Sum(c => (double)c.Quantity * (c.Product.UnitPrice ?? 0d));
            OrderTotalText = total.ToString("C");
        }

        private async Task UpdateBtn_Click()
        {
            var cartId = GetCartId();
            var items = await Db.ShoppingCartItems
                .Where(c => c.CartId == cartId)
                .Include(c => c.Product)
                .ToListAsync();

            foreach (var item in items)
            {
                var qtyField = Request.Form[$"PurchaseQuantity_{item.ItemId}"];
                if (!string.IsNullOrEmpty(qtyField) && int.TryParse(qtyField, out var newQty) && newQty > 0)
                {
                    item.Quantity = newQty;
                }

                var removeField = Request.Form[$"Remove_{item.ItemId}"];
                if (!string.IsNullOrEmpty(removeField))
                {
                    Db.ShoppingCartItems.Remove(item);
                }
            }

            await Db.SaveChangesAsync();
            await LoadCart();
        }

        private void CheckoutBtn_Click()
        {
            Response.Redirect("Checkout/CheckoutStart");
        }

        private string GetCartId()
        {
            const string cartSessionKey = "CartId";
            var existingCartId = Session[cartSessionKey]?.ToString();
            if (!string.IsNullOrWhiteSpace(existingCartId))
            {
                return existingCartId;
            }

            var newCartId = Guid.NewGuid().ToString();
            Session[cartSessionKey] = newCartId;
            return newCartId;
        }
    }
}

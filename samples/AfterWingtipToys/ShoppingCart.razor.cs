using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using WingtipToys.Logic;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ShoppingCart
{
    [Inject] private CartStore CartStore { get; set; } = default!;
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    private List<CartItem> CartItems { get; set; } = [];
    private string ShoppingCartTitle { get; set; } = "Shopping Cart";
    private string TotalText { get; set; } = "$0.00";

    protected override Task OnParametersSetAsync()
    {
        var cartId = GetOrCreateCartId();
        var workingItems = CartStore.GetCart(cartId)
            .Select(item => new CartItem
            {
                ItemId = item.ItemId,
                CartId = item.CartId,
                ProductId = item.ProductId,
                Product = item.Product,
                Quantity = item.Quantity,
                DateCreated = item.DateCreated
            })
            .ToList();

        if (HttpContextAccessor.HttpContext?.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) == true)
        {
            foreach (var item in workingItems.ToList())
            {
                var qtyValue = Request.Form[$"qty_{item.ProductId}"];
                var removeValue = Request.Form[$"remove_{item.ProductId}"];
                var removeItem = string.Equals(removeValue, "on", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(removeValue, "true", StringComparison.OrdinalIgnoreCase);

                if (removeItem)
                {
                    workingItems.Remove(item);
                    continue;
                }

                if (int.TryParse(qtyValue, out var quantity) && quantity > 0)
                {
                    item.Quantity = quantity;
                }
            }

            CartStore.UpdateCart(cartId, workingItems);
        }

        CartItems = workingItems;
        var total = CartItems.Sum(item => Convert.ToDecimal(item.Product?.UnitPrice ?? 0) * item.Quantity);
        TotalText = total.ToString("C");
        ShoppingCartTitle = CartItems.Count == 0 ? "Shopping Cart is Empty" : "Shopping Cart";
        Title = ShoppingCartTitle;
        return Task.CompletedTask;
    }

    private string GetOrCreateCartId()
    {
        var context = HttpContextAccessor.HttpContext;
        if (context is null)
        {
            return "anonymous-cart";
        }

        if (context.Request.Cookies.TryGetValue("CartSessionId", out var existing) && !string.IsNullOrWhiteSpace(existing))
        {
            return existing;
        }

        var cartId = Guid.NewGuid().ToString("N");
        context.Response.Cookies.Append("CartSessionId", cartId, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = context.Request.IsHttps
        });

        return cartId;
    }
}

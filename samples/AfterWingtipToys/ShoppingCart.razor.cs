using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys;

public partial class ShoppingCart : BlazorWebFormsComponents.WebFormsPageBase
{
    [Inject] public CartStore CartStore { get; set; } = default!;
    [Inject] public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    protected IReadOnlyList<CartItem> CartItems { get; private set; } = [];
    protected decimal CartTotal { get; private set; }
    protected bool HasItems => CartItems.Count > 0;
    protected string ShoppingCartTitle => HasItems ? "Shopping Cart" : "Shopping Cart is Empty";

    protected override void OnParametersSet()
    {
        var cartKey = GetOrCreateCartKey();
        var httpRequest = HttpContextAccessor.HttpContext?.Request;

        if (httpRequest?.HasFormContentType == true)
        {
            var form = httpRequest.Form;
            var removeProductId = form["removeProductId"].ToString();
            if (int.TryParse(removeProductId, out var productIdToRemove))
            {
                CartStore.RemoveItem(cartKey, productIdToRemove);
            }
            else if (string.Equals(form["action"].ToString(), "Update", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var item in CartStore.GetCartItems(cartKey))
                {
                    var quantityValue = form[$"quantity-{item.ProductId}"].ToString();
                    if (int.TryParse(quantityValue, out var quantity))
                    {
                        CartStore.UpdateQuantity(cartKey, item.ProductId, quantity);
                    }
                }
            }
        }

        CartItems = CartStore.GetCartItems(cartKey);
        CartTotal = CartStore.GetTotal(cartKey);
        base.OnParametersSet();
    }

    protected string GetOrCreateCartKey()
    {
        var cartKey = Session["cart-key"]?.ToString() ?? HttpContextAccessor.HttpContext?.Session.GetString("cart-key");
        if (string.IsNullOrWhiteSpace(cartKey))
        {
            cartKey = Guid.NewGuid().ToString();
            Session["cart-key"] = cartKey;
            HttpContextAccessor.HttpContext?.Session.SetString("cart-key", cartKey);
        }

        return cartKey;
    }
}
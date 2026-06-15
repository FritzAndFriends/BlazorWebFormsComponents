using System.Collections.Concurrent;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public class ShoppingCartActions : IDisposable
{
    private static readonly ConcurrentDictionary<string, List<CartItem>> Carts = new();

    public string ShoppingCartId { get; set; } = "legacy-cart";

    public const string CartSessionKey = "CartId";

    public void AddToCart(int id)
    {
        var cart = GetCartItems();
        var existing = cart.FirstOrDefault(item => item.ProductId == id);
        if (existing is null)
        {
            cart.Add(new CartItem
            {
                ItemId = Guid.NewGuid().ToString("N"),
                CartId = ShoppingCartId,
                ProductId = id,
                Quantity = 1,
                DateCreated = DateTime.UtcNow
            });
        }
        else
        {
            existing.Quantity++;
        }

        Carts[ShoppingCartId] = cart;
    }

    public string GetCartId() => ShoppingCartId;

    public List<CartItem> GetCartItems()
    {
        return Carts.GetOrAdd(ShoppingCartId, _ => []);
    }

    public decimal GetTotal() => GetCartItems().Sum(item => Convert.ToDecimal(item.Product?.UnitPrice ?? 0) * item.Quantity);

    public ShoppingCartActions GetCart()
    {
      
        this.ShoppingCartId = this.GetCartId();
        return this;
      
    }

    public void UpdateShoppingCartDatabase(string cartId, ShoppingCartUpdates[] cartItemUpdates)
    {
        var items = GetCartItems();
        foreach (var update in cartItemUpdates)
        {
            var item = items.FirstOrDefault(existing => existing.ProductId == update.ProductId);
            if (item is null)
            {
                continue;
            }

            if (update.RemoveItem || update.PurchaseQuantity < 1)
            {
                items.Remove(item);
            }
            else
            {
                item.Quantity = update.PurchaseQuantity;
            }
        }

        Carts[cartId] = items;
    }

    public void RemoveItem(string removeCartId, int removeProductId)
    {
        var items = Carts.GetOrAdd(removeCartId, _ => []);
        items.RemoveAll(item => item.ProductId == removeProductId);
    }

    public void UpdateItem(string updateCartId, int updateProductId, int quantity)
    {
        var item = Carts.GetOrAdd(updateCartId, _ => []).FirstOrDefault(existing => existing.ProductId == updateProductId);
        if (item is not null)
        {
            item.Quantity = quantity;
        }
    }

    public void EmptyCart()
    {
        Carts[ShoppingCartId] = [];
    }

    public int GetCount() => GetCartItems().Sum(item => item.Quantity);

      // Get the count of each item in the cart and sum them up          
      int? count = (from cartItems in _db.ShoppingCartItems
                    where cartItems.CartId == ShoppingCartId
                    select (int?)cartItems.Quantity).Sum();
      // Return 0 if all entries are null         
      return count ?? 0;
    }

    public struct ShoppingCartUpdates
    {
        public int ProductId;
        public int PurchaseQuantity;
        public bool RemoveItem;
    }
}

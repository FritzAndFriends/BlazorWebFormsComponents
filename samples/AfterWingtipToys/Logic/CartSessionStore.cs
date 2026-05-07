using System.Collections.Concurrent;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public sealed class CartSessionStore
{
    private readonly ConcurrentDictionary<string, List<CartItem>> _carts = new();

    public IReadOnlyList<CartItem> GetCartItems(string sessionId)
        => GetCart(sessionId)
            .Select(Clone)
            .ToList();

    public void AddToCart(string sessionId, Product product)
    {
        var cart = GetCart(sessionId);
        lock (cart)
        {
            var existing = cart.FirstOrDefault(item => item.ProductId == product.ProductID);
            if (existing is null)
            {
                cart.Add(new CartItem
                {
                    ItemId = Guid.NewGuid().ToString("N"),
                    CartId = sessionId,
                    ProductId = product.ProductID,
                    Product = product,
                    Quantity = 1,
                    DateCreated = DateTime.UtcNow
                });
            }
            else
            {
                existing.Quantity += 1;
            }
        }
    }

    public void UpdateQuantity(string sessionId, int productId, int quantity)
    {
        var cart = GetCart(sessionId);
        lock (cart)
        {
            var existing = cart.FirstOrDefault(item => item.ProductId == productId);
            if (existing is null)
            {
                return;
            }

            if (quantity <= 0)
            {
                cart.Remove(existing);
                return;
            }

            existing.Quantity = quantity;
        }
    }

    public void RemoveItem(string sessionId, int productId)
    {
        var cart = GetCart(sessionId);
        lock (cart)
        {
            cart.RemoveAll(item => item.ProductId == productId);
        }
    }

    public int GetCount(string sessionId)
        => GetCart(sessionId).Sum(item => item.Quantity);

    public decimal GetTotal(string sessionId)
        => GetCart(sessionId).Sum(item => (decimal)(item.Product.UnitPrice ?? 0) * item.Quantity);

    private List<CartItem> GetCart(string sessionId)
        => _carts.GetOrAdd(sessionId, _ => new List<CartItem>());

    private static CartItem Clone(CartItem item)
        => new()
        {
            ItemId = item.ItemId,
            CartId = item.CartId,
            ProductId = item.ProductId,
            Product = item.Product,
            Quantity = item.Quantity,
            DateCreated = item.DateCreated
        };
}

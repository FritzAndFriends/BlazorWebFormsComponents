using System.Collections.Concurrent;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public class CartStore
{
    private readonly ConcurrentDictionary<string, List<CartItem>> _carts = new();

    public List<CartItem> GetCart(string sessionId)
    {
        return _carts.GetOrAdd(sessionId, _ => []);
    }

    public void AddItem(string sessionId, Product product)
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
                existing.Quantity++;
            }
        }
    }

    public void UpdateCart(string sessionId, List<CartItem> items)
    {
        _carts[sessionId] = items;
    }

    public int Count(string sessionId)
    {
        var cart = GetCart(sessionId);
        lock (cart)
        {
            return cart.Sum(item => item.Quantity);
        }
    }

    public decimal Total(string sessionId)
    {
        var cart = GetCart(sessionId);
        lock (cart)
        {
            return cart.Sum(item => Convert.ToDecimal(item.Product?.UnitPrice ?? 0) * item.Quantity);
        }
    }
}

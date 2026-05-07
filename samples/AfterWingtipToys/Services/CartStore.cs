using System.Collections.Concurrent;
using WingtipToys.Models;

namespace WingtipToys.Services;

public class CartStore
{
    private readonly ConcurrentDictionary<string, List<CartItem>> _carts = new();

    public IReadOnlyList<CartItem> GetCartItems(string cartKey)
    {
        if (!_carts.TryGetValue(cartKey, out var items))
        {
            return [];
        }

        lock (items)
        {
            return items
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
        }
    }

    public void AddToCart(string cartKey, int productId, ProductCatalogService catalog)
    {
        var product = catalog.GetProduct(productId, null);
        if (product is null)
        {
            return;
        }

        var items = _carts.GetOrAdd(cartKey, _ => []);
        lock (items)
        {
            var existing = items.FirstOrDefault(item => item.ProductId == productId);
            if (existing is null)
            {
                items.Add(new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    CartId = cartKey,
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

    public void UpdateQuantity(string cartKey, int productId, int quantity)
    {
        if (!_carts.TryGetValue(cartKey, out var items))
        {
            return;
        }

        lock (items)
        {
            var existing = items.FirstOrDefault(item => item.ProductId == productId);
            if (existing is null)
            {
                return;
            }

            if (quantity <= 0)
            {
                items.Remove(existing);
            }
            else
            {
                existing.Quantity = quantity;
            }
        }
    }

    public void RemoveItem(string cartKey, int productId)
    {
        UpdateQuantity(cartKey, productId, 0);
    }

    public decimal GetTotal(string cartKey) => GetCartItems(cartKey).Sum(item => (decimal)(item.Product?.UnitPrice ?? 0) * item.Quantity);

    public int GetCount(string cartKey) => GetCartItems(cartKey).Sum(item => item.Quantity);
}

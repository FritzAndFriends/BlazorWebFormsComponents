using WingtipToys.Models;

namespace WingtipToys.Services;

public class CartStateService
{
    private readonly List<CartItem> _items = new();
    private readonly string _cartId = Guid.NewGuid().ToString();

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    public void AddItem(Product product)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == product.ProductID);
        if (existing != null)
        {
            existing.Quantity++;
        }
        else
        {
            _items.Add(new CartItem
            {
                ItemId = Guid.NewGuid().ToString(),
                CartId = _cartId,
                ProductId = product.ProductID,
                Product = product,
                Quantity = 1,
                DateCreated = DateTime.Now
            });
        }
    }

    public void RemoveItem(string itemId)
    {
        var item = _items.FirstOrDefault(i => i.ItemId == itemId);
        if (item != null) _items.Remove(item);
    }

    public decimal GetTotal() =>
        _items.Sum(i => (decimal)(i.Product?.UnitPrice ?? 0) * i.Quantity);

    public void UpdateQuantity(string itemId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ItemId == itemId);
        if (item != null) item.Quantity = quantity;
    }

    public int GetCount() => _items.Sum(i => i.Quantity);
}

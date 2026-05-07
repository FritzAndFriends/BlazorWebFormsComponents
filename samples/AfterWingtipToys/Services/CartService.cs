using WingtipToys.Models;

namespace WingtipToys.Services;

public sealed class CartService
{
    private readonly object _sync = new();
    private readonly Dictionary<string, List<CartItem>> _carts = new(StringComparer.Ordinal);
    private readonly CatalogService _catalogService;

    public CartService(CatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    public IReadOnlyList<CartItem> GetItems(string cartId)
    {
        lock (_sync)
        {
            return GetCart(cartId)
                .Select(item => new CartItem
                {
                    ItemId = item.ItemId,
                    CartId = item.CartId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    DateCreated = item.DateCreated,
                    Product = item.Product
                })
                .ToList();
        }
    }

    public void AddToCart(string cartId, int productId)
    {
        var product = _catalogService.GetProduct(productId);
        if (product is null)
        {
            return;
        }

        lock (_sync)
        {
            var cart = GetCart(cartId);
            var existing = cart.FirstOrDefault(item => item.ProductId == productId);
            if (existing is null)
            {
                cart.Add(new CartItem
                {
                    ItemId = Guid.NewGuid().ToString("N"),
                    CartId = cartId,
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

    public void UpdateQuantity(string cartId, int productId, int quantity)
    {
        lock (_sync)
        {
            var cart = GetCart(cartId);
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

    public void RemoveFromCart(string cartId, int productId)
    {
        lock (_sync)
        {
            var cart = GetCart(cartId);
            cart.RemoveAll(item => item.ProductId == productId);
        }
    }

    public int GetCount(string cartId) => GetItems(cartId).Sum(item => item.Quantity);

    public decimal GetTotal(string cartId)
        => GetItems(cartId).Sum(item => (decimal)(item.Product?.UnitPrice ?? 0) * item.Quantity);

    private List<CartItem> GetCart(string cartId)
    {
        if (!_carts.TryGetValue(cartId, out var cart))
        {
            cart = new List<CartItem>();
            _carts[cartId] = cart;
        }

        return cart;
    }
}

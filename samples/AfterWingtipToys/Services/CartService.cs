using BlazorWebFormsComponents;
using WingtipToys.Models;

namespace WingtipToys.Services;

public class CartService(SessionShim session, CatalogService catalog)
{
    private const string CartSessionKey = "WingtipCart";

    public void AddItem(int productId)
    {
        var cart = LoadCart();
        var existing = cart.FirstOrDefault(line => line.ProductId == productId);

        if (existing is null)
        {
            cart.Add(new CartLine { ProductId = productId, Quantity = 1 });
        }
        else
        {
            existing.Quantity++;
        }

        SaveCart(cart);
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        if (quantity < 1)
        {
            RemoveItem(productId);
            return;
        }

        var cart = LoadCart();
        var existing = cart.FirstOrDefault(line => line.ProductId == productId);
        if (existing is null)
        {
            return;
        }

        existing.Quantity = quantity;
        SaveCart(cart);
    }

    public void RemoveItem(int productId)
    {
        var cart = LoadCart();
        cart.RemoveAll(line => line.ProductId == productId);
        SaveCart(cart);
    }

    public IReadOnlyList<CartItem> GetItems()
    {
        return LoadCart()
            .Select(line =>
            {
                var product = catalog.GetProduct(line.ProductId, null);
                return new CartItem
                {
                    ItemId = $"item-{line.ProductId}",
                    CartId = "WingtipCart",
                    ProductId = line.ProductId,
                    Product = product!,
                    Quantity = line.Quantity,
                    DateCreated = DateTime.UtcNow
                };
            })
            .Where(item => item.Product is not null)
            .ToList();
    }

    public int GetCount() => LoadCart().Sum(line => line.Quantity);

    public decimal GetTotal()
    {
        return GetItems().Sum(item => (decimal)(item.Product?.UnitPrice ?? 0d) * item.Quantity);
    }

    private List<CartLine> LoadCart()
    {
        return session.Get<List<CartLine>>(CartSessionKey) ?? [];
    }

    private void SaveCart(List<CartLine> cart)
    {
        session[CartSessionKey] = cart;
    }

    private sealed class CartLine
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

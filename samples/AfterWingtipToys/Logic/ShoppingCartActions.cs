using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public class ShoppingCartActions : IDisposable
{
    private readonly ProductContext _db = new();

    public string ShoppingCartId { get; private set; } = "guest";

    public const string CartSessionKey = "CartId";

    public void AddToCart(int id)
    {
        var product = _db.Products.SingleOrDefault(p => p.ProductID == id);
        if (product is null)
        {
            return;
        }

        var cartItem = _db.ShoppingCartItems
            .Include(c => c.Product)
            .SingleOrDefault(c => c.CartId == ShoppingCartId && c.ProductId == id);

        if (cartItem is null)
        {
            cartItem = new CartItem
            {
                ItemId = Guid.NewGuid().ToString(),
                ProductId = id,
                CartId = ShoppingCartId,
                Product = product,
                Quantity = 1,
                DateCreated = DateTime.UtcNow
            };
            _db.ShoppingCartItems.Add(cartItem);
        }
        else
        {
            cartItem.Quantity++;
        }

        _db.SaveChanges();
    }

    public string GetCartId() => ShoppingCartId;

    public List<CartItem> GetCartItems() => _db.ShoppingCartItems
        .Include(c => c.Product)
        .Where(c => c.CartId == ShoppingCartId)
        .OrderBy(c => c.ProductId)
        .ToList();

    public decimal GetTotal() => GetCartItems().Sum(item => (decimal)((item.Product?.UnitPrice ?? 0) * item.Quantity));

    public void UpdateShoppingCartDatabase(string cartId, ShoppingCartUpdates[] cartItemUpdates)
    {
        foreach (var update in cartItemUpdates)
        {
            if (update.RemoveItem || update.PurchaseQuantity < 1)
            {
                RemoveItem(cartId, update.ProductId);
            }
            else
            {
                UpdateItem(cartId, update.ProductId, update.PurchaseQuantity);
            }
        }
    }

    public void RemoveItem(string removeCartId, int removeProductId)
    {
        var item = _db.ShoppingCartItems.SingleOrDefault(c => c.CartId == removeCartId && c.ProductId == removeProductId);
        if (item is null)
        {
            return;
        }

        _db.ShoppingCartItems.Remove(item);
        _db.SaveChanges();
    }

    public void UpdateItem(string updateCartId, int updateProductId, int quantity)
    {
        var item = _db.ShoppingCartItems.SingleOrDefault(c => c.CartId == updateCartId && c.ProductId == updateProductId);
        if (item is null)
        {
            return;
        }

        item.Quantity = quantity;
        _db.SaveChanges();
    }

    public void EmptyCart()
    {
        var items = _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId).ToList();
        if (items.Count == 0)
        {
            return;
        }

        _db.ShoppingCartItems.RemoveRange(items);
        _db.SaveChanges();
    }

    public int GetCount() => _db.ShoppingCartItems
        .Where(c => c.CartId == ShoppingCartId)
        .Sum(c => (int?)c.Quantity) ?? 0;

    public void MigrateCart(string cartId, string userName)
    {
        var shoppingCart = _db.ShoppingCartItems.Where(c => c.CartId == cartId).ToList();
        foreach (var item in shoppingCart)
        {
            item.CartId = userName;
        }

        ShoppingCartId = userName;
        _db.SaveChanges();
    }

    public void Dispose() => _db.Dispose();

    public struct ShoppingCartUpdates
    {
        public int ProductId;
        public int PurchaseQuantity;
        public bool RemoveItem;
    }
}

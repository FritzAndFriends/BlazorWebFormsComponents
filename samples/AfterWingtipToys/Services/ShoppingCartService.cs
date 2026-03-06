using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys.Services;

public class ShoppingCartService
{
    private readonly ProductContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string? _cartId;

    public ShoppingCartService(ProductContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCartId()
    {
        if (_cartId != null) return _cartId;

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return "";

        var session = httpContext.Session;
        if (string.IsNullOrWhiteSpace(session.GetString("CartId")))
        {
            var id = Guid.NewGuid().ToString();
            session.SetString("CartId", id);
        }
        _cartId = session.GetString("CartId")!;
        return _cartId;
    }

    public void AddToCart(int productId)
    {
        var cartId = GetCartId();
        var cartItem = _db.ShoppingCartItems
            .FirstOrDefault(c => c.CartId == cartId && c.ProductId == productId);

        if (cartItem == null)
        {
            cartItem = new CartItem
            {
                CartId = cartId,
                ProductId = productId,
                Quantity = 1,
                DateCreated = DateTime.Now
            };
            _db.ShoppingCartItems.Add(cartItem);
        }
        else
        {
            cartItem.Quantity++;
        }
        _db.SaveChanges();
    }

    public List<CartItem> GetCartItems()
    {
        var cartId = GetCartId();
        return _db.ShoppingCartItems
            .Include(c => c.Product)
            .Where(c => c.CartId == cartId)
            .ToList();
    }

    public decimal GetTotal()
    {
        var cartId = GetCartId();
        return _db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .Select(c => (decimal)(c.Product!.UnitPrice ?? 0) * c.Quantity)
            .Sum();
    }

    public int GetCount()
    {
        var cartId = GetCartId();
        return _db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .Sum(c => c.Quantity);
    }

    public void UpdateItem(string itemId, int quantity)
    {
        var cartId = GetCartId();
        var item = _db.ShoppingCartItems
            .FirstOrDefault(c => c.CartId == cartId && c.ItemId == itemId);
        if (item != null)
        {
            item.Quantity = quantity;
            _db.SaveChanges();
        }
    }

    public void RemoveItem(string itemId)
    {
        var cartId = GetCartId();
        var item = _db.ShoppingCartItems
            .FirstOrDefault(c => c.CartId == cartId && c.ItemId == itemId);
        if (item != null)
        {
            _db.ShoppingCartItems.Remove(item);
            _db.SaveChanges();
        }
    }
}

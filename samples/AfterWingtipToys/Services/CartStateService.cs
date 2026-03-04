using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys.Services;

public class CartStateService
{
    private readonly ProductContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CartSessionKey = "CartId";

    public CartStateService(ProductContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCartId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return string.Empty;

        if (!context.Request.Cookies.ContainsKey(CartSessionKey))
        {
            var cartId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append(CartSessionKey, cartId, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.Now.AddDays(30)
            });
            return cartId;
        }

        return context.Request.Cookies[CartSessionKey] ?? string.Empty;
    }

    public async Task AddToCartAsync(int productId)
    {
        var cartId = GetCartId();
        var cartItem = await _db.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        if (cartItem == null)
        {
            cartItem = new CartItem
            {
                ItemId = Guid.NewGuid().ToString(),
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

        await _db.SaveChangesAsync();
    }

    public async Task<List<CartItem>> GetCartItemsAsync()
    {
        var cartId = GetCartId();
        return await _db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .Include(c => c.Product)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalAsync()
    {
        var cartId = GetCartId();
        var items = await _db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .Include(c => c.Product)
            .ToListAsync();

        return items.Sum(i => (decimal)(i.Product.UnitPrice ?? 0) * i.Quantity);
    }

    public async Task UpdateCartItemAsync(string itemId, int quantity)
    {
        var item = await _db.ShoppingCartItems.FindAsync(itemId);
        if (item != null)
        {
            if (quantity <= 0)
                _db.ShoppingCartItems.Remove(item);
            else
                item.Quantity = quantity;
            await _db.SaveChangesAsync();
        }
    }

    public async Task RemoveCartItemAsync(string itemId)
    {
        var item = await _db.ShoppingCartItems.FindAsync(itemId);
        if (item != null)
        {
            _db.ShoppingCartItems.Remove(item);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<int> GetCartCountAsync()
    {
        var cartId = GetCartId();
        return await _db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .SumAsync(c => c.Quantity);
    }
}

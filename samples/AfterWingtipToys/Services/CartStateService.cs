using WingtipToys.Models;
using Microsoft.EntityFrameworkCore;

namespace WingtipToys.Services;

public class CartStateService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbContextFactory<ProductContext> _dbFactory;
    private const string CartCookieKey = "WingtipToys_CartId";

    public CartStateService(IHttpContextAccessor httpContextAccessor, IDbContextFactory<ProductContext> dbFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbFactory = dbFactory;
    }

    public string GetCartId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return Guid.NewGuid().ToString();

        var cartId = context.Request.Cookies[CartCookieKey];
        if (string.IsNullOrEmpty(cartId))
        {
            cartId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append(CartCookieKey, cartId, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                SameSite = SameSiteMode.Lax,
                IsEssential = true,
            });
        }
        return cartId;
    }

    public async Task AddToCartAsync(int productId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var cartId = GetCartId();
        var existing = await db.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);
        if (existing != null)
        {
            existing.Quantity++;
        }
        else
        {
            db.ShoppingCartItems.Add(new CartItem
            {
                ItemId = Guid.NewGuid().ToString(),
                CartId = cartId,
                ProductId = productId,
                Quantity = 1,
                DateCreated = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync();
    }

    public async Task<List<CartItem>> GetCartItemsAsync()
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var cartId = GetCartId();
        return await db.ShoppingCartItems
            .Include(c => c.Product)
            .Where(c => c.CartId == cartId)
            .ToListAsync();
    }

    public async Task UpdateQuantityAsync(string itemId, int quantity)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var item = await db.ShoppingCartItems.FindAsync(itemId);
        if (item != null)
        {
            item.Quantity = quantity;
            await db.SaveChangesAsync();
        }
    }

    public async Task RemoveItemAsync(string itemId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var item = await db.ShoppingCartItems.FindAsync(itemId);
        if (item != null)
        {
            db.ShoppingCartItems.Remove(item);
            await db.SaveChangesAsync();
        }
    }

    public async Task<int> GetCountAsync()
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var cartId = GetCartId();
        return await db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .SumAsync(c => c.Quantity);
    }
}

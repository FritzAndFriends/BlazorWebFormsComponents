using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys.Services;

public class ShoppingCartService
{
    private readonly IDbContextFactory<ProductContext> _dbFactory;
    private readonly CartStateService _cartState;

    public ShoppingCartService(IDbContextFactory<ProductContext> dbFactory, CartStateService cartState)
    {
        _dbFactory = dbFactory;
        _cartState = cartState;
    }

    public async Task<List<CartItem>> GetCartItemsAsync()
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var cartId = _cartState.CartId;
        return await db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .Include(c => c.Product)
            .ToListAsync();
    }

    public async Task AddToCartAsync(int productId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var cartId = _cartState.CartId;

        var item = await db.ShoppingCartItems
            .SingleOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        if (item == null)
        {
            item = new CartItem
            {
                ItemId = Guid.NewGuid().ToString(),
                ProductId = productId,
                CartId = cartId,
                Quantity = 1,
                DateCreated = DateTime.Now
            };
            db.ShoppingCartItems.Add(item);
        }
        else
        {
            item.Quantity++;
        }

        await db.SaveChangesAsync();
    }

    public async Task UpdateCartItemAsync(int productId, int quantity)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var cartId = _cartState.CartId;
        var item = await db.ShoppingCartItems
            .SingleOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        if (item != null)
        {
            item.Quantity = quantity;
            await db.SaveChangesAsync();
        }
    }

    public async Task RemoveCartItemAsync(int productId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var cartId = _cartState.CartId;
        var item = await db.ShoppingCartItems
            .SingleOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        if (item != null)
        {
            db.ShoppingCartItems.Remove(item);
            await db.SaveChangesAsync();
        }
    }

    public async Task<decimal> GetTotalAsync()
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var cartId = _cartState.CartId;
        var total = await db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .SumAsync(c => (decimal)(c.Quantity * (c.Product.UnitPrice ?? 0)));
        return total;
    }

    public async Task<int> GetCountAsync()
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var cartId = _cartState.CartId;
        return await db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .SumAsync(c => c.Quantity);
    }
}

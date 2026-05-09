using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public sealed class ShoppingCartService(
    IDbContextFactory<ProductContext> dbFactory,
    IHttpContextAccessor httpContextAccessor,
    SessionShim sessionShim)
{
    public const string CartSessionKey = "CartId";

    public string GetCartId()
    {
        var cartId = httpContextAccessor.HttpContext?.Session.GetString(CartSessionKey)
            ?? sessionShim.Get<string>(CartSessionKey);

        if (!string.IsNullOrWhiteSpace(cartId))
        {
            return cartId;
        }

        cartId = httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true
            ? httpContextAccessor.HttpContext.User.Identity.Name
            : Guid.NewGuid().ToString("N");

        if (string.IsNullOrWhiteSpace(cartId))
        {
            cartId = Guid.NewGuid().ToString("N");
        }

        httpContextAccessor.HttpContext?.Session.SetString(CartSessionKey, cartId);
        sessionShim[CartSessionKey] = cartId;
        return cartId;
    }

    public async Task<List<CartItem>> GetCartItemsAsync()
    {
        var cartId = GetCartId();
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.ShoppingCartItems
            .Include(item => item.Product)
            .Where(item => item.CartId == cartId)
            .OrderBy(item => item.ProductId)
            .ToListAsync();
    }

    public async Task AddToCartAsync(int productId)
    {
        var cartId = GetCartId();
        await using var db = await dbFactory.CreateDbContextAsync();

        var cartItem = await db.ShoppingCartItems
            .Include(item => item.Product)
            .SingleOrDefaultAsync(item => item.CartId == cartId && item.ProductId == productId);

        if (cartItem is null)
        {
            var product = await db.Products.FirstOrDefaultAsync(item => item.ProductID == productId);
            if (product is null)
            {
                return;
            }

            cartItem = new CartItem
            {
                ItemId = Guid.NewGuid().ToString("N"),
                CartId = cartId,
                ProductId = productId,
                Product = product,
                Quantity = 1,
                DateCreated = DateTime.UtcNow
            };

            db.ShoppingCartItems.Add(cartItem);
        }
        else
        {
            cartItem.Quantity++;
        }

        await db.SaveChangesAsync();
    }

    public async Task UpdateQuantitiesAsync(IEnumerable<CartItem> cartItems)
    {
        await UpdateQuantitiesByProductAsync(cartItems.Select(item => (item.ProductId, item.Quantity)));
    }

    public async Task UpdateQuantitiesByProductAsync(IEnumerable<(int ProductId, int Quantity)> updates)
    {
        var cartId = GetCartId();
        await using var db = await dbFactory.CreateDbContextAsync();
        var existingItems = await db.ShoppingCartItems
            .Where(item => item.CartId == cartId)
            .ToListAsync();

        foreach (var update in updates)
        {
            var existingItem = existingItems.FirstOrDefault(current => current.ProductId == update.ProductId);
            if (existingItem is null)
            {
                continue;
            }

            if (update.Quantity < 1)
            {
                db.ShoppingCartItems.Remove(existingItem);
            }
            else
            {
                existingItem.Quantity = update.Quantity;
            }
        }

        await db.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(int productId)
    {
        var cartId = GetCartId();
        await using var db = await dbFactory.CreateDbContextAsync();
        var cartItem = await db.ShoppingCartItems
            .FirstOrDefaultAsync(item => item.CartId == cartId && item.ProductId == productId);

        if (cartItem is null)
        {
            return;
        }

        db.ShoppingCartItems.Remove(cartItem);
        await db.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalAsync()
    {
        var cartId = GetCartId();
        await using var db = await dbFactory.CreateDbContextAsync();
        var items = await db.ShoppingCartItems
            .Include(item => item.Product)
            .Where(item => item.CartId == cartId)
            .ToListAsync();

        return items.Sum(item => Convert.ToDecimal(item.Product?.UnitPrice ?? 0D) * item.Quantity);
    }

    public async Task<int> GetCountAsync()
    {
        var cartId = GetCartId();
        await using var db = await dbFactory.CreateDbContextAsync();
        return await db.ShoppingCartItems
            .Where(item => item.CartId == cartId)
            .SumAsync(item => (int?)item.Quantity) ?? 0;
    }
}

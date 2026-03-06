using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys.Services;

public class ShoppingCartService : IShoppingCartService
{
    private readonly WingtipDbContext _db;
    private readonly ProtectedSessionStorage _sessionStorage;
    private const string CartSessionKey = "CartId";

    public ShoppingCartService(WingtipDbContext db, ProtectedSessionStorage sessionStorage)
    {
        _db = db;
        _sessionStorage = sessionStorage;
    }

    public async Task<string> GetCartIdAsync()
    {
        var result = await _sessionStorage.GetAsync<string>(CartSessionKey);
        
        if (!result.Success || string.IsNullOrEmpty(result.Value))
        {
            var cartId = Guid.NewGuid().ToString();
            await _sessionStorage.SetAsync(CartSessionKey, cartId);
            return cartId;
        }
        
        return result.Value;
    }

    public async Task AddToCartAsync(int productId)
    {
        var cartId = await GetCartIdAsync();

        var cartItem = await _db.ShoppingCartItems
            .SingleOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        if (cartItem == null)
        {
            var product = await _db.Products.SingleOrDefaultAsync(p => p.ProductID == productId);
            
            cartItem = new CartItem
            {
                ItemId = Guid.NewGuid().ToString(),
                ProductId = productId,
                CartId = cartId,
                Product = product,
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
        var cartId = await GetCartIdAsync();

        return await _db.ShoppingCartItems
            .Include(c => c.Product)
            .Where(c => c.CartId == cartId)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalAsync()
    {
        var cartId = await GetCartIdAsync();

        var total = await _db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .Select(c => c.Quantity * c.Product!.UnitPrice)
            .SumAsync();

        return total ?? 0m;
    }

    public async Task UpdateCartAsync(ShoppingCartUpdates[] updates)
    {
        var cartId = await GetCartIdAsync();
        var cartItems = await GetCartItemsAsync();

        foreach (var cartItem in cartItems)
        {
            var update = updates.FirstOrDefault(u => u.ProductId == cartItem.ProductId);
            if (update.ProductId == cartItem.ProductId)
            {
                if (update.PurchaseQuantity < 1 || update.RemoveItem)
                {
                    await RemoveItemInternalAsync(cartId, cartItem.ProductId);
                }
                else
                {
                    await UpdateItemInternalAsync(cartId, cartItem.ProductId, update.PurchaseQuantity);
                }
            }
        }
    }

    public async Task RemoveItemAsync(int productId)
    {
        var cartId = await GetCartIdAsync();
        await RemoveItemInternalAsync(cartId, productId);
    }

    private async Task RemoveItemInternalAsync(string cartId, int productId)
    {
        var cartItem = await _db.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        if (cartItem != null)
        {
            _db.ShoppingCartItems.Remove(cartItem);
            await _db.SaveChangesAsync();
        }
    }

    private async Task UpdateItemInternalAsync(string cartId, int productId, int quantity)
    {
        var cartItem = await _db.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        if (cartItem != null)
        {
            cartItem.Quantity = quantity;
            await _db.SaveChangesAsync();
        }
    }

    public async Task EmptyCartAsync()
    {
        var cartId = await GetCartIdAsync();
        var cartItems = await _db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .ToListAsync();

        foreach (var cartItem in cartItems)
        {
            _db.ShoppingCartItems.Remove(cartItem);
        }

        await _db.SaveChangesAsync();
    }

    public async Task<int> GetCountAsync()
    {
        var cartId = await GetCartIdAsync();

        var count = await _db.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .Select(c => (int?)c.Quantity)
            .SumAsync();

        return count ?? 0;
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public class ShoppingCartActions
{
    private readonly IDbContextFactory<ProductContext> _dbContextFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ShoppingCartActions(IDbContextFactory<ProductContext> dbContextFactory, IHttpContextAccessor httpContextAccessor)
    {
        _dbContextFactory = dbContextFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public const string CartSessionKey = "CartId";
    public string ShoppingCartId { get; private set; } = string.Empty;

    public string GetCartId()
    {
        if (!string.IsNullOrWhiteSpace(ShoppingCartId))
        {
            return ShoppingCartId;
        }

        var context = _httpContextAccessor.HttpContext;
        var session = context?.Session;
        var cartId = session?.GetString(CartSessionKey);

        if (string.IsNullOrWhiteSpace(cartId))
        {
            cartId = !string.IsNullOrWhiteSpace(context?.User?.Identity?.Name)
                ? context!.User.Identity!.Name!
                : Guid.NewGuid().ToString();

            session?.SetString(CartSessionKey, cartId);
        }

        ShoppingCartId = cartId;
        return ShoppingCartId;
    }

    public void AddToCart(int id)
    {
        using var db = _dbContextFactory.CreateDbContext();
        var cartId = GetCartId();

        var cartItem = db.ShoppingCartItems.SingleOrDefault(
            c => c.CartId == cartId && c.ProductId == id);
        if (cartItem == null)
        {
            cartItem = new CartItem
            {
                ItemId = Guid.NewGuid().ToString(),
                ProductId = id,
                CartId = cartId,
                Product = db.Products.SingleOrDefault(p => p.ProductID == id),
                Quantity = 1,
                DateCreated = DateTime.Now
            };
            db.ShoppingCartItems.Add(cartItem);
        }
        else
        {
            cartItem.Quantity++;
        }

        db.SaveChanges();
    }

    public List<CartItem> GetCartItems()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var cartId = GetCartId();

        return db.ShoppingCartItems
            .Include(c => c.Product)
            .Where(c => c.CartId == cartId)
            .AsNoTracking()
            .ToList();
    }

    public decimal GetTotal()
    {
        using var db = _dbContextFactory.CreateDbContext();
        var cartId = GetCartId();

        return db.ShoppingCartItems
            .Include(c => c.Product)
            .Where(c => c.CartId == cartId)
            .Select(c => (decimal?)(c.Quantity * (decimal)(c.Product.UnitPrice ?? 0d)))
            .Sum() ?? decimal.Zero;
    }

    public void UpdateShoppingCartDatabase(string cartId, ShoppingCartUpdates[] cartItemUpdates)
    {
        using var db = _dbContextFactory.CreateDbContext();

        foreach (var cartItemUpdate in cartItemUpdates)
        {
            var cartItem = db.ShoppingCartItems.SingleOrDefault(c => c.CartId == cartId && c.ProductId == cartItemUpdate.ProductId);
            if (cartItem is null)
            {
                continue;
            }

            if (cartItemUpdate.RemoveItem || cartItemUpdate.PurchaseQuantity < 1)
            {
                db.ShoppingCartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = cartItemUpdate.PurchaseQuantity;
            }
        }

        db.SaveChanges();
    }

    public struct ShoppingCartUpdates
    {
        public int ProductId;
        public int PurchaseQuantity;
        public bool RemoveItem;
    }
}

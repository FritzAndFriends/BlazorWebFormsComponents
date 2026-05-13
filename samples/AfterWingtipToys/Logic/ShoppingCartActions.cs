using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public class ShoppingCartActions : IDisposable
{
    public const string CartSessionKey = "CartId";

    private readonly IDbContextFactory<ProductContext> _dbFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ShoppingCartActions(IDbContextFactory<ProductContext> dbFactory, IHttpContextAccessor httpContextAccessor)
    {
        _dbFactory = dbFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public void AddToCart(int id)
    {
        var cartId = GetCartId();
        using var db = _dbFactory.CreateDbContext();

        var cartItem = db.ShoppingCartItems
            .Include(c => c.Product)
            .SingleOrDefault(c => c.CartId == cartId && c.ProductId == id);

        if (cartItem is null)
        {
            cartItem = new CartItem
            {
                ItemId = Guid.NewGuid().ToString(),
                ProductId = id,
                CartId = cartId,
                Product = db.Products.Single(p => p.ProductID == id),
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

    public string GetCartId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var session = httpContext?.Session;
        if (session is null)
        {
            return Guid.NewGuid().ToString();
        }

        var cartId = session.GetString(CartSessionKey);
        if (string.IsNullOrWhiteSpace(cartId))
        {
            cartId = !string.IsNullOrWhiteSpace(httpContext?.User.Identity?.Name)
                ? httpContext!.User.Identity!.Name!
                : Guid.NewGuid().ToString();
            session.SetString(CartSessionKey, cartId);
        }

        return cartId;
    }

    public List<CartItem> GetCartItems()
    {
        var cartId = GetCartId();
        using var db = _dbFactory.CreateDbContext();
        return db.ShoppingCartItems
            .Include(c => c.Product)
            .Where(c => c.CartId == cartId)
            .OrderBy(c => c.ProductId)
            .ToList();
    }

    public decimal GetTotal()
    {
        var total = GetCartItems().Sum(item => item.Product.UnitPrice * item.Quantity);
        return Convert.ToDecimal(total);
    }

    public void UpdateShoppingCartDatabase(string cartId, ShoppingCartUpdates[] cartItemUpdates)
    {
        using var db = _dbFactory.CreateDbContext();
        foreach (var cartUpdate in cartItemUpdates)
        {
            var item = db.ShoppingCartItems.SingleOrDefault(c => c.CartId == cartId && c.ProductId == cartUpdate.ProductId);
            if (item is null)
            {
                continue;
            }

            if (cartUpdate.RemoveItem || cartUpdate.PurchaseQuantity < 1)
            {
                db.ShoppingCartItems.Remove(item);
            }
            else
            {
                item.Quantity = cartUpdate.PurchaseQuantity;
            }
        }

        db.SaveChanges();
    }

    public void RemoveItem(string removeCartId, int removeProductId)
    {
        using var db = _dbFactory.CreateDbContext();
        var myItem = db.ShoppingCartItems.SingleOrDefault(c => c.CartId == removeCartId && c.ProductId == removeProductId);
        if (myItem is not null)
        {
            db.ShoppingCartItems.Remove(myItem);
            db.SaveChanges();
        }
    }

    public void UpdateItem(string updateCartId, int updateProductId, int quantity)
    {
        using var db = _dbFactory.CreateDbContext();
        var myItem = db.ShoppingCartItems.SingleOrDefault(c => c.CartId == updateCartId && c.ProductId == updateProductId);
        if (myItem is not null)
        {
            myItem.Quantity = quantity;
            db.SaveChanges();
        }
    }

    public void EmptyCart()
    {
        var cartId = GetCartId();
        using var db = _dbFactory.CreateDbContext();
        var cartItems = db.ShoppingCartItems.Where(c => c.CartId == cartId).ToList();
        if (cartItems.Count == 0)
        {
            return;
        }

        db.ShoppingCartItems.RemoveRange(cartItems);
        db.SaveChanges();
    }

    public int GetCount()
    {
        return GetCartItems().Sum(item => item.Quantity);
    }

    public void Dispose()
    {
    }

    public struct ShoppingCartUpdates
    {
        public int ProductId;
        public int PurchaseQuantity;
        public bool RemoveItem;
    }
}

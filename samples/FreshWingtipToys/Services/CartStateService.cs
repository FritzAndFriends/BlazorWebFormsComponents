using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys.Services
{
    public class CartStateService
    {
        private readonly IDbContextFactory<ProductContext> _contextFactory;
        private readonly string _cartId;

        public CartStateService(IDbContextFactory<ProductContext> contextFactory, IHttpContextAccessor httpContextAccessor)
        {
            _contextFactory = contextFactory;
            // Use a cookie to persist cart ID across page navigations
            var httpContext = httpContextAccessor.HttpContext;
            var cookieCartId = httpContext?.Request.Cookies["WingtipCartId"];
            if (string.IsNullOrEmpty(cookieCartId))
            {
                cookieCartId = Guid.NewGuid().ToString();
                httpContext?.Response.Cookies.Append("WingtipCartId", cookieCartId, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });
            }
            _cartId = cookieCartId;
        }

        public string GetCartId() => _cartId;

        public async Task AddToCartAsync(int productId)
        {
            using var context = _contextFactory.CreateDbContext();
            var existingItem = await context.CartItems
                .FirstOrDefaultAsync(c => c.CartId == _cartId && c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                context.CartItems.Add(new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    CartId = _cartId,
                    ProductId = productId,
                    Quantity = 1,
                    DateCreated = DateTime.UtcNow
                });
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CartId == _cartId)
                .ToListAsync();
        }

        public async Task UpdateCartItemAsync(string cartItemId, int quantity)
        {
            using var context = _contextFactory.CreateDbContext();
            var item = await context.CartItems
                .FirstOrDefaultAsync(c => c.ItemId == cartItemId && c.CartId == _cartId);

            if (item != null)
            {
                item.Quantity = quantity;
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveCartItemAsync(string cartItemId)
        {
            using var context = _contextFactory.CreateDbContext();
            var item = await context.CartItems
                .FirstOrDefaultAsync(c => c.ItemId == cartItemId && c.CartId == _cartId);

            if (item != null)
            {
                context.CartItems.Remove(item);
                await context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetTotalAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.CartItems
                .Where(c => c.CartId == _cartId)
                .Include(c => c.Product)
                .SumAsync(c => c.Quantity * (c.Product!.UnitPrice ?? 0));
        }

        public async Task EmptyCartAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            var items = await context.CartItems
                .Where(c => c.CartId == _cartId)
                .ToListAsync();

            context.CartItems.RemoveRange(items);
            await context.SaveChangesAsync();
        }
    }
}

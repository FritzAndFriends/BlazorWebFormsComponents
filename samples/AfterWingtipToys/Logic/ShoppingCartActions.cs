using WingtipToys.Models;
using Microsoft.EntityFrameworkCore;

namespace WingtipToys.Logic
{
    public class ShoppingCartActions
    {
        private readonly ProductContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public const string CartSessionKey = "CartId";

        public ShoppingCartActions(ProductContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetCartId()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null) return Guid.NewGuid().ToString();

            var cartId = session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartId))
            {
                var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
                cartId = !string.IsNullOrWhiteSpace(userName)
                    ? userName
                    : Guid.NewGuid().ToString();
                session.SetString(CartSessionKey, cartId);
            }
            return cartId;
        }

        public void AddToCart(int id)
        {
            var cartId = GetCartId();
            var cartItem = _db.ShoppingCartItems
                .SingleOrDefault(c => c.CartId == cartId && c.ProductId == id);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = id,
                    CartId = cartId,
                    Product = _db.Products.SingleOrDefault(p => p.ProductID == id),
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
            decimal? total = (from cartItems in _db.ShoppingCartItems.Include(c => c.Product)
                              where cartItems.CartId == cartId
                              select (decimal?)(cartItems.Quantity * cartItems.Product.UnitPrice)).Sum();
            return total ?? decimal.Zero;
        }

        public void UpdateItem(int productId, int quantity)
        {
            var cartId = GetCartId();
            var item = _db.ShoppingCartItems
                .FirstOrDefault(c => c.CartId == cartId && c.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                _db.SaveChanges();
            }
        }

        public void RemoveItem(int productId)
        {
            var cartId = GetCartId();
            var item = _db.ShoppingCartItems
                .FirstOrDefault(c => c.CartId == cartId && c.ProductId == productId);
            if (item != null)
            {
                _db.ShoppingCartItems.Remove(item);
                _db.SaveChanges();
            }
        }

        public int GetCount()
        {
            var cartId = GetCartId();
            int? count = (from cartItems in _db.ShoppingCartItems
                          where cartItems.CartId == cartId
                          select (int?)cartItems.Quantity).Sum();
            return count ?? 0;
        }
    }
}

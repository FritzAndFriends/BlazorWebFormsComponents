using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using WingtipToys.Models;

namespace WingtipToys.Logic
{
    /// <summary>
    /// Singleton store — cart items keyed by stable cart-ID that is persisted
    /// in the ASP.NET Core session cookie so it survives HTTP redirects and
    /// new Blazor circuits.
    /// </summary>
    public class CartSessionStore
    {
        private readonly ConcurrentDictionary<string, Dictionary<int, CartEntry>> _carts = new();

        public Dictionary<int, CartEntry> GetCart(string cartId) =>
            _carts.GetOrAdd(cartId, _ => new Dictionary<int, CartEntry>());

        public void AddToCart(string cartId, Product product)
        {
            var cart = GetCart(cartId);
            if (cart.TryGetValue(product.ProductID, out var existing))
                existing.Quantity++;
            else
                cart[product.ProductID] = new CartEntry { Product = product, Quantity = 1 };
        }

        public void UpdateQuantity(string cartId, int productId, int quantity)
        {
            var cart = GetCart(cartId);
            if (quantity <= 0) cart.Remove(productId);
            else if (cart.TryGetValue(productId, out var entry)) entry.Quantity = quantity;
        }

        public void RemoveItem(string cartId, int productId) => GetCart(cartId).Remove(productId);
        public void Clear(string cartId) => GetCart(cartId).Clear();
        public decimal GetTotal(string cartId) =>
            (decimal)GetCart(cartId).Values.Sum(e => (e.Product.UnitPrice ?? 0) * e.Quantity);
    }

    /// <summary>
    /// Scoped helper — resolves the session-based cart ID once at construction
    /// (during SSR pre-render when HttpContext is available) and delegates all
    /// operations to the singleton CartSessionStore.
    /// </summary>
    public class CartService
    {
        private const string CartIdKey = "WingtipCartId";
        private readonly CartSessionStore _store;
        public readonly string CartId;

        public CartService(CartSessionStore store, IHttpContextAccessor httpContextAccessor)
        {
            _store = store;
            var session = httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var id = session.GetString(CartIdKey);
                if (string.IsNullOrEmpty(id))
                {
                    id = Guid.NewGuid().ToString("N");
                    session.SetString(CartIdKey, id);
                }
                CartId = id;
            }
            else
            {
                // Fallback when HttpContext is unavailable (interactive-only render)
                CartId = Guid.NewGuid().ToString("N");
            }
        }

        public void AddToCart(Product product) => _store.AddToCart(CartId, product);
        public IReadOnlyList<CartEntry> GetItems() => _store.GetCart(CartId).Values.ToList();
        public void UpdateQuantity(int productId, int quantity) => _store.UpdateQuantity(CartId, productId, quantity);
        public void RemoveItem(int productId) => _store.RemoveItem(CartId, productId);
        public void Clear() => _store.Clear(CartId);
        public decimal GetTotal() => _store.GetTotal(CartId);
    }

    public class CartEntry
    {
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; }
    }
}

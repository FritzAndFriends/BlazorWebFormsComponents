using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using WingtipToys.Models;

namespace WingtipToys.Logic
{
    public class ShoppingCartActions : IDisposable
    {
        private readonly string _cartId;
        private readonly ProductContext _db;

        public ShoppingCartActions(IServiceProvider? services = null)
        {
            _db = services?.GetService<ProductContext>() ?? new ProductContext();
            _cartId = Guid.NewGuid().ToString();
        }

        public string CartId => _cartId;

        public void AddToCart(int id)
        {
            var cartItem = _db.ShoppingCartItems
                .FirstOrDefault(c => c.CartId == _cartId && c.ProductId == id);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ItemId = Guid.NewGuid().ToString(),
                    ProductId = id,
                    CartId = _cartId,
                    Product = _db.Products.Find(id)!,
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

        public List<CartItem> GetCartItems() =>
            _db.ShoppingCartItems.Where(c => c.CartId == _cartId).ToList();

        public decimal GetTotal() =>
            _db.ShoppingCartItems
                .Where(c => c.CartId == _cartId)
                .Sum(c => c.Quantity * (decimal)(c.Product.UnitPrice ?? 0));

        public void EmptyCart()
        {
            var items = _db.ShoppingCartItems.Where(c => c.CartId == _cartId);
            _db.ShoppingCartItems.RemoveRange(items);
            _db.SaveChanges();
        }

        public void Dispose() => _db.Dispose();
    }
}

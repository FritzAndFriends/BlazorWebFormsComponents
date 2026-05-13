using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public class ShoppingCartActions : IDisposable
{
    public struct ShoppingCartUpdates
    {
        public int ProductId { get; set; }
        public int PurchaseQuantity { get; set; }
        public bool RemoveItem { get; set; }
    }

    public string ShoppingCartId { get; set; } = string.Empty;
    public const string CartSessionKey = "CartId";

    private readonly ProductContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ShoppingCartActions(ProductContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public void AddToCart(int id)
    {
        ShoppingCartId = GetCartId();

        var cartItem = _db.ShoppingCartItems.SingleOrDefault(
            c => c.CartId == ShoppingCartId && c.ProductId == id);

        if (cartItem == null)
        {
            cartItem = new CartItem
            {
                ItemId = Guid.NewGuid().ToString(),
                ProductId = id,
                CartId = ShoppingCartId,
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

    public void Dispose() { }

    public string GetCartId()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return Guid.NewGuid().ToString();

        var cartId = session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cartId))
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(userName))
            {
                cartId = userName;
            }
            else
            {
                cartId = Guid.NewGuid().ToString();
            }
            session.SetString(CartSessionKey, cartId);
        }
        return cartId;
    }

    public List<CartItem> GetCartItems()
    {
        ShoppingCartId = GetCartId();
        return _db.ShoppingCartItems
            .Include(c => c.Product)
            .Where(c => c.CartId == ShoppingCartId)
            .ToList();
    }

    public decimal GetTotal()
    {
        ShoppingCartId = GetCartId();
        var total = (from cartItems in _db.ShoppingCartItems
                     where cartItems.CartId == ShoppingCartId
                     select (int?)cartItems.Quantity * cartItems.Product.UnitPrice).Sum();
        return (decimal)(total ?? 0);
    }

    public ShoppingCartActions GetCart()
    {
        ShoppingCartId = GetCartId();
        return this;
    }

    public void UpdateShoppingCartDatabase(string cartId, ShoppingCartUpdates[] cartItemUpdates)
    {
        try
        {
            var myCart = GetCartItems();
            foreach (var cartItem in myCart)
            {
                foreach (var update in cartItemUpdates)
                {
                    if (cartItem.ProductId == update.ProductId)
                    {
                        if (update.PurchaseQuantity < 1 || update.RemoveItem)
                        {
                            RemoveItem(cartId, cartItem.ProductId);
                        }
                        else
                        {
                            UpdateItem(cartId, cartItem.ProductId, update.PurchaseQuantity);
                        }
                    }
                }
            }
        }
        catch (Exception exp)
        {
            throw new Exception("ERROR: Unable to Update Cart Database - " + exp.Message, exp);
        }
    }

    public void RemoveItem(string removeCartID, int removeProductID)
    {
        var myItem = _db.ShoppingCartItems
            .FirstOrDefault(c => c.CartId == removeCartID && c.ProductId == removeProductID);
        if (myItem != null)
        {
            _db.ShoppingCartItems.Remove(myItem);
            _db.SaveChanges();
        }
    }

    public void UpdateItem(string updateCartID, int updateProductID, int quantity)
    {
        var myItem = _db.ShoppingCartItems
            .FirstOrDefault(c => c.CartId == updateCartID && c.ProductId == updateProductID);
        if (myItem != null)
        {
            myItem.Quantity = quantity;
            _db.SaveChanges();
        }
    }

    public void EmptyCart()
    {
        ShoppingCartId = GetCartId();
        var cartItems = _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId);
        foreach (var cartItem in cartItems)
        {
            _db.ShoppingCartItems.Remove(cartItem);
        }
        _db.SaveChanges();
    }

    public int GetCount()
    {
        ShoppingCartId = GetCartId();
        int? count = (from cartItems in _db.ShoppingCartItems
                     where cartItems.CartId == ShoppingCartId
                     select (int?)cartItems.Quantity).Sum();
        return count ?? 0;
    }

    public void MigrateCart(string cartId, string userName)
    {
        var shoppingCart = _db.ShoppingCartItems.Where(c => c.CartId == cartId);
        foreach (var item in shoppingCart)
        {
            item.CartId = userName;
        }
        var session = _httpContextAccessor.HttpContext?.Session;
        session?.SetString(CartSessionKey, userName);
        _db.SaveChanges();
    }
}

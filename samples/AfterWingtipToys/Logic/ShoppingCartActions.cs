using System;
using System.Collections.Generic;
using System.Linq;
using WingtipToys.Models;

namespace WingtipToys.Logic
{
  public class ShoppingCartActions : IDisposable
  {
    private static readonly List<CartItem> Items = [];
    private const string DefaultCartId = "default";

    public string ShoppingCartId { get; set; } = DefaultCartId;

    public const string CartSessionKey = "CartId";

    public void AddToCart(int id)
    {
      ShoppingCartId = GetCartId();

      var cartItem = Items.SingleOrDefault(c => c.CartId == ShoppingCartId && c.ProductId == id);
      if (cartItem is null)
      {
        using var db = new ProductContext();
        var product = db.Products.FirstOrDefault(p => p.ProductID == id);
        if (product is null)
        {
          return;
        }

        Items.Add(new CartItem
        {
          ItemId = Guid.NewGuid().ToString(),
          ProductId = id,
          CartId = ShoppingCartId,
          Product = product,
          Quantity = 1,
          DateCreated = DateTime.UtcNow
        });

        return;
      }

      cartItem.Quantity++;
    }

    public void Dispose()
    {
    }

    public string GetCartId() => DefaultCartId;

    public List<CartItem> GetCartItems()
    {
      ShoppingCartId = GetCartId();
      return Items.Where(c => c.CartId == ShoppingCartId).ToList();
    }

    public decimal GetTotal()
    {
      return GetCartItems().Sum(item => (decimal)(item.Product?.UnitPrice ?? 0) * item.Quantity);
    }

    public ShoppingCartActions GetCart(object? context) => this;

    public void UpdateShoppingCartDatabase(string cartId, ShoppingCartUpdates[] cartItemUpdates)
    {
      foreach (var update in cartItemUpdates)
      {
        if (update.RemoveItem || update.PurchaseQuantity < 1)
        {
          RemoveItem(cartId, update.ProductId);
        }
        else
        {
          UpdateItem(cartId, update.ProductId, update.PurchaseQuantity);
        }
      }
    }

    public void RemoveItem(string removeCartId, int removeProductId)
    {
      Items.RemoveAll(item => item.CartId == removeCartId && item.ProductId == removeProductId);
    }

    public void UpdateItem(string updateCartId, int updateProductId, int quantity)
    {
      var item = Items.FirstOrDefault(c => c.CartId == updateCartId && c.ProductId == updateProductId);
      if (item is null)
      {
        return;
      }

      item.Quantity = quantity;
    }

    public void EmptyCart()
    {
      Items.RemoveAll(item => item.CartId == GetCartId());
    }

    public int GetCount()
    {
      return GetCartItems().Sum(item => item.Quantity);
    }

    public struct ShoppingCartUpdates
    {
      public int ProductId;
      public int PurchaseQuantity;
      public bool RemoveItem;
    }

    public void MigrateCart(string cartId, string userName)
    {
      foreach (var item in Items.Where(c => c.CartId == cartId))
      {
        item.CartId = userName;
      }
    }
  }
}

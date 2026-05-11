using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Logic
{
	public class ShoppingCartActions : IDisposable
	{
		public string ShoppingCartId { get; set; } = "";

		private readonly ProductContext _db;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public const string CartSessionKey = "CartId";

		public ShoppingCartActions(ProductContext db, IHttpContextAccessor httpContextAccessor)
		{
			_db = db;
			_httpContextAccessor = httpContextAccessor;
		}

		public void AddToCart(int id)
		{
			ShoppingCartId = GetCartId();

			var cartItem = _db.ShoppingCartItems.SingleOrDefault(
				c => c.CartId == ShoppingCartId
				&& c.ProductId == id);
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

		public void Dispose()
		{
		}

		public string GetCartId()
		{
			var session = _httpContextAccessor.HttpContext?.Session;
			if (session == null) return "";

			var cartId = session.GetString(CartSessionKey);
			if (string.IsNullOrEmpty(cartId))
			{
				var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
				if (!string.IsNullOrWhiteSpace(userName))
				{
					session.SetString(CartSessionKey, userName);
					cartId = userName;
				}
				else
				{
					var tempCartId = Guid.NewGuid();
					session.SetString(CartSessionKey, tempCartId.ToString());
					cartId = tempCartId.ToString();
				}
			}
			return cartId;
		}

		public List<CartItem> GetCartItems()
		{
			ShoppingCartId = GetCartId();

			return _db.ShoppingCartItems
				.Include(c => c.Product)
				.Where(c => c.CartId == ShoppingCartId).ToList();
		}

		public decimal GetTotal()
		{
			ShoppingCartId = GetCartId();
			decimal? total = (decimal?)(from cartItems in _db.ShoppingCartItems
							  where cartItems.CartId == ShoppingCartId
							  select (int?)cartItems.Quantity *
							  cartItems.Product.UnitPrice).Sum();
			return total ?? decimal.Zero;
		}

		public int GetCount()
		{
			ShoppingCartId = GetCartId();
			int? count = (from cartItems in _db.ShoppingCartItems
						  where cartItems.CartId == ShoppingCartId
						  select (int?)cartItems.Quantity).Sum();
			return count ?? 0;
		}

		public void RemoveItem(string removeCartID, int removeProductID)
		{
			var myItem = _db.ShoppingCartItems
				.FirstOrDefault(c => c.CartId == removeCartID && c.Product.ProductID == removeProductID);
			if (myItem != null)
			{
				_db.ShoppingCartItems.Remove(myItem);
				_db.SaveChanges();
			}
		}

		public void UpdateItem(string updateCartID, int updateProductID, int quantity)
		{
			var myItem = _db.ShoppingCartItems
				.FirstOrDefault(c => c.CartId == updateCartID && c.Product.ProductID == updateProductID);
			if (myItem != null)
			{
				myItem.Quantity = quantity;
				_db.SaveChanges();
			}
		}

		public struct ShoppingCartUpdates
		{
			public int ProductId;
			public int PurchaseQuantity;
			public bool RemoveItem;
		}
	}
}

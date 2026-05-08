using Microsoft.AspNetCore.Http;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public class ShoppingCartActions : IDisposable
{
	private static string? _fallbackCartId;
	private readonly ProductContext _db = new();

	public const string CartSessionKey = "CartId";

	public string ShoppingCartId { get; set; } = string.Empty;

	public void AddToCart(int id)
	{
		ShoppingCartId = GetCartId();

		var cartItem = _db.ShoppingCartItems.SingleOrDefault(c => c.CartId == ShoppingCartId && c.ProductId == id);
		if (cartItem is null)
		{
			cartItem = new CartItem
			{
				ItemId = Guid.NewGuid().ToString(),
				ProductId = id,
				CartId = ShoppingCartId,
				Product = _db.Products.SingleOrDefault(p => p.ProductID == id)!,
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

	public string GetCartId()
	{
		var accessor = new HttpContextAccessor();
		var session = accessor.HttpContext?.Session;
		var existingCartId = session?.GetString(CartSessionKey) ?? _fallbackCartId;
		if (!string.IsNullOrWhiteSpace(existingCartId))
		{
			return existingCartId;
		}

		var userName = accessor.HttpContext?.User?.Identity?.Name;
		var cartId = !string.IsNullOrWhiteSpace(userName) ? userName : Guid.NewGuid().ToString();
		if (session is not null)
		{
			session.SetString(CartSessionKey, cartId);
		}
		else
		{
			_fallbackCartId = cartId;
		}

		return cartId;
	}

	public List<CartItem> GetCartItems()
	{
		ShoppingCartId = GetCartId();
		return _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId).ToList();
	}

	public decimal GetTotal()
	{
		ShoppingCartId = GetCartId();
		decimal? total = (from cartItems in _db.ShoppingCartItems
		                  where cartItems.CartId == ShoppingCartId
		                  select (decimal?)cartItems.Quantity * Convert.ToDecimal(cartItems.Product.UnitPrice)).Sum();
		return total ?? decimal.Zero;
	}

	public void UpdateShoppingCartDatabase(string cartId, ShoppingCartUpdates[] cartItemUpdates)
	{
		var myCart = GetCartItems();
		foreach (var cartItem in myCart)
		{
			for (var i = 0; i < cartItemUpdates.Length; i++)
			{
				if (cartItem.Product.ProductID != cartItemUpdates[i].ProductId)
				{
					continue;
				}

				if (cartItemUpdates[i].PurchaseQuantity < 1 || cartItemUpdates[i].RemoveItem)
				{
					RemoveItem(cartId, cartItem.ProductId);
				}
				else
				{
					UpdateItem(cartId, cartItem.ProductId, cartItemUpdates[i].PurchaseQuantity);
				}
			}
		}
	}

	public void RemoveItem(string removeCartId, int removeProductId)
	{
		var myItem = _db.ShoppingCartItems.FirstOrDefault(c => c.CartId == removeCartId && c.Product.ProductID == removeProductId);
		if (myItem is null)
		{
			return;
		}

		_db.ShoppingCartItems.Remove(myItem);
		_db.SaveChanges();
	}

	public void UpdateItem(string updateCartId, int updateProductId, int quantity)
	{
		var myItem = _db.ShoppingCartItems.FirstOrDefault(c => c.CartId == updateCartId && c.Product.ProductID == updateProductId);
		if (myItem is null)
		{
			return;
		}

		myItem.Quantity = quantity;
		_db.SaveChanges();
	}

	public void EmptyCart()
	{
		ShoppingCartId = GetCartId();
		var cartItems = _db.ShoppingCartItems.Where(c => c.CartId == ShoppingCartId).ToList();
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
		var shoppingCart = _db.ShoppingCartItems.Where(c => c.CartId == cartId).ToList();
		foreach (var item in shoppingCart)
		{
			item.CartId = userName;
		}

		var session = new HttpContextAccessor().HttpContext?.Session;
		if (session is not null)
		{
			session.SetString(CartSessionKey, userName);
		}
		else
		{
			_fallbackCartId = userName;
		}

		_db.SaveChanges();
	}

	public void Dispose()
	{
		_db.Dispose();
	}

	public struct ShoppingCartUpdates
	{
		public int ProductId;
		public int PurchaseQuantity;
		public bool RemoveItem;
	}
}
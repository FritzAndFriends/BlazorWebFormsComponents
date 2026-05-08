using System.Collections.Concurrent;
using WingtipToys.Models;

namespace WingtipToys.Logic;

public class CartStore(CatalogService catalogService)
{
	private readonly ConcurrentDictionary<string, List<CartItem>> _carts = new(StringComparer.Ordinal);
	private readonly object _sync = new();

	public void AddItem(string cartId, int productId)
	{
		lock (_sync)
		{
			var product = catalogService.GetProduct(productId, null);
			if (product is null)
			{
				return;
			}

			var items = _carts.GetOrAdd(cartId, _ => new List<CartItem>());
			var existing = items.FirstOrDefault(item => item.ProductId == productId);
			if (existing is null)
			{
				items.Add(new CartItem
				{
					ItemId = Guid.NewGuid().ToString("N"),
					CartId = cartId,
					ProductId = productId,
					Product = product,
					Quantity = 1,
					DateCreated = DateTime.UtcNow
				});
			}
			else
			{
				existing.Quantity++;
			}
		}
	}

	public IReadOnlyList<CartItem> GetCartItems(string cartId)
	{
		lock (_sync)
		{
			return _carts.TryGetValue(cartId, out var items)
				? items.Select(Clone).ToList()
				: new List<CartItem>();
		}
	}

	public void UpdateQuantity(string cartId, int productId, int quantity)
	{
		lock (_sync)
		{
			if (!_carts.TryGetValue(cartId, out var items))
			{
				return;
			}

			var existing = items.FirstOrDefault(item => item.ProductId == productId);
			if (existing is null)
			{
				return;
			}

			if (quantity < 1)
			{
				items.Remove(existing);
			}
			else
			{
				existing.Quantity = quantity;
			}
		}
	}

	public void RemoveItem(string cartId, int productId)
	{
		lock (_sync)
		{
			if (!_carts.TryGetValue(cartId, out var items))
			{
				return;
			}

			items.RemoveAll(item => item.ProductId == productId);
		}
	}

	public decimal GetTotal(string cartId)
	{
		lock (_sync)
		{
			return _carts.TryGetValue(cartId, out var items)
				? items.Sum(item => (decimal)(item.Product.UnitPrice ?? 0) * item.Quantity)
				: 0m;
		}
	}

	public int GetCount(string cartId)
	{
		lock (_sync)
		{
			return _carts.TryGetValue(cartId, out var items)
				? items.Sum(item => item.Quantity)
				: 0;
		}
	}

	private static CartItem Clone(CartItem item) => new()
	{
		ItemId = item.ItemId,
		CartId = item.CartId,
		ProductId = item.ProductId,
		Product = item.Product,
		Quantity = item.Quantity,
		DateCreated = item.DateCreated
	};
}

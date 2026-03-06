using WingtipToys.Models;

namespace WingtipToys.Services;

public interface IShoppingCartService
{
    Task<string> GetCartIdAsync();
    Task AddToCartAsync(int productId);
    Task<List<CartItem>> GetCartItemsAsync();
    Task<decimal> GetTotalAsync();
    Task UpdateCartAsync(ShoppingCartUpdates[] updates);
    Task RemoveItemAsync(int productId);
    Task EmptyCartAsync();
    Task<int> GetCountAsync();
}

public struct ShoppingCartUpdates
{
    public int ProductId { get; set; }
    public int PurchaseQuantity { get; set; }
    public bool RemoveItem { get; set; }
}

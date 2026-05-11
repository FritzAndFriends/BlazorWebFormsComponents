using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using WingtipToys.Models;
using WingtipToys.Logic;

namespace WingtipToys;

public partial class ShoppingCart
{
	[Inject] private ProductContext Db { get; set; } = default!;
	[Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;
	[Inject] private NavigationManager NavigationManager { get; set; } = default!;

	private List<CartItem> cartItems = new();
	private decimal orderTotal;

	protected override void OnInitialized()
	{
		using var cart = new ShoppingCartActions(Db, HttpContextAccessor);
		cartItems = cart.GetCartItems();
		orderTotal = cart.GetTotal();
	}

	private void UpdateBtn_Click()
	{
		// Reload cart items (full update would require form data)
		using var cart = new ShoppingCartActions(Db, HttpContextAccessor);
		cartItems = cart.GetCartItems();
		orderTotal = cart.GetTotal();
	}

	private void CheckoutBtn_Click()
	{
		NavigationManager.NavigateTo("/Checkout/CheckoutStart");
	}
}

using BlazorWebFormsComponents;
using WingtipToys.Logic;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ShoppingCart
{
	private GridView<CartItem> CartList = default!;
	private TextBox PurchaseQuantity = default!;
	private CheckBox Remove = default!;
	private Label LabelTotalText = default!;
	private Label lblTotal = default!;
	private Button UpdateBtn = default!;
	private ImageButton CheckoutImageBtn = default!;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		UpdateTotalDisplay();
	}

	public IQueryable<CartItem> GetShoppingCartItems(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
	{
		using var usersShoppingCart = new ShoppingCartActions();
		var items = usersShoppingCart.GetCartItems();
		totalRowCount = items.Count;
		return items.AsQueryable();
	}

	protected void UpdateBtn_Click()
	{
		UpdateTotalDisplay();
	}

	protected void CheckoutBtn_Click()
	{
		using var usersShoppingCart = new ShoppingCartActions();
		Session["payment_amt"] = usersShoppingCart.GetTotal();
		Response.Redirect("Checkout/CheckoutStart");
	}

	private void UpdateTotalDisplay()
	{
		using var usersShoppingCart = new ShoppingCartActions();
		var cartTotal = usersShoppingCart.GetTotal();
		if (cartTotal > 0)
		{
			LabelTotalText.Text = "Order Total: ";
			lblTotal.Text = string.Format("{0:c}", cartTotal);
			UpdateBtn.Visible = true;
			CheckoutImageBtn.Visible = true;
		}
		else
		{
			LabelTotalText.Text = string.Empty;
			lblTotal.Text = string.Empty;
			UpdateBtn.Visible = false;
			CheckoutImageBtn.Visible = false;
		}
	}
}
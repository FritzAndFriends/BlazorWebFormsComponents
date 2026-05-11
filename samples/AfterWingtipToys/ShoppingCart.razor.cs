using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Components;
using WingtipToys.Logic;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ShoppingCart
{
    [Inject] private ShoppingCartActions ShoppingCartActions { get; set; } = default!;

    private GridView<CartItem> CartList = default!;
    private TextBox PurchaseQuantity = default!;
    private CheckBox Remove = default!;
    private Label LabelTotalText = default!;
    private Label lblTotal = default!;
    private Button UpdateBtn = default!;
    private ImageButton CheckoutImageBtn = default!;

    private string ShoppingCartTitleText { get; set; } = "Shopping Cart";
    private string OrderTotalLabelText { get; set; } = "Order Total: ";
    private string OrderTotalValueText { get; set; } = string.Empty;
    private bool UpdateBtnVisible { get; set; } = true;
    private bool CheckoutImageBtnVisible { get; set; } = true;
    protected WebColor Transparent { get; } = WebColor.Transparent;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        RefreshCartState();
    }

    private IQueryable<CartItem> GetShoppingCartItems(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
        var items = ShoppingCartActions.GetCartItems();
        totalRowCount = items.Count;
        return items.AsQueryable();
    }

    private void UpdateBtn_Click(EventArgs args)
    {
        RefreshCartState();
    }

    private void CheckoutBtn_Click(EventArgs args)
    {
        Session["payment_amt"] = ShoppingCartActions.GetTotal();
        Response.Redirect("Checkout/CheckoutStart.aspx");
    }

    private void RefreshCartState()
    {
        var cartTotal = ShoppingCartActions.GetTotal();
        if (cartTotal > 0)
        {
            ShoppingCartTitleText = "Shopping Cart";
            OrderTotalLabelText = "Order Total: ";
            OrderTotalValueText = $"{cartTotal:c}";
            UpdateBtnVisible = true;
            CheckoutImageBtnVisible = true;
            return;
        }

        ShoppingCartTitleText = "Shopping Cart is Empty";
        OrderTotalLabelText = string.Empty;
        OrderTotalValueText = string.Empty;
        UpdateBtnVisible = false;
        CheckoutImageBtnVisible = false;
    }
}

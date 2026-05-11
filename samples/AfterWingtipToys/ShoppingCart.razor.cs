using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class ShoppingCart : WebFormsPageBase
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    private GridView<CartItem> CartList = default!;
    private TextBox PurchaseQuantity = default!;
    private CheckBox Remove = default!;
    private Label LabelTotalText = default!;
    private Label lblTotal = default!;
    private Button UpdateBtn = default!;
    private ImageButton CheckoutImageBtn = default!;

    private List<CartItem> cartItems = new();

    private List<CartItem> LoadShoppingCartItems()
    {
        using var db = DbFactory.CreateDbContext();
        var cartId = GetCartId();
        return db.ShoppingCartItems
            .Include(c => c.Product)
            .Where(c => c.CartId == cartId)
            .ToList();
    }

    public IQueryable<CartItem> GetShoppingCartItems(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
        totalRowCount = cartItems.Count;
        return cartItems.AsQueryable();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        cartItems = LoadShoppingCartItems();
        decimal cartTotal = cartItems.Sum(c => (decimal)(c.Quantity * c.Product.UnitPrice));
        if (cartTotal > 0)
        {
            lblTotal.Text = string.Format("{0:c}", cartTotal);
        }
        else
        {
            LabelTotalText.Text = "";
            lblTotal.Text = "";
            UpdateBtn.Visible = false;
            CheckoutImageBtn.Visible = false;
        }
    }

    protected void UpdateBtn_Click()
    {
        cartItems = LoadShoppingCartItems();
        decimal cartTotal = cartItems.Sum(c => (decimal)(c.Quantity * c.Product.UnitPrice));
        lblTotal.Text = string.Format("{0:c}", cartTotal);
    }

    protected void CheckoutBtn_Click()
    {
        Session["payment_amt"] = cartItems.Sum(c => (decimal)(c.Quantity * c.Product.UnitPrice));
        Response.Redirect("Checkout/CheckoutStart");
    }

    private string GetCartId()
    {
        if (Session["CartId"] == null)
        {
            Session["CartId"] = Guid.NewGuid().ToString();
        }
        return Session["CartId"]!.ToString()!;
    }
}

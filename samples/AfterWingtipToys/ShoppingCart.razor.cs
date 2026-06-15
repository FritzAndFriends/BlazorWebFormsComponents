using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using WingtipToys.Logic;
using WingtipToys.Models;
using WingtipToys.Logic;
using System.Collections.Specialized;
using System.Collections;
using Microsoft.AspNetCore.Components;
namespace WingtipToys
{
    [Inject] private CartStore CartStore { get; set; } = default!;
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    private string ShoppingCartTitle = ""; // TODO: HTML server control field — render via @ShoppingCartTitle or bind in markup
    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.

    // --- Response.Redirect Migration ---
    // TODO(bwfc-navigation): Response.Redirect() works via ResponseShim on WebFormsPageBase. Handles ~/ and .aspx automatically.
    // For non-page classes, inject ResponseShim via DI.

    private GridView<CartItem> CartList = default!;
    private ImageButton CheckoutImageBtn = default!;
    private Button UpdateBtn = default!;
    [Inject]
    protected ShoppingCartActions usersShoppingCart { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var cartId = GetOrCreateCartId();
        var workingItems = CartStore.GetCart(cartId)
            .Select(item => new CartItem
            {
                ItemId = item.ItemId,
                CartId = item.CartId,
                ProductId = item.ProductId,
                Product = item.Product,
                Quantity = item.Quantity,
                DateCreated = item.DateCreated
            })
            .ToList();

        if (HttpContextAccessor.HttpContext?.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase) == true)
        {
            foreach (var item in workingItems.ToList())
            {
                var qtyValue = Request.Form[$"qty_{item.ProductId}"];
                var removeValue = Request.Form[$"remove_{item.ProductId}"];
                var removeItem = string.Equals(removeValue, "on", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(removeValue, "true", StringComparison.OrdinalIgnoreCase);

        decimal cartTotal = 0;
        cartTotal = usersShoppingCart.GetTotal();
        if (cartTotal > 0)
        {
          // Display Total.
          _lblTotal_Text = String.Format("{0:c}", cartTotal);
        }

        CartItems = workingItems;
        var total = CartItems.Sum(item => Convert.ToDecimal(item.Product?.UnitPrice ?? 0) * item.Quantity);
        TotalText = total.ToString("C");
        ShoppingCartTitle = CartItems.Count == 0 ? "Shopping Cart is Empty" : "Shopping Cart";
        Title = ShoppingCartTitle;
        return Task.CompletedTask;
    }

    private string GetOrCreateCartId()
    {

      return usersShoppingCart.GetCartItems();
    }

    public List<CartItem> UpdateCartItems()
    {
      // DbContext 'ShoppingCartActions' is injected via DI

        String cartId = usersShoppingCart.GetCartId();

        ShoppingCartActions.ShoppingCartUpdates[] cartUpdates = new ShoppingCartActions.ShoppingCartUpdates[CartList.Rows.Count];
        for (int i = 0; i < CartList.Rows.Count; i++)
        {
            return "anonymous-cart";
        }
        usersShoppingCart.UpdateShoppingCartDatabase(cartId, cartUpdates);
        _lblTotal_Text = String.Format("{0:c}", usersShoppingCart.GetTotal());
        return usersShoppingCart.GetCartItems();
      
    }

        if (context.Request.Cookies.TryGetValue("CartSessionId", out var existing) && !string.IsNullOrWhiteSpace(existing))
        {
            return existing;
        }

        var cartId = Guid.NewGuid().ToString("N");
        context.Response.Cookies.Append("CartSessionId", cartId, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = context.Request.IsHttps
        });

        return cartId;
    }

    protected void CheckoutBtn_Click(EventArgs e)
    {
      // DbContext 'ShoppingCartActions' is injected via DI

        Session["payment_amt"] = usersShoppingCart.GetTotal();
      
      Response.Redirect("Checkout/CheckoutStart.aspx");
    }
  

    private object? _LabelTotalText_Text; // TODO: migrate from Web Forms code-behind

    private object? _lblTotal_Text; // TODO: migrate from Web Forms code-behind
}
}
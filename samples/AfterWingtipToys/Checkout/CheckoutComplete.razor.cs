// =============================================================================
// TODO: This code-behind was copied from Web Forms and needs manual migration.
//
// Common transforms needed (use the BWFC Copilot skill for assistance):
//   - Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
//   - Page_PreRender → OnAfterRenderAsync
//   - IsPostBack checks → remove or convert to state logic
//   - ViewState usage → component [Parameter] or private fields
//   - Session/Cache access → inject IHttpContextAccessor or use DI
//   - Response.Redirect → NavigationManager.NavigateTo
//   - Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   - Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   - UpdatePanel / ScriptManager references → remove (Blazor handles updates)
//   - User controls → Blazor component references
// =============================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WingtipToys.Models;

namespace WingtipToys.Checkout
{
  public partial class CheckoutComplete : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        // Verify user has completed the checkout process.
        if ((string)Session["userCheckoutCompleted"] != "true")
        {
          Session["userCheckoutCompleted"] = string.Empty;
          Response.Redirect("CheckoutError.aspx?" + "Desc=Unvalidated%20Checkout.");
        }

        NVPAPICaller payPalCaller = new NVPAPICaller();

        string retMsg = "";
        string token = "";
        string finalPaymentAmount = "";
        string PayerID = "";
        NVPCodec decoder = new NVPCodec();

        token = Session["token"].ToString();
        PayerID = Session["payerId"].ToString();
        finalPaymentAmount = Session["payment_amt"].ToString();

        bool ret = payPalCaller.DoCheckoutPayment(finalPaymentAmount, token, PayerID, ref decoder, ref retMsg);
        if (ret)
        {
          // Retrieve PayPal confirmation value.
          string PaymentConfirmation = decoder["PAYMENTINFO_0_TRANSACTIONID"].ToString();
          TransactionId.Text = PaymentConfirmation;


          ProductContext _db = new ProductContext();
          // Get the current order id.
          int currentOrderId = -1;
          if (Session["currentOrderId"] != string.Empty)
          {
            currentOrderId = Convert.ToInt32(Session["currentOrderID"]);
          }
          Order myCurrentOrder;
          if (currentOrderId >= 0)
          {
            // Get the order based on order id.
            myCurrentOrder = _db.Orders.Single(o => o.OrderId == currentOrderId);
            // Update the order to reflect payment has been completed.
            myCurrentOrder.PaymentTransactionId = PaymentConfirmation;
            // Save to DB.
            _db.SaveChanges();
          }

          // Clear shopping cart.
          using (WingtipToys.Logic.ShoppingCartActions usersShoppingCart =
              new WingtipToys.Logic.ShoppingCartActions())
          {
            usersShoppingCart.EmptyCart();
          }

          // Clear order id.
          Session["currentOrderId"] = string.Empty;
        }
        else
        {
          Response.Redirect("CheckoutError.aspx?" + retMsg);
        }
      }
    }

    protected void Continue_Click(object sender, EventArgs e)
    {
      Response.Redirect("~/Default.aspx");
    }
  }
}

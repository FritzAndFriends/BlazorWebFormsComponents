// =============================================================================
// TODO(bwfc-general): This code-behind was copied from Web Forms and needs manual migration.
//
// Common transforms needed (use the BWFC Copilot skill for assistance):
//   TODO(bwfc-lifecycle): Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
//   TODO(bwfc-lifecycle): Page_PreRender → OnAfterRenderAsync
//   TODO(bwfc-ispostback): IsPostBack checks → remove or convert to state logic
//   TODO(bwfc-viewstate): ViewState usage → component [Parameter] or private fields
//   TODO(bwfc-session-state): Session/Cache access → auto-wired on WebFormsPageBase via SessionShim/CacheShim
//   TODO(bwfc-navigation): Response.Redirect → auto-wired on WebFormsPageBase via ResponseShim
//   TODO(bwfc-form): Request.Form["key"] → auto-wired on WebFormsPageBase via FormShim (use <WebFormsForm> for interactive mode)
//   TODO(bwfc-server): Server.MapPath/HtmlEncode → auto-wired on WebFormsPageBase via ServerShim
//   TODO(bwfc-config): ConfigurationManager.AppSettings → BWFC shim (call app.UseConfigurationManagerShim() in Program.cs)
//   TODO(bwfc-general): ClientScript.RegisterStartupScript → auto-wired on WebFormsPageBase via ClientScriptShim
//   TODO(bwfc-general): Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   TODO(bwfc-datasource): Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   TODO(bwfc-general): ScriptManager code-behind references → use ScriptManagerShim via ScriptManager.GetCurrent(this)
//   TODO(bwfc-general): UpdatePanel markup preserved by BWFC (ContentTemplate supported) — remove only code-behind API calls
//   TODO(bwfc-general): User controls → Blazor component references
// =============================================================================

// --- Session State Migration ---
// TODO(bwfc-session-state): Session["key"] calls work automatically via SessionShim on WebFormsPageBase.
// Session keys found: token, payerId, payment_amt, currentOrderId, userCheckoutCompleted
// Options for long-term replacement:
//   (1) ProtectedSessionStorage (Blazor Server) — persists across circuits
//   (2) Scoped service via DI — lifetime matches user circuit
//   (3) Cascading parameter from a root-level state provider
// See: https://learn.microsoft.com/aspnet/core/blazor/state-management

using System;
using System.Collections.Generic;
using System.Linq;
using WingtipToys.Models;

namespace WingtipToys.Checkout
{
  public partial class CheckoutReview
  {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.

    // --- Response.Redirect Migration ---
    // TODO(bwfc-navigation): Response.Redirect() works via ResponseShim on WebFormsPageBase. Handles ~/ and .aspx automatically.
    // For non-page classes, inject ResponseShim via DI.

    private Label Address = default!;
    private Button CheckoutConfirm = default!;
    private Label City = default!;
    private Label FirstName = default!;
    private Label LastName = default!;
    private GridView<object> OrderItemList = default!;
    private Label PostalCode = default!;
    private DetailsView<object> ShipInfo = default!;
    private Label State = default!;
    private Label Total = default!;
    // --- ConfigurationManager Migration ---
    // TODO(bwfc-config): ConfigurationManager calls work via BWFC shim.
    // Ensure app.UseConfigurationManagerShim() is called in Program.cs.

    protected override async Task OnInitializedAsync()
    {
        // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
        await base.OnInitializedAsync();

            // BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change
      NVPAPICaller payPalCaller = new NVPAPICaller();

          string retMsg = "";
          string token = "";
          string PayerID = "";
          NVPCodec decoder = new NVPCodec();
          token = Session["token"].ToString();

          bool ret = payPalCaller.GetCheckoutDetails(token, ref PayerID, ref decoder, ref retMsg);
          if (ret)
          {
            Session["payerId"] = PayerID;

            var myOrder = new Order();
            myOrder.OrderDate = Convert.ToDateTime(decoder["TIMESTAMP"].ToString());
            myOrder.Username = User.Identity.Name;
            myOrder.FirstName = decoder["FIRSTNAME"].ToString();
            myOrder.LastName = decoder["LASTNAME"].ToString();
            myOrder.Address = decoder["SHIPTOSTREET"].ToString();
            myOrder.City = decoder["SHIPTOCITY"].ToString();
            myOrder.State = decoder["SHIPTOSTATE"].ToString();
            myOrder.PostalCode = decoder["SHIPTOZIP"].ToString();
            myOrder.Country = decoder["SHIPTOCOUNTRYCODE"].ToString();
            myOrder.Email = decoder["EMAIL"].ToString();
            myOrder.Total = Convert.ToDecimal(decoder["AMT"].ToString());

            // Verify total payment amount as set on CheckoutStart.aspx.
            try
            {
              decimal paymentAmountOnCheckout = Convert.ToDecimal(Session["payment_amt"].ToString());
              decimal paymentAmoutFromPayPal = Convert.ToDecimal(decoder["AMT"].ToString());
              if (paymentAmountOnCheckout != paymentAmoutFromPayPal)
              {
                Response.Redirect("CheckoutError.aspx?" + "Desc=Amount%20total%20mismatch.");
              }
            }
            catch (Exception)
            {
              Response.Redirect("CheckoutError.aspx?" + "Desc=Amount%20total%20mismatch.");
            }

            // Get DB context.
            ProductContext _db = new ProductContext();

            // Add order to DB.
            _db.Orders.Add(myOrder);
            _db.SaveChanges();

            // Get the shopping cart items and process them.
            using (WingtipToys.Logic.ShoppingCartActions usersShoppingCart = new WingtipToys.Logic.ShoppingCartActions())
            {
              List<CartItem> myOrderList = usersShoppingCart.GetCartItems();

              // Add OrderDetail information to the DB for each product purchased.
              for (int i = 0; i < myOrderList.Count; i++)
              {
                // Create a new OrderDetail object.
                var myOrderDetail = new OrderDetail();
                myOrderDetail.OrderId = myOrder.OrderId;
                myOrderDetail.Username = User.Identity.Name;
                myOrderDetail.ProductId = myOrderList[i].ProductId;
                myOrderDetail.Quantity = myOrderList[i].Quantity;
                myOrderDetail.UnitPrice = myOrderList[i].Product.UnitPrice;

                // Add OrderDetail to DB.
                _db.OrderDetails.Add(myOrderDetail);
                _db.SaveChanges();
              }

              // Set OrderId.
              Session["currentOrderId"] = myOrder.OrderId;

              // Display Order information.
              List<Order> orderList = new List<Order>();
              orderList.Add(myOrder);
              ShipInfo.DataSource = orderList;
              
              // Display OrderDetails.
              OrderItemList.DataSource = myOrderList;
                          }
          }
          else
          {
            Response.Redirect("CheckoutError.aspx?" + retMsg);
          }
    }

    protected void CheckoutConfirm_Click()
    {
      Session["userCheckoutCompleted"] = "true";
      Response.Redirect("/Checkout/CheckoutComplete");
    }
  }
}
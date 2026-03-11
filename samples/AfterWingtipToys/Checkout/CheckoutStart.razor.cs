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

namespace WingtipToys.Checkout
{
  public partial class CheckoutStart : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      NVPAPICaller payPalCaller = new NVPAPICaller();
      string retMsg = "";
      string token = "";

      if (Session["payment_amt"] != null)
      {
        string amt = Session["payment_amt"].ToString();

        bool ret = payPalCaller.ShortcutExpressCheckout(amt, ref token, ref retMsg);
        if (ret)
        {
          Session["token"] = token;
          Response.Redirect(retMsg);
        }
        else
        {
          Response.Redirect("CheckoutError.aspx?" + retMsg);
        }
      }
      else
      {
        Response.Redirect("CheckoutError.aspx?ErrorCode=AmtMissing");
      }
    }
  }
}

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
// Session keys found: payment_amt, token
// Options for long-term replacement:
//   (1) ProtectedSessionStorage (Blazor Server) — persists across circuits
//   (2) Scoped service via DI — lifetime matches user circuit
//   (3) Cascading parameter from a root-level state provider
// See: https://learn.microsoft.com/aspnet/core/blazor/state-management

using System;
using System.Collections.Generic;
using System.Linq;
namespace WingtipToys.Checkout
{
  public partial class CheckoutStart
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

    // --- ConfigurationManager Migration ---
    // TODO(bwfc-config): ConfigurationManager calls work via BWFC shim.
    // Ensure app.UseConfigurationManagerShim() is called in Program.cs.

    protected override async Task OnInitializedAsync()
    {
        // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
        await base.OnInitializedAsync();

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
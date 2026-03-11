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
using System.Web.ModelBinding;

namespace WingtipToys
{
  public partial class ProductDetails : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public IQueryable<Product> GetProduct(
                        [SupplyParameterFromQuery(Name = "ProductID")] int? productId,
                        // TODO: Verify RouteData → [Parameter] conversion — ensure @page route has matching {parameter}
                        [Parameter] string productName)
    {
      var _db = new WingtipToys.Models.ProductContext();
      IQueryable<Product> query = _db.Products;
      if (productId.HasValue && productId > 0)
      {
        query = query.Where(p => p.ProductID == productId);
      }
      else if (!String.IsNullOrEmpty(productName))
      {
        query = query.Where(p =>
                  String.Compare(p.ProductName, productName) == 0);
      }
      else
      {
        query = null;
      }
      return query;
    }
  }
}

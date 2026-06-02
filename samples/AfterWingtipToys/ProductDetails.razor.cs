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
using System;
using System.Collections.Generic;
using System.Linq;
using WingtipToys.Models;
using WingtipToys.Logic;
using Microsoft.AspNetCore.Components;
namespace WingtipToys
{
  public partial class ProductDetails : WebFormsPageBase
  {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.

    private FormView<Product> productDetail = default!;
    [Parameter] public string? productName { get; set; }

    [Inject]
    protected ProductContext _productContext { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
        await base.OnInitializedAsync();


    }

    public IQueryable<Product> GetProduct(
                        [QueryString("ProductID")] int? productId,
                        [RouteData] string productName)
    {
      var _db = _productContext; // Injected via DI
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
  
    [Parameter, SupplyParameterFromQuery(Name = "ProductID")] public int? ProductId { get; set; }

    private global::System.Linq.IQueryable<Product> GetProductQueryDetails_SelectMethod(int maxRows, int startRowIndex, string sortByExpression, out int totalRowCount)
    {
        totalRowCount = 0;
        // TODO(bwfc-query-details): Wrapper delegates to the code-behind GetProduct method.
        var query = GetProduct(ProductId, productName);
        if (query != null) totalRowCount = query.Count();
        return query ?? global::System.Linq.Enumerable.Empty<Product>().AsQueryable();
    }
}
}
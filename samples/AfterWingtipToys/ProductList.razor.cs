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
using Microsoft.AspNetCore.Components;
namespace WingtipToys
{
  public partial class ProductList : WebFormsPageBase
  {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    [Parameter, SupplyParameterFromQuery(Name = "id")]
    public int? CategoryId { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "categoryName")]
    public string? CategoryName { get; set; }

    private IReadOnlyList<Product> Products { get; set; } = [];

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        Title = "Products";

        await using var db = await DbFactory.CreateDbContextAsync();
        var query = db.Products.Include(product => product.Category).AsQueryable();

        if (CategoryId is > 0)
        {
            query = query.Where(product => product.CategoryID == CategoryId);
        }

        if (!string.IsNullOrWhiteSpace(CategoryName))
        {
            query = query.Where(product => product.Category != null && product.Category.CategoryName == CategoryName);
        }

        Products = await query.OrderBy(product => product.ProductName).ToListAsync();
    }
}
}
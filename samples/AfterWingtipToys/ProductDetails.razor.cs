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
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;
namespace WingtipToys
{
  public partial class ProductDetails
  {
    [Inject] public IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery(Name = "ProductID")]
    public int? ProductId { get; set; }

    [Parameter]
    public string? ProductName { get; set; }

    private Product? product;

    protected override async Task OnParametersSetAsync()
    {
      await base.OnParametersSetAsync();

      using var db = DbFactory.CreateDbContext();
      IQueryable<Product> query = db.Products.Include(p => p.Category);

      if (ProductId.GetValueOrDefault() > 0)
      {
        query = query.Where(p => p.ProductID == ProductId.Value);
      }
      else if (!string.IsNullOrWhiteSpace(ProductName))
      {
        query = query.Where(p => p.ProductName == ProductName);
      }
      else
      {
        product = null;
        return;
      }

      product = await query.FirstOrDefaultAsync();
    }
  }
}
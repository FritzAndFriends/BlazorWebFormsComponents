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

namespace WingtipToys.Admin
{
  public partial class AdminPage
  {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Server Utility Migration ---
    // TODO(bwfc-server): Server.* calls work automatically via ServerShim on WebFormsPageBase.
    // Methods found: MapPath
    // For non-page classes, inject ServerShim via DI.
    // MapPath("~/path") maps to IWebHostEnvironment.WebRootPath.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.

    // --- Response.Redirect Migration ---
    // TODO(bwfc-navigation): Response.Redirect() works via ResponseShim on WebFormsPageBase. Handles ~/ and .aspx automatically.
    // For non-page classes, inject ResponseShim via DI.

    private Button AddProductButton = default!;
    private TextBox AddProductDescription = default!;
    private TextBox AddProductName = default!;
    private TextBox AddProductPrice = default!;
    private DropDownList<Category> DropDownAddCategory = default!;
    private DropDownList<Product> DropDownRemoveProduct = default!;
    private Label LabelAddCategory = default!;
    private Label LabelAddDescription = default!;
    private Label LabelAddImageFile = default!;
    private Label LabelAddName = default!;
    private Label LabelAddPrice = default!;
    private Label LabelAddStatus = default!;
    private Label LabelRemoveProduct = default!;
    private Label LabelRemoveStatus = default!;
    private FileUpload ProductImage = default!;
    private RegularExpressionValidator RegularExpressionValidator1 = default!;
    private Button RemoveProductButton = default!;
    private RequiredFieldValidator<object> RequiredFieldValidator1 = default!;
    private RequiredFieldValidator<object> RequiredFieldValidator2 = default!;
    private RequiredFieldValidator<object> RequiredFieldValidator3 = default!;
    private RequiredFieldValidator<object> RequiredFieldValidator4 = default!;
    // --- ConfigurationManager Migration ---
    // TODO(bwfc-config): ConfigurationManager calls work via BWFC shim.
    // Ensure app.UseConfigurationManagerShim() is called in Program.cs.

    protected override async Task OnInitializedAsync()
    {
        // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
        await base.OnInitializedAsync();

      string productAction = Request.QueryString["ProductAction"];
      if (productAction == "add")
      {
        LabelAddStatus.Text = "Product added!";
      }

      if (productAction == "remove")
      {
        LabelRemoveStatus.Text = "Product removed!";
      }
    }

    protected void AddProductButton_Click()
    {
      Boolean fileOK = false;
      String path = Server.MapPath("~/Catalog/Images/");
      if (ProductImage.HasFile)
      {
        String fileExtension = System.IO.Path.GetExtension(ProductImage.FileName).ToLower();
        String[] allowedExtensions = { ".gif", ".png", ".jpeg", ".jpg" };
        for (int i = 0; i < allowedExtensions.Length; i++)
        {
          if (fileExtension == allowedExtensions[i])
          {
            fileOK = true;
          }
        }
      }

      if (fileOK)
      {
        try
        {
          // Save to Images folder.
          ProductImage.PostedFile.SaveAs(path + ProductImage.FileName);
          // Save to Images/Thumbs folder.
          ProductImage.PostedFile.SaveAs(path + "Thumbs/" + ProductImage.FileName);
        }
        catch (Exception ex)
        {
          LabelAddStatus.Text = ex.Message;
        }

        // Add product data to DB.
        AddProducts products = new AddProducts();
        bool addSuccess = products.AddProduct(AddProductName.Text, AddProductDescription.Text,
            AddProductPrice.Text, DropDownAddCategory.SelectedValue, ProductImage.FileName);
        if (addSuccess)
        {
          // Reload the page.
          string pageUrl = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.Count() - Request.Url.Query.Count());
          Response.Redirect(pageUrl + "?ProductAction=add");
        }
        else
        {
          LabelAddStatus.Text = "Unable to add new product to database.";
        }
      }
      else
      {
        LabelAddStatus.Text = "Unable to accept file type.";
      }
    }

    public IQueryable GetCategories()
    {
      var _db = new WingtipToys.Models.ProductContext();
      IQueryable query = _db.Categories;
      return query;
    }

    public IQueryable GetProducts()
    {
      var _db = new WingtipToys.Models.ProductContext();
      IQueryable query = _db.Products;
      return query;
    }

    protected void RemoveProductButton_Click()
    {
      using (var _db = new WingtipToys.Models.ProductContext())
      {
        int productId = Convert.ToInt16(DropDownRemoveProduct.SelectedValue);
        var myItem = (from c in _db.Products where c.ProductID == productId select c).FirstOrDefault();
        if (myItem != null)
        {
          _db.Products.Remove(myItem);
          _db.SaveChanges();

          // Reload the page.
          string pageUrl = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.Count() - Request.Url.Query.Count());
          Response.Redirect(pageUrl + "?ProductAction=remove");
        }
        else
        {
          LabelRemoveStatus.Text = "Unable to locate product.";
        }
      }
    }
  }
}
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
using WingtipToys.Logic;

namespace WingtipToys.Admin
{
  public partial class AdminPage : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
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

    protected void AddProductButton_Click(object sender, EventArgs e)
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

    protected void RemoveProductButton_Click(object sender, EventArgs e)
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

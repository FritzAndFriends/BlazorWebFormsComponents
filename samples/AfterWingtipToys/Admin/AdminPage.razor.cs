using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Admin
{
  public partial class AdminPage
  {
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "ProductAction")]
    public string? ProductAction { get; set; }

    private List<Category>? _categories;
    private List<Product>? _products;
    private string _addStatus = "";
    private string _removeStatus = "";

    protected override async Task OnInitializedAsync()
    {
      Page.Title = "Administration";
      using var db = DbFactory.CreateDbContext();
      _categories = await db.Categories.ToListAsync();
      _products = await db.Products.ToListAsync();

      if (ProductAction == "add")
      {
        _addStatus = "Product added!";
      }
      if (ProductAction == "remove")
      {
        _removeStatus = "Product removed!";
      }
    }

    private async Task AddProductButton_Click(MouseEventArgs e)
    {
      // TODO: Implement product add with file upload
      // FileUpload component needs Blazor InputFile replacement
      _addStatus = "TODO: Product add requires InputFile component migration.";
    }

    private async Task RemoveProductButton_Click(MouseEventArgs e)
    {
      // TODO: Read DropDownRemoveProduct.SelectedValue and remove product
      // BWFC DropDownList integration for SelectedValue needs wiring
      _removeStatus = "TODO: Product remove requires DropDownList SelectedValue wiring.";
    }
  }
}

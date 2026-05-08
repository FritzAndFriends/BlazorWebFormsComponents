using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Admin;

public partial class AdminPage
{
    [Inject]
    private ProductContext ProductContext { get; set; } = default!;

    private List<Category> Categories { get; set; } = [];

    private List<Product> Products { get; set; } = [];

    private string AddProductName { get; set; } = string.Empty;

    private string AddProductDescription { get; set; } = string.Empty;

    private string AddProductPrice { get; set; } = string.Empty;

    private string AddProductImagePath { get; set; } = string.Empty;

    private int SelectedCategoryId { get; set; } = 1;

    private int SelectedProductId { get; set; }

    private string AddStatus { get; set; } = string.Empty;

    private string RemoveStatus { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        Categories = await ProductContext.Categories.OrderBy(c => c.CategoryName).ToListAsync();
        Products = await ProductContext.Products.OrderBy(p => p.ProductName).ToListAsync();
        if (Products.Count > 0 && SelectedProductId == 0)
        {
            SelectedProductId = Products[0].ProductID;
        }
        if (Categories.Count > 0 && SelectedCategoryId == 0)
        {
            SelectedCategoryId = Categories[0].CategoryID;
        }
    }

    private async Task AddProductButton_Click()
    {
        if (string.IsNullOrWhiteSpace(AddProductName) || !double.TryParse(AddProductPrice, out var price))
        {
            AddStatus = "Enter a name and numeric price before adding a product.";
            return;
        }

        var nextId = Products.Count == 0 ? 1 : Products.Max(p => p.ProductID) + 1;
        ProductContext.Products.Add(new Product
        {
            ProductID = nextId,
            ProductName = AddProductName,
            Description = string.IsNullOrWhiteSpace(AddProductDescription) ? AddProductName : AddProductDescription,
            ImagePath = string.IsNullOrWhiteSpace(AddProductImagePath) ? "placeholder.png" : AddProductImagePath,
            UnitPrice = price,
            CategoryID = SelectedCategoryId
        });
        await ProductContext.SaveChangesAsync();

        AddStatus = "Product added.";
        AddProductName = string.Empty;
        AddProductDescription = string.Empty;
        AddProductPrice = string.Empty;
        AddProductImagePath = string.Empty;
        await LoadDataAsync();
    }

    private async Task RemoveProductButton_Click()
    {
        var product = await ProductContext.Products.FirstOrDefaultAsync(p => p.ProductID == SelectedProductId);
        if (product is null)
        {
            RemoveStatus = "Unable to locate product.";
            return;
        }

        ProductContext.Products.Remove(product);
        await ProductContext.SaveChangesAsync();
        RemoveStatus = "Product removed.";
        await LoadDataAsync();
    }
}

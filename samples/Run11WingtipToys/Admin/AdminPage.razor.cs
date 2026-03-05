using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys.Admin;

public partial class AdminPage
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    private List<Category> _categories = new();
    private List<Product> _products = new();
    private int _selectedCategoryId;
    private int _selectedRemoveProductId;
    private string _newProductName = string.Empty;
    private string _newProductDescription = string.Empty;
    private string _newProductPrice = string.Empty;
    private string _addStatus = string.Empty;
    private string _removeStatus = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        using var db = DbFactory.CreateDbContext();
        _categories = await db.Categories.ToListAsync();
        _products = await db.Products.ToListAsync();
    }

    private async Task OnAddProduct(EventArgs args)
    {
        if (string.IsNullOrWhiteSpace(_newProductName) || !double.TryParse(_newProductPrice, out var price))
        {
            _addStatus = "Please fill in all required fields.";
            return;
        }

        using var db = DbFactory.CreateDbContext();
        var product = new Product
        {
            ProductName = _newProductName,
            Description = _newProductDescription,
            UnitPrice = price,
            CategoryID = _selectedCategoryId,
            ImagePath = "placeholder.png"
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();

        _addStatus = $"Product '{_newProductName}' added.";
        _newProductName = string.Empty;
        _newProductDescription = string.Empty;
        _newProductPrice = string.Empty;
        await LoadData();
    }

    private async Task OnRemoveProduct(EventArgs args)
    {
        if (_selectedRemoveProductId <= 0)
        {
            _removeStatus = "Please select a product to remove.";
            return;
        }

        using var db = DbFactory.CreateDbContext();
        var product = await db.Products.FindAsync(_selectedRemoveProductId);
        if (product != null)
        {
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            _removeStatus = $"Product '{product.ProductName}' removed.";
        }
        await LoadData();
    }
}

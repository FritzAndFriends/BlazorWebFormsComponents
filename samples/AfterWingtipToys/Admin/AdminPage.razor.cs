using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Admin;

public partial class AdminPage
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IWebHostEnvironment Environment { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "ProductAction")]
    public string? ProductAction { get; set; }

    private List<Category> _categories = new();
    private List<Product> _products = new();
    private string _addStatus = "";
    private string _removeStatus = "";

    protected override async Task OnInitializedAsync()
    {
        using var db = DbFactory.CreateDbContext();
        _categories = await db.Categories.ToListAsync();
        _products = await db.Products.ToListAsync();

        if (ProductAction == "add")
            _addStatus = "Product added!";
        if (ProductAction == "remove")
            _removeStatus = "Product removed!";
    }

    private async Task AddProductButton_Click(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        // TODO: Implement file upload and product add with IWebHostEnvironment for image saving
        _addStatus = "TODO: File upload not yet implemented in Blazor migration";
        await Task.CompletedTask;
    }

    private async Task RemoveProductButton_Click(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
    {
        // TODO: Read selected value from DropDownRemoveProduct and remove product
        // using var db = DbFactory.CreateDbContext();
        // var productId = int.Parse(DropDownRemoveProduct.SelectedValue);
        // var product = await db.Products.FindAsync(productId);
        // if (product != null) { db.Products.Remove(product); await db.SaveChangesAsync(); }
        _removeStatus = "TODO: Remove product not yet wired to dropdown selection";
        await Task.CompletedTask;
    }
}

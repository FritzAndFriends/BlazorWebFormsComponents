using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys.Admin
{
    public partial class AdminPage : ComponentBase
    {
        [Inject] private IDbContextFactory<ProductContext> ContextFactory { get; set; } = default!;
        [Inject] private IWebHostEnvironment Environment { get; set; } = default!;

        private List<Category> categories = new();
        private List<Product> products = new();
        private Product newProduct = new();
        private string selectedCategoryId = "";
        private string selectedProductId = "";
        private string addStatusMessage = "";
        private string removeStatusMessage = "";
        private string priceText = "";
        private FileUpload? fileUpload;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            using var context = ContextFactory.CreateDbContext();
            categories = await context.Categories.ToListAsync();
            products = await context.Products.ToListAsync();

            if (categories.Any() && string.IsNullOrEmpty(selectedCategoryId))
                selectedCategoryId = categories.First().CategoryID.ToString();

            if (products.Any() && string.IsNullOrEmpty(selectedProductId))
                selectedProductId = products.First().ProductID.ToString();
        }

        private void OnCategoryChanged(string value) => selectedCategoryId = value;
        private void OnProductChanged(string value) => selectedProductId = value;

        private void OnPriceChanged(string value)
        {
            priceText = value;
            if (decimal.TryParse(value, out var price))
                newProduct.UnitPrice = price;
        }

        private async Task AddProduct(MouseEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(newProduct.ProductName))
            {
                addStatusMessage = "Product name is required.";
                return;
            }

            if (!int.TryParse(selectedCategoryId, out var categoryId))
            {
                addStatusMessage = "Please select a category.";
                return;
            }

            newProduct.CategoryID = categoryId;

            if (fileUpload?.HasFile == true)
            {
                var imagesPath = Path.Combine(Environment.WebRootPath, "Catalog", "Images");
                Directory.CreateDirectory(imagesPath);
                var filePath = Path.Combine(imagesPath, fileUpload.FileName);
                await fileUpload.SaveAs(filePath);
                newProduct.ImagePath = fileUpload.FileName;
            }

            using var context = ContextFactory.CreateDbContext();
            context.Products.Add(newProduct);
            await context.SaveChangesAsync();

            addStatusMessage = $"Product '{newProduct.ProductName}' added successfully.";
            newProduct = new Product();
            priceText = "";
            await LoadDataAsync();
        }

        private async Task RemoveProduct(MouseEventArgs args)
        {
            if (!int.TryParse(selectedProductId, out var productId))
            {
                removeStatusMessage = "Please select a product.";
                return;
            }

            using var context = ContextFactory.CreateDbContext();
            var product = await context.Products.FindAsync(productId);
            if (product != null)
            {
                var productName = product.ProductName;
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                removeStatusMessage = $"Product '{productName}' removed successfully.";
            }

            await LoadDataAsync();
        }
    }
}

using Microsoft.AspNetCore.Components;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class ShoppingCart : ComponentBase
    {
        public List<CartItem> CartItems { get; set; } = new()
        {
            new() { ItemId = "1", CartId = "demo", Quantity = 1, ProductId = 2, Product = new Product { ProductID = 2, ProductName = "Old-time Car", UnitPrice = 15.95m } },
            new() { ItemId = "2", CartId = "demo", Quantity = 1, ProductId = 4, Product = new Product { ProductID = 4, ProductName = "Super Fast Car", UnitPrice = 8.95m } },
            new() { ItemId = "3", CartId = "demo", Quantity = 2, ProductId = 1, Product = new Product { ProductID = 1, ProductName = "Convertible Car", UnitPrice = 22.50m } },
        };
    }
}


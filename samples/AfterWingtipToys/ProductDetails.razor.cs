using Microsoft.AspNetCore.Components;
using WingtipToys.Models;

namespace WingtipToys
{
    public partial class ProductDetails : ComponentBase
    {
        public Product SampleProduct { get; set; } = new()
        {
            ProductID = 1,
            ProductName = "Convertible Car",
            UnitPrice = 22.50m,
            ImagePath = "car1.png",
            Description = "This convertible car is fast! The engine is powered by a neutrino based battery (not included). Power it up and let it go!"
        };
    }
}

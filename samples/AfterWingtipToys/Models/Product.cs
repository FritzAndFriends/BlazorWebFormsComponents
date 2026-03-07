using System.ComponentModel.DataAnnotations;

namespace WingtipToys.Models;

public class Product
{
    [ScaffoldColumn(false)]
    public int ProductID { get; set; }

    [Required, StringLength(100), Display(Name = "Name")]
    public string ProductName { get; set; } = string.Empty;

    [Required, StringLength(10000), Display(Name = "Product Description"), DataType(DataType.MultilineText)]
    public string Description { get; set; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;

    [Display(Name = "Price")]
    public decimal? UnitPrice { get; set; }

    public int? CategoryID { get; set; }

    public virtual Category? Category { get; set; }
}

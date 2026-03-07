using System.ComponentModel.DataAnnotations;

namespace WingtipToys.Models;

public class CartItem
{
    [Key]
    public string ItemId { get; set; } = Guid.NewGuid().ToString();

    public string CartId { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public DateTime DateCreated { get; set; }

    public int ProductId { get; set; }

    public virtual Product? Product { get; set; }
}

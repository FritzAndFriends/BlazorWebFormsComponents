namespace WingtipToys.Models;

public class CartItem
{
    public string ItemId { get; set; } = Guid.NewGuid().ToString();
    public string CartId { get; set; } = "";
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public virtual Product? Product { get; set; }
}

using Microsoft.EntityFrameworkCore;

namespace WingtipToys.Models
{
  public class ProductContext : DbContext
  {
    public ProductContext()
    {
    }

    public ProductContext(DbContextOptions<ProductContext> options)
      : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        optionsBuilder.UseInMemoryDatabase("WingtipToys");
      }
    }

    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<CartItem> ShoppingCartItems { get; set; } = default!;
    public DbSet<Order> Orders { get; set; } = default!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = default!;
  }
}

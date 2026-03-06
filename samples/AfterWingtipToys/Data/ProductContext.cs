using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Data;

public class ProductContext : IdentityDbContext<IdentityUser>
{
    public ProductContext(DbContextOptions<ProductContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CartItem> ShoppingCartItems => Set<CartItem>();
}

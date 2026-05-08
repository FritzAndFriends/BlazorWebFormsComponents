using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WingtipToys.Models;

public class ApplicationUser : IdentityUser
{
}

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext()
        : this(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=wingtiptoys-auth.db")
            .Options)
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}

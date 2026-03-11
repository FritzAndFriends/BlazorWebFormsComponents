// TODO: Identity migration — replace OWIN/ASP.NET Identity with ASP.NET Core Identity
// For now, provide minimal stubs to compile. Full Identity migration is a separate task.

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WingtipToys.Models;

public class ApplicationUser : IdentityUser
{
}

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}

// TODO: IdentityHelper needs full rewrite for ASP.NET Core Identity
// Original used OWIN, HttpContext.Current, FormsAuthentication — all removed in ASP.NET Core


using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = null!;

    [CascadingParameter] private Task<AuthenticationState>? AuthState { get; set; }

    private List<Category>? _categories;
    private string? _userName;

    protected override async Task OnInitializedAsync()
    {
        using var db = DbFactory.CreateDbContext();
        _categories = await db.Categories.OrderBy(c => c.CategoryID).ToListAsync();

        if (AuthState != null)
        {
            var auth = await AuthState;
            if (auth.User.Identity?.IsAuthenticated == true)
            {
                _userName = auth.User.Identity.Name;
            }
        }
    }
}

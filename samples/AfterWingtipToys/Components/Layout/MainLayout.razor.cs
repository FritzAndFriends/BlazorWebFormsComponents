using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthStateTask { get; set; }

    private List<Category>? _categories;
    private string? _userName;

    protected override async Task OnInitializedAsync()
    {
        using var db = await DbFactory.CreateDbContextAsync();
        _categories = await db.Categories.OrderBy(c => c.CategoryName).ToListAsync();

        if (AuthStateTask != null)
        {
            var authState = await AuthStateTask;
            _userName = authState.User.Identity?.Name;
        }
    }
}

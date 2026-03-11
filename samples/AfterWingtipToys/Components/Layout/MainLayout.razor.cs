using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Components.Layout
{
    public partial class MainLayout
    {
        [CascadingParameter] private Task<AuthenticationState>? AuthStateTask { get; set; }

        private List<Category>? _categories;
        private int _cartCount;
        private string _userName = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            using var db = DbFactory.CreateDbContext();
            _categories = await db.Categories.ToListAsync();
            _cartCount = await db.ShoppingCartItems.CountAsync();

            if (AuthStateTask is not null)
            {
                var authState = await AuthStateTask;
                _userName = authState.User.Identity?.Name ?? string.Empty;
            }
        }
    }
}

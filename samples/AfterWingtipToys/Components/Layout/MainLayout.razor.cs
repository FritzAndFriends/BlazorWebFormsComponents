using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Data;
using WingtipToys.Models;

namespace WingtipToys.Components.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject]
        private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

        public List<Category> Categories { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            using var context = DbFactory.CreateDbContext();
            Categories = await context.Categories.OrderBy(c => c.CategoryID).ToListAsync();
        }
    }
}

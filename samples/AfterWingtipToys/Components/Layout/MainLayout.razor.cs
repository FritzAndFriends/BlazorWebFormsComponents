using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys.Components.Layout;

public partial class MainLayout
{
	[Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

	private List<Category> categories = new();

	protected override async Task OnInitializedAsync()
	{
		using var db = await DbFactory.CreateDbContextAsync();
		categories = await db.Categories.OrderBy(c => c.CategoryName).ToListAsync();
	}
}

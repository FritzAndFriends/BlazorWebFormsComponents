using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Logic;
using WingtipToys.Models;

namespace WingtipToys;

public partial class Site
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;
    [Inject] private CartStore CartStore { get; set; } = default!;
    [Inject] private IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    private IReadOnlyList<Category> Categories { get; set; } = [];

    private string CartLabel => $"Cart ({CartStore.Count(GetOrCreateCartId())})";

    private string? CurrentUserName => HttpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true
        ? HttpContextAccessor.HttpContext.User.Identity?.Name
        : null;

    private bool IsAuthenticated => !string.IsNullOrWhiteSpace(CurrentUserName);

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        await using var db = await DbFactory.CreateDbContextAsync();
        Categories = await db.Categories.OrderBy(category => category.CategoryName).ToListAsync();
    }

    private string GetOrCreateCartId()
    {
        var context = HttpContextAccessor.HttpContext;
        if (context is null)
        {
            return "anonymous-cart";
        }

        if (context.Request.Cookies.TryGetValue("CartSessionId", out var existing) && !string.IsNullOrWhiteSpace(existing))
        {
            return existing;
        }

        var cartId = Guid.NewGuid().ToString("N");
        context.Response.Cookies.Append("CartSessionId", cartId, new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Lax,
            Secure = context.Request.IsHttps
        });

        return cartId;
    }
}

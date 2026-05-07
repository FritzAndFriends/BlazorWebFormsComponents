using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys;

public partial class Site : WebFormsPageBase
{
    [Inject] public ProductCatalogService Catalog { get; set; } = default!;
    [Inject] public CartStore CartStore { get; set; } = default!;
    [Inject] public IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public RenderFragment? ChildComponents { get; set; }

    protected IReadOnlyList<Category> Categories { get; private set; } = [];
    protected string? CurrentUserEmail => Session["auth-email"]?.ToString() ?? HttpContextAccessor.HttpContext?.Session.GetString("auth-email");
    protected string CartText => $"Cart ({CartStore.GetCount(GetOrCreateCartKey())})";

    protected override void OnInitialized()
    {
        Categories = Catalog.GetCategories();
        base.OnInitialized();
    }

    protected string GetOrCreateCartKey()
    {
        var cartKey = Session["cart-key"]?.ToString() ?? HttpContextAccessor.HttpContext?.Session.GetString("cart-key");
        if (string.IsNullOrWhiteSpace(cartKey))
        {
            cartKey = Guid.NewGuid().ToString();
            Session["cart-key"] = cartKey;
            HttpContextAccessor.HttpContext?.Session.SetString("cart-key", cartKey);
        }

        return cartKey;
    }
}

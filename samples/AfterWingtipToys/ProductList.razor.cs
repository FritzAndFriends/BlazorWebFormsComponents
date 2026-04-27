using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys;

public partial class ProductList
{
    [Inject] private CatalogService Catalog { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private IReadOnlyList<Product> Products { get; set; } = Array.Empty<Product>();
    private string Heading { get; set; } = "Products";

    protected override void OnParametersSet()
    {
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        int? categoryId = null;
        if (query.TryGetValue("id", out var idValue) && int.TryParse(idValue, out var parsedId))
        {
            categoryId = parsedId;
        }

        var categoryName = query.TryGetValue("category", out var categoryValue)
            ? categoryValue.ToString()
            : null;

        Products = Catalog.GetProducts(categoryId, categoryName);
        Heading = string.IsNullOrWhiteSpace(categoryName)
            ? "Products"
            : $"{categoryName} Products";
    }
}

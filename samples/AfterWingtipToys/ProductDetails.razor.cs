using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using WingtipToys.Models;
using WingtipToys.Services;

namespace WingtipToys;

public partial class ProductDetails
{
    [Inject] private CatalogService Catalog { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private Product? Product { get; set; }

    protected override void OnParametersSet()
    {
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);

        int? productId = null;
        if (query.TryGetValue("productID", out var idValue) && int.TryParse(idValue, out var parsedId))
        {
            productId = parsedId;
        }

        var productName = query.TryGetValue("productName", out var nameValue)
            ? nameValue.ToString()
            : null;

        Product = Catalog.GetProduct(productId, productName);
    }
}

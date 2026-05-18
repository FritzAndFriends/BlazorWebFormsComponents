using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Models;

namespace WingtipToys;

public partial class Default
{
    [Inject] private IDbContextFactory<ProductContext> DbFactory { get; set; } = default!;

    private IReadOnlyList<Product> FeaturedProducts { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Title = "Wingtip Toys";

        await using var db = await DbFactory.CreateDbContextAsync();
        FeaturedProducts = await db.Products.OrderBy(product => product.ProductID).Take(4).ToListAsync();
    }

    private void Page_Error(EventArgs e)
    {
      // Get last error from the server.
      Exception exc = Server.GetLastError();

      // Handle specific exception.
      if (exc is InvalidOperationException)
      {
        // Pass the error on to the error page.
        Server.Transfer("ErrorPage.aspx?handler=Page_Error%20-%20Default.aspx",
            true);
      }
    }
  }
}
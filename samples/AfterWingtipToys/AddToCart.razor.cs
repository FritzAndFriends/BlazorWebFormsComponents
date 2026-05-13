using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using WingtipToys.Logic;
using Microsoft.AspNetCore.Components;

namespace WingtipToys
{
  public partial class AddToCart
  {
    [Inject] protected ShoppingCartActions _cartActions { get; set; } = default!;

    private WebFormsForm form1 = default!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

      string rawId = Request.QueryString["ProductID"];
      int productId;
      if (!String.IsNullOrEmpty(rawId) && int.TryParse(rawId, out productId))
      {
        _cartActions.AddToCart(Convert.ToInt16(rawId));
      }
      else
      {
        Debug.Fail("ERROR : We should never get to AddToCart.aspx without a ProductId.");
        throw new Exception("ERROR : It is illegal to load AddToCart.aspx without setting a ProductId.");
      }
      Response.Redirect("ShoppingCart.aspx");
    }
  }
}
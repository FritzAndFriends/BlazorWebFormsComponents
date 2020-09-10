using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WingtipToys.Lib;

namespace WingtipToys.Logic
{
  public class ShoppingCartAccess : ICartStateAccess
  {

    internal const string CartSessionKey = "CartId";

    public string CartId
    {
      get {
        if (HttpContext.Current.Session[CartSessionKey] == null)
        {
          if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
          {
            CartId = HttpContext.Current.User.Identity.Name;
          }
          else
          {
            // Generate a new random GUID using System.Guid class.     
            Guid tempCartId = Guid.NewGuid();
            CartId = tempCartId.ToString();
          }
        }
        return HttpContext.Current.Session[CartSessionKey].ToString();
      }
      set {
        HttpContext.Current.Session[CartSessionKey] = value;
      }
    }
  }
}

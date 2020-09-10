using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace WingtipToys.Lib
{

  public interface ICartStateAccess
  {

    /// <summary>
    /// Interact with the ID for the current shopping cart
    /// </summary>
    string CartId { get; set; }

  }

}

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{

  public partial class BasePage : ComponentBase {

    public BasePage(IJSRuntime jsInterop)
    {
        
      this.JsInterop = jsInterop;

    }

    public IJSRuntime JsInterop { get; }

  }
    
}
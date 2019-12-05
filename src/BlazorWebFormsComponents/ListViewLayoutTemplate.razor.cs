using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{

  public partial class ListViewLayoutTemplate<TItem>
  {

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public ListView<TItem> ListView { get; set; }


    void Foo()
    {

      ChildContent.Invoke(null);

    }


  }

}

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents
{

  public partial class ListView<TItem>
  {

    [Parameter]
    public IEnumerable<TItem> Items { get; set; }

    [Parameter]
    public RenderFragment EmptyDataTemplate { get; set; }

    [Parameter]
    public RenderFragment TableHeader { get; set; }

    [Parameter]
    public RenderFragment<TItem> RowTemplate { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> AdditionalAttributes { get; set; }
  }

}

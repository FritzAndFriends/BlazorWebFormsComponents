using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{

	public partial class ListView<ItemType> : BaseModelBindingComponent<ItemType>
  {

    public ListView()
    {
    }

		#region Templates

		[Parameter]
    public RenderFragment<ItemType> AlternatingItemTemplate { get; set; }

    /// <summary>
    /// Defines the content to render if the data source returns no data.
    /// </summary>
    [Parameter]
    public RenderFragment EmptyDataTemplate { get; set; }

    [Parameter]
    public RenderFragment ItemSeparatorTemplate { get; set; }

    [Parameter]
    public RenderFragment<ItemType> ItemTemplate { get; set; }

    /// <summary>
    /// 🚨🚨 LayoutTemplate is not available.  Please wrap the ListView component with the desired layout 🚨🚨
    /// </summary>
    [Parameter]
    [Obsolete("The LayoutTemplate child element is not supported in Blazor.  Instead, wrap the ListView component with the desired layout")]
    public RenderFragment LayoutTemplate { get; set; }

		#endregion

		[Parameter] // TODO: Implement
    public InsertItemPosition InsertItemPosition { get; set; }

    [Parameter] // TODO: Implement
    public int SelectedIndex { get; set; }

    /// <summary>
    /// Style is not applied by this control
    /// </summary>
    [Parameter, Obsolete("Style is not applied by this control")]
    public string Style { get; set; }

	}

}

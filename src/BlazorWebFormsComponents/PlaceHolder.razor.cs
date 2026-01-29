using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents;

public partial class PlaceHolder : BaseWebFormsComponent
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }
}

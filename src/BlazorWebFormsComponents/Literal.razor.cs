using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents;

public partial class Literal : BaseWebFormsComponent
{
    [Parameter]
    public string Text { get; set; } = string.Empty;

    [Parameter]
    public LiteralMode Mode { get; set; } = LiteralMode.Encode; // Changed from PassThrough to Encode to match Web Forms default
}

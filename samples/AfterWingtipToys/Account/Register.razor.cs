using Microsoft.AspNetCore.Components;

namespace WingtipToys.Account;

public partial class Register : BlazorWebFormsComponents.WebFormsPageBase
{
    [SupplyParameterFromQuery(Name = "error")]
    public string? ErrorParam { get; set; }

    private string? errorMessage;

    protected override void OnParametersSet()
    {
        errorMessage = ErrorParam;
    }
}

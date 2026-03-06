using Microsoft.AspNetCore.Components;

namespace WingtipToys.Account
{
    public partial class Register : ComponentBase
    {
        [SupplyParameterFromQuery(Name = "error")]
        public string? Error { get; set; }
    }
}

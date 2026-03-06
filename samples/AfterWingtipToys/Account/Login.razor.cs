using Microsoft.AspNetCore.Components;

namespace WingtipToys.Account
{
    public partial class Login : ComponentBase
    {
        [SupplyParameterFromQuery(Name = "error")]
        public string? Error { get; set; }
    }
}

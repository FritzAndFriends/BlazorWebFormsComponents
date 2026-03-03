using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace WingtipToys.Account
{
    public partial class Manage : ComponentBase
    {
        [SupplyParameterFromQuery] private string? StatusMessage { get; set; }

        private string statusMessage => StatusMessage ?? "";
    }
}

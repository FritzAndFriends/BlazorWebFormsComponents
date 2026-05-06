using System.Collections.Generic;
using BlazorWebFormsComponents;
using Microsoft.AspNetCore.Components;

namespace WingtipToys.Account
{
    public partial class OpenAuthProviders : BlazorWebFormsComponents.WebFormsPageBase
    {
        [Parameter] public string? ReturnUrl { get; set; }

        private ListView<string> providerDetails = default!;

        public IEnumerable<string> GetProviderNames() => [];
    }
}

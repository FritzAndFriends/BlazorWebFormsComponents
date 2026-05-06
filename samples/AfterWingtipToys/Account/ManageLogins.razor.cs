using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;

namespace WingtipToys.Account
{
    public partial class ManageLogins : BlazorWebFormsComponents.WebFormsPageBase
    {
        private PlaceHolder successMessage = default!;
        private string? SuccessMessage { get; set; }
        private bool CanRemoveExternalLogins => true;

        public IEnumerable<UserLoginInfo> GetLogins() => [];

        public void RemoveLogin()
        {
        }
    }
}

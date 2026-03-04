using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Account
{
    public partial class Login : ComponentBase
    {
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "error")]
        public string? ErrorParam { get; set; }

        private string email = "";
        private string password = "";
        private string errorMessage = "";

        protected override void OnParametersSet()
        {
            if (!string.IsNullOrEmpty(ErrorParam))
            {
                errorMessage = ErrorParam;
            }
        }

        private void HandleLogin(MouseEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Email and password are required.";
                return;
            }

            // Navigate to HTTP endpoint so SignInManager can set cookies via the HTTP response
            var loginUrl = $"/Account/PerformLogin?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";
            Navigation.NavigateTo(loginUrl, forceLoad: true);
        }
    }
}

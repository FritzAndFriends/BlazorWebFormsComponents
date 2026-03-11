using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace WingtipToys.Account
{
    public partial class Confirm
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>

        protected string StatusMessage { get; private set; } = string.Empty;

        private bool showSuccess;
        private bool showError;

        protected override async Task OnInitializedAsync()
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original Page_Load code:
            // string code = IdentityHelper.GetCodeFromRequest(Request);
            // string userId = IdentityHelper.GetUserIdFromRequest(Request);
            // if (code != null && userId != null)
            // {
            //     var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //     var result = manager.ConfirmEmail(userId, code);
            //     if (result.Succeeded)
            //     {
            //         showSuccess = true;
            //         return;
            //     }
            // }
            // showSuccess = false;
            // showError = true;

            await Task.CompletedTask;
        }
    }
}

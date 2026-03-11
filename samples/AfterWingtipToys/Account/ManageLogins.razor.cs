using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace WingtipToys.Account
{
    public partial class ManageLogins
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>

        protected string SuccessMessage { get; private set; } = string.Empty;

        protected bool CanRemoveExternalLogins { get; private set; }

        // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
        // private bool HasPassword(UserManager<ApplicationUser> manager)
        // {
        //     return manager.HasPasswordAsync(userId).Result;
        // }

        protected override async Task OnInitializedAsync()
        {
            // TODO: Migrate Identity logic from Web Forms OWIN to ASP.NET Core Identity
            // Original Page_Load code:
            // var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            // CanRemoveExternalLogins = manager.GetLogins(User.Identity.GetUserId()).Count() > 1;
            // SuccessMessage = String.Empty;

            await Task.CompletedTask;
        }

        // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
        // public IEnumerable<UserLoginInfo> GetLogins()
        // {
        //     var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //     var accounts = manager.GetLogins(User.Identity.GetUserId());
        //     CanRemoveExternalLogins = accounts.Count() > 1 || HasPassword(manager);
        //     return accounts;
        // }

        // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
        // public void RemoveLogin(string loginProvider, string providerKey)
        // {
        //     var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //     var result = manager.RemoveLogin(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        //     string msg = String.Empty;
        //     if (result.Succeeded)
        //     {
        //         var user = manager.FindById(User.Identity.GetUserId());
        //         IdentityHelper.SignIn(manager, user, isPersistent: false);
        //         msg = "?m=RemoveLoginSuccess";
        //     }
        //     NavigationManager.NavigateTo("/Account/ManageLogins" + msg);
        // }
    }
}

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Account
{
    public partial class VerifyPhoneNumber
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>

        protected override async Task OnInitializedAsync()
        {
            // TODO: Migrate Identity logic from Web Forms OWIN to ASP.NET Core Identity
            // Original Page_Load code:
            // var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            // var phonenumber = Request.QueryString["PhoneNumber"];
            // var code = manager.GenerateChangePhoneNumberToken(User.Identity.GetUserId(), phonenumber);
            // PhoneNumber.Value = phonenumber;

            await Task.CompletedTask;
        }

        private void Code_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // if (!ModelState.IsValid)
            // {
            //     ModelState.AddModelError("", "Invalid code");
            //     return;
            // }
            // var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            // var result = manager.ChangePhoneNumber(User.Identity.GetUserId(), PhoneNumber.Value, Code.Text);
            // if (result.Succeeded)
            // {
            //     var user = manager.FindById(User.Identity.GetUserId());
            //     if (user != null)
            //     {
            //         IdentityHelper.SignIn(manager, user, false);
            //         NavigationManager.NavigateTo("/Account/Manage?m=AddPhoneNumberSuccess");
            //     }
            // }
            // ModelState.AddModelError("", "Failed to verify phone");
        }
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Account
{
    public partial class AddPhoneNumber
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>

        private void PhoneNumber_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            // var code = manager.GenerateChangePhoneNumberToken(User.Identity.GetUserId(), PhoneNumber.Text);
            // if (manager.SmsService != null)
            // {
            //     var message = new IdentityMessage
            //     {
            //         Destination = PhoneNumber.Text,
            //         Body = "Your security code is " + code
            //     };
            //     manager.SmsService.Send(message);
            // }

            // TODO: URL-encode PhoneNumber.Text for the query string
            NavigationManager.NavigateTo("/Account/VerifyPhoneNumber?PhoneNumber=");
        }
    }
}

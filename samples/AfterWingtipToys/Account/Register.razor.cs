using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Account
{
    public partial class Register
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>

        private void CreateUser_Click(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            // var user = new ApplicationUser() { UserName = Email.Text, Email = Email.Text };
            // IdentityResult result = manager.Create(user, Password.Text);
            // if (result.Succeeded)
            // {
            //     // Email confirmation code (commented in original):
            //     // string code = manager.GenerateEmailConfirmationToken(user.Id);
            //     // string callbackUrl = IdentityHelper.GetUserConfirmationRedirectUrl(code, user.Id, Request);
            //     // manager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.");
            //
            //     IdentityHelper.SignIn(manager, user, isPersistent: false);
            //
            //     // Shopping cart migration
            //     using (WingtipToys.Logic.ShoppingCartActions usersShoppingCart = new WingtipToys.Logic.ShoppingCartActions())
            //     {
            //         String cartId = usersShoppingCart.GetCartId();
            //         usersShoppingCart.MigrateCart(cartId, user.Id);
            //     }
            //
            //     IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            // }
            // else
            // {
            //     ErrorMessage.Text = result.Errors.FirstOrDefault();
            // }
        }
    }
}

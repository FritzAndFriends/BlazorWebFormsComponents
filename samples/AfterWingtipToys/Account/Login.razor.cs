using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Account
{
    public partial class Login
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>

        protected override async Task OnInitializedAsync()
        {
            // TODO: Migrate Identity logic from Web Forms OWIN to ASP.NET Core Identity
            // Original Page_Load code:
            // RegisterHyperLink.NavigateUrl = "Register";
            // OpenAuthLogin.ReturnUrl = Request.QueryString["ReturnUrl"];
            // var returnUrl = HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            // if (!String.IsNullOrEmpty(returnUrl))
            // {
            //     RegisterHyperLink.NavigateUrl += "?ReturnUrl=" + returnUrl;
            // }

            await Task.CompletedTask;
        }

        private void LogIn(MouseEventArgs e)
        {
            // TODO: ASP.NET Core Identity — requires UserManager<ApplicationUser>/SignInManager<ApplicationUser> migration
            // Original logic:
            // if (IsValid)
            // {
            //     var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            //     var signinManager = Context.GetOwinContext().GetUserManager<ApplicationSignInManager>();
            //     var result = signinManager.PasswordSignIn(Email.Text, Password.Text, RememberMe.Checked, shouldLockout: false);
            //     switch (result)
            //     {
            //         case SignInStatus.Success:
            //             WingtipToys.Logic.ShoppingCartActions usersShoppingCart = new WingtipToys.Logic.ShoppingCartActions();
            //             String cartId = usersShoppingCart.GetCartId();
            //             usersShoppingCart.MigrateCart(cartId, Email.Text);
            //             IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
            //             break;
            //         case SignInStatus.LockedOut:
            //             NavigationManager.NavigateTo("/Account/Lockout");
            //             break;
            //         case SignInStatus.RequiresVerification:
            //             NavigationManager.NavigateTo("/Account/TwoFactorAuthenticationSignIn?ReturnUrl=&RememberMe=");
            //             break;
            //         case SignInStatus.Failure:
            //         default:
            //             // FailureText.Text = "Invalid login attempt";
            //             break;
            //     }
            // }
        }
    }
}

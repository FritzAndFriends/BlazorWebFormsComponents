using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace WingtipToys.Account
{
    public partial class OpenAuthProviders
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        // TODO: ASP.NET Core Identity — inject SignInManager<ApplicationUser> and UserManager<ApplicationUser>

        [Parameter] public string ReturnUrl { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            // TODO: Migrate Identity logic from Web Forms OWIN to ASP.NET Core Identity
            // Original Page_Load handled PostBack for external auth provider challenge:
            // var provider = Request.Form["provider"];
            // string redirectUrl = ResolveUrl(String.Format(CultureInfo.InvariantCulture,
            //     "/Account/RegisterExternalLogin?{0}={1}&returnUrl={2}", IdentityHelper.ProviderNameKey, provider, ReturnUrl));
            // var properties = new AuthenticationProperties() { RedirectUri = redirectUrl };
            // if (Context.User.Identity.IsAuthenticated)
            //     properties.Dictionary[IdentityHelper.XsrfKey] = Context.User.Identity.GetUserId();
            // Context.GetOwinContext().Authentication.Challenge(properties, provider);

            await Task.CompletedTask;
        }

        // TODO: ASP.NET Core Identity — requires external authentication provider enumeration
        // public IEnumerable<string> GetProviderNames()
        // {
        //     return Context.GetOwinContext().Authentication.GetExternalAuthenticationTypes().Select(t => t.AuthenticationType);
        // }
    }
}

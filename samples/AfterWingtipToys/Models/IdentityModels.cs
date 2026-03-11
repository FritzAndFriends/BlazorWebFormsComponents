// TODO: ASP.NET Core Identity migration — this file needs complete Identity subsystem replacement.
// The original Web Forms OWIN-based Identity code cannot be directly ported.
// Use `dotnet aspnet-codegenerator identity` to scaffold ASP.NET Core Identity pages.

using Microsoft.AspNetCore.Identity;

namespace WingtipToys.Models
{
    // ApplicationUser extends IdentityUser for ASP.NET Core Identity
    public class ApplicationUser : IdentityUser
    {
        // Add custom user properties here if needed
    }
}

namespace WingtipToys
{
    // TODO: IdentityHelper needs full rewrite for ASP.NET Core Identity
    // SignInManager<ApplicationUser> and UserManager<ApplicationUser> replace OWIN managers.
    // NavigationManager replaces Response.Redirect patterns.
    public static class IdentityHelper
    {
        public const string XsrfKey = "XsrfId";
        public const string ProviderNameKey = "providerName";
        public const string CodeKey = "code";
        public const string UserIdKey = "userId";

        public static bool IsLocalUrl(string? url)
        {
            return !string.IsNullOrEmpty(url)
                && ((url[0] == '/' && (url.Length == 1 || (url[1] != '/' && url[1] != '\\')))
                    || (url.Length > 1 && url[0] == '~' && url[1] == '/'));
        }
    }
}


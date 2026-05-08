// TODO: Review — auto-copied from App_Start. Blazor has no App_Start convention.
// TODO: Move relevant configuration to Program.cs or appropriate service registration.

using System;
using System.Collections.Generic;
// // BWFC: RouteConfig stubs available via BlazorWebFormsComponents namespace
namespace WingtipToys
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Permanent;
            routes.EnableFriendlyUrls(settings);
        }
    }
}

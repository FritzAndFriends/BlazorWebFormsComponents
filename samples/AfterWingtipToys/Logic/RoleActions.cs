using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WingtipToys.Models;

namespace WingtipToys.Logic
{
    internal class RoleActions
    {
        private readonly RoleManager<IdentityRole> _roleMgr;
        private readonly UserManager<ApplicationUser> _userMgr;

        internal RoleActions(RoleManager<IdentityRole> roleMgr, UserManager<ApplicationUser> userMgr)
        {
            _roleMgr = roleMgr;
            _userMgr = userMgr;
        }

        internal async Task AddUserAndRoleAsync()
        {
            if (!await _roleMgr.RoleExistsAsync("canEdit"))
            {
                await _roleMgr.CreateAsync(new IdentityRole { Name = "canEdit" });
            }

            var appUser = new ApplicationUser
            {
                UserName = "canEditUser@wingtiptoys.com",
                Email = "canEditUser@wingtiptoys.com"
            };

            if (await _userMgr.FindByEmailAsync(appUser.Email) == null)
            {
                await _userMgr.CreateAsync(appUser, "Pa$$word1");
            }

            var user = await _userMgr.FindByEmailAsync(appUser.Email);
            if (user != null && !await _userMgr.IsInRoleAsync(user, "canEdit"))
            {
                await _userMgr.AddToRoleAsync(user, "canEdit");
            }
        }
    }
}

using Microsoft.AspNetCore.Identity;

namespace WingtipToys.Data
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            const string adminRole = "canEdit";
            const string adminEmail = "admin@wingtiptoys.com";
            const string adminPassword = "Admin123!";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, adminPassword);
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}

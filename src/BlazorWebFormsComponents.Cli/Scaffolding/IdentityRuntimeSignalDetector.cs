using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

public partial class IdentityRuntimeSignalDetector : IRuntimeSignalDetector
{
    private static readonly string[] IdentityPageNames =
    [
        "Login.aspx",
        "Register.aspx",
        "Manage.aspx",
        "ForgotPassword.aspx",
        "ResetPassword.aspx",
        "Startup.Auth.cs",
        "IdentityConfig.cs"
    ];

    public void Apply(string sourcePath, RuntimeProfile profile)
    {
        if (Directory.Exists(Path.Combine(sourcePath, "Account")))
        {
            profile.NeedsIdentity = true;
        }

        foreach (var file in RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".cs", ".aspx", ".config"))
        {
            var fileName = Path.GetFileName(file);
            if (IdentityPageNames.Contains(fileName, StringComparer.OrdinalIgnoreCase))
            {
                profile.NeedsIdentity = true;
            }

            var content = File.ReadAllText(file);
            if (IdentityUsageRegex().IsMatch(content))
            {
                profile.NeedsIdentity = true;
            }

            // Detect seed roles: roleMgr.Create(new IdentityRole { Name = "canEdit" })
            foreach (Match m in RoleCreatePropertyRegex().Matches(content))
            {
                var roleName = m.Groups[1].Value;
                if (!string.IsNullOrEmpty(roleName) && !profile.DetectedRoleNames.Contains(roleName, StringComparer.OrdinalIgnoreCase))
                    profile.DetectedRoleNames.Add(roleName);
            }
            foreach (Match m in RoleCreateConstructorRegex().Matches(content))
            {
                var roleName = m.Groups[1].Value;
                if (!string.IsNullOrEmpty(roleName) && !profile.DetectedRoleNames.Contains(roleName, StringComparer.OrdinalIgnoreCase))
                    profile.DetectedRoleNames.Add(roleName);
            }

            // Detect seed users: UserName = "user@domain.com"  +  userMgr.Create(appUser, "Pa$$word1")
            foreach (Match m in SeedUserRegex().Matches(content))
            {
                var email = m.Groups[1].Value;
                // Try to find the password on a nearby Create call
                var passwordMatch = UserCreatePasswordRegex().Match(content);
                var password = passwordMatch.Success ? passwordMatch.Groups[1].Value : "P@ssw0rd1";

                // Try to find which role this user is added to
                var roleAssign = UserRoleAssignRegex().Match(content);
                var roleName = roleAssign.Success ? roleAssign.Groups[1].Value :
                    (profile.DetectedRoleNames.Count > 0 ? profile.DetectedRoleNames[0] : "");

                if (!profile.DetectedSeedUsers.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                    profile.DetectedSeedUsers.Add((email, password, roleName));
            }
        }
    }

    [GeneratedRegex(@"\b(SignInManager|UserManager|RoleManager|IdentityUser|IdentityRole|AspNetUsers|ApplicationUserManager|ApplicationSignInManager|DefaultAuthenticationTypes)\b", RegexOptions.Multiline)]
    private static partial Regex IdentityUsageRegex();

    // Matches: new IdentityRole { Name = "roleName" }
    [GeneratedRegex(@"new\s+IdentityRole\s*\{\s*Name\s*=\s*""([^""]+)""", RegexOptions.Multiline)]
    private static partial Regex RoleCreatePropertyRegex();

    // Matches: new IdentityRole("roleName")
    [GeneratedRegex(@"new\s+IdentityRole\s*\(\s*""([^""]+)""\s*\)", RegexOptions.Multiline)]
    private static partial Regex RoleCreateConstructorRegex();

    // Matches: UserName = "email@domain.com"
    [GeneratedRegex(@"UserName\s*=\s*""([^""]+@[^""]+)""", RegexOptions.Multiline)]
    private static partial Regex SeedUserRegex();

    // Matches: userMgr.Create(var, "password") or CreateAsync(var, "password")
    [GeneratedRegex(@"\.Create(?:Async)?\s*\(\s*\w+\s*,\s*""([^""]+)""", RegexOptions.Multiline)]
    private static partial Regex UserCreatePasswordRegex();

    // Matches: AddToRole(id, "roleName") or AddToRoleAsync(user, "roleName")
    [GeneratedRegex(@"\.AddToRole(?:Async)?\s*\([^,]+,\s*""([^""]+)""", RegexOptions.Multiline)]
    private static partial Regex UserRoleAssignRegex();
}

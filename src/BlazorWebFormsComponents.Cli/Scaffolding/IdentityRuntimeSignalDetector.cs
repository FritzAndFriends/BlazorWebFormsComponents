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
            return;
        }

        foreach (var file in RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".cs", ".aspx", ".config"))
        {
            var fileName = Path.GetFileName(file);
            if (IdentityPageNames.Contains(fileName, StringComparer.OrdinalIgnoreCase))
            {
                profile.NeedsIdentity = true;
                return;
            }

            var content = File.ReadAllText(file);
            if (IdentityUsageRegex().IsMatch(content))
            {
                profile.NeedsIdentity = true;
                return;
            }
        }
    }

    [GeneratedRegex(@"\b(SignInManager|UserManager|IdentityUser|AspNetUsers|ApplicationUserManager|ApplicationSignInManager|DefaultAuthenticationTypes)\b", RegexOptions.Multiline)]
    private static partial Regex IdentityUsageRegex();
}

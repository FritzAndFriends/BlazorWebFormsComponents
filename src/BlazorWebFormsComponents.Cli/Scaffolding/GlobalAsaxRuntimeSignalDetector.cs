using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

public partial class GlobalAsaxRuntimeSignalDetector : IRuntimeSignalDetector
{
    public void Apply(string sourcePath, RuntimeProfile profile)
    {
        foreach (var file in RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".asax", ".cs"))
        {
            var fileName = Path.GetFileName(file);
            if (!fileName.Equals("Global.asax", StringComparison.OrdinalIgnoreCase) &&
                !fileName.Equals("Global.asax.cs", StringComparison.OrdinalIgnoreCase) &&
                !fileName.EndsWith(".Auth.cs", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var content = File.ReadAllText(file);
            AddMatches(ApplicationStartRegex(), content, profile.ApplicationStartPatterns);

            if (AuthStartupRegex().IsMatch(content))
                profile.NeedsIdentity = true;

            if (SessionStartupRegex().IsMatch(content))
                profile.NeedsSession = true;
        }
    }

    private static void AddMatches(Regex regex, string content, ICollection<string> matches)
    {
        foreach (Match match in regex.Matches(content))
        {
            matches.Add(match.Groups["pattern"].Value);
        }
    }

    [GeneratedRegex(@"(?<pattern>RouteConfig\.RegisterRoutes|BundleConfig\.RegisterBundles|FilterConfig\.RegisterGlobalFilters|AuthConfig\.RegisterOpenAuth|Startup\.ConfigureAuth|Database\.SetInitializer|Application_Error)", RegexOptions.Multiline)]
    private static partial Regex ApplicationStartRegex();

    [GeneratedRegex(@"\b(AuthConfig\.RegisterOpenAuth|Startup\.ConfigureAuth|ConfigureAuth\s*\()", RegexOptions.Multiline)]
    private static partial Regex AuthStartupRegex();

    [GeneratedRegex(@"\b(Session_Start\s*\(|Application_AcquireRequestState\s*\()", RegexOptions.Multiline)]
    private static partial Regex SessionStartupRegex();
}
